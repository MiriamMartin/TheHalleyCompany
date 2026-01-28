using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public GameObject speaker;
    public Gauge gauge;
    public Blinker blinker;
    public SonarButton sonarButton;
    public AudioSource ambientAudioSource;
    public BlackoutEvent blackoutEvent;


    private AudioSource speakerAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunGame());

    }

    // The coroutine function
    IEnumerator RunGame()
    {
        ambientAudioSource.Play();
        speakerAudioSource = speaker.GetComponent<AudioSource>();
        speakerAudioSource.Play();
        yield return new WaitForSeconds(10);
        gauge.Run();
        yield return new WaitForSeconds(10);
        blinker.Run();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            blackoutEvent.Run();
        }
    }
}
