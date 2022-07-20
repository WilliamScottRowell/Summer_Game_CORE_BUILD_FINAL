using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnHelper : MonoBehaviour
{
    SpawnPoint spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = GameObject.Find("PlayerSpawnPoint").GetComponent<SpawnPoint>();
    }

    private void OnEnable()
    {
        UpdateReferenceToSpawnPoint();
    }

    public void UpdateReferenceToSpawnPoint()
    {
        spawnPoint = GameObject.Find("PlayerSpawnPoint").GetComponent<SpawnPoint>();
    }

    public void Respawn()
    {
        if(spawnPoint != null)
        {
            spawnPoint.RespawnPlayer();
        }
    }
}
