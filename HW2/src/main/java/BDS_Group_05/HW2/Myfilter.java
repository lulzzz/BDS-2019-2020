package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.FilterFunction;
import scala.Tuple2;

public class Myfilter implements
		FilterFunction<Tuple2<Integer, Tuple2<Integer, Tuple2<Tuple2<Float, Float>, Tuple2<Float, Float>>>>> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public boolean filter(Tuple2<Integer, Tuple2<Integer, Tuple2<Tuple2<Float, Float>, Tuple2<Float, Float>>>> input)
			throws Exception 
	{
		//int photo_id = input._1;
		//int user_id = input._2._1;
		float lat1 = input._2._2._1._1;
		float lon1 = input._2._2._1._2;
		float lat2 = input._2._2._2._1;
		float lon2 = input._2._2._2._2;
		double distance = DistanceCalculator.distance(lat1, lat2, lon1, lon2);
		if(distance <= 5) return true;
		else return false;
	}

}
