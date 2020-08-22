using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    //how long it will take to refine 1 item (30seconds)
    public float smeltTimer = 30.0f;
    public bool isSmelting = false;

    //smelting item
    public string smeltItemName = "";
    public byte smeltItemNum = 0;

    //refined item
    public string refinedItemName = "";
    public byte refinedItemNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if not currently smelting and there are items to smelt and don't have 100 refined items being held
        if(!isSmelting && smeltItemNum > 0 && refinedItemNum < 200)
        {
            isSmelting = true;
        }
        //smelt item
        if (isSmelting)
        {
            //removed item in queue
            smeltItemNum -= 1;

            //smelter timer active
            if (smeltTimer > 0)
            {
                smeltTimer -= Time.deltaTime;
            }
            //smelter is done smelting item
            else
            {
                isSmelting = false;
                smeltTimer = 30.0f;
            }
        }
    }
    public void AddSmeltItem(string itemName, byte numberItems)
    {
        if (this.name.CompareTo(itemName) == 0)
        {
            //change to seperate function to ensure only 100 Items can be in furnace
            this.smeltItemNum += numberItems;
        }
        else if (this.name.CompareTo("") == 0)
        {
            this.smeltItemName = itemName;
            //change to seperate function to ensure only 100 Items can be in furnace
            this.smeltItemNum = numberItems;
        }
    }
}
