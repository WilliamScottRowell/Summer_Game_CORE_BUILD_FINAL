using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{

    public Melee meleecontroller;

    public float damage = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log(other.gameObject.GetComponent<Enemies>());
        Debug.Log(other.transform.parent);
        Debug.Log(other.GetComponentInParent<Melee>());

        if (other.gameObject.GetComponent<Enemies>() && meleecontroller.GetComponent<Melee>().isSlashing)
        {
            other.gameObject.GetComponent<Enemies>().onHit(damage);
        }
    }
}
