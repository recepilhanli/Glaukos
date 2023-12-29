using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoadGameHandler : MonoBehaviour
{
    [SerializeField] int _LoadGameID = 0;
    [SerializeField] List<Entity> _Enemies = new List<Entity>();

    void Start()
    {
        if (LevelManager.isLoadingGame)
        {
            if (LevelManager.LoadIDForEnemies == _LoadGameID)
            {
                foreach (var enemy in _Enemies)
                {
                    if (enemy != null) Destroy(enemy.gameObject);
                }
            }
        }
        Destroy(gameObject);
    }


}
