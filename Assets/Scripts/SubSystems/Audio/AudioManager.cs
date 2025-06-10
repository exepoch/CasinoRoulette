using System;
using Data;
using Events.EventTypes.Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace SubSystems.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    { 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip loseClip;
        [SerializeField] private AudioClip chipClip;
        [SerializeField] private AudioClip ballBounceClip;
        [SerializeField] private AudioClip spinStartClip;

        private void OnEnable()
        {
            AudioEvents.OnSoundRequested += HandleSoundRequested;
        }

        private void OnDisable()
        {
            AudioEvents.OnSoundRequested -= HandleSoundRequested;
        }

        private void HandleSoundRequested(SoundType type)
        {
            switch (type)
            {
                case SoundType.ButtonClick:
                    audioSource.PlayOneShot(buttonClickClip);
                    break;
                case SoundType.Win:
                    audioSource.PlayOneShot(winClip);
                    break;
                case SoundType.Lose:
                    audioSource.PlayOneShot(loseClip);
                    break;
                case SoundType.BetPlaced:
                    audioSource.PlayOneShot(chipClip);
                    break;
                case SoundType.BallBounce:
                    audioSource.PlayOneShot(ballBounceClip);
                    break;
                case SoundType.SpinStart:
                    audioSource.PlayOneShot(spinStartClip);
                    break;
            }
        }

    }
}
