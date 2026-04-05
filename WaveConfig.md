using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Tower Defense/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [System.Serializable]
    public class WaveData
    {
        [Header("Настройки волны")]
        [Tooltip("Пауза перед началом этой волны")]
        public float delayBeforeWave = 8f;

        [Tooltip("Список врагов, которые будут в этой волне")]
        public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
    }

    [System.Serializable]
    public class EnemyGroup
    {
        [Tooltip("Какой враг будет спавниться")]
        public GameObject enemyPrefab;

        [Tooltip("Сколько штук этого врага")]
        public int count = 5;

        [Tooltip("Пауза между спавном каждого врага этого типа")]
        public float spawnInterval = 0.8f;
    }

    public WaveData[] waves;
}