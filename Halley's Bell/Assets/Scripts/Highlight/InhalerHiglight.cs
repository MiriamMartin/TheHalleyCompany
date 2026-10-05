using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hotfix to slap on the inhaler to work around its multi materialness
public class InhalerHiglight : MonoBehaviour
{
    public Renderer body1;
    public Renderer body2;
    float EMax1;
    float EMax2;

    private void Start()
    {
        EMax1 = body1.material.GetFloat("_EMax");
        EMax2 = body2.material.GetFloat("_EMax");
    }


    void OnMouseEnter()
    {

        body1.material.SetFloat("_ELevel", EMax1);
        body2.material.SetFloat("_ELevel", EMax2);

    }

    void OnMouseExit()
    {
        body1.material.SetFloat("_ELevel", 0.0f);
        body2.material.SetFloat("_ELevel", 0.0f);

    }
}
