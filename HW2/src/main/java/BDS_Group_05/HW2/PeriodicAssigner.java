package BDS_Group_05.HW2;

import org.apache.flink.streaming.api.functions.AssignerWithPeriodicWatermarks;
import org.apache.flink.streaming.api.watermark.Watermark;

public class PeriodicAssigner implements AssignerWithPeriodicWatermarks<String> 
{
	private long currentMaxTimestamp;
	private final long maxOutOfOrderness = 20;         // allow 20ms delay
	private static final long serialVersionUID = 1L;

    @Override
    public long extractTimestamp(String event, long previousTimeStamp) 
    {
    	String[] parts = event.split(" ");
   	    long TimeStamp = Long.parseLong(parts[parts.length - 1]);   // the last data filed is event time
   	    currentMaxTimestamp = Math.max(currentMaxTimestamp, TimeStamp);
	    return TimeStamp;
    }

	@Override
	public Watermark getCurrentWatermark() 
	{
		return new Watermark(currentMaxTimestamp - maxOutOfOrderness);
	}
}
