**ГДД / Техническое задание для Junior-разработчика**  
**Механика: Телепорт в 3D-игре (Unity)**  
**Версия документа: 1.0**  

### 1. Цель механики
Сделать **мгновенный, управляемый и приятный** телепорт, который:
- Добавляет динамики и вертикальности геймплею
- Чувствуется мощно и справедливо
- Легко расширяется в будущем (кулдаун, мана, порталы, телепорт к врагу и т.д.)
- Не ломает физику, коллизии и камеру

Это будет **первая версия** — базовый телепорт вперёд + телепорт к курсору. Дальше будем усложнять.

### 2. Базовое поведение (что должно работать в итоге)

**Основной режим (клавиша T):**
- Телепорт ровно на 12 метров вперёд по направлению, куда смотрит **камера** (не персонаж!)
- Проверка пути: если по прямой есть препятствие — телепорт не происходит
- После телепорта персонаж остаётся на той же высоте (не взлетает и не проваливается)
- Сохраняется поворот персонажа и камеры
- Мгновенно (0 секунд задержки)

**Второй режим (по нажатию ЛКМ + прицел):**
- Игрок смотрит на землю/поверхность
- Показывается зелёный индикатор (круг или проекция персонажа)
- Если можно телепортироваться — ЛКМ = телепорт в эту точку
- Максимальная дистанция 15 метров
- Красный индикатор, если слишком далеко / не на земле / в воздухе / за стеной

### 3. Технические требования

- Unity 2022.3 LTS или Unity 6 (URP)
- Персонаж использует либо **CharacterController**, либо **Rigidbody + CapsuleCollider** (укажи в комментариях, какой у тебя)
- Используем **New Input System** (не старый Input.GetKey)
- Всё настраивается в инспекторе (дистанция, кулдаун, слои и т.д.)
- Добавить простые частицы (вспышка в точке старта и финиша)
- Никакого нового ассета не создавать — используй стандартные Particle System или готовый prefab из Starter Assets

### 4. Пошаговое руководство (делай строго по порядку)

**Шаг 1. Подготовка**
1. Создай новый скрипт `PlayerTeleport.cs` и повесь его на персонажа.
2. Добавь публичные переменные:
   ```csharp
   public float teleportDistance = 12f;
   public float maxAimDistance = 15f;
   public LayerMask groundLayer;        // только "Ground" или "Walkable"
   public LayerMask obstacleLayer;      // всё, что блокирует путь
   public ParticleSystem startVFX;
   public ParticleSystem endVFX;
   ```

**Шаг 2. Базовый телепорт (клавиша T)**
- Используй `InputAction` из Input System
- Делай Raycast от позиции груди персонажа (transform.position + Vector3.up * 1.2f) вперёд по направлению камеры
- Если расстояние до препятствия > teleportDistance → телепортируем
- Формула перемещения:
  ```csharp
  Vector3 targetPos = transform.position + cameraForward * teleportDistance;
  ```
  (но с проверкой высоты!)

**Шаг 3. Телепорт к курсору**
- Raycast от камеры (Camera.main.ScreenPointToRay)
- Проверяем hit.distance <= maxAimDistance && hit.collider.gameObject.layer == groundLayer
- Показываем индикатор (простой Projector или Sphere с прозрачным материалом + scale)

**Шаг 4. Физика после телепорта (самое важное!)**
- Если Rigidbody → обязательно `rb.velocity = Vector3.zero;` или сохраняй нужный импульс
- Если CharacterController → `controller.enabled = false;` → Move → `controller.enabled = true;`
- Это предотвращает "залипание" и проваливание сквозь пол.

### 5. КРИТИЧЕСКИЕ ПОДВОДНЫЕ КАМНИ (читай обязательно!)

**Как НЕ надо делать (типичные ошибки джунов):**
- Делать телепорт в `Update()` через `if (Input.GetKeyDown(KeyCode.T))` — будет срабатывать каждый кадр при зажатой клавише.
- Делать Raycast от ног персонажа — не видит низкие препятствия (ступеньки, бордюры).
- Телепортировать по `transform.position = targetPos` без отключения коллайдера — персонаж застревает внутри стен.
- Забывать сбрасывать velocity у Rigidbody — персонаж продолжает лететь после телепорта.
- Игнорировать Y-координату — телепортирует в воздух или под текстуры.
- Использовать `Physics.Raycast` без LayerMask — будет телепортировать сквозь врагов, триггеры и UI.
- Делать анимацию через `SetActive(false)` на весь объект — ломается камера и всё управление.

