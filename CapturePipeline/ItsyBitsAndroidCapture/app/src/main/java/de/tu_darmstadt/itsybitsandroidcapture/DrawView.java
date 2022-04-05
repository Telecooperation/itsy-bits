package de.tu_darmstadt.itsybitsandroidcapture;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Path;
import android.view.View;


/**
 * Visualizes the capacitive image and the result of the palm classifier.
 *
 * @author Huy
 */
public class DrawView extends View {

    int mode;
    int circleX;
    int circleY;
    Path path;

    //Fallback
    //just assume phone dimensions are 1920x1080p
    public DrawView(Context context) {
        super(context);

        mode = 0;
        circleX = 0;
        circleY = 0;
    }

    //gets called by the device handler every time a new capacitive image is captured
    public void updatePath(Path p) {
        path = p;
        //forces redraw
        invalidate();
    }

    //gets called by the device handler every time a new capacitive image is captured
    public void updateCircle(int x, int y) {
        circleX = x;
        circleY = y;
        //forces redraw
        invalidate();
    }

    public void updateMode(int mode)
    {
        this.mode = mode;
        //forces redraw
        invalidate();
    }
    @Override
    public void onDraw(Canvas canvas) {
        super.onDraw(canvas);

        System.out.println(mode);
        switch (mode)
        {
            case 0: //do nothing
                break;
            case 1: //draw circle
                drawCircle(canvas, circleX, circleY);
                break;
            case 2: //draw still screen
                drawStillScreen(canvas, circleX, circleY);
                break;
            case 3: //draw stop screen
                drawStopScreen(canvas);
                break;
            case 4: //draw stop screen
                drawThankYouScreen(canvas);
            case 5:
                drawPolygon(canvas, path);
                break;
        }

    }

    private void drawPolygon(Canvas canvas, Path path) {

        Paint paint = new Paint();

        canvas.drawColor(Color.WHITE);

        paint.setStyle(Paint.Style.STROKE);
        paint.setStrokeWidth(5);
        paint.setColor(Color.rgb(216,27,96));
        canvas.drawPath(path, paint);

        paint.setStyle(Paint.Style.FILL);
        paint.setTextSize(70);
        paint.setTextAlign(Paint.Align.CENTER);
        canvas.drawText("Please follow the circle", canvas.getWidth()/2,800, paint);


    }
    private void drawCircle(Canvas canvas, int x, int y)
    {
        Paint paint = new Paint();

        canvas.drawColor(Color.WHITE);

        paint.setStyle(Paint.Style.STROKE);
        paint.setStrokeWidth(5);
        paint.setColor(Color.rgb(216,27,96));
        canvas.drawCircle(x, y, 250, paint);

        paint.setStyle(Paint.Style.FILL);
        paint.setTextSize(70);
        paint.setTextAlign(Paint.Align.CENTER);
        canvas.drawText("Please follow the circle", canvas.getWidth()/2,800, paint);
    }

    private void drawStopScreen(Canvas canvas)
    {
        Paint paint = new Paint();

        canvas.drawColor(Color.RED);

        paint.setColor(Color.WHITE);
        paint.setStyle(Paint.Style.FILL);
        paint.setTextSize(70);
        paint.setTextAlign(Paint.Align.CENTER);
        canvas.drawText("Please remove tangible", canvas.getWidth()/2,800, paint);
    }

    private void drawStillScreen(Canvas canvas, int x, int y)
    {
        Paint paint = new Paint();

        canvas.drawColor(Color.WHITE);

        paint.setColor(Color.rgb(216,27,96));
        paint.setStyle(Paint.Style.STROKE);
        paint.setStrokeWidth(5);

        canvas.drawCircle(x, y, 250, paint);

        paint.setStyle(Paint.Style.FILL);
        paint.setTextSize(70);
        paint.setTextAlign(Paint.Align.CENTER);
        canvas.drawText("Please hold tangible still", canvas.getWidth()/2,800, paint);
    }

    private void drawThankYouScreen(Canvas canvas)
    {
        Paint paint = new Paint();

        canvas.drawColor(Color.WHITE);

        paint.setColor(Color.rgb(216,27,96));
        paint.setStyle(Paint.Style.FILL);
        paint.setTextSize(90);
        paint.setTextAlign(Paint.Align.CENTER);
        canvas.drawText("Thank You", canvas.getWidth()/2,800, paint);
    }
}

