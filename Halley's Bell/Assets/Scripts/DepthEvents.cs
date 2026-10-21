using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[System.Serializable]  // Lets us see the class in the inspector, and define the variables below as well
public class DepthEvent
{
    // This class is used to represent single Messages (be it radio, or just typed text).

    [SerializeField] public string Name;                // What event is this? Used for organization purposes in the inspector.
    [SerializeField] public string EventID;             // The ID for this event (inner use, but shorter than name).
    [SerializeField] public string Text;                // The terminal text associated with this event (what is typed). NONE if there isn't any.
    [SerializeField] public AudioClip Audio;            // The audio clip for the radio message associated with this event (what is heard). NONE if there isn't any.
    [SerializeField] public int EventTrigger;          // The depth value to trigger the typing of the text on the terminal.
    [SerializeField] public int MessageTrigger;        // The depth value to trigger the audio and/or text message associated with this event.

    [SerializeField] public List<int> TextTiming;

    public bool EventTriggered { get; set; }
    public bool MessageTriggered { get; set; }

    public DepthEvent(string nm, string id, string txt, AudioClip audio, int txtTrigger, int audioTrigger)
    {
        // Initialize all the variables for this Message instance
        Name = nm;
        EventID = id;
        Text = txt;
        Audio = audio;
        EventTrigger = txtTrigger;
        MessageTrigger = audioTrigger;
    }

    public void PlayAudio(Radio radio)
    {
        if (Audio != null)
        {
            radio.PlayRadioDescentSeq(Audio);
        }
    }

    public void TEMPAUDIO(AudioSource src)
    {
        if (Audio != null)
        {
            src.clip = Audio;
            src.Play();
        }
    }

    public void TypeText(Terminal term)
    {
        if (Text != null)
        {
            term.TypeMessageDescentSeq(Text, TextTiming);
        }
    }

    public void BarkText(Terminal term)
    {
        if (Text != null && Audio == null)
        {
            term.Bark(Text);
        }
    }
}

[CreateAssetMenu(fileName = "DescentSequence", menuName = "ScriptableObject/DescentSequence")]
public class DescentSequence : ScriptableObject
{
    public List<DepthEvent> events = new();
}