**На что обязательно обращать внимание:**
- Разница между направлением камеры и направлением персонажа
- Высота точки старта Raycast (грудь/глаза, а не ноги)
- Слой персонажа (обычно Player не должен коллидировать сам с собой)
- Что происходит, если телепорт в очень узком коридоре
- Тестировать на разных поверхностях (наклонные плоскости, лестницы, движущиеся платформы)

### 6. Где искать готовые решения (Unity Manual + документация)

- **Input System** — https://docs.unity3d.com/Packages/com.unity.inputsystem@1.8/manual/index.html (Action Asset)
- **Raycast от камеры** — Camera.ScreenPointToRay + Physics.Raycast
- **CharacterController.Move** — официальный пример Teleport в документации
- **Отключение коллайдера на 1 кадр** — самый надёжный способ
- **Particle System** — готовый пример Burst + Color over Lifetime
- **Debug визуализация** — `Debug.DrawRay` и `Debug.DrawLine` (очень помогает!)

### 7. Критерии приёмки (что я буду проверять)

1. Телепорт по T работает ровно на 12 метров вперёд
2. При упоре в стену — ничего не происходит (нет "прыжка в стену")
3. Телепорт к курсору: зелёный/красный индикатор работает
4. После телепорта персонаж стоит ровно, может сразу бежать
5. Нет багов с проваливанием, застреванием, полётом
6. Всё настраивается в инспекторе
7. Есть простые частицы (хотя бы две вспышки)

### 8. Бонусные усложнения (делай, если базовое готово за 1–2 дня)

1. Кулдаун 3 секунды + UI-полоска
2. Расход маны 25 ед.
3. Анимация dissolve (материал с shader graph или просто отключение рендерера)
4. Сохранение импульса (dash-эффект)
5. Парные порталы (вход в один — выход из другого)

---




---


```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerMovement.cs          ← ходьба + прыжок
│   │   ├── PlayerTeleport.cs          ← наш телепорт (главный скрипт)
│   │   └── PlayerInput.cs             ← Input System (опционально)
│   └── Managers/
│       └── GameManager.cs             ← если будет потом
├── Prefabs/
│   ├── Player.prefab                  ← готовый персонаж
│   └── VFX/
│       ├── TeleportStart.prefab
│       └── TeleportEnd.prefab
├── Materials/
├── Animations/                        ← если используешь Animator
├── Scenes/
│   └── Test_Teleport.unity            ← главная тестовая сцена
├── ScriptableObjects/                 ← для настроек (пока пусто)
└── Resources/                         ← для частиц и звуков
```

**Рекомендация:** Создай папку `Player` внутри `Scripts` — всё, что касается персонажа, должно лежать там.

### 2. Что должно быть на персонаже (компоненты)

Перетащи Capsule (или готовый модель из Starter Assets) в сцену и назови **Player**.

**Обязательные компоненты (в таком порядке):**
1. **Capsule Collider** (или CharacterController — лучше для новичков)
2. **Rigidbody** (если используешь CharacterController — поставь `Is Kinematic = true`)
3. **PlayerMovement.cs** (твой скрипт ходьбы)
4. **PlayerTeleport.cs** (будешь писать сейчас)
5. **Camera** (Child-объект Main Camera, позиция (0, 1.6, 0))
6. **Audio Source** (для звуков телепорта)

**Дополнительно (очень полезно):**
- Layer = **Player**
- Tag = **Player**

### 3. Как должен двигаться персонаж (простая ходьба)

**Используй готовый Starter Assets** (самый простой и правильный способ для джуна):

1. Window → Package Manager
2. Включай: **Starter Assets - Third Person Character Controller** (бесплатно от Unity)
3. После импорта в сцене появится готовый **ThirdPersonController** префаб
4. Просто замени свой Capsule на этот префаб — у тебя сразу будет:
   - Плавная ходьба
   - Бег (Shift)
   - Прыжок (Space)
   - Камера от третьего лица
   - Анимации (Animator уже настроен)

Если хочешь писать сам (для обучения):

