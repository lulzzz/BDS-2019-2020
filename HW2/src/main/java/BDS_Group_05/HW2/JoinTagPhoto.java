package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.JoinFunction;

import scala.Tuple2;

public class JoinTagPhoto implements JoinFunction<Tuple2<Integer, Integer>, 
                                                  Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>>, 
                                                  Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>>> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>> join(Tuple2<Integer, Integer> tag, Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>> photo) throws Exception 
	{
		return new Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>>(tag._1, new Tuple2<Integer, Tuple2<Float, Float>>(tag._2, new Tuple2<Float, Float>(photo._2._2._1, photo._2._2._2)));
	}
}
