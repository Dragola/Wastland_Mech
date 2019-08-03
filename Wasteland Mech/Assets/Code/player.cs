using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    //scripts
    private World scriptW = null;

    //player
    public float movex = 0;
    public float movey = 0;
    private float distance = 0;
    private int rotationAngle = 0;
    private Rigidbody playerR = null;
    public int meeleTime = 15;
    bool meele = false;
    private bool jumped = false;
    private GameObject rayPosition = null;
    private bool move = false;

    //camera's
    private Camera cam_First;
    private Camera cam_Third;
    private bool firstPersonCamera = true;

    //UI components
    //private Text player_Health = null;
    //private Text player_Food = null;
    //private Text player_Water = null;
    private Text player_Slot0 = null;
    private Text player_Slot1 = null;
    private Text player_Slot2 = null;
    private Text player_Slot3 = null;

    //bool aim = false;

    //enable and updating
    private byte update_UI = 0; //if UI needs to update
    private bool enable_Movement = true; //enables the player to move and interact
    private bool enable_Inventory = false;

    //inventory
    private GameObject reachableObject = null;
    public sbyte[] inventory_Slot = new sbyte[4];
    public byte[] inventory_Size = new byte[4];
    public string[] ITEM_NAMES = { "Wood","Rock"};

    // Use this for initialization
    void Start()
    {
        //loads previous save (if any) and sets everything up
        LoadGame();
    }
    //updates once per frame
    void Update()
    {
        //player movement
        Movement(enable_Movement);

        //Inventory
        Inventory(enable_Inventory);

    }
    void FixedUpdate()
    {
        if (move)
        {
            //interaction
            if (Physics.Raycast(rayPosition.transform.position, rayPosition.transform.forward, out RaycastHit hit, (float)1.5))
            {
                //if you interact with an object
                if (reachableObject == null)
                {
                    reachableObject = hit.collider.gameObject;
                    Debug.Log(reachableObject.name);
                }
                //if you interact with a new object
                else if (reachableObject != hit.collider.gameObject)
                {
                    reachableObject = hit.collider.gameObject;
                    Debug.Log(reachableObject.name);
                }
            }
            //if not interacting with anything then set reachable_object to null
            else
            {
                reachableObject = null;
            }
        }
        //meele
        //melee timer
        if (meele == true)
        {
            meeleTime -= 1;
        }

        return;
    }

    //checks for update on inventory slot
    private void InventoryUpdate(byte slot)
    {
        if (slot == 0)
        {
            player_Slot0.text = "" + ItemToString((byte)inventory_Slot[0]) + "x" + inventory_Size[0];
        }
        else if (slot == 1)
        {
            player_Slot1.text = "" + ItemToString((byte)inventory_Slot[1]) + "x" + inventory_Size[1];
        }
        else if (slot == 2)
        {
            player_Slot2.text = "" + ItemToString((byte)inventory_Slot[2]) + "x" + inventory_Size[2];
        }
        else if (slot == 3)
        {
            player_Slot3.text = "" + ItemToString((byte)inventory_Slot[3]) + "x" + inventory_Size[3];
        }
        return;
    }

    //adds item to inventory
    private void InventoryAdd(byte item)
    {

        bool slotFound = false;

        for (byte i = 0; i < inventory_Slot.Length && !slotFound; i++)
        {
            //finds first open slot
            if (inventory_Slot[i] < 0 && !slotFound)
            {
                inventory_Slot[i] = (sbyte)item;
                inventory_Size[i]++;
                slotFound = true;
                InventoryUpdate(i);
            }

            //add to existing slot
            else if (inventory_Slot[i] == item)
            {
                inventory_Size[i]++;
                slotFound = true;
                InventoryUpdate(i);
            }
        }
        return;
    }

    //returns string of item
    private string ItemToString(byte item)
    {
        return ITEM_NAMES[item];
    }

    //player movement and interaction
    public bool Movement(bool status)
    {
        //if movement is enabled 
        if (status)
        {
            //resets move detected
            move = false;

            //gets's mouse input
            movex = Input.GetAxis("Mouse X");
            movey = -Input.GetAxis("Mouse Y");

            //rotates player
            if (movex != 0)
            {
                //rotates player (left/right)
                transform.Rotate(0, movex, 0);
                move = true;
            }

            //moves camera's y-axis and ray_position
            if (movey != 0)
            {
                //rotates camera's (up/down)
                cam_First.transform.Rotate(movey, 0, 0);

                cam_Third.transform.Rotate(movey, 0, 0);

                rayPosition.transform.Rotate(movey, 0, 0);

                if (move == false)
                {
                    move = true;
                }
            }
            //melee detection
            if (Input.GetMouseButtonDown(0))
            {

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
                //if the object is a resource then 
                if (reachableObject != null && reachableObject.tag.CompareTo("resource") == 0)
                {
                    InventoryAdd(reachableObject.GetComponentInParent<Resource>().HitResource());
                }
            }
            //jumping
            if (Input.GetKey(KeyCode.Space))
            {
                if (jumped == false)
                {
                    playerR.AddForce(Vector3.up * 250);
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
                cam_Third.gameObject.SetActive(false);
                cam_First.gameObject.SetActive(true);
                firstPersonCamera = true;
            }
            //third person 
            else if (Input.GetKeyDown(KeyCode.Tab) && firstPersonCamera == true)
            {
                playerR.GetComponent<MeshRenderer>().enabled = true;
                cam_First.gameObject.SetActive(false);
                cam_Third.gameObject.SetActive(true);
                firstPersonCamera = false;
            }
            //speed up
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (Time.timeScale == 10)
                {
                    Time.timeScale = 1;
                    Debug.Log("Speed normal");
                }
                else
                {
                    Time.timeScale = 10;
                    Debug.Log("Speed up");
                }

            }
            //open inventory
            if (Input.GetKeyUp(KeyCode.I))
            {
                Debug.Log("Open Inventory");
                enable_Inventory = true;
                enable_Movement = false;
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

    //Saves the game (WIP)
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

    //Loads objects and save file (if present)
    private void LoadGame()
    {

        //cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //keeps in middle (prevents clicking out of game)

        //scripts
        scriptW = GameObject.Find("world").GetComponent<World>();

        //player
        playerR = GetComponent<Rigidbody>();
        rayPosition = GameObject.Find("ray_Position");

        //camera's
        cam_First = GameObject.Find("camera_FP").GetComponent<Camera>();
        cam_Third = GameObject.Find("camera_TP").GetComponent<Camera>();

        //inventory
        player_Slot0 = GameObject.Find("player_Slot0").GetComponent<Text>();
        player_Slot1 = GameObject.Find("player_Slot1").GetComponent<Text>();
        player_Slot2 = GameObject.Find("player_Slot2").GetComponent<Text>();
        player_Slot3 = GameObject.Find("player_Slot3").GetComponent<Text>();

        //Loads previous save (if it exists
        string destination = "Assets/Resources/save.txt";
        FileStream file = null;

        //if file exists then load
        if (File.Exists(destination))
        {
            //get file
            file = File.OpenRead(destination);
            StreamReader read = new StreamReader(file);

            //gets line from file
            string line = read.ReadLine();

            //player position
            string[] playerData = line.Split(',');
            Vector3 playerPOS = new Vector3(float.Parse(playerData[0]), float.Parse(playerData[1]), float.Parse(playerData[2]));
            gameObject.transform.SetPositionAndRotation(playerPOS, Quaternion.Euler(0, 0, 0));

            file.Close();
        }

        //settings before game starts (change later when saving is implemented)
        cam_Third.gameObject.SetActive(false);
        playerR.GetComponent<MeshRenderer>().enabled = false;
        Application.targetFrameRate = 60; //should create or find a method that keeps everything running at certain rate (Time.deltatime?) to not limit FPS

        return;
    }

    //the invenetory method (interacting, opening and closing)
    private void Inventory(bool status)
    {
        //if inventory is enabled
        if (status)
        {

            //closes inventory and re-enables movement
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("Closing Inventory");
                enable_Movement = true;
                enable_Inventory = false;
            }
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

}