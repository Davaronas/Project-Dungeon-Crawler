using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int heldItemID = 0;
    private ItemDatabase.ItemInfo heldItemInfo = new ItemDatabase.ItemInfo();

    private bool equippedItemIsHere = false;
    private bool isCursorOnThisItem = false;
    private bool isCursorHoldingThisItem = false;

    private Camera cam = null;
    private InventoryManager inventoryManager = null;
    private ItemImageDatabase itemImageDatabase = null;

    private enum InventorySlotType { InventorySlot, HotbarSlot };
    [SerializeField] private int slotID = 0;
    [SerializeField] private InventorySlotType slotType = InventorySlotType.InventorySlot;


    [Space]
    [SerializeField] private GameObject inventoryItemPrefab = null;
    [SerializeField] private GameObject hotbarItemPrefab = null;

    private Image slotImage = null;

    private GameObject heldItemImage = null;

    private InventorySlot[] inventorySlots = null;

    public bool IsCursorOnThisSlot()
    {
        return isCursorOnThisItem;
    }

    public int GetSlotID()
    {
        return slotID;
    }

    public int GetItemID()
    {
        return heldItemID;
    }

    public ItemDatabase.ItemInfo GetItemInfo()
    {
        return heldItemInfo;
    }

    public bool IsEmpty()
    {
        if (heldItemImage == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    void Awake()
    {
        slotImage = GetComponent<Image>();
        cam = FindObjectOfType<Camera>();
        inventoryManager = FindObjectOfType<InventoryManager>(); // THIS CAN CAUSE PROBLEMS IN MULTIPLAYER, GET ALL OF THEM AND CHECK FOR OWNER MAYBE OR FEED IT WHEN PLAYER SPAWNS
        inventorySlots = FindObjectsOfType<InventorySlot>();
        itemImageDatabase = FindObjectOfType<ItemImageDatabase>();

        ItemDatabase.ItemInfo None = new ItemDatabase.ItemInfo();
        None.ID = 0;
        None.ItemName = "None";
        None.AnimType = ItemDatabase.AnimationType.None;

        heldItemInfo = None;
    }

    void Update()
    {
        if (inventoryManager != null)
        {
            if (heldItemImage != null) // This is for holding the item
            {
                if (inventoryManager.IsCursorHoldingAnItem() == false || isCursorHoldingThisItem)
                {
                    if (Input.GetButtonDown("Attack") && isCursorOnThisItem)
                    {
                        if (isCursorHoldingThisItem == false)
                        {
                            isCursorHoldingThisItem = true;
                        }

                        if (inventoryManager.IsCursorHoldingAnItem() == false)
                        {
                            inventoryManager.SetCursorHoldingItem(this);
                        }
                    }

                    if (Input.GetButton("Attack") && isCursorHoldingThisItem)
                    {
                        Vector3 _cursorPos = Input.mousePosition;
                        Vector2 _temp = new Vector2(_cursorPos.x, _cursorPos.y);
                        heldItemImage.transform.position = _temp;
                    }

                    if (Input.GetButtonUp("Attack"))
                    {
                        ReleaseItem();
                    }

                }


                if (isCursorHoldingThisItem)
                {
                    if (heldItemImage.transform.parent != transform.parent.parent)
                    {
                        heldItemImage.transform.SetParent(transform.parent.parent, true);
                    }
                }
                else
                {
                    if (heldItemImage.transform.parent != transform)
                    {
                        heldItemImage.transform.SetParent(transform, true);
                    }
                }

            }
        }


    }

    private void ReleaseItem()
    {

        if (isCursorHoldingThisItem)
        {
            bool _found = false;

            if (isCursorOnThisItem)
            {
                heldItemImage.transform.position = transform.position;
            }
            else
            {
                foreach (InventorySlot _slot in inventorySlots)
                {
                    if (_slot.IsCursorOnThisSlot())
                    {
                        _found = true;
                        ItemDatabase.ItemInfo _tempOther = _slot.GetItemInfo();
                        ItemDatabase.ItemInfo _tempOwn = GetItemInfo();

                        RemoveItem();

                        if (!_slot.IsEmpty())
                        {
                            InsertItem(_tempOther);
                            _slot.RemoveItem();
                        }

                        _slot.InsertItem(_tempOwn);

                    }
                }

                if (!_found)
                {
                    heldItemImage.transform.position = transform.position;
                }
            }

            if (isCursorHoldingThisItem == true)
            {
                isCursorHoldingThisItem = false;
            }

            if (inventoryManager.IsCursorHoldingAnItem() == true)
            {
                inventoryManager.SetCursorHoldingItem(null);
            }
        }
    }


    public void Hide()
    {
        if (isCursorHoldingThisItem)
        {
            heldItemImage.transform.position = transform.position;
            inventoryManager.SetCursorHoldingItem(null);
        }
        isCursorHoldingThisItem = false;
        isCursorOnThisItem = false;


        if (slotType != InventorySlotType.HotbarSlot)
        {
            slotImage.enabled = false;

            if (heldItemImage != null)
            {
                ReleaseItem();
                heldItemImage.GetComponent<Image>().enabled = false;
            }
        }
    }

    public void Show()
    {
        if (slotType != InventorySlotType.HotbarSlot)
        {
            slotImage.enabled = true;

            if (heldItemImage != null)
            {
                heldItemImage.GetComponent<Image>().enabled = true;
            }
        }
    }

    public void InsertItem(ItemDatabase.ItemInfo _itemInfo)
    {
        heldItemID = _itemInfo.ID;
        heldItemInfo = _itemInfo;
        inventoryManager.ChangeDictionaryValue(this, heldItemID);


        if (slotType == InventorySlotType.InventorySlot)
        {
            heldItemImage = Instantiate(inventoryItemPrefab, transform.position, Quaternion.identity); // THIS IS LOCAL, NO NEED TO INSTANTIATE FROM BOLT
            if (inventoryManager.IsInventoryActive())
            {
                heldItemImage.GetComponent<Image>().enabled = true;
            }
        }
        else if (slotType == InventorySlotType.HotbarSlot)
        {
            heldItemImage = Instantiate(hotbarItemPrefab, transform.position, Quaternion.identity); // THIS IS LOCAL, NO NEED TO INSTANTIATE FROM BOLT
            heldItemImage.GetComponent<Image>().enabled = true;
        }

        heldItemImage.transform.SetParent(transform, true);

        Image _itemImage = heldItemImage.GetComponent<Image>();
        switch (heldItemID)
        {
            case 1:
                _itemImage.sprite = itemImageDatabase.healthPotionImage;
                break;
            case 2:
                _itemImage.sprite = itemImageDatabase.regenerationPotionImage;
                break;
            case 3:
                _itemImage.sprite = itemImageDatabase.antidoteImage;
                break;
            case 4:
                _itemImage.sprite = itemImageDatabase.calmingPotionImage;
                break;
            case 5:
                _itemImage.sprite = itemImageDatabase.bowImage;
                break;
            case 6:
                _itemImage.sprite = itemImageDatabase.armingSwordImage;
                break;
            case 7:
                _itemImage.sprite = itemImageDatabase.wineImage;
                break;
            case 8:
                _itemImage.sprite = itemImageDatabase.morningstarImage;
                break;


        }
}

    public void RemoveItem()
    {
        if(heldItemImage != null)
        {
            Destroy(heldItemImage);
            heldItemID = 0;

            ItemDatabase.ItemInfo None = new ItemDatabase.ItemInfo();
            None.ID = 0;
            None.ItemName = "None";
            None.AnimType = ItemDatabase.AnimationType.None;

            heldItemInfo = None; 
            isCursorHoldingThisItem = false;
            inventoryManager.ChangeDictionaryValue(this,0);
        }
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        isCursorOnThisItem = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isCursorOnThisItem = false;
    }

}

