using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public float scrollSpeed = 40f;
    private bool stopScrolling = false;
    private Transform rect;

    public Image FirstLogo;
    public Image LastLogo;
    public GameObject AllCreds;

    private bool fadeInComplete = false;
    public float fadeSpeed = 1f;

    public RectTransform lastElement;

    // Start is called before the first frame update
    void Start()
    {
        rect = AllCreds.transform;

    }

    private void Update()
    {
        // fades in the first logo, then starts scrolling the credits, then when last logo is in center of screen --> fades it out and opens end screen

        if (!fadeInComplete)
        {
            LogoFadeIn();
        }
        else
        {
            if (!stopScrolling) { RollCredits(); }
            else
            {
                LogoFadeOut();
            }
        }
    }

    public void LogoFadeIn()
    {
        // fades the logo in first

        Color lgo = FirstLogo.color;

        if (lgo.a < 1)
        {
            lgo.a = lgo.a + 1 * fadeSpeed * Time.deltaTime;
            FirstLogo.color = lgo;
        }
        else
        {
            fadeInComplete = true;
        }
    }
    public void RollCredits()
    {
        rect.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        // check if last logo in center
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null,lastElement.position);
        float screenCenterY = Screen.height / 2f;

        if (Mathf.Abs(screenPos.y - screenCenterY) < 5f)
        {
            stopScrolling = true;
        }
        }

    public void LogoFadeOut()
    {
        // fades the logo out last

        Color logo = LastLogo.color;

        if (logo.a > 0)
        {
            logo.a = logo.a - 1 * fadeSpeed * Time.deltaTime;
            LastLogo.color = logo;
        }
        else
        {
            PauseManager.Instance.End();
        }
    }

}
