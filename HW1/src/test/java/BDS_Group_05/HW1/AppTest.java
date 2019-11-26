package BDS_Group_05.HW1;

import java.io.IOException;
import java.util.*;
import junit.framework.Assert;
import junit.framework.TestCase;

import org.apache.spark.SparkConf;
import org.apache.spark.api.java.JavaRDD;
import org.apache.spark.api.java.JavaSparkContext;
import static BDS_Group_05.HW1.App.hdfspath;
import static BDS_Group_05.HW1.App.userpath;
import static BDS_Group_05.HW1.App.inputpath;
import static BDS_Group_05.HW1.App.outputpath;



public class AppTest extends TestCase
{
	public static final String testhome = "";
	
    public void test()
    {
    	SparkConf conf = new SparkConf().setAppName("BDS-HW1").setMaster("local");
        JavaSparkContext sc = new JavaSparkContext(conf);
        
        JavaRDD<String> input_1 = sc.textFile(hdfspath + userpath + inputpath + "Photo_test");
        JavaRDD<String> input_2 = sc.textFile(hdfspath + userpath + inputpath + "Tag_test");
        JavaRDD<String> input_3 = sc.textFile(hdfspath + userpath + inputpath  + "Like_test");
        
        try {
			App.task_1(input_1, "task1_test");
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
  //      App.task_2(input_2, "task2_test");
  //      App.task_3(input_1, input_2, input_3, "task3_test");
        
        JavaRDD<String> output_1 = sc.textFile(hdfspath + userpath + outputpath + "task1_test/part-00000");
    //    JavaRDD<String> output_2 = sc.textFile(App.outputhome + "task2_test/part-00000");
      //  JavaRDD<String> output_3 = sc.textFile(App.outputhome + "task3_test/part-00000");
        
        JavaRDD<String> expect_output_1 = sc.textFile(hdfspath + userpath + inputpath + "Task_1_test_result");
    //    JavaRDD<String> expect_output_2 = sc.textFile(App.inputhome + "Task_2_test_result");
      //  JavaRDD<String> expect_output_3 = sc.textFile(App.inputhome + "Task_3_test_result");
        
        validate_task1(output_1, expect_output_1); 
    //    validate(output_2, expect_output_2);  // validate task 2
      //  validate(output_3, expect_output_3);  // validate task 3
    }
    
    public static void validate_task1(JavaRDD<String> actual, JavaRDD<String> expected)
    {
    	int count_actual = (int)actual.count();
    	int count_expected = (int)expected.count();
    	Assert.assertEquals(expected.take(count_expected), actual.take(count_actual));
    }

	public static void validate(JavaRDD<String> actual, JavaRDD<String> expected)
    {
		Assert.assertEquals(expected.count(), actual.count());
		List<String> actual_str = actual.take((int)actual.count());
		List<String> expected_str = expected.take((int)expected.count());
		Assert.assertTrue(actual_str.containsAll(expected_str));
    }
}
