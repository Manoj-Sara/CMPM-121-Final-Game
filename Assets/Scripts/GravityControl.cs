using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    // lets object rotate around GravityOrbit obj
    public GravityOrbit gravityObj;
    private Rigidbody rb;

    private float rotationSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // if there is a planet to orbit
        if (gravityObj) {
            // gravity up is the upwards angle of the gravity object
            Vector3 gravityUp = Vector3.zero;

            // get the obj pos compared to the gravity obj pos
            gravityUp = (transform.position-gravityObj.transform.position).normalized;

            // the players current up angle
            Vector3 localUp = transform.up;

            // player rotation angle: get rotation from curr up to grav up; multiply by curr rotation (?)
            Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp)*transform.rotation;

            rb.rotation = targetRotation;
            
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            // set players curr up as a lerp; interpolation from curr Up to new Up
            rb.AddForce((-gravityUp*gravityObj.gravity)*rb.mass, ForceMode.Acceleration);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
