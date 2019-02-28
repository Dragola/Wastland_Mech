using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class energy_storage : MonoBehaviour
{
    public bool stor_full = false;
    public int stor_power = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //full
        if (stor_power >= 1000)
        {
            stor_full = true;
        }
        //not full
        else if (stor_full == true && stor_power < 1000)
        {
            stor_full = false;
        }
    }
}
