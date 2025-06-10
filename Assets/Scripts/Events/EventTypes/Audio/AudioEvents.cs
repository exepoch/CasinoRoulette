using System;
using Data;
using UnityEngine;

namespace Events.EventTypes.Audio
{
    public static class AudioEvents
    {
        public static event Action<SoundType> OnSoundRequested;

        public static void RequestSound(SoundType type)
        {
            OnSoundRequested?.Invoke(type);
        }
    }
}
