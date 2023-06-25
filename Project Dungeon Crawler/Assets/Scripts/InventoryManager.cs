using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(UI_Manager))]
[RequireComponent(typeof(AnimationControl))]

public class InventoryManager : MonoBehaviour
{
    private Dictionary<InventorySlot, int> inventorySlotDictionary = new Dictionary<InventorySlot, int>();
    private List<InventorySlot> hotbarSlots = new List<InventorySlot>();

    private AnimationControl animControl = null;

    private InventorySlot cursorHoldsThisSlot = null;

    private InventorySlot equippedItemIsHere = null;
    private ItemDatabase.ItemInfo equippedItemInfo = new ItemDatabase.ItemInfo();

    private Camera playerCam = null;

    private Image inventoryImage = null;
    private GameObject[] inventoryObjectsToDisable = new GameObject[2];
    private ItemDatabase itemDatabase = null;
    
    private bool isInventoryActive = false;
    private bool canPickUpItem = true;

    private ItemData targetItem = null;

    private PlayerInput playerInput = null;
    private UI_Manager uiManager = null;

    private KeyCode Interact = KeyCode.None;

    private KeyCode Hotbar1 = KeyCode.None;
    private KeyCode Hotbar2 = KeyCode.None;
    private KeyCode Hotbar3 = KeyCode.None;
    private KeyCode Hotbar4 = KeyCode.None;
    private KeyCode Hotbar5 = KeyCode.None;
    private KeyCode Hotbar6 = KeyCode.None;
    private KeyCode Hotbar7 = KeyCode.None;
    private KeyCode Hotbar8 = KeyCode.None;
    private KeyCode Hotbar9 = KeyCode.None;



    public ItemDatabase ReturnItemDatabase()
    {
        return itemDatabase;
    }

    private void Awake()
    {
        itemDatabase = ScriptableObject.CreateInstance("ItemDatabase") as ItemDatabase;
        itemDatabase.Main();
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        uiManager = GetComponent<UI_Manager>();
        animControl = GetComponent<AnimationControl>();

        Interact = playerInput.Interact;

        Hotbar1 = playerInput.Hotbar1;
        Hotbar2 = playerInput.Hotbar2;
        Hotbar3 = playerInput.Hotbar3;
        Hotbar4 = playerInput.Hotbar4;
        Hotbar5 = playerInput.Hotbar5;
        Hotbar6 = playerInput.Hotbar6;
        Hotbar7 = playerInput.Hotbar7;
        Hotbar8 = playerInput.Hotbar8;
        Hotbar9 = playerInput.Hotbar9;

        inventoryImage = GameObject.FindGameObjectWithTag("SlotPanel").GetComponent<Image>();
        inventoryObjectsToDisable[0] = GameObject.FindGameObjectWithTag("ItemPanel");
        inventoryObjectsToDisable[1] = GameObject.FindGameObjectWithTag("GoldPanel");

        playerCam = GetComponentInChildren<Camera>();




        InventorySlot[] _inventorySlots = FindObjectsOfType<InventorySlot>();

        for (int i = 0; i < _inventorySlots.Length - 1; i++)
        {
            for (int k = 0; k < _inventorySlots.Length - 1; k++)
            {
                if (_inventorySlots[k].GetSlotID() > _inventorySlots[k + 1].GetSlotID())
                {
                    InventorySlot _temp = _inventorySlots[k];
                    _inventorySlots[k] = _inventorySlots[k + 1];
                    _inventorySlots[k + 1] = _temp;
                }
            }
        }


        foreach (InventorySlot _invSlot in _inventorySlots)
        {
            inventorySlotDictionary.Add(_invSlot, 0);

            if(_invSlot.GetSlotID() < 10)
            {
                hotbarSlots.Add(_invSlot);
            }
        }

        DisableInventory();
    }

    void Update()
    {
        ManageInventory();
        ManageItemEquip();
    }


    private void LateUpdate()
    {
        ManageItemPickUp();
    }

    void CheckHotbarSlotForItem(int _whichSlot)
    {
        foreach (InventorySlot _hotbarSlot in hotbarSlots)
        {
            if (_hotbarSlot.GetSlotID() == _whichSlot)
            {
                if (!_hotbarSlot.IsEmpty())
                {

                    equippedItemIsHere = _hotbarSlot;
                    equippedItemInfo = equippedItemIsHere.GetItemInfo();

                    animControl.EquipItem(equippedItemInfo);

                }
            }
        }
    }



