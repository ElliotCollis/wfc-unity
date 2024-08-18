using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class TileContainer : MonoBehaviour
{
    public int x, y;
    public GameObject buttonPrefab;
    GridLayoutGroup gridLayout = null;
    GridsManager grid;
    public WFC_Module wfc_Module = null;

    public Sprite finalSprite;
    public string finalColourSetup;
    public string connections;

    public void SetTileSprites(WFC_DataNode[] dataNodes, GridsManager _grid, int _x, int _y)
    {
        if (gridLayout == null) gridLayout = GetComponent<GridLayoutGroup>();
        grid = _grid;
        x = _x;
        y = _y;

       // wfc_Module = grid.manager.GetModule(x, y);
        //wfc_Module.tileReference = this;

        // destroy all children firsts.
        DestroyTheChildren();

        // set cell size

        int columnWidth = Mathf.CeilToInt(Mathf.Sqrt(dataNodes.Length));
        GetComponent<GridLayoutGroup>().constraintCount = columnWidth;
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(100 / columnWidth, 100 / columnWidth);

        // generate buttons for each datanode 
        CreateButtons(dataNodes);
    }

    public void UpdateTileInformation(WFC_DataNode[] newNodes)
    {
        DestroyTheChildren();

        if (newNodes.Length == 1)
        {
            GetComponent<Image>().sprite = newNodes[0].sprite;
            finalSprite = newNodes[0].sprite;
        }
        else CreateButtons(newNodes);
        
    }

    public void ButtonPress(WFC_DataNode dataNode)
    {
        // destroy all buttons.
        DestroyTheChildren();

        // create one image is the size of this rect, that has our selected datanode sprite.
        GetComponent<Image>().sprite = (dataNode.sprite);
        finalSprite = dataNode.sprite;
        finalColourSetup = dataNode.compressedValues;
        connections = dataNode.PossibleNeighboursToString();

        // let the grid manager know we've selected a tile and our location.
        grid.selectTile(x, y, dataNode);
    }

    void DestroyTheChildren()
    {
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    void CreateButtons(WFC_DataNode[] dataNodes)
    {
        for (int i = 0; i < dataNodes.Length; i++)
        {
            WFC_DataNode node = dataNodes[i]; // make a COPY first!!

            GameObject t_Button = Instantiate(buttonPrefab, transform);

            // set each sprite on it
            t_Button.GetComponent<Button>().image.sprite = node.sprite;

            // set the callback on the button.
            t_Button.GetComponent<Button>().onClick.AddListener(() => ButtonPress(node));
        }
    }
}