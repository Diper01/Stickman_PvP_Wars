using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

    [SerializeField] Toggle soundToggle;
    [SerializeField] Toggle musicToggle;
    [SerializeField] Toggle vibroToggle;
    [SerializeField] Dropdown controllDropdown;

    private void Start()
    {
        soundToggle.isOn = GameOptions.Sound;
        musicToggle.isOn = GameOptions.Music;
        vibroToggle.isOn = GameOptions.Vibro;
        controllDropdown.options = new List<Dropdown.OptionData>() {
              new Dropdown.OptionData(Localisation.GetString("Buttons") + " A")
            , new Dropdown.OptionData(Localisation.GetString("Buttons") + " B")
            , new Dropdown.OptionData(Localisation.GetString("Joystick")) };
        controllDropdown.value = (int)GameOptions.ControllType;
    }


    public void OnSoundToggle()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        GameOptions.Sound = soundToggle.isOn;
    }

    public void OnMusicToggle()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        GameOptions.Music = musicToggle.isOn;
    }

    public void OnVibroToggle()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        GameOptions.Vibro = vibroToggle.isOn;
    }

    public void OnControllDropdown()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        GameOptions.ControllType = (ControllType)controllDropdown.value;
    }
}
