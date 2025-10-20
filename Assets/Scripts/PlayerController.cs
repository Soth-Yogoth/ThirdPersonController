using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float jumpHeight;

    private Animator animator;
    private CharacterController characterController;

    private int speed = 5;
    private float velocityY = -9.8f;
    private float raycastRad = 0;
    private bool isFalling = false;
    private bool jumpStarted = false;
    private bool attackStarted = false;
    
    

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        raycastRad = characterController.radius;
    }

    void Update()
    {
        AnimatorStateInfo stateInfo;
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        RaycastHit hit = new RaycastHit();
        bool isGrounded = Physics.SphereCast(transform.TransformPoint(characterController.center), raycastRad, Vector3.down, out hit, 1.1f - raycastRad);

        if (isFalling || velocityY > 0) velocityY -= 9.8f * Time.deltaTime;
        else velocityY = isGrounded ? -9.8f : 0f;

        isFalling = !isGrounded && velocityY <= 0;
        animator.SetBool("IsFalling", isFalling);

        jumpStarted = stateInfo.IsTag("VerticalMovement") || isFalling ? false : jumpStarted;
        attackStarted = stateInfo.IsTag("Attack") || isFalling ? false : attackStarted;

        //
        if (stateInfo.IsTag("Attack") || attackStarted) return;

        //Movement
        float rightMovement = Input.GetAxis("Horizontal");
        float forwardMovement = Input.GetAxis("Vertical");

        bool isMoving = forwardMovement != 0 || rightMovement != 0;
        animator.SetBool("IsMoving", isMoving);

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("IsSprinting", sprint);

        if (isGrounded) speed = sprint ? 10 : 5;

        Vector3 viewDirection = Vector3.ProjectOnPlane(mainCamera.forward, Vector3.up);
        Vector3 movementDirection = (forwardMovement * viewDirection + rightMovement * mainCamera.right).normalized;

        Vector3 movementVelocity = (movementDirection * speed + Vector3.up * velocityY) * Time.deltaTime;
        characterController.Move(movementVelocity);

        if (isMoving)
        {
            Quaternion characterRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = characterRotation;
        }

        //
        if (stateInfo.IsTag("VerticalMovement") || jumpStarted || !isGrounded) return;

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpStarted = true;
            animator.SetTrigger("Jump");
            velocityY = Mathf.Sqrt(jumpHeight * 2 * 9.8f);
            return;
        }

        //Attack
        if (Input.GetMouseButtonDown(0))
        {
            attackStarted = true;
            animator.SetTrigger("Attack");
        }
    }
}
