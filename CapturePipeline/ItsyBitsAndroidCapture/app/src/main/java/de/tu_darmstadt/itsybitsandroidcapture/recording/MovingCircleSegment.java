package de.tu_darmstadt.itsybitsandroidcapture.recording;

import de.tu_darmstadt.itsybitsandroidcapture.DrawView;

public class MovingCircleSegment extends DrawViewSegment {

    private int[] start;
    private int[] destination;

    public MovingCircleSegment(int maxSteps, DrawView drawView, int[] start, int[] destination) {
        super(maxSteps, drawView);

        this.start = start;
        this.destination = destination;
    }

    @Override
    public boolean advancesStep(boolean touched) {
        return touched;
    }

    @Override
    public void doStep() {
        drawView.updateCircle(
                start[0] + (int) ((destination[0] - start[0]) * ((float) step / maxSteps)),
                start[1] + (int) ((destination[1] - start[1]) * ((float) step / maxSteps))
        );
        drawView.updateMode(1);
    }
}
