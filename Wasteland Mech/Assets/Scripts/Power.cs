using UnityEngine;

public class Power : MonoBehaviour
{
    public bool isEnabled = false;
    public float power = 0;

    public void PowerSetUp()
    {

    }
    public void SetEnabled(bool status)
    {
        isEnabled = status;
    }
}
