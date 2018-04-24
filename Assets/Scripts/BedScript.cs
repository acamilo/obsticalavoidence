using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedScript : MonoBehaviour {
    GameObject player;
    AudioSource audio;
    public AudioClip FinishedMusic;

	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player");
        audio = GetComponent<AudioSource>();
        exit = exitScript();
    }

    // Update is called once per frame

    bool coStarted = false;
	void FixedUpdate () {
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distToPlayer<2f)
        {
            if (coStarted == false)
            {
                audio.PlayOneShot(FinishedMusic);
                player.GetComponent<PlayerController>().goToSleep();
                StartCoroutine(exit);
                coStarted = true;
            }

        }
    }


    private IEnumerator exit;
    IEnumerator exitScript()
    {

        
        yield return new WaitForSeconds(10f);
        Debug.Log("Quitting");
        Application.Quit();
        yield break;
    }
}

