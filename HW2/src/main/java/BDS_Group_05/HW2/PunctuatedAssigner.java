package BDS_Group_05.HW2;

import org.apache.flink.streaming.api.functions.AssignerWithPunctuatedWatermarks;
import org.apache.flink.streaming.api.watermark.Watermark;

public class PunctuatedAssigner implements AssignerWithPunctuatedWatermarks<String> 
{
	private static final long serialVersionUID = 1L;
	private long currentMaxTimestamp;
	private static long d = 20;    // delay 20 ms
	
	@Override 
    public Watermark checkAndGetNextWatermark(String lastEvent, long extractedTimestamp) 
    {
		//return extractedTimestamp > currentMaxTimestamp ? new Watermark(extractedTimestamp - d) : null;
		return new Watermark(currentMaxTimestamp - d);

    }

	@Override
    public long extractTimestamp(String event, long previousTimeStamp) 
    {
		String[] parts = event.split(" ");
        long TimeStamp = Long.parseLong(parts[parts.length - 1]);   // the last data filed is event time
        currentMaxTimestamp = Math.max(currentMaxTimestamp, TimeStamp);
        return TimeStamp;
    }
}