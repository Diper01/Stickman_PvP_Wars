using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameOptions {
    public static Action<bool> SoundChanged;
    public static Action<bool> MusicChanged;
    public static Action<bool> JoystickAttachedChanged;
    public static Action<ControllType> ControllChanged;

    public static bool Sound {
        get { return sound; }
        set {
            sound = value;
            PlayerPrefs.SetInt("Sound", Convert.ToInt32(sound));
            if (SoundChanged != null) SoundChanged(sound);
        }
    }
    public static bool Music
    {
        get { return music; }
        set
        {            
            music = value;
            PlayerPrefs.SetInt("Music", Convert.ToInt32(music));
            if (MusicChanged != null) MusicChanged(music);
        }
    }
    public static bool Vibro
    {
        get { return vibro; }
        set
        {
            vibro = value;
            PlayerPrefs.SetInt("Vibro", Convert.ToInt32(vibro));
        }
    }
    public static bool JoystickAttached {
        get { return joystickAttached; }
        set {
            if (joystickAttached != value)
            {
                joystickAttached = value;
                if (JoystickAttachedChanged != null) JoystickAttachedChanged(joystickAttached);
            }
        }
    }
    public static ControllType ControllType
    {
        get { return controllType; }
        set
        {
            controllType = value;
            PlayerPrefs.SetInt("ControllType", (int)controllType);
            if (ControllChanged != null)
            {
                ControllChanged(controllType);
            }
        }
    }
    public static CloudRegionCode SelectedServer {
        get { return selectedServer; }
        set {
            selectedServer = value;
            PlayerPrefs.SetInt("SelectedServer", (int)selectedServer);
        }
    }
   

    private static bool sound;
    private static bool music;
    private static bool vibro;
    private static bool joystickAttached;
    private static CloudRegionCode selectedServer;

    private static ControllType controllType;

    static GameOptions() {
        sound = Convert.ToBoolean(PlayerPrefs.GetInt("Sound", 1));
        music = Convert.ToBoolean(PlayerPrefs.GetInt("Music", 1));
        vibro = Convert.ToBoolean(PlayerPrefs.GetInt("Vibro", 1));
        joystickAttached = true;
        controllType = (ControllType)PlayerPrefs.GetInt("ControllType", 0);
        selectedServer = (CloudRegionCode)PlayerPrefs.GetInt("SelectedServer", (int)CloudRegionCode.none);
    }
}
