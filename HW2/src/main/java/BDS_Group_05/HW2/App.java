package BDS_Group_05.HW2;

import java.util.Properties;

import org.apache.flink.api.java.tuple.Tuple;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.api.java.tuple.Tuple4;
import org.apache.flink.api.java.tuple.Tuple5;
import org.apache.flink.api.java.tuple.Tuple6;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.streaming.api.CheckpointingMode;
import org.apache.flink.streaming.api.TimeCharacteristic;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.util.Collector;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.windowing.WindowFunction;
import org.apache.flink.streaming.api.windowing.assigners.SlidingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.assigners.TumblingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.assigners.TumblingProcessingTimeWindows;

public class App 
{
	public static void main( String[] args )
    {
		Time window_length = Time.seconds(15);
		Time window_slide = Time.seconds(5);
    	StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
    	final String GPS_file = "/Users/lfc746/Desktop/BDS-2019-2020/HW2/testFile/GPS_test"; 
    	final String Tag_file = "/Users/lfc746/Desktop/BDS-2019-2020/HW2/testFile/Tag_test"; 
    	final String Photo_file = "/Users/lfc746/Desktop/BDS-2019-2020/HW2/testFile/Photo_test"; 
    	
    	Properties properties = new Properties();
        properties.setProperty("bootstrap.servers", "localhost:9092");
        properties.setProperty("zookeeper.connect", "localhost:2181");
        properties.setProperty("group.id", "HW2");   // a group of consumers called "HW2"
        
        /**************************** CHECKPOINT *********************************/
        //env.enableCheckpointing(10); // start a checkpoint every 10 ms
        env.enableCheckpointing(10000); // start a checkpoint every 10 s
        env.getCheckpointConfig().setCheckpointingMode(CheckpointingMode.EXACTLY_ONCE);
        //env.setStateBackend(new FsStateBackend("hdfs://localhost:9000/user/krimmity/checkpoint"));   // ??????
        
        /**************************** TIMESTAMP & WATERMARK *********************************/
        env.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);
        
        /**************************** CONSUMER *********************************/
        FlinkKafkaConsumer<String> GPS_consumer = new FlinkKafkaConsumer<String>("GPS", new SimpleStringSchema(), properties);
        FlinkKafkaConsumer<String> Tag_consumer = new FlinkKafkaConsumer<String>("Tag", new SimpleStringSchema(), properties);
        FlinkKafkaConsumer<String> Photo_consumer = new FlinkKafkaConsumer<String>("Photo", new SimpleStringSchema(), properties);
        
