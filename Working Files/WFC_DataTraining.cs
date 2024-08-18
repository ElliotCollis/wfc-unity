using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;

[ExecuteInEditMode]
public class WFC_DataTraining : MonoBehaviour
{
    public string assetName = "Basic_Training_Data"; // add ".asset"
    public string saveLocation = "Assets/TileMaps/Biomes/";
    public string wallTileName = "WallTile";
    public string emptyTileName = "EmptyTile";
    public Vector2Int startLocation = new Vector2Int(0, 0);
    public Vector2Int trainingSize = new Vector2Int(10, 10);
    //biome Type

    public Tilemap tilemap;
    public bool trainingInProgress = false;

    public void TrainData ()
    {
        Debug.Log("Training in progress.");
        trainingInProgress = true;
        string path = saveLocation + assetName + ".asset";

        if (AssetDatabase.FindAssets(path) != null)
        {
            print("asset already exsist, writing over it.");
        }

        WFC_DataAsset asset = ScriptableObject.CreateInstance<WFC_DataAsset>();

        // check save location exsists.
        // check if we have an asset already.
        // create the new asset and train the data.

        Vector3Int tilePosition = Vector3Int.zero;

        for (int x = 0; x < trainingSize.x; x++)
        {
            tilePosition.x = startLocation.x + x;

            for (int y = 0; y < trainingSize.y; y++)
            {
                tilePosition.y = startLocation.y + y;

                if (!tilemap.HasTile(tilePosition)) // if the tile is empty go to next tile.
                {
                    continue;
                }

                Tile tile = tilemap.GetTile<Tile>(tilePosition);

                bool createNewTile = true;
                int tileRef = 0;

                // check if we have the tile in our asset already. Update that if we do.
                for (int i = 0; i < asset.tiles.Count; i++)
                {
                    if (asset.tiles[i].myTile == tile)
                    {
                        tileRef = i;
                        createNewTile = false;
                        break;
                    }
                }

                // check each neighbour and add it to our references if we don't already have it
                Tile neighbourTile;
                WFC_Tile wfc_Tile = createNewTile ? new WFC_Tile(tile) : asset.tiles[tileRef];

                if (createNewTile) asset.tiles.Add(wfc_Tile);   // Add the tile to the list if we created it.
                Vector3Int neighbourPosition = Vector3Int.zero;

                if (x - 1 >= 0) // left
                {
                    neighbourPosition.x = tilePosition.x - 1;
                    neighbourPosition.y = tilePosition.y;

                    if (tilemap.HasTile(neighbourPosition))
                    {
                        neighbourTile = tilemap.GetTile<Tile>(neighbourPosition);
                        if (!wfc_Tile.leftTiles.Contains(neighbourTile))
                        {
                            wfc_Tile.leftTiles.Add(neighbourTile);
                        }
                    }
                }

                if (x + 1 < trainingSize.x) // right
                {
                    neighbourPosition.x = tilePosition.x + 1;
                    neighbourPosition.y = tilePosition.y;

                    if (tilemap.HasTile(neighbourPosition))
                    {
                        neighbourTile = tilemap.GetTile<Tile>(neighbourPosition);
                        if (!wfc_Tile.rightTiles.Contains(neighbourTile))
                        {
                            wfc_Tile.rightTiles.Add(neighbourTile);
                        }
                    }
                }

                if (y - 1 >= 0) // up
                {
                    neighbourPosition.x = tilePosition.x;
                    neighbourPosition.y = tilePosition.y - 1;

                    if (tilemap.HasTile(neighbourPosition))
                    {
                        neighbourTile = tilemap.GetTile<Tile>(neighbourPosition);
                        if (!wfc_Tile.downTiles.Contains(neighbourTile))
                        {
                            wfc_Tile.downTiles.Add(neighbourTile);
                        }
                    }
                }

                if (y + 1 < trainingSize.y) // down
                {
                    neighbourPosition.x = tilePosition.x;
                    neighbourPosition.y = tilePosition.y + 1;

                    if (tilemap.HasTile(neighbourPosition))
                    {
                        neighbourTile = tilemap.GetTile<Tile>(neighbourPosition);
                        if (!wfc_Tile.upTiles.Contains(neighbourTile))
                        {
                            wfc_Tile.upTiles.Add(neighbourTile);
                        }
                    }
                }
            }
        }

        foreach (WFC_Tile tile in asset.tiles)
        {
            //tile.populateNeighbours(asset);

            if (tile.myTile.name == wallTileName) asset.wallTile = tile.myTile;
            if (tile.myTile.name == emptyTileName) asset.emptyTile = tile.myTile;
        }

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets(); // this is one area that might be causing an infinate loop. I might need to add isDirty flags etc. 

        trainingInProgress = false;

        Debug.Log("Training finished."); 
    }
}