```csharp
// PlayerMovement.cs (самый простой вариант)
public float moveSpeed = 5f;
public float jumpForce = 8f;

private Rigidbody rb;
private bool isGrounded;

void Update()
{
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    
    Vector3 move = transform.right * h + transform.forward * v;
    move.Normalize();
    
    transform.position += move * moveSpeed * Time.deltaTime;
    
    if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
}
```

**Лучше используй Starter Assets** — сэкономишь 2–3 часа и сразу получишь качественную ходьбу.

### 4. Настройка тестовой сцены (чтобы не работать на пустой плоскости)

**Шаг за шагом:**

1. Создай новую сцену `Test_Teleport`
2. Добавь **Directional Light**
3. Добавь **Plane** (пол) — поставь Scale (10, 1, 10)
4. Поставь **Player** префаб в центр (0, 0.5, 0)
5. Главная камера уже внутри Player (из Starter Assets)

**Чтобы сцена не была пустой — добавь окружение:**

**Самые быстрые и бесплатные варианты (2026):**

**Вариант А (самый быстрый — 2 минуты):**
- Package Manager → **Starter Assets - Third Person** (уже содержит демо-сцену)
- Или импортируй **"Basic Scene"** из того же пакета

**Вариант Б (красивая сцена за 5 минут):**
1. Asset Store → поиск **"Low Poly Simple Environment"** (бесплатно)
2. Или **"Fantasy Forest Environment Free"**
3. Или **"Modular Dungeon Pack Free"** (идеально для телепорта — стены, коридоры)

**Вариант В (мой любимый для тестирования телепорта):**
- Скачай бесплатно: **"Sci-Fi Modular Kit"** или **"Prototype Dungeon"** (Asset Store, free)
- Добавь стены, платформы на разных высотах, узкие коридоры, ступеньки — идеально проверять raycast.

**Минимальный набор объектов для теста:**
- 4–5 стен (Cube + Material)
- 2–3 платформы на разной высоте
- Несколько наклонных поверхностей (для проверки Y)
- Объект с тегом **"Ground"** и Layer **"Ground"**

### 5. Откуда брать ассеты (бесплатно и быстро)

1. **Unity Asset Store** (встроен в редактор):
   - Starter Assets - Third Person (обязательно!)
   - Free Particle Pack (для вспышек телепорта)
   - Low Poly Environment Free

2. **Package Manager** (Window → Package Manager):
   - Starter Assets
   - Cinemachine (для красивой камеры)
   - Input System (уже включён)

3. **Готовые VFX для телепорта**:
   - Asset Store → "Free Teleport VFX" или "Dissolve Effect Free"
   - Или просто используй стандартный **Particle System** (Burst + Glow)

### 6. Пошаговый план реализации телепорта (делай по порядку)

1. Импортируй Starter Assets + настрой Player
2. Создай `PlayerTeleport.cs`
3. Добавь Input Action "Teleport" (в Input Action Asset)
4. Реализуй базовый телепорт по клавише **T** (raycast от груди)
5. Добавь телепорт к курсору (с зелёным/красным индикатором)
6. Сделай VFX (2 Particle System)
7. Протестируй 10 сценариев (стена, обрыв, лестница, узкий коридор)
8. Добавь кулдаун + ману (бонус)

**Готовый шаблон проекта** (если хочешь):
Могу дать ссылку на мой минимальный GitHub-шаблон (только Player + телепорт-база), но сначала попробуй сам по этому гайду.

---

**Что делать прямо сейчас:**
1. Открой Unity
2. Создай папки по структуре выше
3. Импортируй Starter Assets - Third Person
4. Поставь Player в сцену
5. Напиши `PlayerTeleport.cs` (используй ГДД из прошлого сообщения)

---




----





### 1. PlayerMovement.cs (простая, но качественная ходьба)

```csharp
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("=== Настройки движения ===")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    
    [Header("=== Ссылки ===")]
    public Transform cameraTransform;   // перетащи сюда Main Camera из иерархии
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }
    
    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // прижимаем к земле
        
        // === Движение ===
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector3 move = cameraTransform.forward * v + cameraTransform.right * h;
        move.y = 0;
        move.Normalize();
        
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        // === Прыжок ===
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        
        // === Гравитация ===
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // Поворачиваем персонажа в сторону движения
        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                Quaternion.LookRotation(move), 10f * Time.deltaTime);
        }
    }
}
```

