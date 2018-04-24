using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour {
    enum npcStates {look=0,follow,talk, playing, done};


    npcStates state = npcStates.look;
    public GameObject player;
    public float AwareDistance = 3f;
    public float LookDistance = 15f;
    public float LookAngle = 45f;
    public float ConversationDistance = 1.5f;

    public GameObject scoreObject;
    private ScoreKeeper sk;
    private Rigidbody rb;

    bool doneCantSeePlayer = false;



    private NavMeshAgent navmesh;
    public Canvas bubble;
    private CameraFollowPlayer camscript;
    private PlayerController pc;
    private SpeechBubbleScript bubblectl;

    AudioSource audio;
    public AudioClip walking;
    public AudioClip derp;
    public AudioClip noticed;
    public AudioClip[] talking;

    public bool isPlaying { get { return state == npcStates.playing;  } }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, AwareDistance);
    }
        // Use this for initialization
        void Start () {
        player = GameObject.FindWithTag("Player");
        scoreObject = GameObject.Find("ScoreObject");
        navmesh = GetComponent<NavMeshAgent>();
        bubblectl = bubble.GetComponent<SpeechBubbleScript>();
        bubble.gameObject.SetActive(false);
        camscript = Camera.main.GetComponent<CameraFollowPlayer>();
        pc = player.GetComponent<PlayerController>();
        audio = GetComponent<AudioSource>();
        
        sk = scoreObject.GetComponent<ScoreKeeper>();
        rb = GetComponent<Rigidbody>();
        
        simonGameco = simonGame();
        lookco = lookAround();
        StartCoroutine(lookco);

    }
	
	// Update is called once per frame
	void Update () {


        if (state == npcStates.playing && simonRunning == false)
        {
            StartCoroutine(simonGameco);
            
        }

        if (state == npcStates.look || (state == npcStates.done && doneCantSeePlayer==true))
        {
            float lerpanglef = Mathf.LerpAngle(transform.eulerAngles.y, newAngle, 0.125f);
            Vector3 lerpangle = new Vector3(transform.eulerAngles.x, lerpanglef, transform.eulerAngles.z);
            transform.eulerAngles = lerpangle;
        }

        if (simonRunning == true)
        {
             up = Input.GetKeyDown(KeyCode.UpArrow);
             down = Input.GetKeyDown(KeyCode.DownArrow);
             left = Input.GetKeyDown(KeyCode.LeftArrow);
             right = Input.GetKeyDown(KeyCode.RightArrow);
        }


        if (Input.GetKeyDown(KeyCode.Space) && state == npcStates.playing)
        {
            //Debug.LogWarning(state);
            state = npcStates.done;
        }
        
    }

    // simon vars
    public int SymbolLengths = 4;
    public float symbolDisplayTime=3f;
    public float symbolBetweenTime = 0.5f;
    public float gameStartDelayTime = 3f;
    public float postShowDelayTime = 1f;
    public float scoreDisplayTime = 2f;


    public float looktime = 5f;
    public float looktimefuzz = 1.5f;

    float oldAngle=0f, newAngle=0f;
    IEnumerator lookAround()
    {
        //Debug.Log("Rotation Coroutine Started");
        yield return new WaitForSeconds(2); //*UnityEngine.Random.Range(0.0f, looktimefuzz)
        for (;;)
        {
            //Debug.Log("Calculating new rotation");

            oldAngle = transform.eulerAngles.y;
            newAngle = UnityEngine.Random.Range(0f, 360f);

            

            yield return new WaitForSeconds(looktime);
        }
    }

    List<int> symbols;
    int symbolIndex;
    private IEnumerator simonGameco,lookco;
    private bool simonRunning = false;
    private bool simonDone = false;
    private bool up, down, left, right;

    IEnumerator simonGame()
    {
        //Debug.Log("Simon Started");
        simonRunning = true;
        List<int> symbols = new List<int>();
        int symbolIndex=0;

        // populate our list of symbols.
        for (int i = 0; i < SymbolLengths; i++)
        {
            symbols.Add((int)UnityEngine.Random.Range(0, 3));
        }

        yield return new WaitForSeconds(gameStartDelayTime);
        // show our symbols
        foreach ( int symbol in symbols)
        {
            //Debug.Log("Showing symbol "+symbol);
            bubblectl.displayArrow(symbol);
            playSpeechNoise();
            yield return new WaitForSeconds(symbolDisplayTime);
            bubblectl.displayEmptyBubble();
            yield return new WaitForSeconds(symbolBetweenTime);
        }

        yield return new WaitForSeconds(postShowDelayTime);
        // Now the user repeats them back
        bubblectl.displayRepeat();
        int wrong=0, correct = 0;
        foreach (int symbol in symbols)
        {
            //Debug.Log("Testing symbol " + symbol);


            // Wait for a key
            while (!(up | down | left | right))  yield return null;

            //Debug.Log("Got Key");

            bool success = false;
            switch (symbol)
            {
                case 0:
                    if (up) success = true;
                    break;
                case 1:
                    if (down) success = true;
                    break;
                case 2:
                    if (left) success = true;
                    break;
                case 3:
                    if (right) success = true;
                    break;
            }
            bubblectl.displayEmptyBubble();
            if (success)
            {
                correct++;
                playSpeechNoise();
                bubblectl.displayCorrect();
            }
            else
            {
                wrong++;
                playDerpNoise();
                bubblectl.displayWrong();
            }

            yield return new WaitForSeconds(symbolBetweenTime);
            bubblectl.displayEmptyBubble();
            bubblectl.displayRepeat();
            yield return new WaitForSeconds(symbolBetweenTime);
        }

        // score
        bubblectl.showScore(wrong);
        yield return new WaitForSeconds(scoreDisplayTime);
        bubblectl.displayEmptyBubble();

        sk.addToScore(wrong);
        simonRunning = false;
        simonDone = true;
        yield break;
    }

    void FixedUpdate()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

        Vector3 toVector = player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, toVector);
        
        switch (state)
        {
            // NPC is looking for player. Once found switches to follow state.
            case npcStates.look:
                

                // 
                if (distToPlayer < AwareDistance) { 
                    playAwarenessNoise();
                    state = npcStates.follow;
                }


                if (angleToPlayer < LookAngle)
                    if (distToPlayer < LookDistance) {
                        if (canSeePlayer())
                            playAwarenessNoise();
                            state = npcStates.follow;
                         }


                break;

            // NPC is following player
            case npcStates.follow:
                if (audio.isPlaying == false) audio.PlayOneShot(walking);

                navmesh.enabled = true;
                navmesh.SetDestination(player.transform.position);
                if (distToPlayer < ConversationDistance)
                {
                    navmesh.enabled = false;
                    state = npcStates.talk;
                }

                break;
            case npcStates.talk:
                audio.Stop();
                

                // We need to check if the player is engaged and wait.
                if (pc.isEngaged) break;

                
                bubble.gameObject.SetActive(true);
                pc.engagePlayer(this.gameObject);
                state = npcStates.playing;
                break;
            case npcStates.playing:
                
                if (simonDone == true ) state = npcStates.done;
                break;

            case npcStates.done:
                
                bubble.gameObject.SetActive(false);
                if (canSeePlayer())
                {
                    transform.LookAt(player.transform);
                    doneCantSeePlayer = false;
                }
                else doneCantSeePlayer = true;
                    break;
        }
        
    }

    void playSpeechNoise()
    {
        audio.PlayOneShot(talking[(int)UnityEngine.Random.Range(0, talking.Length - 1)]);
    }

    void playDerpNoise()
    {
        audio.PlayOneShot(derp);
    }

    void playAwarenessNoise()
    {
        audio.PlayOneShot(noticed);
    }

    bool canSeePlayer() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position- transform.position, out hit, 100.0F))
        {
            //Debug.Log("Hit " + hit.collider.gameObject.name);
            //if there something in our front, check if it's the player
            if (hit.collider.gameObject == player) { 
                //Debug.Log("Hit Player");
                return true;
            }
        }
        return false;
    }

    void setColor(Color c)
    {

        GetComponent<Renderer>().material.color = c;
    }
}
