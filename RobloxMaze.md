local model = script.Parent

-- ================== НАСТРОЙКИ (меняй здесь!) ==================
local MAZE_WIDTH = 12      -- ширина лабиринта в клетках (рекомендую 8–25)
local MAZE_HEIGHT = 10     -- высота лабиринта в клетках
local CELL_SIZE = 8        -- размер одной клетки (чем больше — тем крупнее лабиринт)
local WALL_HEIGHT = 6      -- высота стен
local WALL_THICKNESS = 1.5 -- толщина стен
-- ===============================================================

local wallsFolder = Instance.new("Folder")
wallsFolder.Name = "Walls"
wallsFolder.Parent = model

local floorsFolder = Instance.new("Folder")
floorsFolder.Name = "Floors"
floorsFolder.Parent = model

local cells = {}
local directions = {
	{dx = 0,  dy = -1, wall = "north", opposite = "south"},
	{dx = 0,  dy = 1,  wall = "south", opposite = "north"},
	{dx = 1,  dy = 0,  wall = "east",  opposite = "west"},
	{dx = -1, dy = 0,  wall = "west",  opposite = "east"},
}

local function shuffleDirections()
	local dirs = {}
	for _, v in ipairs(directions) do table.insert(dirs, v) end
	for i = #dirs, 2, -1 do
		local j = math.random(i)
		dirs[i], dirs[j] = dirs[j], dirs[i]
	end
	return dirs
end

local function carve(x, y)
	cells[x][y].visited = true
	
	local dirs = shuffleDirections()
	for _, dir in ipairs(dirs) do
		local nx = x + dir.dx
		local ny = y + dir.dy
		
		if nx >= 1 and nx <= MAZE_WIDTH and ny >= 1 and ny <= MAZE_HEIGHT then
			if not cells[nx][ny].visited then
				-- убираем стену между клетками
				cells[x][y].walls[dir.wall] = false
				cells[nx][ny].walls[dir.opposite] = false
				carve(nx, ny)
			end
		end
	end
end

local function generateMaze()
	-- Очищаем старый лабиринт
	wallsFolder:ClearAllChildren()
	floorsFolder:ClearAllChildren()
	
	-- Создаём сетку клеток
	cells = {}
	for x = 1, MAZE_WIDTH do
		cells[x] = {}
		for y = 1, MAZE_HEIGHT do
			cells[x][y] = {
				visited = false,
				walls = {north = true, south = true, east = true, west = true}
			}
		end
	end
	
	math.randomseed(tick())
	
	-- Генерируем лабиринт (Recursive Backtracker)
	carve(1, 1)
	
	-- Открываем ВХОД и ВЫХОД
	cells[1][1].walls.west = false        -- вход слева сверху
	cells[MAZE_WIDTH][MAZE_HEIGHT].walls.east = false  -- выход справа снизу
	
	-- ================== СТРОИМ СТЕНЫ ==================
	-- Вертикальные стены (west + east)
	for vx = 0, MAZE_WIDTH do
		for y = 1, MAZE_HEIGHT do
			local hasWall = false
			
			if vx == 0 or vx == MAZE_WIDTH then
				-- наружные стены
				hasWall = true
				-- открываем вход и выход
				if (vx == 0 and y == 1) or (vx == MAZE_WIDTH and y == MAZE_HEIGHT) then
					hasWall = false
				end
			else
				-- внутренняя стена
				hasWall = cells[vx][y].walls.east
			end
			
			if hasWall then
				local wall = Instance.new("Part")
				wall.Size = Vector3.new(WALL_THICKNESS, WALL_HEIGHT, CELL_SIZE)
				wall.Position = Vector3.new(vx * CELL_SIZE, WALL_HEIGHT/2, (y - 0.5) * CELL_SIZE)
				wall.Anchored = true
				wall.CanCollide = true
				wall.Color = Color3.fromRGB(80, 80, 80)
				wall.Material = Enum.Material.Concrete
				wall.Parent = wallsFolder
			end
		end
	end
	
	-- Горизонтальные стены (north + south)
	for x = 1, MAZE_WIDTH do
		for hy = 0, MAZE_HEIGHT do
			local hasWall = (hy == 0 or hy == MAZE_HEIGHT) -- наружные всегда есть
			
			if hy > 0 and hy < MAZE_HEIGHT then
				hasWall = cells[x][hy].walls.south
			end
			
			if hasWall then
				local wall = Instance.new("Part")
				wall.Size = Vector3.new(CELL_SIZE, WALL_HEIGHT, WALL_THICKNESS)
				wall.Position = Vector3.new((x - 0.5) * CELL_SIZE, WALL_HEIGHT/2, hy * CELL_SIZE)
				wall.Anchored = true
				wall.CanCollide = true
				wall.Color = Color3.fromRGB(80, 80, 80)
				wall.Material = Enum.Material.Concrete
				wall.Parent = wallsFolder
			end
		end
	end
	
	-- ================== ПОЛ (чтобы не падать) ==================
	for x = 1, MAZE_WIDTH do
		for y = 1, MAZE_HEIGHT do
			local floor = Instance.new("Part")
			floor.Size = Vector3.new(CELL_SIZE - 0.2, 0.2, CELL_SIZE - 0.2)
			floor.Position = Vector3.new((x - 0.5) * CELL_SIZE, 0.1, (y - 0.5) * CELL_SIZE)
			floor.Anchored = true
			floor.CanCollide = true
			floor.Color = Color3.fromRGB(40, 40, 40)
			floor.Material = Enum.Material.Grass
			floor.Parent = floorsFolder
		end
	end
	
	-- Спавн игрока у входа (можно убрать)
	local spawn = Instance.new("SpawnLocation")
	spawn.Position = Vector3.new(0.5 * CELL_SIZE - CELL_SIZE/2, 3, 0.5 * CELL_SIZE)
	spawn.Size = Vector3.new(4, 1, 4)
	spawn.Anchored = true
	spawn.Parent = model
	
	print("✅ Лабиринт " .. MAZE_WIDTH .. "×" .. MAZE_HEIGHT .. " сгенерирован!")
end

-- Запуск
generateMaze()