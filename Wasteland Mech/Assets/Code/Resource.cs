using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public byte health = 0;
    private byte resourceType = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.name == "tree")
        {
            health = 100;
            resourceType = 0;

        }
    }

    //when resource is hit
    public byte HitResource()
    {
        //decrease resource health
        this.health--;

        //if gameobject has been fully harvested
        if(this.health <= 0)
        {
            Destroy(this.gameObject);
        }

        return resourceType;
    }
}
