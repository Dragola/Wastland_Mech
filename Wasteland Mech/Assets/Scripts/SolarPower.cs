using UnityEngine;

public class SolarPower : MonoBehaviour
{
    public bool isEnabled = false;
    public bool sunHitting = false;
    public float power = 0;

    // Update is called once per frame
    void Update()
    {
        //generate power if enabled and sun at right angle
        if (isEnabled && sunHitting)
        {
            power += Time.deltaTime;
        }

    }

    public void SetEnabled(bool status)
    {
        isEnabled = status;
    }
}
