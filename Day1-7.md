### День 1 — Создание проекта и базовая сцена

1. Запустите Unity Hub  
2. Нажмите **New project**  
3. Выберите шаблон **2D (Core)**  
4. Project name → `TowerDefence2026` (или любое удобное вам название)  
5. Location → создайте папку, например `D:\UnityProjects\`  
6. Нажмите **Create project**

После открытия проекта:

7. В Project окне найдите папку `Assets/Scenes`  
8. Удалите файл `SampleScene.unity` (правой кнопкой → Delete)  
9. File → New Scene → выберите **Basic (Built-in)** → сохраните сцену сразу:  
   `Assets/Scenes/Main.unity` (Ctrl+S)

10. Выделите в Hierarchy объект **Main Camera**  
11. В Inspector измените следующие параметры:

    - **Projection** → Orthographic  
    - **Size** → 5.5  (потом можно будет подвинуть до 6–9)  
    - **Background** → цвет #1A3C1A (тёмно-зелёный фон)

12. File → Save Scene (Ctrl+S)

**Почему так?**  
Orthographic камера — стандарт для 2D-игр (нет перспективы).  
Size 5.5 даёт хороший стартовый масштаб для карты ~30×20 тайлов.

**Результат дня 1**  
Пустая сцена, камера настроена, фон тёмно-зелёный, сцена сохранена как Main.unity.

---

### День 2 — Tilemap + импорт ассетов + первая карта

1. Window → Package Manager  
2. В левом верхнем углу переключите на **Unity Registry**  
3. В поиск введите `2D Tilemap Extras` → Install (если ещё не установлен)

4. Откройте браузер → перейдите на https://kenney.nl/assets/tower-defense-kit  
5. Нажмите **Download** → распакуйте архив куда удобно  
6. В Unity → Project окно → правой кнопкой на **Assets** → **Create → Folder** → назовите `Sprites`  
7. Перетащите из распакованной папки Kenney в Assets/Sprites следующие файлы:

    - все спрайты дорог (road_*.png)  
    - все спрайты травы / земли (grass_*.png или аналогичные)  
    - спрайты врагов и башен (позже)

8. Выделите все перетащенные спрайты → в Inspector:

    - **Pixels Per Unit** → 32  
    - **Filter Mode** → Point (no filter)  
    - **Compression** → None  
    - Нажмите Apply

9. В Hierarchy → правой кнопкой → 2D Object → Tilemap → Rectangular  
   (появится объект Grid и дочерний Tilemap)

10. Переименуйте Tilemap в **Ground**  
11. Снова правой кнопкой на Grid → 2D Object → Tilemap → Rectangular → переименуйте в **Path**

12. Window → 2D → **Tile Palette** (если окно не видно — View → Tile Palette)

13. В Tile Palette нажмите **Create New Palette** → назовите `GroundPalette` → OK  
14. Перетащите в палитру спрайт травы (любой зелёный) → Create Tile → OK  
15. Аналогично создайте **PathPalette** и перетащите туда спрайты дороги

16. Выберите в Tile Palette **GroundPalette** → закрасьте всю видимую область травой (левой кнопкой мыши)  
17. Переключитесь на **PathPalette** → нарисуйте извилистую дорогу:

    - Начало — левый нижний угол  
    - Конец — правый верхний угол  
    - Длина пути ≈ 120–160 тайлов (12–18 поворотов)

**Совет:** держите зажатой левую кнопку и водите — будет рисоваться непрерывно.

**Результат дня 2**  
Карта с травой и извилистой дорогой, нарисованная тайлами.

---

### День 3 — Waypoints

1. В Hierarchy → правой кнопкой → Create Empty → переименуйте в **Waypoints**

2. Выделите Waypoints → правой кнопкой → Create Empty → назовите **WP0**  
   Повторите 15 раз → WP1 … WP15

3. Перетащите все WP0–WP15 внутрь объекта Waypoints (сделайте их дочерними)

4. Включите инструмент **Move** (W)  
5. По очереди выделяйте каждый WP и ставьте его **точно в центр** соответствующего тайла дороги:

    - WP0 — начало пути  
    - WP15 — конец пути

    **Важно:** позиция должна быть кратна 1 по X и Y (или 0.5, если центр тайла смещён)

6. Assets → Create → **ScriptableObject** → назовите `LevelPath.asset`

7. Создайте скрипт `LevelPath.cs`

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "LevelPath", menuName = "TD/LevelPath")]
public class LevelPath : ScriptableObject
{
    public Transform[] points;
}
```

