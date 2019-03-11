using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
/*
 * Finish adding player related actions (climbing and interaction)
 * Add inventory- later
 * 
 */
class GameSave
{
    //have to be public to be accessible from outside struct
    public Vector3 playerPOS = new Vector3(0, 0, 0);
    public List<GameObject> player_base_buildings = null;
    public List<GameObject> player_power_buildings = null;
}
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
    public float player_to_ground = 0;

    //camera's
    private Camera cam_first;
    private Camera cam_third;
    private bool firstPCam = true;

    //UI components
    Text waterText = null;
    Text foodText = null;
    Text tempText = null;
    Text building_UI = null;
    int foodValue = 100;
    int waterValue = 100;
    int tempValue = 0;

    //bool aim = false;
    public bool spawnB = false;
    public bool placed = false;
    public bool wireCheck = false;
    public bool move = false;

    //building
    sbyte building_select = -1;
    bool buildM = false;
    Transform buildSpot = null;
    Vector3 buildPlace = new Vector3(0, 0, 0);
    GameObject tempB = null;

    //changes
    private byte player_change = 0; //0 = nothing, 1 = buildmode

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
        Movement(1);
        //UI updates
        UI(player_change);
        //build mode
        Build(player_change);
    }
    void FixedUpdate()
    {
        //meele
        //melee timer
        if (meele == true)
        {
            meeletime -= 1;
        }
        if (move == true)
        {
            //checks distance from ground/object below
            if (player_change == 1)
            {
                Ray ray = new Ray(buildSpot.position, -buildSpot.up);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    distance = hit.distance;
                }
            }
        }
    }
    private byte UI(byte enabled)
    {
        //update build mode UI
        if (enabled == 1)
        {
            building_UI.text = "Build Mode: Active\nTo select a building press one of the number on the top of your keyboard\n1. Solar Panel\n2. Generator\n3. Base\n4. Wall";
        }
        else
        {
            building_UI.text = "Build Mode: Inactive\nPress B to enter build mode";
        }
        return 0;
    }
    public byte Movement(byte enabled)
    {
        if (enabled == 1)
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

            //moves camera's y-axis and building position
            if (movey != 0)
            {
                //rotates camera's (up/down)
                cam_first.transform.Rotate(movey, 0, 0);

                cam_third.transform.Rotate(movey, 0, 0);

                if (move == false)
                {
                    move = true;
                }
            }
            //melee detection
            if (Input.GetMouseButtonDown(0) & buildM == false)
            {

                Ray ray = new Ray(this.transform.position, transform.forward);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 4);
                {

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
            //jump
            if (Input.GetKey(KeyCode.Space))
            {
                if (jumped == false)
                {
                    playerR.AddForce(Vector3.up * 250);
                    jumped = true;
                }
            }
            //once landed
            if (jumped == true)
            {
                if (player_to_ground == 1)
                {
                    Debug.Log("Landed");
                    jumped = false;
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
            //build mode
            else if (Input.GetKeyDown(KeyCode.B))
            {
                //enabled buld mode
                if (player_change == 0)
                {
                    player_change = 1;
                }
                //disable build mode
                else
                {
                    player_change = 0;
                }
            }
            //speed up
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                if (Time.timeScale == 10)
                {
                    Time.timeScale = 10;
                    Debug.Log("Speed normal");
                }
                else
                {
                    Time.timeScale = 10;
                    Debug.Log("Speed up");
                }

            }
            //quits game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            
        }
        return 0;
    }
    public byte Build(byte enabled)
    {
        //build mode active
        if (enabled == 1)
        {
            //select  solar_panel
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                building_select = 0;
                if (spawnB == true)
                {
                    Destroy(tempB.gameObject);
                    spawnB = false;
                }

            }
            //select generator
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                building_select = 1;
                if (spawnB == true)
                {
                    Destroy(tempB.gameObject);
                    spawnB = false;
                }
            }
            //select pylon_short
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                building_select = 2;
                if (spawnB == true)
                {
                    Destroy(tempB.gameObject);
                    spawnB = false;
                }
            }
            //select generator
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                building_select = 3;
                if (spawnB == true)
                {
                    Destroy(tempB.gameObject);
                    spawnB = false;
                }
            }
            //select base
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                building_select = 4;
                if (spawnB == true)
                {
                    Destroy(tempB.gameObject);
                    spawnB = false;
                }
            }
            //select wall
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                building_select = 5;
                if (spawnB == true)
                {
                    Destroy(tempB.gameObject);
                    spawnB = false;
                }
            }

            //building select
            if (spawnB == false && building_select != -1)
            {
                tempB = Instantiate(scriptE.prefab_builds[building_select], buildPlace, Quaternion.Euler(0, 0, 0));
                spawnB = true;
            }
            //once building is spawned
            if (spawnB == true)
            {
                //moves building
                buildPlace = new Vector3(buildSpot.position.x, buildSpot.position.y - distance, buildSpot.position.z);
                tempB.transform.position = buildPlace;

                //snap points WIP
                Ray ray = new Ray(cam_first.transform.position, cam_first.transform.forward);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 20);
                {
                    if (hit.collider != null && hit.collider.tag == "Snap Point")
                    {
                        print("Snap");
                    }
                }

                //places building	
                if (Input.GetMouseButtonDown(0) && spawnB == true)
                {
                    tempB.gameObject.tag = "pre_building";
                    tempB = null;
                    spawnB = false;
                    scriptE.new_building = building_select;
                }
                //rotate left
                if (Input.GetKey(KeyCode.Q) && spawnB == true)
                {
                    tempB.transform.RotateAround(Vector3.zero, Vector3.up, 1);
                }
                //rotate right
                else if (Input.GetKey(KeyCode.E) && spawnB == true)
                {
                    tempB.transform.RotateAround(Vector3.zero, Vector3.down, 1);
                }
            }
        }
        //despawns and resets build mode
        else
        {
            //deletes building
            if (tempB != null)
            {
                Destroy(tempB);
                tempB = null;
            }
            spawnB = false;
        }
        return 0;
    }
    //saves game (needs to actually save buildings, objects, etc.)
    static void SaveGame()
    {
        GameSave save = new GameSave();
        save.playerPOS.x = 1000;
        save.playerPOS.y = 1000;
        save.playerPOS.z = 0;

        string destination = "Assets/Resources/save.dat"; //where file is located
        FileStream file;

        //if file exists, write to
        if (File.Exists(destination))
            file = File.OpenWrite(destination);

        //if file doesn't exist, create file
        else
            file = File.Create(destination);

        //converts to binary format and puts into file
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, save);
        file.Close();
    }

    //loads file
    private void LoadGame()
    {
        /*GameSave save = new GameSave();

        string destination = "Assets/Resources/save.dat";
        FileStream file;

        if (File.Exists(destination))
            file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        save = (GameSave)bf.Deserialize(file);
        file.Close();

        GameObject playerObject = GameObject.Find("Player");

        playerObject.transform.position = save.playerPOS;
    */
        //cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //keeps in middle (prevents clicking out of game)

        //scripts
        scriptE = GameObject.Find("world").GetComponent<World>();

        //player
        playerR = GetComponent<Rigidbody>();
        buildSpot = GameObject.Find("build_spot").GetComponent<Transform>();

        //camera's
        cam_first = GameObject.Find("camera_FP").GetComponent<Camera>();
        cam_third = GameObject.Find("camera_TP").GetComponent<Camera>();

        //UI
        waterText = GameObject.Find("water_UI").GetComponent<Text>();
        foodText = GameObject.Find("food_UI").GetComponent<Text>();
        tempText = GameObject.Find("temp_UI").GetComponent<Text>();
        building_UI = GameObject.Find("build_UI").GetComponent<Text>();

        //settings before game starts (change later when saving is implemented)
        cam_third.gameObject.SetActive(false);
        playerR.GetComponent<MeshRenderer>().enabled = false;
        waterText.text = "Water: " + waterValue.ToString();
        foodText.text = "Food: " + foodValue.ToString();
        tempText.text = tempValue.ToString() + "C";
        building_UI.text = "Build Mode: Inactive\nPress B to enter build mode";
        Application.targetFrameRate = 60;
    }
    public void OnCollisionEnter(Collision collision)
    { 
        if(collision.gameObject.tag == "grounded")
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
}