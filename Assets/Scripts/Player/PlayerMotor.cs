using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    //Animation
    private Animator animator;

    //Movement
    public float LANE_DISTANCE = 2.0f;
    public float TURN_SPEED = 0.05f;
    public float speed = 6.0f;
    public float speedMultip = 0.000f;
    public float sideSpeed = 12.0f;
    public float gravity = 9.0f;
    public float jumpForce = 5.0f; 
    private int lane = 2;
    private float verticalVelocity = 0;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        speed += speedMultip;
        //Reading inputs
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            SwitchLane(false);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            SwitchLane(true);

        //Calculate where it should be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (lane == 1)
            targetPosition += Vector3.left * LANE_DISTANCE;
        else if (lane == 3)
            targetPosition += Vector3.right * LANE_DISTANCE;

        //Calculate move
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * sideSpeed;

        bool isGrounded = IsGrounded();
            animator.SetBool("Grounded", isGrounded);

        //Calculate Y
        if (isGrounded)
        {
            verticalVelocity = -0.1f;

            if (Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
        }
        else {
            verticalVelocity -= (gravity * Time.deltaTime);

            //Fast falling
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.DownArrow)){
                verticalVelocity = -jumpForce;
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        //Move
        controller.Move(moveVector * Time.deltaTime);

        //Rotate where going
        Vector3 direction = controller.velocity;
        if (direction != Vector3.zero) {
            direction.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, direction, TURN_SPEED);
        }

    }

    private void SwitchLane(bool goingRight)
    {
        lane += goingRight ? 1 : -1;
        lane = Mathf.Clamp(lane, 1, 3);
    }

    private bool IsGrounded() { 
        Ray groundRay = new Ray( new Vector3(
                controller.bounds.center.x,
                (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f,
                controller.bounds.center.z),
            Vector3.down);
        Debug.DrawLine(groundRay.origin, groundRay.direction, Color.cyan, 10.0f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }
}
