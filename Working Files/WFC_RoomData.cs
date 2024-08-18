using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class WFC_RoomData : ScriptableObject
{
    public Tile[] singleMap;
    public Vector2Int roomSize;
    public WFC_DataAsset dataAsset;

    public void SetMap(Tile[,] map, Vector2Int size)
    {
        roomSize = size;
        singleMap = new Tile[roomSize.x * roomSize.y];
        int index = 0;

        for (int x = 0; x < roomSize.x; x++)
        {
            for (int y = 0; y < roomSize.y; y++)
            {
                index = (roomSize.x * y) + x;
                singleMap[index] = map[x, y];
            }
        }
    }

    // it's starting 12 tiles forward, than wrapping around.

    public Tile[,] GetMap ()
    {
        Tile[,] tiles = new Tile[roomSize.x, roomSize.y];
        int index = 0;

        for (int x = 0; x < roomSize.x; x++)
        {
            for (int y = 0; y < roomSize.y; y++)
            {
                index = (roomSize.x * y) + x;
                tiles[x, y] = singleMap[index];
            }
        }

        return tiles;
    }
}
 