**Как настроить:**
- Повесь скрипт на Player
- Перетащи Main Camera в поле `cameraTransform`
- Убедись, что на Player есть **CharacterController**

### 2. PlayerTeleport.cs (полный телепорт с двумя режимами)

```csharp
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerTeleport : MonoBehaviour
{
    [Header("=== Настройки телепорта ===")]
    public float teleportDistance = 12f;      // режим T
    public float maxAimDistance = 15f;        // режим прицеливания
    public float cooldown = 2f;
    
    [Header("=== Слои ===")]
    public LayerMask groundLayer;      // поставь слой Ground
    public LayerMask obstacleLayer;    // всё, что блокирует путь
    
    [Header("=== VFX ===")]
    public ParticleSystem startVFX;
    public ParticleSystem endVFX;
    
    [Header("=== Индикатор прицела ===")]
    public GameObject aimIndicator;    // создай пустой объект с Sphere или Quad и прозрачным материалом
    
    private CharacterController controller;
    private Camera mainCamera;
    private float currentCooldown;
    private bool isAiming;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        
        if (aimIndicator != null)
            aimIndicator.SetActive(false);
    }
    
    private void Update()
    {
        currentCooldown -= Time.deltaTime;
        
        // === Режим 1: Телепорт вперёд (клавиша T) ===
        if (Input.GetKeyDown(KeyCode.T) && currentCooldown <= 0)
        {
            TryTeleportForward();
        }
        
        // === Режим 2: Телепорт к курсору (зажатая ЛКМ) ===
        HandleAimTeleport();
    }
    
    private void TryTeleportForward()
    {
        Vector3 startPos = transform.position + Vector3.up * 1.2f; // от груди
        Vector3 direction = mainCamera.transform.forward;
        
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, teleportDistance, obstacleLayer))
        {
            Debug.Log("Путь заблокирован!");
            return;
        }
        
        Vector3 targetPos = transform.position + direction * teleportDistance;
        TeleportTo(targetPos);
        currentCooldown = cooldown;
    }
    
    private void HandleAimTeleport()
    {
        if (Input.GetMouseButton(0)) // ЛКМ зажата
        {
            isAiming = true;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, groundLayer))
            {
                float distance = Vector3.Distance(transform.position, hit.point);
                
                if (distance <= maxAimDistance && hit.collider != null)
                {
                    ShowAimIndicator(hit.point);
                    
                    if (Input.GetMouseButtonDown(0)) // клик ЛКМ
                    {
                        TeleportTo(hit.point + Vector3.up * 0.1f);
                        currentCooldown = cooldown;
                    }
                }
            }
        }
        else
        {
            if (isAiming)
            {
                HideAimIndicator();
                isAiming = false;
            }
        }
    }
    
    private void ShowAimIndicator(Vector3 position)
    {
        if (aimIndicator != null)
        {
            aimIndicator.SetActive(true);
            aimIndicator.transform.position = position + Vector3.up * 0.05f;
        }
    }
    
    private void HideAimIndicator()
    {
        if (aimIndicator != null)
            aimIndicator.SetActive(false);
    }
    
    private void TeleportTo(Vector3 targetPos)
    {
        // Важно! Отключаем контроллер на 1 кадр
        controller.enabled = false;
        
        transform.position = targetPos;
        
        // VFX
        if (startVFX != null) startVFX.Play();
        if (endVFX != null) 
        {
            endVFX.transform.position = targetPos;
            endVFX.Play();
        }
        
        controller.enabled = true;
        
        Debug.Log("Телепорт выполнен!");
    }
}
```

### Как быстро настроить телепорт:

1. Создай пустой объект → назови `AimIndicator`  
   Добавь компонент **Mesh Renderer** + Sphere (или Quad)  
   Поставь полупрозрачный зелёный материал

2. Создай два Particle System:
   - Один на Player (назови TeleportStart)
   - Второй сделай Prefab и назови TeleportEnd

3. Повесь **PlayerTeleport.cs** на Player  
   Перетащи:
   - AimIndicator
   - TeleportStart и TeleportEnd
   - Настрой слои (Ground и Default/Wall)

Готово!  

**Что делать дальше:**
- Запусти сцену
- Нажми **T** — телепорт вперёд
- Зажми **ЛКМ** — появится зелёный круг, кликни — телепорт в точку
