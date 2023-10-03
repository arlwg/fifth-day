using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
   [Header("Player Stats")]
   public float health = 100.0f;
   public float maxHealth = 100.0f;
   public float stamina = 0;
   public float maxStamina = 100.0f;
   
   
   [Header("Mouse Properties")]
   public float mouseSensetivity = 5.0f;
   public Vector2 MaxMinYCameraRotation = new Vector2(-85.0f,90.0f);
   [Header("For Tests in Unity")] 
   public bool enableMouseRotation = false;
   [Header("Movement Properties")]
    public CharacterController controller;
    public QuestManager QuestManager;
    public Joystick joystick;
    public float walkSpeed = 5.0f;

    
    private float currentSpeed;
    public float gravity = -50.0f;
    public float jumpHeight = 2.5f;
    Vector3 velocity;


    [Header("Ground Detection Properties")]
    public bool isGrounded;
    public LayerMask groundMask;
    public Transform groundPoint;
    public float groundRadius = 0.4f;
    public bool isGizmosVisible = false;

    public InventoryManagerNew invetoryManager;


    private UIManager _UIManager;

    void Start()
    {
        invetoryManager = FindObjectOfType<InventoryManagerNew>();
        joystick = FindObjectOfType<FixedJoystick>();
        controller = GetComponent<CharacterController>();
        QuestManager = FindObjectOfType<QuestManager>();
        _UIManager = FindObjectOfType<UIManager>();
     //stats part
        {
            currentSpeed = walkSpeed;
            health = maxHealth;
            stamina = maxStamina;
        }
        
        //if game loaded from a saving
        if(DataManager.Instance.selectedSave.seed != -1)
        {
            transform.position = DataManager.Instance.selectedSave.playerPos;
        }
        
        Vector3 move = transform.right * 1.0f + transform.forward * 0.0f;
        controller.Move(move * Time.deltaTime * currentSpeed);
    }

    void Update()
    {
        StaminaRecover();
        float movementLength = 0.0f;
        if (_UIManager.IsPaused)
            return;
        
        isGrounded = Physics.CheckSphere(groundPoint.position,groundRadius,groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }

        float x = Input.GetAxis("Horizontal") + joystick.Horizontal;   // + ((Application.isMobilePlatform) ? joystick.Horizontal : 0.0f) @@Use this later
        float z = Input.GetAxis("Vertical") + joystick.Vertical;       // + ((Application.isMobilePlatform) ? joystick.Vertical : 0.0f) @@Use this later

        movementLength = new Vector2(x, z).magnitude;
       
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * Time.deltaTime * currentSpeed);
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity*Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
             Jump();
        }
    }

    public void StaminaRecover()
    {
        if (stamina < maxStamina)
        {
            stamina += Time.deltaTime * 3.5f;
        }
        else if(stamina > maxStamina)
        {
            stamina = maxStamina;
        }
    }
    public void Jump()
    {
        if (isGrounded && stamina > 5.0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            stamina -= 5.0f;
        }
            
    }
    private void OnDrawGizmos()
     {
        if(isGizmosVisible)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(groundPoint.position,groundRadius);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EnemyAttack"))
        {
            health -= 3;
        }
    }

}
