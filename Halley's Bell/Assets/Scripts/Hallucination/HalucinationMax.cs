using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalucinationMax : MonoBehaviour
{
    //Three types of halucinations: Ones that can happen at any time, ones that are dependent on view angle, and ones that require a specific trigger (which will be handled in their own script?)

    public AudioSource keithHallucination;


    [Header("Halucination Controls")]
    //public float startingFreq;
    //public float endingFreq;
    public float duration;
    public AnimationCurve intensityCurve; //curve for determinine intensity over time (x), x must be 0-1
    [SerializeField] private MonoBehaviour[] hallucinationsInit; //Created because interfaces can't show up in the inspector for some reason... casted later to be the hallucination
    private List<HallucinationInterface> hallucinations;

    [Header("Debug Settings")]
    public bool DEBUGMODE;

    private float currFreq;

    void Start()
    {
        //initiallizing list of different hallucinations
        hallucinations = new List<HallucinationInterface>();

        if (hallucinationsInit.Length > 0)
        {
            foreach (MonoBehaviour hallucination in hallucinationsInit) 
            {
                HallucinationInterface h = hallucination as HallucinationInterface;
                hallucinations.Add(h); //casting
            }
        } else
        {
            Debug.Log("There are no halluciniations in the hallucination list! :(");
        }



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
                SmallHallucination();
                Debug.Log("small halucination occured");
            }

            //Can play larger halucinations at specific times:
            if (normalizedTime > 0.3 && !halucination1Played)
            {
                //CALL LARGER HALUCINATION
                //keithHallucination.Play(); REMOVED FOR FAST-DEMO
                halucination1Played = true;
            }

            yield return null;
        }
    }

    private void SmallHallucination()
    {
        int randomIndex = Random.Range(0, hallucinations.Count);
        hallucinations[randomIndex].Run(1); //REPLACE WITH VOLUME CONTROL
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
