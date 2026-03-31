using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : MonoBehaviour
{
    private void Start()
    {

    }

    public void Run()
    {
        Eye[] eyeScripts = GetComponentsInChildren<Eye>();

        foreach (Eye eyeScript in eyeScripts)
        {
            eyeScript.Run();
        }
    }
}
