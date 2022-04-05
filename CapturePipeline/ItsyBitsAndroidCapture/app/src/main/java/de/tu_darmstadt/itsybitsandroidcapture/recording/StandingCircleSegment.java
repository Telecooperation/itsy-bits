package de.tu_darmstadt.itsybitsandroidcapture.recording;

import de.tu_darmstadt.itsybitsandroidcapture.DrawView;

public class StandingCircleSegment extends DrawViewSegment {

    private int[] position;

    public StandingCircleSegment(int maxSteps, DrawView drawView, int[] position) {
        super(maxSteps, drawView);

        this.position = position;
    }

    @Override
    public boolean advancesStep(boolean touched) {
        return touched;
    }

    @Override
    public void doStep() {
        drawView.updateCircle(
                position[0],
                position[1]
        );
        drawView.updateMode(2);
    }
}
