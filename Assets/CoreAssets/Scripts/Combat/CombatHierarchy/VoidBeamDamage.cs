using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
    public class VoidBeamDamage : MonoBehaviour
    {
        public int Damage = 12;
        GameObject player, talentUI;
        PlayerStatsManager statsHandler;
        void Start()
        {
            player = GameObject.FindWithTag("Player");
            statsHandler = player.GetComponent<CombatController>().GetStatSystem();
            Damage += statsHandler.magicDamage;
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Enemies>())
            {
                other.GetComponent<Enemies>().onHit(Damage);
            }
        }
    }
}
