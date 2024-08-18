using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WFC_DataAsset : ScriptableObject
{
    // our data object with all the trained data for the WFC tilemap.

    public List<WFC_Tile> tiles;

    // currently setting manually.
    public Tile wallTile;
    public Tile emptyTile;

    public WFC_DataAsset ()
    {
        tiles = new List<WFC_Tile>();
    }

    public WFC_Tile GetWfcTile (Tile tile)
    {
        foreach(WFC_Tile wfc_Tile in tiles)
        {
            if (wfc_Tile.myTile == tile) return wfc_Tile;
        }

        return null;
    }
}

[Serializable]
public class WFC_Tile
{
    public Tile myTile;

    public List<Tile> leftTiles;
    public List<Tile> rightTiles;
    public List<Tile> upTiles;
    public List<Tile> downTiles;

    public WFC_Tile(Tile tileToSet)
    {
        myTile = tileToSet;

        leftTiles = new List<Tile>();
        rightTiles = new List<Tile>();
        upTiles = new List<Tile>();
        downTiles = new List<Tile>();
    }

    public List<Tile> SingleNeighbourSide(int sideID) // downtiles got a null reference....
    {
        switch (sideID)
        {
            case 0:
                return leftTiles;
            case 1:
                return upTiles;
            case 2:
                return rightTiles;
            case 3:
                return downTiles;
            default:
                break;
        }

        return new List<Tile>();
    }
}
