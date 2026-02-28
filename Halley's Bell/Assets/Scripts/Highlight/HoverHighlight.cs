using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    Color startcolor;
    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void OnMouseEnter()
    {
        //startcolor = rend.material.color;
        //rend.material.color = Color.yellow;
        rend.material.EnableKeyword("_EMISSION");
    }

    void OnMouseExit()
    {
        //rend.material.color = startcolor;
        rend.material.DisableKeyword("_EMISSION");
    }
}
