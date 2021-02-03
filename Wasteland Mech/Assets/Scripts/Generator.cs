using UnityEngine;

public class Generator : Power
{
    public float fuel = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        //generate power if enabled and has fuel
        if (isEnabled && fuel > 0)
        {
            power += Time.deltaTime;
        }
    }
}
