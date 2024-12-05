using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource; 
    public AudioClip[] audioClips; 
    public float minInterval = 30f; 
    public float maxInterval = 60f; 
    private float timer;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetTimer();
    }

    void Update()//count down for timer.
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)//play conditon and reset timer
        {
            PlayRandomClip();
            SetTimer(); 
        }
    }
    void PlayRandomClip()//array of sounds and randomly choose one
    {           
            int randomIndex = Random.Range(0, audioClips.Length); 
            audioSource.clip = audioClips[randomIndex]; 
            audioSource.Play(); 
    }
    void SetTimer()
    {
        timer = Random.Range(minInterval, maxInterval); //randomly choosing a number between the max and min
    }
}
