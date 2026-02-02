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
    public Ending ending;


    private AudioSource speakerAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(RunGame());

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
        //Press buttons to activate different parts of the game.
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            StartCoroutine(RunGame()); //Runs the whole game sequence
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            blackoutEvent.Run(); //Runs just the blackout event
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            ending.Run(); //Runs just the end of the game.
        }
    }
}
