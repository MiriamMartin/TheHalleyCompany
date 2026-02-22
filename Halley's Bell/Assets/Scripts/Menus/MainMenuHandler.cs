using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Settings")]
    public GameObject SettingsMenu;

    [Header("Window")]
    public GameObject Window;
    private Color baseEmissionColor;
    private Material mat;

    [Header("Flicker Settings")]
    public float minIntensity = 0.05f;
    public float maxIntensity = 0.15f;
    public float flickerSpeed = 0.5f;

    void Start()
    {
        Renderer rend = Window.GetComponent<MeshRenderer>();
        mat = rend.material;
        mat.EnableKeyword("_EMISSION");
        baseEmissionColor = mat.GetColor("_EmissionColor");
        StartCoroutine(Flicker());
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

    public void StartGame()
    {
        // starts game by loading the specified game scene

        SceneManager.LoadScene("UnifiedScene"); // Switch to main game's scene name
    }

    public void OpenSettings(bool val)
    {
        // turns settings on or off based on <val>

        SettingsMenu.SetActive(val);
    }

    public void QuitGame()
    {
        // Quits the application (only works on Build, not editor), and deletes playerprefs.

        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
}
