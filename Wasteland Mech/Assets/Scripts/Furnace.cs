using UnityEngine;
using UnityEngine.UI;

public class Furnace : Refine
{
    //UI
    private Text furnaceNameText = null;
    private Text furnaceSmeltText = null;
    private Text furnaceSmeltedText = null;
    private Text furnaceStatusText = null;

    //---------------------------------------------------------------------------------Start Function
    void Start()
    {
        //sets the tag of the gameobject
        gameObject.tag = "refining";

        //find UI Text's
        furnaceNameText = GameObject.Find("furnaceNameText").GetComponent<Text>();
        furnaceSmeltText = GameObject.Find("furnaceSmeltText").GetComponent<Text>();
        furnaceSmeltedText = GameObject.Find("furnaceSmeltedText").GetComponent<Text>();
        furnaceStatusText = GameObject.Find("furnaceStatusText").GetComponent<Text>();

        //change text so other furnaces don't only find the first furnaces text's
        World world = GameObject.Find("world").GetComponent<World>();
        furnaceSmeltText.gameObject.name = "furnaceSmeltText" + world.GetFurnaceCount();
        furnaceSmeltedText.gameObject.name = "furnaceSmeltedText" + world.GetFurnaceCount();
        furnaceStatusText.gameObject.name = "furnaceStatusText" + world.GetFurnaceCount();

        //have counter in world for furnace count
        furnaceNameText.text = "Furnace";

        //update text once spawned/placed
        updateText = true;
    }

    //---------------------------------------------------------------------------------Update Function
    void Update()
    {
        //if game is not paused then smelt
        if (paused == false)
        {
            //enable smelting if it hasn't smelted the max and have something to smelt
            if (isRefining == false && refinedItemNum != SMELTED_MAX && refineItemNum != 0)
            {
                Debug.Log("Enable Smelting");
                UpdateStatus(true);
                updateText = true;
            }
            //disable is smelted the max
            else if (isRefining == true && (refinedItemNum == SMELTED_MAX || refineItemNum == 0))
            {
                Debug.Log("Disable Smelting");
                UpdateStatus(false);
                updateText = true;
            }
            //smelt item
            if (isRefining)
            {
                //smelter timer active
                if (refineTimer > 0)
                {
                    refineTimer -= Time.deltaTime;
                    updateText = true;
                }
                //smelter is done smelting item
                else
                {
                    //removed item in smelt
                    refineItemNum -= 1;

                    //add to smelted number
                    refinedItemNum += 1;

                    //reset timer
                    refineTimer = 30.0f;

                    //update text
                    updateText = true;
                }
            }
            //update text when indicated
            if (updateText)
            {
                UpdateText();
            }
        }
    }
    //---------------------------------------------------------------------------------UI Update Functions
    private void UpdateText()
    {
        //if there is at least 1 refine or refined item
        if (refinedItemNum > 0 || refineItemNum > 0) { 
            furnaceSmeltText.text = refineItemName + " x " + refineItemNum;
            furnaceSmeltedText.text = refinedItemName + " x " + refinedItemNum;
        }
        else
        {
            furnaceSmeltText.text = "Awaiting resource";
            furnaceSmeltedText.text = "";
        }

        //furnace status
        if (isRefining == true)
        {
            furnaceStatusText.text = "Status: Smelting = " + string.Format("{0:0.#}", refineTimer);
        }
        else
        {
            furnaceStatusText.text = "Status: Idle";
        }
        updateText = false;
    }
}
