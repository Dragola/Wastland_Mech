using UnityEngine;

public class SolarPower : Power
{
    public bool sunHitting = false;

    // Update is called once per frame
    void Update()
    {
        //generate power if enabled and sun at right angle
        if (isEnabled && sunHitting)
        {
            power += Time.deltaTime;
        }

    }
}