    private void ManageItemEquip()
    {
        if (animControl.IsIdle())
        {
            if (Input.GetKeyDown(Hotbar1))
            {
                CheckHotbarSlotForItem(1);
            }
            else if (Input.GetKeyDown(Hotbar2))
            {
                CheckHotbarSlotForItem(2);
            }
            else if (Input.GetKeyDown(Hotbar3))
            {
                CheckHotbarSlotForItem(3);
            }
            else if (Input.GetKeyDown(Hotbar4))
            {
                CheckHotbarSlotForItem(4);
            }
            else if (Input.GetKeyDown(Hotbar5))
            {
                CheckHotbarSlotForItem(5);
            }
            else if (Input.GetKeyDown(Hotbar6))
            {
                CheckHotbarSlotForItem(6);
            }
            else if (Input.GetKeyDown(Hotbar7))
            {
                CheckHotbarSlotForItem(7);
            }
            else if (Input.GetKeyDown(Hotbar8))
            {
                CheckHotbarSlotForItem(8);
            }
            else if (Input.GetKeyDown(Hotbar9))
            {
                CheckHotbarSlotForItem(9);
            }
        }
    }

    private void ManageItemPickUp()
    {
        if(canPickUpItem)
        {


            RaycastHit _ray;
            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out _ray, 2.5f))
            {
               if( _ray.collider.CompareTag("PickUp"))
               {
                    if (_ray.collider.GetComponent<ItemData>() != null)
                    {
                        ItemData _item = _ray.collider.GetComponent<ItemData>();
                        ItemDatabase.ItemInfo _itemInfo = _item.GetItemInfo();

                        if(targetItem != _item)
                        {
                            targetItem = _item;
                            print(targetItem.GetItemInfo().ItemName);
                            uiManager.ChangeItemName(_itemInfo.ItemName +"\n" + "[" + Interact.ToString() + "]");
                        }
                    }
               }
               else
               {
                    if (targetItem != null)
                    {
                        targetItem = null;
                        uiManager.ChangeItemName("");
                    }
               }
            }
            else
            {
                if (targetItem != null)
                {
                    targetItem = null;
                    uiManager.ChangeItemName("");
                }
            }


            if(Input.GetKeyDown(Interact))
            {
                if(targetItem != null)
                {
                    ItemDatabase.ItemInfo _targetItemInfo = targetItem.GetItemInfo();
                    PlaceItemIntoInventory(_targetItemInfo);
                    uiManager.ChangeItemName("");
                    Destroy(targetItem.gameObject);
                }
            }
        }
    }

    private void ManageInventory()
    {
        if (animControl.IsIdle())
        {
            if (Input.GetKeyDown(KeyCode.I))
            {

                if (isInventoryActive)
                {
                    DisableInventory();
                }
                else if (!isInventoryActive)
                {
                    EnableInventory();
                }

                isInventoryActive = !isInventoryActive;
            }
        }
    }

    public void DisableInventory()
    {
        canPickUpItem = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        inventoryImage.enabled = false;
        inventoryObjectsToDisable[0].SetActive(false);
        inventoryObjectsToDisable[1].SetActive(false);

        foreach (KeyValuePair<InventorySlot, int> _invSlot in inventorySlotDictionary)
        {
            { 
                _invSlot.Key.Hide();
            }
        }
    }

    public void EnableInventory()
    {
        canPickUpItem = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        inventoryImage.enabled = true;
        inventoryObjectsToDisable[0].SetActive(true);
        inventoryObjectsToDisable[1].SetActive(true);

        animControl.StopMovementAnimations();
        animControl.ChangeAnimLayer(ItemDatabase.AnimationType.None);

        foreach (KeyValuePair<InventorySlot, int> _invSlot in inventorySlotDictionary)
        {
            {
                _invSlot.Key.Show();
            }
        }
    }

    public bool IsInventoryActive()
    {
        return isInventoryActive;
    }

    public bool IsCursorHoldingAnItem()
    {
        if(cursorHoldsThisSlot != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetCursorHoldingItem(InventorySlot _invSlot)
    {
        cursorHoldsThisSlot = _invSlot;
    }

    public void ChangeDictionaryValue(InventorySlot _slot, int _ID)
    {
      
        foreach (KeyValuePair<InventorySlot, int> _invSlot in inventorySlotDictionary)
        {
            if(_invSlot.Key == _slot)
            {
                inventorySlotDictionary[_invSlot.Key] = _ID;
                break;
            }
        }

    }

    private void PlaceItemIntoInventory( ItemDatabase.ItemInfo _itemInfo)
    {
        foreach (KeyValuePair<InventorySlot, int> _invSlot in inventorySlotDictionary)
        {
            if (_invSlot.Value == 0)
            {
                _invSlot.Key.InsertItem(_itemInfo);
                break;
            }
        }
    }

}
