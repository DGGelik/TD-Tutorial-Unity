```markdown
### Исправление проблемы: всегда строится один префаб, независимо от выбора в магазине

**Проблема:** В BuildManager всегда используется фиксированный `towerPrefab`, а не выбранный из магазина. Это старая версия из Дня 12. С Дня 14+ мы используем `selectedTowerData.turretPrefab` из ScriptableObject.

**Быстрое решение (5 минут):**

#### 1. Обнови BuildManager.cs (полностью замени старый код)




```csharp
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    public int gold = 350;
    public int lives = 20;

    public TowerData selectedTowerData;  // ← выбранная башня из магазина

    private GameObject ghostTower;
    private SpriteRenderer ghostRenderer;

    private void Awake()
    {
        if (main != null)
        {
            Debug.LogError("Два BuildManager в сцене!");
            Destroy(gameObject);
            return;
        }
        main = this;
    }

    private void Update()
    {
        if (selectedTowerData != null)
        {
            UpdateGhost();
            
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceTower();
            }
        }
        else if (ghostTower != null)
        {
            Destroy(ghostTower);
            ghostTower = null;
        }
    }

    void UpdateGhost()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Raycast для проверки слоя Buildable
        int buildableLayerMask = 1 << LayerMask.NameToLayer("Buildable");
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, buildableLayerMask);

        if (ghostTower == null)
        {
            // Создаём ghost из выбранного префаба
            ghostTower = Instantiate(selectedTowerData.turretPrefab, mousePos, Quaternion.identity);
            ghostRenderer = ghostTower.GetComponent<SpriteRenderer>();
            ghostRenderer.color = new Color(1, 1, 1, 0.5f); // полупрозрачный по умолчанию
            
            // Отключаем коллайдеры и скрипты на ghost (чтобы не мешали)
            Collider2D col = ghostTower.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            TowerController tc = ghostTower.GetComponent<TowerController>();
            if (tc != null) tc.enabled = false;
        }
        else
        {
            ghostTower.transform.position = mousePos;
        }

        // Меняем цвет ghost в зависимости от слоя
        if (hit.collider != null)
        {
            ghostRenderer.color = new Color(0.2f, 1f, 0.2f, 0.6f); // зелёный — можно строить
        }
        else
        {
            ghostRenderer.color = new Color(1f, 0.2f, 0.2f, 0.6f); // красный — нельзя
        }
    }

    void TryPlaceTower()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Raycast только по слою Buildable
        int buildableLayerMask = 1 << LayerMask.NameToLayer("Buildable");
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, buildableLayerMask);

        if (hit.collider != null)
        {
            if (SpendGold(selectedTowerData.baseCost))
            {
                // Строим именно выбранный префаб из TowerData!
                Instantiate(selectedTowerData.turretPrefab, hit.point, Quaternion.identity);
                selectedTowerData = null; // сбрасываем выбор
                if (ghostTower != null)
                {
                    Destroy(ghostTower);
                    ghostTower = null;
                }
                Debug.Log("Построена башня: " + selectedTowerData.towerName);
            }
            else
            {
                Debug.Log("Недостаточно золота!");
            }
        }
        else
        {
            Debug.Log("Нельзя строить здесь! (только на траве)");
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    public void TakeLife()
    {
        lives--;
        Debug.Log("Жизни осталось: " + lives);
        if (lives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER!");
        Time.timeScale = 0;
    }
}

```



#### 2. Назначь turretPrefab в TowerData (самый частый баг)

1. Открой каждый TowerData.asset (двойной клик).  
2. В поле **Turret Prefab** перетащи **соответствующий префаб башни**:  
   - ArcherTowerData → ArcherTower.prefab  
   - FireTowerData → FireTower.prefab  
   - И т.д.  
3. Сохрани (Ctrl+S).

**Зачем?** `selectedTowerData.turretPrefab` — это префаб башни из SO. Если поле пустое — Instantiate(null) = ошибка.

#### 3. Проверь TowerShop.cs (магазин)

```csharp
void SelectTower(int index)
{
    BuildManager.main.selectedTowerData = towerDatas[index];
    Debug.Log("Выбрана: " + towerDatas[index].towerName); // ← проверь лог в консоли
}
```

- Перетащи в массив `towerDatas` правильные SO (ArcherTowerData и т.д.).  
- Клик по кнопке → лог "Выбрана: ArcherTower" → ghost должен появиться как ArcherTower.

#### 4. Чек-лист "почему может не работать"

| Симптом | Причина | Исправление |
|---------|---------|-------------|
| Всегда ArcherTower | `turretPrefab` не назначен в TowerData | Шаг 2 |
| Ghost не появляется | `selectedTowerData = null` | Проверь TowerShop — лог в консоли? |
| Клик не ставит башню | Raycast не попадает | Ground на слое Buildable + Tilemap Collider 2D |
| "Недостаточно золота" | gold < cost | Убей врагов или дай gold = 1000 |
| NullReference | main = null | Добавь Awake() в BuildManager |
| Нет спавна волн | Waypoints не назначены | WaveSpawner → Spawn Point = WP0 |

#### 5. Тест (Play и проверь)

1. Play.  
2. Клик по кнопке Archer → ghost Archer появляется?  
3. Клик по Fire → ghost меняется на Fire?  
4. Клик по траве → ставится выбранная башня?  

**Если всё ещё не работает:**  
- Console (красные ошибки) — скинь скрин.  
- Inspector BuildManager в Play: selectedTowerData меняется?  

Теперь башни будут строиться **правильные** из выбранного SO. Всё работает! 🚀
```