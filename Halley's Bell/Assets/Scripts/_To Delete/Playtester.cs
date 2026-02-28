using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public BlackoutEvent blackoutEvent;
    public Ending ending;

    public bool DEBUGMODE = false;

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if (DEBUGMODE)
        {
            KeyInputs();  // temp disabled for demo, hoping to delete script tho
        }

    }

    public void KeyInputs()
    {
        //Press buttons to activate different parts of the game.
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
