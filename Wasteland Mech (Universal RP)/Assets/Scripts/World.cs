using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class World : MonoBehaviour
{
    public GameObject playerGameObject = null;
    public byte solarCount = 0;
    public byte generatorCount = 0;
    public List<GameObject> powerSources = new List<GameObject>();
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
        playerGameObject = GameObject.Find("player");
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
            playerGameObject.transform.position = new Vector3(save.player.playerX, save.player.playerY, save.player.playerZ);

            //Debug.Log("Loaded player rotation: " + save.playerRoataion);
            playerGameObject.transform.eulerAngles = new Vector3(0, save.player.playerRoataion, 0);

            //Debug.Log("Loaded player camera roataion: " + save.playerCameraRotation);
            GameObject.Find("playerCamera").transform.eulerAngles = new Vector3(save.player.playerCameraRotation, save.player.playerRoataion, 0);

            solarCount = save.world.solarCount;
            generatorCount = save.world.generatorCount;
            
            //sun and time
            sun.transform.position = new Vector3(save.world.sunX, save.world.sunY, save.world.sunZ);
            sun.transform.rotation = Quaternion.Euler(save.world.sunRotation, 0, 0);
            dayDuration = save.world.time;

            //resources
            foreach (resourceData resource in save.resources)
            {
                //Debug.Log("Spawning " + resource.resourceName);
                GameObject resourcePrefab = Resources.Load("Prefabs/Resources/" + resource.resourceName) as GameObject;
                GameObject spawnedResource = Instantiate(resourcePrefab, new Vector3(resource.resourcePositionX, resource.resourcePositionY, resource.resourcePositionZ), Quaternion.identity);
                spawnedResource.name = resource.resourceName;
            }
            //mineableResources
            foreach (mineableResourceData minableResource in save.minableResources)
            {
                //Debug.Log("Spawning " + minableResource.mineableResourceName);
                GameObject resourcePrefab = Resources.Load("Prefabs/Resources/" + minableResource.mineableResourceName) as GameObject;
                GameObject spawnedResource = Instantiate(resourcePrefab, new Vector3(minableResource.mineableResourceX, minableResource.mineableResourceY, minableResource.mineableResourceZ), Quaternion.identity);
                spawnedResource.name = minableResource.mineableResourceName;
                spawnedResource.GetComponent<MineableResource>().SetHealth(minableResource.health);
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
        SaveData save = new SaveData();

        //player data
        save.player.playerX = playerGameObject.transform.position.x;
        save.player.playerY = playerGameObject.transform.position.y;
        save.player.playerZ = playerGameObject.transform.position.z;
        save.player.playerRoataion = playerGameObject.transform.eulerAngles.y;
        save.player.playerCameraRotation = GameObject.Find("playerCamera").transform.eulerAngles.x;

        //world data
        save.world.solarCount = solarCount;
        save.world.generatorCount = generatorCount;
        //sun and time
        save.world.sunX = sun.transform.position.x;
        save.world.sunY = sun.transform.position.y;
        save.world.sunZ = sun.transform.position.z;
        save.world.sunRotation = sun.transform.eulerAngles.x;
        save.world.time = dayDuration;

        //get all objects for saving
        GameObject[] resources = GameObject.FindGameObjectsWithTag("resource");
        GameObject[] minableResources = GameObject.FindGameObjectsWithTag("mineableResource");
        //GameObject[] power = GameObject.FindGameObjectsWithTag("power");

        //resources
        foreach (GameObject resource in resources)
        {
            resourceData resourceAdd = new resourceData();
            resourceAdd.resourceName = resource.name;
            resourceAdd.resourcePositionX = resource.transform.position.x;
            resourceAdd.resourcePositionY = resource.transform.position.y;
            resourceAdd.resourcePositionZ = resource.transform.position.z;
            save.resources.Add(resourceAdd);
        }
        //minable resources
        foreach (GameObject mineableResource in minableResources)
        {
            mineableResourceData minableAdd = new mineableResourceData();
            minableAdd.mineableResourceName = mineableResource.name;
            minableAdd.mineableResourceX = mineableResource.transform.position.x;
            minableAdd.mineableResourceY = mineableResource.transform.position.y;
            minableAdd.mineableResourceZ = mineableResource.transform.position.z;
            minableAdd.health = mineableResource.GetComponent<MineableResource>().GetHealth();
            save.minableResources.Add(minableAdd);
        }

        //resource data
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
class SaveData
{
    public playerData player =new playerData();
    public worldData world = new worldData();
    public List<resourceData> resources = new List<resourceData>();
    public List<mineableResourceData> minableResources = new List<mineableResourceData>();
}
[Serializable]
class playerData
{
    public float playerX = 0;
    public float playerY = 0;
    public float playerZ = 0;
    public float playerRoataion = 0;
    public float playerCameraRotation = 0;
}
[Serializable]
class worldData
{
    public float sunX = 0;
    public float sunY = 0;
    public float sunZ = 0;
    public float sunRotation = 0;
    public float time = 0;
    public byte solarCount = 0;
    public byte generatorCount = 0;
}

[Serializable]
class resourceData
{
    public string resourceName = "";
    public float resourcePositionX = 0;
    public float resourcePositionY = 0;
    public float resourcePositionZ = 0;
}
[Serializable]
class mineableResourceData
{
    public string mineableResourceName = "";
    public float mineableResourceX = 0;
    public float mineableResourceY = 0;
    public float mineableResourceZ = 0;
    public byte health = 0;
}