using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleScript : MonoBehaviour {

    string[] scoreline = { "Faux Pas", "Indiscretions", "Rude Remarks", "Awkward Answers" };
    public GameObject up, down, left, right, wrong, correct, tagbox,scorebox,repeat;
    public Text scoretext,scoretag;
    private void Start()
    {
        displayEmptyBubble();
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }

    public void showScore(int score)
    {
        string line = scoreline[(int)UnityEngine.Random.Range(0, scoreline.Length - 1)];
        displayEmptyBubble();
        scoretext.text = score.ToString();
        scoretag.text = line;
        tagbox.SetActive(true);
        scorebox.SetActive(true);

    }

    public void displayEmptyBubble()
    {
        up.SetActive(false);
        down.SetActive(false);
        left.SetActive(false);
        right.SetActive(false);
        wrong.SetActive(false);
        correct.SetActive(false);
        tagbox.SetActive(false);
        scorebox.SetActive(false);
        repeat.SetActive(false);
    }

    public void displayRepeat()
    {
        displayEmptyBubble();
        repeat.SetActive(true);
    }

    public void displayWrong()
    {
        
        displayEmptyBubble();
        wrong.SetActive(true);
    }

    public void displayCorrect()
    {
        displayEmptyBubble();
        correct.SetActive(true);
    }

    public void displayArrow(int s)
    {
        displayEmptyBubble();
        switch (s)
        {
            case 0:
                up.SetActive(true);
                break;
            case 1:
                down.SetActive(true);
                break;
            case 2:
                left.SetActive(true);
                break;
            case 3:
                right.SetActive(true);
                break;
        }
    }
}
