using UnityEngine;

public class Refine : MonoBehaviour
{
    //pause if game is paused
    public bool paused = false;

    //status if smelting or not
    public bool isRefining = false;

    //how long it will take to refine 1 item
    public float refineTimer = 30.0f;

    //smelting item
    public string refineItemName = "";
    public byte refineItemNum = 0;

    //smelted item
    public string refinedItemName = "";
    public byte refinedItemNum = 0;

    //private byte SMELT_MAX = 50;
    public byte SMELTED_MAX = 100;

    public bool updateText = false;

    //---------------------------------------------------------------------------------Add Function
    public void AddRefineItem(string itemName, byte numberItems)
    {
        //same item
        if (refineItemName.CompareTo(itemName) == 0)
        {
            refineItemNum += numberItems;
            //UpdateText(0);

        }
        //different item
        else
        {
            //set smelt item + num and update text
            refineItemName = itemName;
            refineItemNum = numberItems;
            //UpdateText(0);

            //set and update text for smelted item
            //SetSmeltedName(smeltItemName);
        }
        //indicate to update text
        updateText = true;
    }
    //---------------------------------------------------------------------------------Update Function
    public void UpdateStatus(bool status)
    {
        //disable smelting
        if (status == false && isRefining == true)
        {
            isRefining = false;
        }
        //enable smelting
        else if (status == true && isRefining == false)
        {
            isRefining = true;
        }
        //set the refined name
        SetRefinedName();
    }
    //---------------------------------------------------------------------------------Get/Set Functions
    public string GetRefinedItem()
    {
        string smelted = refinedItemName + "," + refinedItemNum;

        refinedItemNum = 0;

        //indicate to update text
        updateText = true;

        return smelted;
    }
    public string GetRefinedName()
    {
        string name = "";

        //set smelted name when smelt name is set
        if (refineItemName.CompareTo("scrap") == 0)
        {
            name = "metal";
        }
        return name;
    }
    private void SetRefinedName()
    {
        string refinedName = GetRefinedName();
        
        //update the refined name if it's not that already
        if (refinedName.CompareTo(refinedItemName) != 0)
        {
            refinedItemName = refinedName;
        }
    }
    public void SetPaused(bool paused)
    {
        this.paused = paused;
    }
}
