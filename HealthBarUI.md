```chsharp
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private EnemyHealth health;

    private void Awake()
    {
        health = GetComponentInParent<EnemyHealth>();

        // === Создаём белый спрайт прямо в коде (один раз) ===
        if (fillImage.sprite == null)
        {
            Texture2D whiteTex = new Texture2D(1, 1);
            whiteTex.SetPixel(0, 0, Color.white);
            whiteTex.Apply();

            Sprite whiteSprite = Sprite.Create(whiteTex, new Rect(0, 0, 1, 1), Vector2.zero);

            fillImage.sprite = whiteSprite;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = 0; // слева направо
        }

        // Делаем бар зелёным (можно поменять на любой цвет)
        fillImage.color = new Color(0f, 1f, 0f); // чисто зелёный
    }

    private void LateUpdate()
    {
        if (health == null) return;

        fillImage.fillAmount = health.GetHealthNormalized();

        // Поворот к камере (чтобы бар всегда смотрел на игрока)
        transform.rotation = Camera.main.transform.rotation;
    }
}
```



```csharp
using UnityEngine;

public class TowerController : MonoBehaviour   // ← или как называется твой скрипт на башне
{
    [Header("UI для продажи/улучшения")]
    [SerializeField] private GameObject upgradeSellCanvas;   // ← перетащи сюда Canvas с кнопками

    private bool isPanelOpen = false;

    // ===================================================================
    // 1. Показываем панель ПРИ НАЖАТИИ мыши (не при наведении!)
    // ===================================================================
    private void OnMouseDown()
    {
        if (upgradeSellCanvas == null) return;

        isPanelOpen = !isPanelOpen;                    // переключаем (кликнул → открыл, кликнул ещё раз → закрыл)
        upgradeSellCanvas.SetActive(isPanelOpen);

        // Если хочешь, чтобы панель закрывалась при клике на другую башню — добавь в Update проверку
    }

    // ===================================================================
    // 2. Кнопка "Продать"
    // ===================================================================
    public void SellTower()
    {
        // 65% возврата от базовой стоимости (можно потом улучшить)
        int refund = Mathf.RoundToInt(data.baseCost * 0.65f);
        BuildManager.main.AddGold(refund);

        Destroy(gameObject);           // удаляем башню
    }

    // ===================================================================
    // 3. Кнопка "Улучшить" (заглушка — потом заменишь на свою логику)
    // ===================================================================
    public void UpgradeTower()
    {
        // Пример простой логики улучшения
        if (data.currentLevel < data.maxLevel)
        {
            int cost = data.upgradeCost;                 // предполагаем, что в твоём data есть upgradeCost

            if (BuildManager.main.HasEnoughGold(cost))
            {
                BuildManager.main.SpendGold(cost);

                // Увеличиваем уровень и характеристики
                data.currentLevel++;
                damage *= 1.4f;          // +40% урона (можно менять)
                range  *= 1.2f;          // +20% радиуса

                Debug.Log($"Башня улучшена до уровня {data.currentLevel}");

                // Можно закрыть панель после улучшения
                upgradeSellCanvas.SetActive(false);
                isPanelOpen = false;
            }
            else
            {
                Debug.Log("Недостаточно золота для улучшения!");
            }
        }
        else
        {
            Debug.Log("Башня уже на максимальном уровне!");
        }
    }

    // ===================================================================
    // Дополнительно: закрытие панели при клике вне башни (по желанию)
    // ===================================================================
    private void Update()
    {
        if (isPanelOpen && Input.GetMouseButtonDown(0))
        {
            // Проверяем, кликнули ли мы по другой башне или по пустому месту
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics2D.Raycast(ray.origin, ray.direction).collider == null ||
                Physics2D.Raycast(ray.origin, ray.direction).collider.gameObject != gameObject)
            {
                upgradeSellCanvas.SetActive(false);
                isPanelOpen = false;
            }
        }
    }
}
```

### Что нужно сделать в Unity:

1. На префабе башни (`ArcherTower.prefab`) создай дочерний **Canvas** (World Space)  
   - Назови его `UpgradeSellCanvas`  
   - Поставь Scale `(0.01, 0.01, 1)`  
   - Добавь два **Button**:
     - Кнопка **Sell** → OnClick → TowerController → SellTower()
     - Кнопка **Upgrade** → OnClick → TowerController → UpgradeTower()

2. Перетащи этот Canvas в поле `upgradeSellCanvas` в инспекторе башни.

3. Сохрани префаб.

Теперь:
- Кликнул ЛКМ по башне → появляется панель с двумя кнопками  
- Кликнул ещё раз по той же башне → панель исчезает  
- Кликнул мимо → панель тоже исчезает (благодаря Update)

