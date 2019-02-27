using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class GameSave
{
    //have to be public to be accessible from outside struct
    public Vector3 playerPOS = new Vector3(0, 0, 0);
    public List<GameObject> buildings = null;
    public List<GameObject> zombies = null;
}
public class player : MonoBehaviour
{
    //scripts
    private world scriptE = null;

    //player
    public float movex = 0;
    public float movey = 0;
    private float distance = 0;
    private int rotation_angle = 0;
    private Rigidbody playerR = null;

    //camera's
    public Camera cam_first;
    public Camera cam_third;
    private bool firstPCam = true;


    //UI components
    Text waterText = null;
    Text foodText = null;
    Text tempText = null;
    Text buildingSelect = null;
    int foodValue = 100;
    int waterValue = 100;
    int tempValue = 0;
    int building = 0;
    public int meeletime = 15;
    bool meele = false;
    bool buildM = false;

    //bool aim = false;
    public bool spawnB = false;
    public bool placed = false;
    public bool wireCheck = false;
    public bool move = false;

    //building select
    Transform buildSpot = null;
    Vector3 buildPlace = new Vector3(0,0,0);
    GameObject tempB = null;

    // Use this for initialization
    void Start()
    {
        //loads previous save (if any)
        //LoadGame();

        //cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //keeps in middle (prevents clicking out of game)

        //---------------------------------------references

        //scripts
        scriptE = GameObject.Find("world").GetComponent<world>();
 
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
        buildingSelect = GameObject.Find("build_UI").GetComponent<Text>();

        //settings before game starts (change later when saving is implemented)
        cam_third.gameObject.SetActive(false);
        buildingSelect.gameObject.SetActive(false);
        playerR.GetComponent<MeshRenderer>().enabled = false;
        Application.targetFrameRate = 60;
        waterText.text = "Water: " + waterValue.ToString();
        foodText.text = "Food: " + foodValue.ToString();
        tempText.text = tempValue.ToString() + "C";
    }
    void Update()
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
        //------------------------------------------Movement-----------------------------------------------------

        //move forward
        if (Input.GetKey(KeyCode.W))
        {
            //sprint
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
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(0, 0, -4 * Time.deltaTime);
            }
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
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(-4 * Time.deltaTime, 0, 0);
            }
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

            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(4 * Time.deltaTime, 0, 0);
            }
            else
            {
                transform.Translate(2 * Time.deltaTime, 0, 0);
            }
            if (move == false)
            {
                move = true;
            }
        }
        //aim
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Aim not implemented yet :(");
        }
        //---------------------------------------------------------------------------------------

        //quits game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
        //enables build mode
        if (Input.GetKeyDown(KeyCode.B) && buildM == false)
        {
            buildM = true;
            buildingSelect.gameObject.SetActive(true);
        }
        //disables build mode
        else if (Input.GetKeyDown(KeyCode.B) && buildM == true)
        {
            buildM = false;
            spawnB = false;
            building = 0;
            if (tempB != null)
            {
                Destroy(tempB.gameObject);
            }
            buildingSelect.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {

        }
        //calls build void
        if (buildM == true)
        {
            Build();
        }
        //speed up
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale = 2;
        }
    }
    void FixedUpdate()
    {
        //WIP (temperature + food + water)
        //waterV -= 1;
        //foodV -= 1;
        //water.text = "Water: " + waterV.ToString ();
        //food.text = "Food: " + foodV.ToString ();
        //tempCV -= 1;
        //tempFV -= 1;
        //tempC.text = tempCV.ToString () + " C /";

        //meele
        //melee timer
        if (meele == true)
        {
            meeletime -= 1;
        }
        if (move == true)
        {
            //checks distance from ground/object below
            if (spawnB == true)
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
    //building
    void Build()
    {
        //select Base
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            building = 1;
            if (spawnB == true)
            {
                Destroy(tempB.gameObject);
                spawnB = false;
            }
        }
        //select Wall
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            building = 2;
            if (spawnB == true)
            {
                Destroy(tempB.gameObject);
                spawnB = false;
            }
        }
        //select Solar Panel
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            building = 3;
            if (spawnB == true)
            {
                Destroy(tempB.gameObject);
                spawnB = false;
            }
        }
        //select Generator
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            building = 4;
            if (spawnB == true)
            {
                Destroy(tempB.gameObject);
                spawnB = false;
            }
        }
        //select Wire
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            building = 5;
            if (spawnB == true)
            {
                Destroy(tempB.gameObject);
                spawnB = false;
            }
        }
        //building select
        if (spawnB == false && building != 0)
        {

            switch (building)
            {
                //base
                case 1:
                    tempB = Instantiate(scriptE.base_buildings[0], buildPlace, Quaternion.Euler(0, 0, 0));
                    spawnB = true;
                    break;
                //wall
                case 2:
                    tempB = Instantiate(scriptE.base_buildings[1], buildPlace, Quaternion.Euler(0, 0, 0));
                    spawnB = true;
                    break;
                //solar panel
                case 3:
                    tempB = Instantiate(scriptE.base_buildings[2], buildPlace, Quaternion.Euler(0, 0, 0));
                    spawnB = true;
                    break;
                //generator
                case 4:
                    tempB = Instantiate(scriptE.base_buildings[3], buildPlace, Quaternion.Euler(0, 0, 0));
                    spawnB = true;
                    break;
                //wire
                case 5:
                    tempB = Instantiate(scriptE.base_buildings[4], buildPlace, Quaternion.Euler(0, 0, 0));
                    spawnB = true;
                    break;
            }
        }
        if (spawnB == true)
        {
            //moves building
            buildPlace = new Vector3(buildSpot.position.x, buildSpot.position.y - distance, buildSpot.position.z);
            tempB.transform.position = buildPlace;

            //building snap points
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
                
                //chagne to a byte (indicates type of building that was spawned)
                scriptE.new_power_building = 1;
            }
            //rotate left
            if (Input.GetKey(KeyCode.Q) && spawnB == true)
            {
                tempB.transform.Rotate(0, rotation_angle += 90, 0);
            }
            //rotate right
            else if (Input.GetKey(KeyCode.E) && spawnB == true)
            {
                tempB.transform.Rotate(0, rotation_angle -= 90, 0);
            }
        }
    }
    //saves game (needs to actually save position, buildings and zombies)
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
    static void LoadGame()
    {
        GameSave save = new GameSave();

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
    }
    
    
}


