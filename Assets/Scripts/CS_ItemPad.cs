using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ItemPad : MonoBehaviour
{
    [SerializeField] public bool BelongsToP1; // Dictate who owns this pad
    [SerializeField] public GameObject[] Slots;

    public bool[] IsSlotFilled = new bool[4];

    private void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            IsSlotFilled[i] = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == CS_Gamemanager.Players[0] && BelongsToP1)
        {
            SetPadSlot(other);            
        }
        else if(other.gameObject == CS_Gamemanager.Players[1] && !BelongsToP1)
        {
            SetPadSlot(other);
        }
    }

    private void SetPadSlot(Collider other) // For when a player returns an item to their own pad
    {
        if (other.GetComponent<CS_PlayerController>().bItemPickedUp)
        {
            int id = other.GetComponent<CS_PlayerController>().Item.GetComponent<CS_Item>().ID;

            if(IsSlotFilled[id])
            {
                // Already have
            }
            else
            {
                Slots[id].GetComponent<CS_PadItem>().SetSlot(other.GetComponent<CS_PlayerController>().Item);
                other.GetComponent<CS_PlayerController>().PutItemOnPad();

                IsSlotFilled[id] = true;
            }
        }
    }

    public void DropHalfItems()
    {
        List<GameObject> filledItems = new List<GameObject>();

        // Get all the filled objects
        for(int i = 0; i < 4; i++)
        {
            if(IsSlotFilled[i])
            {
                filledItems.Add(Slots[i]);
            }
        }

        // Figure how many items to remove
        int amountToRemove = (int)Mathf.Floor(filledItems.Count * 0.5f);

        int[] items = new int[2]; // Stores the id's of the items to be removed
        
        if(filledItems.Count > 1)
        {
            int iItemCount = Random.Range(0, filledItems.Count - 1);
            Slots[filledItems[iItemCount].GetComponent<CS_PadItem>().fieldItem.ID].GetComponent<MeshRenderer>().enabled = false;
            items[0] = filledItems[iItemCount].GetComponent<CS_PadItem>().fieldItem.ID;
            filledItems.RemoveAt(iItemCount);
            if (amountToRemove > 1)
            {
                int iItem1Count = Random.Range(0, filledItems.Count - 1);
                Slots[filledItems[iItem1Count].GetComponent<CS_PadItem>().fieldItem.ID].GetComponent<MeshRenderer>().enabled = false;
                items[1] = filledItems[iItem1Count].GetComponent<CS_PadItem>().fieldItem.ID;
                filledItems.RemoveAt(iItem1Count);
            }
        }
        for(int i = 0; i < amountToRemove; i++)
        {
            CS_Gamemanager.DropItem(items[i], Slots[items[i]].transform);
            Slots[items[i]].GetComponent<CS_PadItem>().DropSlot();
        }
        
        for(int i = 0; i < amountToRemove; i++)
        {
            int slot = items[i];
            IsSlotFilled[slot] = false;
        }
    }


}