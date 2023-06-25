using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(PlayerInput))]

public class AnimationControl : MonoBehaviour
{
    private CharacterMovement cM = null;
    private Animator animator = null;
    private InventoryManager inventoryManager = null;
    private PlayerInput playerInput = null;
    private UI_Manager uiManager = null;

    private ItemDatabase.AnimationType currentAnimationType = ItemDatabase.AnimationType.None;

    public enum Directions { Left, Right };
    private Directions attackDirection = Directions.Right;
    private Directions nextAttackDirection = Directions.Left;

    private enum AttackType { Slash, Stab, Overhead, None };
    private AttackType comboInput = AttackType.None;

    private bool recoveryPhase = false;
    private bool stopDoubleInput = false;
    private bool canCancelAttack = false;
    private bool holdingBow = false;

    [SerializeField] private float doubleInputProtectTime = 0.1f;


    private KeyCode Attack = KeyCode.None;
    private KeyCode Block = KeyCode.None;
    private KeyCode Kick = KeyCode.None;
    private KeyCode Dodge = KeyCode.None;

    private KeyCode AltOH = KeyCode.None;
    private KeyCode AltStab = KeyCode.None;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        Attack = playerInput.Attack;
        Block = playerInput.Block;
        Kick = playerInput.Kick;
        Dodge = playerInput.Dodge;

        AltOH = playerInput.AltOH;
        AltStab = playerInput.AltStab;

