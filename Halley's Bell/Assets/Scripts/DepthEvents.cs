using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]  // Lets us see the class in the inspector, and define the variables below as well
public class DepthEvent
{
    // This class is used to represent single events (be it radio, bark, checkpoint, or any mix)

    [SerializeField] public string Name;                // What event is this? Used for organization purposes in the inspector.
    [SerializeField] public string EventID;             // The ID for this event (inner use, but shorter than name).
    [SerializeField] public string Text;                // The terminal text associated with this event (what is typed). NONE if there isn't any.
    [SerializeField] public AudioClip Audio;            // The audio clip for the radio message associated with this event (what is heard). NONE if there isn't any.
    [SerializeField] public int MessageTrigger;         // The depth value to trigger the audio and/or text message associated with this event.
    [SerializeField] public int EventTriggerOffset;     // The depth value to trigger the actual event
    [SerializeField] public int CheckpointDepthOffset;
    [SerializeField] public List<int> TextTiming;

    public int EventTrigger => MessageTrigger + EventTriggerOffset;
    public int CheckpointDepth => MessageTrigger + CheckpointDepthOffset;

    public DepthEvent(string nm, string id, string txt, AudioClip audio, int evntTrigger, int mssgTrigger)
    {
        // Initialize all the variables for this DepthEvent instance
        Name = nm;
        EventID = id;
        Text = txt;
        Audio = audio;
        EventTriggerOffset = evntTrigger;  // CURRENTLY, event triggers always occur after message ones, so I'm just hardcoding that as truth
        MessageTrigger = mssgTrigger;
    }

    public void PlayMessage(Radio radio, Terminal term)
    {
        if (EventID.Contains("T")) // Just a bark
        {
            BarkText(term);
        }
        else
        {
            PlayAudio(radio);
            TypeText(term);
        }
    }

    public void PlayAudio(Radio radio)
    {
        if (Audio != null)
        {
            radio.PlayRadioDescentSeq(Audio);
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

    public void addSavedText(Terminal term)
    {
        if (Text != null)
        {
            if (EventID.Contains("T")) { term.addTextBark(Text); }
            else { term.addTextRadio(Text); }
        }
    }
}

[CreateAssetMenu(fileName = "Depth Events", menuName = "ScriptableObject/DescentSequence")]
public class DepthEvents : ScriptableObject
{
    public List<DepthEvent> events = new();
}
