package BDS_Group_05.HW2;

import org.apache.flink.api.java.tuple.Tuple;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.streaming.api.functions.windowing.WindowFunction;
import org.apache.flink.streaming.api.windowing.windows.TimeWindow;
import org.apache.flink.util.Collector;

public class RemoveDuplicate implements WindowFunction<Tuple2<Integer, Integer>, Tuple2<Integer, Integer>, Tuple, TimeWindow> 
{
	private static final long serialVersionUID = 1L;

	@Override
    public void apply(Tuple tuple, 
    		          TimeWindow window, 
    		          Iterable<Tuple2<Integer, Integer>> input, 
    		          Collector<Tuple2<Integer, Integer>> out) throws Exception 
	{
        out.collect(input.iterator().next());
    }
}
