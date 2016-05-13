using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioSource sfxSource;
    public AudioSource musicSource;

    public float lowPitch = 0.95f;
    public float highPitch = 1.05f;

    // Singleton
    public static SoundManager instance = null;

	// Use this for initialization
	void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
	}

    public void PlaySingle(AudioClip clip)
    {
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void RandomSfx(params AudioClip[] clips)
    {
        int randomInt = Random.Range(0, clips.Length);
        float randomFloat = Random.Range(lowPitch, highPitch);

        sfxSource.pitch = randomFloat;
        sfxSource.clip = clips[randomInt];
        sfxSource.Play();
    }
}
