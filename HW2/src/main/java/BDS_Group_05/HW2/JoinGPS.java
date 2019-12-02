package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.JoinFunction;
import org.apache.flink.api.java.tuple.Tuple4;
import org.apache.flink.api.java.tuple.Tuple5;
import org.apache.flink.api.java.tuple.Tuple6;


public class JoinGPS implements JoinFunction<Tuple5<Integer, Integer, Float, Float, Long>,         // <photo_id, user_id, lat1, lon1, timestamp>
                                             Tuple4<Integer, Float, Float, Long>,                  // <user_id, lat2, lon2, timestamp>
                                             Tuple6<Integer, Integer, Float, Float, Float, Float>> // <photo_id, user_id, lat1, lon1, lat2, lon2>
{
	private static final long serialVersionUID = 1L;

	@Override
	public Tuple6<Integer, Integer, Float, Float, Float, Float> 
	       join(Tuple5<Integer, Integer, Float, Float, Long> input1, Tuple4<Integer, Float, Float, Long> gps) throws Exception 
	{
		return new Tuple6<Integer, Integer, Float, Float, Float, Float>(input1.f0, input1.f1, input1.f2, input1.f3, gps.f1, gps.f2);
	}
}
