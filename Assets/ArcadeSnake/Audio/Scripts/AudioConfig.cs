using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcadeSnake
{
    [Serializable]
    public struct AudioModel
    {
        public string ID;
        public AudioClip Clip;
        public float Volume;
        public bool Loop;
    }

    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Config/AudioConfig", order = 0)]
    public class AudioConfig : ScriptableObject
    {
        [SerializeField] private AudioModel[] _clips;

        private Dictionary<string, AudioModel> _dictAudio = new Dictionary<string, AudioModel>();

        public AudioModel Get(string id)
        {
            Init();
            return _dictAudio[id];
        }

        private void Init()
        {
            if (_dictAudio.Count > 0)
            {
                return;
            }
            foreach (var clip in _clips)
            {
                _dictAudio.Add(clip.ID, clip);
            }
        }
    }
}