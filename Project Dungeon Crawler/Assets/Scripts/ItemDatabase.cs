using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : ScriptableObject 
{
    public enum AnimationType {None,OneHanded_W,TwoHanded_W,Bow,Potion}

    
    
    public struct ItemInfo
    {
        public int ID;
        public string ItemName;
        public AnimationType AnimType;
        public string ItemDescription;

        public void SetData(int _ID, string _ItemName, AnimationType _AnimType)
        {
            ID = _ID;
            ItemName = _ItemName;
            AnimType = _AnimType;

        }
    }



    public ItemInfo NoItem = new ItemInfo();
    public ItemInfo HealthPotion = new ItemInfo();
    public ItemInfo RegenPotion = new ItemInfo();
    public ItemInfo Antidote = new ItemInfo();
    public ItemInfo CalmingPotion = new ItemInfo();
    public ItemInfo HuntingBow = new ItemInfo();
    public ItemInfo ArmingSword = new ItemInfo();
    public ItemInfo Wine = new ItemInfo();
    public ItemInfo Morningstar = new ItemInfo();



    public void Main()
    {
        NoItem.SetData(0, "None", AnimationType.None);
        HealthPotion.SetData(1, "Health Potion", AnimationType.Potion);
        RegenPotion.SetData(2, "Regen Potion", AnimationType.Potion);
        Antidote.SetData(3, "Antidote", AnimationType.Potion);
        CalmingPotion.SetData(4, "Calming Potion", AnimationType.Potion);
        HuntingBow.SetData(5, "Hunting Bow", AnimationType.Bow);
        ArmingSword.SetData(6, "Arming Sword", AnimationType.OneHanded_W);
        Wine.SetData(7, "Wine", AnimationType.Potion);
        Morningstar.SetData(8, "Morningstar", AnimationType.OneHanded_W);


    }
    
    public ItemInfo[] GetAllItemInfo()
    {
        ItemInfo[] allItemInfo = new ItemInfo[50];

        allItemInfo[0] = HealthPotion;
        Debug.Log(HealthPotion.ItemName);
        allItemInfo[1] = RegenPotion;
        allItemInfo[2] = Antidote;
        allItemInfo[3] = CalmingPotion;
        allItemInfo[4] = HuntingBow;
        allItemInfo[5] = ArmingSword;
        allItemInfo[6] = Wine;
        allItemInfo[7] = Morningstar;


        return allItemInfo;
    }

}

