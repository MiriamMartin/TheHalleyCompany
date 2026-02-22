using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoudnessDetection : MonoBehaviour
{

    public int sampleWindow = 64;

    //From: https://www.youtube.com/watch?v=dzD0qP8viLw&list=LL&index=42
    public float GetLoudnessFromAudio(int clipPosition, AudioClip clip )
    {
        int startPosition = clipPosition - sampleWindow;

        if(startPosition < 0)
        {
            return 0;
        }

        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        float totalLoudness = 0;

        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        return totalLoudness / sampleWindow;
    }
}
