package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.JoinFunction;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.api.java.tuple.Tuple4;
import org.apache.flink.api.java.tuple.Tuple6;


public class JoinGPS implements JoinFunction<Tuple4<Integer, Integer, Float, Float>,
                                             Tuple3<Integer, Float, Float>,
                                             // <photo_id, user_id, lat1, lon1, lat2, lon2>
                                             Tuple6<Integer, Integer, Float, Float, Float, Float>> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Tuple6<Integer, Integer, Float, Float, Float, Float> 
	       join(Tuple4<Integer, Integer, Float, Float> input1, Tuple3<Integer, Float, Float> gps) throws Exception 
	{
		return new Tuple6<Integer, Integer, Float, Float, Float, Float>(input1.f0, input1.f1, input1.f2, input1.f3, gps.f1, gps.f2);
	}
}
