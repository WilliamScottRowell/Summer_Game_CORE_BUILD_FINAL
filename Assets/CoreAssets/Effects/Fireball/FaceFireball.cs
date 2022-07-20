using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceFireball : MonoBehaviour
{

    public Transform player;

    public bool rotate = true;
    Quaternion playerRotation;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("ModelRoot").GetComponent<Transform>();
        playerRotation = player.rotation;

    }

    // Update is called once per frame
    

    void Update()
    {
        

        if (rotate == true)
        {
            gameObject.transform.rotation = playerRotation;
            rotate = false;
            
        }
    }
}
