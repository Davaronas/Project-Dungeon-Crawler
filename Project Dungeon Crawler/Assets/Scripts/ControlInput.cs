using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ControlInput : MonoBehaviour
{

    private Text buttonText = null;

    private Button backButton = null;

    [SerializeField] private KeyCode defaultKeyCode;
    private KeyCode keyCodeForThisAction;

    [SerializeField] private string playerPrefsName = "";

    private bool isWaitingForInput = false;

    private ControlInput[] allInputButtons = null;
    private ControlInput_Toggle toggleInputButton = null;

    private bool canGiveInput = true;
   
    public bool IsWaitingForInput()
    {
        return isWaitingForInput;
    }

    public KeyCode GetKeyCode()
    {
        return keyCodeForThisAction;
    }

    void Awake()
    {
        buttonText = GetComponentInChildren<Button>().GetComponentInChildren<Text>();
        keyCodeForThisAction = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(playerPrefsName, defaultKeyCode.ToString()));
        buttonText.text = keyCodeForThisAction.ToString();
        allInputButtons = FindObjectsOfType<ControlInput>();
        backButton = transform.parent.parent.Find("Back").GetComponent<Button>();
        toggleInputButton = FindObjectOfType<ControlInput_Toggle>();
    }


   private void Update()
   {
        if (isWaitingForInput)
        {
            if (Input.anyKeyDown)
            {
                KeyCode _pressedButton = KeyCode.None;

                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kcode))
                    {
                        if (kcode == KeyCode.Escape)
                        {
                            _pressedButton = KeyCode.None;
                        }
                        else
                        {
                            _pressedButton = kcode;
                        }

                        SetPlayerPref(_pressedButton.ToString());

                        foreach (ControlInput _controlInput in allInputButtons)
                        {
                            if (_controlInput.GetKeyCode() == keyCodeForThisAction && _controlInput != this)
                            {
                                _controlInput.ClearButton();
                            }
                        }

                        isWaitingForInput = false;
                        backButton.interactable = true;

                        Invoke("ButtonsInteractableAgain", 0.3f);
                    }
                }
            }
        }
    }
    

   

    void ButtonsInteractableAgain()
    {
        foreach (ControlInput _controlInput in allInputButtons)
        {
            _controlInput.GetComponentInChildren<Button>().interactable = true;
        }
        toggleInputButton.GetComponentInChildren<Toggle>().interactable = true;
    }

    public void StartWaitingForInput()
    {
        foreach (ControlInput _controlInput in allInputButtons)
        {
            _controlInput.GetComponentInChildren<Button>().interactable = false;
        }
        toggleInputButton.GetComponentInChildren<Toggle>().interactable = false;

            buttonText.text = "...";
            isWaitingForInput = true;
            backButton.interactable = false;
    }

    public void SetPlayerPref(string _value)
    {
        PlayerPrefs.SetString(playerPrefsName, _value);
        keyCodeForThisAction = (KeyCode)Enum.Parse(typeof(KeyCode), _value);
        buttonText.text = _value;
    }

    private void ClearButton()
    {
        PlayerPrefs.SetString(playerPrefsName, KeyCode.None.ToString());
        keyCodeForThisAction = KeyCode.None;
        buttonText.text = KeyCode.None.ToString();
    }

    public void ResetToDefault()
    {
        PlayerPrefs.SetString(playerPrefsName,defaultKeyCode.ToString());
        keyCodeForThisAction = defaultKeyCode;
        buttonText.text = defaultKeyCode.ToString();
    }
}
