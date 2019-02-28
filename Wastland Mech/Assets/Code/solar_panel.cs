using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class solar_panel : MonoBehaviour
{
    world scriptW = null;
    public int power = 0;
    // Start is called before the first frame update
    void Start()
    {
        scriptW = GameObject.Find("world").GetComponent<world>();
    }

    // Update is called once per frame
    void Update()
    {
        //generated power
        if (scriptW.DayDuration < 3500 && scriptW.DayDuration > 300)
        {
            power += 1;
        }
    }
}
