using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GridsManager : MonoBehaviour
{
    public List<WFC_Module> modulesToForward = new List<WFC_Module>();

    [SerializeField] public int height;
    [SerializeField] public int width;

    public Button buttonPrefab;

    public WFC_Manager manager;

    public GameObject tileContainer;

    public TempWFCTiles wfcTiles;
   
    public SaveSerial saveData;

    public Transform savedContainer;


    public void Start()
    {

        if (manager == null)
        {
          //  manager = new WFC_Manager(wfcTiles.tiles, width, height);

           // CreateContainers(manager.data.dataNodes);
        }
    }


    public void CreateContainers(WFC_DataNode[] dataNodes)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject t_TileContainer = Instantiate(tileContainer, transform);

                t_TileContainer.transform.localScale = new Vector2(GetComponentInParent<RectTransform>().rect.width / width / 100, GetComponentInParent<RectTransform>().rect.height / height / 100);
                t_TileContainer.GetComponent<TileContainer>().SetTileSprites(dataNodes, this, x, y);

                Vector2 position = new Vector2(x * RectTransformExtensions.GetWorldRect(t_TileContainer.GetComponent<RectTransform>()).width, y * RectTransformExtensions.GetWorldRect(t_TileContainer.GetComponent<RectTransform>()).height);
                Vector2 adjustedPosition = position + RectTransformExtensions.GetWorldRect(GetComponent<RectTransform>()).position;
                t_TileContainer.transform.position = new Vector2(adjustedPosition.x, adjustedPosition.y);

            }
        }

    }

    public void selectTile(int x, int y, WFC_DataNode dataNode)
    {
       // manager.selectNode(manager.GetModule(x, y), dataNode);
    }

    public void Reset()
    {
        // just reloading the scene at the moment, but I need to reload all the tiles to be reset and reset all the data.

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveData()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
               // WFC_Module modulePicked = manager.data.map[x, y];
                //modulesToForward.Add(modulePicked);
            }
        }
        

        for (int i = 0; i < modulesToForward.Count; i++)
        {
            saveData.PopulateSaveData(modulesToForward[i]);
        }

        saveData.moduleID++;

        saveData.SaveConstantData();
        modulesToForward.Clear();
        saveData.ClearTemps();
        saveData.LoadData();
    }

    public void Solve()
    {
        List<WFC_Module> list = new List<WFC_Module>();

        //FORM LIST
        //foreach (var item in manager.data.map)
        //{
        //    list.Add(item);
        //}

        //SHUFFLE THE LIST
        for (int i = 0; i < list.Count; i++)
        {
            WFC_Module temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        //
        for (int i = 0; i < list.Count; i++)
        {
            //WFC_DataNode nodePicked = list[i].possibleModuleState[Random.Range(0,list[i].possibleModuleState.Count)];
            //manager.selectNode(list[i], nodePicked );
        }
    }

    public void ResetData()
    {
        modulesToForward.Clear();
        DeleteChildren();
        saveData.ResetData();
    }
    
    public void RefreshSide(List<WFCModuleSave> constantData)
    {
        DeleteChildren();
        UnpackDataToContainers(constantData);
    }

    void DeleteChildren()
    {
        foreach (Transform child in savedContainer)
        {
            Destroy(child.gameObject);
        }
    }
    void UnpackDataToContainers(List<WFCModuleSave> constantData)
    {
        for (int i = 0; i < saveData.moduleID; i++)
        {
            GameObject _container = new GameObject();
            _container.transform.parent = savedContainer;
            _container.transform.localScale = new Vector3(1, 1, 1);
            _container.AddComponent<GridLayoutGroup>();
            _container.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _container.GetComponent<GridLayoutGroup>().constraintCount = width;
            _container.GetComponent<GridLayoutGroup>().cellSize = new Vector2(190 / width, 190 / width);
            _container.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.LowerLeft;
            _container.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Vertical;


            for (int j = 0; j < constantData.Count; j++)
            {
                if (constantData[j].moduleID == i)
                {
                    GameObject _go = new GameObject(constantData[j].x.ToString() + "," + constantData[j].y.ToString());
                    _go.transform.parent = _container.transform;
                    _go.transform.localScale = new Vector3(1f / Mathf.CeilToInt(Mathf.Sqrt(constantData[j].spriteIDs.Length)), 1f / Mathf.CeilToInt(Mathf.Sqrt(constantData[j].spriteIDs.Length)), 1f / Mathf.CeilToInt(Mathf.Sqrt(constantData[j].spriteIDs.Length)));
                    _go.AddComponent<GridLayoutGroup>();
                    _go.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    _go.GetComponent<GridLayoutGroup>().constraintCount = width;

                    for (int k = 0; k < constantData[j].spriteIDs.Length; k++)
                    {
                        GameObject _go2 = new GameObject(constantData[j].spriteIDs.ToString());
                        _go2.transform.parent = _go.transform;
                        _go2.transform.localScale = new Vector3(1, 1, 1);

                        Image _image = _go2.AddComponent<Image>();

                        int _tID = spriteIDs(constantData[j].spriteIDs)[k];
                        Debug.Log(_tID);

                         _image.sprite = (Sprite)UnityEditor.EditorUtility.InstanceIDToObject(_tID);
                         _image.transform.localPosition = new Vector2(constantData[j].x, constantData[j].y);
                        
                    }
                }
            }
        }                  
    }

    void UnpackColourString(string colourValues)
    {
        //Heirachy of packing:
        //Row collections separated by "?"
        //Cells collections separated by "/"
        //Color collections separated by "."

        //Unpack into Rows (L >> U >> R >> D)
        string[] colourRows = colourValues.Split('?');

        //Unpack into Cells (L cells >> U cells >> R cells >> D cells)
        string[] colourCells = colourValues.Split('/');

        //Unpack into Colours (L cell colours >> U cell colours >> R cell colours >> D cell colours)
        string[] colours = colourValues.Split('.');
    }

    public int[] spriteIDs(string spriteIds)
    {
        //Heirachy of packing:
        //ID collections separated by "."

        string[] sprites = spriteIds.Split(' ');

        int[] spriteAddresses = new int[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            //Debug.Log(sprites[i]);

            if(sprites[i] != "")
                spriteAddresses[i] = int.Parse(sprites[i]);
        }   
        
        return spriteAddresses;
    }
}


/*
public UnpackWFCModule(List<WFCModuleSave> constantData, int saveModuleID)
{
    //Unpack Location
    int locationX = constantData[saveModuleID].x;
    int locationY = constantData[saveModuleID].y;

    //Unpack ColourNodes into Rows
    //Row collections separated by "?"
    //Cells collections separated by "/"
    //Color collections separated by "."
    string[] colourNodes = constantData[saveModuleID].colourNodes.Split('?');

    //Unpack ColourNodes into Rows
    //IDs separated by "."  
    string[] neighbourSpriteIDs = constantData[saveModuleID].spriteIDs.Split('.');

}
*/

public static class RectTransformExtensions
{
    public static Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        // Get the bottom left corner.
        Vector3 position = corners[0];

        Vector2 size = new Vector2(
            rectTransform.lossyScale.x * rectTransform.rect.size.x,
            rectTransform.lossyScale.y * rectTransform.rect.size.y);

        return new Rect(position, size);
    }
}