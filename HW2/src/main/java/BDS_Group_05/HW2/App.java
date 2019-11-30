package BDS_Group_05.HW2;

import java.util.Properties;

import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.api.common.typeinfo.TypeHint;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.api.java.tuple.Tuple4;
import org.apache.flink.streaming.api.TimeCharacteristic;
import org.apache.flink.streaming.api.environment.StreamExecutionEnvironment;
import org.apache.flink.streaming.api.functions.AssignerWithPunctuatedWatermarks;
import org.apache.flink.streaming.api.functions.sink.SinkFunction;
import org.apache.flink.streaming.api.watermark.Watermark;
import org.apache.flink.streaming.api.windowing.assigners.SlidingEventTimeWindows;
import org.apache.flink.streaming.api.windowing.time.Time;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaConsumer;
import org.apache.flink.streaming.connectors.kafka.FlinkKafkaProducer;
import org.apache.log4j.BasicConfigurator;



import org.apache.flink.streaming.api.datastream.DataStream;

public class App 
{
	public static void main( String[] args )
    {
    	// reference: https://stackoverflow.com/questions/12532339/no-appenders-could-be-found-for-loggerlog4j
    	BasicConfigurator.configure();
    	StreamExecutionEnvironment env = StreamExecutionEnvironment.getExecutionEnvironment();
    	
    	Properties properties = new Properties();
        properties.setProperty("bootstrap.servers", "localhost:9092");
        properties.setProperty("zookeeper.connect", "localhost:2181");
        properties.setProperty("group.id", "test");   // a group of consumers called "test"
        
        //env.enableCheckpointing(10); // checkpoint every 10 ms
        env.enableCheckpointing(10000); // checkpoint every 10 s
        env.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);
        
        DataStream<Tuple3<Integer, Float, Float>> GPS = env.addSource(new FlinkKafkaConsumer<>("GPS", new SimpleStringSchema(), properties))
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_GPS(input))
        		.returns(new TypeHint<Tuple3<Integer, Float, Float>>(){});
        
        DataStream<Tuple4<Integer, Integer, Float, Float>> Photo = env.addSource(new FlinkKafkaConsumer<>("Photo", new SimpleStringSchema(), properties))
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_Photo(input))
        		.returns(new TypeHint<Tuple4<Integer, Integer, Float, Float>>(){});
        
        DataStream<Tuple2<Integer, Integer>> Tag = env.addSource(new FlinkKafkaConsumer<>("Tag", new SimpleStringSchema(), properties))
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_Tag(input))
        		// reference: https://www.codota.com/code/java/methods/org.apache.flink.streaming.api.datastream.SingleOutputStreamOperator/returns
        		.returns(new TypeHint<Tuple2<Integer, Integer>>(){});
        
        // join on photo_id, to get where each photo was posted, <photo_id, user_id, lat, lon>
		DataStream<Tuple4<Integer, Integer, Float, Float>> join_Tag_Photo = Tag.join(Photo)
				.where(new SelectKeyFromTag())
	            .equalTo(new SelectKeyFromPhoto())
        		.window(SlidingEventTimeWindows.of(Time.seconds(15), Time.seconds(5)))
        		.apply(new JoinTagPhoto());
		
		// join on user_id, to get where the user is tracked, <photo_id, user_id, lat1, lon1, lat2, lon2>
        DataStream<Tuple2<Integer, Integer>> join_GPS = join_Tag_Photo.join(GPS)
        		.where(new SelectKey())            // select user_id from join_Tag_Photo
        		.equalTo(new SelectKeyFromGPS())   // select user_id from GPS
        		.window(SlidingEventTimeWindows.of(Time.seconds(15), Time.seconds(5)))
        		.apply(new JoinGPS())              // join on user_id, the result is <photo_id, user_id, lat1, lon1, lat2, lon2>
        		.filter(new Myfilter())            // filter out the location with distance > 5km
        		.map(input -> new Tuple2<Integer, Integer>(input.f1, 1))   // project to get <user_id, 1>
        		.returns(new TypeHint<Tuple2<Integer, Integer>>(){})
        		.keyBy(1)                          // all records with the same key are assigned to the same partition
        		// (user, 1) + (user, 1) = (user, 2)
        		.reduce((a, b) -> new Tuple2<Integer, Integer>(a.f0, a.f1 + b.f1));

        //join_GPS.print();
        /*
        SinkFunction<Tuple2<Integer, Integer>> myProducer = new FlinkKafkaProducer<Tuple2<Integer, Integer>>(
        		"localhost:9092",            // broker list
                "HW2",                  // target topic
                (KeyedSerializationSchema<Tuple2<Integer, Integer>>) new SimpleStringSchema());   // serialization schema
        
        result.addSink(myProducer);*/

        try 
        {
			env.execute("Window Join and Aggregation");
		} 
        catch (Exception e) 
        {
			e.printStackTrace();
		}
    }
	
	public static Tuple4<Integer, Integer, Float, Float> parser_Photo(String str)
    {
    	String[] parts = str.split(" ");
    	int photo_id = Integer.parseInt(parts[0]);
    	int user_id = Integer.parseInt(parts[1]);
    	float lat = Float.parseFloat(parts[2]);
    	float lon = Float.parseFloat(parts[3]);
    	return new Tuple4<Integer, Integer, Float, Float>(photo_id, user_id, lat, lon);
    }
    
    public static Tuple3<Integer, Float, Float> parser_GPS(String str)
    {
    	String[] parts = str.split(" ");
    	int user_id = Integer.parseInt(parts[0]);
    	float lat = Float.parseFloat(parts[1]);
    	float lon = Float.parseFloat(parts[2]);
    	return new Tuple3<Integer, Float, Float>(user_id, lat, lon);
    }
    
    public static Tuple2<Integer, Integer> parser_Tag(String str)
    {
    	String[] parts = str.split(" ");
    	int photo_id = Integer.parseInt(parts[0]);
    	int user_id = Integer.parseInt(parts[1]);
    	return new Tuple2<Integer, Integer>(photo_id, user_id);
    }
}
