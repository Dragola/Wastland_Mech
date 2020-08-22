using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public byte solarCount = 0;
    public byte generatorCount = 0;
    public List<GameObject> powerSources = new List<GameObject>();
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
            powerStatusUpdate();
        }
        else if ((dayDuration > 3600 && dayDuration < 100) && solarEnabled == true)
        {
            solarEnabled = false;
            powerStatusUpdate();
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
    public void addPowerSource(GameObject powerSource)
    {
        //increase solar count
        if (powerSource.name.Contains("solar"))
        {
            solarCount++;
        }
        //increase generator count
        else if (powerSource.name.Contains("generator"))
        {        
            generatorCount++;
        }
        //add powerSource to list and update generator status
        powerSources.Add(powerSource);
        powerStatusUpdate();
        return;
    }
    public void removePowerSource(GameObject solarPanel)
    {
        //decrease solar count and remove from list
        solarCount--;
        powerSources.Remove(solarPanel);
        return;
    }

    public byte getSolarCount()
    {
        return solarCount;
    }

    public void powerStatusUpdate()
    {
        foreach (GameObject powerSource in powerSources)
        {
            if (powerSource.name.Contains("solar"))
            {
                powerSource.GetComponent<SolarPower>().solarEnabled(solarEnabled);
                //Debug.Log("Enabled solar generation for: " + powerSource.name);
            }
            else if (powerSource.name.Contains("generator"))
            {
                powerSource.GetComponent<GeneratorPower>().generatorEnabled(true);
                //Debug.Log("Enabled solar generation for: " + powerSource.name);
            }

        }
        return;
    }
}