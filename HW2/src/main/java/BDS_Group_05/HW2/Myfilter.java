package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.FilterFunction;
import org.apache.flink.api.java.tuple.Tuple6;

public class Myfilter implements
		FilterFunction<Tuple6<Integer, Integer, Float, Float, Float, Float>> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public boolean filter(Tuple6<Integer, Integer, Float, Float, Float, Float> input)
			throws Exception 
	{
		//int photo_id = input.f0;
		//int user_id = input.f1;
		float lat1 = input.f2;
		float lon1 = input.f3;
		float lat2 = input.f4;
		float lon2 = input.f5;
		double distance = DistanceCalculator.distance(lat1, lat2, lon1, lon2);
		if(distance <= 5) return true;
		else return false;
	}

}
