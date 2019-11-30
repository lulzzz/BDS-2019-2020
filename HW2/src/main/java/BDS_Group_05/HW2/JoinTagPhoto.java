package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.JoinFunction;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.api.java.tuple.Tuple4;

public class JoinTagPhoto implements JoinFunction<Tuple2<Integer, Integer>, 
                                                  Tuple4<Integer, Integer, Float, Float>, 
                                                  Tuple4<Integer, Integer, Float, Float>> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Tuple4<Integer, Integer, Float, Float> join(Tuple2<Integer, Integer> tag, Tuple4<Integer, Integer, Float, Float> photo) throws Exception 
	{
		// <photo_id, user_id, lat, lon>
		return new Tuple4<Integer, Integer, Float, Float>(tag.f0, tag.f1, photo.f2, photo.f3);
	}
}
