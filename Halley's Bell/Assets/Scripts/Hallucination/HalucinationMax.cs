using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalucinationMax : MonoBehaviour
{
    //Three types of halucinations: Ones that can happen at any time, ones that are dependent on view angle, and ones that require a specific trigger (which will be handled in their own script?)


    [Header("Halucination Controls")]
    //public float startingFreq;
    //public float endingFreq;
    public float duration;
    public AnimationCurve intensityCurve; //curve for determinine intensity over time (x), x must be 0-1

    [Header("Debug Settings")]
    public bool DEBUGMODE;

    private float currFreq;

    void Start()
    {
        if (DEBUGMODE)
        {
            Run();
        }
    }

    public void Run()
    {
        StartCoroutine(Halucinate());
    }

    IEnumerator Halucinate()
    {
        bool halucination1Played = false;

        float elapsed = 0f; //time for whole halucination event
        float timer = 0f; //time for intervals between halucinations

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            //All this assumes the animation curves' x is 0-1
            float normalizedTime = Mathf.Clamp01(elapsed / duration); //find the time normalized from 0-1 (originaly 0 - duration)
            float frequency = intensityCurve.Evaluate(normalizedTime); //get y from x
            float interval = 1f / frequency; //frequency to interval in seconds

            timer += Time.deltaTime; //update interval timer
            if (timer >= interval) //interval will go down as timer goes up. When they pass, a halucination will happen. This will happen more frequently as time goes on
            {
                timer -= interval; //Reset timer
                //CALL SMALL HALUCINATION
                Debug.Log("small halucination occured");
            }

            //Can play larger halucinations at specific times:
            if (normalizedTime > 0.5 && !halucination1Played)
            {
                //CALL LARGER HALUCINATION
                halucination1Played = true;
            }

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
