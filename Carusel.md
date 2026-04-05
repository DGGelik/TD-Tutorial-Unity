Вот максимально простая и рабочая версия карусели «Летающая тарелка» в Roblox Studio.
Делаем только из базовых Part (цилиндры и блоки), без Toolbox, без красивых моделей, без лишних эффектов. Главное — чтобы вращалось и работало.
Шаг 1: Создаём главную модель

В Workspace создай новую Model.
Назови модель SimpleCarousel.
Всё, что будет дальше, делай внутри этой Model.

Шаг 2: Центральная колонна (неподвижная ось)

Вставь Part → назови его CentralPole.
В Properties:
Shape → Cylinder
Size → 4, 40, 4 (толстый и высокий столб)
Position → примерно 0, 20, 0
Anchored = true (обязательно!)
CanCollide = true (по желанию)

В CentralPole добавь Attachment:
Назови PoleAttachment
Поставь его вверху колонны (примерно на высоте 35–38 studs, Position внутри Part: 0, 18, 0)


Шаг 3: Вращающаяся платформа

Вставь новый Part → назови RotatingPlatform.
В Properties:
Shape → Cylinder
Size → 35, 2, 35 (большой плоский диск)
Position → поставь точно над верхом колонны (примерно 0, 37, 0)
Anchored = false
CanCollide = true

В RotatingPlatform добавь Attachment:
Назови PlatformCenter
Поставь его ровно в центр платформы (Position внутри Part: 0, 0, 0)


Шаг 4: Соединяем платформу с колонной (чтобы она крутилась)

Выдели Model → Constraints → HingeConstraint.
В свойствах HingeConstraint:
Attachment0 → CentralPole.PoleAttachment
Attachment1 → RotatingPlatform.PlatformCenter
ActuatorType → Motor (это заставит крутиться автоматически)
AngularVelocity → 1.5 (скорость вращения, можешь менять от 0.5 до 3)
LimitsEnabled → false


Теперь платформа должна крутиться вокруг колонны.
Шаг 5: Создаём одну простую летающую тарелку

Вставь новый Part → назови SaucerBody.
В Properties:
Shape → Cylinder
Size → 10, 3, 10 (сплющенная тарелка)
Anchored = false

Добавь в SaucerBody Attachment:
Назови SaucerAttach
Поставь его в центр тарелки (0, 0, 0)

(Опционально, но рекомендуется) Добавь сиденье:
Вставь Seat (не VehicleSeat).
Положи его сверху на тарелку.
Сделай WeldConstraint между Seat и SaucerBody (чтобы сиденье не отвалилось).


Шаг 6: Прикрепляем тарелку к платформе

В RotatingPlatform создай новый Attachment:
Назови Arm1
Поставь его на краю платформы (например, Position внутри платформы: 15, 1, 0)

Вставь RigidConstraint (Constraints → RigidConstraint):
Attachment0 → RotatingPlatform.Arm1
Attachment1 → SaucerBody.SaucerAttach
Enabled = true


Тарелка теперь жёстко прикреплена к платформе и будет летать по кругу вместе с ней.
Шаг 7: Делаем несколько тарелок (самый быстрый способ)

Выдели всю модель тарелки (SaucerBody + Seat + все её Attachments и Constraints, которые к ней относятся).
Нажми Ctrl + D (дублировать) 5–7 раз.
Переименуй тарелки: Saucer1, Saucer2 и т.д.
Для каждой новой тарелки:
Перемести её Attachment на платформе (Arm2, Arm3 и т.д.) по кругу (используй инструмент Move и поворачивай платформу, чтобы равномерно расставить).
Пример позиций на платформе (радиус ~15–17):
Arm1: 15, 1, 0
Arm2: 10, 1, 12
Arm3: -5, 1, 16
и т.д. по кругу.



Шаг 8: Простой скрипт для дополнительного вращения тарелок (по желанию)
Если хочешь, чтобы каждая тарелка ещё и крутилась вокруг своей оси (как настоящая карусель):
Вставь Script внутрь модели SimpleCarousel:
Lualocal platform = script.Parent:WaitForChild("RotatingPlatform")

-- Дополнительное вращение самих тарелок
while task.wait() do
    for _, saucer in ipairs(script.Parent:GetChildren()) do
        if saucer.Name:find("Saucer") and saucer:FindFirstChild("SaucerBody") then
            local body = saucer.SaucerBody
            if body then
                body.CFrame = body.CFrame * CFrame.Angles(0, 0.05, 0)  -- медленное вращение
            end
        end
    end
end
Важные проверки:

CentralPole.Anchored = true
RotatingPlatform.Anchored = false
Все тарелки Anchored = false
Все соединения внутри одной тарелки — WeldConstraint
Соединение тарелки с платформой — RigidConstraint
Соединение платформы с колонной — HingeConstraint с Motor

Что делать, если не крутится или дёргается:

Убедись, что Attachments стоят точно в нужных местах.
В HingeConstraint поставь больший AngularVelocity (например 2–3).
Можно вместо Motor в HingeConstraint использовать AngularVelocity constraint отдельно (но Hinge + Motor проще для начала).