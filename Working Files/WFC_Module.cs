using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

[Serializable]
public class WFC_Module // live data
{
    public int x, y;
    public List<WFC_Tile> possibleModuleState;
    public WFC_Tile finalState = null;

    public WFC_Module(int _x, int _y, List<WFC_Tile> startStates)
    {
        x = _x;
        y = _y;
        possibleModuleState = new List<WFC_Tile>(startStates.Count);
        for (int i = 0; i < startStates.Count; i++)
        {
            possibleModuleState.Add(startStates[i]);
        }
    }

    public WFC_Module(int _x, int _y, WFC_Tile tile)
    {
        x = _x;
        y = _y;
        possibleModuleState = new List<WFC_Tile>();
        possibleModuleState.Add(tile);
        finalState = tile;
    }    

    public void CollapseNode (WFC_Tile node)
    {
        if (!possibleModuleState.Contains(node))
        {
            //Debug.Log("No nodes detected in possible module states. Number of avaliable nodes is " + possibleModuleState.Count.ToString()); // problems all around
            //Debug.Log("Module information: " + x.ToString() + ", " + y.ToString());
            return;
        }

        if (finalState == null)
        {
            possibleModuleState = new List<WFC_Tile>();
            possibleModuleState.Add(node);
            finalState = node;
            return;
        }

        Debug.Log("collapsing an already collapsed tile");
    }

    public void UpdatePossibleConnections (List<WFC_Tile> newConnections)
    {
        possibleModuleState = new List<WFC_Tile>(newConnections.Count);
        foreach(WFC_Tile tile in newConnections)
        {
            possibleModuleState.Add(tile);
        }
    }

    public void UpdateTile()
    {
        if (possibleModuleState.Count == 1 && finalState == null)
        {
            CollapseNode(possibleModuleState[0]);
        }

        //if (tileReference != null)
        //    tileReference.UpdateTileInformation(possibleModuleState.ToArray());
    }
}
