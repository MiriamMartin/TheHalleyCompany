using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialSounder : MonoBehaviour
{
    public AudioSource spatVolume; //Used to control all spatial objects in group!
    public float targetTime = 0f;
    public bool DEBUGMODE = false;

    private void Start()
    {
        if (DEBUGMODE)
        {
            Run(1);
        }
    }

    void Run(float volume)
    {
        SpatialSoundObject[] spatialSoundObjectScripts = GetComponentsInChildren<SpatialSoundObject>();

        foreach (SpatialSoundObject spatSoundObj in spatialSoundObjectScripts)
        {
            spatSoundObj.Run(volume * spatVolume.volume, targetTime);
        }
    }

    void UpdateVolume(float volume)
    {
        SpatialSoundObject[] spatialSoundObjectScripts = GetComponentsInChildren<SpatialSoundObject>();

        foreach (SpatialSoundObject spatSoundObj in spatialSoundObjectScripts)
        {
            spatSoundObj.UpdateVolume(volume * spatVolume.volume);
        }
    }
}
