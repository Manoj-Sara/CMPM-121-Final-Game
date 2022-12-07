using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CODE FROM: https://www.firemind-academy.com/p/how-to-shoot-lasers-in-unity
public class RayGun : MonoBehaviour
{
    public float shootRate;
    private Camera cam;
    private float m_shootRateTimeStamp;

    public GameObject m_shotPrefab;

    RaycastHit hit;
    float range = 1000.0f;

    void Start() {
        cam = GameObject.Find("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            if (Time.time > m_shootRateTimeStamp)
            {
                shootRay();
                m_shootRateTimeStamp = Time.time + shootRate;
            }
        }

    }

    void shootRay()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, range))
        {
            GameObject laser = GameObject.Instantiate(m_shotPrefab, transform.position, transform.rotation) as GameObject;
            // laser.GetComponent<ShotBehavior>().setTarget(hit.point);
            GameObject.Destroy(laser, 2f);


        }

    }



}