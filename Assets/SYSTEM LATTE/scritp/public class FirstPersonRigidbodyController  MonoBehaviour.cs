using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class FirstPersonRigidbodyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public float feetOffset = 0.1f;       // Offset ??? centre ??????? “????”
    public float feetRayLength = 0.2f;    // ???? ray ??????? “??????????????”

    [Header("References")]
    public Camera playerCamera;

    Rigidbody rb;
    Collider col;

    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // ??? Rigidbody ??? Physics ??????????: ??????????????????? ??????????
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleLook();
        HandleMovementInput();
        HandleJumpInput();
    }

    void FixedUpdate()
    {
        UpdateGroundedStatus();
        ApplyMovement();
    }

    void HandleLook()
    {
        // ????????: ???????-??? (Yaw) ??? GameObject ???, ???????-?? (Pitch) ??? camera
        float yaw = Input.GetAxis("Mouse X");
        float pitch = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * yaw, Space.World);

        if (playerCamera)
        {
            playerCamera.transform.Rotate(Vector3.right * -pitch, Space.Self);
        }
    }

    float moveInputX;
    float moveInputZ;

    void HandleMovementInput()
    {
        moveInputX = Input.GetAxis("Horizontal");
        moveInputZ = Input.GetAxis("Vertical");
    }

    bool jumpRequest;

    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequest = true;
        }
    }

    void UpdateGroundedStatus()
    {
        // ????????? “????” ?????? collider bounds + offset
        Vector3 centre = col.bounds.center;
        float halfHeight = col.bounds.extents.y;
        Vector3 feetPos = centre + Vector3.down * (halfHeight - feetOffset);

        // Raycast ?????? “????” ????????? layer mask ????? (??? ~0 = ??? layer)
        if (Physics.Raycast(feetPos, Vector3.down, out RaycastHit hit, feetRayLength, ~0))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void ApplyMovement()
    {
        // ??????????????????? local ??????????
        Vector3 localMovement = new Vector3(moveInputX, 0f, moveInputZ);
        localMovement = Vector3.ClampMagnitude(localMovement, 1f);
        localMovement *= moveSpeed;

        // ??????????? world space
        Vector3 worldMovement = transform.TransformDirection(localMovement);

        // ???? linearVelocity — ???? y (vertical) ??????????? ???????????????? / ?????????
        Vector3 currentVel = rb.linearVelocity;
        Vector3 newVel = new Vector3(worldMovement.x, currentVel.y, worldMovement.z);
        rb.linearVelocity = newVel;

        // ??????
        if (jumpRequest)
        {
            // ??? ForceMode.VelocityChange ????????????????
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            jumpRequest = false;
        }
    }
}
