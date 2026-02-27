using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;


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
    public GameObject ad;
    public GameObject space;
    public GameObject w;

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
        ad.SetActive(true);
        space.SetActive(false);
        w.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Activating rotation/movement coroutines based on user input
        if (!isRotating && !isMoving && !PauseManager.Instance.getIsPaused())
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(Rotate(1));
                AudioHandler.Instance.PlayMovement(1);
                ad.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Rotate(-1));
                AudioHandler.Instance.PlayMovement(1);
                ad.SetActive(false);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (standing)
                {
                    StartCoroutine(Move());
                    AudioHandler.Instance.PlayMovement(2);
                    w.SetActive(false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !standing && canStand)
            {
                StartCoroutine(StandingRotation(1));
                space.SetActive(false);
                w.SetActive(true);
                standing = true;
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
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
            canStand = false;
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

    public void setGridTile(int[,] grid, int row, int col, int tileType)
    {
        grid[row, col] = tileType;
    }

    public void BlackoutEvent()
    {
        canStand = true;
        space.SetActive(true);
    }

    public void BlackoutEnd()
    {
        canSit = true;
        StartCoroutine(CrazyTimeTrigger());
    }

    //post-blackout waits until the player is sitting, then invokes crazytime event
    IEnumerator CrazyTimeTrigger()
    {
        yield return new WaitUntil(() => (!standing));
        CrazyTime.Invoke();

        Depth.Instance.runSwitches = true;  // Can add to Blackout Event after Demo, for now this will only start switches again post-blackout.
        Depth.Instance.setDescending(true);  // won't overlap ending with blackout if standing too long
    }

    // ==================== Camera Shake =======================

    void CameraShake()
    {
        // checks when to start camera shake

        if (Depth.Instance.runHitFloor)
        {
            Depth.Instance.runHitFloor = false;
            StartCoroutine(Shaking());
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
}
