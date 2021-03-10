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

    public void CharacterSetUp(bool isPlayer, byte characterType)
    {
        //get rigidbody component from character
        characterRigidbody = gameObject.GetComponent<Rigidbody>();
        
        //set character type
        this.characterType = characterType;

        //if player
        if (isPlayer)
        {
            health = 100;
        }
        //if not player
        else
        {
            //normal NPC (civilian)
            if (characterType == 0)
            {

            }
        }
        //set initial health and water
        food = 100f;
        water = 100f;

        //set array size
        inventorySlot = new string[16];
        inventorySlotSize = new byte[16];
    }
    public void UpdateFood(bool ateFood, float amount)
    {
        //if character ate food then increase food
        if (ateFood)
        {
            food += amount;
        }
        //if character didn't eat food then decrease food
        else
        {
            food -= amount;
        }
        
    }
    public void UpdateWater(bool drankWater, float amount)
    {
        //if character had water then increase water
        if (drankWater)
        {
            water += amount;
        }
        //if character didn't drink water then decrease water
        else
        {
            water -= amount;
        }
    }
    public void UpdateHealth(bool wasHurt, bool wasHealed, float amount)
    {
        //if character was hurt then update health
        if (wasHurt)
        {
            health -= amount;
        }
    }
}
