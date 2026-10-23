using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    float EMax;
    Renderer rend;

    private bool isHighlighted = false;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        try
        {
            EMax = rend.material.GetFloat("_EMax");
        }
        catch { }
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
        rend.material.EnableKeyword("_EMISSION");
        try
        {
            rend.material.SetFloat("_ELevel", EMax);
        }
        catch { }
        isHighlighted = true;
    }

    private void highlightOff()
    {
        rend.material.DisableKeyword("_EMISSION");
        try
        {
            rend.material.SetFloat("_ELevel", 0.0f);
        }
        catch { }
        isHighlighted = false;
    }
}
