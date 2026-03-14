
# День 26 — Летающий враг (Wraith) + иммунитет к наземным башням

⏱ Время выполнения: **20–30 минут**

---

# 🎯 Цель дня

Реализовать механику летающих врагов.  
Призрак (Wraith) будет проходить мимо стрелковых и ледяных башен, но огненные и цепные смогут его атаковать.

Это классическая механика Tower Defense — игрок должен думать, какие башни ставить против разных типов врагов.

---

# 🧩 Что будет добавлено

- Новый скрипт **FlyingEnemy**
- Проверка в методе поиска цели
- Поле **canHitFlying** в TowerData

---

# 🔧 Шаг 1 — Создаём скрипт FlyingEnemy

1. В окне **Project** открой папку:
   ```
   Assets → Scripts
   ```
2. Правой кнопкой мыши → **Create** → **C# Script**
3. Назови файл **FlyingEnemy**
4. Двойной клик — открой скрипт
5. Полностью замени код на:

```csharp
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public bool isFlying = true;
}
```

**Зачем этот скрипт?**  
Он просто говорит: «Я летаю». Башни будут проверять этот флаг и решать, стрелять или нет. В реальных играх такие простые скрипты-метки используются для иммунитетов.

---

# 🔧 Шаг 2 — Прикрепляем скрипт только к Wraith

1. Двойной клик по префабу **EnemyWraith.prefab** (откроется режим редактирования)
2. В Inspector нажми большую кнопку **Add Component**
3. Найди и добавь **FlyingEnemy**
4. Оставь галочку **isFlying = true** (по умолчанию)

**Важно:**  
Не прикрепляй этот скрипт к другим врагам — только к Wraith!

---

# 🔧 Шаг 3 — Изменяем TowerController

1. Открой скрипт **TowerController.cs**
2. Найди метод **UpdateTarget** и полностью замени его на этот код:

```csharp
void UpdateTarget()
{
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRange);

    Transform nearest = null;
    float minDist = float.MaxValue;

    foreach (var hit in hits)
    {
        if (hit.CompareTag("Enemy"))
        {
            // Проверяем, летает ли враг
            FlyingEnemy flying = hit.GetComponent<FlyingEnemy>();
            if (flying != null && flying.isFlying)
            {
                // Пропускаем летающих, если башня не умеет их атаковать
                if (!data.canHitFlying) continue;
            }

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }
    }

    target = nearest;
}
```

**Пояснение кода**  
- Мы получаем все объекты в радиусе  
- Если враг летает и башня не может его бить — пропускаем  
- Иначе выбираем ближайшего

---

# 🔧 Шаг 4 — Добавляем поле в TowerData

1. Открой **TowerData.cs**
2. В конец класса добавь:

```csharp
[Header("Механики")]
public bool canHitFlying = false;
```

---

# 🔧 Шаг 5 — Заполняем значения в ассетах

Открой каждый из 4 TowerData ассетов и поставь:

- **ArcherTowerData** → canHitFlying = **false**
- **FireTowerData** → canHitFlying = **true**
- **FrostTowerData** → canHitFlying = **false**
- **ChainTowerData** → canHitFlying = **true**

---

# ✅ Проверка дня 26

1. Поставь стрелковую и ледяную башню рядом с дорогой
2. Поставь огненную и цепную башню
3. Запусти волну с Wraith (перетащи префаб Wraith в WaveSpawner или спавни вручную)

Результат:
- Призрак проходит мимо Archer и Frost без урона
- Fire и Chain стреляют по нему

---

# ⚠️ Частые ошибки

### ❌ Призрак получает урон от всех башен
Причина: поле canHitFlying не заполнено или скрипт FlyingEnemy не прикреплён  
Решение: проверь оба префаба и ассеты TowerData

### ❌ Башня вообще не видит врага
Причина: Tag "Enemy" потерялся на префабе Wraith  
Решение: проверь Tag = "Enemy"

### ❌ Ошибка в консоли про FlyingEnemy
Причина: скрипт не прикреплён к Wraith  
Решение: добавь компонент

---

# 📊 Итог дня

Теперь у нас есть настоящая механика летающих врагов!  
Игрок должен правильно выбирать башни — это делает игру стратегической.

---

# 📋 Финальный Checklist

- [ ] FlyingEnemy скрипт создан и прикреплён только к Wraith
- [ ] canHitFlying настроено в каждом TowerData
- [ ] Призрак игнорируется стрелковыми и ледяными башнями
- [ ] Огненная и цепная башни его атакуют

---

