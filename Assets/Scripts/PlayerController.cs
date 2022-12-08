using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public Transform cam;
    private Rigidbody rb;
    public float speed;
    public float airSpeed;
    public float jumpForce = 20;
    private float airVel;
    private bool grounded;
    private float distToGround;
    private bool jumping = false;
    private Animator anim;
    private AudioSource walkSfx;
    private AudioSource spinSfx;

    private EnemyController enemyScript;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        anim = transform.Find("robotSphere").GetComponent<Animator>();
        walkSfx = GameObject.Find("WalkSfx").GetComponent<AudioSource>();
        spinSfx = GameObject.Find("SpinSfx").GetComponent<AudioSource>();
        //transform.Find("Collider").
    }

    private Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point) {
        planeNormal.Normalize();
        float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
        return (point + planeNormal * distance);
    }

    void FixedUpdate()
    {
        //grounded = isGrounded();
        if (anim.GetBool("InitAnim")) return;
        // get axis' of movement
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 deltaVel = new Vector3(horiz, 0f, vert).normalized;
        deltaVel = transform.TransformDirection(deltaVel);
        deltaVel *= speed;

        Vector3 vel = rb.velocity;
        deltaVel = deltaVel - vel;
        deltaVel.x = Mathf.Clamp(deltaVel.x, -speed, speed);
        deltaVel.z = Mathf.Clamp(deltaVel.z, -speed, speed);

        // deltaVel.y = 0f;
        Vector3 relativeVelocity = Quaternion.Inverse(transform.rotation) * rb.velocity;
        /*
            anims:
                roll_anim: do if space is pressed and grounded and roll_anim = false
                jump_anim: happens out of exit for roll. Sets jumping to true
                land_anim: happens when grounded, jumping = true, landing = false, and y_vel < 0.1

        */
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("anim_Walk_Loop")) {
            if (!walkSfx.isPlaying) {
                walkSfx.Play();
            }
        }
        /*else if (anim.GetCurrentAnimatorStateInfo(0).IsName("anim_open_GoToRoll")) {
            if (!spinSfx.isPlaying)
                spinSfx.Play();
            if (walkSfx.isPlaying)
                walkSfx.Stop();
        }*/
        else {
            if (walkSfx.isPlaying)
                walkSfx.Stop();
        }
        // know when the player has left into the air
        if (anim.GetBool("Jumping") && !grounded) {
            jumping = true;
        }
        // when the player is landing on the ground
        else if (jumping && grounded && relativeVelocity.y < 0.1f) {
            anim.SetBool("Jumping", false);
            jumping = false;
        }
        // if the player is falling without having jumped
        else if (!anim.GetBool("Jumping") && !grounded && (relativeVelocity.y > 0.1f || relativeVelocity.y < -0.1f)) {
            anim.SetBool("Jumping", true);
            jumping = true;
        }

        if (Mathf.Abs(deltaVel.magnitude) >= 0.1f) {
            FaceCamRelativeDir();
            Vector3 dir = deltaVel.normalized;
            // float targetAngle = Mathf.Atan2(dir.x, dir.z)*Mathf.Rad2Deg + cam.eulerAngles
            anim.SetBool("Walk_Anim", true);
            if (anim.GetBool("Jumping")) {
                if (walkSfx.isPlaying)
                    walkSfx.Stop();
                rb.AddForce(deltaVel.normalized*airSpeed);
            }
            else {
                //if (!walkSfx.isPlaying)
                //    walkSfx.Play();
                rb.AddForce(deltaVel, ForceMode.VelocityChange);
            }
        }
        else {
            if (walkSfx.isPlaying)
                walkSfx.Stop();
            anim.SetBool("Walk_Anim", false);
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

    void OnCollisionEnter(Collision obj) {
        if (obj.gameObject.tag == "Ground") {
            print("grounded");
            grounded = true;
        }

        if (obj.gameObject.tag == "Enemy")
        {
            enemyScript = obj.gameObject.GetComponent<EnemyController>();
            if (!enemyScript.stunned) {
                if (jumping) {
                    enemyScript.Stun();
                }
                else {
                    print("GAME OVER");
                    Lose();
                }
            }
        }

        if (obj.gameObject.tag == "Spaceship") {
            print("YOU WIN!");
            Win();
        }
    }

    void OnCollisionExit(Collision obj) {
        if (obj.gameObject.tag == "Ground") {
            print("in the air");
            grounded = false;
        }
    }


    void OnJump() {
        if (grounded && !anim.GetBool("Roll_Anim") && !anim.GetBool("Jumping")) {
            print("Queue Jump");
			anim.SetBool("Roll_Anim", true);
        }
    }

    public void Jump() {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        // jumping = true;
    }

    public void Lose() {
        // deathSfx.PlayOneShot(deathSfx.clip, eventVolume);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Win() {
        // winSfx.PlayOneShot(winSfx.clip, eventVolume);
    }
    
    public void PlaySpinSfx() {
        spinSfx.Play();
    }
}
