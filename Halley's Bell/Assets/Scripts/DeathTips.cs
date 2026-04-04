using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathTips : MonoBehaviour
{
    public TextMeshProUGUI tips;

    private float fadeSpeed = 0.5f;
    private bool flashing = false;

    // Start is called before the first frame update
    void Start()
    {
        tips.text = PlayerPrefs.GetString("DeathTip", "Tip: Obey the company!");
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
