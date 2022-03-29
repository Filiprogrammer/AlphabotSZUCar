package at.mg6.filip.alphabotandroidclient;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.util.AttributeSet;
import android.view.MotionEvent;
import android.view.View;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

public class LPSView extends View implements View.OnTouchListener {
    private Bitmap bitmap;
    LPSViewListener listener;
    private Paint paint;
    private int posX = 0;
    private int posY = 0;
    private float dir = 0;
    private Map<Integer, Integer> obstacleDistances = new HashMap<>();
    private Obstacle selectedObstacle = null;
    private int targetX = 0;
    private int targetY = 0;
    private byte[] path = null;
    private List<Obstacle> obstacles = new ArrayList<>();

    public short anchor0x =   0;
    public short anchor0y =   0;
    public short anchor1x = 100;
    public short anchor1y =   0;
    public short anchor2x =   0;
    public short anchor2y = 100;

    public LPSView(Context context, AttributeSet attrs) {
        super(context, attrs);
        try {
            listener = (LPSViewListener) context;
        } catch (ClassCastException e) {
            throw new ClassCastException(context.toString() + " must implement LPSViewListener");
        }

        paint = new Paint();
        paint.setAntiAlias(false);
        paint.setColor(Color.RED);
        paint.setStyle(Paint.Style.STROKE);
        paint.setStrokeJoin(Paint.Join.ROUND);
        paint.setStrokeWidth(4f);

        setOnTouchListener(this);
    }

    @Override
    protected void onSizeChanged(int w, int h, int oldw, int oldh) {
        super.onSizeChanged(w, h, oldw, oldh);

        bitmap = Bitmap.createBitmap(w, h, Bitmap.Config.ARGB_8888);
    }

    @Override
    protected void onDraw(Canvas canvas) {
        super.onDraw(canvas);

        int minX = Math.min(anchor0x, Math.min(anchor1x, Math.min(anchor2x, posX))) - 5;
        int minY = Math.min(anchor0y, Math.min(anchor1y, Math.min(anchor2y, posY))) - 5;
        int maxX = Math.max(anchor0x, Math.max(anchor1x, Math.max(anchor2x, posX))) + 5;
        int maxY = Math.max(anchor0y, Math.max(anchor1y, Math.max(anchor2y, posY))) + 5;

        float zoom = Math.min(canvas.getWidth() / (float)(maxX - minX), canvas.getHeight() / (float)(maxY - minY));

        paint.setAlpha(128);
        paint.setColor(Color.GRAY);
        canvas.drawRect(0, 0, canvas.getWidth(), canvas.getHeight(), paint);

        for (Obstacle obstacle : obstacles) {
            if (selectedObstacle == obstacle)
                paint.setColor(Color.GREEN);
            else if (obstacle.hasId())
                paint.setColor(Color.RED);
            else
                paint.setColor(Color.GRAY);

            int obstacleX = obstacle.getX();
            int obstacleY = obstacle.getY();
            int obstacleW = obstacle.getWidth();
            int obstacleH = obstacle.getHeight();
            paint.setAlpha(64);
            paint.setStyle(Paint.Style.FILL);
            canvas.drawRect((obstacleX - minX) * zoom, (obstacleY - minY) * zoom, (obstacleX + obstacleW - minX) * zoom, (obstacleY + obstacleH - minY) * zoom, paint);
            paint.setAlpha(192);
            paint.setStyle(Paint.Style.STROKE);
            canvas.drawRect((obstacleX - minX) * zoom, (obstacleY - minY) * zoom, (obstacleX + obstacleW - minX) * zoom, (obstacleY + obstacleH - minY) * zoom, paint);
        }

        paint.setColor(Color.YELLOW);
        canvas.drawRect((anchor0x - 5 - minX) * zoom, (anchor0y - 5 - minY) * zoom, (anchor0x + 5 - minX) * zoom, (anchor0y + 5 - minY) * zoom, paint);
        canvas.drawRect((anchor1x - 5 - minX) * zoom, (anchor1y - 5 - minY) * zoom, (anchor1x + 5 - minX) * zoom, (anchor1y + 5 - minY) * zoom, paint);
        canvas.drawRect((anchor2x - 5 - minX) * zoom, (anchor2y - 5 - minY) * zoom, (anchor2x + 5 - minX) * zoom, (anchor2y + 5 - minY) * zoom, paint);
        paint.setColor(Color.MAGENTA);
        paint.setAlpha(255);
        canvas.drawText("1", (anchor0x - minX) * zoom, (anchor0y - minY) * zoom, paint);
        canvas.drawText("2", (anchor1x - minX) * zoom, (anchor1y - minY) * zoom, paint);
        canvas.drawText("3", (anchor2x - minX) * zoom, (anchor2y - minY) * zoom, paint);

        if (path != null && path.length >= 3) {
            int prevX = path[0] * 10;
            int prevY = path[1] * 10;
            int len = path[2] & 0x3F;

            for (int i = 0; i < len; ++i) {
                int val = ((path[2 + (i * 3 + 6) / 8] & 0xFF) >> ((i * 3 + 6) % 8)) & 7;

                if (((i * 3 + 6) % 8) > 5)
                    val |= ((path[3 + (i * 3 + 6) / 8] & 0xFF) << (8 - ((i * 3 + 6) % 8))) & 7;

                if (val == 4)
                    val = 8;

                String str = Integer.toString(val, 3);

                if (str.length() == 1)
                    str = "0" + str;

                int coord_x = prevX + ((str.charAt(0) - '0') - 1) * 10;
                int coord_y = prevY + ((str.charAt(1) - '0') - 1) * 10;

                canvas.drawLine((prevX - minX) * zoom, (prevY - minY) * zoom, (coord_x - minX) * zoom, (coord_y - minY) * zoom, paint);

                prevX = coord_x;
                prevY = coord_y;
            }
        }

        paint.setColor(Color.GREEN);
        canvas.drawCircle((posX - minX) * zoom, (posY - minY) * zoom, 15 * zoom, paint);
        canvas.drawLine((posX - minX) * zoom, (posY - minY) * zoom, (float) (posX + Math.cos(Math.toRadians(dir)) * 20 - minX) * zoom, (float) (posY + Math.sin(Math.toRadians(dir)) * 20 - minY) * zoom, paint);
        paint.setColor(Color.CYAN);

        for (Integer direction : obstacleDistances.keySet()) {
            int distance = obstacleDistances.get(direction);
            canvas.drawLine((posX - minX) * zoom, (posY - minY) * zoom, (float) (posX + Math.cos(Math.toRadians(dir + direction)) * distance - minX) * zoom, (float) (posY + Math.sin(Math.toRadians(dir + direction)) * distance - minY) * zoom, paint);
        }

        canvas.drawCircle((targetX - minX) * zoom, (targetY - minY) * zoom, 5 * zoom, paint);
    }

