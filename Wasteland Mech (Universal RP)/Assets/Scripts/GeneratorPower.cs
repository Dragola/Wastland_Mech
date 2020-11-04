using UnityEngine;

public class GeneratorPower : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool generatePower = false;
    public float power = 0;

    // Update is called once per frame
    void Update()
    {
        //generate power
        if (generatePower)
        {
            power += Time.deltaTime;
        }
    }

    public void generatorEnabled(bool status)
    {
        generatePower = status;
        //Debug.Log(this.name + "has updated status");
    }
}
