using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    // When game is paused by setting timescale to 0, unscaledDeltaTime will still run
    // (useful for things we want working while paused, like animations / buttons on the menu)
    // can put this on animator, doing * Time.unscaledDeltaTime, or WaitForSecondsRealtime() in a coroutine.

    // Need to manually pause audio still, but can pause ALL audio sources at once with AudioListener.pause = true.
    // For audio we want to play while paused, you can use audiosource.ignoreListenerPause = true;

    // Will need to manually pause input handling (mouse input and keyboard input), so use isPaused to check.

    public static PauseManager Instance;
    private bool isPaused { get; set; }

    [Header("----------------- Pause Key -----------------")]
    public KeyCode pauseInput = KeyCode.Escape;

    [Header("----------------- GameObjects -----------------")]
    public GameObject PauseMenu;
    public GameObject SettingsMenu;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseInput) && !isPaused) // Pauses game 
        {
            Pause();
        }
        else if (Input.GetKeyDown(pauseInput) && isPaused  && !SettingsMenu.activeSelf)  // can't unpause if in settings
        {
            Unpause();
        }
    }

    private void Pause()
    {
        // Pause the game. Invoked by pressing pauseInput at any point in the game.

        setIsPaused(true);
        Time.timeScale = 0f;  // pauses timescale, anything running on time.deltaTime will stop, but time.unscaledTime will be fine.
        AudioListener.pause = true; // pauses all audiosources
        PauseMenu.SetActive(true); // turns on the pause menu overlay
    }

    private void Unpause()
    {
        // Unpause the game. Invoked by pressing pauseInput while in the pause menu,
        // or by hitting the 'continue' button in the pause menu.

        setIsPaused(false);
        Time.timeScale = 1f;  // resumes timescale
        AudioListener.pause = false; // resumes all audiosources
        PauseMenu.SetActive(false); // turns off the pause menu overlay
    }
    public bool getIsPaused()
    {
        return isPaused;
    }

    public void setIsPaused(bool val)
    {
        isPaused = val;
    }

    public void Continue()
    {
        // Unpause the game when 'continue' button clicked

        Unpause();
    }

    public void Settings(bool val)
    {
        // Turn settings menu on / off based on val

        SettingsMenu.SetActive(val);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Switch to main menu, need warning about progress not being saved
    }

    public void Restart()
    {
        SceneManager.LoadScene("tempScene"); // Reloads game scene
    }

}
