-- Flying Saucer Attraction Script (Roblox Studio)
-- Автор: Grok (адаптировано для аттракциона)

local saucer = script.Parent  -- Главная модель или MainBody
local mainPart = saucer:WaitForChild("MainBody") or saucer.PrimaryPart  -- Основная часть тарелки

-- Настройки (меняй под себя)
local rotationSpeed = 2.5          -- Скорость вращения вокруг своей оси (чем больше — быстрее)
local flightRadius = 20            -- Радиус круга полёта (в studs)
local flightHeight = 40            -- Средняя высота полёта
local flightSpeed = 0.8            -- Скорость движения по кругу (медленнее = плавнее)
local upDownAmplitude = 5          -- Амплитуда "качки" вверх-вниз
local upDownSpeed = 1.2            -- Скорость качки

local RunService = game:GetService("RunService")

-- Создаём constraints один раз (современный способ вместо старых Body*)
local angularVelocity = Instance.new("AngularVelocity")
angularVelocity.Attachment0 = mainPart:FindFirstChild("CenterAttachment") or Instance.new("Attachment", mainPart)
angularVelocity.MaxTorque = math.huge
angularVelocity.AngularVelocity = Vector3.new(0, rotationSpeed, 0)  -- Вращение только по Y (вокруг вертикали)
angularVelocity.Parent = mainPart

local linearVelocity = Instance.new("LinearVelocity")
linearVelocity.Attachment0 = angularVelocity.Attachment0
linearVelocity.MaxForce = math.huge
linearVelocity.Parent = mainPart

-- Время для синусоиды (для плавного движения)
local timeOffset = 0

RunService.Heartbeat:Connect(function(dt)
    timeOffset += dt * flightSpeed
    
    -- Позиция по кругу + лёгкая качка вверх-вниз
    local x = math.sin(timeOffset) * flightRadius
    local z = math.cos(timeOffset) * flightRadius
    local y = flightHeight + math.sin(timeOffset * upDownSpeed) * upDownAmplitude
    
    local targetPosition = Vector3.new(x, y, z)  -- Центр круга в (0, flightHeight, 0). Если хочешь центр в другом месте — добавь offset.
    
    -- Плавно двигаем тарелку
    linearVelocity.VectorVelocity = (targetPosition - mainPart.Position) * 10  -- Чем больше множитель — тем резче реакция
    
    -- Дополнительно можно поворачивать тарелку "носом" по направлению движения (опционально)
    -- mainPart.CFrame = CFrame.lookAt(mainPart.Position, targetPosition) * CFrame.Angles(0, math.rad(90), 0) -- подкрути если нужно
end)

print("Летающая тарелка запущена! Вращение и полёт активны.")







Вот пример скрипта для твоей «летающей тарелки» с двумя движениями:

- **постоянное вращение** вокруг вертикальной оси (круг с сиденьями крутится)
- **плавное покачивание / наклон** в стороны (на шарнире, как будто тарелка немного качается из стороны в сторону)

Предполагаем такую структуру модели:

```
FlyingSaucer (Model)
├─ CentralCylinder (Part — центральный цилиндр, Anchored = true)
├─ Platform (Part/Mesh — круглая платформа с сиденьями, Anchored = false)
├─ HingeTilt (HingeConstraint)  ← для качания в стороны
│   ├─ Attachment0 (в CentralCylinder, смотри вверх)
│   └─ Attachment1 (в Platform, смотри вверх)
└─ RotationMotor (CylinderConstraint или HingeConstraint)  ← для вращения по Y
    ├─ Attachment0 (в CentralCylinder или в невидимой неподвижной части)
    └─ Attachment1 (в Platform, в центре)
```

### Скрипт (ServerScript внутри модели FlyingSaucer)

