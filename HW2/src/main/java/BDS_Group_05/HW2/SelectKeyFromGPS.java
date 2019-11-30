package BDS_Group_05.HW2;

import org.apache.flink.api.java.functions.KeySelector;
import org.apache.flink.api.java.tuple.Tuple3;

public class SelectKeyFromGPS implements KeySelector<Tuple3<Integer, Float, Float>, Integer> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Integer getKey(Tuple3<Integer, Float, Float> input) throws Exception 
	{
		return input.f0;
	}

}
