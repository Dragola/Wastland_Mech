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
            health = 200;

        }
        else if (this.gameObject.name == "rock")
        {
            health = 1;
            resourceType = 1;
        }
        else if (this.gameObject.name == "wood")
        {
            health = 1;
        }
        else
        {
            health = 1;
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
