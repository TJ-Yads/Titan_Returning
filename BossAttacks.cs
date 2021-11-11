using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttacks : MonoBehaviour//used for the bosses attack patterns and is updated by the BossHealth script
{
    public GameObject TurretLaser, TurretSniper, FirePit, PulseWave, CryoPit;
    public GameObject[] TurrentSpawns, FireSpawns, CryoSpawns;
    GameObject currentPoint;
    GameObject LastPoint;
    public GameObject PulsePrep;
    public Transform PulseSpawn;
    public float FireWait;
    public int FireCount;
    public float TurretTimer;

    int index;
    // Start is called before the first frame update
    void Start()//on start begin coroutines for turrets and firePits
    {
        StartCoroutine(SpawnTurrets());
        StartCoroutine(SpawnFire());
    }
    //while active this will spawn turrets in once the timer ends and only if the spawn is missing a turret, turret timer drops over time as the boss weakens
    public IEnumerator SpawnTurrets()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            if(TurrentSpawns[0].transform.childCount < 1)
            {
                Instantiate(TurretLaser, TurrentSpawns[0].transform.position, TurrentSpawns[0].transform.rotation, TurrentSpawns[0].transform);
            }
            if (TurrentSpawns[1].transform.childCount < 1)
            {
                Instantiate(TurretLaser, TurrentSpawns[1].transform.position, TurrentSpawns[1].transform.rotation, TurrentSpawns[1].transform);
            }
            if (TurrentSpawns[2].transform.childCount < 1)
            {
                Instantiate(TurretLaser, TurrentSpawns[2].transform.position, TurrentSpawns[2].transform.rotation, TurrentSpawns[2].transform);
            }
            if (TurrentSpawns[3].transform.childCount < 1)
            {
                Instantiate(TurretLaser, TurrentSpawns[3].transform.position, TurrentSpawns[3].transform.rotation, TurrentSpawns[3].transform);
            }
            yield return new WaitForSeconds(TurretTimer);
        }
    }
    //while active this will spawn turrets in once the timer ends and only if the spawn is missing a turret, this one activates when 2 gens are broken
    public IEnumerator SpawnSnipers()//this has the same turret respawn timer as above but each turret is a sniper variant with lower fire rate and more damage
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            if (TurrentSpawns[4].transform.childCount < 1)
            {
                Instantiate(TurretSniper, TurrentSpawns[4].transform.position, TurrentSpawns[4].transform.rotation, TurrentSpawns[4].transform);
            }
            if (TurrentSpawns[5].transform.childCount < 1)
            {
                Instantiate(TurretSniper, TurrentSpawns[5].transform.position, TurrentSpawns[5].transform.rotation, TurrentSpawns[5].transform);
            }
            if (TurrentSpawns[6].transform.childCount < 1)
            {
                Instantiate(TurretSniper, TurrentSpawns[6].transform.position, TurrentSpawns[6].transform.rotation, TurrentSpawns[6].transform);
            }
            if (TurrentSpawns[7].transform.childCount < 1)
            {
                Instantiate(TurretSniper, TurrentSpawns[7].transform.position, TurrentSpawns[7].transform.rotation, TurrentSpawns[7].transform);
            }
            yield return new WaitForSeconds(TurretTimer);
        }
    }
    //once a gen is broken cryo is spawned at each gen location
    public void SpawnCryo()//these pits deal damage and slow the player
    {
        foreach (GameObject Spawns in CryoSpawns)
        {
            Instantiate(CryoPit, Spawns.transform.position, Spawns.transform.rotation);
        }
    }
    //spawn fire will spawn fire pits at random location marked on the boss area
    public IEnumerator SpawnFire()//this loop will spawn a set amount of pits based on fireCount and will wait for the delay to spawn more
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            for (int i = 0; i < FireCount; i++)
            {
                index = Random.Range(0, FireSpawns.Length);
                currentPoint = FireSpawns[index];
                if (currentPoint != LastPoint)
                {
                    Instantiate(FirePit, currentPoint.transform);
                    LastPoint = currentPoint;
                }
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(FireWait);
        }
    }
    public void PulseStarter()
    {
        StartCoroutine(SpawnPulse());
    }
    public void SniperStarter()
    {
        StartCoroutine(SpawnSnipers());
    }
    public IEnumerator SpawnPulse()//start the FX for the pulse attack and then create the pulse which fires outwards from the center
    {
        PulsePrep.SetActive(true);
        yield return new WaitForSeconds(3f);
        PulsePrep.SetActive(false);
        Instantiate(PulseWave, PulseSpawn);
    }
}
