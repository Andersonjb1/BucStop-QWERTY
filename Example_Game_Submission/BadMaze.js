const canvas = document.getElementById('game');
const ctx = canvas.getContext('2d');

const WIDTH = canvas.width;
const HEIGHT = canvas.height;
const TILE = 50;

let MAP_ROWS = 7;
let MAP_COLS = 10;
let map = [];

let player = { x: 0, y: 0, angle: 0, speed: 2 };
const FOV = Math.PI / 3;
const NUM_RAYS = 120;
const MAX_DEPTH = 300;
let gameWon = false;

// --- Maze generation with guaranteed path ---
function generateMaze(rows = 7, cols = 10) {
  MAP_ROWS = rows;
  MAP_COLS = cols;
  map = Array.from({ length: rows }, () => Array(cols).fill(1)); // start with walls

  // Start spawn in top-left inner area
  let startX = 1;
  let startY = 1;
  let exitX = cols - 2;
  let exitY = rows - 2;

  // Recursive DFS maze carving
  function carve(x, y) {
    const dirs = [[1,0],[-1,0],[0,1],[0,-1]].sort(() => Math.random() - 0.5);
    for (const [dx, dy] of dirs) {
      const nx = x + dx*2;
      const ny = y + dy*2;
      if (nx > 0 && nx < cols-1 && ny > 0 && ny < rows-1 && map[ny][nx] === 1) {
        map[ny-dy][nx-dx] = 0; // carve between
        map[ny][nx] = 0;       // carve target
        carve(nx, ny);
      }
    }
  }

  map[startY][startX] = 0;
  carve(startX, startY);

  // Place exit on border (always reachable)
  map[exitY][exitX] = 2;

  // Place player at spawn
  player.x = startX * TILE + TILE / 2;
  player.y = startY * TILE + TILE / 2;
  player.angle = 0;
  gameWon = false;
}

// --- Raycasting ---
function castRays() {
  const sliceWidth = WIDTH / NUM_RAYS;
  for (let r = 0; r < NUM_RAYS; r++) {
    const rayAngle = player.angle - FOV / 2 + (r / NUM_RAYS) * FOV;
    let distance = 0;
    let hit = false;
    let hitTile = 0; // track tile type
    const sin = Math.sin(rayAngle);
    const cos = Math.cos(rayAngle);

    while (!hit && distance < MAX_DEPTH) {
      distance += 1;
      let testX = Math.floor((player.x + cos * distance) / TILE);
      let testY = Math.floor((player.y + sin * distance) / TILE);
      if (testX < 0 || testX >= MAP_COLS || testY < 0 || testY >= MAP_ROWS) {
        hit = true;
        distance = MAX_DEPTH;
        hitTile = 1;
      } else if (map[testY][testX] === 1 || map[testY][testX] === 2) {
        hit = true;
        hitTile = map[testY][testX];
      }
    }

    distance = distance * Math.cos(rayAngle - player.angle);
    const wallHeight = (TILE * 5) / (distance / 10);

    // Color: red walls, blue exit
    let color;
    if (hitTile === 2) color = `rgb(0,0,255)`; 
    else color = `rgb(${255 - Math.min(distance * 1.5, 255)},0,0)`;

    ctx.fillStyle = color;
    ctx.fillRect(r * sliceWidth, HEIGHT / 2 - wallHeight / 2, sliceWidth + 1, wallHeight);
  }
}

// --- Game loop ---
function clear() {
  ctx.fillStyle = 'gray';
  ctx.fillRect(0, 0, WIDTH, HEIGHT);
}

const keys = {};
document.addEventListener('keydown', e => keys[e.key] = true);
document.addEventListener('keyup', e => keys[e.key] = false);

function movePlayer() {
  if (gameWon) return;

  let nx = player.x;
  let ny = player.y;

  if (keys['ArrowUp']) {
    nx = player.x + Math.cos(player.angle) * player.speed;
    ny = player.y + Math.sin(player.angle) * player.speed;
  }
  if (keys['ArrowDown']) {
    nx = player.x - Math.cos(player.angle) * player.speed;
    ny = player.y - Math.sin(player.angle) * player.speed;
  }
  if (keys['ArrowLeft']) player.angle -= 0.05;
  if (keys['ArrowRight']) player.angle += 0.05;

  const mapX = Math.floor(nx / TILE);
  const mapY = Math.floor(ny / TILE);
  if (map[mapY][mapX] !== 1) {
    player.x = nx;
    player.y = ny;
  }

  if (map[mapY][mapX] === 2) {
    gameWon = true;
    setTimeout(generateMaze, 1000);
  }
}

function loop() {
  clear();
  movePlayer();
  castRays();

  if (gameWon) {
    ctx.fillStyle = 'yellow';
    ctx.font = '40px Arial';
    ctx.fillText('YOU WIN!', WIDTH / 2 - 100, HEIGHT / 2);
  }

  requestAnimationFrame(loop);
}

// Start game
generateMaze();
loop();
