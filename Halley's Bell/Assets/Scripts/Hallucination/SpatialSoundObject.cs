using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialSoundObject : MonoBehaviour
{
    //This class on Run() plays it a random sound at random times. To be used in combination with SpatialSounder to play lots of sounds around the player
    private float waitTime;
    public AudioSource _as;
    public AudioClip[] audioClipArray;
    private float targetTime = 0f;

    void Awake()
    {
        _as = GetComponent<AudioSource>();
    }

    void Start()
    {
        waitTime = Random.Range(0f, 5f);
    }

    public void Run(float volume, float targetTime)
    {
        this.targetTime = targetTime;
        Debug.Log("Running");
        _as.volume = volume;
        StartCoroutine(Player());
        //StartCoroutine(Waiter());
    }

    public void UpdateVolume(float volume)
    {
        _as.volume = volume;
    }

    IEnumerator Waiter()
    {
        yield return new WaitUntil(() =>
        {
            targetTime -= Time.deltaTime;
            return targetTime <= 0.0f;
        });
        Debug.Log("finished");
        Destroy(gameObject);
    }

    // Update is called once per frame
    IEnumerator Player()
    {
        float speed = Random.Range(4f, 5f);
        bool mid = false;
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < 7; i++)
        {
            _as.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
            waitTime = Random.Range(speed, speed + 3);
            _as.PlayOneShot(_as.clip);
            //Debug.Log(_as.clip.length);
            //yield return new WaitForSeconds(_as.clip.length);
            yield return new WaitForSeconds(waitTime);

            if (speed > 1.1f && mid == false)
            {
                speed -= 1.0f;
            }
            else
            {
                mid = true;
            }
            if (mid == true)
            {
                speed += 3.0f;
            }
        }

    }
}
