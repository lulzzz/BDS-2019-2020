package BDS_Group_05.HW2;

import java.io.File;
import java.util.Set;
import java.util.HashSet;
import java.io.FileReader;
import java.io.IOException;
import java.io.BufferedReader;
import junit.framework.Assert;
import junit.framework.TestCase;

import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.TimeCharacteristic;
import org.apache.flink.streaming.api.datastream.DataStream;

public class AppTest extends TestCase
{
	public static final String usr_path = "/Users/lfc746/Desktop/BDS-2019-2020/HW2/testFile/";
	public void testSmallDataset() throws IOException
    {
    	App.env.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);
    	
    	final String GPS_file = usr_path + "smallDataset-GPS"; 
    	final String Tag_file = usr_path + "smallDataset-Tag"; 
    	final String Photo_file = usr_path + "smallDataset-Photo"; 
    	final String path = usr_path + "smallDataset-actual";
    	
    	DataStream<String> Photo = App.env.readTextFile(Photo_file);
    	DataStream<String> Tag = App.env.readTextFile(Tag_file);
    	DataStream<String> GPS = App.env.readTextFile(GPS_file);
    	
    	try {
			App.process(Photo, Tag, GPS, true, path);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    	
    	File file1 = new File(usr_path + "smallDataset-actual");
    	File file2 = new File(usr_path + "smallDataset-expect");
    	@SuppressWarnings("resource")
		BufferedReader buffer1 = new BufferedReader(new FileReader(file1));
    	@SuppressWarnings("resource")
		BufferedReader buffer2 = new BufferedReader(new FileReader(file2));
    	
    	Set<Tuple2<Integer, Integer>> actual = new HashSet<Tuple2<Integer, Integer>>();
    	Set<Tuple2<Integer, Integer>> expect = new HashSet<Tuple2<Integer, Integer>>();
    	
    	String st; 
    	while ((st = buffer1.readLine()) != null) 
    	{
    		Tuple2<Integer, Integer> record = parse_Integer(st);
    		actual.add(record);
    	}
    	
    	while ((st = buffer2.readLine()) != null) 
    	{
    		Tuple2<Integer, Integer> record = parse_Integer(st);
    		expect.add(record);
    	}
    	
    	Assert.assertEquals(actual.size(), expect.size());
    	assertTrue(actual.containsAll(expect));
    }
    
    public void testOutOfOrderData() throws IOException
    {
    	App.env.setStreamTimeCharacteristic(TimeCharacteristic.EventTime);
    	
    	// process out-of-order data
    	final String out_GPS_file = usr_path + "out-of-order-GPS"; 
    	final String out_Tag_file = usr_path + "out-of-order-Tag"; 
    	final String out_Photo_file = usr_path + "out-of-order-Photo"; 
    	final String out_path = usr_path + "out-of-order-result";
    	
    	DataStream<String> out_Photo = App.env.readTextFile(out_Photo_file);
    	DataStream<String> out_Tag = App.env.readTextFile(out_Tag_file);
    	DataStream<String> out_GPS = App.env.readTextFile(out_GPS_file);
    	
    	try {
			App.process(out_Photo, out_Tag, out_GPS, true, out_path);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    	
    	// process in-order data
    	final String in_GPS_file = usr_path + "in-order-GPS"; 
    	final String in_Tag_file = usr_path + "in-order-Tag"; 
    	final String in_Photo_file = usr_path + "in-order-Photo"; 
    	final String in_path = usr_path + "in-order-result";
    	
    	DataStream<String> in_Photo = App.env.readTextFile(in_Photo_file);
    	DataStream<String> in_Tag = App.env.readTextFile(in_Tag_file);
    	DataStream<String> in_GPS = App.env.readTextFile(in_GPS_file);
    	
    	try {
			App.process(in_Photo, in_Tag, in_GPS, true, in_path);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    	
    	// compare two results
    	File file1 = new File(usr_path + "out-of-order-result");
    	File file2 = new File(usr_path + "in-order-result");
    	@SuppressWarnings("resource")
		BufferedReader buffer1 = new BufferedReader(new FileReader(file1));
    	@SuppressWarnings("resource")
		BufferedReader buffer2 = new BufferedReader(new FileReader(file2));
    	
    	Set<Tuple2<Integer, Integer>> out = new HashSet<Tuple2<Integer, Integer>>();
    	Set<Tuple2<Integer, Integer>> in = new HashSet<Tuple2<Integer, Integer>>();
    	
    	String st; 
    	while ((st = buffer1.readLine()) != null) 
    	{
    		Tuple2<Integer, Integer> record = parse_Integer(st);
    		out.add(record);
    	}
    	
    	while ((st = buffer2.readLine()) != null) 
    	{
    		Tuple2<Integer, Integer> record = parse_Integer(st);
    		in.add(record);
    	}
    	
    	Assert.assertEquals(out.size(), in.size());
    	assertTrue(out.containsAll(in));
    }
    
    public Tuple2<Integer, Integer> parse_Integer(String str)
    {
    	String[] parts = str.split(" ");
    	int user_id = Integer.parseInt(parts[0]);
    	int count = Integer.parseInt(parts[1]);
    	return new Tuple2<Integer, Integer>(user_id, count);
    }
}
