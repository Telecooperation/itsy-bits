package de.tu_darmstadt.itsybitsandroidcapture.recording;

import android.support.v7.app.AppCompatActivity;

import java.io.FileOutputStream;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.io.File;

public class Recorder {
    private int recordCounter;
    private String filename;
    private String shapeName;
    private String participant;
    private AppCompatActivity activity;
    private FileOutputStream outputStream;

    private static final int FLUSH_RECORD_BATCH_SIZE = 1000;

    public Recorder(AppCompatActivity activity, String shapeName, String participant) {
        this.activity = activity;
        this.shapeName = shapeName;
        this.participant = participant;

        SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd-HH-mm-ss");
        String currentDateandTime = sdf.format(new Date());

        this.filename = "/sdcard/dt/RecordedCapImg-"+shapeName+"-"+participant+"-"+currentDateandTime+".csv";

        openRecording();
    }

    public void writeToFile(String capMatrix) {
        long time = System.currentTimeMillis();
        //String formattedTime = NumberFormat.getInstance(Locale.ENGLISH).format(time);

        String content = time + ";" + capMatrix+"\n";

        try {
            outputStream.write(content.getBytes());
        } catch (Exception e) {
            e.printStackTrace();
        }
        recordCounter++;

        if (recordCounter >= FLUSH_RECORD_BATCH_SIZE) {
            flush();
        }
    }

    private void flush() {
        //saveRecording();
        try {
            outputStream.flush();
        } catch (Exception e) {
            
        }
        recordCounter = 0;
        //resetRecording();
    }

    private void openRecording() {
        recordCounter = 0;
        String header = "AndroidTime;Data\n";

        try {
            File file = new File(filename);
            boolean newFile = !file.exists();

            File dir = activity.getFilesDir();
            String path = dir.getPath();

            //outputStream = activity.openFileOutput(filename, Context.MODE_PRIVATE | Context.MODE_APPEND);
            outputStream = new FileOutputStream(file, true);

            if (newFile) {
                outputStream.write(header.getBytes());
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void saveRecording() {
        try {
            outputStream.close();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