        cM = GetComponent<CharacterMovement>();
        animator = GetComponent<Animator>();
        inventoryManager = GetComponent<InventoryManager>();
        uiManager = GetComponent<UI_Manager>();
        animator.SetFloat("RunMultiplier", cM.GetRunMultiplier());
        animator.SetBool("Right", true);
        animator.SetBool("NoItemLayer", true);

    }

    void Update() // MAKE INVENTORY OPEN STOP ANIMATIONS!
    {

        if (!inventoryManager.IsInventoryActive())
        {
            MovementAnimations();
            SwitchDirection();


            if (currentAnimationType != ItemDatabase.AnimationType.Bow && currentAnimationType != ItemDatabase.AnimationType.Potion)
            {
                AttackInput();
                BlockInput();
            }

            if (currentAnimationType == ItemDatabase.AnimationType.Potion)
            {
                if (IsIdle())
                {
                    if (Input.GetKeyDown(Attack))
                    {
                        animator.SetBool("Slash", true);
                    }
                }
            }

            if (currentAnimationType == ItemDatabase.AnimationType.Bow)
            {
                if (IsIdle())
                {
                    if (Input.GetKeyDown(Attack))
                    {
                        animator.SetBool("Slash", true);
                    }
                }

                if (holdingBow)
                {
                    if (Input.GetKeyUp(Attack))
                    {
                        animator.SetFloat("StopBow", 1f);
                        holdingBow = false;
                    }

                }
            }


        }
    }

    public bool IsIdle()
    {
        if (animator.GetBool("Slash") == true ||
           animator.GetBool("Stab") == true ||
           animator.GetBool("Overhead") == true ||
           recoveryPhase == true ||
           animator.GetBool("Block") == true)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void StopMovementAnimations()
    {
        animator.SetFloat("VelX", 0f);
        animator.SetFloat("VelZ", 0f);

        animator.SetBool("Moving", false);
        animator.SetBool("Running", false);
    }

    private void MovementAnimations()
    {
        float[] _ccVel = cM.GetCC_Vel();

        animator.SetFloat("VelX", _ccVel[0]);
        animator.SetFloat("VelZ", _ccVel[1]);

        if (cM.IsMoving())
        {
            if (animator.GetBool("Moving") == false)
            {
                animator.SetBool("Moving", true);
            }

            if (animator.GetBool("Running") == true)
            {
                animator.SetFloat("RunMultiplier", 0.7f);
            }
            else
            {
                animator.SetFloat("RunMultiplier", 0.5f);
            }
        }
        else
        {
            if (animator.GetBool("Moving") == true)
            {
                animator.SetBool("Moving", false);
            }
        }


        if (cM.IsRunning())
        {
            if (animator.GetBool("Running") == false)
            {
                animator.SetBool("Running", true);
            }
        }
        else
        {
            if (animator.GetBool("Running") == true)
            {
                animator.SetBool("Running", false);
            }
        }
    }

    #region Combat


    #region Normal
    #region Attack
    private void SwitchDirection()
    {
        if (!LockedInAttack())
        {
            if (Input.GetAxis("Mouse X") < -1)
            {
                if (attackDirection != Directions.Left)
                {
                    attackDirection = Directions.Left;
                    animator.SetBool("Right", false);
                    animator.SetBool("Left", true);
                }
            }
            else if (Input.GetAxis("Mouse X") > 1)
            {
                if (attackDirection != Directions.Right)
                {
                    attackDirection = Directions.Right;
                    animator.SetBool("Left", false);
                    animator.SetBool("Right", true);
                }
            }
        }
    }

    private void AttackInput() // MAKE RECOVERY PHASE
    {
        if (stopDoubleInput == false)
        {
            if (animator.GetBool("Block") == false)
            {

                if (!LockedInAttack())
                {
                    if (Input.GetKeyDown(Attack))
                    {
                        animator.SetBool("Slash", true);
                        comboInput = AttackType.None;
                        canCancelAttack = true;
                        StartCoroutine(DoubleInputProtection());
                    }

                    if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(AltStab)) // MAKE TURNING OFF SCROLLWHEEL
                    {
                        animator.SetBool("Stab", true);
                        comboInput = AttackType.None;
                        canCancelAttack = true;
                        StartCoroutine(DoubleInputProtection());
                    }

                    if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(AltOH)) // MAKE TURNING OFF SCROLLWHEEL
                    {
                        animator.SetBool("Overhead", true);
                        comboInput = AttackType.None;
                        canCancelAttack = true;
                        StartCoroutine(DoubleInputProtection());
                    }
                }
                else // COMBO WINDOW
                {
                    if (attackDirection == Directions.Right)
                    {
                        nextAttackDirection = Directions.Left;
                    }
                    else
                    {
                        nextAttackDirection = Directions.Right;
                    }

                    if (Input.GetKeyDown(Attack))
                    {
                        comboInput = AttackType.Slash;
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(AltStab)) // MAKE TURNING OFF SCROLLWHEEL
                    {
                        comboInput = AttackType.Stab;
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(AltOH)) // MAKE TURNING OFF SCROLLWHEEL
                    {
                        comboInput = AttackType.Overhead;
                    }
                    else
                    {

                    }
                }
            }
            else
            {

            }
        }
    }


    public Directions GetAttackDirection()
    {
        return attackDirection;
    }

    public bool LockedInAttack()
    {
        if (animator.GetBool("Slash") == true ||
            animator.GetBool("Stab") == true ||
            animator.GetBool("Overhead") == true ||
            recoveryPhase == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FreeAttackAnimation()
    {
        animator.SetBool("Slash", false);
        animator.SetBool("Stab", false);
        animator.SetBool("Overhead", false);
        comboInput = AttackType.None;
    }

    IEnumerator StartRecoveryPhase()
    {
        recoveryPhase = true;
        yield return new WaitForSeconds(0.3f);
        recoveryPhase = false;
    }

    IEnumerator DoubleInputProtection()
    {
        stopDoubleInput = true;
        yield return new WaitForSeconds(doubleInputProtectTime);
        stopDoubleInput = false;
    }
    #endregion

    #region Block
    private void BlockInput()
    {
        if (!LockedInAttack() || canCancelAttack)
        {
            if (Input.GetKey(Block))
            {
                if (animator.GetBool("Block") == false)
                {

                    animator.SetBool("Block", true);
                    if (canCancelAttack)
                    {
                        FreeAttackAnimation();
                        canCancelAttack = false;
                    }
                }
            }
            else
            {
                if (animator.GetBool("Block") == true)
                {
                    animator.SetBool("Block", false);
                }
            }
        }
        else
        {
            if (animator.GetBool("Block") == true)
            {
                animator.SetBool("Block", false);
            }
        }
    }
    #endregion

    #endregion


    #region Bow



    #endregion

    #endregion


    #region Animation Layers



    public void EquipItem(ItemDatabase.ItemInfo _itemInfo)
    {
        ChangeAnimLayer(_itemInfo.AnimType);
    }

    public void ChangeAnimLayer(ItemDatabase.AnimationType _animType)
    {

        switch (currentAnimationType) // TURNS OFF CURRENT ANIM LAYER
        {
            case ItemDatabase.AnimationType.None:

                animator.SetBool("NoItemLayer", false);

                int _layerIndex_CurrentMovement_None = animator.GetLayerIndex("FPS_Arms_NoItem_Movement");
                animator.SetLayerWeight(_layerIndex_CurrentMovement_None, 0f);

                int _layerIndex_Current_None = animator.GetLayerIndex("FPS_Arms_NoItem");
                animator.SetLayerWeight(_layerIndex_Current_None, 0f);

                break;

            case ItemDatabase.AnimationType.OneHanded_W:
                // animator.SetBool("OneHandedLayer", false);
                break;
            case ItemDatabase.AnimationType.TwoHanded_W:
                //   animator.SetBool("TwoHandedLayer", false);
                break;
            case ItemDatabase.AnimationType.Bow:

                animator.SetBool("BowLayer", false);

                int _layerIndex_CurrentMovement_Bow = animator.GetLayerIndex("FPS_Arms_Bow_Movement");
                animator.SetLayerWeight(_layerIndex_CurrentMovement_Bow, 0f);

                int _layerIndex_Current_Bow = animator.GetLayerIndex("FPS_Arms_Bow");
                animator.SetLayerWeight(_layerIndex_Current_Bow, 0f);

                uiManager.TurnOffDirectionIndicator();

                break;
            case ItemDatabase.AnimationType.Potion:

                animator.SetBool("PotionLayer", false);

                int _layerIndex_CurrentMovement_Potion = animator.GetLayerIndex("FPS_Arms_Potion_Movement");
                animator.SetLayerWeight(_layerIndex_CurrentMovement_Potion, 0f);

                int _layerIndex_Current_Potion = animator.GetLayerIndex("FPS_Arms_Potion");
                animator.SetLayerWeight(_layerIndex_Current_Potion, 0f);

                break;
        }

        currentAnimationType = _animType;

        switch (currentAnimationType) // TURNS ON NEXT LAYER
        {
            case ItemDatabase.AnimationType.None:

                animator.SetBool("NoItemLayer", true);

                int _layerIndex_CurrentMovement_None = animator.GetLayerIndex("FPS_Arms_NoItem_Movement");
                animator.SetLayerWeight(_layerIndex_CurrentMovement_None, 1f);

                int _layerIndex_Current_None = animator.GetLayerIndex("FPS_Arms_NoItem");
                animator.SetLayerWeight(_layerIndex_Current_None, 1f);

                uiManager.TurnOnDirectionIndicator();

                break;

            case ItemDatabase.AnimationType.OneHanded_W:
                // animator.SetBool("OneHandedLayer", false);

                uiManager.TurnOnDirectionIndicator();
                break;
            case ItemDatabase.AnimationType.TwoHanded_W:
                //   animator.SetBool("TwoHandedLayer", false);

                uiManager.TurnOnDirectionIndicator();
                break;
            case ItemDatabase.AnimationType.Bow:
                animator.SetBool("BowLayer", true);

                int _layerIndex_CurrentMovement_Bow = animator.GetLayerIndex("FPS_Arms_Bow_Movement");
                animator.SetLayerWeight(_layerIndex_CurrentMovement_Bow, 1f);

                int _layerIndex_Current_Bow = animator.GetLayerIndex("FPS_Arms_Bow");
                animator.SetLayerWeight(_layerIndex_Current_Bow, 1f);

                uiManager.TurnOffDirectionIndicator();
                break;
            case ItemDatabase.AnimationType.Potion:

                animator.SetBool("PotionLayer", true);

                int _layerIndex_CurrentMovement_Potion = animator.GetLayerIndex("FPS_Arms_Potion_Movement");
                animator.SetLayerWeight(_layerIndex_CurrentMovement_Potion, 1f);

                int _layerIndex_Current_Potion = animator.GetLayerIndex("FPS_Arms_Potion");
                animator.SetLayerWeight(_layerIndex_Current_Potion, 1f);

                uiManager.TurnOffDirectionIndicator();
                break;
        }

    }

    #endregion



    #region Animation Events




    public void EndOfAttack()
    {
        animator.SetBool("Slash", false);
        animator.SetBool("Stab", false);
        animator.SetBool("Overhead", false);

        switch (comboInput)
        {
            case AttackType.Slash:

                if (nextAttackDirection == Directions.Right)
                {
                    animator.SetBool("Right", true);
                    animator.SetBool("Slash", true);
                    animator.SetBool("Left", false);

                    attackDirection = Directions.Right;
                }
                else
                {
                    animator.SetBool("Left", true);
                    animator.SetBool("Slash", true);
                    animator.SetBool("Right", false);

                    attackDirection = Directions.Left;
                }

                break;
            case AttackType.Stab:

                if (nextAttackDirection == Directions.Right)
                {
                    animator.SetBool("Right", true);
                    animator.SetBool("Stab", true);
                    animator.SetBool("Left", false);

                    attackDirection = Directions.Right;
                }
                else
                {
                    animator.SetBool("Left", true);
                    animator.SetBool("Stab", true);
                    animator.SetBool("Right", false);

                    attackDirection = Directions.Left;
                }

                break;
            case AttackType.Overhead:

                if (nextAttackDirection == Directions.Right)
                {
                    animator.SetBool("Right", true);
                    animator.SetBool("Overhead", true);
                    animator.SetBool("Left", false);

                    attackDirection = Directions.Right;
                }
                else
                {
                    animator.SetBool("Left", true);
                    animator.SetBool("Overhead", true);
                    animator.SetBool("Right", false);

                    attackDirection = Directions.Left;
                }

                break;
            case AttackType.None:
                StartCoroutine(StartRecoveryPhase());
                break;
        }

        comboInput = AttackType.None;
    }

    public void EndOfAttackCancelWindow()
    {
        canCancelAttack = false;
    }

    public void CheckForHoldingBow()
    {
        if (Input.GetKey(Attack))
        {
            animator.SetFloat("StopBow", 0f);
            holdingBow = true;
        }
    }

    public void ShootArrow()
    {

    }

    public void ExitPotionLayer()
    {




        animator.SetBool("NoItemLayer", true);

        int _layerIndex_CurrentMovement_None = animator.GetLayerIndex("FPS_Arms_NoItem_Movement");
        animator.SetLayerWeight(_layerIndex_CurrentMovement_None, 1f);

        int _layerIndex_Current_None = animator.GetLayerIndex("FPS_Arms_NoItem");
        animator.SetLayerWeight(_layerIndex_Current_None, 1f);

        uiManager.TurnOnDirectionIndicator();

        currentAnimationType = ItemDatabase.AnimationType.None;



        animator.SetBool("PotionLayer", false);

        animator.SetBool("Slash", false);

        int _layerIndex_CurrentMovement_Potion = animator.GetLayerIndex("FPS_Arms_Potion_Movement");
        animator.SetLayerWeight(_layerIndex_CurrentMovement_Potion, 0f);

        int _layerIndex_Current_Potion = animator.GetLayerIndex("FPS_Arms_Potion");
        animator.SetLayerWeight(_layerIndex_Current_Potion, 0f);
    }

    

    #endregion
}
