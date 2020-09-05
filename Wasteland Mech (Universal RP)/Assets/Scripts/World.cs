using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class World : MonoBehaviour
{
    public GameObject player = null;
    public byte solarCount = 0;
    public byte generatorCount = 0;
    public List<GameObject> powerSources = new List<GameObject>();
    public List<GameObject> resources = new List<GameObject>();
    public List<GameObject> minableResources = new List<GameObject>();
    public bool solarEnabled = false;
    public bool paused = false;
    
    //world
    GameObject sun = null;
    public float dayDuration = 3600;

    // Use this for initialization
    void Start()
    {
        //sun
        sun = GameObject.Find("sun");

        //player
        player = GameObject.Find("player");

        //resource (test)
        resources.Add(GameObject.Find("wood"));
    }

    // Update is called once per frame (60fps = 60calls)
    void Update()
    {
        //if game isn't paused
        if (paused == false)
        {
            //enable solar panels to generate
            if ((dayDuration < 3600 && dayDuration > 100) && solarEnabled == false)
            {
                solarEnabled = true;
                powerStatusUpdate();
            }
            else if ((dayDuration > 3600 && dayDuration < 100) && solarEnabled == true)
            {
                solarEnabled = false;
                powerStatusUpdate();
            }
            //sun rotation
            sun.transform.RotateAround(Vector3.zero, Vector3.right, 1 * Time.deltaTime);

            //clock
            dayDuration -= Time.deltaTime;

            //reset clock/day
            if (dayDuration <= 0)
            {
                dayDuration = 3600;
            }
        }
    }
    //runs at fixed rate
    void FixedUpdate()
    {
        
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Solar Power
    public void addPowerSource(GameObject powerSource)
    {
        //increase solar count
        if (powerSource.name.Contains("solar"))
        {
            solarCount++;
        }
        //increase generator count
        else if (powerSource.name.Contains("generator"))
        {        
            generatorCount++;
        }
        //add powerSource to list and update generator status
        powerSources.Add(powerSource);
        powerStatusUpdate();
        return;
    }
    public void removePowerSource(GameObject solarPanel)
    {
        //decrease solar count and remove from list
        solarCount--;
        powerSources.Remove(solarPanel);
        return;
    }

    public byte getSolarCount()
    {
        return solarCount;
    }

    public void powerStatusUpdate()
    {
        foreach (GameObject powerSource in powerSources)
        {
            if (powerSource.name.Contains("solar"))
            {
                powerSource.GetComponent<SolarPower>().solarEnabled(solarEnabled);
                //Debug.Log("Enabled solar generation for: " + powerSource.name);
            }
            else if (powerSource.name.Contains("generator"))
            {
                powerSource.GetComponent<GeneratorPower>().generatorEnabled(true);
                //Debug.Log("Enabled solar generation for: " + powerSource.name);
            }

        }
        return;
    }
    public void SaveGame()
    {
        //save object
        SaveData save = CreateSaveObject();
        BinaryFormatter bf = new BinaryFormatter();

        //file location + name
        FileStream file = File.Create(Application.persistentDataPath + "/save.save");

        //serilizes data and puts into file
        bf.Serialize(file, save);

        //closes file
        file.Close();
        return;
    }

    public void LoadGame()
    {
        //if there is a save file
        if (File.Exists(Application.persistentDataPath + "/save.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.save", FileMode.Open);
            SaveData save = (SaveData)bf.Deserialize(file);
            file.Close();

            //loads values
            //player position and rotation
            //Debug.Log("Loaded player transform- X:" + save.playerX + " Y: " + save.playerY + " Z: " + save.playerZ);
            player.transform.position = new Vector3(save.playerX, save.playerY, save.playerZ);
            
            //Debug.Log("Loaded player rotation: " + save.playerRoataion);
            player.transform.eulerAngles = new Vector3(0, save.playerRoataion, 0);

            //Debug.Log("Loaded player camera roataion: " + save.playerCameraRotation);
            GameObject.Find("playerCamera").transform.eulerAngles = new Vector3(save.playerCameraRotation, save.playerRoataion, 0);

            solarCount = save.solarCount;
            generatorCount = save.generatorCount;
            
            //sun and time
            sun.transform.position = new Vector3(save.sunX, save.sunY, save.sunZ);
            sun.transform.rotation = Quaternion.Euler(save.sunRotation, 0, 0);
            dayDuration = save.time;

            //test
            foreach (Vector3 resource in save.resourceObjects)
            {
                Debug.Log(resource);
            }
        }
        //if there is not a save file
        else
        {
            Debug.Log("No save file present");
        }
        return;
    }
    private SaveData CreateSaveObject() //makes save file
    {
        //didn't know you could do this...
        SaveData save = new SaveData
        {
            //player position
            playerX = player.transform.position.x,
            playerY = player.transform.position.y,
            playerZ = player.transform.position.z,
            playerRoataion = player.transform.eulerAngles.y,
            playerCameraRotation = GameObject.Find("playerCamera").transform.eulerAngles.x,

            //solar and generator count
            solarCount = solarCount,
            generatorCount = generatorCount,

            //sun and time
            sunX = sun.transform.position.x,
            sunY = sun.transform.position.y,
            sunZ = sun.transform.position.z,
            sunRotation = sun.transform.eulerAngles.x,
            time = dayDuration,
        };

        //add resources
        foreach (GameObject resource in resources)
        {
            save.resourceObjects.Add(resource.transform.position);
        }

        return save;
    }
    public void PauseWorld()
    {
        paused = true;
    }
    public void ResumeWorld()
    {
        paused = false;
    }
}

[Serializable]
class playerData
{
    public float playerX;
    public float playerY;
    public float playerZ;
    public float playerRoataion;
    public float playerCameraRotation;
}
[Serializable]
class worldData
{
    public float sunX;
    public float sunY;
    public float sunZ;
    public float sunRotation;
    public float time;
    public byte solarCount;
    public byte generatorCount;
}

[Serializable]
class resourceData
{
    public string resourceName;
    public float resourcePositionX;
    public float resourcePositionY;
    public float resourcePositionZ;
}
[Serializable]
class minableResourceData
{
    public string minableResourceName;
    public float minableResourceX;
    public float minableResourceY;
    public float minableResourceZ;
}