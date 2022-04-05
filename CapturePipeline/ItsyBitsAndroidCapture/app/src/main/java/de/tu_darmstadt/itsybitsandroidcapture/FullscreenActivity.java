package de.tu_darmstadt.itsybitsandroidcapture;

import android.annotation.SuppressLint;
import android.graphics.Color;
import android.os.AsyncTask;
import android.support.constraint.ConstraintLayout;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;
import android.widget.EditText;
import android.widget.RelativeLayout;
import android.widget.Spinner;
import android.widget.Toast;

//import libftsp
import org.hcilab.libftsp.LocalDeviceHandler;
import org.hcilab.libftsp.capacitivematrix.capmatrix.CapacitiveImageTS;
import org.hcilab.libftsp.listeners.LocalCapImgListener;

import java.util.concurrent.ThreadLocalRandom;

import de.tu_darmstadt.itsybitsandroidcapture.recording.Recorder;
import de.tu_darmstadt.itsybitsandroidcapture.recording.Sequence;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 */
public class FullscreenActivity extends AppCompatActivity {

    private RelativeLayout recordingLayout;
    private DrawView drawView;
    Recorder recorder;
    Sequence sequence;
    Spinner shapeNameField;
    EditText participantField;
    ConstraintLayout controls;

    /**
     * Touch listener for the start button
     * starts the record if no recording is running
     */
    private final View.OnTouchListener mStartButtonTouchListener = new View.OnTouchListener() {
        @Override
        public boolean onTouch(View view, MotionEvent motionEvent) {
            if (!recording()) {
                startRecording();
            }
            return true;
        }
    };

