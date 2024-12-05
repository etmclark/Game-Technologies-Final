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
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        SetTimer();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayRandomClip();
            SetTimer(); 
        }
    }
    void PlayRandomClip()
    {
            int randomIndex = Random.Range(0, audioClips.Length); 
            audioSource.clip = audioClips[randomIndex]; 
            audioSource.Play(); 
    }
    void SetTimer()
    {
        timer = Random.Range(minInterval, maxInterval); 
    }
}
