using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
/*
 * Add spawning method (keep track and spawn objects/loot)
 * Add resources
 * 
 */
public class World : MonoBehaviour
{
    //world
    GameObject sun = null;
    public ushort dayDuration = 3600;

    //player
    Transform player = null;

    //power
    private byte power = 0; //gnerate based on number of power sources
    Text powerT;

    //lists of objects
    public List<GameObject> power_prefabs = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        //sun
        sun = GameObject.Find("Sun");

        //player
        player = GameObject.Find("Player").GetComponent<Transform>();

        //UI
        powerT = GameObject.Find("Power_UI").GetComponent<Text>();

        //buildings
        power_prefabs.Add(Resources.Load("Prefabs/Power/solar_panel") as GameObject);
        power_prefabs.Add(Resources.Load("Prefabs/Power/generator") as GameObject);
        power_prefabs.Add(Resources.Load("Prefabs/Power/energy_storage") as GameObject);

    }

    // Update is called once per frame (60fps = 60calls)
    void Update()
    {
        //sun rotation
        sun.transform.RotateAround(Vector3.zero, Vector3.right, 1 * Time.deltaTime);

        //reset clock
        if(dayDuration <= 0)
        {
            dayDuration = 3600;
        }

    }
    //runs at fixed rate
    void FixedUpdate()
    {
        dayDuration -= 1;
    }
}

       