using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilityController : MonoBehaviour//this script allows the player to use abilites on a cooldown
{
    private Rigidbody rb;
    public Transform AbilitySpawn;
    //all abilites use the cooldown feature while only some use a OBJ if it doesnt use an OBJ use [0]
    private float CurrentAbCoolDown;
    private GameObject CurrentAbOBJ;
    //cooldown order is heal, grenade, dash, hunterkiller and gravity well
    public float[] AbilityCooldowns;
    public GameObject[] AbilityOBJs;
    public int DodgePower;
    //data used for UI of active ability
    public string[] AbilityNames;
    public Text CurrentAbility;

    private bool OnCooldown, GrenadeDown, DodgeDown;
    private int LastAbility, ActiveAbility;
    public GameObject AbilityWheel, DodgeFullIcon, GrenadeFullIcon, AbilFullIcon, WheelIcon;
    //used for UI of current ability
    public Sprite[] AbilityLogo;
    public Image CurrentLogo, DodgeVis, GrenadeVis, AbilityVis, AbilLogo2, AbilityVisLarge, AbilLogoLarge;

    //
    //Hayden
    public GameObject dashFX, HealFX;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ChangeAbility(1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.timeScale > 0)
        {
            if (Input.GetButtonDown("UseAbility") && OnCooldown == false)//if the ability is not cooling and the player uses it then this plays
            {

                OnCooldown = true;
                //activates heal
                if (ActiveAbility == 1)//abilites work by an Int value for tracking along with a CurrentOBJ that changes based on the select ability in the weapon wheel
                {
                    //ability 1 is a heal
                    StartCoroutine(HealingFX());
                }
                //activates dodge
                if (ActiveAbility == 3)//ability num 3 is a dodge by applying force in a direction
                {
                    rb.AddForce(rb.velocity.normalized * DodgePower, ForceMode.VelocityChange);
                }
                //throws other abilites like grenade, gravity or hunter
                else
                {
                    Instantiate(CurrentAbOBJ, AbilitySpawn.position, AbilitySpawn.rotation);//any other number will create a object based on the set ability object
                }
                AbilityVis.fillAmount = 1;
                AbilityVisLarge.fillAmount = 1;
                StartCoroutine(AbilityCooling());
            }
            if (Input.GetButtonDown("AbilitySwap"))
            {
                ChangeAbility(LastAbility);
            }
            if (Input.GetButtonDown("Grenade") && GrenadeDown == false)//used for the set grenade ability, throw a grenade outward with an arc
            {
                GrenadeVis.fillAmount = 1;
                GrenadeDown = true;
                Instantiate(AbilityOBJs[1], AbilitySpawn.position, AbilitySpawn.rotation);
                StartCoroutine(GrenadeCooling());
            }
            if (Input.GetButtonDown("Dodge") && DodgeDown == false)//used for the set dodge ability, apply force based on move direction
            {
                DodgeDown = true;
                DodgeVis.fillAmount = 1;
                rb.AddForce(rb.velocity * DodgePower,ForceMode.Impulse);
                StartCoroutine(DodgeCooling());
            }
        }
        //these functions while have a visual cooldown circle when said ability is used, each one has a cooldown circle that fills with time
        if (OnCooldown == true)//prevents ability use through the use ability button
        {
            WheelIcon.SetActive(true);
            AbilFullIcon.SetActive(true);
            AbilityVis.fillAmount -= 100 / (CurrentAbCoolDown * 50) / 100;
            AbilityVisLarge.fillAmount -= 100 / (CurrentAbCoolDown * 50) / 100;
        }
        if (DodgeDown == true)//prevent dodge while cooling
        {
            DodgeFullIcon.SetActive(true);
            DodgeVis.fillAmount -= 100 / (AbilityCooldowns[2] * 50) / 100;
        }
        if (GrenadeDown == true)//prevent grenades while cooling
        {
            GrenadeFullIcon.SetActive(true);
            GrenadeVis.fillAmount -= 100 / (AbilityCooldowns[1] * 50) / 100;
        }

        
    }
    public void ChangeAbility(int AbVal)//changes the players active ability and relative data such as logo, cooldown time and ability value
    {
        //data is still available for grenade and dash but is no longer used
        //both abilites use there own button
        LastAbility = ActiveAbility;
        //heal
        if (AbVal == 1)
        {
            ActiveAbility = 1;
            CurrentAbCoolDown = AbilityCooldowns[0];
            CurrentAbOBJ = AbilityOBJs[0];
            CurrentLogo.sprite = AbilityLogo[0];
            CurrentAbility.text = "" + AbilityNames[0];
            AbilLogo2.sprite = AbilityLogo[0];
        }
        //grenade
        if (AbVal == 2)
        {
            ActiveAbility = 2;
            CurrentAbCoolDown = AbilityCooldowns[1];
            CurrentAbOBJ = AbilityOBJs[1];
            CurrentLogo.sprite = AbilityLogo[1];
            CurrentAbility.text = "" + AbilityNames[1];
            AbilLogo2.sprite = AbilityLogo[1];
        }
        //dash
        if (AbVal == 3)
        {
            ActiveAbility = 3;
            CurrentAbCoolDown = AbilityCooldowns[2];
            CurrentAbOBJ = AbilityOBJs[0];
            CurrentLogo.sprite = AbilityLogo[2];
            CurrentAbility.text = "" + AbilityNames[2];
            AbilLogo2.sprite = AbilityLogo[2];
        }
        //hunter
        if (AbVal == 4)
        {
            ActiveAbility = 4;
            CurrentAbCoolDown = AbilityCooldowns[3];
            CurrentAbOBJ = AbilityOBJs[2];
            CurrentLogo.sprite = AbilityLogo[3];
            CurrentAbility.text = "" + AbilityNames[3];
            AbilLogo2.sprite = AbilityLogo[3];
        }
        //gravity
        if (AbVal == 5)
        {
            ActiveAbility = 5;
            CurrentAbCoolDown = AbilityCooldowns[4];
            CurrentAbOBJ = AbilityOBJs[3];
            CurrentLogo.sprite = AbilityLogo[4];
            CurrentAbility.text = "" + AbilityNames[4];
            AbilLogo2.sprite = AbilityLogo[4];
        }
        AbilLogoLarge.sprite = CurrentLogo.sprite;
    }
    public void AbilitySwap()//quick  swap between 2 abilites
    {
        ChangeAbility(LastAbility);
    }
    //these coroutines manage the cooldown of each ability
    IEnumerator AbilityCooling()//couroutine for regular ability cooldowns
    {
        yield return new WaitForSeconds(CurrentAbCoolDown);
        OnCooldown = false;
        AbilityVis.fillAmount = 0;
        AbilFullIcon.SetActive(false);
        WheelIcon.SetActive(false);
    }
    IEnumerator GrenadeCooling()//couroutine for grenade ability cooldown
    {
        yield return new WaitForSeconds(AbilityCooldowns[1]);
        GrenadeDown = false;
        GrenadeVis.fillAmount = 0;
        GrenadeFullIcon.SetActive(false);
    }
    IEnumerator DodgeCooling()//couroutine for dodge ability cooldown
    {
        yield return new WaitForSeconds(AbilityCooldowns[2]);
        DodgeDown = false;
        DodgeVis.fillAmount = 0;
        DodgeFullIcon.SetActive(false);
    }
    IEnumerator HealingFX()//causes the player to heal and begin shield regen while also setting a heal FX
    {
        PlayerHealth Healing = gameObject.GetComponent<PlayerHealth>();
        Healing.RestoreHealth();
        Healing.ShieldCounter = 10;
        Healing.Shield += 10;
        HealFX.SetActive(true);
        yield return new WaitForSeconds(10f);
        HealFX.SetActive(false);
    }
}
