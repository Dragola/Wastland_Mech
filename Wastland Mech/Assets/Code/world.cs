using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class world : MonoBehaviour
{
    //world
    GameObject sun = null;
    public ushort DayDuration = 3600;

    //player
    Transform player = null;

    //power
    private byte power = 0; //get from solar panels
    private byte power_building_index = 0;
    private byte power_pylon_index = 0;
    Text powerT;
    private byte num_solar_panels = 0;
    private byte num_generators = 0;

    //building
    public sbyte new_building = -1;
    private byte base_count = 0;

    //lists of objects
    public List<GameObject> prefab_builds = new List<GameObject>();
    public List<GameObject> base_builds = new List<GameObject>();
    public List<GameObject> pwr_builds = new List<GameObject>();
    public List<GameObject> pwr_pylons = new List<GameObject>();


    // Use this for initialization
    void Start()
    {
        //sun
        sun = GameObject.Find("sun");

        //player
        player = GameObject.Find("player").GetComponent<Transform>();

        //UI
        powerT = GameObject.Find("power_UI").GetComponent<Text>();

        //buildings
        prefab_builds.Add(Resources.Load("Prefabs/Energy/solar_panel") as GameObject);
        prefab_builds.Add(Resources.Load("Prefabs/Energy/generator") as GameObject);
        prefab_builds.Add(Resources.Load("Prefabs/Energy/pylon_short") as GameObject);
        prefab_builds.Add(Resources.Load("Prefabs/Energy/energy_storage") as GameObject);
        prefab_builds.Add(Resources.Load("Prefabs/Base/base") as GameObject);
        prefab_builds.Add(Resources.Load("Prefabs/Base/wall") as GameObject);

    }

    // Update is called once per frame (60fps = 60calls)
    void Update()
    {
        //sun rotation
        sun.transform.RotateAround(Vector3.zero, Vector3.right, 1 * Time.deltaTime);

        //if new building was placed (triggered in player)
        if (new_building >= 0)
        {
            //solar_panel
            if (new_building == 0)
            {
                pwr_builds.Add(GameObject.FindGameObjectWithTag("pre_building"));
                pwr_builds[power_building_index].gameObject.name = ("Solar_Panel");
                pwr_builds[power_building_index].gameObject.tag = "power_source";
                num_solar_panels += 1;
                power_building_index += 1;
            }
            //generator
            else if (new_building == 1)
            {
                pwr_builds.Add(GameObject.FindGameObjectWithTag("pre_building"));
                pwr_builds[power_building_index].gameObject.name = ("Generator");
                pwr_builds[power_building_index].gameObject.tag = "power_source";
                num_generators += 1;
                power_building_index += 1;
            }

            //pylon_short
            else if (new_building == 2)
            {
                pwr_pylons.Add(GameObject.FindGameObjectWithTag("pre_building"));
                pwr_pylons[power_building_index].gameObject.name = ("Pylon_Short");
                pwr_pylons[power_building_index].gameObject.tag = "Untagged";
                power_pylon_index += 1;
            }
            //energy_stroage
            else if (new_building == 3)
            {
                pwr_builds.Add(GameObject.FindGameObjectWithTag("pre_building"));
                pwr_builds[power_building_index].gameObject.name = ("Energy_Stroage");
                pwr_builds[power_building_index].gameObject.tag = "Untagged";
                power_building_index += 1;
            }

            //base
            else if (new_building == 4)
            {
                base_builds.Add(GameObject.FindGameObjectWithTag("pre_building"));
                base_builds[base_count].gameObject.name = "base";
                base_builds[base_count].gameObject.tag = "Untagged";
                base_count += 1;
            }
            //wall
            else if (new_building == 5)
            {
                base_builds.Add(GameObject.FindGameObjectWithTag("pre_building"));
                base_builds[base_count].gameObject.name = "wall";
                base_builds[base_count].gameObject.tag = "Untagged";
                base_count += 1;
            }

            new_building = -1;
        }

        //if at least one power generator placed
        if (pwr_builds.Count > 0)
        {
            //updates UI
            powerT.text = "" + power + "|" +  "1000 Power";
        }
    }
    //runs at fixed rate
    void FixedUpdate()
    {
        DayDuration -= 1;
    }
}

       