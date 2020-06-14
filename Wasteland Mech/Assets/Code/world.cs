using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public byte solarCount = 0;
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

        //sun rotation
        sun.transform.RotateAround(Vector3.zero, Vector3.right, 1 * Time.deltaTime);

        //
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

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Power
    public void addSolar()
    {
        solarCount++;
    }
    public void removeSolar()
    {
        solarCount++;
        return;
    }
    public byte getSolarCount()
    {
        return solarCount;
    }
}