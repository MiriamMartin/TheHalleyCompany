using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class tempPhoto : MonoBehaviour
{

    public GameObject text;
    public GameObject lines;
    private bool act = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Depth.Instance.runBlackout || Checkpoints.Instance.getCheckpoint() >= 6) && !act)
        {
            act = true;
            text.SetActive(true);
            lines.SetActive(true);
        }
    }
}
