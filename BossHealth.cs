using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour//the primary script for the boss enemy, this script pass data to other boss scripts to change attack patterns 
{
    public float GensAlive, PipesAlive, MainUnit;
    public GameObject[] Pipes, Gens;
    public GameObject Shield, BossShield, BreakFX;
    public Image HealthBar, ShieldBar;
    public bool BrokeTarget;
    public BossAttacks BAttack;
    public GameObject WinScreen;
    public CMF.CameraController CameraController;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShieldPipes());
    }
    //when a gen is broken this will activate, places shields on them along with cryo pits, more fire pits and possibly the sniper turrets
    public IEnumerator ShieldGenerators()
    {
        if(GensAlive == 2)
        {
            BAttack.SniperStarter();//starts sniper spawns in BossAttacks
        }
        BAttack.SpawnCryo();//in BossAttacks it creates cryo pits that slow and hurt the player
        BAttack.FireCount += 2;//increase the max amount of fire pits the boss can spawn
        BrokeTarget = true;
        HealthShields();//updates the bosses health bar in game
        yield return new WaitForSeconds(1f);
        BrokeTarget = false;
        foreach (GameObject Gen in Gens)//if a generator is still active then place a shield on it
        {
            Instantiate(Shield, Gen.transform);
        }
        if(GensAlive == 0)//if there is no generators then break the main shield
        {
            Instantiate(BreakFX, BossShield.transform.position, BossShield.transform.rotation);
            Destroy(BossShield);
        }
    }
    public IEnumerator ShieldPipes()
    {
        BAttack.TurretTimer -= 12.5f;//reduce the delay for more turrets to respawn
        BAttack.FireCount += 4;//increase the max fire pit counter that can spawn
        BrokeTarget = true;
        HealthShields();
        yield return new WaitForSeconds(1f);
        BAttack.PulseStarter();//start the pulse wave, an attack the fires a large wave around the boss arena
        BrokeTarget = false;
        foreach (GameObject Pipe in Pipes)//place shields on each pipe that is still alive
        {
            Instantiate(Shield, Pipe.transform);
        }
    }
    public void HealthShields()//update the bosses healthbars based on remaining pipes and generators
    {
        HealthBar.fillAmount = PipesAlive / 3;
        ShieldBar.fillAmount = GensAlive / 4;
    }
    public void ShieldGens()
    {
        StartCoroutine(ShieldGenerators());
    }
    public void ShieldPipesM()
    {
        StartCoroutine(ShieldPipes());
    }
    public void WinMethod()
    {
        StartCoroutine(Win());
    }
    public IEnumerator Win()//upon winning slow the game for a moment then disable controls and present win menu
    {
        Time.timeScale = .33f;
        yield return new WaitForSeconds(1.5f);
        CameraController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        WinScreen.SetActive(true);
    }
}
