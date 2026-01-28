using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    //ROTATION INITIALIZING
    public float angleAmount = 90f;
    public float duration = 0.5f;
    private bool isRotating = false;
    private bool isMoving = false;


    //MOVMENT INITIALIZING
    private int gridDir; //0 is east (facing 90degrees right of hallway), rotates clockwise (1 is east)

    private bool standing = false;

    private int[,] grid;
    private int[] playerPos;
    public float moveMult = 16f;

    // Start is called before the first frame update
    void Start()
    {
        //initializing 2d representation of moveable area
        grid = new int[,]
        {
            { 0, 0, 0, 0, 0 },
            { 0, 1, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0 }
        };


        playerPos = new int[] { 3, 2 }; //Starting position of player
        gridDir = 3; //MAKE SURE THIS NUMBER CORROSPONDS TO STARTING DIRECTION
    }

    // Update is called once per frame
    void Update()
    {
        //Activating rotation/movement coroutines based on user input
        if (!isRotating && !isMoving)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(Rotate(1));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Rotate(-1));
            } 
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (standing)
                {
                    StartCoroutine(Move());
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                standing = true;
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            }
        }

    }

    
    IEnumerator Rotate(int dir)
    {
        isRotating = true;
        //Debug.Log("Rotating " + dir);
        float elapsed = 0f;
        gridDir = (gridDir - dir + 4) % 4; //To track direction facing for movement
        Debug.Log("Calculated gridDir to be " + gridDir);

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.AngleAxis(angleAmount * dir, Vector3.up);

        while (elapsed < duration)
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
        Debug.Log("Moving facing: " + gridDir);
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

        if (grid[target[0], target[1]] == 1)
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
            Debug.Log("Invalid Move");

        }

    }


}
