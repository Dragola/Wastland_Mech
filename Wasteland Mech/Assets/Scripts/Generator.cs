using UnityEngine;

public class Generator : MonoBehaviour
{
    public bool isEnabled = false;
    public float fuel = 0f;
    public float power = 0f;
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

    public void SetEnabled(bool status)
    {
        isEnabled = status;
    }
}
