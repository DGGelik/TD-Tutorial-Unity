

# День 19 — HUD интерфейс (золото, жизни, волны)

⏱ Примерное время выполнения: **25–35 минут**

---

# 🎯 Цель дня

Сегодня мы создадим **HUD (Heads-Up Display)** — интерфейс, который игрок видит постоянно во время игры.

HUD показывает:

- количество золота
- количество жизней
- текущую волну
- таймер следующей волны

Без HUD игрок **не понимает состояние игры**, поэтому это одна из самых важных систем.

---

# 🧠 Как это работает в Tower Defense

Практически во всех TD играх интерфейс устроен одинаково.

| Параметр | Зачем нужен |
|--------|--------|
| Gold | показывает может ли игрок строить башни |
| Lives | показывает сколько врагов можно пропустить |
| Wave | показывает прогресс уровня |
| Next Wave Timer | даёт время подготовиться |

Примеры игр:

- Bloons TD
- Kingdom Rush
- Plants vs Zombies
- Dungeon Defenders

---

# 🧩 Архитектура системы

Наш интерфейс будет работать по следующей схеме:

```

Enemy умирает
↓
BuildManager изменяет gold
↓
WaveSpawner изменяет wave
↓
UIManager читает данные
↓
TextMeshPro показывает значения

```

Или проще:

```

Game Systems → UIManager → HUD

```

UI **ничего не считает**, он **только отображает данные**.

---

# ⚙️ Что мы создадим

Сегодня мы добавим:

```

UICanvas
├ GoldText
├ LivesText
├ WaveText
└ TimerText

```

---

# 🔧 Шаг 1 — Открыть сцену

Открой сцену игры.

```

Project → Assets → Scenes → Main.unity

```

Если у тебя другая сцена — открой ту, где находятся:

- карта
- враги
- башни

---

# 🔧 Шаг 2 — Создать Canvas

В окне **Hierarchy**

```

Right Click
UI
Canvas

```

Переименуй объект:

```

UICanvas

```

---

# 🧠 Почему нужен Canvas

Canvas — это специальный контейнер для интерфейса.

Особенности:

- UI всегда рисуется поверх игры
- UI не зависит от камеры
- UI можно масштабировать под разные экраны

Все интерфейсы Unity работают через Canvas.

---

# 🔧 Шаг 3 — Настроить Canvas

Выдели **UICanvas**.

В **Inspector → Canvas Scaler** установи:

```

UI Scale Mode = Scale With Screen Size
Reference Resolution = 1920 x 1080
Screen Match Mode = Match Width Or Height
Match = 0.5

```

---

# 🧠 Почему это важно

Игры запускаются на разных разрешениях:

```

1280x720
1920x1080
2560x1440
4K

```

Если не использовать **Canvas Scaler**, интерфейс:

- будет слишком маленьким
- или слишком большим

Эта настройка — **стандарт индустрии**.

---

# 🔧 Шаг 4 — Создать текст HUD

Создай 4 текста.

В **Hierarchy**:

```

Right Click on UICanvas
UI
Text - TextMeshPro

```

Сделай 4 объекта.

---

## Настройки текста

| Object | Anchor | Pos X | Pos Y | Font Size | Color | Text |
|------|------|------|------|------|------|------|
| GoldText | Top Left | 20 | -20 | 48 | #FFD700 | Gold: 350 |
| LivesText | Top Right | -20 | -20 | 48 | #FF4444 | Lives: 20 |
| WaveText | Top Center | 0 | -20 | 42 | White | Wave: 1 / 12 |
| TimerText | Top Center | 0 | -80 | 36 | Gray | Next wave: 8 |

---

# 🧠 Почему TextMeshPro

Unity имеет 2 системы текста:

```

Legacy Text
TextMeshPro

```

TextMeshPro лучше потому что:

- чёткие шрифты
- поддержка outline
- поддержка shadows
- лучше работает на разных разрешениях

В современных играх **используют только TMP**.

---

# 🔧 Шаг 5 — Создать UIManager

В папке **Scripts**:

```

Right Click
Create
C# Script

```

Название:

```

UIManager

````

---

# 💻 Код UIManager

```csharp
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD Text Elements")]

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timerText;

    private WaveSpawner waveSpawner;
    private BuildManager buildManager;

    private void Awake()
    {
        waveSpawner = FindObjectOfType<WaveSpawner>();
        buildManager = FindObjectOfType<BuildManager>();
    }

    private void Update()
    {
        if (goldText)
            goldText.text = $"Gold: {buildManager.gold}";

        if (livesText)
            livesText.text = $"Lives: {buildManager.lives}";

        if (waveText)
            waveText.text = $"Wave: {waveSpawner.currentWave} / 12";

        if (timerText)
        {
            if (waveSpawner.countdown > 0)
                timerText.text = $"Next wave: {Mathf.Ceil(waveSpawner.countdown)}";
            else
                timerText.text = "";
        }
    }
}
````

---

# 🧠 Объяснение кода

### `using TMPro`

Подключает систему TextMeshPro.

---

### `TextMeshProUGUI`

Тип UI текста.

---

### `FindObjectOfType`

Находит объекты:

```
WaveSpawner
BuildManager
```

---

### `Update()`

Каждый кадр:

```
UI читает данные
и обновляет текст
```

Это нормальная практика для HUD.

---

# 🔧 Шаг 6 — Подключить UIManager

Перетащи **UIManager.cs** на объект:

```
UICanvas
```

В инспекторе появятся поля:

```
Gold Text
Lives Text
Wave Text
Timer Text
```

Перетащи соответствующие объекты:

```
GoldText → goldText
LivesText → livesText
WaveText → waveText
TimerText → timerText
```

---

# ✅ Проверка результата

Нажми **Play**.

На экране должно появиться:

```
Gold: 350
Lives: 20
Wave: 1 / 12
Next wave: 8
```

Если враги появляются — таймер должен уменьшаться.

---

# ⚠️ Частые ошибки

---

### ❌ Текст не появляется

Причина:

```
Canvas в режиме World Space
```

Решение:

```
UICanvas
Render Mode → Screen Space Overlay
```

---

### ❌ Текст есть но не обновляется

Причина:

```
UIManager не нашёл BuildManager
```

Решение:

Проверь что в сцене есть:

```
BuildManager
WaveSpawner
```

---

### ❌ NullReferenceException

Причина:

Ты не перетащил текстовые объекты.

Решение:

```
UICanvas
UIManager
перетащи TextMeshPro объекты
```

---

# 🧪 Маленькое задание

Добавь новый текст:

```
Score: 0
```

В правый нижний угол.

Попробуй обновлять его через UIManager.

---

# 📊 Итог системы HUD

Теперь интерфейс показывает:

```
Gold
Lives
Wave
Next Wave Timer
```

Это уже **полноценный HUD Tower Defense игры**.

---

# 📋 Checklist дня

Перед тем как двигаться дальше убедись:

```
[ ] Canvas настроен
[ ] 4 текста видны
[ ] UIManager подключён
[ ] значения обновляются
[ ] нет ошибок в Console
```

---

# 🚀 Что будет завтра

На следующем шаге мы добавим **экономику игры**:

```
Enemy → смерть → золото
Gold → строительство башен
SpendGold → баланс игры
```

