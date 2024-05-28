using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PoolManager.Instance.GetEnemy(PoolManager.EnemyType.Enemy1);
		PoolManager.Instance.GetEnemy(PoolManager.EnemyType.Enemy20);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
