using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float jumpSpeed;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float gravityMultiplier;

    private Vector3 moveDirection = Vector3.zero;
    private Vector2 input;
    private CharacterController characterController;
    private bool jumping;
    private bool jump;
    private bool previouslyGrounded;
    private bool previouslyOnCeiling;
    private bool isWalking;
    private bool onCeiling;


    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        jumping = false;
    }

    void Update()
    {
        if (characterController.collisionFlags == CollisionFlags.Above)
        {
            onCeiling = true;
        }
        else
        {
            onCeiling = false;
        }

        if (!jump)
        {
            jump = Input.GetButtonDown("Jump");
        }

        if ((!previouslyGrounded && characterController.isGrounded) || (!previouslyOnCeiling && onCeiling))
        {
            moveDirection.y = 0;
            jumping = false;
        }

        if ((!characterController.isGrounded || !onCeiling) && !jumping && (previouslyGrounded || previouslyOnCeiling))
        {
            moveDirection.y = 0f;
        }

        previouslyGrounded = characterController.isGrounded;
        previouslyOnCeiling = onCeiling;
    }

    void FixedUpdate()
    {
        float speed;
        GetInput(out speed);

         Vector3 desiredMove = transform.forward * input.y + transform.right * input.x;

        RaycastHit hitInfo;

        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                   characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        moveDirection.x = desiredMove.x * speed;
        moveDirection.z = desiredMove.z * speed;

        if ((characterController.isGrounded || onCeiling) && !jumping)
        {
            moveDirection.y = Physics.gravity.y;
            if (jump)
            {
                moveDirection.y = jumpSpeed;
                jump = false;
                jumping = true;

            }
        }
        else
        {
            moveDirection += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        }


        characterController.Move(moveDirection * Time.fixedDeltaTime);
    }

    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //bool waswalking =isWalking;

#if !MOBILE_INPUT
        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        isWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
        // set the desired speed to be walking or running
        speed = isWalking ? walkSpeed : runSpeed;
       input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        //if (isWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
        //{
            //StopAllCoroutines();
            //StartCoroutine(!isWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        //}
    }

}