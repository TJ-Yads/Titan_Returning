using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyKnight : MonoBehaviour//script of one of the enemy types that can be seen in game
{
    //this script controls the AI of the knight
    public NavMeshAgent agent;

    GameObject player;

    public Animator anim;
    public GameObject spawn;

    private float LastFired;
    public float AttackSpeed, SlamCooldown;
    private float ColdModifier = 1;
    public int AttackDMG;
    private bool SlamCooling;
    public GameObject SlamOBJ, AimTarget, SlamSpawn, AttackVis;
    public int Speed;
    public GameObject ColdFX;
    public SphereCollider DetectionRange;
    public GameObject AttackFX;

    //bool when active makes it so the enemy wont move unless there is line of sight with player
    public bool NeedLOS;
    private void Start()//find player and target point
    {
        AimTarget = GameObject.FindGameObjectWithTag("enemyAimPoint");
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && agent.isActiveAndEnabled == true)//if the player enters the detection range then the enemy will move towards the player
        {
            agent.speed = Speed;
        }
        if (other.tag == "bullet1" && agent.isActiveAndEnabled == true)//if the player fires near the enemy then the enemies detetion range greatly increases
        {
            DetectionRange.radius = 70;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && agent.isActiveAndEnabled == true)//while the player is in detection range the enemy will try to hunt and kill the player
        {
            spawn.transform.LookAt(AimTarget.transform.position);
            if (NeedLOS)
            {
                RaycastHit LOS;
                if (Physics.Raycast(spawn.transform.position, spawn.transform.TransformDirection(Vector3.forward), out LOS))
                {
                    Collider HitCollider = player.GetComponent<Collider>();
                    if (LOS.collider == HitCollider)
                    {
                        anim.SetBool("enemyRun", true);
                        agent.SetDestination(player.transform.position);
                    }
                }
            }
            else if (NeedLOS == false)
            {
                anim.SetBool("enemyRun", true);
                agent.SetDestination(player.transform.position);
            }
            //if the enemy distance is at the proper value and the knight is not cooling down then they will start the slam wave attack
            if (Time.time - LastFired > AttackSpeed * ColdModifier && Vector3.Distance(transform.position, player.transform.position) < 17 && Vector3.Distance(transform.position, player.transform.position) > 8)
            {
                SlamSpawn.transform.LookAt(AimTarget.transform.position);
                if (SlamCooling == false)
                {
                    LastFired = Time.time;
                    StartCoroutine(SlamCool());
                }
            }
            //the basic attack of the knight when near the player
            if (Time.time - LastFired > AttackSpeed * ColdModifier && Vector3.Distance(transform.position, player.transform.position) < 4)
            {
                StartCoroutine(BasicAttack());
                LastFired = Time.time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && agent.isActiveAndEnabled == true)//if the player gets away then the enemy will stop fighting and stop moving
        {
            anim.SetBool("enemyRun", false);

            agent.speed = 0;
        }
    }
    public IEnumerator Cold()//a debuff that causes the enemy to slow move and attack speed and activate a freeze FX
    {
        agent.speed = Speed - 4;
        ColdFX.SetActive(true);
        ColdModifier = 1.5f;
        yield return new WaitForSeconds(4f);
        ColdFX.SetActive(false);
        ColdModifier = 1;
        agent.speed = Speed;
    }
    public void HitCold()
    {
        StartCoroutine(Cold());
    }
    //when active it will create the slam and start its cooldown
    IEnumerator SlamCool()//a special attack that works on a cooldown and happens randomly during combat
    {
        agent.speed = 0;
        SlamCooling = true;
        AttackFX.SetActive(true);
        yield return new WaitForSeconds(1f);
        Instantiate(SlamOBJ, SlamSpawn.transform.position, SlamSpawn.transform.rotation);
        yield return new WaitForSeconds(1f);
        AttackFX.SetActive(false);
        agent.speed = 5.5f;
        yield return new WaitForSeconds(SlamCooldown);
        SlamCooling = false;
    }
    //when active it will swipe at the player and deal damage if the player is still nearby
    IEnumerator BasicAttack()//used at all times when the slam is on cooldown
    {
        agent.speed = 0;
        AttackFX.SetActive(true);
        yield return new WaitForSeconds(1f);
        Instantiate(AttackVis, SlamSpawn.transform.position, SlamSpawn.transform.rotation);
        if (Vector3.Distance(transform.position, player.transform.position) < 4)
        {
            GameObject PlayerHit = player.gameObject;
            PlayerHealth HitReact = PlayerHit.GetComponent<PlayerHealth>();
            HitReact.TakingDMG(AttackDMG);
        }
        AttackFX.SetActive(false);
        yield return new WaitForSeconds(1f);
        agent.speed = 5.5f;
    }
}
