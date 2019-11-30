package BDS_Group_05.HW2;

import org.apache.flink.api.java.functions.KeySelector;
import org.apache.flink.api.java.tuple.Tuple2;

public class SelectKeyFromTag implements KeySelector<Tuple2<Integer, Integer>, Integer> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Integer getKey(Tuple2<Integer, Integer> input) throws Exception 
	{
		return input.f0;
	}
}
