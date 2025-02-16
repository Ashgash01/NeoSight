using Unity.VisualScripting;
using UnityEngine;

public class AudioMannager : MonoBehaviour
{

    public bool isPlaying;
    AudioSource  audioData;


    public void Update()
    {
        if(!isPlaying)
        {
            audioData = GetComponent<AudioSource>();
            audioData.Play();
            isPlaying = true;
        }
        else
        {
            return;
        }
    }


    }
