using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
    public class MeleeDamage : MonoBehaviour
    {
        public Melee meleeController;
        Type type = Type.MELEE;
        public int Damage;
        GameObject player, talentUI;
        PlayerStatsManager statsHandler;
        void Start()
        {
            Invoke("FindAfterStart", 0.1f);
            //FindAfterStart();
        }
        public void FindAfterStart()
        {
            meleeController = GetComponentInParent<Melee>();
            player = GameObject.FindWithTag("Player");
            statsHandler = player.GetComponent<CombatController>().GetStatSystem();
            Damage += statsHandler.physicalDamage;
        }


        private void OnTriggerEnter(Collider c)
        {
            if (meleeController.isSlashing)
            {
                if (c.gameObject.GetComponent<Enemies>())
                {
                    c.gameObject.GetComponent<Enemies>().onHit(Damage);
                }
            }
        }



        


        private void OnTriggerExit(Collider c)
        {
            if (meleeController.isSlashing && c.gameObject.tag != "WindBlade")
            {
                if (c.gameObject.GetComponent<Enemies>())
                {
                    c.gameObject.GetComponent<Enemies>().onHit(Damage);
                }
            }
        }
    }
}
