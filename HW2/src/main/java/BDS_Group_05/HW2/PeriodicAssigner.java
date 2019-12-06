package BDS_Group_05.HW2;

import org.apache.flink.streaming.api.functions.AssignerWithPeriodicWatermarks;
import org.apache.flink.streaming.api.watermark.Watermark;

public class PeriodicAssigner implements AssignerWithPeriodicWatermarks<String>{
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private final long maxOutOfOrderness = 20; //delay 20 ms

    private long currentMaxTimestamp;

    @Override
    public long extractTimestamp(String event, long previousTimeStamp) 
    {
		String[] parts = event.split(" ");
        long TimeStamp = Long.parseLong(parts[parts.length - 1]);   // the last data filed is event time
        currentMaxTimestamp = Math.max(currentMaxTimestamp, TimeStamp);
        return TimeStamp;
    }

    @Override
    public Watermark getCurrentWatermark() {
        // return the watermark as current highest timestamp minus the out-of-orderness bound
        return new Watermark(currentMaxTimestamp - maxOutOfOrderness);
    }
}

