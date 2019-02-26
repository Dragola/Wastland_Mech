using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class world : MonoBehaviour
{
    //world
    GameObject sun = null;
    int DayDuration = 3600;

    //player
    Transform player = null;

    //power
    private  int MAXPOWER = 1000;
    private int power_division = 120;
    private int usable_power = 0;
    private byte totalPowerGen = 0;
    private byte power = 0;
    bool solarEnable = false;
    bool powerGen = false;
    Text powerT;
    public bool new_power_building = false;

    //zombie
    public int zombiesSpawned = 0;
    public int maxZombies = 50;
    int zombieSpawnTime = 60;
    public GameObject[] zombieSpawn;
    public bool zombieSpawnTrue = false;

    //lists
    public List<GameObject> zombieList = new List<GameObject>();
    public List<GameObject> all_wires = new List<GameObject>();
    public List<GameObject> buildings = new List<GameObject>();
    public List<GameObject> power_buildings = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        sun = GameObject.Find("sun");

        //player
        player = GameObject.Find("player").GetComponent<Transform>();

        //UI
        powerT = GameObject.Find("power_UI").GetComponent<Text>();

        //zombies
        zombieList.Add(Resources.Load("Prefabs/Zombie") as GameObject);
        zombieSpawn = GameObject.FindGameObjectsWithTag("ZombieSpawn");

        //buildings
        buildings.Add(Resources.Load("Prefabs/Base/base") as GameObject);
        buildings.Add(Resources.Load("Prefabs/Base/wall") as GameObject);
        buildings.Add(Resources.Load("Prefabs/Energy/solar_panel") as GameObject);
        buildings.Add(Resources.Load("Prefabs/Energy/generator") as GameObject);
        buildings.Add(Resources.Load("Prefabs/Energy/cable") as GameObject);
    }

    // Update is called once per frame (60fps = 60calls)
    void Update()
    {
        //sun rotation
        sun.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 60, player.transform.position.z);

        if (new_power_building == true)
        {
            //need to change for any power building
            power_buildings.Add(GameObject.Find("solar_panel(Clone)"));
            totalPowerGen += 1;
            new_power_building = false;
        }

        //if at least one building placed
        if (power_buildings.Count > 0)
        {

            //adds power and resets counter
            if (powerGen == true && power <= MAXPOWER)
            {
                power += totalPowerGen;

                if (power >= power_division)
                {
                    power = 0;
                    usable_power += 1;
                }
                powerT.text = power.ToString() + "(" + usable_power + ")" + "|" + MAXPOWER.ToString() + " Power";
            }
            //prevents generation once maxed
            if (power >= MAXPOWER && powerGen == true)
            {
                powerGen = false;
            }
            else if (power < MAXPOWER && powerGen == false)
            {
                powerGen = true;
            }
        }
    }
    void FixedUpdate()
    {
        DayDuration -= 1;
        zombieSpawnTime -= 1;
    }
}

       