using System;
using Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace UI
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private GameObject settingsMenu;
        [SerializeField] private Toggle ppToggle;
        [SerializeField] private Toggle fullScreenToggle;
        [SerializeField] private TMPro.TMP_Dropdown resoultionDropDown;
        [SerializeField] private Slider masterAudioSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider ambianceSlider;
        [SerializeField] private Slider voSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button closeButton;
        
        private int _currPPSetting;
        private int _currFullScreenSetting;
        private int _currResoultion;
        private float _currMasterAudioLevel;
        private float _currMusicAudioLevel;
        private float _currAmbianceAudioLevel;
        private float _currSfxAudioLevel;
        private float _currVoAudioLevel;

        public Action OnSettingsClosed;
        
        private void Start()
        {
            closeButton.onClick.AddListener(() =>
            {
                OnSettingsClosed?.Invoke();
                settingsMenu.SetActive(false);
            });
            
            ppToggle.onValueChanged.AddListener(OnPPSettingChanged);
            fullScreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
            resoultionDropDown.onValueChanged.AddListener(OnResoultionChanged);
            masterAudioSlider.onValueChanged.AddListener(OnMasterAudioChanged);
            musicSlider.onValueChanged.AddListener(OnMusicAudioChanged);
            ambianceSlider.onValueChanged.AddListener(OnAmbianceAudioChanged);
            voSlider.onValueChanged.AddListener(OnVOAudioChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXAudioChanged);
            
            _currPPSetting = PlayerPrefs.GetInt("Settings.EnablePP", 1);
            _currFullScreenSetting = PlayerPrefs.GetInt("Settings.Fullscreen", 1);
            _currResoultion = PlayerPrefs.GetInt("Settings.ResolutionId", 1);
            _currMasterAudioLevel = PlayerPrefs.GetFloat("Settings.MasterAudio", 1);
            _currMusicAudioLevel = PlayerPrefs.GetFloat("Settings.MusicAudio", 1f);
            _currAmbianceAudioLevel = PlayerPrefs.GetFloat("Settings.AmbianceAudio", 1f);
            _currSfxAudioLevel = PlayerPrefs.GetFloat("Settings.SFXAudio", 1f);
            _currVoAudioLevel = PlayerPrefs.GetFloat("Settings.VoiceOverAudio", 1f);

            resoultionDropDown.value = _currResoultion;
            ppToggle.isOn = _currPPSetting == 1;
            fullScreenToggle.isOn = _currFullScreenSetting == 1;
            
            masterAudioSlider.value = _currMasterAudioLevel;
            musicSlider.value = _currMusicAudioLevel;
            ambianceSlider.value = _currAmbianceAudioLevel;
            voSlider.value = _currVoAudioLevel;
            sfxSlider.value = _currSfxAudioLevel;
        }

        private void OnPPSettingChanged(bool change)
        {
            var val = change? 1 : 0;
            PlayerPrefs.SetInt("Settings.EnablePP", val);
            PlayerPrefs.Save();
            
            _currPPSetting = val;
            FindObjectOfType<PostProcessVolume>().enabled = change;
        }
        
        private void OnFullscreenToggled(bool change)
        {
            var val = change? 1 : 0;
            PlayerPrefs.SetInt("Settings.Fullscreen", val);
            PlayerPrefs.Save();
            _currFullScreenSetting = val;
            Screen.fullScreen = change;
        }
        
        private void OnResoultionChanged(int id)
        {
            PlayerPrefs.SetInt("Settings.ResolutionId", id);
            PlayerPrefs.Save();
            _currFullScreenSetting = id;
            bool fullscreen = _currFullScreenSetting == 1;
            switch (id)
            {
                case 0: Screen.SetResolution(1280, 720, fullscreen);
                    break;
                case 1: Screen.SetResolution(1920, 1080, fullscreen);
                    break;
            }
        }
        
        private void OnMasterAudioChanged(float value)
        {
            _currMasterAudioLevel = value;
            PlayerPrefs.SetFloat("Settings.MasterAudio", value);
            PlayerPrefs.Save();
            AudioManager.Instance.SetMasterAudioLevel(value);
            OnMusicAudioChanged(_currMusicAudioLevel);
            OnAmbianceAudioChanged(_currAmbianceAudioLevel);
            OnVOAudioChanged(_currVoAudioLevel);
            OnSFXAudioChanged(_currSfxAudioLevel);
        }
        
        private void OnMusicAudioChanged(float value)
        {
            _currMusicAudioLevel = value;
            PlayerPrefs.SetFloat("Settings.MusicAudio", value);
            PlayerPrefs.Save();
            AudioManager.Instance.SetMusicAudioLevel(value);
        }
        
        private void OnAmbianceAudioChanged(float value)
        {
            _currAmbianceAudioLevel = value;
            PlayerPrefs.SetFloat("Settings.AmbianceAudio", value);
            PlayerPrefs.Save();
            AudioManager.Instance.SetAmbianceAudioLevel(value);
        }
        
        private void OnSFXAudioChanged(float value)
        {
            _currSfxAudioLevel = value;
            PlayerPrefs.SetFloat("Settings.SFXAudio", value);
            PlayerPrefs.Save();
            AudioManager.Instance.SetSFXAudioLevel(value);
        }
        
        private void OnVOAudioChanged(float value)
        {
            _currVoAudioLevel = value;
            PlayerPrefs.SetFloat("Settings.SFXAudio", value);
            PlayerPrefs.Save();
            AudioManager.Instance.SetVOAudioLevel(value);
        }

        public void ShowSettings()
        {
            settingsMenu.SetActive(true);
        }
        
    }
}