using UnityEngine;

public class BuildingRequirements : MonoBehaviour
{

    public TextAsset resourceJson;

    public SolarPanelRequirements solarRequirements;
    public GeneratorRequirements generatorRequirements;

    //call this method to determine which variable needs to be filled
    public void LoadBuildingRequirements()
    {
        if (gameObject.name.Contains("solar_panel")){
            //read json file and store in class
            solarRequirements = JsonUtility.FromJson<SolarPanelRequirements>(resourceJson.text);
        }
        else if(gameObject.name.Contains("generator"))
        {
            //read json file and store in class
            generatorRequirements = JsonUtility.FromJson<GeneratorRequirements>(resourceJson.text);
        }
    }
}
//classes for the json file
[System.Serializable]
public class BuildingReq
{
    //inner class, holds the variables for each resource and amount variables
    public string resource;
    public int amount;
}
public class SolarPanelRequirements
{
    //outer class, holds the list that contains all the BuildingReq objects
    public BuildingReq[] SolarPanel;
}
public class GeneratorRequirements
{
    //outer class, holds the list that contains all the BuildingReq objects
    public BuildingReq[] Generator;
}