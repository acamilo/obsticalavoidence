using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {


    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Vector3 zoomInOffset;
    public bool zoom = false;
    private bool hidden = false;
    RaycastHit[] hits;


    private void FixedUpdate()
    {
        Vector3 desiredPos = Vector3.zero;
        transform.LookAt(target);
        if (zoom == true)
            desiredPos = target.position + zoomInOffset;
        else
            desiredPos = target.position + offset;


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key hit!, Exiting!");
            Application.Quit();
        }

        Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothPos;

        if (zoom == true && hidden == false && transform.position == desiredPos)
        {
            // we dont want to hide objects beyond the player
            float distToPlayer = Vector3.Distance(transform.position, target.transform.position);

            // Make a list of objects in front of camera
            hits = Physics.RaycastAll(transform.position, target.position-transform.position, distToPlayer);

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                Renderer rend = hit.transform.GetComponent<Renderer>();
                if (hit.transform.tag == "hide")
                {
                    hit.transform.gameObject.SetActive(false);
                }
                    
            }
            hidden = true;
        }

        if (zoom == false && hidden == true)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                Renderer rend = hit.transform.GetComponent<Renderer>();
                if (hit.transform.tag == "hide")
                {
                    hit.transform.gameObject.SetActive(true);
                }

            }
            hidden = false;
        }

    }
}
