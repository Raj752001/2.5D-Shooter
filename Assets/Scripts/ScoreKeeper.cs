using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; protected set; }
    float lastEnemyKillTime;
    int streakCount;
    float streakExpiryTime = 1;


    void Start()
    {
        score = 0;
        Enemy.OnDeathStatic += OnEnemyKill;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyKill() {

        if(Time.time < lastEnemyKillTime + streakExpiryTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
        }

        lastEnemyKillTime = Time.time;

        score += 4 + (int)Mathf.Pow(2,streakCount);
    }

    void OnPlayerDeath() {
        Enemy.OnDeathStatic -= OnEnemyKill;
    }
    
}
