
# День 21 — Система жизней (враг дошёл до конца)

⏱ Время выполнения: **20–30 минут**

---

# 🎯 Цель дня

Добавить систему жизней игрока.

Если враг дошёл до конца дороги:

```
Enemy reaches end
↓
Player loses life
↓
Lives = 0
↓
Game Over
```

---

# 🧩 Архитектура системы

```
EnemyMovement
↓
TakeLife()
↓
BuildManager
↓
Lives уменьшается
↓
UIManager обновляет HUD
```

---

# ⚙️ Что будет добавлено

```
BuildManager
 ├ lives
 ├ TakeLife()
 └ GameOver()

EnemyMovement
 └ проверка конца пути
```

---

# 🔧 Шаг 1 — Открыть BuildManager

Открой файл:

```
Assets/Scripts/BuildManager.cs
```

Добавь переменную:

```csharp
public int lives = 20;
```

---

# 🔧 Шаг 2 — Добавить метод TakeLife

Добавь код:

```csharp
public void TakeLife()
{
    lives--;

    if (lives <= 0)
    {
        GameOver();
    }
}
```

---

# 🔧 Шаг 3 — Добавить GameOver

Добавь метод:

```csharp
private void GameOver()
{
    Debug.Log("GAME OVER");
    Time.timeScale = 0f;
}
```

---

# 🔧 Шаг 4 — Открыть EnemyMovement

Файл:

```
Assets/Scripts/EnemyMovement.cs
```

Найди `Update()`.

В конце добавь:

```csharp
if (currentPointIndex >= points.Length - 1)
{
    if (BuildManager.main != null)
    {
        BuildManager.main.TakeLife();
    }

    Destroy(gameObject);
    return;
}
```

---

# 🧠 Что происходит

Когда враг достигает последней точки пути:

```
Enemy reaches last waypoint
↓
TakeLife()
↓
Lives уменьшается
↓
Enemy уничтожается
```

---

# ✅ Проверка результата

Нажми **Play**.

Сделай тест:

1. Пропусти врага
2. Посмотри HUD

Ожидаемый результат:

```
Lives: 20 → Lives: 19
```

Если пропустить 20 врагов:

```
GAME OVER
```

Игра остановится.

---

# ⚠️ Частые ошибки

### ❌ Lives не уменьшается

Причина:

```
EnemyMovement не вызывает TakeLife
```

Проверь код:

```csharp
BuildManager.main.TakeLife();
```

---

### ❌ Ошибка NullReference

Причина:

```
BuildManager.main = null
```

Убедись что в сцене есть объект:

```
BuildManager
```

---

# 🧪 Маленькое задание

Сделай так чтобы:

```
Boss enemy снимает 3 жизни
```

Подсказка:

```
TakeLife(int amount)
```

---

# 📋 Checklist дня

```
[ ] В BuildManager есть lives
[ ] Добавлен TakeLife()
[ ] Добавлен GameOver()
[ ] EnemyMovement вызывает TakeLife
[ ] HUD показывает уменьшение жизней
[ ] Нет ошибок в Console
```

---

# 🚀 Следующий шаг

Дальше мы добавим **экраны победы и поражения**.

```
Lives = 0
↓
Game Over Screen

Wave 12 completed
↓
Victory Screen
```

---
