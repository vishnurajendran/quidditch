using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Utils;

namespace Gameplay
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        [SerializeField]
        private AudioClip menuMusic;
        [FormerlySerializedAs("ambience")] [SerializeField]
        private AudioClip menuAmbience;
        [SerializeField]
        private AudioClip matchMusic;
        [FormerlySerializedAs("ambience")] [SerializeField]
        private AudioClip matchAmbience;
        [SerializeField]
        private AudioClip cheerOnGoal;
        [SerializeField]
        private AudioClip hitSfx;
        [SerializeField]
        private AudioClip[] myGoalVo;
        [SerializeField]
        private AudioClip[] theirGoalVo;
        [SerializeField]
        private AudioClip[] winVo;
        [SerializeField]
        private AudioClip[] loseVo;
        [SerializeField]
        private AudioClip whistle;

        [SerializeField] 
        private AudioClip goalScored;

        [SerializeField] private AudioSource _bgSource;
        [SerializeField] private AudioSource _ambianceSource;
        [SerializeField] private AudioMixer _mixer;

        private float _masterLevel=1;
        private float _sfxAudioLevel=1;
        private float _voAudioLevel=1;

        public float SFXAudioLevel => _sfxAudioLevel;
        
        [RuntimeInitializeOnLoadMethod]
        private static void LoadAudioManager()
        {
            Instantiate(Resources.Load("AudioManager"));
        }
        
        public void SetupMenuAudio()
        {
            _bgSource.clip = menuMusic;
            _bgSource.loop = true;
            _bgSource.Play();
            
            _ambianceSource.clip = menuAmbience;
            _ambianceSource.loop = true;
            _ambianceSource.Play();
        }
        
        public void SetupMatchAudio()
        {
            _bgSource.clip = matchMusic;
            _bgSource.loop = true;
            _bgSource.Play();
            
            _ambianceSource.clip = matchAmbience;
            _ambianceSource.loop = true;
            _ambianceSource.Play();
        }
        
        private void PlaySFXClip(AudioClip clip)
        {
            var src = new GameObject($"SFX Clip {clip.name}").AddComponent<AudioSource>();
            src.volume = _sfxAudioLevel;
            src.transform.parent = this.transform;
            src.outputAudioMixerGroup = _mixer.FindMatchingGroups("Master/SFX")[0];
            src.PlayOneShot(clip);
            Destroy(src.gameObject, clip.length);
        }
        
        private void PlayVOClip(AudioClip clip)
        {
            var src = new GameObject($"VO Clip {clip.name}").AddComponent<AudioSource>();
            src.volume = _voAudioLevel;
            src.transform.parent = this.transform;
            src.outputAudioMixerGroup = _mixer.FindMatchingGroups("Master/VO")[0];
            src.PlayOneShot(clip);
            Destroy(src.gameObject, clip.length);
        }

        public void PlayMyGoalVO()
        {
            PlaySFXClip(myGoalVo[Random.Range(0, myGoalVo.Length)]);
        }

        public void PlayTheirGoalVO()
        {
            PlaySFXClip(theirGoalVo[Random.Range(0, theirGoalVo.Length)]);
        }

        public void PlayCheerOnGoal()
        {
            PlaySFXClip(cheerOnGoal);
        }
        
        public void PlayWinVO()
        {
           PlayVOClip(winVo[Random.Range(0, winVo.Length)]);
        }
        
        public void PlayLoseVO()
        {
            PlayVOClip(loseVo[Random.Range(0, loseVo.Length)]);
        }

        public void HitByBludger()
        {
            PlaySFXClip(hitSfx);
        }

        public void PlayWhistle()
        {
            PlaySFXClip(whistle);
        }

        public void PlayGoalSFX()
        {
            PlaySFXClip(goalScored);
        }

        public void SetMasterAudioLevel(float level)
        {
            _masterLevel = level;
        }
        
        public void SetMusicAudioLevel(float level)
        {
            _bgSource.volume = level * _masterLevel;
        }
        
        public void SetAmbianceAudioLevel(float level)
        {
            _ambianceSource.volume = level * _masterLevel;
        }
        
        public void SetSFXAudioLevel(float level)
        {
            _sfxAudioLevel = level * _masterLevel;
        }
        
        public void SetVOAudioLevel(float level)
        {
            _voAudioLevel = level * _masterLevel;
        }
    }
}