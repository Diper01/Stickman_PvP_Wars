using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BotWeaponController : Photon.PunBehaviour, IPunObservable, IWeaponController
{
    public event Action<WeaponType> WeaponChanged;
    public event Action<WeaponType> Fire;

    public GameObject ProjectilePistol;
    public GameObject ProjectileAutomaton;
    public GameObject ProjectileFlame;
    public GameObject ProjectileRocket;
    public GameObject ProjectileLaser;
    public GameObject ProjectileMine;
       
    public GameObject PistolShot;
    public GameObject AutomatonShot;
    public GameObject RocketShot;
    public GameObject FlameShot;
    public GameObject SnipeShot;

    public Transform BulletsSpawnPosition;
    public Transform PistolShotPosition;
    public Transform AutomatonShotPosition;
    public Transform RocketShotPosition;
    public Transform FlameShotPosition;
    public Transform SnipeShotPosition;
    public Transform MineShotPosition;
    public WeaponType CurrentWeapon
    {
        get
        {
            return currentWeapon;
        }
        set
        {
            currentWeapon = value;
            OnWeaponChanged(currentWeapon);
        }
    }
    public float pistolCoolDown = 0.3f;
    public float automatonCoolDown = 0.2f;
    public float flamethrowerCoolDown = 2f;
    public float rocketLauncherCoolDown = 1.3f;
    public float laserCoolDown = 1.3f;
    public float mineCoolDown = 1.5f;

    public PlayerSounds Sounds;

    private int pistolCount;
    private int automatonCount;
    private int flamethrowerCount;
    private int rocketLauncherCount;
    private int laserCount;
    private int mineCount;
    private float coolDown;
    private float currentWeaponCoolDown;  
    private WeaponType currentWeapon;
    private PlayerBot playerBot;

    private void Start()
    {
        playerBot = GetComponent<PlayerBot>();
        ResetWeapon();       
    }

    private void Update()
    {
        coolDown += Time.deltaTime;
    }

    public void ResetWeapon()
    {
        coolDown = 0f;
        currentWeaponCoolDown = pistolCoolDown;
        pistolCount = -1;
        automatonCount = 0;
        flamethrowerCount = 0;
        rocketLauncherCount = 0;
        laserCount = 0;
        mineCount = 0;
        ChangeWeapon(WeaponType.PISTOL);
    }

    public void AddBullets(WeaponType type, int bulletsCount)
    {
        int newBulletsCount = 0;
        switch (type)
        {
            case WeaponType.AUTOMATON:
                automatonCount += bulletsCount;
                newBulletsCount = automatonCount;
                break;
            case WeaponType.FLAMETHROWER:
                flamethrowerCount += bulletsCount;
                newBulletsCount = flamethrowerCount;
                break;
            case WeaponType.ROKET_LAUNCHER:
                rocketLauncherCount += bulletsCount;
                newBulletsCount = rocketLauncherCount;
                break;
            case WeaponType.LASER:
                laserCount += bulletsCount;
                newBulletsCount = laserCount;
                break;
            case WeaponType.MINE:
                mineCount += bulletsCount;
                newBulletsCount = mineCount;
                break;
        }     
    }

    public void ChangeWeapon()
    {
        WeaponType newWeaponType = CurrentWeapon;
        
        while (true)
        {
            if ((int)newWeaponType >= (int)WeaponType.MINE)
            {
                newWeaponType = 0;
            }
            else
            {
                newWeaponType += 1;
            }

            if (GetWeaponCount(newWeaponType) != 0)
            {
                break;
            }
        }

        ChangeWeapon(newWeaponType);

    }

    public void ChangeWeapon(WeaponType type)
    {
        CurrentWeapon = type;      
        SetNewCurrentCoolDown(CurrentWeapon);       
    }

    public void MakeShot(Vector2 direction)
    {
        if (coolDown < currentWeaponCoolDown)
        {
            return;
        }

        if (GetWeaponCount(CurrentWeapon) == 0)
        {
            ChangeWeapon();
            return;
        }

        coolDown = 0;

        Vector2 position = new Vector2();
        switch (CurrentWeapon)
        {
            case WeaponType.PISTOL:
                position = PistolShotPosition.position;
                break;
            case WeaponType.AUTOMATON:
                position = AutomatonShotPosition.position;
                break;
            case WeaponType.FLAMETHROWER:
                position = FlameShotPosition.position;
                break;
            case WeaponType.ROKET_LAUNCHER:
                position = RocketShotPosition.position;
                break;
            case WeaponType.LASER:
                position = SnipeShotPosition.position;
                break;
            case WeaponType.MINE:
                position = MineShotPosition.position;
                break;
        }
        double shotTime = PhotonNetwork.time;
      
        GetComponent<PhotonView>().RPC("Shot", PhotonTargets.All, new object[] { CurrentWeapon, direction, position, shotTime });
    }

    [PunRPC]
    private void Shot(WeaponType weapon, Vector2 direction, Vector2 position, double shotTime)
    {
        switch (weapon)
        {
            case WeaponType.PISTOL:
                MakePistolShot(direction, position, shotTime);
                break;
            case WeaponType.AUTOMATON:
                MakeAutomatonShot(direction, position, shotTime);
                break;
            case WeaponType.FLAMETHROWER:
                MakeFlameShot(direction, position, shotTime);
                break;
            case WeaponType.ROKET_LAUNCHER:
                MakeRocketShot(direction, position, shotTime);
                break;
            case WeaponType.LASER:
                MakeLaserShot(direction, position, shotTime);
                break;
            case WeaponType.MINE:
                MakeMineShot(direction, position, shotTime);
                break;
        }

        if (photonView.isMine)
        {
            if (GetWeaponCount(CurrentWeapon) == 0)
            {
                ChangeWeapon();
            }
        }
    }

    private void MakePistolShot(Vector2 direction, Vector2 position, double shotTime)
    {
        Sounds.PlayClip(Sounds.ClipPistol);
        GameObject pistolBullet = GameObject.Instantiate(ProjectilePistol);
        pistolBullet.GetComponent<ProjectilePistol>().LaunchProjectile(direction, position, shotTime, playerBot.PlayerId, playerBot.Team);
        GameObject shot = GameObject.Instantiate(PistolShot, this.transform);
        shot.transform.position = position;
        OnFire(WeaponType.PISTOL);
    }

    private void MakeAutomatonShot(Vector2 direction, Vector2 position, double shotTime)
    {
        Sounds.PlayClip(Sounds.ClipAutomaton);
        GameObject automatonBullet = GameObject.Instantiate(ProjectileAutomaton);
        automatonBullet.GetComponent<ProjectileAutomaton>().LaunchProjectile(direction, position, shotTime, playerBot.PlayerId, playerBot.Team);
        automatonCount--;
        GameObject shot = GameObject.Instantiate(AutomatonShot, this.transform);
        shot.transform.position = position;
        OnFire(WeaponType.AUTOMATON);
    }

    private void MakeFlameShot(Vector2 direction, Vector2 position, double shotTime)
    {
        Sounds.PlayClip(Sounds.ClipFlame);
        GameObject flameBullet = GameObject.Instantiate(ProjectileFlame);
        flameBullet.GetComponent<ProjectileFlame>().LaunchProjectile(direction, position, shotTime, playerBot.PlayerId, playerBot.Team);
        flameBullet.transform.parent = this.transform;
        flamethrowerCount--;
        OnFire(WeaponType.FLAMETHROWER);
    }

    private void MakeRocketShot(Vector2 direction, Vector2 position, double shotTime)
    {
        Sounds.PlayClip(Sounds.ClipRocket);
        GameObject rocketBullet = GameObject.Instantiate(ProjectileRocket);
        rocketBullet.GetComponent<ProjectileRocket>().LaunchProjectile(direction, position, shotTime, playerBot.PlayerId, playerBot.Team);
        rocketLauncherCount--;
        GameObject shot = GameObject.Instantiate(RocketShot, this.transform);
        shot.transform.position = position;
        OnFire(WeaponType.ROKET_LAUNCHER);
    }

    private void MakeLaserShot(Vector2 direction, Vector2 position, double shotTime)
    {
        Sounds.PlayClip(Sounds.ClipLaser);
        GameObject laserBullet = GameObject.Instantiate(ProjectileLaser);
        laserBullet.GetComponent<ProjectileLaser>().LaunchProjectile(direction, position, shotTime, playerBot.PlayerId, playerBot.Team);
        laserCount--;
        GameObject shot = GameObject.Instantiate(SnipeShot, this.transform);
        shot.transform.position = position;
        OnFire(WeaponType.LASER);
    }

    private void MakeMineShot(Vector2 direction, Vector2 position, double shotTime)
    {
        Sounds.PlayClip(Sounds.ClipMine);
        GameObject mine = GameObject.Instantiate(ProjectileMine);       
        mine.GetComponent<ProjectileMine>().LaunchProjectile(direction, position, shotTime, playerBot.PlayerId, playerBot.Team);
        mineCount--;
        OnFire(WeaponType.MINE);
    }


    private void SetNewCurrentCoolDown(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.PISTOL:
                currentWeaponCoolDown = pistolCoolDown;
                break;
            case WeaponType.AUTOMATON:
                currentWeaponCoolDown = automatonCoolDown;
                break;
            case WeaponType.FLAMETHROWER:
                currentWeaponCoolDown = flamethrowerCoolDown;
                break;
            case WeaponType.ROKET_LAUNCHER:
                currentWeaponCoolDown = rocketLauncherCoolDown;
                break;
            case WeaponType.LASER:
                currentWeaponCoolDown = laserCoolDown;
                break;
            case WeaponType.MINE:
                currentWeaponCoolDown = mineCoolDown;
                break;
        }
    }

    private int GetWeaponCount(WeaponType weaponType)
    {
        int count;

        switch (weaponType)
        {
            case WeaponType.PISTOL:
                count = pistolCount;
                break;
            case WeaponType.AUTOMATON:
                count = automatonCount;
                break;
            case WeaponType.FLAMETHROWER:
                count = flamethrowerCount;
                break;
            case WeaponType.ROKET_LAUNCHER:
                count = rocketLauncherCount;
                break;
            case WeaponType.LASER:
                count = laserCount;
                break;
            case WeaponType.MINE:
                count = mineCount;
                break;
            default:
                count = pistolCount;
                break;
        }

        return count;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(CurrentWeapon);
        }
        else
        {
            WeaponType newType = (WeaponType)stream.ReceiveNext();
            CurrentWeapon = newType;
        }
    }


    private void OnFire(WeaponType weapon)
    {
        if (Fire != null)
        {
            Fire(weapon);
        }
    }

    private void OnWeaponChanged(WeaponType weapon)
    {
        if (WeaponChanged != null)
        {
            WeaponChanged(weapon);
        }
    }
}
