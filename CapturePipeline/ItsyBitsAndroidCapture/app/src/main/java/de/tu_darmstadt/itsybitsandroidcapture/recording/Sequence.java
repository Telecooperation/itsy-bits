package de.tu_darmstadt.itsybitsandroidcapture.recording;

import java.util.List;

import de.tu_darmstadt.itsybitsandroidcapture.FullscreenActivity;

public class Sequence {
    private long lastTimestamp;
    private int minTimeDifference;
    private FullscreenActivity activity;
    private List<ISegment> segments;

    public Sequence(FullscreenActivity activity, int minTimeDifference, List<ISegment> segments) {
        this.activity = activity;
        this.minTimeDifference = minTimeDifference;
        lastTimestamp = System.currentTimeMillis();

        this.segments = segments;
    }

    public boolean proceed(int[] capImg) {

        //if current segment is finished, then pop it and go on with next segment
        if(getNextSegment().isFinished())
        {
            segments.remove(0);
        }

        //stop recording if no segments are left
        if(segments.size() == 0)
        {
            activity.stopRecording();
            return false;
        }

        //only advance step counter if at least minTimeDifference ms are passed
        long current = System.currentTimeMillis();
        if (current < lastTimestamp + minTimeDifference) {
            return getNextSegment().isCurrentlyRecordable();
        }
        lastTimestamp = current;


        //todo: needs rework
        int sum = 0;
        for (int value : capImg) {
            sum += value;
        }
        boolean touched = false;
        if (sum > 255 * 2) {
            touched = true;
        }
        //todo: needs rework


        ISegment segment = this.getNextSegment();
        return segment.run(touched);
    }

    private ISegment getNextSegment()
    {
        return segments.get(0);
    }

}
