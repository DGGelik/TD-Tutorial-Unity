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

тимур хочет научиться пользоваться гитом, что ему для этого надо???



Отлично! Вот **Вариант Б** — современная и удобная версия с несколькими группами врагов в одной волне.

### 1. WaveConfig.cs (обновлённая версия)

```csharp
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Tower Defense/Wave Config", order = 1)]
public class WaveConfig : ScriptableObject
{
    [Header("Волны уровня")]
    public List<WaveData> waves = new List<WaveData>();

    [System.Serializable]
    public class WaveData
    {
        [Header("=== Настройки волны ===")]
        [Tooltip("Задержка перед началом этой волны")]
        public float delayBeforeWave = 8f;

        [Header("Группы врагов в этой волне")]
        [Tooltip("Можно добавить несколько разных типов врагов")]
        public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();

        [Header("Дополнительно")]
        [Tooltip("Награда за прохождение волны")]
        public int waveReward = 100;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        [Tooltip("Префаб врага")]
        public GameObject enemyPrefab;

        [Tooltip("Сколько штук спавнить")]
        public int count = 5;

        [Tooltip("Пауза между спавном каждого врага этого типа")]
        public float spawnInterval = 0.8f;

        [Tooltip("Задержка перед началом спавна этой группы (от начала волны)")]
        public float startDelay = 0f;
    }

    // Полезные методы
    public int TotalWaves => waves.Count;

    public bool IsValidWave(int index) => index >= 0 && index < waves.Count;
}
```

---

### 2. WaveSpawner.cs (под новую WaveConfig)

```csharp
using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [Header("Конфигурация волн")]
    public WaveConfig waveConfig;

    [Header("Точка спавна")]
    public Transform spawnPoint;

    private int currentWaveIndex = 0;
    private Coroutine currentWaveCoroutine;

    public void StartNextWave()
    {
        if (!waveConfig.IsValidWave(currentWaveIndex))
        {
            Debug.Log("Все волны завершены!");
            return;
        }

        if (currentWaveCoroutine != null)
            StopCoroutine(currentWaveCoroutine);

        currentWaveCoroutine = StartCoroutine(SpawnWave(currentWaveIndex));
        currentWaveIndex++;
    }

    private IEnumerator SpawnWave(int waveIndex)
    {
        WaveConfig.WaveData wave = waveConfig.waves[waveIndex];

        Debug.Log($"=== Волна {waveIndex + 1} началась ===");

        // Пауза перед началом волны
        yield return new WaitForSeconds(wave.delayBeforeWave);

        // Спавним все группы врагов
        foreach (WaveConfig.EnemyGroup group in wave.enemyGroups)
        {
            if (group.enemyPrefab == null)
            {
                Debug.LogWarning($"В волне {waveIndex + 1} пропущена группа без префаба");
                continue;
            }

            StartCoroutine(SpawnEnemyGroup(group, waveIndex));
        }
    }

    private IEnumerator SpawnEnemyGroup(WaveConfig.EnemyGroup group, int waveIndex)
    {
        // Задержка перед этой группой
        if (group.startDelay > 0f)
            yield return new WaitForSeconds(group.startDelay);

        for (int i = 0; i < group.count; i++)
        {
            Instantiate(group.enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // Пауза между врагами одной группы
            if (i < group.count - 1)
                yield return new WaitForSeconds(group.spawnInterval);
        }
    }

    // === Полезные методы ===
    public int CurrentWaveNumber => currentWaveIndex;
    public int TotalWaves => waveConfig.TotalWaves;
    public bool IsAllWavesCompleted => currentWaveIndex >= waveConfig.TotalWaves;

    public void ResetWaves()
    {
        currentWaveIndex = 0;
        if (currentWaveCoroutine != null)
            StopCoroutine(currentWaveCoroutine);
    }
}
```

### Как настроить в Unity:

1. Удали старый `WaveConfig` asset или создай новый.
2. Открой новый `WaveConfig` в инспекторе.
3. Нажми "+" чтобы добавить волны.
4. В каждой волне нажми "+" чтобы добавить `EnemyGroup`.
5. Назначь `enemyPrefab`, укажи `count`, `spawnInterval` и при желании `startDelay`.

### Пример использования:

```csharp
public class GameManager : MonoBehaviour
{
    public WaveSpawner waveSpawner;

    void Start()
    {
        waveSpawner.StartNextWave();   // начать первую волну
    }

    // Например, когда все враги мертвы:
    public void OnWaveCleared()
    {
        if (!waveSpawner.IsAllWavesCompleted)
            waveSpawner.StartNextWave();
        else
            Debug.Log("Уровень пройден!");
    }
}
```

Готово!

Хочешь, я добавлю ещё:
- События (`UnityEvent`): OnWaveStarted, OnWaveCompleted, OnAllWavesFinished
- Множители сложности (здоровье, скорость)
- Прогресс бар волны
