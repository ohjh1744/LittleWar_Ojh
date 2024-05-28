using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사운드 비활성화
public class SoundDeActive : MonoBehaviour
{
    public AudioSource audioSource;
    
    void Update()
    {
        if(!audioSource.isPlaying) gameObject.SetActive(false);
    }
}
