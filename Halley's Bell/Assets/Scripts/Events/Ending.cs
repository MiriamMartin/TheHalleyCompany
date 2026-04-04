using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{
    private int[,] grid;
    private int[] playerPosition;
    private bool trigger = false;

    [Header("Camera")]
    public CameraMovement cameraMovement;
    public Camera mainCamera;

    [Header("Eyes")]
    public Eyes eyes;

    [Header("Ending Objects")]
    public Material eyeMat;
    public GameObject wall1;
    public GameObject wall2;
    public GameObject wallMisc;
    public GameObject wallLever;

    [Header("Credits")]
    public GameObject Creds;

    [Header("End Music")]
    public AudioSource end1;
    public AudioSource end2;
    public AudioSource endBackground;
    public AudioSource endHit;
    public AudioSource endEyes;
    public AudioSource endCredits;


    // Start is called before the first frame update
    void Start()
    {
        grid = cameraMovement.getGrid();
        eyeMat.DisableKeyword("_EMISSION");
        wall1.SetActive(false);
        wall2.SetActive(false);
        wallMisc.SetActive(false);
        wallLever.SetActive(false);
        end1.volume = 0;
        end2.volume = 0;
        endBackground.volume = 0;
    }

    public void Run()
    {
        StartCoroutine(EndSequence());
    }

    // Update is called once per frame
    void Update()
    {
        if (Depth.Instance.runEnding)  // will only start listening for triggers when Run() called && runEnding
        {
            WatchTrigger();
        }
    }

    public void WatchTrigger()
    {
        playerPosition = cameraMovement.getPlayerPosition();

        if (grid[playerPosition[0], playerPosition[1]] == 2)
        {
            trigger = true;
        }
        else
        {
            trigger = false;
        }
    }

    IEnumerator EndSequence()
    {
        yield return new WaitUntil(() => trigger == true);
        cameraMovement.setGridTile(grid, 2, 5, 0);
        wall1.SetActive(true);

        yield return new WaitUntil(() => trigger == false);
        yield return new WaitUntil(() => cameraMovement.getPlayerPosition()[0] == 1 && cameraMovement.getPlayerPosition()[1] == 5 && cameraMovement.GetDirection() == 1);
        wallMisc.SetActive(true);
        endHit.Play();
        end1.Play();
        StartCoroutine(AdjustVolume(end1, 1, 0.5f));
        StartCoroutine(AdjustVolume(endBackground, 1, 0.5f));
        yield return new WaitForSeconds(12f);

        yield return new WaitUntil(() => (trigger == true) && (cameraMovement.GetDirection() == 2));
        
        yield return new WaitForSeconds(0.5f);
        wall2.SetActive(true);

        yield return new WaitUntil(() => cameraMovement.GetDirection() == 0);
        wallLever.SetActive(true);
        endHit.Play();
        end2.Play();
        StartCoroutine(AdjustVolume(end2, 1, 0.5f));
        cameraMovement.setGridTile(grid, 1, 4, 0);
        yield return new WaitForSeconds(12);
        yield return new WaitUntil(() => cameraMovement.GetDirection() == 3);
        cameraMovement.enabled = false;
        StartCoroutine(AdjustVolume(end1, 0, 0.5f));
        StartCoroutine(AdjustVolume(end2, 0, 0.5f));
        StartCoroutine(AdjustVolume(endBackground, 0, 0.5f));
        endEyes.Play();
        StartCoroutine(AdjustFOV(-10, 12));
        yield return new WaitForSeconds(12);
        eyeMat.EnableKeyword("_EMISSION");
        eyes.Run();
        yield return new WaitForSeconds(8);
        Creds.SetActive(true);  // rolls credits, which will then call the ending screen
        yield return new WaitForSeconds(3);
        endCredits.Play();
    }

    IEnumerator AdjustVolume(AudioSource audioSource, float endVolume, float duration)
    {
        float startVolume = audioSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, (t / duration));
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    IEnumerator AdjustFOV(float FOVchange, float duration)
    {
        float startFOV = mainCamera.fieldOfView;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, startFOV + FOVchange, (t / duration));
            yield return null;
        }

        mainCamera.fieldOfView = startFOV + FOVchange;
    }
}
