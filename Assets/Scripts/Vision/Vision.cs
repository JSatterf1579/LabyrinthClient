using UnityEngine;
using System.Collections.Generic;

public static class Vision {
    
    public static IEnumerable<Tile> GetVisibleTiles(int xPos, int yPos, int vision) {
        HashSet<Tile> tiles = new HashSet<Tile>();
        ForEachVisibleTileInRange(xPos, yPos, vision, t => tiles.Add(t));
        return tiles;
    }

    public static void ForEachVisibleTileInRange(int xPos, int yPos, int vision, System.Action<Tile> func) {
        ForEachTileInRange(xPos, yPos, vision, t => {
            if (LineOfSight.Test(xPos, yPos, t.XPos, t.YPos)) {
                func(t);
            }
        });
    }

    public static void ForEachTileInRange(int xPos, int yPos, int vision, System.Action<Tile> func) {
        var map = Map.Current;
        var visionSq = vision * vision;
        for (int xd = -vision; xd < xPos + vision; xd++) {
            int x = xd + xPos;
            int minY = yPos - Mathf.RoundToInt(Mathf.Sqrt(visionSq - xd * xd));
            int maxY = yPos + Mathf.RoundToInt(Mathf.Sqrt(visionSq - xd * xd));
            for (int y = minY; y <= maxY; y++) {
                if (map.CheckCoordinates(x, y)) {
                    Tile t = map.GetTileAtPosition(x, y);
                    func(t);
                }
            }
        }
    }
}
