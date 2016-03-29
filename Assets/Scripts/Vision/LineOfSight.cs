using System;

public class LineOfSight {

    public static bool Test(Tile from, Tile to) {
        return Test(from.XPos, from.YPos, to.XPos, to.YPos);
    }

    public static bool Test(int x0, int y0, int x1, int y1) {
        var map = Map.Current;
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int x = x0;
        int y = y0;
        int n = 1 + dx + dy;
        int x_inc = (x1 > x0) ? 1 : -1;
        int y_inc = (y1 > y0) ? 1 : -1;
        int error = dx - dy;
        dx *= 2;
        dy *= 2;

        while (n > 0) {
            if(!map.CheckCoordinates(x, y)) {
                return false;
            }
            Tile t = map.GetTileAtPosition(x, y);
            if (t.IsObstacle && !(t.XPos == x1 && t.YPos == y1)) {
                return false;
            }

            if (error > 0) {
                x += x_inc;
                error -= dy;
            } else {
                y += y_inc;
                error += dx;
            }

            n--;
        }

        return true;
    }
}
