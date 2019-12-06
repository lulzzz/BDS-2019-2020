package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.JoinFunction;
import org.apache.flink.api.java.tuple.Tuple4;
import org.apache.flink.api.java.tuple.Tuple5;
import org.apache.flink.api.java.tuple.Tuple6;

public class testJoinGPS implements JoinFunction<Tuple4<Integer, Float, Float, Long>,         // <photo_id, user_id, lat1, lon1, timestamp>
Tuple4<Integer, Float, Float, Long>,                  // <user_id, lat2, lon2, timestamp>
Tuple4<Integer, Float, Float, Long>> // <photo_id, user_id, lat1, lon1, lat2, lon2>
{
private static final long serialVersionUID = 1L;
@Override
public Tuple4<Integer, Float, Float, Long> join(Tuple4<Integer, Float, Float, Long> input1,
		Tuple4<Integer, Float, Float, Long> second) throws Exception {
	// TODO Auto-generated method stub
	return new Tuple4<Integer, Float, Float, Long>(input1.f0, input1.f1, input1.f2, input1.f3);

}
}