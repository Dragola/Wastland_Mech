using System;
using UnityEngine;
using UnityEngine.UI;

public class Furnace : MonoBehaviour
{
    //pause furnace if game is paused
    //private bool paused = false;

    //UI
    private Text furnaceNameText = null;
    private Text furnaceSmeltText = null;
    private Text furnaceSmeltedText = null;
    private Text furnaceStatusText = null;

    //how long it will take to refine 1 item (30seconds)
    public float smeltTimer = 30.0f;
    public bool isSmelting = false;
   

    //smelting item
    public string smeltItemName = "";
    public byte smeltItemNum = 0;

    //smelted item
    public string smeltedItemName = "";
    public byte smeltedItemNum = 0;

    //private byte SMELT_MAX = 50;
    private byte SMELTED_MAX = 100;

    // Start is called before the first frame update
    void Start()
    {
        //find UI Text's
        furnaceNameText = GameObject.Find("furnaceNameText").GetComponent<Text>();
        furnaceSmeltText = GameObject.Find("furnaceSmeltText").GetComponent<Text>();
        furnaceSmeltedText = GameObject.Find("furnaceSmeltedText").GetComponent<Text>();
        furnaceStatusText = GameObject.Find("furnaceStatusText").GetComponent<Text>();

        //have counter in world for furnace count
        furnaceNameText.text = "Furnace";
    }

    // Update is called once per frame
    void Update()
    {
        //enable smelting if it hasn't smelted the max and have something to smelt
        if (isSmelting == false && smeltedItemNum != SMELTED_MAX && smeltItemNum != 0)
        {
            Debug.Log("Enable Smelting");
            UpdateStatus(true);
            UpdateText(2);
        }
        //disable is smelted the max
        else if (isSmelting == true && (smeltedItemNum == SMELTED_MAX || smeltItemNum == 0))
        {
            Debug.Log("Disable Smelting");
            UpdateStatus(false);
            UpdateText(2);
        }
        //smelt item
        if (isSmelting)
        {
            //smelter timer active
            if (smeltTimer > 0)
            {
                smeltTimer -= Time.deltaTime;
                UpdateText(2);
            }
            //smelter is done smelting item
            else
            {
                //removed item in smelt
                smeltItemNum -= 1;

                //add to smelted number
                smeltedItemNum += 1;

                //reset timer
                smeltTimer = 30.0f;
                
                //update text's
                UpdateText(0);
                UpdateText(1);
            }
        }
    }
    public void AddSmeltItem(string itemName, byte numberItems)
    {
        //same item
        if (smeltItemName.CompareTo(itemName) == 0)
        {
            smeltItemNum += numberItems;
            UpdateText(0);
        }
        //different item
        else
        {
            smeltItemName = itemName;
            smeltItemNum = numberItems;
            UpdateText(0);

            //set smelted name when smelt name is set
            if (smeltItemName.CompareTo("scrap") == 0)
            {
                smeltedItemName = "metal";
                UpdateText(1);
            }
        }
    }
    private void UpdateStatus(Boolean status)
    {
        //disable smelting
        if(status == false && isSmelting == true)
        {
            isSmelting = false;
        }
        //enable smelting
        else if (status == true && isSmelting == false)
        {
            isSmelting = true;
        }
    }
    private void UpdateText(byte textIndex)
    {
        //smelt item
        if(textIndex == 0)
        {
            furnaceSmeltText.text = smeltItemName + " x "+ smeltItemNum;
        }
        //smelted item
        else if (textIndex == 1)
        {
            furnaceSmeltedText.text = smeltedItemName + " x " + smeltedItemNum;
        }
        //furnace status
        else if (textIndex == 2)
        {
            if (isSmelting == true)
            {
                furnaceStatusText.text = "Status: Smelting = " + string.Format("{0:0.#}", smeltTimer);
            }
            else
            {
                furnaceStatusText.text = "Status: Idle";
            }
        }
    }
}
