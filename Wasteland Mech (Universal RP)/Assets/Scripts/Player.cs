using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    //scripts
    private World scriptW = null;

    //player
    public byte health = 100;
    public float moveX = 0;
    public float moveY = 0;
    private Rigidbody playerRigidbody = null;
    public float meleeTime = 3;
    public bool melee = false;
    private bool jumped = false;
    private GameObject playerRaycastPoint = null;
    private bool move = false;
    public byte harvestRate = 1;
    public byte buildingSelected = 0;
    public GameObject buildingGameObject = null;
    private GameObject playerDropSpot = null;

    //camera's
    private Camera playerCamera;
    private bool firstPersonCamera = true;

    //UI components
    private Text playerHealth = null;
    private Text playerFood = null;
    private Text playerWater = null;
    private Button playerSlot0 = null;
    private Button playerSlot1 = null;
    private Button playerSlot2 = null;
    private Button playerSlot3 = null;
    public Button playerDrop = null;
    private Text playerInteractText = null;
    private Text playerModeText = null;
    private Canvas playerInventoryUI = null;
    private Canvas playerPauseMenuUI = null;
    private Canvas playerOptionsUI = null;

    //bool aim = false;

    //movement and inventory
    public bool enableMovement = true;
    public bool enableInventory = false;
    public bool enablePause = false;
    public bool enableBuild = false;
    public bool methodKeyHit = false;

    //inventory
    public GameObject reachableObject = null;
    public string[] inventorySlot = null;
    public byte[] inventorySize = null;
    public sbyte slotSelected = -1;

    public int frameRate = 0;
    public Text avgFrameRateText = null;

    //runs before awake
    private void Awake()
    {
        //have to size arrays after initialization
        inventorySlot = new string[4];
        inventorySize = new byte[4];
    }
    // Use this for initialization
    void Start()
    {
        //cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //keeps in middle (prevents clicking out of game)

        //scripts
        scriptW = GameObject.Find("world").GetComponent<World>();

        //player
        playerRigidbody = GameObject.Find("player").GetComponent<Rigidbody>();
        playerRaycastPoint = GameObject.Find("playerRaycastPoint");
        playerDropSpot = GameObject.Find("playerDropSpot");

        //camera's
        playerCamera = GameObject.Find("playerCamera").GetComponent<Camera>();

        //UI
        playerHealth = GameObject.Find("playerHealth").GetComponent<Text>();
        playerFood = GameObject.Find("playerFood").GetComponent<Text>();
        playerWater = GameObject.Find("playerWater").GetComponent<Text>();
        playerSlot0 = GameObject.Find("playerSlot0").GetComponent<Button>();
        playerSlot1 = GameObject.Find("playerSlot1").GetComponent<Button>();
        playerSlot2 = GameObject.Find("playerSlot2").GetComponent<Button>();
        playerSlot3 = GameObject.Find("playerSlot3").GetComponent<Button>();
        playerDrop = GameObject.Find("playerDrop").GetComponent<Button>();
        playerInteractText = GameObject.Find("playerInteract").GetComponent<Text>();
        playerModeText = GameObject.Find("playerMode").GetComponent<Text>();
        playerInventoryUI = GameObject.Find("playerInventoryUI").GetComponent<Canvas>();
        playerPauseMenuUI = GameObject.Find("playerPauseMenuUI").GetComponent<Canvas>();
        playerOptionsUI = GameObject.Find("playerOptionsUI").GetComponent<Canvas>();
        avgFrameRateText = GameObject.Find("playerFPS").GetComponent<Text>();
        playerInventoryUI.gameObject.SetActive(false);
        playerPauseMenuUI.gameObject.SetActive(false);
        playerOptionsUI.gameObject.SetActive(false);
        playerDrop.gameObject.SetActive(false);


        //fill inventorySlot with "" for default names and inventory Size with 0
        for (byte i = 0; i < inventorySlot.Length; i++)
        {
            inventorySlot[i] = "";
            inventorySize[i] = 0;
        }
    }
    //updates once per frame
    void Update()
    {
        //Movement
        if (enableMovement)
        {
            Movement();
        }
        //Inventory
        else if (enableInventory)
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

        //frame rate (not sure how accurate it is)
        frameRate = (int)(Time.frameCount / Time.time);
        avgFrameRateText.text = frameRate.ToString() + " FPS";
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ Physics and constants
    void FixedUpdate()
    {

    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Movement/Controls
    //player movement and interaction
    public void Movement()
    {
        //resets move detected
        move = false;

        //gets's mouse input
        moveX = Input.GetAxis("Mouse X");
        moveY = -Input.GetAxis("Mouse Y");

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
                            InventoryAdd(GetHarvestableResource(reachableObject.transform.parent.name));
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
                    InventoryAdd(reachableObject.name);
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
                playerRigidbody.AddForce(Vector3.up * 8000);
                jumped = true;
            }
        }
        //aim
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Aim not implemented yet :(");
        }
        //first person
        if (Input.GetKeyDown(KeyCode.Tab) && firstPersonCamera == false)
        {
            //playerRigidbody.GetComponent<MeshRenderer>().enabled = false;
            firstPersonCamera = true;
        }
        //third person 
        else if (Input.GetKeyDown(KeyCode.Tab) && firstPersonCamera == true)
        {
            //playerRigidbody.GetComponent<MeshRenderer>().enabled = true;
            firstPersonCamera = false;
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

        //raycast for interaction (disabled if in build mode)
        if (move && enableBuild == false)
        {
            //interaction
            if (Physics.Raycast(playerRaycastPoint.transform.position, playerRaycastPoint.transform.forward, out RaycastHit hit, (float)1.5))
            {
                //get interacted object if not already
                if (reachableObject == null)
                {
                    reachableObject = hit.collider.gameObject;
                }
                //new interacted object
                else if (reachableObject != hit.collider.gameObject)
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
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Inventory
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
    public void InventoryUpdate(byte slot)
    {
        if (slot == 0)
        {
            //if slot has item
            if (inventorySize[0] > 0)
            {
                //enables button if inventory slot has item
                if (playerSlot0.enabled == false)
                {
                    playerSlot0.enabled = true;
                }
                //update slot
                playerSlot0.GetComponentInChildren<Text>().text = "" + inventorySlot[0] + "x" + inventorySize[0];
            }
            else
            {
                //enables button if inventory slot has item
                if (playerSlot0.enabled == true)
                {
                    playerSlot0.enabled = false;
                }

                inventorySlot[0] = "";
                //update slot
                playerSlot0.GetComponentInChildren<Text>().text = "";
            }

        }
        else if (slot == 1)
        {
            //if slot has item
            if (inventorySize[1] > 0)
            {
                //enables button if inventory slot has item
                if (playerSlot1.enabled == false)
                {
                    playerSlot1.enabled = true;
                }
                //update slots text with item and amount
                playerSlot1.GetComponentInChildren<Text>().text = "" + inventorySlot[1] + "x" + inventorySize[1];
            }
            else
            {
                //enables button if inventory slot has item
                if (playerSlot1.enabled == true)
                {
                    playerSlot1.enabled = false;
                }
                inventorySlot[1] = "";
                //update slots text with item and amount
                playerSlot1.GetComponentInChildren<Text>().text = "";
            }
        }
        else if (slot == 2)
        {
            //if slot has item
            if (inventorySize[2] > 0)
            {
                //enables button if inventory slot has item
                if (playerSlot2.enabled == false)
                {
                    playerSlot2.enabled = true;
                }
                //update slots text with item and amount
                playerSlot2.GetComponentInChildren<Text>().text = "" + inventorySlot[2] + "x" + inventorySize[2];
            }
            else
            {
                //enables button if inventory slot has item
                if (playerSlot2.enabled == true)
                {
                    playerSlot2.enabled = false;
                }
                inventorySlot[2] = "";
                //update slots text with item and amount
                playerSlot2.GetComponentInChildren<Text>().text = "";
            }
        }
        else if (slot == 3)
        {
            //if slot has item
            if (inventorySize[3] > 0)
            {
                //enables button if inventory slot has item
                if (playerSlot3.enabled == false)
                {
                    playerSlot3.enabled = true;
                }
                //update slots text with item and amount
                playerSlot3.GetComponentInChildren<Text>().text = "" + inventorySlot[3] + "x" + inventorySize[3];
            }
            else
            {
                //enables button if inventory slot has item
                if (playerSlot3.enabled == true)
                {
                    playerSlot3.enabled = false;
                }
                inventorySlot[3] = "";
                //update slots text with item and amount
                playerSlot3.GetComponentInChildren<Text>().text = "";
            }
        }
        return;
    }

    //adds item to inventory
    private void InventoryAdd(string item)
    {
        //first free slot incase item isn't in inventory
        sbyte firstFreeSlot = -1;
        bool slotFound = false;

        for (byte i = 0; i < inventorySlot.Length; i++)
        {

            //finds first open slot and marks
            if (inventorySlot[i].CompareTo("") == 0 && firstFreeSlot == -1)
            {
                firstFreeSlot = (sbyte)i;
            }

            //add to existing slot
            else if (inventorySlot[i].CompareTo(item) == 0 && inventorySize[i] != 100)
            {
                slotFound = true;
                inventorySize[i]++;
                InventoryUpdate(i);
            }
        }

        //if item isn't in inventory and inventory isn't full
        if (slotFound == false && firstFreeSlot != -1)
        {
            inventorySlot[firstFreeSlot] = item;
            inventorySize[firstFreeSlot]++;
            InventoryUpdate((byte)firstFreeSlot);
        }
        return;
    }

    //reads which inventory button was hit
    public void InventoryButton()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;

        //selects the inventory slot and enables the drop button
        if (buttonName.CompareTo("playerSlot0") == 0)
        {
            slotSelected = 0;

            playerDrop.gameObject.SetActive(true);
        }
        else if (buttonName.CompareTo("playerSlot1") == 0)
        {
            slotSelected = 1;

            playerDrop.gameObject.SetActive(true);
        }
        else if (buttonName.CompareTo("playerSlot2") == 0)
        {
            slotSelected = 2;
            playerDrop.gameObject.SetActive(true);
        }
        else if (buttonName.CompareTo("playerSlot3") == 0)
        {
            slotSelected = 3;
            playerDrop.gameObject.SetActive(true);
        }

        //drop the item that was selected
        else if (buttonName.CompareTo("playerDrop") == 0)
        {
            if (slotSelected != -1 && inventorySize[slotSelected] > 0)
            {
                GameObject item = Instantiate(((GameObject)(Resources.Load("Prefabs/Resources/" + inventorySlot[slotSelected]))), playerDropSpot.transform.position, Quaternion.identity.normalized);
                item.name = inventorySlot[slotSelected];
                inventorySize[slotSelected]--;
                InventoryUpdate((byte)slotSelected);
            }
        }
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Resource type
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
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Inventory Space
    //check if player can collect resource (inventory is full)
    private bool OpenInventorySlot(string resource)
    {
        bool openSlot = false;

        //check slots
        for (byte i = 0; i < inventorySize.Length; i++)
        {
            //if slot is that resource type and isn't full (need to later change resource damage so slot of 99 won't mine x5 since you can only store 1).
            if (inventorySlot[i].CompareTo("") == 0 || (inventorySlot[i].CompareTo(resource) == 0 && inventorySize[i] < 100))
            {
                openSlot = true;
            }
        }
        return openSlot;
    }
    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Interaction Text
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
                text = "Press m1 to mine " + reachableObject.transform.parent.name;
            }
            //if objecct is resource or item
            else if (reachableObject.tag.CompareTo("resource") == 0 || reachableObject.tag.CompareTo("item") == 0)
            {
                //update interation text for item/resource
                text = "Press e to pickup " + reachableObject.name;
            }
            //if object is for crafting
            else if (reachableObject.tag.CompareTo("crafting") == 0)
            {
                //update interation text for item/resource
                text = "Press e to access " + reachableObject.transform.parent.name;
            }
        }
        //only update text if it's different
        if (playerInteractText.text.CompareTo(text) != 0)
        {
            playerInteractText.text = text;
        }
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Building
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
            if (buildingSelected == 1)
            {
                //add power script- may be a way to add script to prefab for blender model
                buildingGameObject.AddComponent<SolarPower>();
            }
            else if (buildingSelected == 2)
            {
                //add power script- may be a way to add script to prefab for blender model
                buildingGameObject.AddComponent<GeneratorPower>();
            }
            //set layers for object- raycast can hit
            buildingGameObject.layer = 0;
            foreach (Transform child in buildingGameObject.transform)
            {
                child.gameObject.layer = 0;
            }
            //add powerSource to list
            scriptW.addPowerSource(buildingGameObject);

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

        //if position isn't zero and a building is selected then spawn
        if (buildingSelected > 0 && buildingGameObject == null)
        {
            //solar panel
            if (buildingSelected == 1)
            {
                //spawns building, finds it, then removes (Clone) from name
                buildingGameObject = Instantiate(Resources.Load("Prefabs/Power/solar_panel") as GameObject, new Vector3(0, -100, 0), Quaternion.identity);
                buildingGameObject.name = "solar_panel" + scriptW.getSolarCount();

                //sets the tag of the gameobject
                buildingGameObject.tag = "power";
            }
            //generator
            else if (buildingSelected == 2)
            {
                //spawns building, finds it, then removes (Clone) from name
                buildingGameObject = Instantiate(Resources.Load("Prefabs/Power/generator") as GameObject, new Vector3(0, -100, 0), Quaternion.identity);
                buildingGameObject.name = "generator" + scriptW.getSolarCount();
                
                //sets the tag of the gameobject
                buildingGameObject.tag = "power";
            }
            //set layer for object (ignores raycast so doesn't hit its self)
            buildingGameObject.layer = 2;

            //set layer for each child object in building (ignores raycast so doesn't hit its self)
            foreach (Transform child in buildingGameObject.transform)
            {
                child.gameObject.layer = 2;
            }
        }
        //if player moved and raycast hit then update (or move) building location
        if (move && Physics.Raycast(playerRaycastPoint.transform.position, playerRaycastPoint.transform.forward, out RaycastHit hit, 10) && buildingGameObject != null)
        {
            //set building position
            buildingGameObject.transform.position = hit.point;
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
    public byte GetHealth()
    {
        return health;
    }
    public void SetHealth(byte health)
    {
        this.health = health;
    }
    public string GetinventoryItem(byte slot)
    {
        return inventorySlot[slot];
    }
    public byte GetinventoryAmount(byte slot)
    {
        return inventorySize[slot];
    }
    public void SetInventory(byte slot, string name, byte amount)
    {
        this.inventorySlot[slot] = name;
        this.inventorySize[slot] = amount;
        return;
    }
}