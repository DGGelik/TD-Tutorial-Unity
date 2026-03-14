
# День 22 — Экраны победы и поражения

⏱ Время выполнения: **25–35 минут**

---

# 🎯 Цель дня

Создать UI экраны:

```
Victory
Game Over
```

---

# 🧩 Архитектура

```
BuildManager
↓
GameOver()
↓
GameOverPanel

WaveSpawner
↓
Max Waves
↓
VictoryPanel
```

---

# ⚙️ Что будет добавлено

```
UICanvas
 ├ GameOverPanel
 └ VictoryPanel
```

---

# 🔧 Шаг 1 — Создать GameOverPanel

В Hierarchy:

```
Right Click UICanvas
UI
Panel
```

Название:

```
GameOverPanel
```

Настройки:

```
Color = #000000AA
SetActive = false
```

---

# 🔧 Шаг 2 — Добавить текст

Внутри панели:

```
UI
TextMeshPro
```

Текст:

```
Game Over
```

Настройки:

```
Font Size = 72
Color = Red
Alignment = Center
```

---

# 🔧 Шаг 3 — Добавить кнопку

Внутри панели:

```
UI
Button
```

Текст кнопки:

```
Restart
```

---

# 🔧 Шаг 4 — Создать VictoryPanel

Повтори шаги.

Название:

```
VictoryPanel
```

Текст:

```
Victory
```

Цвет:

```
Gold
```

---

# 🔧 Шаг 5 — Подключить панели

Открой:

```
BuildManager.cs
```

Добавь:

```csharp
public GameObject gameOverPanel;
```

Измени GameOver():

```csharp
private void GameOver()
{
    if (gameOverPanel)
        gameOverPanel.SetActive(true);

    Time.timeScale = 0f;
}
```

---

# 🔧 Шаг 6 — Подключить Victory

Открой:

```
WaveSpawner.cs
```

Добавь:

```csharp
public GameObject victoryPanel;
public int maxWaves = 12;
```

В Update():

```csharp
if (currentWave > maxWaves)
{
    victoryPanel.SetActive(true);
    Time.timeScale = 0f;
}
```

---

# 🔧 Шаг 7 — Кнопка Restart

Добавь using:

```csharp
using UnityEngine.SceneManagement;
```

Метод:

```csharp
public void Restart()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene("Main");
}
```

Подключи метод к кнопке.

---

# ✅ Проверка результата

Тест 1:

```
Lives = 0
```

Должно появиться:

```
Game Over Panel
```

Тест 2:

```
Wave 12 complete
```

Появится:

```
Victory Panel
```

---

# ⚠️ Частые ошибки

### ❌ Панель не появляется

Причина:

```
Panel не подключена в Inspector
```

---

### ❌ Игра не перезапускается

Причина:

```
SceneManager не подключён
```

Добавь:

```csharp
using UnityEngine.SceneManagement;
```

---

# 📋 Checklist дня

```
[ ] GameOverPanel создан
[ ] VictoryPanel создан
[ ] GameOver вызывает панель
[ ] WaveSpawner вызывает Victory
[ ] Кнопка Restart работает
```

---

# 🚀 Следующий шаг

Дальше мы добавим:

```
Tower Selling System
```

Игрок сможет:

```
Продать башню
Получить часть золота назад
Перестроить защиту

