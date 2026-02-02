using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchEvent : MonoBehaviour
{

    public List<GameObject> switches;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))  // if press keypad 4, run event
        {
            foreach (GameObject sw in switches)  // for each switch, it tries a random light
            {
                if (sw != null)
                {
                    sw.GetComponent<Switch>().RandomLight();
                }
            }
        }
    }
}
