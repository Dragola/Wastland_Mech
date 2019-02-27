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
    private  int MAXPOWER = 1000; // max power (can be changed via upgrades/storage)
    private int power_division = 100; //divides into usable power
    private int usable_power = 0; //actual power
    byte power = 0;
    bool solarEnable = false;
    public byte new_power_building = 0;
    byte solar_count = 0;
    byte generator_count = 0;
    Text powerT;

    //lists
    public List<GameObject> base_buildings = new List<GameObject>();
    public List<GameObject> solar_buildings = new List<GameObject>();
    public List<GameObject> generator_buildings = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        sun = GameObject.Find("sun");

        //player
        player = GameObject.Find("player").GetComponent<Transform>();

        //UI
        powerT = GameObject.Find("power_UI").GetComponent<Text>();

        //buildings
        base_buildings.Add(Resources.Load("Prefabs/Base/base") as GameObject);
        base_buildings.Add(Resources.Load("Prefabs/Base/wall") as GameObject);
        base_buildings.Add(Resources.Load("Prefabs/Energy/solar_panel") as GameObject);
        base_buildings.Add(Resources.Load("Prefabs/Energy/generator") as GameObject);
        base_buildings.Add(Resources.Load("Prefabs/Energy/cable") as GameObject);
    }

    // Update is called once per frame (60fps = 60calls)
    void Update()
    {
        //sun rotation
        sun.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 60, player.transform.position.z);

        //if new power building was placed (triggered in player)
        if (new_power_building > 0)
        {
            //solar panel
            if (new_power_building == 1)
            {
                Debug.Log("looking for solar panel");
                solar_buildings.Add(GameObject.FindGameObjectWithTag("pre_building"));
                solar_buildings[solar_count].gameObject.name = ("Solar Panel" + solar_count);
                solar_buildings[solar_count].gameObject.tag = "Untagged";
                solar_count += 1;
            }
            new_power_building = 0;
            
        }

        //if at least one power generator placed
        if (solar_count > 0 || generator_count > 0)
        {

            //if not at power max capacity
            if (power <= MAXPOWER)
            {
                //generates power
                power += solar_count;
                power += generator_count;

                //converts to usable energy
                if (power >= power_division)
                {
                    power -= 100;
                    usable_power += 1;

                    //updates UI
                    powerT.text = power.ToString() + "(" + usable_power + ")" + "|" + MAXPOWER.ToString() + " Power";
                }
                
            }
        }
    }
    void FixedUpdate()
    {
        DayDuration -= 1;
    }
}

       