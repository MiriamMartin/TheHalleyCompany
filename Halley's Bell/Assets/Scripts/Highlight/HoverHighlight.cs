using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    Color startcolor;
    float EMax;
    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        try
        {
            EMax = rend.material.GetFloat("_EMax");
        }
        catch { }
    }
    void OnMouseEnter()
    {
        //startcolor = rend.material.color;
        //rend.material.color = Color.yellow;
        rend.material.EnableKeyword("_EMISSION");
        try
        {
            rend.material.SetFloat("_ELevel", EMax);
        }
        catch{}
    }

    void OnMouseExit()
    {
        //rend.material.color = startcolor;
        rend.material.DisableKeyword("_EMISSION");
        try
        {
            rend.material.SetFloat("_ELevel", 0.0f);
        }
        catch{}
    }
}
