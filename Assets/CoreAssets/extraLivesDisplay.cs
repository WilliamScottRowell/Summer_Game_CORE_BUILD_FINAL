using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CMF;
public class extraLivesDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<TMPro.TextMeshProUGUI>().text = "Extra Lives: " + GameObject.Find("Player").GetComponent<CombatSystem>().extraLives;
    }
}
