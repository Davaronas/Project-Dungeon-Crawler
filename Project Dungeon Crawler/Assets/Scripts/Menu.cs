using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject controls;
    private ControlInput[] allControlInput = null;
    private ControlInput_Toggle toggleInput = null;

    private void Awake()
    {
        allControlInput = FindObjectsOfType<ControlInput>();
        toggleInput = FindObjectOfType<ControlInput_Toggle>();
    }

    private void Start()
    {
        controls.SetActive(false);
    }

    public void GoToTestMap()
    {
        SceneManager.LoadScene("Testing");
    }

    public void ResetAllInput()
    {
        foreach(ControlInput _cI in allControlInput)
        {
            _cI.ResetToDefault();
        }
        toggleInput.ResetToDefault();
    }
}
