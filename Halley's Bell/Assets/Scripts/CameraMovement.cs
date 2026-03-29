using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;


public class CameraMovement : MonoBehaviour, BlackoutInterface
{

    [Header("Blackout Event")]
    public UnityEvent CrazyTime;

    [Header("Player Movement Rotation")]
    public float angleAmount = 120f;
    public int numberOfDirections = 3;
    public float duration = 0.5f;
    public float startAngle = -90;
    private bool isRotating = false;
    private bool isMoving = false;

    public bool DEBUGMODE = false; //so the player can stand whenever

    [Header("Controls")]
    public GameObject controls_A;
    public GameObject controls_D;
    public GameObject controls_Space;
    public GameObject controls_W;
    public GameObject controls_Esc;
    private float controlFadeSpeed = 1f;  // how long it takes to fade
    private float controlHangTime = 2f;  // how long before fade

    //MOVMENT INITIALIZING
    private int gridDir; //0 is east (facing 90degrees right of hallway), rotates clockwise (1 is east)

    private bool standing = false;
    private bool canStand = false;
    private bool canSit = false;

    private int[,] grid;
    private int[] playerPos;
    public float moveMult = 1f;

    [Header("Camera Shake")]
    public float shakeDuration = 1f;
    public AnimationCurve shakeCurve;

    // Start is called before the first frame update
    void Awake()
    {
        transform.rotation = Quaternion.AngleAxis(startAngle, Vector3.up);
        canStand = DEBUGMODE;
        numberOfDirections = 3;
        //initializing 2d representation of moveable area (0 is wall, 1 & 2 is moveable, 2 is special trigger
        grid = new int[,]
        {
            { 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 2, 1, 1, 0 },
            { 0, 0, 0, 1, 0, 1, 0 },
            { 0, 0, 0, 0, 0, 1, 0 },
            { 0, 0, 0, 1, 3, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 0 }
        };


        playerPos = new int[] { 4, 4 }; //Starting position of player
        gridDir = 3; //MAKE SURE THIS NUMBER CORROSPONDS TO STARTING DIRECTION

        //text stuff
        controls_A.SetActive(true);
        controls_D.SetActive(true);
        controls_Esc.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(FadeControls(controls_Esc));
        }

