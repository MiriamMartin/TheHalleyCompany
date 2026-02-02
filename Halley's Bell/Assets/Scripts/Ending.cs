using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{
    public CameraMovement cameraMovement;
    private int[,] grid;
    private int[] playerPosition;
    private bool trigger = false;
    public Material eyeMat;


    public GameObject wall1;
    public GameObject wall2;

    // Start is called before the first frame update
    void Start()
    {
        grid = cameraMovement.getGrid();
        eyeMat.DisableKeyword("_EMISSION");
        wall1.SetActive(false);
        wall2.SetActive(false);
    }

    public void Run()
    {
        StartCoroutine(EndSequence());
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = cameraMovement.getPlayerPosition();
    
        if (grid[playerPosition[0], playerPosition[1]] == 2) {
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
        cameraMovement.setGridTile(grid, 2, 2, 0);
        wall1.SetActive(true);
        yield return new WaitUntil(() => trigger == false);
        yield return new WaitUntil(() => (trigger == true) && (cameraMovement.GetDirection() == 2));
        yield return new WaitForSeconds(0.5f);
        wall2.SetActive(true);
        cameraMovement.setGridTile(grid, 1, 2, 0);
        yield return new WaitUntil(() => cameraMovement.GetDirection() == 1);
        yield return new WaitUntil(() => cameraMovement.GetDirection() == 3);
        cameraMovement.enabled = false;
        yield return new WaitForSeconds(4);
        eyeMat.EnableKeyword("_EMISSION");
    }
}
