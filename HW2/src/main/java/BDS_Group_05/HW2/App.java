package BDS_Group_05.HW2;

import java.util.Properties;

import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.api.java.tuple.Tuple4;
import org.apache.flink.api.java.tuple.Tuple5;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.streaming.api.CheckpointingMode;
import org.apache.flink.streaming.api.TimeCharacteristic;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.api.datastream.DataStream;
import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.windowing.assigners.SlidingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.assigners.TumblingProcessingTimeWindows;

public class App 
{
	public static void main( String[] args )
    {
		Time window_length = Time.seconds(15);
		Time window_slide = Time.seconds(5);
    	StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
    	
    	Properties properties = new Properties();
        properties.setProperty("bootstrap.servers", "localhost:9092");
        properties.setProperty("zookeeper.connect", "localhost:2181");
        properties.setProperty("group.id", "test");   // a group of consumers called "test"
        
        /**************************** CHECKPOINT *********************************/
        //env.enableCheckpointing(10); // start a checkpoint every 10 ms
        env.enableCheckpointing(10000); // start a checkpoint every 10 s
        env.getCheckpointConfig().setCheckpointingMode(CheckpointingMode.EXACTLY_ONCE);
        //env.setStateBackend(new FsStateBackend("hdfs://localhost:9000/user/krimmity/checkpoint"));   // ??????
        
        /**************************** METRICS *********************************/
        // TODO: latency metrics??????
        
        /**************************** TIMESTAMP & WATERMARK *********************************/
        env.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);
        
        /**************************** CONSUMER *********************************/
        FlinkKafkaConsumer<String> GPS_consumer = new FlinkKafkaConsumer<String>("GPS", new SimpleStringSchema(), properties);
        FlinkKafkaConsumer<String> Tag_consumer = new FlinkKafkaConsumer<String>("Tag", new SimpleStringSchema(), properties);
        FlinkKafkaConsumer<String> Photo_consumer = new FlinkKafkaConsumer<String>("Photo", new SimpleStringSchema(), properties);
        //consumer.setStartFromTimestamp(startupOffsetsTimestamp);
        
        // reference: https://www.codota.com/code/java/methods/org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator/returns
		//returns(new TypeHint<Tuple4<Integer, Float, Float, Long>>(){});
        
        // reference: https://ci.apache.org/projects/flink/flink-docs-stable/dev/stream/operators/windows.html#sliding-wixandows
        // GPS: <user_id, lat, lon, timestamp>
        DataStream<Tuple4<Integer, Float, Float, Long>> GPS = env.addSource(GPS_consumer)
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		//.map(new parser_GPS())
        		.map(input -> parser_GPS(input))
        		// reference: https://www.codota.com/code/java/methods/org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator/returns
        		.returns(new TypeHint<Tuple4<Integer, Float, Float, Long>>(){});
        
        // Photo: <photo_id, user_id, lat, lon, timestamp>
        DataStream<Tuple5<Integer, Integer, Float, Float, Long>> Photo = env.addSource(Photo_consumer)
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_Photo(input))
        		.returns(new TypeHint<Tuple5<Integer, Integer, Float, Float, Long>>(){});
        
        // Tag: <photo_id, user_id>
        DataStream<Tuple3<Integer, Integer, Long>> Tag = env.addSource(Tag_consumer)
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_Tag(input))
        		.returns(new TypeHint<Tuple3<Integer, Integer, Long>>(){});
        
        // join on photo_id, to get where each photo was posted, <photo_id, user_id, lat, lon>
        // reference (window join): https://ci.apache.org/projects/flink/flink-docs-stable/dev/stream/operators/joining.html
		DataStream<Tuple5<Integer, Integer, Float, Float, Long>> windowed_join_Tag_Photo = Tag.join(Photo)
				.where(new SelectKeyFromTag())     // select photo_id from Tag 
	            .equalTo(new SelectKeyFromPhoto()) // select photo_id from Photo
	            .window(SlidingEventTimeWindows.of(window_length, window_slide))
        		.apply(new JoinTagPhoto());        // the join function
		
		DataStream<Tuple4<Integer, Float, Float, Long>> windowed_GPS = GPS.join(GPS)
        		.where(new SelectKeyFromGPS())
        		.equalTo(new SelectKeyFromGPS())
        		.window(SlidingEventTimeWindows.of(window_length, window_slide))
        		.apply(new JoinTwoGPS());
		
		// join on user_id, to get where the user is tracked, <photo_id, user_id, lat1, lon1, lat2, lon2>
		// lat1, lon1: where the photo was posted
		// lat2, lon2: where the user was tracked
        DataStream<Tuple2<Integer, Integer>> join_GPS = windowed_join_Tag_Photo.join(windowed_GPS)
        		.where(new SelectKey())            // select user_id from join_Tag_Photo
        		.equalTo(new SelectKeyFromGPS())   // select user_id from GPS
        		.window(TumblingProcessingTimeWindows.of(window_length))
        		.apply(new JoinGPS())              // join on user_id, the result is <photo_id, user_id, lat1, lon1, lat2, lon2>
        		.filter(new Myfilter())            // filter out the location with distance > 5km
        		.map(input -> new Tuple2<Integer, Integer>(input.f1, input.f0))
        		.returns(new TypeHint<Tuple2<Integer, Integer>>(){})
        		.keyBy(0)                          // partition by user_id
        		.flatMap(new DuplicateFilter());
        /*
        		.map(input -> new Tuple2<Integer, Integer>(input.f1, 1))   // project to get <user_id, 1>
        		.returns(new TypeHint<Tuple2<Integer, Integer>>(){})
        		.keyBy(0)
        		.reduce((a, b) -> new Tuple2<Integer, Integer>(a.f0, a.f1 + b.f1))  // (user, 1) + (user, 1) = (user, 2)
        		.map(input -> tostring(input));    // Tuple2<Integer, Integer> --> String*/
		
        join_GPS.print();
        
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
