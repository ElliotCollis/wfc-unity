using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using System.Reflection;

public class WFC_Manager
{
    //public WFC_ManagerData data;


    // public string compressedString;

    // string leftRow;
    // string rightRow;
    // string topRow;
    //  string bottomRow;

    //public List<WFC_Module> finalModules = new List<WFC_Module>();

    public WFC_SimpleTiled simpleTiled;
    public Vector2Int mapSize;
    public WFC_DataAsset dataAsset;
    public Tile[,] map;
    public WFC_Module[,] wfcMap; // each of our moduels in the map and their current state.

    public WFC_Manager(WFC_DataAsset data, Vector2Int size, Tile[,] newMap)
    {
        dataAsset = data;
        mapSize = size;
        map = newMap;
        MapToWfcMap();
        simpleTiled = new WFC_SimpleTiled(wfcMap);
    }

    public Tile[,] SolveMap ()
    {
        // solve the walls first since we know we want them to be walls.
        SolveWallTiles();

        // get a list of all the remaining tiles to solve.
        List<Vector2Int> unsolvedTileLocations = new List<Vector2Int>();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y] == null)
                {
                    unsolvedTileLocations.Add(new Vector2Int(x, y));
                }
            }
        }

        // call solve on each one in a random order.
        foreach (Vector2Int tileLocation in unsolvedTileLocations.OrderBy(_ => Random.value))
        {
            SolveNode(tileLocation);
        }

        // remove the empty tiles from the map.
        RemoveEmptyTiles();

        return map;
    }

    public Tile[,] SolveWallTiles ()
    {
        List<Vector2Int> wallTileLocations = new List<Vector2Int>();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y] == dataAsset.wallTile)
                {
                    map[x, y] = null;
                    wallTileLocations.Add(new Vector2Int(x, y));
                }
            }
        }

        foreach (Vector2Int tileLocation in wallTileLocations.OrderBy(_ => Random.value)) // solve in a random order
        {
            SolveNode(tileLocation);
        }

        return map;
    }

    public Tile[,] RemoveEmptyTiles()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y] == dataAsset.emptyTile)
                {
                    map[x, y] = null;
                }
            }
        }

        return map;
    }

    void SolveNode (Vector2Int location)
    {
        // check node hasn't been solved first so we don't run in to errors.
        int x = location.x;
        int y = location.y;

        if (wfcMap[x, y].finalState != null)
            return;

        // pick an outcome for the node, call propergate on the wfc.

        //Debug.Log(wfcMap[x, y].possibleModuleState.Count);

        WFC_Tile randomTile = wfcMap[x, y].possibleModuleState[Random.Range(0, wfcMap[x, y].possibleModuleState.Count)];

       

        wfcMap[x, y].CollapseNode(randomTile);

        // after propergation, call wfcmap to map.
        simpleTiled.StartPropagation(wfcMap[x, y], UpdateTiles);

        WfcMapToMap();
    }

    void MapToWfcMap ()
    {
        wfcMap = new WFC_Module[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // if we're in an empty space set a new module otherwise set the tile.
               // WFC_Module module = map[x,y] == null || map[x,y] == dataAsset.wallTile ? new WFC_Module(x,y,dataAsset.tiles) : new WFC_Module(x,y, dataAsset.GetWfcTile(map[x, y]));
               // let's try just setting everything first.

                WFC_Module module = new WFC_Module(x, y, dataAsset.tiles);
                wfcMap[x, y] = module;
            }
        }

        // make sure the wall tiles cannot be an empty tile.
        WFC_Tile emptyTile = dataAsset.GetWfcTile(dataAsset.emptyTile);

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y] == dataAsset.wallTile)
                {
                    wfcMap[x, y].possibleModuleState.Remove(emptyTile);
                }
            }
        }

        // collapse empty or not set tiles.
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y] == dataAsset.emptyTile || (map[x,y] != null && map[x,y] != dataAsset.wallTile))
                {
                    wfcMap[x, y].CollapseNode(dataAsset.GetWfcTile(map[x, y]));
                }
            }
        }

        //collapse all the empty nodes.
        //for (int x = 0; x < mapSize.x; x++)
        //{
        //    for (int y = 0; y < mapSize.y; y++)
        //    {
        //        if (map[x, y] == dataAsset.emptyTile)
        //        {
        //            wfcMap[x, y].CollapseNode(emptyTile);
        //        }
        //    }
        //}


    }

    void WfcMapToMap ()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // translate each state of the wfcMap to our map if it has picked a final tile.
                if(wfcMap[x,y].finalState != null)
                {
                    map[x, y] = wfcMap[x, y].finalState.myTile;
                }
            }
        }
    }

    /*
    public void ProcessTiles(int width, int height)
    {
    // creating all the nodes.
        List<WFC_DataNode> t_Nodes = new List<WFC_DataNode>();

        for (int i = 0; i < data.tilesToPickFrom.Length; i++)
        {
            t_Nodes.Add(new WFC_DataNode(data.tilesToPickFrom[i]));
        }

        data.dataNodes = t_Nodes.ToArray();

        // needs all nodes created first. Process the nodes.
        // calculate the neighbours.
        // we need to do this for each sprite to know possible neighbours, but there's many errors here

        foreach (WFC_DataNode node in data.dataNodes)
        {
            node.CalculatePossibleNeighbours(data.dataNodes);
        }


        // generate the map and fill it with new moduels.
        data.map = new WFC_Module[width,height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                WFC_Module t_mod = new WFC_Module(x, y, data.dataNodes);
                data.map[x, y] = t_mod;
            } 
        }
    }

    public WFC_Module GetModule (int x, int y)
    {
        return data.map[x, y];
    }

    public void selectNode(WFC_Module module, WFC_DataNode dataNode) // change of state, pass in the datanode selected and module effected.
    {

        // module, we set the datanode as the only possible state
        module.CollapseNode(dataNode);
        finalModules.Add(module);

        for (int i = 0; i < module.possibleModuleState.Count; i++)
        {
            if (module.possibleModuleState[i] != dataNode) module.possibleModuleState.RemoveAt(i);
        }

        // we call propergate on simpletiled.
        data.simpleTiled.StartPropagation(data.map, module, UpdateTiles);

        if (finalModules.Count == data.map.Length)
        {
            Debug.Log("All squares have been filled");

            StoreColours();
        }
    }
    void StoreColours()
    {
        //String split out to reflect the outer colour points.
        for (int i = 0; i < finalModules.Count; i++)
        {
            WFC_Module item = finalModules[i];
            //Testing deconstruction
            string[] colorList = item.finalNodeColors.Split('.');

            if (item.x == 0)
                if (item.y == 0) { leftRow += colorList[0] + "/" + colorList[3] + "/"; }
                else if (item.y < 3) { leftRow += colorList[0] + "/"; }
                else if (item.y == 3) { leftRow += colorList[0] + "/" + colorList[1]; }


            if (item.x == 3)
                if (item.y == 0) { rightRow += colorList[2] + "/" + colorList[3] + "/"; }
                else if (item.y < 3) { rightRow += colorList[2] + "/"; }
                else if (item.y == 3) { rightRow += colorList[2] + "/" + colorList[1]; }

            if (item.y == 0)
                if (item.x == 0) { bottomRow += colorList[0] + "/" + colorList[3] + "/"; }
                else if (item.x < 3) { bottomRow += colorList[3] + "/"; }
                else if (item.x == 3) { bottomRow += colorList[3] + "/" + colorList[2]; }

            if (item.y == 3)
                if (item.x == 0) { topRow += colorList[0] + "/" + colorList[1] + "/"; }
                else if (item.x < 3) { topRow += colorList[1] + "/"; }
                else if (item.x == 3) { topRow += colorList[1] + "/" + colorList[2]; }
        }
        compressedString = (leftRow + "?" + topRow + "?" + rightRow + "?" + bottomRow + "?");

    }

    */

    public void UpdateTiles(List<WFC_Module> TilesToUpdate)
    {
        foreach(WFC_Module module in TilesToUpdate)
        {
            module.UpdateTile();
        }
    }
    

    public class WFC_ManagerData
    {
        public Sprite[] tilesToPickFrom;
        [SerializeReference] public WFC_DataNode[] dataNodes;
        [SerializeReference] public WFC_Module[,] map;

        public WFC_SimpleTiled simpleTiled;
    }
}
