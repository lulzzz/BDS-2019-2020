package BDS_Group_05.HW2;

import org.apache.flink.api.common.functions.RichFlatMapFunction;
import org.apache.flink.api.common.state.ValueState;
import org.apache.flink.api.common.state.ValueStateDescriptor;
import org.apache.flink.api.java.tuple.Tuple2;
import org.apache.flink.configuration.Configuration;
import org.apache.flink.util.Collector;

public class DuplicateFilter extends RichFlatMapFunction<Tuple2<Integer, Integer>, Tuple2<Integer, Integer>> 
{
	private static final long serialVersionUID = 1L;
	static final ValueStateDescriptor<Boolean> descriptor = new ValueStateDescriptor<>("seen", Boolean.class, false);
    private ValueState<Boolean> operatorState;

    @Override
    public void open(Configuration configuration) 
    {
        operatorState = this.getRuntimeContext().getState(descriptor);
    }

    @Override
    public void flatMap(Tuple2<Integer, Integer> value, Collector<Tuple2<Integer, Integer>> out) throws Exception 
    {
        if (!operatorState.value()) // we haven't seen the element yet
        {
            out.collect(value);
            operatorState.update(true);  // set operator state to true so that we don't emit elements with this key again
        }
    }
}