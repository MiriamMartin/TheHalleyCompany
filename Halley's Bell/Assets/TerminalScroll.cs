using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalScroll : MonoBehaviour
{

    public string dir;
    public Terminal term;


    private bool mouseDown = false;
    private bool canScroll = true;
    private bool changing = false;

    private void OnMouseDown()
    {
        mouseDown = true;
    }

    private void OnMouseOver()
    {
        if (mouseDown && dir == "changeScreen" && !changing) { term.ChangeScreens(); changing = true; }
        else if (mouseDown && canScroll) { StartCoroutine(scrolling()); }
    }

    public IEnumerator scrolling()
    {
        canScroll = false;
        term.Scroll(dir);
        yield return new WaitForSeconds(0.1f);
        canScroll = true;
    }

    private void OnMouseUp()
    {
        mouseDown = false;
        changing = false;
    }
}
