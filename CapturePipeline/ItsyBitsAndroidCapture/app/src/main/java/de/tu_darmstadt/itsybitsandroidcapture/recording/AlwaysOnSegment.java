package de.tu_darmstadt.itsybitsandroidcapture.recording;

public class AlwaysOnSegment implements ISegment {

    private boolean currentlyRecordable = true;

    public AlwaysOnSegment() {

    }

    public void stop() {
        currentlyRecordable = false;
    }

    @Override
    public boolean run(boolean touched) {
        return touched;
    }

    @Override
    public boolean advancesStep(boolean touched) {
        return touched;
    }

    @Override
    public void doStep() {

    }

    @Override
    public boolean isCurrentlyRecordable() {
        return currentlyRecordable;
    }

    @Override
    public boolean isFinished() {
        return currentlyRecordable;
    }
}
