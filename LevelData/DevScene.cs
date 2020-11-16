using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DevScene : SuperScene
{
    // Start is called before the first frame update
    void Start()
    {
        this.PlayerHangar = PersistantDataManager.Instance.GetPlayerHangar();
        enemyAI = FindObjectOfType<EnemyAi>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
