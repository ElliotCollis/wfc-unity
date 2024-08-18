using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WFC_SimpleTiled
{
    //bool running = false;
    WFC_Module[,] map; 
    WFC_Module target;
    Action<List<WFC_Module>> callBack;

    public WFC_SimpleTiled(WFC_Module[,] _map)
    {
        map = _map;
        UpdateAllNeighboursOnMap();
    }

    public void StartPropagation(WFC_Module _target, Action<List<WFC_Module>> _callBack)
    {
        target = _target;
        callBack = _callBack;
        Propagate();
    }

    public void UpdateAllNeighboursOnMap()
    {
        foreach (WFC_Module module in map)
        {
            if (module.finalState == null)
            {
                //Debug.Log(module.possibleModuleState.Count);
                module.UpdatePossibleConnections(UpdatePossibleModules(module));
                //Debug.(module.possibleModuleState.Count.ToString() + ": ");
            }
        }
    }

    void Propagate()
    {
        Stack<WFC_Module> openNodes = new Stack<WFC_Module>();
        List<WFC_Module> changedNodes = new List<WFC_Module>();

        // set target node to our selected.
        changedNodes.Add(target);

        // add nieghbours to open list. 
        foreach (WFC_Module t_module in GetNeighbours(target))
            if (t_module != null)
                openNodes.Push(t_module);

        while (openNodes.Count > 0) // while we have nodes to check .
        {
            // select one node to start. 
            WFC_Module module = openNodes.Pop();

            // update the possible neighbours list.
            List<WFC_Tile> updatedPossibleModules = UpdatePossibleModules(module);

            if (updatedPossibleModules.Count == 0)
            {
                //Debug.Log("No neighbours found at: " +module.x.ToString() + ", " + module.y.ToString()); // called 29 times for one off module.
                //Debug.Log("Module had " + module.possibleModuleState.Count + " possible states previously.");
                // print out the before possible module number as well.
                continue;
            }
            // if we've changed, add our neihgbours to open nodes.

            //Debug.Log(module.possibleModuleState.Count);
            //Debug.Log(updatedPossibleModules.Count);
            //break;
            if (module.possibleModuleState.Count != updatedPossibleModules.Count)
            {
                foreach (WFC_Module t_module in GetNeighbours(module))
                    if (t_module != null)
                        openNodes.Push(t_module);
            }

            //update out connections.
            module.UpdatePossibleConnections(updatedPossibleModules);

            if (!changedNodes.Contains(module))
                changedNodes.Add(module);
        }

        // call update on all our modules.
        // instead of callback I'm going to call them all directly.
        callBack(changedNodes);
    }


    List<WFC_Tile> UpdatePossibleModules(WFC_Module module)
    {
        // for each possible tile, we want to check each neighbour has a copy of us in there possible connections,
        // if they do add it to the list to be updated.

        List<WFC_Tile> updatedPossibleModules = new List<WFC_Tile>();
        WFC_Module[] neighbours = GetNeighbours(module);

        foreach (WFC_Tile possibleState in module.possibleModuleState) // check out each current state.
        {
            int SideMatchsFound = 0;
            int sideConnections = 0;

            // go through each neighbour side in cardinal directions.
            for (int i = 0; i < 4; i++) // 0 left, 1 top, 2 right, 3 bottom
            {
                bool hasCurrentSide = false;
                //Debug.Log(neighbours[i]);

                if (neighbours[i] != null) // if we have a neighbour, so not against a wall etc.
                {
                    sideConnections++; // this marks how many neighbours we have?

                    foreach (WFC_Tile tiles in neighbours[i].possibleModuleState) 
                    {
                        // for each of our neighbours states. check we're in their list.
                        // ok it's working now but there's an odd issue where sometimes we return 0 neighbours. and it should reset and start again?

                        if (tiles.SingleNeighbourSide(ReverseEdge(i)).Contains(possibleState.myTile))  
                        {
                            //Debug.Log("found a match");
                            hasCurrentSide = true;
                        }
                    }
                }

                if (hasCurrentSide) SideMatchsFound++;
            }

            // if we have a connection for each side. and we're not already in the updated modules.
            if (SideMatchsFound == sideConnections && !updatedPossibleModules.Contains(possibleState))
                updatedPossibleModules.Add(possibleState);
        }

        return updatedPossibleModules;
    }


    WFC_Module[] GetNeighbours(WFC_Module module) // get array or list
    {
        WFC_Module[] neighbours = new WFC_Module[4];

        // get width and height of map
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        // check cardinal directions are in bounds, add them to the list to check.
        if (module.x - 1 >= 0) neighbours[0] = (map[module.x - 1, module.y]); // left
        if (module.y + 1 < height) neighbours[1] = (map[module.x, module.y + 1]); // top
        if (module.x + 1 < width) neighbours[2] = (map[module.x + 1, module.y]); // right
        if (module.y - 1 >= 0) neighbours[3] = (map[module.x, module.y - 1]); // down

        return neighbours;
    }

    int ReverseEdge(int id)
    {
        switch (id)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 0;
            case 3:
                return 1;
        }
        return 0;
    }
}
