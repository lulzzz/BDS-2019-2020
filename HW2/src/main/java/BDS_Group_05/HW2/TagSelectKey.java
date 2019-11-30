package BDS_Group_05.HW2;

import org.apache.flink.api.java.functions.KeySelector;

import scala.Tuple2;

public class TagSelectKey implements KeySelector<Tuple2<Integer, Integer>, Integer> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Integer getKey(Tuple2<Integer, Integer> input) throws Exception 
	{
		return input._1;
	}
}
