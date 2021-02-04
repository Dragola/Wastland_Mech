using System.Collections.Generic;
using UnityEngine;

public class Schematic : BuildingRequirements
{
    public List<string> resourcesRequiredNames = new List<string>();
    public List<int> resourcesRequiredNumbers = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        //set tag for all object in building
        gameObject.tag = "schematic";
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.tag = "schematic";
        }

        //load data if game was loaded, otherwise run code below

        //get file
        resourceJson = Resources.Load<TextAsset>("ObjectResourceRequirements");

        //get resources required for this building
        GetBuildingRequirements();
    }
    public void AddResource(string resourceName, int resourceAmount)
    {
        //if the resource is a required one then add to list
        if (resourcesRequiredNames.Contains(resourceName))
        {
            //index of resource in list
            int index = resourcesRequiredNames.IndexOf(resourceName);

            //subtract the required amount of resources
            resourcesRequiredNumbers[index] -= resourceAmount;  
        }

        //check if all resources required are given
        RequirementsMet();
    }
    private void RequirementsMet()
    {
        bool allResources = true;

        foreach (int resource in resourcesRequiredNumbers)
        {
            //still resource missing
            if(allResources == false && resource > 0)
            {
                allResources = false;
            }
        }

        //if all required resopurces
        if (allResources == true)
        {
            Destroy(gameObject.GetComponent<Schematic>());

            //solar
            if (gameObject.name.Contains("solar_panel"))
            {
                gameObject.AddComponent<SolarPower>();
                gameObject.tag = "power";
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.tag = "power";
                }
            }
            //generator
            else if (gameObject.name.Contains("generator"))
            {
                gameObject.AddComponent<Generator>();
                gameObject.tag = "power";
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.tag = "power";
                }
            }
            
        }
    }
    public List<string> GetRequiredResourceNames()
    {
        List<string> names = new List<string>();

        //put every resource name in the list to be retuned
        foreach (string resourceName in resourcesRequiredNames){
            names.Add(resourceName);
        }
        return names;
    }
    public List<int> GetRequiredResourceAmount()
    {
        List<int> amount = new List<int>();

        //put every resource number in the list to be retuned
        foreach (int resourceNum in resourcesRequiredNumbers)
        {
            amount.Add(resourceNum);
        }
        return amount;
    }
    private void GetBuildingRequirements()
    {
        //get data
        LoadBuildingRequirements();

        //solar panel
        if (gameObject.name.Contains("solar_panel"))
        {
            foreach (BuildingReq resource in solarRequirements.SolarPanel)
            {
                resourcesRequiredNames.Add(resource.resource);
                resourcesRequiredNumbers.Add(resource.amount);
            }
        }
        //generator
        else if (gameObject.name.Contains("generator"))
        {
            foreach (BuildingReq resource in generatorRequirements.Generator)
            {
                resourcesRequiredNames.Add(resource.resource);
                resourcesRequiredNumbers.Add(resource.amount);
            }
        }
    }
    public void SetComplete()
    {
        //set all resources to 0
        for (int i=0; i < resourcesRequiredNumbers.Count; i++)
        {
            resourcesRequiredNumbers[i] = 0;
        }

        //check requirements
        RequirementsMet();
    }
}

