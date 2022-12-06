using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public Transform cam;
    private Rigidbody rb;
    public float speed;
    public float jumpForce = 20;
    public float gravity = -9.18f;
    private float airVel;
    public bool grounded;
    private float distToGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    private Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point) {
        planeNormal.Normalize();
        float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
        return (point + planeNormal * distance);
    }

    void FixedUpdate()
    {
        grounded = isGrounded();

        // get axis' of movement
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(horiz, 0f, vert).normalized;
        dir = transform.TransformDirection(dir);
        
        print(rb.velocity.y);
        // move the dir
        if (Mathf.Abs(dir.magnitude) >= 0.1f) {
            FaceCamRelativeDir();
            //rb.velocity = dir*speed;
            rb.velocity = new Vector3(dir.x*speed, rb.velocity.y + dir.y*speed, dir.z*speed);
        }
    }

    void FaceCamRelativeDir() {
        Vector3 frontDir = cam.forward*100;
        frontDir = transform.position + frontDir;
        frontDir = ProjectPointOnPlane(transform.up, transform.position, frontDir);
        transform.LookAt(frontDir, transform.up);
    }

    bool isGrounded() {
        return Physics.Raycast(transform.position, -transform.up, distToGround+0.1f);
    }

    /*void OnCollisionEnter(Collision obj) {
        if (obj.gameObject.tag == "Ground") {
            print("grounded");
            grounded = true;
        }
    }

    void OnCollisionExit(Collision obj) {
        if (obj.gameObject.tag == "Ground") {
            print("in the air");
            grounded = false;
        }
    }*/


    void OnJump() {
        if (grounded) {
            print("JUMP");
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
}