using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CMF {
    public class SteampunkMiniboss : Enemies
    {
        // Start is called before the first frame update

        public Transform playerTransform;
        public NavMeshAgent agent;
        private Rigidbody rb;

        public GameObject normalAttackProjectile;
        public GameObject barrageProjectile;
        public GameObject rangedAttackProjectile;
        


        // Normal attack: Boss fires a 5 round burst at the player
        // Ranged Attack: Boss fires continuously at the player
        // Barrage attack: Boss fires 40 rounds in 1.5 seconds at the player, in a large spread range
        // close attack: boss swings its melee weapon.
        // summon attack: boss summons 2 summons that can track and attack the player
        private enum BossAttackStates
        {
            normalAttack,
            RangedAttack,
            barrage,
            closeAttack,
            summon
        }


        // Keeping at range: the boss remains at a range from the player, but moves closer to the player when it
        //  cannot see the player. The boss will attempt to move left and right when reaching the range
        // Tracking Player: the boss approaches the player at normal speeds.
        // Dashing: the boss dashes at the player at a high speed.
        // Sieging: the boss stops, and gains a damage bonus.
        private enum BossMovementStates
        {
            keepingAtRange,
            trackingPlayer,
            dashing,
            sieging
        }

        [SerializeField]
        BossAttackStates currentAttackState = BossAttackStates.normalAttack;
        [SerializeField]
        BossMovementStates currentMovementState = BossMovementStates.keepingAtRange;
        IEnumerator currentAttackAction;
        IEnumerator currentMovementAction;

        public float SiegeDamageReductionMultiplier = 0.45f;

        public override void onHit(float damageAmount)
        {   if (!(currentMovementState == BossMovementStates.sieging))
            {
                base.onHit(damageAmount);
            }
            else
            {
                base.onHit(damageAmount * SiegeDamageReductionMultiplier);
            }
        }



        void Start()
        {
            playerTransform = GameObject.Find("Player").transform;
            agent = GetComponent<NavMeshAgent>();
            rb = GetComponent<Rigidbody>();
            health = 250;
            maxHealth = 250;
        }


        private void FixedUpdate()
        {
            FiniteStateMachine();
            HandleMovement();
            HandleAttacks();
        }

        void FiniteStateMachine()
        {
            FSMAttack();
            FSMMovement();


            
        }

        void FSMAttack()
        {
            // range >60: ranged attack
            // 60 >= range > 40: normal attack
            // 40 >= range > 10: barrage
            // 10 >= range > 0: melee

            float range = getDistanceToPlayer();
            if (range > 60)
            {
                currentAttackState = BossAttackStates.RangedAttack;
            }else if (range > 40)
            {
                currentAttackState = BossAttackStates.normalAttack;
            }else if (range > 10)
            {
                currentAttackState = BossAttackStates.barrage;
            }
            else
            {
                currentAttackState = BossAttackStates.closeAttack;
            }

        }
        void FSMMovement()
        {
            // range < 25: dash at player at full speed
            // health > 90%: approach player with 100% speed
            // 90% >= health > 50%: keep at range while moving sideways
            // 50% >= health: siege up and wait for player to come


            float healthPercentage = health / maxHealth * 100;
            float range = getDistanceToPlayer();
            if (range < 50)
            {
                currentMovementState = BossMovementStates.dashing;
            }else if (healthPercentage > 90)
            {
                currentMovementState = BossMovementStates.trackingPlayer;
            }else if (healthPercentage > 50)
            {
                currentMovementState = BossMovementStates.keepingAtRange;
            }
            else
            {
                currentMovementState = BossMovementStates.sieging;
            }
        }

        // this method makes the boss movement correspond to the state
        void HandleMovement()
        {
            switch(currentMovementState){
                case BossMovementStates.dashing:
                    if (!(currentMovementState == BossMovementStates.dashing))
                    {
                        StopCoroutine(currentMovementAction);
                    }
                    
                    
                        currentMovementAction = Dash();
                        StartCoroutine(currentMovementAction);
                    
                    break;
                case BossMovementStates.keepingAtRange:
                    if (!(currentMovementState == BossMovementStates.keepingAtRange))
                    {
                        StopCoroutine(currentMovementAction);
                    }
                    
                    
                        currentMovementAction = KeepAtRange();
                        StartCoroutine(currentMovementAction);
                    
                    break;
                case BossMovementStates.sieging:
                    if (!(currentMovementState == BossMovementStates.sieging))
                    {
                        StopCoroutine(currentMovementAction);
                    }
                    
                    
                        currentMovementAction = Siege();
                        StartCoroutine(currentMovementAction);
                    
                    break;
                case BossMovementStates.trackingPlayer:
                    if (!(currentMovementState == BossMovementStates.trackingPlayer))
                    {
                        StopCoroutine(currentMovementAction);
                    }
                    
                    
                        currentMovementAction = follow();
                        StartCoroutine(currentMovementAction);
                    
                    break;
            }

        }
        void HandleAttacks()
        {
            switch (currentAttackState)
            {
                case BossAttackStates.barrage:
                    if(currentAttackState != BossAttackStates.barrage)
                    {
                        StopCoroutine(currentAttackAction);
                    }
                    currentAttackAction = BarrageAttack();
                    StartCoroutine(currentAttackAction);
                    break;
                    
            }
        }





        bool canPerceivePlayer()
        {

            if (Physics.Raycast(transform.position, playerTransform.position - transform.position, out RaycastHit hit, 2000))
            {
                if (hit.collider.gameObject.GetComponent<Mover>() || hit.collider.gameObject.GetComponentInParent<Mover>())
                {
                    return true;
                }

            }
            return false;
        }
        float getDistanceToPlayer()
        {
            return Mathf.Abs(Vector3.Distance(transform.position, playerTransform.position));

        }
        IEnumerator follow()
        {
            agent.speed = agentNormalSpeed;
            agent.updatePosition = true;
            while (true)
            {
                agent.SetDestination(playerTransform.position);
                yield return new WaitForSeconds(0.2f);
            }
        }


        public float range = 45;
        IEnumerator KeepAtRange()
        {
            agent.speed = agentNormalSpeed;
            agent.updatePosition = true;


            while (true)
            {
                if (Mathf.Abs(Vector3.Distance(transform.position, playerTransform.position)) > range && !canPerceivePlayer())
                {
                    agent.SetDestination(playerTransform.position);
                }

                yield return new WaitForSeconds(0.2f);

            }

        }

        public float dashMultiplier = 2;
        public float agentNormalSpeed = 9;
        IEnumerator Dash()
        {
            agent.updatePosition = true;
            agent.speed = agentNormalSpeed;
            agent.speed *= 2;
            while (true)
            {
                agent.SetDestination(playerTransform.position);
                yield return new WaitForSeconds(0.2f);
            }
            
        }
        IEnumerator Siege()
        {
            agent.speed = 0;
            
            yield return 0;
        }

        // attacks
        IEnumerator NormalAttack()
        {
            while (true)
            {
                
                yield return new WaitForSeconds(0.3f);
            }
        }

        IEnumerator BarrageAttack()
        {



            yield return 0;
        }

        IEnumerator RangedAttack()
        {

            yield return 0;
        }
    }
}
