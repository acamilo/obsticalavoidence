using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    enum States { walking = 0, talking, sleeping };

    States state = States.walking;
    public float moveSpeed;
    private Rigidbody rBody;

    private Vector3 moveInput;
    private Vector3 moveVel;
    private Camera mainCam;

    private GameObject engagednpc;
    private CameraFollowPlayer camscript;

    private AudioSource audio;
    public AudioClip walking;

    public bool isEngaged
    {
        get
        {
            return state == States.talking;
        }
    }

    public void engagePlayer(GameObject npc)
    {
        state = States.talking;
        engagednpc = npc;
    }

    // Use this for initialization

    GameObject bed;
    void Start () {
        rBody = GetComponent<Rigidbody>();
        mainCam = FindObjectOfType<Camera>();
        camscript = Camera.main.GetComponent<CameraFollowPlayer>();
        audio = GetComponent<AudioSource>();
        bed = GameObject.Find("playerbed");
    }
	
	// Update is called once per frame

	void Update () {
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0f, Input.GetAxisRaw("Vertical"));
        moveVel = moveInput * moveSpeed;

        if(state == States.sleeping && inBed==true)
        {

            transform.position = sleepPosition;
            transform.rotation = sleepRotation;// Quaternion.Lerp(standRotation, sleepRotation, 0.2f);
        }



    }

    public void goToSleep()
    {
        sleepFlag = true;
    }
    Vector3 sleepPosition,standPosition;
    Quaternion sleepRotation, standRotation;
    bool sleepFlag = false;
    bool inBed = false;
    void FixedUpdate()
    {
        switch (state)
        {
            case States.walking:
                if (rBody.velocity.magnitude > 0.1) { 
                    if (audio.isPlaying == false) audio.PlayOneShot(walking);}
                    else audio.Stop();
                camscript.zoom = false;
                rBody.velocity = moveVel;
                if (rBody.velocity != Vector3.zero)
                {
                    Quaternion desiredLook = Quaternion.LookRotation(rBody.velocity.normalized);
                    Quaternion look = Quaternion.Lerp(transform.rotation, desiredLook, 0.125f);
                    transform.rotation = look;
                }
                if (sleepFlag == true)
                {
                    state = States.sleeping;
                }
                break;

            case States.talking:
                audio.Stop();
                camscript.zoom = true;
                transform.LookAt(engagednpc.transform);
                if (engagednpc.GetComponent<NPCController>().isPlaying) break;
                state = States.walking;
                break;
            case States.sleeping:
                if (inBed == false)
                {
                    standPosition = transform.position;
                    standRotation = transform.rotation;
                    sleepPosition = bed.transform.position;
                    sleepPosition.y = sleepPosition.y + 0.3f;
                    sleepRotation = Quaternion.Euler(270f, 0f, 0f);
                    inBed = true;
                }
                break;
                
        }



    }


}
