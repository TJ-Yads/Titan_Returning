using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// the script manages the save data of the player, this data includes the following
/// what weapons the player has unlocked
/// the current magazine of each weapon is at
/// the total ammo currently available for each weapon
/// </summary>
public class SavePlayerData : MonoBehaviour
{
    //script is used to save player data including each weapons ammo total, mag amount and what weapons are active
    public PlayerController PlayerGunData;
    private int RestartNumber;//used to reset data for new games
    public bool DevMode;
    private void Start()
    {
        RestartNumber = PlayerPrefs.GetInt("Restart");
        GameObject PlayerData1 = GameObject.FindWithTag("Player");
        PlayerGunData = PlayerData1.GetComponent<PlayerController>();
        if (RestartNumber == 1 ^ DevMode == true)
        {
            RestartData();
        }
        LoadPlayer();
    }
    public void RestartData()//resets the data so that weapons are locked and ammo is set to max again
    {
        Debug.Log("Restart");
        //Pistol Data
        PlayerPrefs.SetFloat("Wep1MagTotal", PlayerGunData.WeaponMagCap[0]);
        PlayerPrefs.SetFloat("Wep1AmmoTotal", PlayerGunData.WeaponAmmoCap[0]);
        //Rifle Data
        PlayerPrefs.SetFloat("Wep2MagTotal", PlayerGunData.WeaponMagCap[1]);
        PlayerPrefs.SetFloat("Wep2AmmoTotal", PlayerGunData.WeaponAmmoCap[1]);
        PlayerPrefs.SetInt("WepUnlock2", 0);
        //Sniper Data
        PlayerPrefs.SetFloat("Wep3MagTotal", PlayerGunData.WeaponMagCap[2]);
        PlayerPrefs.SetFloat("Wep3AmmoTotal", PlayerGunData.WeaponAmmoCap[2]);
        PlayerPrefs.SetInt("WepUnlock3", 0);
        //Rocket Data
        PlayerPrefs.SetFloat("Wep4MagTotal", PlayerGunData.WeaponMagCap[3]);
        PlayerPrefs.SetFloat("Wep4AmmoTotal", PlayerGunData.WeaponAmmoCap[3]);
        PlayerPrefs.SetInt("WepUnlock4", 0);
        //Shotgun Data
        PlayerPrefs.SetFloat("Wep5MagTotal", PlayerGunData.WeaponMagCap[4]);
        PlayerPrefs.SetFloat("Wep5AmmoTotal", PlayerGunData.WeaponAmmoCap[4]);
        PlayerPrefs.SetInt("WepUnlock5", 0);
        //Laser Data
        PlayerPrefs.SetFloat("Wep6MagTotal", PlayerGunData.WeaponMagCap[5]);
        PlayerPrefs.SetFloat("Wep6AmmoTotal", PlayerGunData.WeaponAmmoCap[5]);
        PlayerPrefs.SetInt("WepUnlock6", 0);
        //Cryo Data
        PlayerPrefs.SetFloat("Wep7MagTotal", PlayerGunData.WeaponMagCap[6]);
        PlayerPrefs.SetFloat("Wep7AmmoTotal", PlayerGunData.WeaponAmmoCap[6]);
        PlayerPrefs.SetInt("WepUnlock7", 0);
        PlayerPrefs.SetInt("Restart", 0);
        PlayerGunData.ChangeWeapon(2);
    }
    public void LoadPlayer()//reload player data, used when you enter the game after quitting or when you die
    {
        Debug.Log("Load");
        //Pistol Data
        PlayerGunData.WeaponCurMag[0] = PlayerPrefs.GetFloat("Wep1MagTotal");
        PlayerGunData.WeaponCurCap[0] = PlayerPrefs.GetFloat("Wep1AmmoTotal");
        //Rifle Data
        PlayerGunData.WeaponCurMag[1] = PlayerPrefs.GetFloat("Wep2MagTotal");
        PlayerGunData.WeaponCurCap[1] = PlayerPrefs.GetFloat("Wep2AmmoTotal");
        PlayerGunData.UnlockWeapon(PlayerPrefs.GetInt("WepUnlock2"));
        //Sniper Data
        PlayerGunData.WeaponCurMag[2] = PlayerPrefs.GetFloat("Wep3MagTotal");
        PlayerGunData.WeaponCurCap[2] = PlayerPrefs.GetFloat("Wep3AmmoTotal");
        PlayerGunData.UnlockWeapon(PlayerPrefs.GetInt("WepUnlock3"));
        //Rocket Data
        PlayerGunData.WeaponCurMag[3] = PlayerPrefs.GetFloat("Wep4MagTotal");
        PlayerGunData.WeaponCurCap[3] = PlayerPrefs.GetFloat("Wep4AmmoTotal");
        PlayerGunData.UnlockWeapon(PlayerPrefs.GetInt("WepUnlock4"));
        //Shotgun Data
        PlayerGunData.WeaponCurMag[4] = PlayerPrefs.GetFloat("Wep5MagTotal");
        PlayerGunData.WeaponCurCap[4] = PlayerPrefs.GetFloat("Wep5AmmoTotal");
        PlayerGunData.UnlockWeapon(PlayerPrefs.GetInt("WepUnlock5"));
        //Laser Data
        PlayerGunData.WeaponCurMag[5] = PlayerPrefs.GetFloat("Wep6MagTotal");
        PlayerGunData.WeaponCurCap[5] = PlayerPrefs.GetFloat("Wep6AmmoTotal");
        PlayerGunData.UnlockWeapon(PlayerPrefs.GetInt("WepUnlock6"));
        //Cryo Data
        PlayerGunData.WeaponCurMag[6] = PlayerPrefs.GetFloat("Wep7MagTotal");
        PlayerGunData.WeaponCurCap[6] = PlayerPrefs.GetFloat("Wep7AmmoTotal");
        PlayerGunData.UnlockWeapon(PlayerPrefs.GetInt("WepUnlock7"));
        PlayerGunData.ChangeWeapon(1);
    }
    public void SavePlayer()
    {
        Debug.Log("Save");
        //Pistol Data
        PlayerPrefs.SetFloat("Wep1MagTotal", PlayerGunData.WeaponCurMag[0]);
        PlayerPrefs.SetFloat("Wep1AmmoTotal", PlayerGunData.WeaponCurCap[0]);
        //Rifle Data
        PlayerPrefs.SetFloat("Wep2MagTotal", PlayerGunData.WeaponCurMag[1]);
        PlayerPrefs.SetFloat("Wep2AmmoTotal", PlayerGunData.WeaponCurCap[1]);
        if (PlayerGunData.WeaponButton[1].activeSelf == true)
        {
            PlayerPrefs.SetInt("WepUnlock2", 1);
        }
        //Sniper Data
        PlayerPrefs.SetFloat("Wep3MagTotal", PlayerGunData.WeaponCurMag[2]);
        PlayerPrefs.SetFloat("Wep3AmmoTotal", PlayerGunData.WeaponCurCap[2]);
        if (PlayerGunData.WeaponButton[2].activeSelf == true)
        {
            PlayerPrefs.SetInt("WepUnlock3", 2);
        }
        //Rocket Data
        PlayerPrefs.SetFloat("Wep4MagTotal", PlayerGunData.WeaponCurMag[3]);
        PlayerPrefs.SetFloat("Wep4AmmoTotal", PlayerGunData.WeaponCurCap[3]);
        if (PlayerGunData.WeaponButton[3].activeSelf == true)
        {
            PlayerPrefs.SetInt("WepUnlock4", 3);
        }
        //Shotgun Data
        PlayerPrefs.SetFloat("Wep5MagTotal", PlayerGunData.WeaponCurMag[4]);
        PlayerPrefs.SetFloat("Wep5AmmoTotal", PlayerGunData.WeaponCurCap[4]);
        if (PlayerGunData.WeaponButton[4].activeSelf == true)
        {
            PlayerPrefs.SetInt("WepUnlock5", 4);
        }
        //Laser Data
        PlayerPrefs.SetFloat("Wep6MagTotal", PlayerGunData.WeaponCurMag[5]);
        PlayerPrefs.SetFloat("Wep6AmmoTotal", PlayerGunData.WeaponCurCap[5]);
        if (PlayerGunData.WeaponButton[5].activeSelf == true)
        {
            PlayerPrefs.SetInt("WepUnlock6", 5);
        }
        //Cryo Data
        PlayerPrefs.SetFloat("Wep7MagTotal", PlayerGunData.WeaponCurMag[6]);
        PlayerPrefs.SetFloat("Wep7AmmoTotal", PlayerGunData.WeaponCurCap[6]);
        if (PlayerGunData.WeaponButton[6].activeSelf == true)
        {
            PlayerPrefs.SetInt("WepUnlock7", 6);
        }
    }
}
