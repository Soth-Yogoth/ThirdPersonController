using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    //private Rigidbody rb;
    private CharacterController characterController;
    private float gravity = -9.8f;

    [SerializeField] private Transform camera;

    void Start()
    {
        animator = GetComponent<Animator>();
        //rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        RaycastHit hit;
        bool touchGround = Physics.BoxCast(transform.position + Vector3.up, new Vector3(0.375f, 0.05f, 0.3f), Vector3.down, out hit, Quaternion.LookRotation(transform.forward), 1f);

        animator.SetBool("OnGround", touchGround);

        if (!touchGround)
        {
            animator.SetBool("LockAction", true);

            animator.ResetTrigger("Jump");
            animator.ResetTrigger("Attack");
        }

        if (animator.GetBool("LockMovement"))
        {
            //rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        //Movement
        Vector3 viewDirection = new Vector3(camera.forward.x, 0, camera.forward.z).normalized;

        float velocityX = Input.GetAxis("Horizontal");
        float velocityZ = Input.GetAxis("Vertical");

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        
        int speed = sprint ? 10 : 5;
        animator.SetBool("IsSprinting", sprint);

        if (velocityX != 0 || velocityZ != 0)
        {
            animator.SetBool("IsMoving", true);

            Vector3 movementDirection = (velocityZ * viewDirection + velocityX * camera.right).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = targetRotation;

            if (hit.normal != Vector3.up) 
            {
                movementDirection = Vector3.ProjectOnPlane(movementDirection, hit.normal);
            }

            Vector3 movementVelocity = (movementDirection * speed + Vector3.up * gravity) * Time.deltaTime;
            //rb.linearVelocity = new Vector3(movementVelocity.x, rb.linearVelocity.y, movementVelocity.z);
            characterController.Move(movementVelocity);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            characterController.Move(Vector3.up * gravity * Time.deltaTime);
            //rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        if (animator.GetBool("LockAction")) return;

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
            //characterController.Move(Vector3.up * 3);
        }

        //Attack
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }
}
