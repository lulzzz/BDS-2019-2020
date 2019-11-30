package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.JoinFunction;

import scala.Tuple2;

public class JoinGPS implements JoinFunction<Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>>,
                                             Tuple2<Integer, Tuple2<Float, Float>>,
                                             // <photo_id, <user_id, <<lat1, lon1>, <lat2, lon2>>>>
                                             Tuple2<Integer, Tuple2<Integer, Tuple2<Tuple2<Float, Float>, Tuple2<Float, Float>>>>> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Tuple2<Integer, Tuple2<Integer, Tuple2<Tuple2<Float, Float>, Tuple2<Float, Float>>>> 
	       join(Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>> input1, Tuple2<Integer, Tuple2<Float, Float>> input2) throws Exception 
	{
		return new Tuple2(input1._1, new Tuple2(input1._2._1, new Tuple2(new Tuple2(input1._2._2._1, input1._2._2._2), new Tuple2(input2._2._1, input2._2._2))));
	}
}
