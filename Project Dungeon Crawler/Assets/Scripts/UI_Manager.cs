using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AnimationControl))]

public class UI_Manager : MonoBehaviour
{
    private AnimationControl animControl = null;
    private RectTransform directionImageRectTransform = null;
    private Image directionimage = null;
    private AnimationControl.Directions lastDirection = AnimationControl.Directions.Right;
    private bool lastIsAttackingState = false;
    private Text raycastItemNameText = null;

    void Start()
    {
        animControl = GetComponent<AnimationControl>();
        directionImageRectTransform = GameObject.Find("DirectionImage").GetComponent<RectTransform>();
        directionimage = directionImageRectTransform.GetComponent<Image>();
        raycastItemNameText = GameObject.Find("RaycastItemName").GetComponent<Text>();
    }

    void Update()
    {
        SwitchDirection();
        SwitchColor();
    }

    private void SwitchDirection()
    {
        AnimationControl.Directions direction = animControl.GetAttackDirection();

        if (direction == AnimationControl.Directions.Right)
        {
            if (lastDirection == AnimationControl.Directions.Left)
            {
                lastDirection = direction;
                SwitchToRightSide();
            }
        }
        else
        {
            if (lastDirection == AnimationControl.Directions.Right)
            {
                lastDirection = direction;
                SwitchToLeftSide();
            }
        }
    }


    private void SwitchToRightSide()
    {
        Vector3 _screenPos = new Vector3((Screen.width / 2) + 30, Screen.height / 2, 0);
        Vector3 _screenRotEulerAngles = new Vector3(0, 0, 0);
        Quaternion _rot = Quaternion.Euler(_screenRotEulerAngles);
        directionImageRectTransform.position = _screenPos;
        directionImageRectTransform.rotation = _rot;
    }

    private void SwitchToLeftSide()
    {
        Vector3 _screenPos = new Vector3((Screen.width / 2) - 30, Screen.height / 2, 0);
        Vector3 _screenRotEulerAngles = new Vector3(0, 0, 180);
        Quaternion _rot = Quaternion.Euler(_screenRotEulerAngles);
        directionImageRectTransform.position = _screenPos;
        directionImageRectTransform.rotation = _rot;
    }

    private void SwitchColor()
    {
        bool _attacking = animControl.LockedInAttack();

        if(_attacking)
        {
            if(lastIsAttackingState == false)
            {
                directionimage.color = new Color32(255, 0, 0, 255);
            }
            lastIsAttackingState = true;
        }
        else
        {
            if(lastIsAttackingState == true)
            {
                directionimage.color = new Color32(200, 200, 200, 255);
            }
            lastIsAttackingState = false;
        }
    }

    public void TurnOffDirectionIndicator()
    {
        directionimage.enabled = false;
    }

    public void TurnOnDirectionIndicator()
    {
        directionimage.enabled = true;
    }

    public void ChangeItemName(string _name)
    {
        raycastItemNameText.text = _name;
    }
}
