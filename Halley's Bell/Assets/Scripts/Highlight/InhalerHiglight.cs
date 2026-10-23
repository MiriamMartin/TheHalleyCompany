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

    private bool isHighlighted = false;

    private void Start()
    {
        EMax1 = body1.material.GetFloat("_EMax");
        EMax2 = body2.material.GetFloat("_EMax");
    }

    private void Update()
    {
        if (PauseManager.Instance.getIsPaused() && isHighlighted)
        {
            highlightOff();
        }
    }

    void OnMouseOver()
    {
        if (!PauseManager.Instance.getIsPaused() && !isHighlighted)
        {
            highlightOn();
        }
    }

    void OnMouseExit()
    {
        if (!PauseManager.Instance.getIsPaused())
        {
            highlightOff();
        }
    }
    
    private void highlightOn()
    {
        body1.material.SetFloat("_ELevel", EMax1);
        body2.material.SetFloat("_ELevel", EMax2);
        isHighlighted = true;
    }
    private void highlightOff()
    {
        
        body1.material.SetFloat("_ELevel", 0.0f);
        body2.material.SetFloat("_ELevel", 0.0f);
        isHighlighted = false;
    }
}
