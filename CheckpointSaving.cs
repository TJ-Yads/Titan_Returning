using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSaving : MonoBehaviour//used for saving checkpoints at various areas of the game to restart when you died
{
    //script is used to save and load the players checkpoint when start, continuing or respawning
    //script will place the player at a checkpoint and remove any enemies behind it 
    public GameObject[] CheckPoints, EnemyGroups;
    public Transform SpawnPoint;
    public GameObject Player;
    public int ActiveCheckPoint, ActiveEnemyGroup, ActiveSceneVal;
    private bool DespawnActive;
    public SaveStats StatSaver;
    public SavePlayerData PlayerSaver;
    public int LastCheckPoint;
    public bool DevMode;
    public GameObject EnemyGroup;

    // Start is called before the first frame update
    void Start()
    {
        if (DevMode)
        {
            PlayerPrefs.SetInt("EnemyGroups", 0);
            PlayerPrefs.SetInt("SpawnPoint", 0);
        }
        LoadCheckPoint();
    }
    //respawn button places the player back at the last checkpoint
    public void Respawn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(ActiveSceneVal);
    }
    //positions the player at the checkpoint
    public void PositionPlayer()
    {
        SpawnPoint = CheckPoints[ActiveCheckPoint].transform;
        Player.transform.position = SpawnPoint.transform.position;
    }
    //saves checkpoint data
    public void SaveCheckPoint()
    {
        if (ActiveCheckPoint != LastCheckPoint)
        {
            StatSaver.SaveNewStats();
            PlayerPrefs.SetInt("EnemyGroups", ActiveEnemyGroup);
            PlayerPrefs.SetInt("SpawnPoint", ActiveCheckPoint);
            PlayerPrefs.SetInt("ActiveScene", ActiveSceneVal);
            PlayerPrefs.Save();
            StartCoroutine(ForwardSpawner());
            PlayerSaver.SavePlayer();
        }
    }
    //loads checkpoint data
    public void LoadCheckPoint()
    {
        ActiveEnemyGroup = PlayerPrefs.GetInt("EnemyGroups");
        ActiveCheckPoint = PlayerPrefs.GetInt("SpawnPoint");
        PositionPlayer();
        StartCoroutine(ForwardSpawner());
    }
    //alternative to despawner this will take the next group and spawn them in instead
    public IEnumerator ForwardSpawner()
    {
        yield return new WaitForSeconds(.1f);
        Instantiate(EnemyGroup);
        //EnemyGroups[ActiveEnemyGroup].SetActive(true);
        yield return new WaitForSeconds(.1f);
    }
    //despawner will remove enemies behind the player
    public IEnumerator Despawner()
    {
        DespawnActive = true;
        int T = -1;
        while (DespawnActive)
        {
            for (int i = 0; i < ActiveEnemyGroup; i++)
            {
                T += 1;
                EnemyGroups[i].SetActive(false);
            }
            if (T >= ActiveEnemyGroup)
            {
                DespawnActive = false;
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
