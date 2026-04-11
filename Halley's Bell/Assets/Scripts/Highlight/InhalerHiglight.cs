using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hotfix to slap on the inhaler to work around its multi materialness
public class InhalerHiglight : MonoBehaviour
{
    public Renderer body1;
    public Renderer body2;


    void OnMouseEnter()
    {
        body1.material.EnableKeyword("_EMISSION");
        body2.material.EnableKeyword("_EMISSION");

    }

    void OnMouseExit()
    {
        body1.material.DisableKeyword("_EMISSION");
        body2.material.DisableKeyword("_EMISSION");

    }
}
