using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

[System.Serializable]
public class WFC_RoomLayout : MonoBehaviour
{
    public Tilemap tilemap;

    public Vector2Int sizeOfRoom = new Vector2Int(27, 15);
    public string roomName = "TempNewRoom"; // + .asset
    public string roomAssetPath = "Assets/TileMaps/RoomLayouts/";
    // room type enum
    // level or generic, maybe this data doesn't need to be here?
    // public bool autoSaveOnEdit = false;

    public WFC_DataAsset dataAsset;
    public WFC_Manager wfc_Manager = null;

    // this doesn't work. I could save the list to persistant data or something? I'll do it later.
    [HideInInspector] public string roomToLoad = "";
    [HideInInspector] public List<string> roomsSaved = new List<string>();

    [SerializeField] public Tile[,] map;
    private Tile[,] savedMap;

    void Init()
    {
        if (dataAsset == null)
        {
            print("Please set a data asset to use.");
            return;
        }

        wfc_Manager = new WFC_Manager(dataAsset, sizeOfRoom, map);
    }

    private void SaveTileMap()
    {
        map = new Tile[sizeOfRoom.x, sizeOfRoom.y];

        for (int x = 0; x < sizeOfRoom.x; x++)
        {
            for (int y = 0; y < sizeOfRoom.y; y++)
            {
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    map[x, y] = tilemap.GetTile<Tile>(new Vector3Int(x, y, 0));
                }
            }
        }
    }

    private void UpdateTileMap()
    {
        for (int x = 0; x < sizeOfRoom.x; x++)
        {
            for (int y = 0; y < sizeOfRoom.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), map[x, y]);
            }
        }
    }

    public void SolveRoom()
    {
        // solve the current room with WFC.
        SaveTileMap(); // make sure we use the current active tilemap
        if (wfc_Manager == null) Init();
        map = wfc_Manager.SolveMap();
        UpdateTileMap();
    }

    public void SolveWallTiles()
    {
        SaveTileMap();
        if (wfc_Manager == null) Init();
        map = wfc_Manager.SolveWallTiles();
        UpdateTileMap();
    }

    public void ClearBoard ()
    {
        map = new Tile[sizeOfRoom.x, sizeOfRoom.y];

        for (int x = 0; x < sizeOfRoom.x; x++)
        {
            for (int y = 0; y < sizeOfRoom.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), map[x, y]);
            }
        }
    }

    public void ResetRoom()
    {
        // Reset the room tiles to the currently saved room
        map = (Tile[,])savedMap.Clone();
        UpdateTileMap();

        // reload our manager.
        Init();
    }

    public void LoadRoom()
    {
        // load the selected room than reset the room
        string path = roomAssetPath + roomName + ".asset";

        if (AssetDatabase.FindAssets(path) == null)
        {
            Debug.Log("No asset found at path");
            return;
        }

        WFC_RoomData asset = AssetDatabase.LoadAssetAtPath<WFC_RoomData>(path);

        if (asset == null)
        {
            Debug.Log("Failed to load asset at path");
            return;
        }
        sizeOfRoom = asset.roomSize;
        map = asset.GetMap();
        dataAsset = asset.dataAsset;
        savedMap = (Tile[,])map.Clone();

        UpdateTileMap();

        // reload the manager with the new loaded map.
        Init();
    }

    public void SaveRoom()
    {
        SaveTileMap();
        savedMap = (Tile[,])map.Clone();

        // save the current room to a scriptable object.
        WFC_RoomData asset = ScriptableObject.CreateInstance<WFC_RoomData>();
        string path = roomAssetPath + roomName + ".asset";

        if (AssetDatabase.FindAssets(path) != null)
        {
            print("room already exsist, overwriting saved room data.");
        }

        asset.SetMap(map, sizeOfRoom);
        asset.dataAsset = dataAsset;

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        // reload the manager with the new saved map.
        Init();
    }
}
