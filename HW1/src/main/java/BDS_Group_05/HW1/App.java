package BDS_Group_05.HW1;

import java.util.*;
import scala.Tuple2;
import java.time.Instant;
import java.text.SimpleDateFormat;

import org.apache.spark.SparkConf;
import org.apache.spark.api.java.JavaRDD;
import org.apache.spark.api.java.JavaPairRDD;
import org.apache.spark.api.java.JavaSparkContext;

public class App {
    public static void main(String[] args) {
        SparkConf conf = new SparkConf().setAppName("BDS-HW1").setMaster("local");
        JavaSparkContext sc = new JavaSparkContext(conf);
        
        JavaRDD<String> input_1 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/Photo");
        JavaRDD<String> input_2 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/Tag");
        JavaRDD<String> input_3 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/Like");
        
        task_1(input_1, "task1");
        task_2(input_2, "task2");
        task_3(input_1, input_2, input_3, "task3");
    }
    
    public static void task_1(JavaRDD<String> input, String name)
    {
    	// 1st String: time, 2nd String: original str
        JavaPairRDD<String, String> Photo = input.mapToPair(str -> new Tuple2(parser_time(str), str));
        Photo = Photo.sortByKey();
        JavaRDD<String> output = Photo.map(pair -> pair._2);
        
        output.saveAsTextFile("hdfs://localhost:9000/user/krimmity/HW1/" + name);
    }
    
    public static void task_2(JavaRDD<String> input, String name)
    {
    	// 1st String: photo_id, 2nd Integer: 1 (used to count)
        JavaPairRDD<Integer, Integer> Tag = input.mapToPair(str -> parser_int(str));
        Tag = Tag.reduceByKey((a, b) -> a + b);
        
        Tag.saveAsTextFile("hdfs://localhost:9000/user/krimmity/HW1/" + name);
    }
    
    public static void task_3(JavaRDD<String> input_1, JavaRDD<String> input_2, JavaRDD<String> input_3, String name)
    {
    	// 1st Integer: Photo_id, 2nd Integer: User_id
    	JavaPairRDD<Integer, Integer> Tag = input_2.mapToPair(str -> parser_id_1(str)); 
    	JavaPairRDD<Integer, Integer> Like = input_3.mapToPair(str -> parser_id_2(str));
    	// get all (photo, user) pairs that satisfy (1) user is tagged in the photo, and (2) user likes the photo
    	JavaPairRDD<Integer, Integer> new_Tag = Tag.intersection(Like).distinct();
    	
    	// get the date when the photo is posted
    	JavaPairRDD<Integer, String> Photo_Time = input_1.mapToPair(str -> parser_photo_time(str));
    	// get the date when the photos in "Tag" are posted
    	// <photo_id, <user_id, date>>
    	JavaPairRDD<Integer, Tuple2<Integer, String>> join_on_photo = new_Tag.join(Photo_Time);
    	// <<user_id, date>, photo_id>
    	JavaPairRDD<Tuple2<Integer, String>, Integer> user_time_photo = join_on_photo.mapToPair(pair -> pair.swap());
    	
    	// get the date when the user posted photo
    	JavaPairRDD<Tuple2<Integer, String>, String> User_Time = input_1.mapToPair(str -> new Tuple2(parser_user_time(str), "tmp"));
    	// <<user_id, date>, <photo_id, "tmp">>
    	JavaPairRDD<Tuple2<Integer, String>, Tuple2<Integer, String>> join_on_user = user_time_photo.join(User_Time);
    	
    	JavaRDD<Tuple2> final_result = join_on_user.map(a -> new Tuple2(a._2._1, a._1._1)).distinct();
    	
    	// output
    	final_result.saveAsTextFile("hdfs://localhost:9000/user/krimmity/HW1/" + name);
    }
    
    public static String parser_time(String str)
    {
    	String[] parts = str.split(" ");
    	//int photo_id = Integer.parseInt(parts[0]);
    	//int user_id = Integer.parseInt(parts[1]);
    	//Instant time = Instant.parse(parts[2]);
    	//float lat = Float.parseFloat(parts[3]);
    	//float lon = Float.parseFloat(parts[4]);
    	return parts[2];
    }
    
    public static Tuple2<Integer, Integer> parser_int(String str)
    {
    	String[] parts = str.split(" ");
    	int photo_id = Integer.parseInt(parts[0]);
    	return new Tuple2(photo_id, 1);
    }
    
    public static Tuple2<Integer, Integer> parser_id_1(String str)
    {
    	String[] parts = str.split(" ");
    	int photo_id = Integer.parseInt(parts[0]);
    	int user_id = Integer.parseInt(parts[1]);
    	return new Tuple2(photo_id, user_id);
    }
    
    public static Tuple2<Integer, Integer> parser_id_2(String str)
    {
    	String[] parts = str.split(" ");
    	int user_id = Integer.parseInt(parts[0]);
    	int photo_id = Integer.parseInt(parts[1]);
    	return new Tuple2(photo_id, user_id);
    }
    
    public static Tuple2<Integer, String> parser_photo_time(String str)
    {
    	String[] parts = str.split(" ");
    	int photo_id = Integer.parseInt(parts[0]);
    	Instant time = Instant.parse(parts[2]);
    	Date date = Date.from(time);
    	SimpleDateFormat simpleDateFormat = new SimpleDateFormat("MM-dd-yyyy");
    	String day = simpleDateFormat.format(date);
    	return new Tuple2(photo_id, day);
    }
    
    public static Tuple2<Integer, String> parser_user_time(String str)
    {
    	String[] parts = str.split(" ");
    	int user_id = Integer.parseInt(parts[1]);
    	Instant time = Instant.parse(parts[2]);
    	Date date = Date.from(time);
    	SimpleDateFormat simpleDateFormat = new SimpleDateFormat("MM-dd-yyyy");
    	String day = simpleDateFormat.format(date);
    	return new Tuple2(user_id, day);
    }
}
