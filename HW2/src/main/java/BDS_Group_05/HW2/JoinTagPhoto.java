package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.JoinFunction;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.api.java.tuple.Tuple5;

public class JoinTagPhoto implements JoinFunction<Tuple3<Integer, Integer, Long>,                // <photo_id, user_id>
                                                  Tuple5<Integer, Integer, Float, Float, Long>,  // <photo_id, user_id, lat, lon>
                                                  Tuple5<Integer, Integer, Float, Float, Long>>  // <photo_id, user_id, lat, lon>
{
	private static final long serialVersionUID = 1L;

	@Override
	public Tuple5<Integer, Integer, Float, Float, Long> join(Tuple3<Integer, Integer, Long> tag, Tuple5<Integer, Integer, Float, Float, Long> photo) throws Exception 
	{
		// <photo_id, user_id, lat, lon>
	//	return new Tuple5<Integer, Integer, Float, Float, Long>(1, 1, 1, 1, 1);   // timestamp????

		return new Tuple5<Integer, Integer, Float, Float, Long>(tag.f0, tag.f1, photo.f2, photo.f3, photo.f4);   // timestamp????
	}
}
