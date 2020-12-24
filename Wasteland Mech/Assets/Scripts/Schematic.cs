using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schematic : MonoBehaviour
{
    public List<string> resourcesRequired = new List<string>();
    public List<int> resourceAmountRequired = new List<int>();
    public List<int> resourceAmountGiven = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        //load data if game was loaded, otherwise run code below

        //JsonUtility.FromJson<>
        //furnace, generator, etc.


        //fill resources given with 0's initially
        for(int i =0; i < resourcesRequired.Count; i++)
        {
            resourceAmountGiven.Add(0);
        }
    }
    public int AddResource(string resourceName, int resourceAmount)
    {
        //if the resource is a required one then add to list
        if (resourcesRequired.Contains(resourceName))
        {
            //index of resource in list
            int index = resourcesRequired.IndexOf(resourceName);

            //take full resource amount
            if (resourceAmountGiven[index] + resourceAmount < resourceAmountRequired[index])
            {
                resourceAmountGiven[index] = resourceAmount;
                resourceAmount = 0;
            }
            //take partial resource amount
            else
            {
                //subtract the required amount from amount given
                resourceAmount -= resourceAmountRequired[index] - resourceAmountGiven[index];
            }
        }

        //check if all resources required are given
        RequirementsMet();

        //return any remaining resource
        return resourceAmount;
    }
    private void RequirementsMet()
    {

    }
}
