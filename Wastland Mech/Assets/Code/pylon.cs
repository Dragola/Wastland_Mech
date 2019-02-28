using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pylon : MonoBehaviour
{
    //only 5 connections per pylon (change later)
    public List<GameObject> pwr_conns = new List<GameObject>();
    public List<GameObject> pylon_conns = new List<GameObject>();
    public bool pwr_stor_conn = false;
    public sbyte pwr_storage_first = -1;
    public byte num_solar_panels = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //power + storage
        if (num_solar_panels > 0 && pwr_stor_conn == true)
        {
            Debug.Log("Power + Storage");

            for (sbyte i = 0; i < pwr_conns.Count; i++)
                {
                Debug.Log("Loop");
                //find energy storage(that need charging in order)
                if (pwr_storage_first == -1)
                {
                    if (pwr_conns[i].name == "Energy_Storage")
                    {
                        //fins first storage that needs power
                        if (pwr_conns[i].GetComponent<energy_storage>().stor_full == false)
                        {
                            Debug.Log("Found Non full storage at: " + i);
                            pwr_storage_first = i;
                        }
                    }
                }
                //gets power from solar panel (if there is storage)
                if (pwr_conns[i].name == "Solar Panel" && pwr_storage_first != -1)
                {
                    Debug.Log("Power Transfer");
                    pwr_conns[pwr_storage_first].GetComponent<energy_storage>().stor_power += 1;
                    pwr_conns[i].GetComponent<solar_panel>().power -= 1;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    { 
        //can only add 5 power objects
        if (pwr_conns.Count < 5)
        {
            //solar_panel
            if (other.name == "solar_panel(Clone)" || other.name == "Solar Panel")
            {
                pwr_conns.Add(other.gameObject);
                num_solar_panels += 1;
            }
            //generator
            else if (other.name == "generator(Clone)" || other.name == "Generator")
            {
                pwr_conns.Add(other.gameObject);
            }
            //energy_storage
            else if (other.name == "energy_storage(Clone)" || other.name == "Energy_Storage")
            {
                pwr_conns.Add(other.gameObject);
                pwr_stor_conn = true;
            }

        }
        //other pylon connections
        if (other.name == "pylon_short(Clone)" || other.name == "Pylon_Short")
        {
            pylon_conns.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //solar_panel
        if (other.name == "solar_panel(Clone)" || other.name == "Solar Panel")
        {
            pwr_conns.Remove(other.gameObject);
            num_solar_panels -= 1;
        }
        //generator
        else if (other.name == "geberator(Clone)" || other.name == "Generator")
        {
            pwr_conns.Remove(other.gameObject);
        }
        //enerygy_storage
        else if (other.name == "energy_storage(Clone)" || other.name == "Energy_Storage")
        {
            pwr_conns.Remove(other.gameObject);
        }
        //other pylon connections
        else if (other.name == "pylon_short(Clone)" || other.name == "Pylon_Short")
        {
            pylon_conns.Remove(other.gameObject);
        }
        

    }

    //FOR SCENE ONLY (need a way for actual game view)
    void OnDrawGizmos()
    {
        if (pwr_conns.Count > 0)
        {
            for (byte i = 0; i < pylon_conns.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, pwr_conns[i].transform.position);
            }
        }
        if (pylon_conns.Count > 0)
        {
            for (byte j = 0; j < pylon_conns.Count; j++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, pylon_conns[j].transform.position);
            }
        }
    }
}
