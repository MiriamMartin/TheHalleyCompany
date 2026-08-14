using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject removeableWall;
    public GameObject bellHull;
    public GameObject door;
    public GameObject fakeEye;

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
        //First Segment
        yield return new WaitUntil(() => trigger == true); //Wait until stepping onto tile "2"
        cameraMovement.setGridTile(grid, 12, 5, 0); //Create wall
        wall1.SetActive(true); //Create wall visual
        yield return new WaitUntil(() => trigger == false); //Wait until stepping off of "2"
        yield return new WaitUntil(() => cameraMovement.getPlayerPosition()[0] == 11 && cameraMovement.getPlayerPosition()[1] == 5 && cameraMovement.GetDirection() == 1); //Wait until looking at new wall
        wallMisc.SetActive(true); //Add pipes in hallway
        //Play scary sounds
        endHit.Play(); 
        end1.Play();
        StartCoroutine(AdjustVolume(end1, 1, 0.5f));
        StartCoroutine(AdjustVolume(endBackground, 1, 0.5f));
        //yield return new WaitForSeconds(12f); No pause post-maze
        //Remove hull and wall leading into maze
        bellHull.SetActive(false);
        removeableWall.SetActive(false);
        fakeEye.SetActive(false);
        door.SetActive(false);
        cameraMovement.setGridTile(grid, 10, 3, 1); //Make maze opening tile walkable


        //Second Segment
        yield return new WaitUntil(() => (trigger == true) && (cameraMovement.GetDirection() == 3)); //Wait until looking at the newly formed maze
        endHit.Play();
        end2.Play();
        StartCoroutine(AdjustVolume(end2, 1, 0.5f));

        /*
        //Second Segment (Pre-Maze)
        yield return new WaitUntil(() => (trigger == true) && (cameraMovement.GetDirection() == 2));
        yield return new WaitForSeconds(0.5f);
        wall2.SetActive(true);

        yield return new WaitUntil(() => cameraMovement.GetDirection() == 0);
        wallLever.SetActive(true);
        endHit.Play();
        end2.Play();
        StartCoroutine(AdjustVolume(end2, 1, 0.5f));
        cameraMovement.setGridTile(grid, 11, 4, 0);
        yield return new WaitForSeconds(12);
        */

        //Third Segment (Zoom)
        yield return new WaitUntil(() => (cameraMovement.GetDirection() == 3) && cameraMovement.getPlayerPosition()[0] == 5 && cameraMovement.getPlayerPosition()[1] == 7); //Wait until in front of and looking at maze exit
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

    // This is such a shitty way to do omg im so sorry
    public void RUN_DEMO_ENDING()
    {
        StartCoroutine(DEMO_ENDING());
    }
    public IEnumerator DEMO_ENDING()
    {
        cameraMovement.enabled = false;
        
        Creds.SetActive(true);  // rolls credits, which will then call the ending screen
        yield return new WaitForSeconds(3);
        endCredits.Play();

        Image fb = Creds.transform.Find("BLACKOUT").GetComponent<Image>();

        Color fblack = fb.color;

        while (fblack.a < 1)
        {
            // print(fblack.a);
            fblack.a = fblack.a + 1 * 0.075f * Time.deltaTime; // OG FOR LONG DEMO == 1 * 0.05f, QUICK DEMO == 1 * 0.075f
            fb.color = fblack;
            yield return null;
        }
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
