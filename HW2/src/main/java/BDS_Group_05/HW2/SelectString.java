package BDS_Group_05.HW2;

import org.apache.flink.api.java.functions.KeySelector;

public class SelectString implements KeySelector<String, String> 
{

	private static final long serialVersionUID = 1L;

	@Override
	public String getKey(String input) throws Exception {
		return input;
	}

}