        //Activating rotation/movement coroutines based on user input
        if (!isRotating && !isMoving && !PauseManager.Instance.getIsPaused())
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(Rotate(1));
                AudioHandler.Instance.PlayMovement(1);
                StartCoroutine(FadeControls(controls_A));
                StartCoroutine(FadeControls(controls_D));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Rotate(-1));
                AudioHandler.Instance.PlayMovement(1);
                StartCoroutine(FadeControls(controls_A));
                StartCoroutine(FadeControls(controls_D));
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (standing)
                {
                    StartCoroutine(Move());
                    AudioHandler.Instance.PlayMovement(2);
                    StartCoroutine(FadeControls(controls_W));
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !standing && canStand)
            {
                StartCoroutine(StandingRotation(1));
                StartCoroutine(FadeControls(controls_Space));
                controls_W.SetActive(true);
                standing = true;
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                canSit = true;
            }

            CameraShake();  // when Depth asks for camera shake, runs it.
        }

    }

    
    IEnumerator Rotate(int dir)
    {
        isRotating = true;
        float elapsed = 0f;
        gridDir = (gridDir - dir + (numberOfDirections)) % (numberOfDirections); //To track direction facing for movement
        //Debug.Log("Calculated gridDir to be " + gridDir);

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.AngleAxis(angleAmount * dir, Vector3.up);

        float dur = duration;

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
    }

    IEnumerator StandingRotation(int dir)
    {
        numberOfDirections = 4;
        angleAmount = -90f;
        isRotating = true;
        float elapsed = 0f;
        gridDir = 0;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.AngleAxis(0f * dir, Vector3.up);

        float dur = duration;

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
        Move();
        //Debug.Log("Finished standing up");
    }

    IEnumerator SittingRotation(int dir)
    {
        numberOfDirections = 3;
        angleAmount = -120f;
        isRotating = true;
        float elapsed = 0f;
        gridDir = 3;
        //Debug.Log("Calculated gridDir to be " + gridDir);

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.AngleAxis(270f * dir, Vector3.up);

        float dur = duration;

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;

    }

    IEnumerator Move()
    {
        float elapsed = 0f;

        int xMove = 0;
        int yMove= 0;

        if (gridDir == 0)
        {
            yMove = 1;
        }
        else if (gridDir == 1)
        {
            xMove = 1;
        }
        else if (gridDir == 2)
        {
            yMove = -1;
        }
        else if (gridDir == 3) 
        {
            xMove = -1;
        } else
        {
            Debug.Log("Error: Invalid grid direction in CameraMovement Move function. gridDir = " + gridDir );
        }

        int[] target = new int[] {playerPos[0] + xMove, playerPos[1] + yMove};

        if (grid[target[0], target[1]] != 0)
        {
            playerPos = target; //updating playerPos to the target position

            isMoving = true;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = new Vector3(startPosition.x + (xMove * moveMult), startPosition.y, startPosition.z + (yMove * moveMult));

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.position = Vector3.Lerp(startPosition, endPosition, t);
                yield return null;
            }

            transform.position = endPosition;
            isMoving = false;
        } 
        else
        {
            //Debug.Log("Invalid Move");

        }
        //Debug.Log("target is " + grid[target[0], target[1]] + " and canSit is " + canSit);
        if ((grid[target[0], target[1]] == 3) && canSit) //when canSit, sit down on tile 3 when on it (starting tile)
        {
            StartCoroutine(SittingRotation(1));
            standing = false;
            canSit = false;
            canStand = true;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        }

    }

    public int GetDirection()
    {
        return this.gridDir;
    }

    public int[] getPlayerPosition()
    {
        return this.playerPos;
    }

    public int[,] getGrid()
    {
        return this.grid;
    }

    public bool getIsSitting()
    {
        return !standing;
    }

    public void setGridTile(int[,] grid, int row, int col, int tileType)
    {
        grid[row, col] = tileType;
    }

    public float getDuration()
    {
        return this.duration;
    }

    public void BlackoutEvent()
    {
        canStand = true;
        controls_Space.SetActive(true);
    }

    public void BlackoutEnd()
    {
        StartCoroutine(CrazyTimeTrigger());
    }

    //post-blackout waits until the player is sitting, then invokes crazytime event
    IEnumerator CrazyTimeTrigger()
    {
        yield return new WaitUntil(() => (!standing));
        CrazyTime.Invoke();

        Depth.Instance.runSwitches = true;  // Can add to Blackout Event after Demo, for now this will only start switches again post-blackout.
        Depth.Instance.setDescending(true);  // won't overlap ending with blackout if standing too long
        Depth.Instance.runInhaler = true;
    }

    // ==================== Camera Shake =======================

    void CameraShake()
    {
        // checks when to start camera shake

        if (Depth.Instance.runHitFloor)
        {
            Depth.Instance.runHitFloor = false;
            if (PlayerPrefs.GetInt("SavedScreenShakeValue") == 1) { StartCoroutine(Shaking()); }  // only runs if setting is on
        }
    }

    public IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float strength = shakeCurve.Evaluate(elapsedTime / shakeDuration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }

    // needed for testing, delete
    public void setStand(bool val)
    {
        canStand = val;
    }

    public IEnumerator FadeControls(GameObject controlOverlay)
    {

        if (controlOverlay.activeSelf == false) { yield break; }  // don't run if it's no longer active

        yield return new WaitForSeconds(controlHangTime);

        var img = controlOverlay.GetComponentInChildren<Image>();
        var txt = controlOverlay.GetComponentInChildren<TextMeshProUGUI>();

        Color img_c = img.color;
        Color txt_c = txt.color;

        // fade alpha out
        while (img_c.a > 0)
        {
            img_c.a = img_c.a - 1 * controlFadeSpeed * Time.deltaTime;
            img.color = img_c;

            txt_c.a = txt_c.a - 1 * controlFadeSpeed * Time.deltaTime;
            txt.color = txt_c;
            yield return null;
        }


        // reset to 100 in case controls re-appear later
        Color img_fin = img.color;
        Color txt_fin = txt.color;

        img_fin.a = 1;
        txt_fin.a = 1;
        img.color = img_fin;
        txt.color = txt_fin;

        // disable it
        controlOverlay.SetActive(false);
    }
}