8. Откройте LevelPath.asset в инспекторе → перетащите все WP0–WP15 в массив **Points** (размер 16)

**Результат дня 3**  
Один ScriptableObject с массивом из 16 точек пути.

---

### День 4 — Префаб первого врага

1. Hierarchy → Create Empty → назовите **Slime**

2. Добавьте компонент **Sprite Renderer**  
   → Sprite → перетащите спрайт slime из Kenney

3. Добавьте **Circle Collider 2D**  
   → Is Trigger = true  
   → Radius ≈ 0.4–0.5

4. Добавьте **Rigidbody 2D**  
   → Body Type = **Kinematic**  
   → Gravity Scale = 0

5. Перетащите объект Slime из Hierarchy в папку **Assets/Prefabs/Enemies**  
   (если папки нет — создайте)

6. Удалите Slime из сцены (он теперь префаб)

**Результат дня 4**  
Префаб врага Slime готов.

---

### День 5 — Движение по waypoints

Создайте скрипт **EnemyMovement.cs**

```csharp
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private LevelPath path;
    [SerializeField] private float speed = 2f;

    private int currentPointIndex = 0;
    private Transform[] points => path.points;

    private void Start()
    {
        if (points.Length == 0 || path == null)
        {
            Debug.LogError("Путь не назначен!", this);
            enabled = false;
            return;
        }

        // Ставим врага на стартовую точку
        transform.position = points[0].position;
    }

    private void Update()
    {
        if (currentPointIndex >= points.Length - 1)
        {
            // Дошёл до конца — здесь позже будет урон по базе
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = points[currentPointIndex + 1].position;
        Vector3 direction = (targetPos - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        // Достигли точки?
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            currentPointIndex++;
            transform.position = targetPos; // точная фиксация
        }
    }
}
```

1. Прикрепите скрипт к префабу Slime  
2. Перетащите LevelPath.asset в поле **Path**

**Результат дня 5**  
Если перетащить Slime на сцену и нажать Play — он должен пойти по всему пути и исчезнуть в конце.

---

### День 6 — Здоровье и HP-бар

Создайте скрипт **EnemyHealth.cs**

```csharp
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 80f;
    private float currentHealth;

    [SerializeField] private GameObject deathEffectPrefab; // позже

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Здесь позже: золото, эффект смерти
        Destroy(gameObject);
    }

    public float GetHealthNormalized() => currentHealth / maxHealth;
}
```

1. Прикрепите к префабу Slime  
2. Создайте дочерний объект **Canvas** → переименуйте в **HealthBar**  
   → Render Mode → **World Space**  
   → Scale → 0.01, 0.01, 0.01  
   → Width 100, Height 10

3. В Canvas добавьте Image → назовите **Fill**  
   → Color зелёный  
   → Image Type → Filled → Fill Method → Horizontal

4. Создайте скрипт **HealthBarUI.cs**

```csharp
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private EnemyHealth health;

    private void Awake()
    {
        health = GetComponentInParent<EnemyHealth>();
    }

    private void LateUpdate()
    {
        if (health == null) return;

        fillImage.fillAmount = health.GetHealthNormalized();

        // Поворот к камере
        transform.rotation = Camera.main.transform.rotation;
    }
}
```

5. Прикрепите скрипт к объекту HealthBar → перетащите Fill в поле

**Результат дня 6**  
У врага есть здоровье и полоска HP над головой.
