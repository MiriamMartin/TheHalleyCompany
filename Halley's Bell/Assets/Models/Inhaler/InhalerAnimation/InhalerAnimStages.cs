using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhalerAnimStages : MonoBehaviour
{
    Animator animator;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(InhAnim());
    }

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
    }

    private IEnumerator ShakeAnim()
    {
        animator.SetTrigger("Shake");
        yield return new WaitForSeconds(3f);
        animator.ResetTrigger("Shake");
    }

    private IEnumerator PuffAnim()
    {
        animator.SetTrigger("Puff");
        yield return new WaitForSeconds(3f);
        animator.ResetTrigger("Puff");
    }

    private IEnumerator InhAnim()
    {
        animator.SetTrigger("Shake");
        yield return new WaitForSeconds(3f);
        animator.ResetTrigger("Shake");
        animator.SetTrigger("Puff");
        yield return new WaitForSeconds(3f);
        animator.ResetTrigger("Puff");
    }

}