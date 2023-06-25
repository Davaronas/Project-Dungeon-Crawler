using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class CharacterMovement : MonoBehaviour
{
    private CharacterController cc = null;
    private Camera cam = null;
    private PlayerInput playerInput = null;

    private Vector3 currentMovementVector = Vector3.zero;
    private float verticalAxis = 0f;
    private float horizontalAxis = 0f;
    private float currentMovementSpeedForward = 2f;
    private float currentMovementSpeedNotForward = 1f;
    private Vector3 verticalMovementVector = Vector3.zero;
    private Vector3 horizontalMovementVector = Vector3.zero;

    private float mouseLookY = 0f;
    private float mouseLookX_Camera = 0f;
    private float currentCameraRotationX = 0f;

    private Vector3 gravity = Physics.gravity;

    private bool moving = false;
    private bool running = false;
    private bool pressingRunButton = false;

    private InventoryManager inventoryManager = null;


    KeyCode Run = KeyCode.None;
    KeyCode Dodge = KeyCode.None;




    [Header("Movement")]
    [SerializeField] private float movementSpeedForward = 15f;
    [SerializeField] private float movementSpeedNotForward = 10f;
    [SerializeField] private float gravityStrenght = 1f;
    [SerializeField] private float runMultiplier = 1.8f;
    [SerializeField] private float jumpStrength = 1f;

    [Header("Rotation")]
    [SerializeField] private float lookSensitivity = 1f;
    [SerializeField] private float maxTiltAngleDown = 30f;
    [SerializeField] private float maxTiltAngleUp = -45f;




    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        Run = playerInput.Run;
        Dodge = playerInput.Dodge;

        cc = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        gravity *= gravityStrenght;

        currentMovementSpeedForward = movementSpeedForward;
        currentMovementSpeedNotForward = movementSpeedNotForward;

        inventoryManager = GetComponent<InventoryManager>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        
        MovementInput();
        RotatePlayer();
    }

    void Update()
    {
        RotateCamera();
       
    }



    private void MovementInput()
    {
        if(Input.GetKey(Run))
        {
            currentMovementSpeedForward = movementSpeedForward * runMultiplier;
            currentMovementSpeedNotForward = movementSpeedNotForward * runMultiplier;
            pressingRunButton = true;
        }
        else
        {
            currentMovementSpeedForward = movementSpeedForward;
            currentMovementSpeedNotForward = movementSpeedNotForward;
            pressingRunButton = false;
        }

        if (Input.GetAxis("Vertical") != 0 && !inventoryManager.IsInventoryActive())
        {
            verticalAxis = Input.GetAxis("Vertical");

            if (verticalAxis > 0)
            {
                verticalMovementVector = transform.forward * currentMovementSpeedForward * verticalAxis * Time.fixedDeltaTime; // Change to networking time
            }
            else
            {
                verticalMovementVector = transform.forward * currentMovementSpeedNotForward * verticalAxis * Time.fixedDeltaTime; // Change to networking time
            }
        }
        else
        {
            verticalAxis = 0f;
        }

        if (Input.GetAxis("Horizontal") != 0 && !inventoryManager.IsInventoryActive())
        {
            horizontalAxis = Input.GetAxis("Horizontal");

            horizontalMovementVector = transform.right * currentMovementSpeedNotForward * horizontalAxis * Time.fixedDeltaTime; // Change to networking time
        }
        else
        {
            horizontalAxis = 0f;
        }

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if (!inventoryManager.IsInventoryActive())
            {
                moving = true;
                if (pressingRunButton)
                {
                    running = true;
                }
                else
                {
                    running = false;
                }
            }
            else
            {
                moving = false;
                running = false;
            }
        }
        else
        {
            moving = false;
            running = false;
        }

        if(verticalAxis != 0f || horizontalAxis != 0f)
        {
            if (!inventoryManager.IsInventoryActive())
            {
                currentMovementVector = verticalMovementVector + horizontalMovementVector + gravity;
                cc.Move(currentMovementVector);
            }

        }
        else
        {
            currentMovementVector = gravity;
            cc.Move(currentMovementVector);
        }


    }

    private void RotatePlayer()
    {
        if (!inventoryManager.IsInventoryActive())
        {
            mouseLookY = Input.GetAxis("Mouse X") * lookSensitivity * Time.fixedDeltaTime; // Change to networking time

            Vector3 currentRotation = new Vector3(0f, mouseLookY, 0f);

            transform.Rotate(currentRotation);
        }

    }

    private void RotateCamera()
    {

        if (!inventoryManager.IsInventoryActive())
        {
            mouseLookX_Camera = Input.GetAxis("Mouse Y") * lookSensitivity * Time.fixedDeltaTime; // Change to networking time
                                                                                        //  print(mouseLookY + " " + mouseLookX_Camera);
            
            currentCameraRotationX -= mouseLookX_Camera;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, maxTiltAngleUp, maxTiltAngleDown);

        }
        
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
        
       

    }



    public bool IsMoving()
    {
        return moving;
    }

    public bool IsRunning()
    {
        return running;
    }

    public float GetRunMultiplier()
    {
        return runMultiplier;
    }

    public float[] GetCC_Vel()
    {
        float[] _vel = new float[2];

            _vel[0] = horizontalAxis;
            _vel[1] = verticalAxis;
      

        return _vel;
    }
}
