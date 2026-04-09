using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TerminalBoot : MonoBehaviour
{
    public TextMeshProUGUI Loading;

    private void OnEnable()
    {
        Loading.text = "LOADING";
        StartCoroutine(loading());
    }

    public IEnumerator loading()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.4f);
            Loading.text += " .";
        }
    }
}
