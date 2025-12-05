/*

                                                                                                                                                                                                                 
                                                                                                              
    ███        ▄█    █▄       ▄████████         ▄██████▄     ▄████████   ▄▄▄▄███▄▄▄▄      ▄████████           
▀█████████▄   ███    ███     ███    ███        ███    ███   ███    ███ ▄██▀▀▀███▀▀▀██▄   ███    ███           
   ▀███▀▀██   ███    ███     ███    █▀         ███    █▀    ███    ███ ███   ███   ███   ███    █▀            
    ███   ▀  ▄███▄▄▄▄███▄▄  ▄███▄▄▄           ▄███          ███    ███ ███   ███   ███  ▄███▄▄▄               
    ███     ▀▀███▀▀▀▀███▀  ▀▀███▀▀▀          ▀▀███ ████▄  ▀███████████ ███   ███   ███ ▀▀███▀▀▀               
    ███       ███    ███     ███    █▄         ███    ███   ███    ███ ███   ███   ███   ███    █▄            
    ███       ███    ███     ███    ███        ███    ███   ███    ███ ███   ███   ███   ███    ███           
   ▄████▀     ███    █▀      ██████████        ████████▀    ███    █▀   ▀█   ███   █▀    ██████████           
                                                                                                              
                                                                                                              
                                                                                                              
                                                                                                          
      ___      __      _____  ___        ________     __        _______    __  ___      ___  ____  ____    ______    
     |"  |    /""\    (\"   \|"  \      /"       )   /""\      /"      \  |" \|"  \    /"  |("  _||_ " |  /    " \   
     ||  |   /    \   |.\\   \    |    (:   \___/   /    \    |:        | ||  |\   \  //  / |   (  ) : | // ____  \  
     |:  |  /' /\  \  |: \.   \\  |     \___  \    /' /\  \   |_____/   ) |:  | \\  \/. ./  (:  |  | . )/  /    ) :) 
  ___|  /  //  __'  \ |.  \    \. |      __/  \\  //  __'  \   //      /  |.  |  \.    //    \\ \__/ //(: (____/ //  
 /  :|_/ )/   /  \\  \|    \    \ |     /" \   :)/   /  \\  \ |:  __   \  /\  |\  \\   /     /\\ __ //\ \        /   
(_______/(___/    \___)\___|\____\)    (_______/(___/    \___)|__|  \___)(__\_|_)  \__/     (__________) \"_____/    
                                                                                                                     
             ___________  __  ___      ___  __          _______     ______    _______    _______                     
            ("     _   ")|" \|"  \    /"  ||" \        /"     "\   /    " \  /"     "\  /"     "\                    
             )__/  \\__/ ||  |\   \  //  / ||  |      (__/\    :) // ____  \(__/\    :)(__/\    :)                   
                \\_ /    |:  | \\  \/. ./  |:  |          / ___/ /  /    ) :)   / ___/     / ___/                    
                |.  |    |.  |  \.    //   |.  |         // \___(: (____/ //   // \___    // \___                    
                \:  |    /\  |\  \\   /    /\  |\       (:  /  "\\        /   (:  /  "\  (:  /  "\                   
                 \__|   (__\_|_)  \__/    (__\_|_)       \_______)\"_____/     \_______)  \_______)        
                   
                                     © 2025 Jan Sarivuo. All Rights Reserved.        
                                                                                                                     

*/




using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class CharacterControls : MonoBehaviour
{
    public float walkSpeed = 15.0f;
    public float speed = 15.0f;
    public float runSpeed = 18.0f;
    public float airVelocity = 8f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f; // Maksimi nopeuden muutos, estää äkkiliikkeet
    public float jumpHeight = 2.0f;
    public float maxFallSpeed = 20.0f;
    public float rotateSpeed = 25f; 
    private Vector3 moveDir; // Liikesuunta
    public GameObject cam;
    private Rigidbody rb;
    public Animator animator;

    private float distToGround; // Käytetään maassa olemisen tarkistamiseen

    private bool canMove = true;
    private bool isStuned = false;
    private bool wasStuned = false;
    private float pushForce;
    private Vector3 pushDir;

    public Vector3 checkPoint;

    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;
        animator.ResetTrigger("Jump");
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, distToGround + 0.2f))
        {
            return !hit.collider.isTrigger && hit.collider.gameObject != gameObject;
        }
        return false;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;

        checkPoint = transform.position;
        Cursor.visible = false;
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            // Hahmon kääntyminen liikesuuntaan
            if (moveDir.x != 0 || moveDir.z != 0)
            {
                Vector3 targetDir = moveDir;
                targetDir.y = 0;
                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed);
                transform.rotation = targetRotation;
            }

            if (IsGrounded())
            {
                Vector3 targetVelocity = moveDir * speed;
                Vector3 velocity = rb.linearVelocity;
                if (targetVelocity.magnitude < velocity.magnitude)
                {
                    targetVelocity = velocity;
                    rb.linearVelocity /= 1.1f;
                }

                Vector3 velocityChange = targetVelocity - velocity;
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;

                if (Mathf.Abs(rb.linearVelocity.magnitude) < speed * 1.0f)
                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
            }
            else
            {
                Vector3 velocity = rb.linearVelocity;
                Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, velocity.y, moveDir.z * airVelocity);
                Vector3 velocityChange = targetVelocity - velocity;
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                rb.AddForce(velocityChange, ForceMode.VelocityChange);

                if (velocity.y < -maxFallSpeed)
                    rb.linearVelocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
            }
        }
        else
        {
            rb.linearVelocity = pushDir * pushForce;
        }
        // Lisätään painovoima manuaalisesti
        rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 v2 = v * cam.transform.forward;
        Vector3 h2 = h * cam.transform.right;
        moveDir = (v2 + h2).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }
        // Juoksu (shift pohjassa)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        speed = isRunning ? runSpeed : walkSpeed;
        
        // Animaatioiden ohjaus
        float moveAmount = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
        animator.SetFloat("Speed", moveAmount);
        animator.SetBool("IsRunning", isRunning);
    }

    float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    void Jump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = CalculateJumpVerticalSpeed();
        rb.linearVelocity = velocity;

        animator.SetTrigger("Jump");
    }

    public void HitPlayer(Vector3 velocityF, float time)
    {
        rb.linearVelocity = velocityF;
        pushForce = velocityF.magnitude;
        pushDir = Vector3.Normalize(velocityF);
        StartCoroutine(Decrease(velocityF.magnitude, time));
    }

    public void LoadCheckPoint()
    {
        transform.position = checkPoint;
    }

    private IEnumerator Decrease(float value, float duration)
    {
        if (isStuned)
            wasStuned = true;
        isStuned = true;
        canMove = false;

        float delta = value / duration;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;
            pushForce = Mathf.Max(0, pushForce - Time.deltaTime * delta);
            rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));
        }

        if (wasStuned)
        {
            wasStuned = false;
        }
        else
        {
            isStuned = false;
            canMove = true;
        }
    }
}
