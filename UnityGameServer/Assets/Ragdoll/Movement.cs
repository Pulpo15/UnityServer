using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    public float speed;
    public float sprintSpeed = 1.5f;
    public float gravity = -9.81f;
    public float strafeSpeed;
    public float jumpForce;
    public Rigidbody pelvis;
    public bool isGrounded;
    public Animator animator;
    public CharacterController controller;
    private float yVelocity = 0;

    private void Start() {
        pelvis = GetComponent<Rigidbody>();
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        speed *= Time.fixedDeltaTime;
        sprintSpeed *= Time.fixedDeltaTime;

    }

    private void FixedUpdate() {
        Vector2 _inputDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
            _inputDirection.x += 1;
        if (Input.GetKey(KeyCode.S))
            _inputDirection.x -= 1;
        if (Input.GetKey(KeyCode.D))
            _inputDirection.y -= 1;
        if (Input.GetKey(KeyCode.A))
            _inputDirection.y += 1;
        Move(_inputDirection);
    }

    private void Move(Vector2 _inputDirection) {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= speed;

        if (controller.isGrounded) {
            yVelocity = 0f;
            if (Input.GetKey(KeyCode.Space)) {
                yVelocity = jumpForce;
            }
        }
        yVelocity += gravity;
        _moveDirection.y = yVelocity;

        controller.Move(_moveDirection);
    }

    //private void FixedUpdate() {
    //    if(pelvis.velocity.magnitude >= 10f) {
    //        speed = 0f;
    //        sprintSpeed = 0f;
    //    } else {
    //        speed = 150f;
    //        sprintSpeed = 1.5f;
    //    }


    //    if (Input.GetKey(KeyCode.W)) {
    //        pelvis.drag = 500f;
    //        pelvis.angularDrag = 500f;
    //        pelvis.drag = 0f;
    //        pelvis.angularDrag = 0.1f;
    //        if (!animator.GetBool("Walk"))
    //            animator.SetBool("Walk", true);
    //        if (Input.GetKey(KeyCode.LeftShift))
    //            pelvis.AddForce(pelvis.transform.right * speed * sprintSpeed);
    //        else
    //            pelvis.AddForce(pelvis.transform.right * speed);
    //    }
    //    if (Input.GetKey(KeyCode.A)) {
    //        pelvis.drag = 500f;
    //        pelvis.angularDrag = 500f;
    //        pelvis.drag = 0f;
    //        pelvis.angularDrag = 0.1f;
    //        if (!animator.GetBool("Walk"))
    //            animator.SetBool("Walk", true);
    //        pelvis.AddForce(pelvis.transform.forward * strafeSpeed);
    //    }
    //    if (Input.GetKey(KeyCode.S)) {
    //        pelvis.drag = 500f;
    //        pelvis.angularDrag = 500f;
    //        pelvis.drag = 0f;
    //        pelvis.angularDrag = 0.1f;
    //        if (!animator.GetBool("Walk"))
    //            animator.SetBool("Walk", true);
    //        pelvis.AddForce(-pelvis.transform.right* speed);
    //    }
    //    if (Input.GetKey(KeyCode.D)) {
    //        pelvis.drag = 500f;
    //        pelvis.angularDrag = 500f;
    //        pelvis.drag = 0f;
    //        pelvis.angularDrag = 0.1f;
    //        if (!animator.GetBool("Walk"))
    //            animator.SetBool("Walk", true);
    //        pelvis.AddForce(-pelvis.transform.forward * strafeSpeed);
    //    }

    //    if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)) {
    //        if (animator.GetBool("Walk"))
    //            animator.SetBool("Walk", false);
    //        if (isGrounded) {
    //            pelvis.drag = 500f;
    //            pelvis.angularDrag = 500f;
    //        } else {
    //            pelvis.drag = 0f;
    //            pelvis.angularDrag = 0f;
    //        }

    //    }

    //    if (Input.GetAxis("Jump") > 0) {
    //        if (isGrounded) {
    //            pelvis.drag = 0f;
    //            pelvis.angularDrag = 0f;
    //            pelvis.AddForce(new Vector3(0, jumpForce, 0));
    //            isGrounded = false;
    //        }

    //    }
    //}
}
