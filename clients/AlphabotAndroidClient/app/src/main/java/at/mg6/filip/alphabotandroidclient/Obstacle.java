package at.mg6.filip.alphabotandroidclient;

public class Obstacle {
    private short x;
    private short y;
    private int width;
    private int height;
    private int id;
    private boolean hasId;

    public Obstacle(short x, short y, int width, int height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        hasId = false;
    }

    public Obstacle(short x, short y, int width, int height, int id) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.id = id;
        hasId = true;
    }

    public void setX(short x) {
        this.x = x;
    }

    public short getX() {
        return x;
    }

    public void setY(short y) {
        this.y = y;
    }

    public short getY() {
        return y;
    }

    public void setWidth(int width) {
        this.width = width;
    }

    public int getWidth() {
        return width;
    }

    public void setHeight(int height) {
        this.height = height;
    }

    public int getHeight() {
        return height;
    }

    public void setId(int id) {
        this.id = id;
        hasId = true;
    }

    public int getId() {
        return id;
    }

    public boolean hasId() {
        return hasId;
    }

    public String toString() {
        return x + ";" + y + ";" + width + ";" + height;
    }
}
