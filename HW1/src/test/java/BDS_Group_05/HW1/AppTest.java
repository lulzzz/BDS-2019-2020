package BDS_Group_05.HW1;

import java.util.*;
import junit.framework.Assert;
import junit.framework.TestCase;

import org.apache.spark.SparkConf;
import org.apache.spark.api.java.JavaRDD;
import org.apache.spark.api.java.JavaSparkContext;

public class AppTest extends TestCase
{
    public void test()
    {
    	App app = new App();
    	SparkConf conf = new SparkConf().setAppName("BDS-HW1").setMaster("local");
        JavaSparkContext sc = new JavaSparkContext(conf);
        
        JavaRDD<String> input_1 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/Photo_test");
        JavaRDD<String> input_2 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/Tag_test");
        JavaRDD<String> input_3 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/Like_test");
        
        app.task_1(input_1, "task1_test");
        app.task_2(input_2, "task2_test");
        app.task_3(input_1, input_2, input_3, "task3_test");
        
        JavaRDD<String> output_1 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/task1_test/part-00000");
        JavaRDD<String> output_2 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/task2_test/part-00000");
        JavaRDD<String> output_3 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/task3_test/part-00000");
        
        JavaRDD<String> expect_output_1 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/Task_1_test_result");
        JavaRDD<String> expect_output_2 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/Task_2_test_result");
        JavaRDD<String> expect_output_3 = sc.textFile("hdfs://localhost:9000/user/krimmity/HW1/Task_3_test_result");
        
        validate_task1(output_1, expect_output_1);
        validate(output_2, expect_output_2);  // validate task 2
        validate(output_3, expect_output_3);  // validate task 1
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
