using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depth : MonoBehaviour
{
 
    public float maxDepth;
    public float descentSpeed;
    public Transform needle;
    
    private float needleX;
    private float depth = 0f;

    // Start is called before the first frame update
    void Start()
    {
        descentSpeed = descentSpeed / 20f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseManager.Instance.getIsPaused() && depth < maxDepth)
        {
            depth += descentSpeed * Time.deltaTime;
            needle.localPosition += Vector3.right * descentSpeed * Time.deltaTime;
        }
    }
}
