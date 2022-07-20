using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemies : MonoBehaviour
{
    public float health = 2;
    public float maxHealth = 2;
    public float attackInvulnerabilityTime = 0.2f;
    [SerializeReference]
    public bool canBeAttacked = true;
    [SerializeReference]
    public bool waiting = false;
    public int expGain = 5;

    public LevelingSystem LS;

    // Start is called before the first frame update
    void Start()
    {
        
        Debug.Log(LS);
    }

    // Update is called once per frame

    public IEnumerator waitCoroutine()
    {
        waiting = true;
        canBeAttacked = false;
        yield return new WaitForSeconds(attackInvulnerabilityTime);
        canBeAttacked = true;
        waiting = false;
    }
    
    public virtual void Attack()
    {


    }

    public virtual void onHit(float damageAmount)
    {
        LS = GameObject.Find("StatManager").GetComponent<LevelingSystem>();

        if (canBeAttacked)
            health -= damageAmount;
            StartCoroutine(waitCoroutine());
        
        if (health <= 0)
        {
            // exp gain here
            LS.EarnExperience(expGain);
            Destroy(gameObject);
        }
    }
}
