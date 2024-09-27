using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip loginSuccesfull, error, switchButton, feedButton, nofication;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlayLoginSound()
    {
        audioSource.volume = 0.3f;
        audioSource.PlayOneShot(loginSuccesfull);
    }
    public void PlayErrorSound()
    {
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(error);
    }
    public void PlaySwitchButtonSound()
    {
        audioSource.volume = 0.7f;
        audioSource.PlayOneShot(switchButton);
    }
    public void PlayFeedButtonSound()
    {
        audioSource.volume = 0.2f;
        audioSource.PlayOneShot(feedButton);
    }
    public void PlayNoficationSound()
    {
        audioSource.volume = 0.5f;
        audioSource.PlayOneShot(nofication);
    }
}
