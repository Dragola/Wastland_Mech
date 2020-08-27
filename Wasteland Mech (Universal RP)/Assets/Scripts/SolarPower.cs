using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPower : MonoBehaviour
{
    public bool solarGenerate = false;
    public float power = 0;

    // Update is called once per frame
    void Update()
    {
        //generate power
        if (solarGenerate)
        {
            power += Time.deltaTime;
        }
    }

    public void solarEnabled(bool status)
    {
        solarGenerate = status;
        //Debug.Log(this.name + "has updated status");
    }
}
