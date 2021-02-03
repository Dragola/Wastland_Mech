using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float health = 0;
    public Rigidbody characterRigidbody = null;
    public float meleeTime = 3;
    public bool melee = false;
    public bool jumped = false;
    public bool move = false;
    public byte harvestRate = 1;
    public float food = 0f;
    public float water = 0f;

    //inventory
    public string[] inventorySlot = null;
    public byte[] inventorySize = null;

    public void CharacterSetUp()
    {
        //get rigidbody component from character
        characterRigidbody = gameObject.GetComponent<Rigidbody>();
        
        //
        if (gameObject.name.CompareTo("player") == 0)
        {
            health = 100;
            food = 100f;
            water = 100f;
        }
        else
        {
            health = 50;
        }

        //have to size arrays after initialization
        inventorySlot = new string[4];
        inventorySize = new byte[4];
    }
    public void UpdateCondition()
    {
        food -= 0.01f;
        water -= 0.01f;
    }
    public void UpdateHealth(float damage)
    {
        health -= damage;
    }
}
