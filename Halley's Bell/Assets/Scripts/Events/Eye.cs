using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1f, 1f, 1f, 0f);
    }

    public void Run()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(Random.Range(0f, 3f));
        float randBrightness = Random.Range(0.6f, 0.9f);
        float t = 0f;
        float duration = Random.Range(1f, 3f);

        Color c = sr.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, randBrightness, (t / duration));
            sr.color = c;
            yield return null;
        }
    }
}
