package de.tu_darmstadt.itsybitsandroidcapture.recording;

import de.tu_darmstadt.itsybitsandroidcapture.DrawView;

public abstract class DrawViewSegment implements ISegment {

    int step;
    int maxSteps;
    private boolean currentlyRecordable;
    DrawView drawView;

    DrawViewSegment(int maxSteps, DrawView drawView) {
        this.maxSteps = maxSteps;
        step = 0;
        currentlyRecordable = false;
        this.drawView = drawView;
    }

    @Override
    public boolean isFinished() {
        return (step == maxSteps);
    }

    @Override
    public boolean isCurrentlyRecordable() {
        return currentlyRecordable;
    }

    @Override
    public boolean run(boolean touched) {
        doStep();

        if (advancesStep(touched)) {
            step++;
            currentlyRecordable = touched;
            return touched;
        }

        currentlyRecordable = false;
        return false;
    }
}
