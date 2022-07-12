using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public static InputSystem instance;
    private void Awake()
    {
        if (InputSystem.instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    [Header("Processed Inputs")]
    public float MovementX;
    public float MovementY;

    public float CameraX;
    public float CameraY;

    public bool Sprint;
    public bool Punch;
    public bool Jump;
    public bool Crouch;
    public bool StartGame;
    public bool Grab;

    [Space]
    [Header("Input Keyboard & Mouse")]

    [SerializeField] private KeyCode punchKey;
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode crouchKey;
    [SerializeField] private KeyCode startGameKey;
    [SerializeField] private KeyCode grabKey;

    [Space]
    [Header("Input Controller")]
    [SerializeField] private KeyCode punchControllerKey;
    [SerializeField] private KeyCode jumpControllerKey;
    [SerializeField] private KeyCode crouchControllerKey;
    [SerializeField] private KeyCode startGameControllerKey;
    [SerializeField] private KeyCode grabControllerKey;

    private bool invertMovementInput = false;

    public void SetInvertMovementInput(bool value)
    {
        invertMovementInput = value;
    }



    private void Update()
    {
        //Set move Inputs
        if (!invertMovementInput)
        {
            MovementX = Input.GetAxis("Horizontal");
            MovementY = Input.GetAxis("Vertical");
        }
        else
        {
            MovementX =  Input.GetAxis("Horizontal");
            MovementY = -Input.GetAxis("Vertical");
        }
      

        //set camera inputs
        CameraX = Input.GetAxis("Mouse X");
        CameraY = Input.GetAxis("Mouse Y");

        //set sprint input
        Sprint = Input.GetAxis("Sprint") > 0.95f;

        //set punch input
        Punch = Input.GetKeyDown(punchKey) || Input.GetKeyDown(punchControllerKey);

        //set jump input
        Jump = Input.GetKeyDown(jumpKey) || Input.GetKeyDown(jumpControllerKey);

        //set crouch input
        Crouch = Input.GetKey(crouchKey) || Input.GetKey(crouchControllerKey);

        //start game input
        StartGame = Input.GetKeyDown(startGameKey) || Input.GetKeyDown(startGameControllerKey);

        Grab = Input.GetKeyDown(grabKey) || Input.GetKeyDown(grabControllerKey);
    }


}
