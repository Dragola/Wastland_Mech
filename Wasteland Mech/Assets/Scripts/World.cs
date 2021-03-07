using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class World : MonoBehaviour
{
    //Player
    public GameObject playerGameObject = null;
    
    //Power
    public int solarCount = 0;
    public int generatorCount = 0;
    public int furnaceCount = 0;
    public List<GameObject> powerSources = new List<GameObject>();
    public List<GameObject> craftingObjects = new List<GameObject>();
    public List<GameObject> refiningObjects = new List<GameObject>();
    public bool solarEnabled = false;

    //Pause
    public bool paused = false;

    //Sun
    GameObject sun = null;
    public float dayDuration = 3600;

    private void Awake()
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
            //indicate solar can generate power
            if ((dayDuration < 3600 && dayDuration > 100) && solarEnabled == false)
            {
                solarEnabled = true;
            }
            //indicate solar can't generate power
            else if ((dayDuration > 3600 && dayDuration < 100) && solarEnabled == true)
            {
                solarEnabled = false;
            }
            //sun rotation
            sun.transform.RotateAround(Vector3.zero, Vector3.right, 1 * Time.deltaTime);

            //clock/timer
            dayDuration -= Time.deltaTime;

            //reset clock/day
            if (dayDuration <= 0)
            {
                dayDuration = 3600;
            }
        }
    }
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------Solar Power
    public void AddPowerSource(GameObject powerSource)
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
        //add powerSource to list
        powerSources.Add(powerSource);
        return;
    }
    public void RemovePowerSource(GameObject powerDevice)
    {
        //increase solar count
        if (powerDevice.name.Contains("solar"))
        {
            solarCount--;
        }
        //increase generator count
        else if (powerDevice.name.Contains("generator"))
        {
            //decrease solar count and remove from list
            generatorCount--;
        }
        //remove device from list
        powerSources.Remove(powerDevice);
        return;
    }

    public int GetSolarCount()
    {
        return solarCount;
    }
    public int GetGeneratorCount()
    {
        return generatorCount;
    }
    public int GetFurnaceCount()
    {
        return furnaceCount;
    }
    public void AddRefiningObject(GameObject refiningObject)
    {
        //add crafting object/device to 
        refiningObjects.Add(refiningObject);

        //add furnace to list
        if (refiningObject.name.Contains("furnace"))
        {
            furnaceCount++;
        }
        return;
    }
    public void SaveGame() // save game
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

    public void LoadGame() // load game
    {
        //if there is a save file
        if (File.Exists(Application.persistentDataPath + "/save.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.save", FileMode.Open);
            SaveData save = (SaveData)bf.Deserialize(file);
            file.Close();

            //player position and rotation
            playerGameObject.transform.position = new Vector3(save.player.playerX, save.player.playerY, save.player.playerZ);
            playerGameObject.transform.eulerAngles = new Vector3(0, save.player.playerRoataion, 0);
            playerGameObject.GetComponent<Player>().SetHealth(save.player.health);
            GameObject.Find("playerCamera").transform.eulerAngles = new Vector3(save.player.playerCameraRotation, save.player.playerRoataion, 0);
            GameObject.Find("playerRaycastPoint").transform.eulerAngles = new Vector3(save.player.playerCameraRotation, save.player.playerRoataion, 0);

            byte i = 0;
            //inventory
            foreach (playerInventoryData slot in save.inventory)
            {
                Debug.Log("Adding inventory slot " + i + "= " + slot.slotItem + "x" + slot.slotAmount);
                playerGameObject.GetComponent<Player>().SetInventory(i, slot.slotItem, slot.slotAmount);
                playerGameObject.GetComponent<Player>().InventoryUpdate();
                i++;
            }

            solarCount = save.world.solarCount;
            generatorCount = save.world.generatorCount;

            //sun and time
            sun.transform.position = new Vector3(save.world.sunX, save.world.sunY, save.world.sunZ);
            sun.transform.rotation = Quaternion.Euler(save.world.sunRotation, 0, 0);
            dayDuration = save.world.time;

            //remove any objects
            DestroyWorldObjects();

            //resources
            foreach (resourceData resource in save.resources)
            {
                GameObject resourcePrefab = Resources.Load("Prefabs/Resources/" + resource.resourceName) as GameObject;
                GameObject spawnedResource = Instantiate(resourcePrefab, new Vector3(resource.resourcePositionX, resource.resourcePositionY, resource.resourcePositionZ), Quaternion.Euler(resource.resourceRotationX, resource.resourceRotationY, resource.resourceRotationZ));
                spawnedResource.name = resource.resourceName;
            }
            //mineableResources
            foreach (mineableResourceData minableResource in save.minableResources)
            {
                GameObject resourcePrefab = Resources.Load("Prefabs/Resources/" + minableResource.mineableResourceName) as GameObject;
                GameObject spawnedResource = Instantiate(resourcePrefab, new Vector3(minableResource.mineableResourceX, minableResource.mineableResourceY, minableResource.mineableResourceZ), Quaternion.identity);
                spawnedResource.name = minableResource.mineableResourceName;
                spawnedResource.GetComponent<MineableResource>().SetHealth(minableResource.health);
            }
            //player's inventory
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
        save.player.health = playerGameObject.GetComponent<Player>().GetHealth();

        //inventory
        for (byte i = 0; i < 4; i++)
        {
            playerInventoryData data = new playerInventoryData();
            data.slotItem = playerGameObject.GetComponent<Player>().GetinventoryItem(i);
            data.slotAmount = playerGameObject.GetComponent<Player>().GetinventoryAmount(i);
            save.inventory.Add(data);
        }
        //save inventory slots

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
            resourceAdd.resourceRotationX = resource.transform.eulerAngles.x;
            resourceAdd.resourceRotationY = resource.transform.eulerAngles.y;
            resourceAdd.resourceRotationZ = resource.transform.eulerAngles.z;
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
    public void DestroyWorldObjects() //destroys any resources, mineableResources in scene
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("resource");
        GameObject[] mineableResources = GameObject.FindGameObjectsWithTag("mineableResource");

        foreach (GameObject resource in resources)
        {
            Destroy(resource);
        }
        foreach (GameObject mineableResource in mineableResources)
        {
            Destroy(mineableResource);
        }
    }
}

