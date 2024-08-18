using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveSerial : MonoBehaviour
{
	public int moduleID;
	int x, y;
	string colourNodes;
	public string savedSprites;
	int savedDataCount;

	public List<WFCModuleSave> savedModules = new List<WFCModuleSave>();

	public void Awake()
	{
		LoadData();
	}
	public void PopulateSaveData(WFC_Module module)
	{
		x = module.x;
		y = module.y;
		//if (module.finalNodeColors != null) colourNodes = module.finalNodeColors;

		for (int i = 0; i < module.possibleModuleState.Count; i++)
		{
			//savedSprites += module.possibleModuleState[i].sprite.GetInstanceID();
			if (i < module.possibleModuleState.Count) savedSprites += " ";

			//SAVE ALL SPRITE IDS SEPARATED BY A FULLSTOP
		}

		SaveModuleData();
	}
	public void SaveModuleData()
	{
		//SAVE NEW MODULE DATA

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath
					 + "/" + moduleID + ".dat");

		WFCModuleSave moduleData = new WFCModuleSave();

		moduleData.moduleID = moduleID;
		moduleData.x = x;
		moduleData.y = y;
		moduleData.colourNodes = colourNodes;

		moduleData.spriteIDs = savedSprites;

		savedModules.Add(moduleData);

		bf.Serialize(file, moduleData);
		file.Close();
	}

	public void SaveConstantData()
	{
		//SAVE CONSTANT DATA
		BinaryFormatter bfc = new BinaryFormatter();
		FileStream filec = File.Create(Application.persistentDataPath
					 + "/constant.dat");

		ConstantData constantData = new ConstantData();

		constantData.saveDataCount = savedDataCount;
		constantData.moduleDataSaves = savedModules;
		constantData.moduleID = moduleID;

		bfc.Serialize(filec, constantData);
		filec.Close();

		Debug.Log("Game data saved!");

		GetComponent<GridsManager>().RefreshSide(constantData.moduleDataSaves);
	}

	public void LoadData()
	{
		if (File.Exists(Application.persistentDataPath
				   + "/constant.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file =
					   File.Open(Application.persistentDataPath
					   + "/constant.dat", FileMode.Open);

			ConstantData constantData = (ConstantData)bf.Deserialize(file);

			moduleID = constantData.moduleID;
			savedModules = constantData.moduleDataSaves;

			file.Close();

			GetComponent<GridsManager>().RefreshSide(constantData.moduleDataSaves);
			Debug.Log("Game data loaded!");
		}
		else return;
	}

	public void ResetData()
	{
		for (int i = 0; i < moduleID; i++)
		{
			if (File.Exists(Application.persistentDataPath + "/" + 0 + ".dat"))
			{
				File.Delete(Application.persistentDataPath + "/" + 0 + ".dat");

				Debug.Log("Data reset complete!");
			}
		}
		if (File.Exists(Application.persistentDataPath + "/constant.dat"))
			File.Delete(Application.persistentDataPath + "/constant.dat");

		ClearTemps();
	}

	public void ClearTemps()
	{
		x = 0;
		y = 0;

		moduleID = 0;
		colourNodes = null;
		savedSprites = null;
		savedModules.Clear();
	}
	void UnpackColourString(string colourValues)
	{
		//Heirachy of packing:
		//Row collections separated by "?"
		//Cells collections separated by "/"
		//Color collections separated by " "

		//Unpack into Rows (L >> U >> R >> D)
		string[] colourRows = colourValues.Split('?');

		//Unpack into Cells (L cells >> U cells >> R cells >> D cells)
		string[] colourCells = colourValues.Split('/');

		//Unpack into Colours (L cell colours >> U cell colours >> R cell colours >> D cell colours)
		string[] colours = colourValues.Split(' ');
	}

}

[Serializable]
struct ConstantData
{
	[SerializeField] public int moduleID;
	[SerializeField] public int saveDataCount;
	[SerializeField] public WFCModuleSave chunkDefaultModule;
	[SerializeField] public List<WFCModuleSave> moduleDataSaves;
}

[Serializable]
public struct WFCModuleSave
{
	[SerializeField] public bool modified;
	[SerializeField] public int moduleID;
	[SerializeField] public int x, y;

	[SerializeField] public string spriteIDs;
	[SerializeField] public string colourNodes;
}

