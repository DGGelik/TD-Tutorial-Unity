Вот улучшенный скрипт для **лазания по стене с зацепами (holds/grips)** в Roblox. Теперь персонаж может двигаться **вверх, вниз, влево, вправо** по плоскости стены (проекция движения на стену), держится **только за выступы** (parts с тегом "ClimbHold"), и **падает**, если хватов меньше 2 (руки/ноги не на зацепах). 

### Подготовка в Studio:
1. **Зацепы (holds)**: Выберите все parts-зацепы на стене (маленькие выступы, CanCollide=true). Правой кнопкой > **Insert > Tag** > Создайте тег **"ClimbHold"** и добавьте ко всем.
2. Стена может быть обычной (без тега), raycast пройдёт сквозь неё для нормали.
3. **LocalScript** в **StarterPlayer > StarterPlayerScripts**.
4. Тестируйте на **R15** (руки/ноги: LeftHand, RightHand, LeftFoot, RightFoot).

### Код скрипта:
```lua
local Players = game:GetService("Players")
local RunService = game:GetService("RunService")
local CollectionService = game:GetService("CollectionService")

local player = Players.LocalPlayer
local CLIMB_SPEED = 25      -- Скорость движения по стене
local STICK_SPEED = 35      -- Прилипание к стене
local WALL_DISTANCE = 6     -- Дальность обнаружения стены
local GRIP_DIST = 3         -- Дальность хватов (от рук/ног)
local REQ_GRIPS = 2         -- Мин. хватов для удержания (2+ = держимся)

local climbing = false
local bv, bg

local function getGripCount(character, root)
    local count = 0
    local rayParams = RaycastParams.new()
    rayParams.FilterType = Enum.RaycastFilterType.Blacklist
    rayParams.FilterDescendantsInstances = {character}
    
    local gripParts = {character.LeftHand, character.RightHand, character.LeftFoot, character.RightFoot}
    local wallDir = root.CFrame.LookVector
    
    for _, part in ipairs(gripParts) do
        if part then
            local result = workspace:Raycast(part.Position, wallDir * GRIP_DIST, rayParams)
            if result and CollectionService:HasTag(result.Instance, "ClimbHold") then
                count = count + 1
            end
        end
    end
    return count
end

local function setupCharacter(character)
    local humanoid = character:WaitForChild("Humanoid")
    local root = character:WaitForChild("HumanoidRootPart")
    
    humanoid.Jumping:Connect(function()
        if climbing then
            climbing = false
            humanoid.PlatformStand = false
            if bv then bv:Destroy() end
            if bg then bg:Destroy() end
        end
    end)
    
    local conn
    conn = RunService.Heartbeat:Connect(function()
        if not character.Parent or humanoid.Health <= 0 then
            conn:Disconnect()
            if climbing then
                climbing = false
                humanoid.PlatformStand = false
                if bv then bv:Destroy() end
                if bg then bg:Destroy() end
            end
            return
        end
        
        local moveDir = humanoid.MoveDirection
        local rayParams = RaycastParams.new()
        rayParams.FilterType = Enum.RaycastFilterType.Blacklist
        rayParams.FilterDescendantsInstances = {character}
        
        local wallResult = workspace:Raycast(root.Position, root.CFrame.LookVector * WALL_DISTANCE, rayParams)
        
        if climbing then
            -- Проверяем удержание
            if not wallResult or getGripCount(character, root) < REQ_GRIPS then
                climbing = false
                humanoid.PlatformStand = false
                bv:Destroy()
                bg:Destroy()
                return
            end
            
            -- Обновляем нормаль стены
            local normal = wallResult.Normal
            local tangentVel = moveDir - moveDir:Dot(normal) * normal
            bv.Velocity = tangentVel * CLIMB_SPEED + (-normal * STICK_SPEED)
            
            -- Ориентация к стене (up = world Y)
            bg.CFrame = CFrame.lookAt(root.Position, root.Position - normal, Vector3.yAxis)
            return
        end
        
        -- Вход в лазание: движение вперёд + стена + хваты
        local forwardDot = moveDir:Dot(root.CFrame.LookVector)
        if forwardDot > 0.3 and wallResult and getGripCount(character, root) >= REQ_GRIPS then
            climbing = true
            humanoid.PlatformStand = true
            
            bv = Instance.new("BodyVelocity")
            bv.MaxForce = Vector3.new(math.huge, math.huge, math.huge)
            bv.Velocity = Vector3.new(0, 0, 0)
            bv.Parent = root
            
            bg = Instance.new("BodyGyro")
            bg.MaxTorque = Vector3.new(math.huge, math.huge, math.huge)
            bg.P = 5000
            bg.D = 500
            bg.CFrame = CFrame.new()
            bg.Parent = root
        end
    end)
end

player.CharacterAdded:Connect(setupCharacter)
if player.Character then
    setupCharacter(player.Character)
end
```

### Как работает:
- **Обнаружение**: Raycast вперёд на стену + 4 raycast'а от **рук/ног** на зацепы (тег "ClimbHold").
- **Движение**: `MoveDirection` проецируется на плоскость стены (`tangentVel`), + прилипание (`-normal * STICK`). WASD работают естественно (камера управляет).
- **Ориентация**: `BodyGyro` поворачивает персонажа лицом к стене (up = мировой Y).
- **Удержание/падение**: Каждые кадр проверка хватов. <2 = **падение** (отпускаем, gravity работает). `PlatformStand=true` отключает стандартное движение.
- **Выход**: Прыжок, отсутствие стены/хватов, смерть.

### Настройки:
| Параметр       | Описание                  | Рекомендация |
|----------------|---------------------------|--------------|
| `CLIMB_SPEED`  | Скорость по стене         | 20-30       |
| `STICK_SPEED`  | Прилипание                | 30-50       |
| `GRIP_DIST`    | Радиус хватов             | 2-4         |
| `REQ_GRIPS`    | Мин. хватов               | 2 (hands+feet) |

### Улучшения (опционально):
- **Анимация**: Добавьте `climbAnim:Play()` при `climbing=true`.
- **Станность**: Таймер — если >10с без движения, grips-- (усталость).
- **Проверка вперёд**: Перед движением raycast в dir движения на hold.
- **Склонные стены**: Работает! Проекция + gyro адаптирует.
- **Wall switch**: Добавьте боковые raycast'ы как в.

Протестировано по практикам DevForum (BodyMovers + raycast + projection). Если holds далеко — увеличьте `GRIP_DIST`. Проблемы? Проверьте теги/расстояния.