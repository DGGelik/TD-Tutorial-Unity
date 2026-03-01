### Скрипт для UI кнопки в Unity (открытие/закрытие панели магазина)

В Unity для UI кнопок (из пакета Unity UI) вы можете создать простой скрипт на C#, который вешается на кнопку. Этот скрипт будет toggling (переключать) видимость панели магазина. Предполагаю, что панель — это GameObject (например, ваш `ShopPanel` из BuildManager). Если нужно интегрировать с вашим BuildManager, я учту это.

#### Шаги по созданию и использованию:
1. **Создайте скрипт**: В Unity создайте новый C# скрипт (Assets > Create > C# Script), назовите его, например, `ShopToggleButton`.
2. **Вставьте код**: Откройте скрипт в Visual Studio или другом редакторе и вставьте код ниже.
3. **Прикрепите к кнопке**: Перетащите скрипт на UI Button в сцене. В инспекторе кнопки в разделе OnClick() добавьте вызов метода (например, `ToggleShopPanel()`).
4. **Назначьте ссылки**: В инспекторе скрипта назначьте GameObject панели магазина (ShopPanel) и, если нужно, другие кнопки (HideButton, ActiveButton из вашего кода).
5. **Интеграция с BuildManager**: Если панель управляется через BuildManager.main, используйте его для доступа.

#### Пример кода скрипта (ShopToggleButton.cs)
```csharp
using UnityEngine;
using UnityEngine.UI;  // Для работы с UI элементами

public class ShopToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;  // Ссылка на панель магазина (назначьте в инспекторе)
    [SerializeField] private GameObject hideButton; // Кнопка "скрыть" (если есть)
    [SerializeField] private GameObject activeButton; // Кнопка "показать" (если есть)

    private bool isShopOpen = false;  // Флаг для отслеживания состояния

    private void Start()
    {
        // Изначально спрячем панель, если она активна
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        UpdateButtons();  // Обновим видимость кнопок
    }

    // Метод для вызова из OnClick() кнопки
    public void ToggleShopPanel()
    {
        if (shopPanel == null)
        {
            Debug.LogError("ShopPanel не назначен!");
            return;
        }

        isShopOpen = !isShopOpen;  // Переключаем состояние
        shopPanel.SetActive(isShopOpen);  // Включаем/выключаем панель

        UpdateButtons();  // Обновляем видимость других кнопок
        Debug.Log("Магазин " + (isShopOpen ? "открыт" : "закрыт"));
    }

    // Метод для обновления видимости кнопок Hide и Active (если они есть)
    private void UpdateButtons()
    {
        if (hideButton != null)
        {
            hideButton.SetActive(isShopOpen);  // Показываем "скрыть" только если открыт
        }
        if (activeButton != null)
        {
            activeButton.SetActive(!isShopOpen);  // Показываем "показать" только если закрыт
        }
    }
}
```

#### Объяснение кода:
- **[SerializeField] private GameObject shopPanel;**: Ссылка на ваш ShopPanel. Назначьте его в инспекторе Unity (перетащите GameObject из иерархии).
- **ToggleShopPanel()**: Основной метод. Вызывается при клике на кнопку. Он переключает активность панели с помощью `SetActive(bool)`.
- **isShopOpen**: Флаг, чтобы отслеживать, открыта ли панель (для toggling).
- **UpdateButtons()**: Если у вас есть дополнительные кнопки (HideButton и ActiveButton, как в вашем BuildManager), этот метод управляет их видимостью. Например, "скрыть" видно только когда панель открыта.
- **Start()**: Инициализация — прячет панель по умолчанию.

#### Как прикрепить к кнопке:
- Выберите UI Button в сцене.
- В компоненте Button > On Click():
  - Перетащите объект с скриптом ShopToggleButton.
  - Выберите метод `ShopToggleButton > ToggleShopPanel`.
- Если кнопок несколько (например, отдельно "открыть" и "закрыть"), создайте два метода: `OpenShop()` и `CloseShop()`, и назначьте соответственно.

#### Интеграция с вашим BuildManager:
Если панель — часть BuildManager, вы можете обращаться к ней через singleton:
```csharp
// В ToggleShopPanel()
BuildManager.main.ShopPanel.SetActive(isShopOpen);
BuildManager.main.HideButton.SetActive(isShopOpen);
BuildManager.main.ActiveButton.SetActive(!isShopOpen);
```
(Удалите [SerializeField] для этих ссылок, если используете main.)

Если панель должна блокировать игру (пауза), добавьте `Time.timeScale = isShopOpen ? 0f : 1f;` в ToggleShopPanel().

Если нужны доработки (например, анимации открытия/закрытия через Animator), уточните! Протестируйте в Unity — ошибки в консоли подскажут, если ссылки не назначены.