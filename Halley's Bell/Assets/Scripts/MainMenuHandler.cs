using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    private Material mat;
    public GameObject Window;
    public GameObject SettingsMenu;
    private Color baseEmissionColor;

    [Header("Flicker Settings")]
    public float minIntensity = 0.05f;
    public float maxIntensity = 0.15f;
    public float flickerSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Renderer rend = Window.GetComponent<MeshRenderer>();
        mat = rend.material;
        mat.EnableKeyword("_EMISSION");
        baseEmissionColor = mat.GetColor("_EmissionColor");
        StartCoroutine(Flicker());
    }

    // Update is called once per frame
    void Update()
    {

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
        SceneManager.LoadScene("tempScene"); // Switch to main game's scene name
    }

    public void OpenSettings(bool on)
    {
        SettingsMenu.SetActive(on);
    }
}