        /**************************** READ DATA FROM KAFKA *********************************/
        // Photo: <photo_id, user_id, lat, lon, timestamp>
      //  DataStream<Tuple5<Integer, Integer, Float, Float, Long>> Photo = env.addSource(Photo_consumer)
        DataStream<Tuple5<Integer, Integer, Float, Float, Long>> Photo = env.readTextFile(Photo_file)
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_Photo(input))
        		// reference: https://www.codota.com/code/java/methods/org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator/returns
        		.returns(new TypeHint<Tuple5<Integer, Integer, Float, Float, Long>>(){});
        
        // Tag: <photo_id, user_id>
      //  DataStream<Tuple3<Integer, Integer, Long>> Tag = env.addSource(Tag_consumer)
        DataStream<Tuple3<Integer, Integer, Long>> Tag = env.readTextFile(Tag_file)
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_Tag(input))
        		.returns(new TypeHint<Tuple3<Integer, Integer, Long>>(){});
        
        // GPS: <user_id, lat, lon, timestamp>
     //   DataStream<String> preGPS = env.addSource(GPS_consumer)
        DataStream<String> preGPS = env.readTextFile(GPS_file)
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner());
        
        // Windowed_GPS: GPS join itself
		DataStream<Tuple4<Integer, Float, Float, Long>> windowed_GPS =preGPS.join(preGPS)
        		.where(new SelectString())
        		.equalTo(new SelectString())
        		.window(SlidingEventTimeWindows.of(window_length, window_slide))
        		.apply(new JoinTwoGPS())
        		.map(input -> parser_GPS(input))
        		.returns(new TypeHint<Tuple4<Integer, Float, Float, Long>>(){});
        	/*	.keyBy(0)
        		.window(TumblingEventTimeWindows.of(window_slide))
        		.apply(new WindowFunction<Tuple4<Integer, Float, Float, Long>, Tuple4<Integer, Float, Float, Long>, Tuple, TimeWindow>() {

					private static final long serialVersionUID = 1L;

					@Override
        		    public void apply(Tuple tuple, TimeWindow window, Iterable<Tuple4<Integer, Float, Float, Long>> input, Collector<Tuple4<Integer, Float, Float, Long>> out) throws Exception {
        		        out.collect(input.iterator().next());
        		    }
        		});*/
		
        // join on photo_id, to get where each photo was posted, <photo_id, user_id, lat, lon>
        // reference (window join): https://ci.apache.org/projects/flink/flink-docs-stable/dev/stream/operators/joining.html
		DataStream<Tuple5<Integer, Integer, Float, Float, Long>> windowed_join_Tag_Photo = Tag.join(Photo)
				.where(new SelectKeyFromTag())     // select photo_id from Tag 
	            .equalTo(new SelectKeyFromPhoto()) // select photo_id from Photo
	            .window(SlidingEventTimeWindows.of(window_length, window_slide))
        		.apply(new JoinTagPhoto());        // the join function

		// join on user_id, to get where the user is tracked, <photo_id, user_id, lat1, lon1, lat2, lon2>
		// lat1, lon1: where the photo was posted
		// lat2, lon2: where the user was tracked
      //  DataStream<String> join_GPS =
        		windowed_join_Tag_Photo.join(windowed_GPS)
        		.where(new SelectKey())            // select user_id from join_Tag_Photo
        		.equalTo(new SelectKeyFromGPS())   // select user_id from GPS
        		.window(TumblingEventTimeWindows.of(window_slide))
        		.apply(new JoinGPS())             // join on user_id, the result is <photo_id, user_id, lat1, lon1, lat2, lon2>
           		.filter(new Myfilter())            // filter out the location with distance > 5km
        		.map(input -> new Tuple2<Integer, Integer>(input.f1, input.f0))  // user_id, photo_id
        		.returns(new TypeHint<Tuple2<Integer, Integer>>(){})
        		.keyBy(0, 1)
        		.window(TumblingEventTimeWindows.of(window_slide))
        		.apply(new WindowFunction<Tuple2<Integer, Integer>, Tuple2<Integer, Integer>, Tuple, TimeWindow>() {
        		    /**
					 * 
					 */
					private static final long serialVersionUID = 1L;

					@Override
        		    public void apply(Tuple tuple, TimeWindow window, Iterable<Tuple2<Integer, Integer>> input, Collector<Tuple2<Integer, Integer>> out) throws Exception {
        		        out.collect(input.iterator().next());
        		    }
        		})
        		.map(input -> new Tuple2<Integer, Integer>(input.f1, 1))   // project to get <user_id, 1>
        		.returns(new TypeHint<Tuple2<Integer, Integer>>(){})
        		.keyBy(0)
        		.reduce((a, b) -> new Tuple2<Integer, Integer>(a.f0, a.f1 + b.f1))  // (user, 1) + (user, 1) = (user, 2)
        		.map(input -> tostring(input));
        		//.print();// Tuple2<Integer, Integer> --> String
         /**************************** TEST *********************************/
        	//	System.out.print("preGPS:");
        	//	preGPS.print();
        		
        		
        
        /**************************** PRODUCER *********************************/
        // TODO output to Kafka topic
		//FlinkKafkaProducer<String> myProducer = new FlinkKafkaProducer<>("localhost:9092", "output", new SimpleStringSchema());
		//join_GPS.addSink(myProducer );

        try 
        {
			env.execute("Window Join and Aggregation");
		} 
        catch (Exception e) 
        {
			e.printStackTrace();
		}
    }
	
	public static Tuple5<Integer, Integer, Float, Float, Long> parser_Photo(String str)
    {
    	String[] parts = str.split(" ");
    	int photo_id = Integer.parseInt(parts[0]);
    	int user_id = Integer.parseInt(parts[1]);
    	float lat = Float.parseFloat(parts[2]);
    	float lon = Float.parseFloat(parts[3]);
    	long timestamp = Long.parseLong(parts[4]);
    	return new Tuple5<Integer, Integer, Float, Float, Long>(photo_id, user_id, lat, lon, timestamp);
    }
    
	public static Tuple4<Integer, Float, Float, Long> parser_GPS(String str)
    {
    	String[] parts = str.split(" ");
    	int user_id = Integer.parseInt(parts[0]);
    	float lat = Float.parseFloat(parts[1]);
    	float lon = Float.parseFloat(parts[2]);
    	long timestamp = Long.parseLong(parts[3]);
    	return new Tuple4<Integer, Float, Float, Long>(user_id, lat, lon, timestamp);
    }
    
    public static Tuple3<Integer, Integer, Long> parser_Tag(String str)
    {
    	String[] parts = str.split(" ");
    	int photo_id = Integer.parseInt(parts[0]);
    	int user_id = Integer.parseInt(parts[1]);
    	long timestamp = Long.parseLong(parts[2]);
    	return new Tuple3<Integer, Integer, Long>(photo_id, user_id, timestamp);
    }
    
    public static String tostring(Tuple2<Integer, Integer> input)
    {
    	String user_id = Integer.toString(input.f0);
    	String count = Integer.toString(input.f1);
    	return user_id + " " + count;
    }
}
