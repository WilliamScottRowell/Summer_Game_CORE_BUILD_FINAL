using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CMF
{
 
    // master system. controls DamageDisplay, which is the slave system, during a HPbar pulse.
    public class CombatSystem : MonoBehaviour
    {
        public int attackDamage = 1;
        public float invulnerabilityTime = 0.2f;
        public float knockbackTime = 5f;
        public int whatIsEnemy = 8;
        public LayerMask EnemyLayerMask;

        public PlayerStatsManager stm;

        public GameObject HPText;

        public DamageDisplay dd;

        public int extraLives = 0;


        [SerializeField]
        private bool canBeAttacked = true;

        private bool waiting = false;

        [SerializeField]
        private ModifiedWalkerController AWC;
        // Start is called before the first frame update
        void Start()
        {
            AWC = gameObject.GetComponent<ModifiedWalkerController>();


        }

        // Update is called once per frame
        void Update()
        {

            //HPText.GetComponent<TMPro.TextMeshProUGUI>().text = "HP: " + health;
        }

        public void onHit(int damage)
        {
            if (canBeAttacked && stm.health > 0)
            {
                stm.health -= (int)(Mathf.Clamp((damage - stm.defense), 0, 2147483647));


                dd.StartPulse();
            }
            if (!waiting)
            {

                StartCoroutine(waitCoroutine());
            }
            if (stm.health<= 0)
            {
                if (extraLives > 0)
                {
                    extraLives--;
                    stm.health = stm.maxHealth;
                }
                else
                {
                 // placeholder for death message
                    Debug.Log("health is zero.");
                }
            }

        }

        public void onHeal(int healing)
        {
            stm.health += healing;
            if (stm.health > stm.maxHealth)
            {
                stm.health = stm.maxHealth;
            }

            dd.StartPulse();

        }




        private void OnCollisionEnter(Collision collision)
        {

            if (collision.gameObject.layer == whatIsEnemy)
            {
                JumpEnemies enemy = collision.gameObject.GetComponent<JumpEnemies>();
                


                //Debug.Log(collision.gameObject);
                Vector3 pointOfCollision = collision.GetContact(0).point;
                RaycastHit hit;
                Vector3 normal = new Vector3();
                //Debug.Log(Physics.Raycast(transform.position, pointOfCollision - transform.position, out hit, 20, EnemyLayerMask));
                //Debug.DrawRay(transform.position, pointOfCollision - transform.position, Color.red);
                if (Physics.Raycast(transform.position, pointOfCollision-transform.position, out hit, 20, EnemyLayerMask))
                {
                    
                    normal = hit.normal;
                    //Debug.Log(normal);
                    //StartCoroutine(knockbackCoroutine(normal));
                    transform.position += Vector3.up * 0.2f;
                    AWC.SetMomentum(normal * 10 + Vector3.up * 5);
                    //GetComponent<Rigidbody>().velocity = normal * 10;
                    if (canBeAttacked && stm.health > 0)
                    {
                        stm.health -= enemy.attackDamage;

                        if (dd.enableFade)
                        {
                            dd.StartPulse();
                            Debug.Log("begin pulse");
                        }

                    }
                    if (!waiting)
                    {
                        StartCoroutine(waitCoroutine());
                    }
                    if (stm.health <= 0)
                    {
                        Debug.Log("Health is zero.");
                    }

                }

            }

        }


        IEnumerator waitCoroutine()
        {
            waiting = true;
            canBeAttacked = false;
            yield return new WaitForSeconds(invulnerabilityTime);
            canBeAttacked = true;
            waiting = false;
        }


    }
}