
# День 20 — Экономика игры (золото за убийство врагов)

⏱ Время выполнения: **25–35 минут**

---

# 🎯 Цель дня

Сегодня мы создадим **экономику Tower Defense**.

Игрок будет:

1. Убивать врагов  
2. Получать золото  
3. Строить башни за золото  

Это создаёт **основной игровой цикл**.

```
Kill Enemy → Get Gold → Build Tower → Kill More Enemies
```

Без экономики TD игра **не имеет стратегии**.

---

# 🧠 Как экономика работает в Tower Defense

Практически все TD игры используют похожую систему.

| Игра | Награда |
|-----|-----|
Bloons TD | деньги за каждый шар |
Kingdom Rush | золото за убийства |
Dungeon Defenders | мана |
Plants vs Zombies | солнце |

Принцип одинаковый:

```
Enemy dies
↓
Player receives currency
↓
Player builds stronger defense
```

---

# 🧩 Архитектура системы

Сегодня мы добавим связь между врагами и экономикой.

```
Enemy получает урон
↓
EnemyHealth проверяет смерть
↓
EnemyHealth вызывает AddGold()
↓
BuildManager увеличивает gold
↓
UIManager показывает новое значение
```

Или коротко:

```
Enemy → BuildManager → UI
```

---

# ⚙️ Что мы добавим

Сегодня мы добавим:

```
EnemyHealth
 └ goldReward

BuildManager
 ├ AddGold()
 └ SpendGold()
```

---

# 🔧 Шаг 1 — Открыть EnemyHealth

Открой файл:

```
Assets/Scripts/EnemyHealth.cs
```

В начале класса добавь переменную:

```csharp
[SerializeField] private int goldReward = 6;
```

---

# 🧠 Что делает SerializeField

`SerializeField` позволяет **изменять значение прямо в Inspector**.

Пример:

| Враг | Награда |
|----|----|
Slime | 6 |
Orc | 15 |
Boss | 120 |

Без SerializeField пришлось бы менять код.

---

# 🔧 Шаг 2 — Изменить метод TakeDamage

Найди метод:

```csharp
TakeDamage()
```

И замени на этот:

```csharp
public void TakeDamage(float amount)
{
    currentHealth -= amount;

    if (currentHealth <= 0)
    {
        if (BuildManager.main != null)
        {
            BuildManager.main.AddGold(goldReward);
        }

        Die();
    }
}
```

---

# 🧠 Что происходит

Когда здоровье врага падает до нуля:

```
Enemy dies
↓
BuildManager.AddGold()
↓
Player получает награду
```

---

# 🔧 Шаг 3 — Открыть BuildManager

Открой файл:

```
Assets/Scripts/BuildManager.cs
```

Добавь два метода.

---

# 💻 Код экономики

```csharp
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
```

---

# 🧠 Объяснение методов

### AddGold

```
gold += amount
```

Просто увеличивает количество золота.

---

### SpendGold

Проверяет хватает ли денег.

```
если денег достаточно
    снимаем деньги
иначе
    ничего не делаем
```

Это защищает игру от ситуации:

```
gold = -200
```

---

# 🧠 Почему используется bool

Метод возвращает:

```
true  → покупка прошла
false → денег не хватает
```

Это удобно использовать при строительстве башен.

---

# 🔧 Шаг 4 — Использовать SpendGold при строительстве

В методе строительства башни найди строку:

```csharp
if (SpendGold(selectedTowerData.baseCost))
```

Теперь башня строится **только если хватает денег**.

---

# ✅ Проверка результата

Нажми **Play**.

Сделай тест:

1. Убей 1 врага  
2. Посмотри на Gold  

Если награда = 6:

```
Gold: 350 → Gold: 356
```

---

# 🧪 Дополнительный тест

Убей 5 врагов.

Ожидаемый результат:

```
5 × 6 = 30
```

```
Gold: 350 → 380
```

---

# ⚠️ Частые ошибки

---

## ❌ Золото не увеличивается

Причина:

```
EnemyHealth не вызывает AddGold
```

Проверь:

```
TakeDamage()
```

---

## ❌ Ошибка NullReference

Причина:

```
BuildManager.main = null
```

Решение:

Убедись что в сцене есть объект:

```
BuildManager
```

---

## ❌ HUD не обновляется

Причина:

```
UIManager не читает gold
```

Проверь строку:

```csharp
goldText.text = $"Gold: {buildManager.gold}";
```

---

# 🧪 Маленькое задание

Добавь **разную награду врагам**.

Пример:

| Enemy | Gold |
|------|------|
Slime | 6 |
Orc | 15 |
Boss | 120 |

Сделай:

```
Boss reward = 120
```

И проверь в игре.

---

# 📊 Экономический цикл TD

Теперь в игре работает базовая экономика.

```
Spawn Enemy
↓
Kill Enemy
↓
Get Gold
↓
Build Towers
↓
Kill More Enemies
```

Это **основной цикл Tower Defense игр**.

---

# 📋 Checklist дня

Перед переходом дальше проверь:

```
[ ] В EnemyHealth есть goldReward
[ ] TakeDamage вызывает AddGold
[ ] В BuildManager есть AddGold
[ ] В BuildManager есть SpendGold
[ ] HUD показывает изменение золота
[ ] Нет ошибок в Console
```

---

# 🚀 Что будет дальше

Следующий шаг — **жизни игрока**.

Мы добавим систему:


Enemy reaches end
↓
Player loses life
↓
Lives = 0
↓
Game Over

