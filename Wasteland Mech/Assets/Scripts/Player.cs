using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : Character
{
    //scripts
    private World scriptW = null;

    //player
    public float moveX = 0;
    public float moveY = 0;
    private GameObject playerRaycastPoint = null;
    public byte buildingSelected = 0;
    public GameObject buildingGameObject = null;
    private GameObject playerDropSpot = null;
    public float conditionUpdateTime = 1;

    //camera's
    private Camera playerCamera;

    //UI components
    private Text playerHealth = null;
    private Text playerFood = null;
    private Text playerWater = null;
    public Button playerDrop = null;
    private Text playerInteractText = null;
    private Text playerModeText = null;
    private Canvas playerInventoryUI = null;
    private Canvas playerPauseMenuUI = null;
    private Canvas playerOptionsUI = null;

    //bool aim = false;

    //movement and menu's
    public bool enableMovement = true;
    public bool enablePause = false;
    public bool enableBuild = false;
    public bool methodKeyHit = false;

    //inventory
    public bool enableInventory = false;
    public GameObject reachableObject = null;
    public sbyte slotSelected = -1;
    public Button[] inventoryButtons = null;

    //framerate
    public int frameRate = 0;
    public Text avgFrameRateText = null;

    //---------------------------------------------------------------------------------Awake Function
    private void Awake()
    {
        //disable VR asset until game is more complete
        //GameObject.Find("OVRCameraRig").SetActive(false);

        //set player up
        CharacterSetUp(true, 0);

        //inventory
        inventoryButtons = new Button[16];

        //scripts
        scriptW = GameObject.Find("world").GetComponent<World>();
        playerRaycastPoint = GameObject.Find("playerRaycastPoint");
        playerDropSpot = GameObject.Find("playerDropSpot");

        //camera's
        playerCamera = GameObject.Find("playerCamera").GetComponent<Camera>();

        //UI
        playerHealth = GameObject.Find("playerHealth").GetComponent<Text>();
        playerFood = GameObject.Find("playerFood").GetComponent<Text>();
        playerWater = GameObject.Find("playerWater").GetComponent<Text>();
        playerDrop = GameObject.Find("playerDrop").GetComponent<Button>();
        playerInteractText = GameObject.Find("playerInteract").GetComponent<Text>();
        playerModeText = GameObject.Find("playerMode").GetComponent<Text>();
        playerInventoryUI = GameObject.Find("playerInventoryUI").GetComponent<Canvas>();
        playerPauseMenuUI = GameObject.Find("playerPauseMenuUI").GetComponent<Canvas>();
        playerOptionsUI = GameObject.Find("playerOptionsUI").GetComponent<Canvas>();
        avgFrameRateText = GameObject.Find("playerFPS").GetComponent<Text>();
        
        //create buttons
        InventoryButtonLayout(4);
    }
    //---------------------------------------------------------------------------------Start Function
    void Start()
    {
        //Cursor set initial state (invisible + locked to prevent going off screen)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //keeps in middle (prevents clicking out of game)

        //UI set initial states (disabled)
        playerInventoryUI.gameObject.SetActive(false);
        playerPauseMenuUI.gameObject.SetActive(false);
        playerOptionsUI.gameObject.SetActive(false);
        playerDrop.gameObject.SetActive(false);

        //set conditons
        playerFood.text = food.ToString() + "%";
        playerWater.text = water.ToString() + "%";
    }
    //---------------------------------------------------------------------------------Update Function
    void Update()
    {
        //gets's mouse input
        moveX = Input.GetAxis("Mouse X");
        moveY = -Input.GetAxis("Mouse Y");

        //if any key was hit or mouse movement
        if (Input.anyKey || moveX != 0 || moveY != 0)
        {
            //Movement
            if (enableMovement)
            {
                Movement();
            }
        }
        //Inventory
        if (enableInventory)
        {
            Inventory();
        }
        //Building
        if (enableBuild)
        {
            Building();
        }
        //Paused
        if (enablePause)
        {
            Paused();
        }

        //melee timer
        if (melee == true && meleeTime > 0)
        {
            meleeTime -= Time.deltaTime;
        }
        //reset meele
        else if (meleeTime <= 0)
        {
            melee = false;
            meleeTime = 3;
        }

        //food + water updater
        if (conditionUpdateTime <= 0)
        {
            ConditionUpdate();
        }
        //decrease time for condition to update (1 sec countdown)
        else
        {
            conditionUpdateTime -= Time.deltaTime;
        }

        //frame rate (not sure how accurate it is)
        frameRate = (int)(Time.frameCount / Time.time);
        avgFrameRateText.text = frameRate.ToString() + " FPS";
    }
    //---------------------------------------------------------------------------------Physics Functions
    void FixedUpdate()
    {
        RaycastHit hit;

        //raycast for interaction (disabled if in build mode)
        if (move && enableBuild == false)

            //interaction
            if (Physics.Raycast(playerRaycastPoint.transform.position, playerRaycastPoint.transform.forward, out hit, (float)1.5))
            {

                //get interacted object if not already
                if (reachableObject == null)
                {
                    reachableObject = hit.collider.gameObject;
                }
                //new interacted object
                else if (reachableObject.name.CompareTo(hit.collider.gameObject.name) != 0)
                {
                    reachableObject = hit.collider.gameObject;
                }
                UpdateInteractionText();
            }
            //if not interacting with anything then set reachable_object to null
            else
            {
                reachableObject = null;

                UpdateInteractionText();
            }
        //if player moved and raycast hit then update (or move) building location
        if (move && Physics.Raycast(playerRaycastPoint.transform.position, playerRaycastPoint.transform.forward, out hit, 10) && buildingGameObject != null)
        {
            //set building position
            buildingGameObject.transform.position = hit.point;
        }
    }
    //---------------------------------------------------------------------------------Player Controls
    //player movement and interaction
    public void Movement()
    {
        //resets move detected
        move = false;

        //rotates player
        if (moveX != 0)
        {
            //rotates player (left/right)
            transform.Rotate(0, moveX, 0);
            move = true;
        }

        //moves camera's y-axis and ray_position
        if (moveY != 0)
        {
            //rotates camera's (up/down)
            playerCamera.transform.Rotate(moveY, 0, 0);
            playerRaycastPoint.transform.Rotate(moveY, 0, 0);

            if (move == false)
            {
                move = true;
            }
        }
        //melee/interaction
        if (Input.GetMouseButtonDown(0))
        {
            melee = true;

            //if an object is reachable
            if (reachableObject != null)
            {
                //if object is a harvestable resource (tree, rock, etc.) then mine/harvest (add tool requirements later)
                if (reachableObject.tag.CompareTo("harvestable") == 0)
                {
                    //check there is room in inventory
                    if (OpenInventorySlot(GetHarvestableResource(reachableObject.transform.parent.name)))
                    {
                        //accesses resource script to subtract health
                        reachableObject.transform.parent.GetComponent<MineableResource>().HitResource(harvestRate);

                        //multiple recource collection by harvest rate
                        for (byte i = 0; i < harvestRate; i++)
                        {
                            //adds resource to inventory
                            InventoryAdd(GetHarvestableResource(reachableObject.transform.parent.name), 1);
                            UpdateInteractionText();
                        }
                    }
                    //no room for resource
                    else
                    {
                        Debug.Log("Not room for resource");
                    }
                }
            }
        }
        //move forward
        if (Input.GetKey(KeyCode.W))
        {
            //running
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(0, 0, 4 * Time.deltaTime);
            }
            //walk
            else
            {
                transform.Translate(0, 0, 2 * Time.deltaTime);
            }
            if (move == false)
            {
                move = true;
            }
        }
        //move back
        if (Input.GetKey(KeyCode.S))
        {
            //running
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(0, 0, -4 * Time.deltaTime);
            }
            //walk
            else
            {
                transform.Translate(0, 0, -2 * Time.deltaTime);
            }
            if (move == false)
            {
                move = true;
            }
        }
        //move left
        if (Input.GetKey(KeyCode.A))
        {
            //running
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(-4 * Time.deltaTime, 0, 0);
            }
            //walk
            else
            {
                transform.Translate(-2 * Time.deltaTime, 0, 0);
            }
            if (move == false)
            {
                move = true;
            }
        }
        //move right
        if (Input.GetKey(KeyCode.D))
        {
            //running
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(4 * Time.deltaTime, 0, 0);
            }
            //walk
            else
            {
                transform.Translate(2 * Time.deltaTime, 0, 0);
            }
            if (move == false)
            {
                move = true;
            }
        }
        //interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            //if object can be picked up (resource/item)
            if (reachableObject != null && (reachableObject.tag.CompareTo("resource") == 0 || reachableObject.tag.CompareTo("item") == 0))
            {
                //check if there is room in inventory
                if (OpenInventorySlot(reachableObject.name))
                {
                    InventoryAdd(reachableObject.name, 1);
                    Destroy(reachableObject);
                    reachableObject = null;
                    UpdateInteractionText();
                }
                //no room for resource
                else
                {
                    Debug.Log("No room for resource");
                }
            }
            //crafting/refining interaction
            else if (reachableObject != null && (reachableObject.tag.CompareTo("crafting") == 0 || reachableObject.tag.CompareTo("refining") == 0))
            {
                //furnace
                if (reachableObject.name.Contains("furnaceBody") && InventoryLocateItem("scrap") != 5)
                {
                    Debug.Log("Furnaceing added scrap: " + reachableObject.name);

                    sbyte slot = InventoryLocateItem("scrap");

                    //add scrap to furnace
                    reachableObject.GetComponentInParent<Furnace>().AddRefineItem("scrap", inventory[slot].GetItemAmount());

                    //remove scrap from inventory and update inventory
                    inventory[slot].UpdateAmount(0);
                    InventoryUpdate();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            //crafting interaction
            if (reachableObject != null && (reachableObject.tag.CompareTo("refining") == 0 || reachableObject.tag.CompareTo("schematic") == 0))
            {
                //check if player can get resource from furnace (free/existing slot)
                if (reachableObject.name.Contains("furnaceBody") && OpenInventorySlot(reachableObject.GetComponentInParent<Furnace>().GetRefinedName()))
                {
                    string[] refinedItem = reachableObject.GetComponentInParent<Furnace>().GetRefinedItem().Split(',');
                    
                    //only add to inventory if there is resource(s) to pull from furnace
                    if(refinedItem[0].CompareTo("") != 0)
                    {
                        InventoryAdd(refinedItem[0], sbyte.Parse(refinedItem[1]));
                    }
                }
                //if shematic then set to complete
                else if (reachableObject.tag.CompareTo("schematic") == 0)
                {
                    reachableObject.GetComponentInParent<Schematic>().SetComplete();
                }
            }
        }
        //build mode
        if (Input.GetKeyDown(KeyCode.B))
        {
            //enables build mode
            if (enableBuild == false)
            {
                enableBuild = true;
            }
        }
        //jump
        if (Input.GetKey(KeyCode.Space))
        {
            if (jumped == false)
            {
                characterRigidbody.AddForce(Vector3.up * 8000);
                jumped = true;
            }
        }
        //aim
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Aim not implemented yet :(");
        }
        //speed up
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (Time.timeScale == 10)
            {
                Time.timeScale = 1;
                harvestRate = 1;
                Debug.Log("Speed normal");
            }
            else
            {
                Time.timeScale = 10;
                harvestRate = 5;
                Debug.Log("Speed up");
            }
        }
        //open inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            enableMovement = false;
            enableInventory = true;
        }
        //pauses game
        if (Input.GetKeyDown(KeyCode.Escape) && enableBuild == false && enableInventory == false)
        {
            PauseGame();
        }
        //backpack upgrade
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (hasBackpack == false) {
                BackpackStatus(true);
            }
            else
            {
                BackpackStatus(false);
            }

        }

        //load game
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
        //save game
        else if (Input.GetKeyDown(KeyCode.O))
        {
            SaveGame();
        }
        return;
    }

    //collision detection
    private void OnCollisionEnter(Collision collision)
    {
        //detects collision from the bottom of player
        var normal = collision.GetContact(0).normal;

        //if bottom of collider hits object
        if (normal.y > 0)
        {
            //resets jump
            if (jumped == true)
            {
                jumped = false;
            }
        }
        return;
    }
    //---------------------------------------------------------------------------------Inventory Functions
    //invenetory method
    private void Inventory()
    {
        //detects when inventory key is released (prevents opening and closing at same time)
        if (Input.GetKeyUp(KeyCode.I) && methodKeyHit == false)
        {
            methodKeyHit = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerInventoryUI.gameObject.SetActive(true);
        }
        //closes inventory(resets variables and hides buttons) and re-enables movement
        else if (Input.GetKeyDown(KeyCode.Escape) && methodKeyHit || Input.GetKeyDown(KeyCode.I) && methodKeyHit)
        {
            playerDrop.gameObject.SetActive(false);
            slotSelected = -1;
            methodKeyHit = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            enableInventory = false;
            enableMovement = true;
            playerInventoryUI.gameObject.SetActive(false);
        }
        return;
    }
    //checks for update on inventory slot
    public void InventoryUpdate()
    {
        //update all texts for range of inventorySize
        for (int i = 0; i < inventorySize; i++)
        {
            //if the button is there
            if (inventoryButtons[i] != null)
            {
                inventoryButtons[i].GetComponentInChildren<Text>().text = inventory[i].GetItemName() + " x " + inventory[i].GetItemAmount();
            }
            else
            {
                Debug.Log("inventoryButton[" + i + "] == null");
            }
        }
        return;
    }
    //adds item to inventory
    private void InventoryAdd(string item, sbyte num)
    {
        //first free slot incase item isn't in inventory
        sbyte firstFreeSlot = -1;
        bool slotFound = false;

        for (byte i = 0; i < inventorySize; i++)
        {

            //finds first open slot and marks
            if (inventory[i].GetItemName().CompareTo("") == 0 && firstFreeSlot == -1)
            {
                firstFreeSlot = (sbyte)i;
            }

            //add to existing slot
            else if (inventory[i].GetItemName().CompareTo(item) == 0 && (inventory[i].GetItemAmount() + num) != 100)
            {
                slotFound = true;
                inventory[i].UpdateAmount(num);
                InventoryUpdate();
            }
        }

        //if item isn't in inventory and inventory isn't full
        if (slotFound == false && firstFreeSlot != -1)
        {
            inventory[firstFreeSlot].SetItem(item, num);
            InventoryUpdate();
        }
        return;
    }
    //reads which inventory button was hit
    public void InventoryButton()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;

        //drop the item that was selected
        if (buttonName.CompareTo("playerDrop") == 0)
        {
            if (slotSelected != -1 && inventory[slotSelected].GetItemAmount() > 0)
            {
                GameObject item = Instantiate(((GameObject)(Resources.Load("Prefabs/Resources/" + inventory[slotSelected].GetItemName()))), playerDropSpot.transform.position, Quaternion.identity.normalized);
                item.name = inventory[slotSelected].GetItemName();
                inventory[slotSelected].UpdateAmount(-1);
                InventoryUpdate();
            }
        }
    }
    //check if player can collect resource (look for resource in slots that has less then 100 or open slot)
    private bool OpenInventorySlot(string resource)
    {
        bool openSlot = false;

        //check slots
        for (byte i = 0; i < inventorySize && openSlot == false; i++)
        {
            //if slot is that resource type and isn't full (need to later change resource damage so slot of 99 won't mine x5 since you can only store 1).
            if (inventory[i].GetItemName().CompareTo("") == 0 || (inventory[i].GetItemName().CompareTo(resource) == 0 && inventory[i].GetItemAmount() < 100))
            {
                openSlot = true;
            }
        }
        return openSlot;
    }
    //look for resource in players inventory
    private sbyte InventoryLocateItem(string resource)
    {
        //slot
        sbyte slot = 5;

        //check slots
        for (sbyte i = 0; i < inventorySize; i++)
        {
            if (inventory[i].GetItemName().CompareTo(resource) == 0)
            {
                slot = i;
            }
        }
        return slot;
    }
    //creates buttons for inventory
    public void InventoryButtonLayout(byte numButtons)
    {
        int startingX = -790;
        int startingY = -490;
        //create number of buttons for inventorySize and set their respective text's
        for (byte i = 0; i < inventoryButtons.Length; i++)
        {
            if (i < numButtons && inventoryButtons[i] == null)
            {
                //spawn button
                GameObject button = Instantiate((GameObject)Resources.Load("Prefabs/Player/inventoryButton"));

                //set parent
                button.transform.SetParent(playerInventoryUI.GetComponent<Transform>(), false);

                //set scale
                button.transform.localScale = new Vector3(1, 1, 1);

                //set position of first 4 buttons
                if (i < 4)
                {
                    //set position based on the total number of inventory slots
                    button.transform.localPosition = new Vector3(startingX + Math.Abs((i * 500)), startingY, 0);
                }
                else if (i < 8)
                {
                    //set position based on the total number of inventory slots
                    button.transform.position = new Vector3(startingX + Math.Abs((i * 500)), startingY + 100, 0);
                }
                else
                {
                    //set position based on the total number of inventory slots
                    button.transform.position = new Vector3(startingX + Math.Abs((i * 500)), startingY + 200, 0);
                }

                //this is unique to each button
                byte buttonNumber = i;

                //set button name
                button.name = "inventoryButton" + buttonNumber;

                //reference the button compenent
                Button buttonComponent = button.GetComponentInChildren<Button>();

                //add listener to button
                buttonComponent.onClick.AddListener(() => ButtonClicked(buttonNumber));

                //set button in array
                inventoryButtons[i] = buttonComponent;
            }
            //if at a button that's not needed
            else if (i >= numButtons && inventoryButtons[i] != null)
            {
                Destroy(inventoryButtons[i].gameObject);
                inventoryButtons[i] = null;
                
            }
        }
        
    }
    //set's the text for the inventory slot (should this be UI or Invenotory group?)
    public void SetInventory(byte slot, string name, sbyte amount)
    {
        inventory[slot].SetItem(name, amount);
        return;
    }
    //used for inventory buttons when clicked
    void ButtonClicked(byte buttonNumber)
    {
        Debug.Log("Button hit = " + buttonNumber);

        //set slotSelected
        slotSelected = Convert.ToSByte(buttonNumber);

        //make drop button visible
        playerDrop.gameObject.SetActive(true);

        InventoryButton();
    }
    //---------------------------------------------------------------------------------BackPack Functions
    // functions for backback
    public void BackpackStatus(bool pickedUp)
    {
        //picked up backpack
        if (pickedUp == true && hasBackpack == false)
        {
            hasBackpack = true;
            SetInventorySize(true, 12);
            InventoryButtonLayout(12);
        }
        //dropped backpack
        else if (pickedUp == false && hasBackpack == true)
        {
            hasBackpack = false;
            SetInventorySize(false, 4);
            InventoryButtonLayout(4);
        }
    }
    //---------------------------------------------------------------------------------UI Functions
    //updates text to indicate how to interact/use
    private void UpdateInteractionText()
    {
        string text = "";

        //if there is a object
        if (reachableObject != null)
        {

            //if object is harvestable
            if (reachableObject.tag.CompareTo("harvestable") == 0 && playerInteractText.text.CompareTo("harvestable") != 0)
            {
                //update interation text for harvestable object
                text = "Press m1 to mine " + reachableObject.transform.parent.name + ": " + reachableObject.GetComponentInParent<MineableResource>().GetHealth();
            }
            //if objecct is resource or item
            else if (reachableObject.tag.CompareTo("resource") == 0 || reachableObject.tag.CompareTo("item") == 0)
            {
                //update interation text for item/resource
                text = "Press e to pickup " + reachableObject.name;
            }
            //if object is for crafting
            else if (reachableObject.tag.CompareTo("refining") == 0)
            {
                //furnace text
                if (reachableObject.name.CompareTo("furnaceBody") == 0)
                {
                    text = "Press e to put scrap into the furnace. R to retrive metal.";
                }
                //other crafting
                else
                {
                    //update interation text for item/resource
                    text = "Press e to access " + reachableObject.transform.parent.name + ".";
                }
            }
            //if object is a building schematic
            else if (reachableObject.tag.CompareTo("schematic") == 0)
            {
                text = "Press e to put resources into " + reachableObject.transform.parent.name + ".";
            }
        }
        //only update text if it's different
        if (playerInteractText.text.CompareTo(text) != 0)
        {
            playerInteractText.text = text;
        }
    }
    //---------------------------------------------------------------------------------Building Functions
    //building method
    private void Building()
    {
        //indicate build mode was enabled (prevent instant disabling)
        if (Input.GetKeyUp(KeyCode.B) && methodKeyHit == false)
        {
            methodKeyHit = true;
            playerModeText.text = "Press Esc or B to exit build mode";
        }
        //disables build mode (and destroys structure so not left in world)
        else if (Input.GetKeyDown(KeyCode.Escape) && methodKeyHit || Input.GetKeyDown(KeyCode.B) && methodKeyHit)
        {
            methodKeyHit = false;
            enableBuild = false;
            Destroy(buildingGameObject);
            buildingGameObject = null;
            buildingSelected = 0;
            playerModeText.text = "Press B to enter build mode";
        }
        //place building
        if (buildingGameObject != null && Input.GetKeyDown(KeyCode.Mouse0))
        {
            //set layers for object- raycast can hit
            buildingGameObject.layer = 0;
            foreach (Transform child in buildingGameObject.transform)
            {
                child.gameObject.layer = 0;
            }
            //solar
            if(buildingSelected == 1)
            {
                //add power source to list
                scriptW.AddPowerSource(buildingGameObject);
                //add schematic to building
                buildingGameObject.AddComponent<Schematic>();
            }
            //generator
            else if (buildingSelected == 2)
            {
                scriptW.AddPowerSource(buildingGameObject);
                //add schematic to building
                buildingGameObject.AddComponent<Schematic>();
            }
            //furnace
            else if (buildingSelected == 3)
            {
                scriptW.AddRefiningObject(buildingGameObject);
            }
            
            //null refernece so building stays in world
            buildingGameObject = null;
        }
        //select solar panel
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {     
            //if not the same building then destroy and null for new one
            if (buildingSelected != 1)
            {
                Destroy(buildingGameObject);
                buildingGameObject = null;
            }
            buildingSelected = 1;
        }

        //select generator
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //if not the same building then destroy and null for new one
            if (buildingSelected != 2)
            {
                Destroy(buildingGameObject);
                buildingGameObject = null;
            }
            buildingSelected = 2;
        }
        //select generator
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //if not the same building then destroy and null for new one
            if (buildingSelected != 3)
            {
                Destroy(buildingGameObject);
                buildingGameObject = null;
            }
            buildingSelected = 3;
        }

        //if position isn't zero and a building is selected then spawn
        if (buildingSelected > 0 && buildingGameObject == null)
        {
            //solar panel
            if (buildingSelected == 1)
            {
                //spawns building, finds it, then removes (Clone) from name
                buildingGameObject = Instantiate(Resources.Load("Prefabs/Power/solar_panel") as GameObject, new Vector3(0, -100, 0), Quaternion.identity);
                buildingGameObject.name = "solar_panel" + scriptW.GetSolarCount();
            }
            //generator
            else if (buildingSelected == 2)
            {
                //spawns building, finds it, then removes (Clone) from name
                buildingGameObject = Instantiate(Resources.Load("Prefabs/Power/generator") as GameObject, new Vector3(0, -100, 0), Quaternion.identity);
                buildingGameObject.name = "generator" + scriptW.GetGeneratorCount();
            }
            //furnace
            else if (buildingSelected == 3)
            {
                //spawns building, finds it, then removes(Clone) from name
                buildingGameObject = Instantiate(Resources.Load("Prefabs/Refining/furnace") as GameObject, new Vector3(0, -100, 0), Quaternion.identity);

                //set both parent name and body name (body name causeing issues with furnace interaction so now there's diff names so the rechable object doesn't point to the same furnace)
                buildingGameObject.name = "furnace" + scriptW.GetFurnaceCount();
                GameObject.Find("furnaceBody").name = "furnaceBody" + scriptW.GetFurnaceCount();
            }
            //set layer for object (ignores raycast so doesn't hit its self)
            buildingGameObject.layer = 2;

            //set layer for each child object in building (ignores raycast so doesn't hit its self)
            foreach (Transform child in buildingGameObject.transform)
            {
                child.gameObject.layer = 2;
            }
        }
        //if building is spawned enabled rotation
        if (buildingGameObject != null)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                buildingGameObject.transform.Rotate(0, 1.5f, 0);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                buildingGameObject.transform.Rotate(0, -1.5f, 0);
            }
        }
        return;
    }
    //---------------------------------------------------------------------------------Menu Functions
    public void QuitGame()
    {
        Application.Quit();
        return;
    }
    public void LoadGame()
    {
        scriptW.LoadGame();
        ResumeGame();
        return;
    }
    public void SaveGame()
    {
        scriptW.SaveGame();
        return;
    }
    public void ResumeGame()
    {
        //resume time at normal rate
        Time.timeScale = 1;

        //enables movement and disables pause
        enableMovement = true;
        enablePause = false;
        methodKeyHit = false;

        //resume world
        scriptW.ResumeWorld();

        //hide and lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //diable UI
        playerPauseMenuUI.gameObject.SetActive(false);
        return;
    }
    public void PauseGame()
    {
        //pause time
        Time.timeScale = 0;

        //disable movement and enable pause
        enableMovement = false;
        enablePause = true;

        //pause world
        scriptW.PauseWorld();

        //show and unlock cursor 
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        //enable UI
        playerPauseMenuUI.gameObject.SetActive(true);
        return;
    }
    private void Paused()
    {
        //prevents resuming as soon as enter is hit
        if (Input.GetKeyDown(KeyCode.Escape) && methodKeyHit == false)
        {
            methodKeyHit = true;
        }
        //resumes game when esc is hit
        else if (Input.GetKeyDown(KeyCode.Escape) && methodKeyHit == true)
        {
            methodKeyHit = false;
            enablePause = false;
            ResumeGame();
        }
        return;
    }
    private void OptionsMenu()
    {

    }
    //---------------------------------------------------------------------------------Get/Set Functions
    public float GetHealth()
    {
        return health;
    }
    public void SetHealth(float health)
    {
        this.health = health;
    }
    public string GetInventoryName(sbyte slot)
    {
        return inventory[slot].GetItemName();
    }
    public sbyte GetInventoryAmount(sbyte slot)
    {
        return inventory[slot].GetItemAmount();
    }
    //gets rescouce type for harvestable resource
    private string GetHarvestableResource(string name)
    {
        string resource = "";

        //tree
        if (name.CompareTo("tree") == 0)
        {
            resource = "wood";
        }
        //rock
        else if (name.CompareTo("largeRock") == 0)
        {
            resource = "rock";
        }
        //metal
        else if (name.CompareTo("rustedMetal") == 0)
        {
            resource = "scrap";
        }
        return resource;
    }
    //---------------------------------------------------------------------------------Player's Wellbeing Functions
    void ConditionUpdate()
    {
        //update food and water
        UpdateFood(false, 0.01f);
        UpdateWater(false, 0.01f);

        //update UI for food and water
        playerFood.text = food.ToString() + "%";
        playerWater.text = water.ToString() + "%";
        conditionUpdateTime = 1;
    }
}