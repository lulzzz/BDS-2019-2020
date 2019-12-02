package BDS_Group_05.HW2;

import org.apache.flink.api.java.functions.KeySelector;
import org.apache.flink.api.java.tuple.Tuple3;
import org.apache.flink.api.java.tuple.Tuple4;

public class SelectKeyFromGPS implements KeySelector<Tuple4<Integer, Float, Float, Long>, Integer> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Integer getKey(Tuple4<Integer, Float, Float, Long> input) throws Exception 
	{
		return input.f0;
	}

}
