using UnityEngine;

public class AudioController : Singleton<AudioController>
{
    public AudioClip clickSound;
    public AudioClip cardMatchSound;
    public AudioClip cardMisMatchSound;
    public AudioClip gameOverSound;

    public AudioSource audioPlayer;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        audioPlayer = gameObject.AddComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        audioPlayer.PlayOneShot(clickSound);
    }

    public void PlayCardMatchSound()
    {
        audioPlayer.PlayOneShot(cardMatchSound);
    }

    public void PlayCardMisMatchSound()
    {
        audioPlayer.PlayOneShot(cardMisMatchSound);
    }

    public void PlayGameOverSound()
    {
        audioPlayer.PlayOneShot(gameOverSound);
    }

   
}
