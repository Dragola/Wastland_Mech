using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public byte characterType = 0;
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
    public byte[] inventorySlotSize = null;
    public byte inventorySize = 4;
    public bool hasBackpack = false;

    public void CharacterSetUp(byte characterType)
    {
        //get rigidbody component from character
        characterRigidbody = gameObject.GetComponent<Rigidbody>();
        
        //set character type
        this.characterType = characterType;

        //player
        if (characterType == 0)
        {
            health = 100;
        }
        //normal person
        else if( characterType == 1)
        {
            health = 50;
        }
        food = 100f;
        water = 100f;

        //set array size
        inventorySlot = new string[16];
        inventorySlotSize = new byte[16];
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
