package de.tu_darmstadt.itsybitsandroidcapture.recording;

import de.tu_darmstadt.itsybitsandroidcapture.DrawView;

public class StopSegment extends DrawViewSegment {

    public StopSegment(int maxSteps, DrawView drawView) {
        super(maxSteps, drawView);
    }

    @Override
    public boolean advancesStep(boolean touched) {
        return !touched;
    }

    @Override
    public void doStep() {
        drawView.updateMode(3);
    }
}
