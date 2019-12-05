package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.JoinFunction;
import org.apache.flink.api.java.tuple.Tuple4;

public class JoinTwoGPS implements JoinFunction<Tuple4<Integer, Float, Float, Long>, 
                                                Tuple4<Integer, Float, Float, Long>, 
                                                Tuple4<Integer, Float, Float, Long>> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Tuple4<Integer, Float, Float, Long> join(Tuple4<Integer, Float, Float, Long> GPS_1, Tuple4<Integer, Float, Float, Long> GPS_2) throws Exception 
	{
		return new Tuple4<Integer, Float, Float, Long>(GPS_1.f0, GPS_1.f1, GPS_1.f2, GPS_1.f3);
	}

}
