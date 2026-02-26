using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, ButtonInterface
{

    public GameObject TopLight;
    public GameObject SwitchButton;
    private Material TLMat;

    public bool switchOn;  // true for turned on, false for turned off.

    // Start is called before the first frame update
    void Start()
    {
        switchOn = false;
        TLMat = SetUpLight(TopLight);
        TLMat.DisableKeyword("_EMISSION");
    }

    public void Button(bool mouseDown, string message)
    {
        if (!mouseDown || PauseManager.Instance.getIsPaused()) return;  // only on mousedown, and not while paused

        if (!switchOn)  // if facing down, flip up
        {
            FlipSwitch(-1);
            switchOn = !switchOn;
        }
        else  // if facing up, flip down
        {
            FlipSwitch(1);
            switchOn = !switchOn;
        }

        CheckLight(); // if at any point the switch aligns with the light, the light turns off
    }

    public void FlipSwitch(float dir)
    {
        // Transforms the switch's X rotation (dir = -1 flips up, dir = 1 flips down)

        SwitchButton.transform.Rotate(15f * dir, 0f, 0f);
    }

    public Material SetUpLight(GameObject light)
    {
        // prob don't need since only have 1 light now

        Renderer rend = light.GetComponent<MeshRenderer>();
        return rend.material;
    }

    public void RandomLight()
    {
        // Randomly turns switch off

        int randInt = UnityEngine.Random.Range(0,2);

        if (randInt == 1)
        {
            turnSwitchOff();
        }

    }

    public void CheckLight()
    {
        // Checks to see if the switch direction is aligned with the light
        // If so, turns it on.

        if (switchOn)
        {
            TLMat.EnableKeyword("_EMISSION");
        }
        else
        {
            TLMat.DisableKeyword("_EMISSION");
        }
    }

    public bool isSwitchOn()
    {
        // getter for switch on, used by switch event

        return switchOn;
    }

    public void turnSwitchOff()
    {
        FlipSwitch(1);
        switchOn = !switchOn;
        TLMat.DisableKeyword("_EMISSION");
    }
}
