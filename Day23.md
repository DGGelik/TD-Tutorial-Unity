
# День 23 — Продажа башен

⏱ Время выполнения: **25–35 минут**

---

# 🎯 Цель дня

Добавить возможность **продавать построенные башни**.

Это важная механика Tower Defense, потому что игрок должен иметь возможность:

```
изменять стратегию
перестраивать защиту
возвращать часть золота
```

---

# 🧩 Архитектура системы

```
Mouse Hover Tower
↓
Sell Button появляется
↓
Игрок нажимает Sell
↓
TowerController.Sell()
↓
BuildManager.AddGold()
↓
Башня уничтожается
```

---

# ⚙️ Что будет добавлено

```
Tower Prefab
 └ SellButtonCanvas

TowerController
 ├ OnMouseEnter()
 ├ OnMouseExit()
 └ Sell()
```

---

# 🔧 Шаг 1 — Открыть префаб башни

Открой любой префаб башни.

Например:

```
Assets/Prefabs/Towers/ArcherTower.prefab
```

---

# 🔧 Шаг 2 — Создать Canvas кнопки

Внутри башни:

```
Right Click
UI
Canvas
```

Переименуй:

```
SellButtonCanvas
```

---

# 🔧 Шаг 3 — Настроить Canvas

В Inspector:

```
Render Mode = World Space
Scale = 0.01 / 0.01 / 0.01
Width = 100
Height = 40
```

---

# 🔧 Шаг 4 — Добавить кнопку

Внутри SellButtonCanvas:

```
Right Click
UI
Button
```

Текст кнопки:

```
Sell
```

Настройки:

```
Font Size = 24
Color = White
```

---

# 🔧 Шаг 5 — Сделать Canvas скрытым

В Inspector:

```
SellButtonCanvas
SetActive = false
```

Кнопка будет появляться **только при наведении мыши**.

---

# 🔧 Шаг 6 — Открыть TowerController

Файл:

```
Assets/Scripts/TowerController.cs
```

Добавь поле:

```csharp
[SerializeField] private GameObject sellButtonCanvas;
```

---

# 🔧 Шаг 7 — Добавить показ кнопки

Добавь методы:

```csharp
private void OnMouseEnter()
{
    if (sellButtonCanvas)
        sellButtonCanvas.SetActive(true);
}

private void OnMouseExit()
{
    if (sellButtonCanvas)
        sellButtonCanvas.SetActive(false);
}
```

---

# 🔧 Шаг 8 — Добавить метод продажи

Добавь метод:

```csharp
public void Sell()
{
    int refund = Mathf.RoundToInt(data.baseCost * 0.65f);

    if (BuildManager.main != null)
    {
        BuildManager.main.AddGold(refund);
    }

    Destroy(gameObject);
}
```

---

# 🧠 Что происходит

Когда игрок нажимает кнопку:

```
Sell()
↓
возврат золота
↓
башня уничтожается
```

Игрок получает:

```
65% стоимости башни
```

---

# 🔧 Шаг 9 — Подключить кнопку

Выдели кнопку **Sell**.

В Inspector:

```
Button
OnClick()
```

Добавь объект башни.

Выбери метод:

```
TowerController → Sell()
```

---

# 🔧 Шаг 10 — Подключить Canvas

Выдели объект башни.

В Inspector:

```
TowerController
sellButtonCanvas
```

Перетащи:

```
SellButtonCanvas
```

---

# ✅ Проверка результата

Нажми **Play**.

Тест:

1. Построй башню  
2. Наведи мышь на башню  

Ожидаемый результат:

```
появляется кнопка Sell
```

Нажми кнопку.

Результат:

```
башня исчезает
игрок получает золото
```

---

# ⚠️ Частые ошибки

### ❌ Кнопка не появляется

Причина:

```
SellButtonCanvas не подключён
```

Проверь поле:

```
TowerController → sellButtonCanvas
```

---

### ❌ Кнопка появляется всегда

Причина:

```
Canvas не отключён
```

Проверь:

```
SetActive = false
```

---

### ❌ Золото не возвращается

Причина:

```
BuildManager.main = null
```

Проверь что объект BuildManager есть в сцене.

---

# 🧪 Маленькое задание

Измени возврат золота.

Сделай:

```
80% возврат
```

Подсказка:

```
baseCost * 0.8f
```

---

# 📋 Checklist дня

```
[ ] SellButtonCanvas создан
[ ] Canvas World Space
[ ] OnMouseEnter работает
[ ] OnMouseExit работает
[ ] Метод Sell работает
[ ] Золото возвращается
[ ] Башня уничтожается
```

---

# 🚀 Следующий шаг

Последний день второго месяца:

```
Полное тестирование игры
Создание сборки
Исправление багов
```

---