    public void updatePosition(int x, int y, boolean invalidate) {
        posX = x;
        posY = y;

        if (invalidate)
            invalidate();
    }

    public void updateDirection(float dir, boolean invalidate) {
        this.dir = dir;

        if (invalidate)
            invalidate();
    }

    public void updateObstacleSensorDistance(int direction, int distance, boolean invalidate) {
        obstacleDistances.put(direction, distance);

        if (invalidate)
            invalidate();
    }

    public void updatePath(byte[] vals) {
        this.path = vals;
        invalidate();
    }

    public void addObstacle(Obstacle obstacle) {
        obstacles.add(obstacle);
    }

    public void removeObstacle(Obstacle obstacle) {
        obstacles.remove(obstacle);
    }

    public void removeObstacle(short id) {
        for (Obstacle obstacle : obstacles) {
            if (id == obstacle.getId()) {
                obstacles.remove(obstacle);
                break;
            }
        }
    }

    public void removeObstacle(short x, short y) {
        Iterator<Obstacle> obstacleIterator = obstacles.iterator();

        while (obstacleIterator.hasNext()) {
            Obstacle obstacle = obstacleIterator.next();

            if (x >= obstacle.getX() && x <= obstacle.getX() + obstacle.getWidth() &&
                y >= obstacle.getY() && y <= obstacle.getY() + obstacle.getHeight()) {
                obstacleIterator.remove();
            }
        }
    }

    public List<Obstacle> getObstacles() {
        return Collections.unmodifiableList(obstacles);
    }

    public Obstacle getObstacle(short id) {
        for (Obstacle obstacle : obstacles)
            if (obstacle.hasId() && obstacle.getId() == id)
                return obstacle;

        return null;
    }

    public void clearObstacles() {
        obstacles.clear();
    }

    @Override
    public boolean onTouch(View view, MotionEvent motionEvent) {
        if (motionEvent.getAction() != MotionEvent.ACTION_DOWN)
            return true;

        float touchX = motionEvent.getX();
        float touchY = motionEvent.getY();
        int minX = Math.min(anchor0x, Math.min(anchor1x, Math.min(anchor2x, posX))) - 5;
        int minY = Math.min(anchor0y, Math.min(anchor1y, Math.min(anchor2y, posY))) - 5;
        int maxX = Math.max(anchor0x, Math.max(anchor1x, Math.max(anchor2x, posX))) + 5;
        int maxY = Math.max(anchor0y, Math.max(anchor1y, Math.max(anchor2y, posY))) + 5;
        float zoom = Math.min(bitmap.getWidth() / (float)maxX, bitmap.getHeight() / (float)maxY);

        for (Obstacle obstacle : obstacles) {
            if (!obstacle.hasId())
                continue;

            int obstacleX = obstacle.getX();
            int obstacleY = obstacle.getY();
            int obstacleW = obstacle.getWidth();
            int obstacleH = obstacle.getHeight();

            if (touchX >= (obstacleX - minX) * zoom && touchX <= (obstacleX + obstacleW - minX) * zoom &&
                touchY >= (obstacleY - minY) * zoom && touchY <= (obstacleY + obstacleH - minY) * zoom) {
                selectedObstacle = obstacle;
                invalidate();
                return true;
            }
        }

        selectedObstacle = null;

        targetX = (int)(minX + touchX / zoom);
        targetY = (int)(minY + touchY / zoom);
        listener.onLPSSetTargetPosition((short)targetX, (short)targetY);

        invalidate();
        return true;
    }

    public Obstacle getSelectedObstacle() {
        return selectedObstacle;
    }

    public interface LPSViewListener {
        void onLPSSetTargetPosition(short x, short y);
    }
}
