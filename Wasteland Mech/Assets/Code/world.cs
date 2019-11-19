using UnityEngine;

public class World : MonoBehaviour
{
    //world
    GameObject sun = null;
    public ushort dayDuration = 3600;

    // Use this for initialization
    void Start()
    {
        //sun
        sun = GameObject.Find("sun");

        //player
        //player = GameObject.Find("Player").GetComponent<Transform>();

    }

    // Update is called once per frame (60fps = 60calls)
    void Update()
    {
        //sun rotation
        sun.transform.RotateAround(Vector3.zero, Vector3.right, 1 * Time.deltaTime);

        //reset clock
        if(dayDuration <= 0)
        {
            dayDuration = 3600;
        }

    }
    //runs at fixed rate
    void FixedUpdate()
    {
        dayDuration -= 1;
    }
}

       