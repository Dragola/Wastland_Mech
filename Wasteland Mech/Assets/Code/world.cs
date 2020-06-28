using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public byte solarCount = 0;
    public List<GameObject> solarPanels = new List<GameObject>();
    public bool solarEnabled = false;
    
    //world
    GameObject sun = null;
    public float dayDuration = 3600;

    // Use this for initialization
    void Start()
    {
        //sun
        sun = GameObject.Find("sun");
    }

    // Update is called once per frame (60fps = 60calls)
    void Update()
    {
        //enable solar panels to generate
        if((dayDuration < 3600 && dayDuration > 100) && solarEnabled == false)
        {
            solarEnabled = true;
            generatorStatus();
        }
        else if ((dayDuration > 3600 && dayDuration < 100) && solarEnabled == true)
        {
            solarEnabled = false;
            generatorStatus();
        }
        //sun rotation
        sun.transform.RotateAround(Vector3.zero, Vector3.right, 1 * Time.deltaTime);

        //clock
        dayDuration -= Time.deltaTime;

        //reset clock/day
        if(dayDuration <= 0)
        {
            dayDuration = 3600;
        }

    }
    //runs at fixed rate
    void FixedUpdate()
    {
        
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Solar Power
    public void addSolar(GameObject solarPanel)
    {
        //increase solar count and add to list
        solarCount++;
        solarPanels.Add(solarPanel);
        generatorStatus();
        return;
    }
    public void removeSolar(GameObject solarPanel)
    {
        //decrease solar count and remove from list
        solarCount--;
        solarPanels.Remove(solarPanel);
        return;
    }

    public byte getSolarCount()
    {
        return solarCount;
    }

    public void generatorStatus()
    {
        foreach (GameObject solar in solarPanels)
        {
            solar.GetComponent<Power>().generatorStatus(solarEnabled);
            Debug.Log("Enabled solar generation for: " + solar.name);
        }
        return;
    }
}