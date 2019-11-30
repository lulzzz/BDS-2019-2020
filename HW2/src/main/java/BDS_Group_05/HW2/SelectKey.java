package BDS_Group_05.HW2;

import org.apache.flink.api.java.functions.KeySelector;
import org.apache.flink.api.java.tuple.Tuple4;

// select the user id from join_Tag_Photo
public class SelectKey implements KeySelector<Tuple4<Integer, Integer, Float, Float>, Integer> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public Integer getKey(Tuple4<Integer, Integer, Float, Float> input) throws Exception 
	{
		return input.f1;   // user_id
	}

}
