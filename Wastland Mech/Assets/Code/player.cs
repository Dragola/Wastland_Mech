using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
    int foodValue = 100;
    int waterValue = 100;
    int tempValue = 0;

    //bool aim = false;
    public bool spawnB = false;
    public bool placed = false;
    public bool wireCheck = false;
    public bool move = false;

    //changes
    private bool UI_change = false;
    private bool player_change = false;

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
        Movement(true);
        //UI updates
        UI(false);
    }
    void FixedUpdate()
    {
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
    public bool Movement(bool changed)
    {
        if (changed)
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
            if (Input.GetMouseButtonDown(0))
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
                SaveGame();
                Application.Quit();
            }
            
        }
        return false;
    }
    private void SaveGame()
    {
        //file located
        string destination = "Assets/Resources/save.txt";
        FileStream file = null;

        //if file exists then load
        if (File.Exists(destination)) {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }
        //writes to file
        StreamWriter write = new StreamWriter(file);

        //player position
        write.WriteLine("" + gameObject.transform.position.x + "," + gameObject.transform.position.y + "," + gameObject.transform.position.z);

        //pushes the output to the file
        write.Flush();

        file.Close();
    }

    //loads file
    private void LoadGame()
    {

        //cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; //keeps in middle (prevents clicking out of game)

        //scripts
        scriptE = GameObject.Find("world").GetComponent<World>();

        //player
        playerR = GetComponent<Rigidbody>();

        //camera's
        cam_first = GameObject.Find("camera_FP").GetComponent<Camera>();
        cam_third = GameObject.Find("camera_TP").GetComponent<Camera>();

        //UI
        waterText = GameObject.Find("Water_UI").GetComponent<Text>();
        foodText = GameObject.Find("Food_UI").GetComponent<Text>();
        tempText = GameObject.Find("Temp_UI").GetComponent<Text>();

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
        Application.targetFrameRate = 60;
    }
    public void OnCollisionEnter(Collision collision)
    {
        //jumping
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