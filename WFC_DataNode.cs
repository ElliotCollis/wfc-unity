using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WFC_DataNode
{

    // hold all the data needed for the module and algorythm to work.

    // colour values.
    // strings, hex.
    [SerializeField] public Sprite sprite;
    [SerializeField] public string[][] colourValues;
    [SerializeField] public string compressedValues;



    //Calculate possible neighbours.
    public WFC_DataNode[][] possibleNeighbours;

    int edgeBuffer = 1; //out of %100 // width * (edgebuffer / 100)

    public WFC_DataNode() { }
    public WFC_DataNode(Sprite _sprite)
    {
        SetEdgeReferences(_sprite);
    }

    public void SetEdgeReferences (Sprite _sprite)
    {
        sprite = _sprite;
        int edgeBufferAmount = Mathf.RoundToInt(sprite.texture.width * (edgeBuffer / 100f));
        int edgeDividAmount = Mathf.RoundToInt(sprite.texture.width / 4);
        colourValues = new string[4][];

        //create a temp array to set the colour values to.
        string[] t_array;
        t_array = new string[3];
        Color t_colour;

        // Left side
        for (int i = 0; i < t_array.Length; i++)
        {
            t_colour = sprite.texture.GetPixel(0 + edgeBufferAmount, (i + 1) * edgeDividAmount);

            if (t_colour.a == 0)
                t_array[i] = "transparent";
            else
                t_array[i] = ColorUtility.ToHtmlStringRGB(t_colour);

        }

        // set the array in the nested array after every for loop
        colourValues[0] = t_array;
        t_array = new string[3];

        //top side
        for (int i = 0; i < t_array.Length; i++)
        {
            t_colour = sprite.texture.GetPixel((i + 1) * edgeDividAmount, sprite.texture.width - edgeBufferAmount);
            if (t_colour.a == 0)
                t_array[i] = "transparent";
            else
                t_array[i] = ColorUtility.ToHtmlStringRGB(t_colour);

        }

        // set the array in the nested array after every for loop
        colourValues[1] = t_array;
        t_array = new string[3];

        // right side
        for (int i = 0; i < t_array.Length; i++)
        {
            t_colour = sprite.texture.GetPixel(sprite.texture.width - edgeBufferAmount, (i + 1) * edgeDividAmount);
            if (t_colour.a == 0)
                t_array[i] = "transparent";
            else
                t_array[i] = ColorUtility.ToHtmlStringRGB(t_colour);

        }

        // set the array in the nested array after every for loop
        colourValues[2] = t_array;
        t_array = new string[3];

        // bottom side
        for (int i = 0; i < t_array.Length; i++)
        {
            t_colour = sprite.texture.GetPixel((i + 1) * edgeDividAmount, 0 + edgeBufferAmount);
            if (t_colour.a == 0)
                t_array[i] = "transparent";
            else
                t_array[i] = ColorUtility.ToHtmlStringRGB(t_colour);
        }

        // set the array in the nested array after every for loop
        colourValues[3] = t_array;

        compressedValues += 
                colourValues[0][0] + "," + colourValues[0][1] + "," + colourValues[0][2] + "." + //L
                colourValues[1][0] + "," + colourValues[1][1] + "," + colourValues[1][2] + "." + //U
                colourValues[2][0] + "," + colourValues[2][1] + "," + colourValues[2][2] + "." + //R
                colourValues[3][0] + "," + colourValues[3][1] + "," + colourValues[3][2] + "."; //D
    }

    public void CalculatePossibleNeighbours(WFC_DataNode[] dataNodes)
    {

        possibleNeighbours = new WFC_DataNode[4][];

        List<WFC_DataNode> left = new List<WFC_DataNode>();
        List<WFC_DataNode> up = new List<WFC_DataNode>();
        List<WFC_DataNode> right = new List<WFC_DataNode>();
        List<WFC_DataNode> down = new List<WFC_DataNode>();


        for (int i = 0; i < dataNodes.Length; i++)
        {
            //left
            if (dataNodes[i].colourValues[2][0] == colourValues[0][0] && dataNodes[i].colourValues[2][1] == colourValues[0][1] && dataNodes[i].colourValues[2][2] == colourValues[0][2])
            {
                left.Add(dataNodes[i]);
            }

            //up
            if (dataNodes[i].colourValues[3][0] == colourValues[1][0] && dataNodes[i].colourValues[3][1] == colourValues[1][1] && dataNodes[i].colourValues[3][2] == colourValues[1][2])
            {
                up.Add(dataNodes[i]);
            }

            //right
            if (dataNodes[i].colourValues[0][0] == colourValues[2][0] && dataNodes[i].colourValues[0][1] == colourValues[2][1] && dataNodes[i].colourValues[0][2] == colourValues[2][2])
            {
                right.Add(dataNodes[i]);
            }

            // Problem here.
            //down
            if (dataNodes[i].colourValues[1][0] == colourValues[3][0] && dataNodes[i].colourValues[1][1] == colourValues[3][1] && dataNodes[i].colourValues[1][2] == colourValues[3][2])

            {
                down.Add(dataNodes[i]);
            }
        }

        /*
        Debug.Log(sprite.name + "Left: " + left.Count); 
        Debug.Log(sprite.name + "Up: " + up.Count); 
        Debug.Log(sprite.name + "Right: " + right.Count); 
        Debug.Log(sprite.name + "Down: " + down.Count); 
        */
        
        possibleNeighbours[0] = left.ToArray();
        possibleNeighbours[1] = up.ToArray();
        possibleNeighbours[2] = right.ToArray();
        possibleNeighbours[3] = down.ToArray();
        
    }

    // save the lists so we can optimise?
    public List<WFC_DataNode> SingleNeighbourSide (int sideID)
    {
        List<WFC_DataNode> tempList = new List<WFC_DataNode>();
        foreach(WFC_DataNode node in possibleNeighbours[sideID])
        {
            tempList.Add(node);
        }

        return tempList;
    }

    public string PossibleNeighboursToString ()
    {
        string temp = "";

        for (int i = 0; i < possibleNeighbours.Length; i++)
        {
            WFC_DataNode[] lists = possibleNeighbours[i];
            temp = temp + " | "  + i + " | ";
            foreach(WFC_DataNode node in lists)
            {
                temp = temp + ", " + node.sprite.name;
            }
        }

        return temp;
    }
}
