using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    public bool generaterEnabled = false;
    public float power = 0;

    // Update is called once per frame
    void Update()
    {
        //generate power
        if (generaterEnabled)
        {
            power += Time.deltaTime;
        }
    }

    public void generatorStatus(bool status)
    {
        generaterEnabled = status;
        Debug.Log(this.name + "has updated status");
    }
}
