using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    [SerializeField] private Vector3 LocPos = Vector3.zero;
    [SerializeField] private Vector3 LocRot = Vector3.zero;
    [SerializeField] private int ID = 0;

    private bool canBePickedUp = false;

    private ItemDatabase.ItemInfo itemInfo = new ItemDatabase.ItemInfo();

    public ItemDatabase.ItemInfo GetItemInfo()
    {
        return itemInfo;
    }


    void Start()
    {
        ItemDatabase.ItemInfo[] allItemInfos = FindObjectOfType<InventoryManager>().ReturnItemDatabase().GetAllItemInfo();
        foreach(ItemDatabase.ItemInfo _itemInfo in allItemInfos)
        {
            if (ID == _itemInfo.ID)
            {
                itemInfo = _itemInfo;
                break;
            }
        }
    }
        

    public void ChangePickUpState(bool _state)
    {
        canBePickedUp = _state;
    }
    

    
}

