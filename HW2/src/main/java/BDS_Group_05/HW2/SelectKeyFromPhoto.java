package BDS_Group_05.HW2;

import org.apache.flink.api.java.functions.KeySelector;
import org.apache.flink.api.java.tuple.Tuple4;

public class SelectKeyFromPhoto implements KeySelector<Tuple4<Integer, Integer, Float, Float>, Integer> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Integer getKey(Tuple4<Integer, Integer, Float, Float> input) throws Exception 
	{
		return input.f0;
	}
}
