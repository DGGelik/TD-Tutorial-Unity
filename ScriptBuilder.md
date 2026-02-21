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