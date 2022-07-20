using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CMF {
    public class HealingEnvironment : MonoBehaviour
    {

        public int healAmount = 1;
        public float healInterval = 1;
        private float nextHeal;
        // Start is called before the first frame update
        void Start()
        {
            nextHeal = Time.time;
        }

        // Update is called once per frame
        private void OnTriggerStay(Collider other)
        {
            if (other.GetComponentInParent<CombatSystem>() && Time.time >= nextHeal)
            {
                other.GetComponentInParent<CombatSystem>().onHeal(healAmount);
                nextHeal = Time.time + healInterval;
            }
        }
    }
}
