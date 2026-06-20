using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Settings")]
    public GameObject SettingsMenu;
    public GameObject ControlsMenu;

    [Header("Window")]
    public GameObject Window;
    private Color baseEmissionColor;
    private Material mat;

    [Header("Flicker Settings")]
    public float minIntensity = 0.05f;
    public float maxIntensity = 0.15f;
    public float flickerSpeed = 0.5f;

    [Header("Checkpoint")]
    public SpriteRenderer darkness;
    public List<float> darknessVals;
    public GameObject ContinueButton;
    public GameObject EmptyContinue;

    void Start()
    {
        Renderer rend = Window.GetComponent<MeshRenderer>();
        mat = rend.material;
        mat.EnableKeyword("_EMISSION");
        baseEmissionColor = mat.GetColor("_EmissionColor");
        StartCoroutine(Flicker());

        //SetCheckpointVals();  // sets checkpoint stuff (darkness, continue button)   ==========> Removed for quick demo since both gone, add back for game!

    }

    public void Update()
    {
        // testing, can delete later
        if (Input.GetKeyDown(KeyCode.P)) { StartCoroutine(VisDark()); }  // press P to see how darkness values look
    }

    public void SetCheckpointVals()
    {
        if (PlayerPrefs.GetInt("CurrentCheckpoint") > 0) { SetDarkness(); }  // sets darkness based on current checkpoint
        SetContinue();
    }
    public IEnumerator VisDark()
    {
        // just testing to see how darkness looks
        PlayerPrefs.SetInt("CurrentCheckpoint", 1);
        SetDarkness();
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetInt("CurrentCheckpoint", 2);
        SetDarkness();
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetInt("CurrentCheckpoint", 3);
        SetDarkness();
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetInt("CurrentCheckpoint", 4);
        SetDarkness();
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetInt("CurrentCheckpoint", 5);
        SetDarkness();
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetInt("CurrentCheckpoint", 6);
        SetDarkness();
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetInt("CurrentCheckpoint", 7);
        SetDarkness();
    }
    public IEnumerator Flicker()
    {
        // Flicker light in window of Diving Bell (idk if we'll use it but it's here lol)

        float targetIntensity = maxIntensity;
        float currentIntensity = minIntensity;

        while (true)
        {
            // Pick a new target brightness
            targetIntensity = Random.Range(minIntensity, maxIntensity);

            float startIntensity = currentIntensity;
            float t = 0f;

            // Smoothly move toward target
            while (t < Random.Range(1f, 2f))
            {
                t += Time.deltaTime * flickerSpeed;
                currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);

                mat.SetColor("_EmissionColor", baseEmissionColor * currentIntensity);
                yield return null;
            }

            t = 0f;
            // Smoothly move toward target
            while (t < 1f)
            {
                t += Time.deltaTime * flickerSpeed;
                currentIntensity = Mathf.Lerp(targetIntensity, startIntensity, t);

                mat.SetColor("_EmissionColor", baseEmissionColor * currentIntensity);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }

    public void SetDarkness()
    {
        Color DarknessColor = darkness.color;
        DarknessColor.b = darknessVals[PlayerPrefs.GetInt("CurrentCheckpoint", 1) - 1];
        darkness.color = DarknessColor;
    }

    public void SetContinue()
    {
        if (PlayerPrefs.GetInt("CurrentCheckpoint") >= 1)  // if have hit a checkpoint
        {
            ContinueButton.SetActive(true);
            EmptyContinue.SetActive(false);
        }
        else 
        {
            EmptyContinue.SetActive(true);
            ContinueButton.SetActive(false);
        }
    }

    public void ContinueGame()
    {
        // Continues game from current checkpoint game

        SceneManager.LoadScene("FastDemo"); // Switch to main game's scene name
    }

    public void StartGame()
    {
        // starts game by loading the specified game scene

        PlayerPrefs.SetInt("CurrentCheckpoint", 0);  // resets checkpoints
        SceneManager.LoadScene("FastDemo"); // Switch to main game's scene name
    }

    public void OpenSettings(bool val)
    {
        // turns settings on or off based on <val>

        SettingsMenu.SetActive(val);
    }

    public void OpenControls(bool val)
    {
        // turns settings on or off based on <val>

        ControlsMenu.SetActive(val);
    }

    public void QuitGame()
    {
        // Quits the application (only works on Build, not editor), and deletes playerprefs.

        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
