package BDS_Group_05.HW2;

import org.apache.flink.api.java.functions.KeySelector;

import scala.Tuple2;

// select the user id from join_Tag_Photo
public class SelectKey implements KeySelector<Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>>, Integer> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Integer getKey(Tuple2<Integer, Tuple2<Integer, Tuple2<Float, Float>>> input) throws Exception 
	{
		return input._2._1;
	}

}
