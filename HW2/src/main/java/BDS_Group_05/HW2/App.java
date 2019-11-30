package BDS_Group_05.HW2;

import java.util.Properties;

import org.apache.flink.api.common.serialization.SimpleStringSchema;
import org.apache.flink.api.common.typeinfo.Types;
import org.apache.flink.api.java.tuple.Tuple;
import org.apache.flink.api.java.typeutils.TupleTypeInfo;
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

import scala.Tuple2;

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
        
        DataStream<Tuple2<Integer, Tuple2<Float, Float>>> GPS = env.addSource(new FlinkKafkaConsumer<>("GPS", new SimpleStringSchema(), properties))
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_GPS(input));
        
        DataStream<Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>>> Photo = env.addSource(new FlinkKafkaConsumer<>("Photo", new SimpleStringSchema(), properties))
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_Photo(input));
        
        DataStream<Tuple2<Integer, Integer>> Tag = env.addSource(new FlinkKafkaConsumer<>("Photo", new SimpleStringSchema(), properties))
        		.assignTimestampsAndWatermarks(new PunctuatedAssigner())
        		.map(input -> parser_Tag(input));
        
        // join on photo_id, to get where each photo was posted
		DataStream<Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>>> join_Tag_Photo = Tag.join(Photo)
				.where(new TagSelectKey())
	            .equalTo(new PhotoSelectKey())
        		.window(SlidingEventTimeWindows.of(Time.seconds(15), Time.seconds(5)))
        		.apply(new JoinTagPhoto());
		
		// join on user_id, to get where the user is tracked
        DataStream<Tuple2<Integer, Tuple2<Integer, Tuple2<Tuple2<Float, Float>, Tuple2<Float, Float>>>>> join_GPS = join_Tag_Photo.join(GPS)
        		.where(new SelectKey())
        		.equalTo(new UserSelectKey())
        		.window(SlidingEventTimeWindows.of(Time.seconds(15), Time.seconds(5)))
        		.apply(new JoinGPS())
        		.filter(new Myfilter());   // to see if the location is within 5km
        
        DataStream<Tuple2<Integer, Integer>> result = join_GPS.map(input -> new Tuple2(input._1, input._2._1));
        		//.windowAll(SlidingEventTimeWindows.of(Time.seconds(15), Time.seconds(5)));
        		//.aggregate(new Myaggregate());   // aggregate by key
        result.print();
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
	
	public static Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>> parser_Photo(String str)
    {
    	String[] parts = str.split(" ");
    	int photo_id = Integer.parseInt(parts[0]);
    	int user_id = Integer.parseInt(parts[1]);
    	float lat = Float.parseFloat(parts[2]);
    	float lon = Float.parseFloat(parts[3]);
    	return new Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>>(photo_id, new Tuple2<Integer, Tuple2<Float, Float>>(user_id, new Tuple2<Float, Float>(lat, lon)));
    }
    
    public static Tuple2<Integer, Tuple2<Float, Float>> parser_GPS(String str)
    {
    	String[] parts = str.split(" ");
    	int user_id = Integer.parseInt(parts[0]);
    	float lat = Float.parseFloat(parts[1]);
    	float lon = Float.parseFloat(parts[2]);
    	return new Tuple2<Integer, Tuple2<Float, Float>>(user_id, new Tuple2<Float, Float>(lat, lon));
    }
    
    public static Tuple2<Integer, Integer> parser_Tag(String str)
    {
    	String[] parts = str.split(" ");
    	int photo_id = Integer.parseInt(parts[0]);
    	int user_id = Integer.parseInt(parts[1]);
    	return new Tuple2<Integer, Integer>(photo_id, user_id);
    }
}
