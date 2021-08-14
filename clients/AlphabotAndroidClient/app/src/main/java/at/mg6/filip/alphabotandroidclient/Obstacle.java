package at.mg6.filip.alphabotandroidclient;

public class Obstacle {
    private short x;
    private short y;
    private short width;
    private short height;

    public Obstacle(short x, short y, short width, short height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public short getX() {
        return x;
    }

    public short getY() {
        return y;
    }

    public short getWidth() {
        return width;
    }

    public short getHeight() {
        return height;
    }

    public String toString() {
        return x + ";" + y + ";" + width + ";" + height;
    }
}
