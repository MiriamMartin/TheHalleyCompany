using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hotfix to slap on the inhaler to work around its multi materialness
public class WrenchHighlight : MonoBehaviour
{
    public Renderer rend;
    float EMax;

    private void Start()
    {
        EMax = rend.material.GetFloat("_EMax");
        

    }


    void OnMouseEnter()
    {
        rend.material.SetFloat("_ELevel", EMax);


    }

    void OnMouseExit()
    {
        rend.material.SetFloat("_ELevel", 0.0f);

    }
}
