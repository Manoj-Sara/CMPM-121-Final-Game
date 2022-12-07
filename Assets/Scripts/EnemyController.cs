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
    public float baseChaseSpeed = 25f;
    public float baseSearchSpeed = 5f;
    public float detectDist = 20f;
    public int chaseLevel = 20;

    private float dist;
    // timers
    public bool chasing = false;
    private float chaseDuration = 10f;
    private float chaseTimer = 0f;

    public bool chaseCooldown = false;
    private float chaseCooldownDuration = 5f;
    private float chaseCooldownTimer = 0f;

    private Vector3 dir;
    private bool gameOver = false;

    // obj vars
    public Transform player;
    public Collider rigidCollider;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dist = Vector3.Distance(transform.position, player.position);
        dir = (player.position - transform.position).normalized;
        Vector3 pos = ProjectPointOnPlane(transform.up, transform.position, player.position);
        transform.LookAt(pos, transform.up);

        if (dist <= detectDist*(1+chaseLevel*0.2) && !chaseCooldown) {
            ChasePlayer();
        }
        // else {
        //     SearchPlayer();
        // }
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
            float chaseSpeed = baseChaseSpeed*(1f+(chaseLevel*0.1f));
            rb.velocity = transform.forward*chaseSpeed;
            /*transform.position = Vector3.MoveTowards(
                transform.position, player.position, Time.deltaTime * chaseSpeed
            );*/
            
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= chaseDuration) {
                //print("done chasing");
                chaseCooldown = true;
                chasing = false;
                chaseTimer = 0f;
                chaseCooldown = true;
            }
        }
    }

    // private void SearchPlayer() {
    //     print("searching for player");
    //     float searchSpeed = baseSearchSpeed*(1f+(chaseLevel*0.1f));
    //     rb.velocity = transform.forward*searchSpeed;
    // }
}
