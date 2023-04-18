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

    private float[] moveVertices(float[] vertices, float x, float y) {
        float[] new_points = new float[vertices.length];

        for (int i = 0; i < vertices.length / 2; ++i) {
            new_points[i * 2] = vertices[i * 2] + x;
            new_points[i * 2 + 1] = vertices[i * 2 + 1] + y;
        }

        return new_points;
    }

    private float[] rotateVertices(float[] vertices, float angle, float origin_x, float origin_y) {
        angle *= 3.14159265359 / 180;
        double cos_val = Math.cos(angle);
        double sin_val = Math.sin(angle);
        float[] new_points = new float[vertices.length];

        for (int i = 0; i < vertices.length / 2; ++i) {
            float x_old = vertices[i * 2];
            float y_old = vertices[i * 2 + 1];
            x_old -= origin_x;
            y_old -= origin_y;
            float x_new = (float) (x_old * cos_val - y_old * sin_val);
            float y_new = (float) (x_old * sin_val + y_old * cos_val);
            new_points[i * 2] = x_new + origin_x;
            new_points[i * 2 + 1] = y_new + origin_y;
        }

        return new_points;
    }

    private void drawCar(Canvas canvas) {
        int minX = Math.min(anchor0x, Math.min(anchor1x, Math.min(anchor2x, posX))) - 5;
        int minY = Math.min(anchor0y, Math.min(anchor1y, Math.min(anchor2y, posY))) - 5;
        int maxX = Math.max(anchor0x, Math.max(anchor1x, Math.max(anchor2x, posX))) + 5;
        int maxY = Math.max(anchor0y, Math.max(anchor1y, Math.max(anchor2y, posY))) + 5;
        float zoom = Math.min(canvas.getWidth() / (float)(maxX - minX), canvas.getHeight() / (float)(maxY - minY));

        float[] car_base_vertices = { -4.5f, -8, 0, -10, 4.5f, -8, 2, -8, 2, -4, 6, -2.5f, 2.5f, -1, 2.5f, 2, 6.5f, 6, 6.5f, 18, 3, 18, 2, 21, 7.5f, 21, 7.5f, 26.5f, -7.5f, 26.5f, -7.5f, 21, -2, 21, -3, 18, -6.5f, 18, -6.5f, 6, -2.5f, 2, -2.5f, -1, -6, -2.5f, -2, -4, -2, -8, -4.5f, -8 };
        float[] car_wheel_vertices = { -1.8f, -3.8f, 1.8f, -3.8f, 1.8f, 3.8f, -1.8f, 3.8f, -1.8f, -3.8f };

        for (int i = 0; i < car_base_vertices.length; ++i)
            car_base_vertices[i] *= zoom;

        for (int i = 0; i < car_wheel_vertices.length; ++i)
            car_wheel_vertices[i] *= zoom;

        car_base_vertices = moveVertices(car_base_vertices, 0, -3f * zoom);
        car_base_vertices = rotateVertices(car_base_vertices, dir + 90, 0, 0);
        car_base_vertices = moveVertices(car_base_vertices, (posX - minX) * zoom, (posY - minY) * zoom);

        float[] car_wheel_left_front_moved = moveVertices(car_wheel_vertices, -8f * zoom, -5f * zoom);
        float[] car_wheel_left_front = rotateVertices(car_wheel_left_front_moved, dir + 90, 0, 0);
        car_wheel_left_front = moveVertices(car_wheel_left_front, (posX - minX) * zoom, (posY - minY) * zoom);

        float[] car_wheel_right_front_moved = moveVertices(car_wheel_vertices, 8f * zoom, -5f * zoom);
        float[] car_wheel_right_front = rotateVertices(car_wheel_right_front_moved, dir + 90, 0, 0);
        car_wheel_right_front = moveVertices(car_wheel_right_front, (posX - minX) * zoom, (posY - minY) * zoom);

        float[] car_wheel_left_back_moved = moveVertices(car_wheel_vertices, -8.5f * zoom, 13.5f * zoom);
        float[] car_wheel_left_back = rotateVertices(car_wheel_left_back_moved, dir + 90, 0, 0);
        car_wheel_left_back = moveVertices(car_wheel_left_back, (posX - minX) * zoom, (posY - minY) * zoom);

        float[] car_wheel_right_back_moved = moveVertices(car_wheel_vertices, 8.5f * zoom, 13.5f * zoom);
        float[] car_wheel_right_back = rotateVertices(car_wheel_right_back_moved, dir + 90, 0, 0);
        car_wheel_right_back = moveVertices(car_wheel_right_back, (posX - minX) * zoom, (posY - minY) * zoom);

        canvas.drawLines(car_base_vertices, 0, car_base_vertices.length, paint);
        canvas.drawLines(car_base_vertices, 2, car_base_vertices.length-4, paint);

        canvas.drawLines(car_wheel_left_front, 0, car_wheel_left_front.length-2, paint);
        canvas.drawLines(car_wheel_left_front, 2, car_wheel_left_front.length-2, paint);

        canvas.drawLines(car_wheel_right_front, 0, car_wheel_right_front.length-2, paint);
        canvas.drawLines(car_wheel_right_front, 2, car_wheel_right_front.length-2, paint);

        canvas.drawLines(car_wheel_left_back, 0, car_wheel_left_back.length-2, paint);
        canvas.drawLines(car_wheel_left_back, 2, car_wheel_left_back.length-2, paint);

        canvas.drawLines(car_wheel_right_back, 0, car_wheel_right_back.length-2, paint);
        canvas.drawLines(car_wheel_right_back, 2, car_wheel_right_back.length-2, paint);
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
        drawCar(canvas);
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

    public Obstacle getObstacle(int id) {
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
