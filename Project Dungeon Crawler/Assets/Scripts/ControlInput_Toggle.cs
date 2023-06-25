using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlInput_Toggle : MonoBehaviour
{
    [SerializeField] private int defaultState = 1;
    private int state = 1;

    Toggle toggle = null;

    

    [SerializeField] string playerPrefsName = "";


    private void Awake()
    {
        state = PlayerPrefs.GetInt(playerPrefsName, defaultState);
        toggle = GetComponentInChildren<Toggle>();

        if (state == 1)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }
    }


    public void SetToggle()
   {
        if (state == 1)
        {
            state = 0;
        }
        else
        {
            state = 1;
        }

        if (state == 1)
        {
            PlayerPrefs.SetInt(playerPrefsName, state);
            toggle.isOn = true;
        }
        else
        {
            PlayerPrefs.SetInt(playerPrefsName, state);
            toggle.isOn = false;
        }
    }

    public void ResetToDefault()
    {
        PlayerPrefs.SetInt(playerPrefsName, defaultState);
        toggle.isOn = true;
    }
}
