package BDS_Group_05.HW2;

import org.apache.flink.api.java.functions.KeySelector;
import org.apache.flink.api.java.tuple.Tuple5;

public class SelectKeyFromPhoto implements KeySelector<Tuple5<Integer, Integer, Float, Float, Long>, Integer> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Integer getKey(Tuple5<Integer, Integer, Float, Float, Long> input) throws Exception 
	{
		return input.f0;
	}
}
