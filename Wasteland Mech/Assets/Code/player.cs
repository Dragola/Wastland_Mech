using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    //scripts
    private World scriptW = null;

    //player
    public float moveX = 0;
    public float moveY = 0;
    private Rigidbody playerR = null;
    public int meeleTime = 15;
    bool meele = false;
    private bool jumped = false;
    private GameObject rayPosition = null;
    private bool move = false;
    public byte harvestRate = 1;

    //camera's
    private Camera camFirst;
    private Camera camThird;
    private bool firstPersonCamera = true;

    //UI components
    //private Text playerHealth = null;
    //private Text playerFood = null;
    //private Text playerWater = null;
    private Button playerSlot0 = null;
    private Button playerSlot1 = null;
    private Button playerSlot2 = null;
    private Button playerSlot3 = null;
    public Button playerDrop = null;
    private Text playerInteractText = null;

    //bool aim = false;

    //movement and inventory
    private bool enableMovement = true;
    private bool enableInventory = false;

    //inventory
    public GameObject reachableObject = null;
    public string[] inventorySlot = null;
    public byte[] inventorySize = null;
    public bool inventoryKeyHit = false;
    public sbyte slotSelected = -1;

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
        playerR = GetComponent<Rigidbody>();
        rayPosition = GameObject.Find("rayPosition");

        //camera's
        camFirst = GameObject.Find("cameraFP").GetComponent<Camera>();
        camThird = GameObject.Find("cameraTP").GetComponent<Camera>();

        //inventory
        playerSlot0 = GameObject.Find("playerSlot0").GetComponent<Button>();
        playerSlot1 = GameObject.Find("playerSlot1").GetComponent<Button>();
        playerSlot2 = GameObject.Find("playerSlot2").GetComponent<Button>();
        playerSlot3 = GameObject.Find("playerSlot3").GetComponent<Button>();
        playerDrop = GameObject.Find("playerDrop").GetComponent<Button>();
        playerInteractText = GameObject.Find("playerInteractText").GetComponent<Text>();
        playerSlot0.enabled = false;
        playerSlot1.enabled = false;
        playerSlot2.enabled = false;
        playerSlot3.enabled = false;
        playerDrop.gameObject.SetActive(false);

        //settings before game starts (change later when saving is implemented)
        camThird.gameObject.SetActive(false);
        playerR.GetComponent<MeshRenderer>().enabled = false;
        Application.targetFrameRate = 60; //should create or find a method that keeps everything running at certain rate (Time.deltatime?) to not limit FPS

        //fill inventorySlot with "" for default names and inventory Size with 0
        for (byte i = 0; i < inventorySlot.Length; i++)
        {
            inventorySlot[i] = "";
            inventorySize[i] = 0;
        }

        //loads data from save-WIP
        //LoadGame();
    }
    //updates once per frame
    void Update()
    {
        //player movement
        Movement(enableMovement);

        //Inventory
        Inventory(enableInventory);

    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ Physics and constants
    void FixedUpdate()
    {
        if (move)
        {
            //interaction
            if (Physics.Raycast(rayPosition.transform.position, rayPosition.transform.forward, out RaycastHit hit, (float)1.5))
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
            return;
        }

        //meele
        //melee timer
        if (meele == true)
        {
            meeleTime -= 1;
        }
        return;
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Movement
    //player movement and interaction
    public bool Movement(bool status)
    {
        //if movement is enabled 
        if (status)
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
                camFirst.transform.Rotate(moveY, 0, 0);
                camThird.transform.Rotate(moveY, 0, 0);
                rayPosition.transform.Rotate(moveY, 0, 0);

                if (move == false)
                {
                    move = true;
                }
            }
            //melee detection
            if (Input.GetMouseButtonDown(0))
            {
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
                            reachableObject.transform.parent.GetComponent<Resource>().HitResource(harvestRate);

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
            //interact
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
            //jumping
            if (Input.GetKey(KeyCode.Space))
            {
                if (jumped == false)
                {
                    playerR.AddForce(Vector3.up * 7000);
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
                playerR.GetComponent<MeshRenderer>().enabled = false;
                camThird.gameObject.SetActive(false);
                camFirst.gameObject.SetActive(true);
                firstPersonCamera = true;
            }
            //third person 
            else if (Input.GetKeyDown(KeyCode.Tab) && firstPersonCamera == true)
            {
                playerR.GetComponent<MeshRenderer>().enabled = true;
                camFirst.gameObject.SetActive(false);
                camThird.gameObject.SetActive(true);
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
                enableInventory = true;
                enableMovement = false;
            }
            //quits game
            if (Input.GetKeyDown(KeyCode.L))
            {
                SaveGame();
                Application.Quit();
            }

        }
        return false;
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
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Load && Save Game-WIP
    private void SaveGame()
    {
        //path to the File
        string destination = "Assets/Resources/save.txt";
        FileStream file = null;

        //Checks if file exists and pulls it, othwerwise creates a new file 
        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }
        //File writer
        StreamWriter write = new StreamWriter(file);

        //adds the player position to be written (Queue's it)
        write.WriteLine("" + gameObject.transform.position.x + "," + gameObject.transform.position.y + "," + gameObject.transform.position.z);

        //pushes the output to the File
        write.Flush();

        //closes File
        file.Close();

        return;
    }
    
    private void LoadGame()
    {    
        return;     
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Inventory

    //the invenetory method (interacting, opening and closing)
    private void Inventory(bool status)
    {
        //if inventory is enabled
        if (status)
        {
            //detects when inventory key is released (prevents opening and closing at same time)
            if (Input.GetKeyUp(KeyCode.I) && inventoryKeyHit == false)
            {
                inventoryKeyHit = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

            }
            //closes inventory(resets variables and hides buttons) and re-enables movement
            else if (Input.GetKeyDown(KeyCode.Escape) && inventoryKeyHit || Input.GetKeyDown(KeyCode.I) && inventoryKeyHit)
            {
                playerDrop.gameObject.SetActive(false);
                slotSelected = -1;
                enableMovement = true;
                enableInventory = false;
                inventoryKeyHit = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        return;
    }

    //checks for update on inventory slot
    private void InventoryUpdate(byte slot)
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
        //Debug.Log("InventoryAdd called with item = " + item);
        //first free slot incase item isn't in inventory
        sbyte firstFreeSlot = -1;
        bool slotFound = false;

        for (byte i = 0; i < inventorySlot.Length; i++)
        {

            //finds first open slot and marks
            if (inventorySlot[i].CompareTo("") == 0 && firstFreeSlot == -1)
            {
                //Debug.Log("First free slot = " + i);
                firstFreeSlot = (sbyte)i;
            }

            //add to existing slot
            else if (inventorySlot[i].CompareTo(item) == 0 && inventorySize[i] != 100)
            {
                //Debug.Log("Existing item: " + item);
                slotFound = true;
                inventorySize[i]++;
                InventoryUpdate(i);
            }
        }

        //if item isn't in inventory and inventory isn't full
        if (slotFound == false && firstFreeSlot != -1)
        {
            //Debug.Log("New item: " + item);
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
                GameObject item = Instantiate(((GameObject)(Resources.Load("Prefabs/Resources/" + inventorySlot[slotSelected]))), GameObject.Find("playerDropSpot").GetComponent<Transform>().position, Quaternion.identity.normalized);
                item.name = inventorySlot[slotSelected];
                inventorySize[slotSelected]--;
                InventoryUpdate((byte)slotSelected);
            }
        }
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Resource type
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
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Inventory Space
    //check if player can collect resource (inventory is full)
    private bool OpenInventorySlot(string resource)
    {
        //Debug.Log("Running Openinventory Slot");
        bool isOpen = false;
        //Debug.Log("Resource to chekc for: " + resource);
        //check slots
        for (byte i=0; i< inventorySize.Length; i++)
        {
            //if slot is that resource type and isn't full (need to later change resource damage so slot of 99 won't mine x5 since you can only store 1).
            if(inventorySlot[i].CompareTo("") == 0 || (inventorySlot[i].CompareTo(resource) == 0 && inventorySize[i] < 100))
            {
                isOpen = true;
                //Debug.Log("Slot " + i + "is open/has room.");
            }
        }
        return isOpen;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Interaction Text
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
}