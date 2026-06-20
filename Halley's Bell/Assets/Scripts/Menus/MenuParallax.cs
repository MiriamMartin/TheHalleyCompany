using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    Vector3 StartPos;

    [SerializeField] float moveModifier;

    // Start is called before the first frame update
    void Start()
    {
        StartPos = transform.position;

        // This fixes the music and parallax breaking on reload.
        Time.timeScale = 1f;  // resumes timescale
        AudioListener.pause = false; // resumes all audiosources
    }

    // Update is called once per frame
    void Update()
    {

        if (Camera.main == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        //Debug.Log("here");
        Vector2 pz = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        float posX = Mathf.Lerp(transform.position.x, StartPos.x + (pz.x * moveModifier), 2f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.position.y, StartPos.y + (pz.y * moveModifier), 2f * Time.deltaTime);


        //Debug.Log(posX);

        transform.position = new Vector3 (posX, posY, StartPos.z);
    }
}
