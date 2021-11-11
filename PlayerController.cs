using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour//primary controls for the player including aiming, movement and weapon mechanics
{
    //data used for raycast target point
    public RaycastHit target;
    public Vector3 Targeter;
    public GameObject Gun, CurrentGun;
    //data used for a variety of firing effects
    public float shotForce;
    private float lastFired;
    private bool reloaded, SemiAuto;
    private bool Paused;
    //FX OBJs and firing spawn points
    //bullet spawns are used for firing a bullet out of the correct gun barrel
    public GameObject BulletSpawn1, BulletSpawn2, NoAimPoint, AbilitySpawnOBJ;
    public GameObject spawn2;
    public GameObject bullet;
    public GameObject shellFX;
    public GameObject magFX;

    //text objs
    public Text ammoText, GunName;

    //camera objs used for zooming and menu controls
    public CMF.CameraController CameraController;
    public CMF.CameraMouseInput AimAssistant;
    public Camera PlayerCam;

    //current bullet is the active bullet type the player will fire along with active FX of the bullet and bullet spawn point
    private GameObject CurrentBullet, CurrentFireFX, ActiveBulletSpawn;
    //current fire rate is the fire rate of the currently in use weapon
    private float CurrentFireRate;
    //list of current values below tracks the players equiped weapons ammo capacity, mag capacity, current magazine and current ammo count
    private float CurrentAmmoCap, CurrentMagCap, CurrentMag, CurrentCapacity, CurrentFOV, CurrentReloadSpeed;
    //bullet list holds all bullet type prefabs in order from pistol, rifle,sniper, rocket, shotgun, laser, cryo
    public GameObject[] BulletList, GunList, FireFXList;
    //values below track each weapons ammo cap, mag cap, current mag size and current bullet count and uses the same list order as the bulletlist
    //values set in inspector
    //cur mag and cur cap are loaded in when the player starts or continues the game
    public float[] WeaponAmmoCap, WeaponMagCap, WeaponCurMag, WeaponCurCap, WepReloadSpeed;
    //active weapon holds which weapon is currently in use and follows the order for bulletlist
    public int ActiveWeapon;
    //last weapon holds the value of activeweapon before a swap so the player can quick swap between them
    private int LastWeapon;
    //object that will  allow weapon swapping
    public GameObject WeaponWheel, AbilityWheel;
    //used for UI of current weapon
    public GameObject NoAmmoLogo;
    public Sprite[] GunLogo;
    public Image CurrentLogo;
    public GameObject[] WeaponButton;
    public GameObject ReticleRed, ReticleWhite;
    //used for game testing while active all weapons are available
    public bool DevMode;

    private void Start()
    {
        if (DevMode)
        {
            UnlockWeapon(0);
            UnlockWeapon(1);
            UnlockWeapon(2);
            UnlockWeapon(3);
            UnlockWeapon(4);
            UnlockWeapon(5);
            UnlockWeapon(6);
        }
        reloaded = true;
        ChangeWeapon(1);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //Raycast is used for aiming the players gun at targets
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out target);
        if (target.transform == null)
        {
            AbilitySpawnOBJ.transform.LookAt(NoAimPoint.transform);
            ActiveBulletSpawn.transform.LookAt(NoAimPoint.transform);
        }
        else//if there is an aim target such as an enemy it will change the reticle and reduce mouse sensitivity for aiming
        {
            Targeter = (target.point);
            ActiveBulletSpawn.transform.LookAt(Targeter);
            AbilitySpawnOBJ.transform.LookAt(Targeter);
            EnemyHealth Enemy = target.collider.GetComponentInChildren<EnemyHealth>();
            Turrets Turret = target.collider.GetComponentInParent<Turrets>();
            TargetHealth Boss = target.collider.GetComponentInParent<TargetHealth>();
            if (Enemy != null)
            {
                AimAssistant.mouseSensitivity = .4f;
                ReticleWhite.SetActive(false);
                ReticleRed.SetActive(true);
            }
            else if (Boss != null)
            {
                AimAssistant.mouseSensitivity = .4f;
                ReticleWhite.SetActive(false);
                ReticleRed.SetActive(true);
            }
            else if (Turret != null)
            {
                AimAssistant.mouseSensitivity = .4f;
                ReticleWhite.SetActive(false);
                ReticleRed.SetActive(true);
            }
            else
            {
                AimAssistant.mouseSensitivity = 1;
                ReticleWhite.SetActive(true);
                ReticleRed.SetActive(false);
            }
        }
        Debug.DrawRay(ActiveBulletSpawn.transform.position, ActiveBulletSpawn.transform.TransformDirection(Vector3.forward) * target.distance, Color.yellow);
        //manages the weapon wheel uses Q to open when held the game is paused and resumes on release
        if (Input.GetButtonDown("Weapons"))//open weapon wheel
        {
            Paused = true;
            CameraController.enabled = false;
            WeaponWheel.SetActive(true);
            AbilityWheel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        if (Input.GetButtonUp("Weapons"))
        {
            CameraController.enabled = true;
            Paused = false;
            WeaponWheel.SetActive(false);
            AbilityWheel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
        if (Paused == false && Time.timeScale > 0)
        {
            //manual reload uses R
            if (Input.GetButtonDown("Reload"))
            {
                StartCoroutine(Reload());
                reloaded = false;
            }
            //manages quick swapping of two weapons uses 1
            if (Input.GetButtonDown("Quickswap"))
            {
                ChangeWeapon(LastWeapon);
            }
            if (Input.GetButtonDown("Fire1")&& SemiAuto == true)//semi auto forces weapons to be clicked for each projectile fired
            {

                if (CurrentMag < 1 && reloaded == true)
                {
                    reloaded = false;
                    StartCoroutine(Reload());
                }
                if (reloaded)
                {
                    if (Time.time - lastFired > CurrentFireRate && CurrentMag > 0)//fire your weapon which will cause FX, projectiles and ammo changes based on the current weapon
                    {
                        lastFired = Time.time;

                        GameObject fx2 = Instantiate(CurrentFireFX, ActiveBulletSpawn.transform.position, ActiveBulletSpawn.transform.rotation);
                        GameObject fx3 = Instantiate(shellFX, spawn2.transform.position, spawn2.transform.rotation);

                        GameObject bullets = Instantiate(CurrentBullet, ActiveBulletSpawn.transform.position, ActiveBulletSpawn.transform.rotation);
                        CurrentMag -= 1;
                        ammoText.text = CurrentMag + "/" + CurrentCapacity;
                        WeaponCurMag[ActiveWeapon - 1] = CurrentMag;

                        Destroy(fx2, 5f);
                        Destroy(fx3, 5f);
                        Destroy(bullets, 5f);

                        if (CurrentMag < 1)
                        {
                            GameObject magFX1 = Instantiate(magFX, spawn2.transform.position, spawn2.transform.rotation);
                            Destroy(magFX1, 5f);
                        }
                    }
                }
            }
            if (Input.GetButton("Fire1")&& SemiAuto == false)//allows the weapon to fire while the button is held down
            {
               
                if (CurrentMag < 1 && reloaded == true)
                {
                    reloaded = false;
                    StartCoroutine(Reload());
                }
                if (reloaded)
                {
                    if (Time.time - lastFired > CurrentFireRate && CurrentMag > 0)
                    {
                        lastFired = Time.time;

                        GameObject fx2 = Instantiate(CurrentFireFX, ActiveBulletSpawn.transform.position, ActiveBulletSpawn.transform.rotation);
                        GameObject fx3 = Instantiate(shellFX, spawn2.transform.position, spawn2.transform.rotation);

                        GameObject bullets = Instantiate(CurrentBullet, ActiveBulletSpawn.transform.position, ActiveBulletSpawn.transform.rotation);
                        CurrentMag -= 1;
                        ammoText.text = CurrentMag + "/" + CurrentCapacity;
                        WeaponCurMag[ActiveWeapon - 1] = CurrentMag;

                        Destroy(fx2, 5f);
                        Destroy(fx3, 5f);
                        Destroy(bullets, 5f);

                        if (CurrentMag < 1)
                        {
                            AudioController.playReload1 = true;
                            GameObject magFX1 = Instantiate(magFX, spawn2.transform.position, spawn2.transform.rotation);
                            Destroy(magFX1, 5f);
                        }
                    }
                }
            }
            if (Input.GetButtonDown("Fire2"))//change FOV for a zooming effect
            {
                PlayerCam.fieldOfView = CurrentFOV;
            }
            if (Input.GetButtonUp("Fire2"))
            {
                PlayerCam.fieldOfView = 90;
            }
        }
    }

    IEnumerator Reload()//reload a player weapon which will decrease reserves and refill the current magazine
    {
        NoAmmoLogo.SetActive(true);
        AudioController.playReload1 = true;
        yield return new WaitForSeconds(CurrentReloadSpeed);
        if (CurrentMagCap < CurrentCapacity)
        {
            NoAmmoLogo.SetActive(false);
            CurrentCapacity -= CurrentMagCap - CurrentMag;
            CurrentMag = CurrentMagCap;
        }
        else
        {
            NoAmmoLogo.SetActive(false);
            CurrentMag = CurrentCapacity;
            CurrentCapacity = 0;
        }
        WeaponCurCap[ActiveWeapon - 1] = CurrentCapacity;
        WeaponCurMag[ActiveWeapon - 1] = CurrentMag;
        reloaded = true;
        ammoText.text = CurrentMag + "/" + CurrentCapacity;
        StopCoroutine(Reload());
    }
    //method that changes the active weapon and all active weapon variables
    public void ChangeWeapon(int WeaponInt)
    {
        //based off the int recived the player will swap to a certain weapon listed below
        //weapon data uses a current variable for various weapon mechanics, each weapon uses a diffrent variable that will replace the proper current variable
        LastWeapon = ActiveWeapon;
        if (WeaponInt == 1)
        {
            //pistol RPM 200 DPS 2 DMG .85
            ActiveWeapon = 1;
            //CurrentFireRate = .3f;
            CurrentFireRate = .15f;
            CurrentBullet = BulletList[0];
            CurrentAmmoCap = WeaponAmmoCap[0];
            CurrentMagCap = WeaponMagCap[0];
            CurrentMag = WeaponCurMag[0];
            CurrentCapacity = WeaponCurCap[0];
            CurrentLogo.sprite = GunLogo[0];
            CurrentFOV = 60;
            SemiAuto = true;
            CurrentFireFX = FireFXList[0];
            ActiveBulletSpawn = BulletSpawn1;

            CurrentReloadSpeed = WepReloadSpeed[0];
            GunName.text = "9mm Pistol";
            StartCoroutine(GunSwap(0));
        }
        if (WeaponInt == 2)
        {
            //rifle RPM 400 DPS 2.2 DMG .33
            ActiveWeapon = 2;
            CurrentFireRate = .13f;
            CurrentBullet = BulletList[1];
            CurrentAmmoCap = WeaponAmmoCap[1];
            CurrentMagCap = WeaponMagCap[1];
            CurrentMag = WeaponCurMag[1];
            CurrentCapacity = WeaponCurCap[1];
            CurrentLogo.sprite = GunLogo[1];
            CurrentFOV = 50;
            SemiAuto = false;
            CurrentFireFX = FireFXList[1];
            ActiveBulletSpawn = BulletSpawn2;

            CurrentReloadSpeed = WepReloadSpeed[1];
            GunName.text = "FA-Rifle";
            StartCoroutine(GunSwap(1));
        }
        if (WeaponInt == 3)
        {
            //sniper RPM 50 DPS 4.16 DMG 5
            ActiveWeapon = 3;
            CurrentFireRate = .65f;
            CurrentBullet = BulletList[2];
            CurrentAmmoCap = WeaponAmmoCap[2];
            CurrentMagCap = WeaponMagCap[2];
            CurrentMag = WeaponCurMag[2];
            CurrentCapacity = WeaponCurCap[2];
            CurrentLogo.sprite = GunLogo[2];
            CurrentFOV = 30;
            SemiAuto = true;
            CurrentFireFX = FireFXList[2];
            ActiveBulletSpawn = BulletSpawn2;

            CurrentReloadSpeed = WepReloadSpeed[2];
            GunName.text = "PR-Railgun";
            StartCoroutine(GunSwap(2));
        }
        if (WeaponInt == 4)
        {
            //rocket RPM 30 DPS 4 DMG 8
            ActiveWeapon = 4;
            CurrentFireRate = 2;
            CurrentBullet = BulletList[3];
            CurrentAmmoCap = WeaponAmmoCap[3];
            CurrentMagCap = WeaponMagCap[3];
            CurrentMag = WeaponCurMag[3];
            CurrentCapacity = WeaponCurCap[3];
            CurrentLogo.sprite = GunLogo[3];
            CurrentFOV = 50;
            SemiAuto = true;
            CurrentFireFX = FireFXList[3];
            ActiveBulletSpawn = BulletSpawn2;

            CurrentReloadSpeed = WepReloadSpeed[3];
            GunName.text = "HYE-Launcher";
            StartCoroutine(GunSwap(3));
        }
        if (WeaponInt == 5)
        {
            //shotgun RPM 85 DPS 5.6 DMG .45 + 9xprojectile
            ActiveWeapon = 5;
            CurrentFireRate = .7f;
            CurrentBullet = BulletList[4];
            CurrentAmmoCap = WeaponAmmoCap[4];
            CurrentMagCap = WeaponMagCap[4];
            CurrentMag = WeaponCurMag[4];
            CurrentCapacity = WeaponCurCap[4];
            CurrentLogo.sprite = GunLogo[4];
            CurrentFOV = 65;
            SemiAuto = true;
            CurrentFireFX = FireFXList[4];
            ActiveBulletSpawn = BulletSpawn2;

            CurrentReloadSpeed = WepReloadSpeed[4];
            GunName.text = "SG-Bucker";
            StartCoroutine(GunSwap(4));
        }
        if (WeaponInt == 6)
        {
            //laser RPM 1200 DPS 4 DMG .2 + burn
            ActiveWeapon = 6;
            //CurrentFireRate = .05f;
            CurrentFireRate = .0975f;
            CurrentBullet = BulletList[5];
            CurrentAmmoCap = WeaponAmmoCap[5];
            CurrentMagCap = WeaponMagCap[5];
            CurrentMag = WeaponCurMag[5];
            CurrentCapacity = WeaponCurCap[5];
            CurrentLogo.sprite = GunLogo[5];
            CurrentFOV = 55;
            SemiAuto = false;
            CurrentFireFX = FireFXList[5];
            ActiveBulletSpawn = BulletSpawn2;

            CurrentReloadSpeed = WepReloadSpeed[5];
            GunName.text = "HL-Rifle";
            StartCoroutine(GunSwap(5));
        }
        if (WeaponInt == 7)
        {
            //cryo RPM 60 DPS 1-9 DMG 1 + freeze + 9xprojectile
            ActiveWeapon = 7;
            CurrentFireRate = .75f;
            CurrentBullet = BulletList[6];
            CurrentAmmoCap = WeaponAmmoCap[6];
            CurrentMagCap = WeaponMagCap[6];
            CurrentMag = WeaponCurMag[6];
            CurrentCapacity = WeaponCurCap[6];
            CurrentLogo.sprite = GunLogo[6];
            CurrentFOV = 60;
            SemiAuto = true;
            CurrentFireFX = FireFXList[6];
            ActiveBulletSpawn = BulletSpawn2;

            CurrentReloadSpeed = WepReloadSpeed[6];
            GunName.text = "Cryo-Cannon";
            StartCoroutine(GunSwap(6));
        }
        ammoText.text = CurrentMag + "/" + CurrentCapacity;
    }
    public void QuickSwap()//used to swap between 2 weapons
    {
        ChangeWeapon(LastWeapon);
    }

    //replenishes 20% ammo cap for each weapon when ammo is picked up
    public void ReplenishAmmo()
    {

        CurrentCapacity += CurrentAmmoCap / .2f;
        WeaponCurCap[0] += WeaponAmmoCap[0] / .2f;
        WeaponCurCap[1] += WeaponAmmoCap[1] / .2f;
        WeaponCurCap[2] += WeaponAmmoCap[2] / .2f;
        WeaponCurCap[3] += WeaponAmmoCap[3] / .2f;
        WeaponCurCap[4] += WeaponAmmoCap[4] / .2f;
        WeaponCurCap[5] += WeaponAmmoCap[5] / .2f;
        WeaponCurCap[6] += WeaponAmmoCap[6] / .2f;
        if (CurrentCapacity > CurrentAmmoCap)
        {
            CurrentCapacity = CurrentAmmoCap;
        }
        if (WeaponCurCap[0] > WeaponAmmoCap[0])
        {
            WeaponCurCap[0] = WeaponAmmoCap[0];
        }
        if (WeaponCurCap[1] > WeaponAmmoCap[1])
        {
            WeaponCurCap[1] = WeaponAmmoCap[1];
        }
        if (WeaponCurCap[2] > WeaponAmmoCap[2])
        {
            WeaponCurCap[2] = WeaponAmmoCap[2];
        }
        if (WeaponCurCap[3] > WeaponAmmoCap[3])
        {
            WeaponCurCap[3] = WeaponAmmoCap[3];
        }
        if (WeaponCurCap[4] > WeaponAmmoCap[4])
        {
            WeaponCurCap[4] = WeaponAmmoCap[4];
        }
        if (WeaponCurCap[5] > WeaponAmmoCap[5])
        {
            WeaponCurCap[5] = WeaponAmmoCap[5];
        }
        if (WeaponCurCap[6] > WeaponAmmoCap[6])
        {
            WeaponCurCap[6] = WeaponAmmoCap[6];
        }
        ammoText.text = CurrentMag + "/" + CurrentCapacity;
    }
    //gives 1 magazine to a specific weapon based on weapon number entered in
    public void AmmoSpecific(int WeaponNum)
    {
        if (ActiveWeapon == WeaponNum + 1)
        {
            CurrentCapacity += CurrentMagCap;
            if (CurrentCapacity > CurrentAmmoCap)
            {
                CurrentCapacity = CurrentAmmoCap;
            }
        }
        WeaponCurCap[WeaponNum] += WeaponMagCap[WeaponNum];
        if (WeaponCurCap[WeaponNum] > WeaponAmmoCap[WeaponNum])
        {
            WeaponCurCap[WeaponNum] = WeaponAmmoCap[WeaponNum];
        }
        ammoText.text = CurrentMag + "/" + CurrentCapacity;
    }
    //unlocks a weapon on the wheel based on the weapon number
    public void UnlockWeapon(int WeaponNum)
    {
        WeaponButton[WeaponNum].SetActive(true);
    }
    //used for the gun swap animation 
    IEnumerator GunSwap(int WeaponNum)
    {
        NoAmmoLogo.SetActive(true);
        CurrentGun.SetActive(false);
        CurrentGun = GunList[WeaponNum];
        Paused = true;
        yield return new WaitForSeconds(1f);
        CurrentGun.SetActive(true);
        NoAmmoLogo.SetActive(false);
        Paused = false;
    }
}