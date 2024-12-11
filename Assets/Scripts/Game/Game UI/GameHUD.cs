using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHUD : MonoBehaviour {
    [SerializeField] GameObject pistolImage;
    [SerializeField] GameObject automatonImage;
    [SerializeField] GameObject flameImage;
    [SerializeField] GameObject rocketImage;
    [SerializeField] GameObject snipeImage;
    [SerializeField] GameObject mineImage;   
    [SerializeField] Text weaponNameText;
    [SerializeField] Text bulletsCountText;
    [SerializeField] Text roundTimerText;

    private int oldRoundTime = 0;

    public void Update()
    {
      
    }

    private void OnEnable() {
        roundTimerText.text = Localisation.GetString("RoundTime") + " " + string.Format("{0}:{1:00}", 0, 0);
        EventManager.WeaponChaged += ChangeWeapon;
        EventManager.BulletsCountChaged += ChangeBulletsCount;
        EventManager.RoundTimerUpdate += OnRoundTimerUpdate;
    }

    private void OnDisable() {
        EventManager.WeaponChaged -= ChangeWeapon;
        EventManager.BulletsCountChaged -= ChangeBulletsCount;
        EventManager.RoundTimerUpdate -= OnRoundTimerUpdate;
    }


    private void ChangeWeapon(WeaponType type) {
        switch (type)
        {
            case WeaponType.PISTOL:
                weaponNameText.text = Localisation.GetString("Pistol");
                pistolImage.SetActive(true);
                automatonImage.SetActive(false);
                flameImage.SetActive(false);
                rocketImage.SetActive(false);
                snipeImage.SetActive(false);
                mineImage.SetActive(false);
                break;
            case WeaponType.AUTOMATON:
                weaponNameText.text = Localisation.GetString("Rifle");
                pistolImage.SetActive(false);
                automatonImage.SetActive(true);
                flameImage.SetActive(false);
                rocketImage.SetActive(false);
                snipeImage.SetActive(false);
                mineImage.SetActive(false);
                break;
            case WeaponType.FLAMETHROWER:
                weaponNameText.text = Localisation.GetString("Flamethrower");
                pistolImage.SetActive(false);
                automatonImage.SetActive(false);
                flameImage.SetActive(true);
                rocketImage.SetActive(false);
                snipeImage.SetActive(false);
                mineImage.SetActive(false);
                break;
            case WeaponType.ROKET_LAUNCHER:
                weaponNameText.text = Localisation.GetString("RocketLauncher");
                pistolImage.SetActive(false);
                automatonImage.SetActive(false);
                flameImage.SetActive(false);
                rocketImage.SetActive(true);
                snipeImage.SetActive(false);
                mineImage.SetActive(false);
                break;
            case WeaponType.LASER:
                weaponNameText.text = Localisation.GetString("Snipe");
                pistolImage.SetActive(false);
                automatonImage.SetActive(false);
                flameImage.SetActive(false);
                rocketImage.SetActive(false);
                snipeImage.SetActive(true);
                mineImage.SetActive(false);
                break;
            case WeaponType.MINE:
                weaponNameText.text = Localisation.GetString("Mines");               
                pistolImage.SetActive(false);
                automatonImage.SetActive(false);
                flameImage.SetActive(false);
                rocketImage.SetActive(false);
                snipeImage.SetActive(false);
                mineImage.SetActive(true);
                break;           
        }
    }

    private void ChangeBulletsCount(int count) {
        if (count < 0)
        {
            bulletsCountText.text = "∞";
        }
        else
        {
            bulletsCountText.text = count.ToString();
        }
    }

    private void OnRoundTimerUpdate(int timeSec)
    {
        if (oldRoundTime != timeSec)
        {
            oldRoundTime = timeSec;
            int minutes = timeSec / 60;
            int seconds = timeSec - minutes * 60;
            roundTimerText.text = Localisation.GetString("RoundTime") + " " + string.Format("{0}:{1:00}", minutes, seconds);
            if (timeSec <= 15)
            {
                roundTimerText.color = new Color(1, 0.3f, 0.2f);
            }
            else
            {
                roundTimerText.color = Color.white;
            }
        }
    }

}
