using UnityEngine;

public class Resource : MonoBehaviour
{
    public byte health = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.name == "tree")
        {
            health = 100;
        }
        else if (this.gameObject.name == "largeRock")
        {
            health = 255;
        }
    }

    //when resource is hit
    public string HitResource(byte damage)
    {
        //decrease resource health
        this.health -= damage;

        //if gameobject has been fully harvested
        if(this.health <= 0)
        {
            Destroy(this.gameObject);
        }

        return this.name;
    }
}
