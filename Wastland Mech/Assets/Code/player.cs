using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    //scripts
    private World scriptE = null;

    //player
    public float movex = 0;
    public float movey = 0;
    private float distance = 0;
    private int rotation_angle = 0;
    private Rigidbody playerR = null;
    public int meeletime = 15;
    bool meele = false;
    private bool jumped = false;
    private GameObject ray_position = null;
    public bool move = false;


    //camera's
    private Camera cam_first;
    private Camera cam_third;
    private bool firstPCam = true;

    //UI components
    GameObject mainUI = null;
    Text waterText = null;
    Text foodText = null;
    Text tempText = null;
    int foodValue = 100;
    int waterValue = 100;
    int tempValue = 0;
    GameObject inventoryUI = null;

    //bool aim = false;

    //enable and updating
    private bool update_UI = false; //if UI needs to update
    private bool enable_movement = true; //enables the player to move and interact
    private bool enable_inventory = false;

    //inventory
    public GameObject reachable_object = null;
    public string[] inventory_objects = new  string[5];

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
        Movement(enable_movement);

        //UI updates (maybe run in a different script?)- don't need to technically
        UI(update_UI);

        //Inventory
        Inventory(enable_inventory);

    }
    void FixedUpdate()
    {
        //interaction
        RaycastHit hit;
        if(Physics.Raycast(ray_position.transform.position, ray_position.transform.forward, out hit, 1))
        {
            //if you interact with an object
            if(reachable_object == null)
            {
                reachable_object = hit.collider.gameObject;
            }
            //if you interact with a new object
            else if(reachable_object != hit.collider.gameObject)
            {
                reachable_object = hit.collider.gameObject;
            }
        }
        //if not interacting with anything then set reachable_object to null
        else
        {
            reachable_object = null;
        }

        //meele
        //melee timer
        if (meele == true)
        {
            meeletime -= 1;
        }
    }
    private bool UI(bool changed)
    {

        return true;
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
                cam_first.transform.Rotate(movey, 0, 0);

                cam_third.transform.Rotate(movey, 0, 0);

                ray_position.transform.Rotate(movey, 0, 0);

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
            if (Input.GetKeyDown(KeyCode.Tab) && firstPCam == false)
            {
                playerR.GetComponent<MeshRenderer>().enabled = false;
                cam_third.gameObject.SetActive(false);
                cam_first.gameObject.SetActive(true);
                firstPCam = true;
            }
            //third person 
            else if (Input.GetKeyDown(KeyCode.Tab) && firstPCam == true)
            {
                playerR.GetComponent<MeshRenderer>().enabled = true;
                cam_first.gameObject.SetActive(false);
                cam_third.gameObject.SetActive(true);
                firstPCam = false;
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
            if (Input.GetKey(KeyCode.I))
            {
                mainUI.SetActive(false);
                inventoryUI.SetActive(true);
                enable_inventory = true;
                enable_movement = false;
            }
            //quits game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SaveGame();
                Application.Quit();
            }
            
        }
        return false;
    }

    //Saves the game
    private void SaveGame()
    {
        //path to the File
        string destination = "Assets/Resources/save.txt";
        FileStream file = null;

        //Cehcks if File exists, it it does then grab, othwerwise create new File 
        if (File.Exists(destination)) {
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
    }

    //Loads objects and save file (if present)
    private void LoadGame()
    {

        //cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //keeps in middle (prevents clicking out of game)

        //scripts
        scriptE = GameObject.Find("World").GetComponent<World>();

        //player
        playerR = GetComponent<Rigidbody>();
        ray_position = GameObject.Find("Ray Position");

        //camera's
        cam_first = GameObject.Find("Camera_FP").GetComponent<Camera>();
        cam_third = GameObject.Find("Camera_TP").GetComponent<Camera>();

        //UI
        mainUI = GameObject.Find("MainUI");
        inventoryUI = GameObject.Find("InventoryUI");
        waterText = GameObject.Find("Water_UI").GetComponent<Text>();
        foodText = GameObject.Find("Food_UI").GetComponent<Text>();
        tempText = GameObject.Find("Temp_UI").GetComponent<Text>();
        inventoryUI.SetActive(false);

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
            gameObject.transform.SetPositionAndRotation(playerPOS, Quaternion.Euler(0,0,0));

            file.Close();
        }

        //settings before game starts (change later when saving is implemented)
        cam_third.gameObject.SetActive(false);
        playerR.GetComponent<MeshRenderer>().enabled = false;
        waterText.text = "Water: " + waterValue.ToString();
        foodText.text = "Food: " + foodValue.ToString();
        tempText.text = tempValue.ToString() + "C";
        Application.targetFrameRate = 60; //should create or find a method that keeps everything running at certain rate (Time.deltatime?) to not limit FPS
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
                Debug.Log("Closed Inventory");
                inventoryUI.SetActive(false);
                mainUI.SetActive(true);
                enable_movement = true;
                enable_inventory = false;
            }
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        //detects collision from the bottom of player
        var normal = collision.GetContact(0).normal;
        if (normal.y > 0)
        {
            //resets jump
            if (jumped == true)
            {
                jumped = false;
            }
        }
    }
    
}