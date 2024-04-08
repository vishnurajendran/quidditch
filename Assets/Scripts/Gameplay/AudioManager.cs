using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Gameplay
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        [SerializeField]
        private AudioClip matchMusic;
        [SerializeField]
        private AudioClip ambience;
        [SerializeField]
        private AudioClip cheerOnGoal;
        [SerializeField]
        private AudioClip hitSfx;
        [SerializeField]
        private AudioClip[] myGoalVo;
        [SerializeField]
        private AudioClip[] theirGoalVo;
        [SerializeField]
        private AudioClip winVo;
        [SerializeField]
        private AudioClip loseVo;
        [SerializeField]
        private AudioClip whistle;

        [SerializeField] 
        private AudioClip goalScored;

        [SerializeField] private AudioSource _bgSource;
        [SerializeField] private AudioSource _ambianceSource;
        [SerializeField] private AudioMixer _mixer;
        
        public void SetupMatchAudio()
        {
            _bgSource.clip = matchMusic;
            _bgSource.loop = true;
            _bgSource.Play();
            
            _ambianceSource.clip = ambience;
            _ambianceSource.loop = true;
            _ambianceSource.Play();
        }
        
        private void PlaySFXClip(AudioClip clip)
        {
            var src = new GameObject($"SFX Clip {clip.name}").AddComponent<AudioSource>();
            src.transform.parent = this.transform;
            src.outputAudioMixerGroup = _mixer.FindMatchingGroups("Master/SFX")[0];
            src.PlayOneShot(clip);
            Destroy(src.gameObject, clip.length);
        }
        
        private void PlayVOClip(AudioClip clip)
        {
            var src = new GameObject($"VO Clip {clip.name}").AddComponent<AudioSource>();
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
           PlayVOClip(winVo);
        }
        
        public void PlayLoseVO()
        {
            PlayVOClip(loseVo);
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
    }
}