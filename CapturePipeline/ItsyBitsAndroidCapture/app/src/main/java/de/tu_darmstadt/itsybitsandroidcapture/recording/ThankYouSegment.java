package de.tu_darmstadt.itsybitsandroidcapture.recording;

import de.tu_darmstadt.itsybitsandroidcapture.DrawView;

public class ThankYouSegment extends DrawViewSegment {

    public ThankYouSegment(int maxSteps, DrawView drawView) {
        super(maxSteps, drawView);
    }

    @Override
    public boolean advancesStep(boolean touched) {
        return true;
    }

    @Override
    public void doStep() {
        drawView.updateMode(4);
    }
}
