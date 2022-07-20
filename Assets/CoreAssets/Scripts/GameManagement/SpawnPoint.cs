using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    GameObject player;
    PlayerSpawnHelper spawnHelper;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    public void RespawnPlayer()
    {
        player.transform.position = gameObject.transform.position;
    }
}
