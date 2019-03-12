using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_PadItem : MonoBehaviour
{
    public CS_Item fieldItem; // Set to save item

    private bool IsRemovable = true;
    public float fProbablitlityMod = 0;
    private MeshRenderer meshRenderer; // Draws the model
    private MeshFilter meshFilter; // Set models to this

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    public void DropSlot()
    {
        if(IsRemovable)
        {
            fieldItem = null;
            fProbablitlityMod = 0;
            meshRenderer.enabled = false;
        }
    }

    public void SetSlot(GameObject a_item)
    {
        if (meshRenderer.enabled == false)
        {
            meshRenderer.enabled = true;
        }

        meshFilter.mesh = a_item.GetComponent<CS_Item>().Model;

        fieldItem = a_item.GetComponent<CS_Item>();

        if (fieldItem.GetComponent<CS_Item>().Value == ItemValue.HQ)
        {
            fProbablitlityMod = 0.25f;
        }
        else
        {
            fProbablitlityMod = 0.15f;
        }

        if (fieldItem.GetComponent<CS_Item>().Value == ItemValue.LEDGENDARY)
        {
            IsRemovable = false;
        }
    }
}
