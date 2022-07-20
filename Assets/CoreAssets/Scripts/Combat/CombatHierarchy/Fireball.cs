using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CMF
{
    public class Fireball : Projectile1
    {
        public int size = 20;
        public float growthSpeed = 100f;
        public Fireball() : base("Fireball", Type.MAGIC)
        {
        }
        void Start()
        {
            transform.Rotate(180, 0, 0);
            base.Start();
            rigid.isKinematic = true;
            Debug.Log(InitialVelocity);
            StartCoroutine(Grow());
        }
        IEnumerator Grow()
        {
            for (int i = 0; i < size; i++)
            {
                Debug.Log(Time.deltaTime);
                this.transform.localScale = this.transform.localScale * 1.05f;
                yield return new WaitForSeconds(0.05f);
            }
            rigid.isKinematic = false;
            rigid.velocity = InitialVelocity;
        }
        private void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
        }
    }
}