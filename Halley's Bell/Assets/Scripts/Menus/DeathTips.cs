using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathTips : MonoBehaviour
{
    public TextMeshProUGUI tips;
    private string tipType;

    private float fadeSpeed = 0.5f;
    private bool flashing = false;

    private List<string> switchTips = new List<string>() { "Didn't I tell you to check on those Switches, Op?" };
    private List<string> gaugeTips = new List<string>() { "What did I say about keeping an eye on the Gauge, Op?", "Forget to keep an eye on the internal presssure, Op?" };
    private List<string> inhalerTips = new List<string>() { "Think you forgot to puff puff, Op.", "The Inhaler's part of maintenance too, Op." };


    // Start is called before the first frame update
    void Start()
    {
        tipType = PlayerPrefs.GetString("DeathTip", "obey");
        SetTip();
    }

    // Update is called once per frame
    void Update()
    {
        if (!flashing)
        {
            flashing = true;
            StartCoroutine(flash());
        }
    }

    public void SetTip()
    {
        if (tipType == "switches")
        {
            tips.text = switchTips[Random.Range(0, switchTips.Count)];
        }
        else if (tipType == "gauges")
        {
            tips.text = gaugeTips[Random.Range(0, gaugeTips.Count)];
        }
        else if (tipType == "inhaler")
        {
            tips.text = inhalerTips[Random.Range(0, inhalerTips.Count)];
        }
        else
        {
            tips.text = "Tip: Obey the company!";  // you should never see this since you can't die from anything else
        }
    }

    private IEnumerator flash()
    {
        Color logo = tips.color;

        while (logo.a > 0)
        {
            logo.a = logo.a - 1 * fadeSpeed * Time.deltaTime;
            tips.color = logo;
            yield return null;
        }
        while (logo.a < 1)
        {
            logo.a = logo.a + 1 * fadeSpeed * Time.deltaTime;
            tips.color = logo;
            yield return null;
        }

        flashing = false;
    }
}
