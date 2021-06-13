using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Boss : MonoBehaviour { 

    public BossController boss;

    private bool phase2_playing = false;

    [Header("Audio Elements")]
    [SerializeField] public AudioSource audio;
    [SerializeField] public AudioSource audio2;
    [SerializeField] public AudioClip Final_Boss_1;
    [SerializeField] public AudioClip Final_Boss_2;

    // Start is called before the first frame update
    void Start()
    {
        audio.clip = Final_Boss_1;
        audio.loop = true;
        audio.Play();

        audio2.clip = Final_Boss_2;
        audio2.loop = true;
        audio2.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.hasSpawned && !phase2_playing)
        {
            phase2_playing = true;
            audio.Stop();
            audio2.Play();
        } 
    }
}
