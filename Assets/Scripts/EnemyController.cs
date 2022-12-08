using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
// using Kino;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // stats
    public float chaseSpeed = 10f;
    public float detectDist = 20f;
    private float chatterDist = 30f;
    public bool stunned = false;

    private float dist;
    // timers
    public bool chasing = false;
    private float stunTime = 5f;

    private Vector3 dir;

    // obj vars
    private Transform player;
    public Collider rigidCollider;
    private Rigidbody rb;
    private Animator anim;
    private AudioSource chatterSfx;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody>();
        anim = GameObject.Find("Stylized Astronaut").GetComponent<Animator>();
        chatterSfx = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stunned) {
            rb.velocity = Vector3.zero;
            return;
        }

        dist = Vector3.Distance(transform.position, player.position);
        dir = (player.position - transform.position).normalized;
        Vector3 pos = ProjectPointOnPlane(transform.up, transform.position, player.position);
        transform.LookAt(pos, transform.up);
        
        // peekabooSfx.volume = ((peekabooDist-dist)/dist)+0.3f;
        if (dist <= chatterDist) {
            if (!chatterSfx.isPlaying) {
                chatterSfx.Play();
            }
            chatterSfx.volume = (chatterDist-dist)/dist+0.2f;
        }
        else {
            if (chatterSfx.isPlaying) {
                chatterSfx.Stop();
            }
        }
        if (dist <= detectDist) {
            ChasePlayer();
        }
        else {
            chasing = false;
        }
    }

    private Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point) {
        planeNormal.Normalize();
        float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
        return (point + planeNormal * distance);
    }

    private void ChasePlayer() {
        //print("chasing player");
        if (!chasing)
            chasing = true;
        else {
            if (!anim.GetBool("Running")) {
                anim.SetBool("Running", true);
            }
            rb.velocity = transform.forward*chaseSpeed;
            /*transform.position = Vector3.MoveTowards(
                transform.position, player.position, Time.deltaTime * chaseSpeed
            );*/
        }
    }

    public void Stun() {
        stunned = true;
        print("STUN!");
        StartCoroutine(StunDelay());
    }

    private IEnumerator StunDelay() {
        yield return new WaitForSeconds(stunTime);
        stunned = false;
    }
}
