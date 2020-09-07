using UnityEngine;

public class MineableResource : MonoBehaviour
{
    public byte health = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.name.CompareTo("tree") == 0)
        {
            health = 100;
        }
        else if (this.gameObject.name.CompareTo("largeRock") == 0)
        {
            health = 255;
        }
        else if (this.gameObject.name.CompareTo("rustedMetal") == 0)
        {
            health = 50;
        }
    }

    //when resource is hit
    public string HitResource(byte damage)
    {
        //if health will be above or 0
        if (this.health - damage >= 0)
        {
            //decrease resource health
            this.health -= damage;
        }
        //hit destroys resource
        else
        {
            this.health = 0;
        }

        //if gameobject has been fully harvested
        if(this.health <= 0)
        {
            Destroy(this.gameObject);
        }
        return this.name;
    }
    public byte GetHealth()
    {
        return health;
    }
    public void SetHealth(byte health)
    {
        this.health = health;
        return;
    }
}
