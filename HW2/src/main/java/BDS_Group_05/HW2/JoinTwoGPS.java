package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.JoinFunction;
import org.apache.flink.api.java.tuple.Tuple4;

public class JoinTwoGPS implements JoinFunction<String, String, String> 
{
	private static final long serialVersionUID = 1L;

	@Override
	public String join(String GPS_1, String GPS_2) throws Exception 
	{
		return GPS_1;
	}

}
