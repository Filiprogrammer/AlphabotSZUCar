package at.mg6.filip.alphabotandroidclient;

public class Obstacle {
    private short x;
    private short y;
    private short width;
    private short height;
    private short id;
    private boolean hasId;

    public Obstacle(short x, short y, short width, short height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        hasId = false;
    }

    public Obstacle(short x, short y, short width, short height, short id) {
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

    public void setWidth(short width) {
        this.width = width;
    }

    public short getWidth() {
        return width;
    }

    public void setHeight(short height) {
        this.height = height;
    }

    public short getHeight() {
        return height;
    }

    public void setId(short id) {
        this.id = id;
        hasId = true;
    }

    public short getId() {
        return id;
    }

    public boolean hasId() {
        return hasId;
    }

    public String toString() {
        return x + ";" + y + ";" + width + ";" + height;
    }
}
