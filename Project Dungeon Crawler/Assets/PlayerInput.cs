using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public KeyCode Hotbar1 = KeyCode.None;
    [HideInInspector] public KeyCode Hotbar2 = KeyCode.None;
    [HideInInspector] public KeyCode Hotbar3 = KeyCode.None;
    [HideInInspector] public KeyCode Hotbar4 = KeyCode.None;
    [HideInInspector] public KeyCode Hotbar5 = KeyCode.None;
    [HideInInspector] public KeyCode Hotbar6 = KeyCode.None;
    [HideInInspector] public KeyCode Hotbar7 = KeyCode.None;
    [HideInInspector] public KeyCode Hotbar8 = KeyCode.None;
    [HideInInspector] public KeyCode Hotbar9 = KeyCode.None;

    [HideInInspector] public KeyCode Attack = KeyCode.None;
    [HideInInspector] public KeyCode Interact = KeyCode.None;
    [HideInInspector] public KeyCode Block = KeyCode.None;
    [HideInInspector] public KeyCode Dodge = KeyCode.None;
    [HideInInspector] public KeyCode Run = KeyCode.None;
    [HideInInspector] public KeyCode Kick = KeyCode.None;
    [HideInInspector] public KeyCode AltOH = KeyCode.None;
    [HideInInspector] public KeyCode AltStab = KeyCode.None;
    [HideInInspector] public bool UseScrollWheelForOH_AndStab = false;


    void Awake()
    {
        Hotbar1 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hotbar1", "Alpha1"));
        Hotbar2 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hotbar2", "Alpha2"));
        Hotbar3 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hotbar3", "Alpha3"));
        Hotbar4 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hotbar4", "Alpha4"));
        Hotbar5 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hotbar5", "Alpha5"));
        Hotbar6 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hotbar6", "Alpha6"));
        Hotbar7 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hotbar7", "Alpha7"));
        Hotbar8 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hotbar8", "Alpha8"));
        Hotbar9 = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Hotbar9", "Alpha9"));

        Attack = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Attack", "Mouse0"));
        Interact = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Interact", "E"));
        Block = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Block", "Mouse1"));
        Kick = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Kick", "F"));
        Dodge = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Dodge", "Space"));
        Run = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Run", "LeftShift"));

        AltOH = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("AltOH", "None"));
        AltStab = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("AltStab", "None"));
    }

    void Update()
    {
        
    }
}
