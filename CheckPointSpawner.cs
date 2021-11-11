using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointSpawner : MonoBehaviour
{
    //holds data at each checkpoint that is the targets spawnpoint, enemy groups and active scene
    //when triggered it will save this data 
    public int SpawnVal, EnemyVal, SceneVal;
    GameObject CheckPointMaster;
    public GameObject EnemyGroup;
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CheckPointMaster = GameObject.FindGameObjectWithTag("CheckPointSYS");
            CheckpointSaving HitCheck = CheckPointMaster.GetComponent<CheckpointSaving>();
            HitCheck.LastCheckPoint = HitCheck.ActiveCheckPoint;
            HitCheck.ActiveCheckPoint = SpawnVal;
            HitCheck.ActiveEnemyGroup = EnemyVal;
            HitCheck.ActiveSceneVal = SceneVal;
            HitCheck.EnemyGroup = EnemyGroup;
            HitCheck.SaveCheckPoint();
        }
    }
}
