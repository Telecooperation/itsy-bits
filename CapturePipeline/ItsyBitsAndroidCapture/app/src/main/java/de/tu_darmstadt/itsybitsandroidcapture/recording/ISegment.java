package de.tu_darmstadt.itsybitsandroidcapture.recording;

public interface ISegment {

    boolean run(boolean touched);

    boolean advancesStep(boolean touched);

    void doStep();

    boolean isCurrentlyRecordable();

    boolean isFinished();
}
