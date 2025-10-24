using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float jumpHeight;

    private Animator animator;
    private CharacterController characterController;

    private Vector3 controllerCenter = Vector3.zero;
    private float controllerRad = 0;

    private int speed = 4;
    private float fallVelocity = 0;
    private float jumpVelocity = 0;
    private bool attack = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        controllerRad = characterController.radius;
        controllerCenter = characterController.center;
    }

    void Update()
    {
        AnimatorStateInfo stateInfo;
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        RaycastHit hit = new RaycastHit();
        bool isGrounded = Physics.SphereCast(transform.TransformPoint(controllerCenter), controllerRad, Vector3.down, out hit, 1.1f - controllerRad);

        if (isGrounded)
        {
            Physics.Raycast(hit.point + 0.05f * Vector3.up, Vector3.down, out hit, 0.1f);
            if (fallVelocity > jumpVelocity || Vector3.Angle(Vector3.up, hit.normal) > 45) jumpVelocity = 0f;
        }

        fallVelocity += isGrounded && jumpVelocity == 0 ? -fallVelocity : 9.8f * Time.deltaTime;

        animator.SetBool("IsFalling", fallVelocity > 0);
        animator.SetBool("Jump", jumpVelocity > 0);

        //
        if (stateInfo.IsTag("AfterAttack") || fallVelocity > 0) attack = false;
        if (attack) return;

        //Movement
        Vector3 movementInput = Vector3.zero;
        movementInput.x = Input.GetAxis("Horizontal");
        movementInput.z = Input.GetAxis("Vertical");

        bool isMoving = movementInput.magnitude != 0;
        animator.SetBool("IsMoving", isMoving);

        bool sprint = Input.GetKey(KeyCode.LeftShift);
        animator.SetBool("IsSprinting", sprint);

        Vector3 horizontalMovement = Vector3.zero;

        if (isMoving)
        {
            if (isGrounded) speed = sprint ? 8 : 4;

            Vector3 viewDirection = Vector3.ProjectOnPlane(mainCamera.forward, Vector3.up);
            Vector3 movementDirection = (movementInput.z * viewDirection + movementInput.x * mainCamera.right).normalized;

            Quaternion characterRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = characterRotation;

            horizontalMovement = movementDirection * speed * Time.deltaTime; 
        }

        float verticalMovementVelocity = (!isGrounded || jumpVelocity > 0) ? (jumpVelocity - fallVelocity) : -9.8f;
        Vector3 verticalMovement = verticalMovementVelocity * Vector3.up * Time.deltaTime;

        characterController.Move(horizontalMovement + verticalMovement);

        //
        if (stateInfo.IsTag("VerticalMovement") || fallVelocity + jumpVelocity > 0) return;

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
            jumpVelocity = Mathf.Sqrt(jumpHeight * 2 * 9.8f);
            return;
        }

        //Attack
        if (Input.GetMouseButtonDown(0))
        {
            attack = true;
            animator.SetTrigger("Attack");
        }
    }
}
