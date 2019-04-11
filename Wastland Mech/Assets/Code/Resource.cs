using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public byte health = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.name == "Tree")
        {
            health = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //returns health 
    public byte GetHealth()
    {
        return this.health;
    }
    public void HitResource()
    {
        //decrease resource health
        this.health--;

        //if gameobject has been fully harvested
        if(this.health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
