using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, ButtonInterface
{

    // We want:
    // 
    // On Click, it flips next direction -- need a way to track direction, use Y rotation (180) to 'flip' switch
    // Lights to turn on based on direction to be flipped.

    public GameObject TopLight;
    public GameObject BottomLight;
    public GameObject SwitchButton;

    private Material TLMat;
    private Material BLMat;

    public bool up;  // true for flipped up, false for flipped down.

    // Start is called before the first frame update
    void Start()
    {
        up = false;
        TLMat = SetUpLight(TopLight);
        BLMat = SetUpLight(BottomLight);
        TLMat.DisableKeyword("_EMISSION");
        BLMat.DisableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Button(bool mouseDown, string message)
    {
        if (!mouseDown || PauseManager.Instance.getIsPaused()) return;  // only on mousedown, and not while paused

        if (!up)  // if facing down, flip up
        {
            FlipSwitch(-80f);
            up = !up;
        }
        else  // if facing up, flip down
        {
            FlipSwitch(-65f);
            up = !up;
        }

        CheckLightAligned(); // if at any point the switch aligns with the light, the light turns off
    }

    public void FlipSwitch(float dir)
    {
        // Transforms the switch's X rotation

        Vector3 currentRotation = SwitchButton.transform.eulerAngles;
        SwitchButton.transform.eulerAngles = new Vector3(dir, currentRotation.y, currentRotation.z);
    }

    public Material SetUpLight(GameObject light)
    {
        Renderer rend = light.GetComponent<MeshRenderer>();
        return rend.material;
    }

    public void LightControl(Material light, int state)
    {
        // Turns light on or off based on state
        // state = 0 : turn on 
        // state = 1 : turn off

        if (state == 0)
        {
            light.EnableKeyword("_EMISSION");
        }
        else if (state == 1)
        {
            light.DisableKeyword("_EMISSION");
        }
    }

    public void RandomLight()
    {
        // Randomly picks a switch direction to turn on
        // Only does so if the switch isn't already pointed in that direction

        int randInt = UnityEngine.Random.Range(0,2);

        if (randInt == 0 && !up)
        {
            LightControl(TLMat, 0);
        }
        else if (randInt == 1 && up)
        {
            LightControl(BLMat, 0);
        }

    }

    public void CheckLightAligned()
    {
        // Checks to see if the switch direction is aligned with the light
        // If so, turns it off

        if (up && TLMat.IsKeywordEnabled("_EMISSION"))
        {
            TLMat.DisableKeyword("_EMISSION");
        }
        else if (!up && BLMat.IsKeywordEnabled("_EMISSION"))
        {
            BLMat.DisableKeyword("_EMISSION");
        }
    }

}
