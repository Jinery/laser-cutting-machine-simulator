using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AlarmSystem : MonoBehaviour
{
    private AudioSource _alarmAudio;

    private void Awake()
    {
        _alarmAudio = GetComponent<AudioSource>();
    }

    public void Activate()
    {
        if(_alarmAudio.clip != null)
        {
            _alarmAudio.Play();
        }
    }

    public void Deactivate()
    {
        if (_alarmAudio.isPlaying)
        {
            _alarmAudio.Stop();
        }
    }
}
