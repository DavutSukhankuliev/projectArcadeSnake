using UnityEngine;
using Zenject;

namespace ArcadeSnake {
    public class AudioSourceController : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        
        private bool _isPlay;

        private float _defaultVolume; 

        public void SetAudioSource(AudioModel audioModel)
        {
            _audioSource.clip = audioModel.Clip;
            _audioSource.volume = audioModel.Volume;
            _defaultVolume = _audioSource.volume;
            _audioSource.loop = audioModel.Loop;
            _audioSource.Play();
            _isPlay = true;
        }

        public void Update()
        {
            if(_audioSource.loop == false)
            {
                if (!_isPlay)
                {
                    return;
                }
                if (!_audioSource.isPlaying)
                {
                    _isPlay = false;
                    Delete();
                }
            }
        }

        public void Delete()
        {
            _audioSource.Stop();
            Destroy(gameObject);
        }

        public void PauseMusic()
        {
            _audioSource.Pause();
        }

        public void UnPauseMusic()
        {
            _audioSource.UnPause();
        }

        public void ResetMusic()
        {
            _audioSource.Stop();
        }

        public void PlayMusic()
        {
            _audioSource.Play();
        }

        public void ResetMusicVolume()
        {
            SetVolume(0);
        }

        public void SetVolume(float vol)
        {
            if (vol == 0)
            {
                _audioSource.volume = _defaultVolume;
            }
            else
            {
                _audioSource.volume = vol;
            }
        }

        public void SetVolumeToHalf()
        {
            _audioSource.volume = _defaultVolume / 3;
        }

        public void Mute(bool e)
        {
            _audioSource.mute = e;
        }

        public class Factory : PlaceholderFactory<AudioSourceController>
        {

        }
    }
}