```lua
-- ServerScriptService → FlyingSaucer → Script

local RunService = game:GetService("RunService")

local model     = script.Parent
local platform  = model:WaitForChild("Platform")

local hingeTilt = platform:FindFirstChild("HingeTilt")     -- HingeConstraint для качания
local motorRot  = platform:FindFirstChild("RotationMotor") -- CylinderConstraint или HingeConstraint для вращения

if not hingeTilt or not motorRot then
    warn("Не найдены HingeTilt или RotationMotor!")
    return
end

-- ────────────────────────────────────────────────
-- Настройки
-- ────────────────────────────────────────────────

local SPIN_SPEED_RPM     =  18      -- обороты в минуту (постоянное вращение)
local TILT_MAX_ANGLE     =  12      -- максимальный наклон в градусах (±12°)
local TILT_PERIOD        =  5.8     -- период одного качания туда-обратно (сек)
local TILT_SMOOTHNESS    =  0.92    -- насколько плавно меняется скорость наклона (1 = мгновенно, <1 = инерция)

-- ────────────────────────────────────────────────
-- Постоянное вращение (очень просто)
-- ────────────────────────────────────────────────

local spinRadPerSec = SPIN_SPEED_RPM * (2 * math.pi / 60)

if motorRot:IsA("CylinderConstraint") or motorRot:IsA("HingeConstraint") then
    motorRot.ActuatorType     = Enum.ActuatorType.Motor
    motorRot.AngularVelocity  = spinRadPerSec
    motorRot.AngularSpeed     = spinRadPerSec   -- на всякий случай
    motorRot.MotorMaxTorque   = 1e6             -- сильно, чтобы не тормозило
else
    warn("RotationMotor должен быть CylinderConstraint или HingeConstraint!")
end

-- ────────────────────────────────────────────────
-- Плавное качание (синусоида + инерция)
-- ────────────────────────────────────────────────

local currentTiltVel   = 0
local timePassed       = 0

RunService.Heartbeat:Connect(function(dt)
    timePassed += dt

    -- желаемый угол по синусоиде
    local targetAngle = TILT_MAX_ANGLE * math.sin(timePassed * (2 * math.pi / TILT_PERIOD))

    -- разница между желаемым и текущим
    local error = targetAngle - hingeTilt.CurrentAngle

    -- желаемая скорость
    local targetVel = error * 15   -- коэффициент жёсткости (15–25 обычно хорошо)

    -- плавно подгоняем текущую скорость
    currentTiltVel = currentTiltVel * TILT_SMOOTHNESS + targetVel * (1 - TILT_SMOOTHNESS)

    -- применяем
    hingeTilt.AngularVelocity = currentTiltVel
end)

print("Летающая тарелка запущена: вращение", SPIN_SPEED_RPM, "об/мин + качание ±", TILT_MAX_ANGLE, "°")
```

### Вариант 2 — если хочешь более резкие / дискретные качания (как в первом твоём скрипте)

```lua
-- Альтернатива: качание импульсами (туда-сюда)

local MAX_TILT       = 14
local TILT_SPEED     = 0.9     -- рад/с целевая
local STEP           = 0.018
local STOP_THRESHOLD = 0.04

local function accelerateTilt(dir)
    local target = dir * TILT_SPEED
    while math.abs(hingeTilt.CurrentAngle) < MAX_TILT do
        if math.abs(hingeTilt.AngularVelocity) < math.abs(target) then
            hingeTilt.AngularVelocity += dir * STEP
        end
        task.wait()
    end
end

local function decelerateTilt()
    while math.abs(hingeTilt.AngularVelocity) > STOP_THRESHOLD do
        hingeTilt.AngularVelocity *= 0.90
        task.wait()
    end
    hingeTilt.AngularVelocity = 0
end

task.spawn(function()
    while true do
        accelerateTilt( 1)
        decelerateTilt()
        accelerateTilt(-1)
        decelerateTilt()
        task.wait(1.2)   -- пауза между полными качаниями
    end
end)
```

### Полезные советы

- **HingeTilt** должен быть настроен так, чтобы ось вращения была **горизонтальной** (обычно по X или Z).
- Attachments в HingeTilt должны смотреть **вверх** (Front вектор вдоль оси Y).
- Если платформа слишком лёгкая — добавь **массу** (CustomPhysicalProperties) или увеличь **MotorMaxTorque**.
- Чтобы игроки не слетали при качании — можно поставить **Sit-сиденья** или невидимые **WeldConstraint** во время аттракциона (но это уже отдельная механика).

Удачи с аттракционом! 🛸