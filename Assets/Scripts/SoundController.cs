using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    public AudioClip[] Mine;
    public AudioClip[] Throw;
    public AudioClip[] GrabStick;
    public AudioClip[] GrabTorch;
    public AudioClip[] Stun;
    public AudioClip[] DieOfHeat;
    public AudioClip[] DropTorch;

    private AudioSource emitter;

    void Start()
    {
        emitter = GetComponent<AudioSource>();
    }

    public float PlayMine()
    {
        return PlayAudio(SelectRandomClip(Mine));
    }

    public float PlayThrow()
    {
        return PlayAudio(SelectRandomClip(Throw));
    }

    public float PlayGrabStick()
    {
        return PlayAudio(SelectRandomClip(GrabStick));
    }

    public float PlayGrabTorch()
    {
        return PlayAudio(SelectRandomClip(GrabTorch));
    }

    public float PlayStun()
    {
        return PlayAudio(SelectRandomClip(Stun));
    }

    public float PlayDieOfHeat()
    {
        return PlayAudio(SelectRandomClip(DieOfHeat));
    }

    public float PlayDropTorch()
    {
        return PlayAudio(SelectRandomClip(DropTorch));
    }

    private AudioClip SelectRandomClip(AudioClip[] list)
    {
        if (list.Length == 0)
        {
            return null;
        }

        int pos = Random.Range(0, list.Length);

        return list[pos];
    }

    private float PlayAudio(AudioClip clip)
    {
        if (clip != null)
        {
            emitter.PlayOneShot(clip);
            return clip.length;
        }

        return 0;
    }
}