[Serializable]
class SaveData //main save
{
    public playerData player = new playerData();
    public worldData world = new worldData();
    public List<playerInventoryData> inventory = new List<playerInventoryData>();
    public List<resourceData> resources = new List<resourceData>();
    public List<mineableResourceData> minableResources = new List<mineableResourceData>();
}
[Serializable]
class playerData //player data
{
    public float playerX = 0;
    public float playerY = 0;
    public float playerZ = 0;
    public float playerRoataion = 0;
    public float playerCameraRotation = 0;
    public float health = 0;
}
[Serializable]
class playerInventoryData // player's inventory
{
    public string slotItem = "";
    public byte slotAmount = 0;
}
[Serializable]
class worldData //world data
{
    public float sunX = 0;
    public float sunY = 0;
    public float sunZ = 0;
    public float sunRotation = 0;
    public float time = 0;
    public int solarCount = 0;
    public int generatorCount = 0;
}

[Serializable]
class resourceData //resource data
{
    public string resourceName = "";
    public float resourcePositionX = 0;
    public float resourcePositionY = 0;
    public float resourcePositionZ = 0;
    public float resourceRotationX = 0;
    public float resourceRotationY = 0;
    public float resourceRotationZ = 0;
}
[Serializable]
class mineableResourceData //mineableResource Data
{
    public string mineableResourceName = "";
    public float mineableResourceX = 0;
    public float mineableResourceY = 0;
    public float mineableResourceZ = 0;
    public byte health = 0;
}