    /**
     * Creation of the activity
     * Initialize views and handlers
     *
     * @param savedInstanceState
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_fullscreen);

        recordingLayout = findViewById(R.id.recordingScreen);

        //shapeNameField = findViewById(R.id.shape);
        //participantField = findViewById(R.id.participant);
        controls = findViewById(R.id.setupControls);

        //device handler used for recording capacitive image
        LocalDeviceHandler localDeviceHandler = new LocalDeviceHandler();
        localDeviceHandler.setLocalCapImgListener(new LocalCapImgListener() {
            @Override
            public void onLocalCapImg(final CapacitiveImageTS capImg) {
                runOnUiThread(new Runnable() {
                    @Override
                    public void run() {

                        if (recording() && capImg.isValidMatrix()) {
                            checkIfSomethingIsPlaced(capImg);
                            if (shapePlaced) {
                                recorder.writeToFile(capImg.toString());
                            }
                        }
/*                        if (recording() && sequence != null && capImg.isValidMatrix()) {
                            if (sequence.proceed(capImg.getFlattenedMatrix())) {
                                recorder.writeToFile(capImg.toString());
                            }
                        }*/
                    }
                });
            }
        });
        localDeviceHandler.startHandler();

        // fill the whole screen.
        RelativeLayout.LayoutParams params = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WRAP_CONTENT, RelativeLayout.LayoutParams.WRAP_CONTENT);
        params.addRule(RelativeLayout.ALIGN_PARENT_LEFT, RelativeLayout.TRUE);
        params.addRule(RelativeLayout.ALIGN_PARENT_TOP, RelativeLayout.TRUE);
        params.addRule(RelativeLayout.ALIGN_PARENT_RIGHT, RelativeLayout.TRUE);
        params.addRule(RelativeLayout.ALIGN_PARENT_BOTTOM, RelativeLayout.TRUE);

        drawView = new DrawView(this);
        drawView.setVisibility(View.GONE);

        recordingLayout.removeAllViews();
        recordingLayout.addView(drawView, params);


        findViewById(R.id.start_button).setOnTouchListener(mStartButtonTouchListener);

        showSystemUI();

    }

    private boolean shapePlaced = false;

    private void checkIfSomethingIsPlaced(CapacitiveImageTS capImg) {
        int[] values = capImg.getFlattenedMatrix();
        int sum = 0;
        for (int i = 0; i < values.length; i++) {
            sum += values[i];
        }

        Log.d("SUM", "sum is " + sum);

        if (sum > 200) { //sanity check of sum of capacitances is enough
            if (!shapePlaced && mTcpClient != null && mTcpClient.isConnected()) {
                    mTcpClient.sendMessage("ShapePlaced");
            }
            shapePlaced = true;
        } else {
            if (shapePlaced && mTcpClient != null && mTcpClient.isConnected()) {
                    mTcpClient.sendMessage("ShapeRemoved");
            }
            shapePlaced = false;
        }
    }

    private void hideSystemUI() {
        recordingLayout.setSystemUiVisibility(
                View.SYSTEM_UI_FLAG_IMMERSIVE
                        | View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                        | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                        | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_FULLSCREEN
        );

        ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            actionBar.hide();
        }
    }

    @SuppressLint("InlinedApi")
    private void showSystemUI() {
        recordingLayout.setSystemUiVisibility(
                View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                        | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
        );

        ActionBar actionBar = getSupportActionBar();
        if (actionBar != null) {
            actionBar.show();
        }
    }

    private void startRecording() {

        int minTimeDifference = 125;
        final int stepsPerSecond = 1000 / minTimeDifference;

        /*
        sequence = new Sequence(this, minTimeDifference, new ArrayList<ISegment>(){{
            add(new MovingCircleSegment(11 * stepsPerSecond, drawView, new int[]{300,300},new int[]{700,1600}));
            add(new MovingCircleSegment(11 * stepsPerSecond, drawView, new int[]{700,1600},new int[]{700,300}));
            add(new MovingCircleSegment(11 * stepsPerSecond, drawView, new int[]{700,300},new int[]{300,1600}));
            add(new StopSegment(2 * stepsPerSecond, drawView));
            add(new StandingCircleSegment(5 * stepsPerSecond, drawView, new int[]{randomInRange(300, 700), randomInRange(300, 1600)}));
            add(new StopSegment(2 * stepsPerSecond, drawView));
            add(new StandingCircleSegment(5 * stepsPerSecond, drawView, new int[]{randomInRange(300, 700), randomInRange(300, 1600)}));
            add(new StopSegment(2 * stepsPerSecond, drawView));
            add(new StandingCircleSegment(5 * stepsPerSecond, drawView, new int[]{randomInRange(300, 700), randomInRange(300, 1600)}));
            add(new StopSegment(2 * stepsPerSecond, drawView));
            add(new StandingCircleSegment(5 * stepsPerSecond, drawView, new int[]{randomInRange(300, 700), randomInRange(300, 1600)}));
            add(new ThankYouSegment(3 * stepsPerSecond, drawView));
        }});
        */

/*        sequence = new Sequence(this, minTimeDifference, new ArrayList<ISegment>() {{
            add(new AlwaysOnSegment());
        }});
*/
        //recorder = new Recorder(this, shapeNameField.getSelectedItem().toString(), participantField.getText().toString());
        recorder = new Recorder(this, "empty", "empty");

        drawView.setVisibility(View.VISIBLE);
        hideSystemUI();
        controls.setVisibility(View.GONE);


        new ConnectTask().execute("");
    }

    private int randomInRange(int start, int end) {
        return ThreadLocalRandom.current().nextInt(start, end + 1);
    }

    public void stopRecording() {
        recorder.saveRecording();
        recorder = null;
        sequence = null;

        drawView.setVisibility(View.GONE);
        showSystemUI();
        controls.setVisibility(View.VISIBLE);

        if (mTcpClient != null) {
            if (mTcpClient.isConnected()) {
                mTcpClient.sendMessage("StopRecording");
            }
            mTcpClient.stopClient();
        }
    }

    private boolean recording() {
        return recorder != null;
    }


    TcpClient mTcpClient;
    String ip = "192.168.0.105";
    int port = 9999;

    public class ConnectTask extends AsyncTask<String, String, TcpClient> {

        @Override
        protected TcpClient doInBackground(String... message) {

            //we create a TCPClient object
            mTcpClient = new TcpClient(ip, port,
                    new TcpClient.OnMessageReceived() {
                        @Override
                        //here the messageReceived method is implemented
                        public void messageReceived(String message) {
                            //this method calls the onProgressUpdate
                            publishProgress(message);
                        }
                    },
                    new TcpClient.OnConnected() {
                        @Override
                        public void connected() {
                            if (mTcpClient != null && mTcpClient.isConnected()) {
                                mTcpClient.sendMessage("StartRecording");
                            }
                        }
                    }
            );
            try {
                mTcpClient.run();
            } catch (Exception e) {
                Log.d("server", e.getMessage());


                runOnUiThread(new Runnable() {

                    @Override
                    public void run() {
                        recordingLayout.setBackgroundColor(Color.RED);
                        Toast.makeText(getApplicationContext(), "Failed to connect to server!", Toast.LENGTH_LONG);
                    }
                });

            }

            return null;
        }

        @Override
        protected void onProgressUpdate(String... values) {
            super.onProgressUpdate(values);
            //response received from server
            Log.d("test", "response " + values[0]);
            //process server response here....

        }
    }
}
