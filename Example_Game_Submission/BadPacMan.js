const canvas = document.getElementById('game');
const ctx = canvas.getContext('2d');

const TILE_SIZE = 20;
const ROWS = canvas.height / TILE_SIZE;
const COLS = canvas.width / TILE_SIZE;

const initialMap = [
  [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1],
  [1,2,2,2,1,2,2,2,2,1,2,2,2,2,1,2,2,2,2,1],
  [1,2,1,2,1,2,1,1,2,1,2,1,1,2,1,2,1,1,2,1],
  [1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1],
  [2,2,1,1,1,2,1,1,2,2,2,1,1,1,1,1,1,1,2,2],
  [1,2,2,2,1,2,2,2,2,2,2,2,2,2,1,2,2,2,2,1],
  [1,1,2,1,1,1,2,1,1,1,1,1,2,1,1,1,2,1,1,1],
  [1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1],
  [1,2,1,1,2,1,1,2,1,1,1,2,1,1,2,1,1,2,1,1],
  [1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,2,1],
  [1,1,2,1,1,1,1,1,2,1,2,1,1,1,1,1,1,2,1,1],
  [1,2,2,2,1,2,2,2,2,1,2,2,2,2,1,2,2,2,2,1],
  [1,2,1,1,1,2,1,1,2,1,2,1,1,2,1,2,1,1,1,1],
  [1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1],
  [1,2,1,1,2,1,1,1,2,2,2,1,1,1,2,1,1,2,1,1],
  [1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1],
  [1,1,1,2,1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1],
  [1,2,2,2,1,2,2,2,2,1,2,2,2,2,1,2,2,2,2,1],
  [1,2,1,1,1,2,1,1,2,1,2,1,1,2,1,2,1,1,1,1],
  [2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2],
  [1,2,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1],
  [1,2,2,2,1,2,2,2,2,1,2,2,2,2,1,2,2,2,2,1],
  [1,2,1,1,1,2,1,1,2,1,2,1,1,2,1,2,1,1,1,1],
  [1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1],
  [1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1]
];

let map = initialMap.map(row => [...row]); // Copy for runtime

let pacman = { x: 1, y: 1, dirX: 0, dirY: 0, radius: TILE_SIZE / 2 - 2 };

const ghosts = [
  { x: 9, y: 11, dirX: 0, dirY: -1, color: 'red' },
  { x: 10, y: 11, dirX: 0, dirY: 1, color: 'pink' },
  { x: 9, y: 12, dirX: 1, dirY: 0, color: 'cyan' },
  { x: 10, y: 12, dirX: -1, dirY: 0, color: 'orange' }
];

let gameOver = false;

// Controls
document.addEventListener('keydown', e => {
  switch (e.key) {
    case 'ArrowUp': pacman.dirX = 0; pacman.dirY = -1; break;
    case 'ArrowDown': pacman.dirX = 0; pacman.dirY = 1; break;
    case 'ArrowLeft': pacman.dirX = -1; pacman.dirY = 0; break;
    case 'ArrowRight': pacman.dirX = 1; pacman.dirY = 0; break;
    case 'r':
    case 'R':
      if (gameOver) resetGame();
      break;
  }
});

// Draw map
function drawMap() {
  for (let r = 0; r < ROWS; r++) {
    for (let c = 0; c < COLS; c++) {
      if (map[r][c] === 1) {
        ctx.fillStyle = 'blue';
        ctx.fillRect(c * TILE_SIZE, r * TILE_SIZE, TILE_SIZE, TILE_SIZE);
      } else if (map[r][c] === 2) {
        ctx.fillStyle = 'white';
        ctx.beginPath();
        ctx.arc(c * TILE_SIZE + TILE_SIZE / 2, r * TILE_SIZE + TILE_SIZE / 2, 3, 0, Math.PI * 2);
        ctx.fill();
      }
    }
  }
}

// Move Pac-Man
let moveCounter = 0;
const MOVE_SPEED = 12;
function movePacman() {
  if (gameOver) return;

  moveCounter++;
  if (moveCounter % MOVE_SPEED !== 0) return;

  let nextX = pacman.x + pacman.dirX;
  let nextY = pacman.y + pacman.dirY;

  if (nextX < 0) nextX = COLS - 1;
  if (nextX >= COLS) nextX = 0;

  if (map[nextY] && map[nextY][nextX] !== 1) {
    pacman.x = nextX;
    pacman.y = nextY;
  }

  if (map[pacman.y][pacman.x] === 2) map[pacman.y][pacman.x] = 0;
}

// Move Ghosts
let ghostMoveCounter = 0;
const GHOST_SPEED = 18; // slower than Pac-Man
function moveGhosts() {
  if (gameOver) return;

  ghostMoveCounter++;
  if (ghostMoveCounter % GHOST_SPEED !== 0) return;

  ghosts.forEach(g => {
    // Choose new direction if blocked
    let nextX = g.x + g.dirX;
    let nextY = g.y + g.dirY;
    if (!map[nextY] || map[nextY][nextX] === 1) {
      const dirs = [
        { x: 0, y: -1 },
        { x: 0, y: 1 },
        { x: -1, y: 0 },
        { x: 1, y: 0 }
      ];
      const validDirs = dirs.filter(d => map[g.y + d.y] && map[g.y + d.y][g.x + d.x] !== 1);
      const choice = validDirs[Math.floor(Math.random() * validDirs.length)];
      g.dirX = choice.x;
      g.dirY = choice.y;
    }

    g.x += g.dirX;
    g.y += g.dirY;

    if (g.x < 0) g.x = COLS - 1;
    if (g.x >= COLS) g.x = 0;
  });

  // Check collisions after moving ghosts
  ghosts.forEach(g => {
    if (g.x === pacman.x && g.y === pacman.y) gameOver = true;
  });
}

// Draw Ghosts
function drawGhosts() {
  ghosts.forEach(g => {
    ctx.fillStyle = g.color;
    ctx.beginPath();
    ctx.arc(g.x * TILE_SIZE + TILE_SIZE / 2, g.y * TILE_SIZE + TILE_SIZE / 2, TILE_SIZE / 2 - 2, 0, Math.PI * 2);
    ctx.fill();
  });
}

// Draw Pac-Man
function drawPacman() {
  ctx.fillStyle = 'yellow';
  ctx.beginPath();
  ctx.arc(pacman.x * TILE_SIZE + TILE_SIZE / 2, pacman.y * TILE_SIZE + TILE_SIZE / 2, pacman.radius, 0.25 * Math.PI, 1.75 * Math.PI);
  ctx.lineTo(pacman.x * TILE_SIZE + TILE_SIZE / 2, pacman.y * TILE_SIZE + TILE_SIZE / 2);
  ctx.fill();
}

// Reset game
function resetGame() {
  map = initialMap.map(row => [...row]);
  pacman.x = 1; pacman.y = 1; pacman.dirX = 0; pacman.dirY = 0;
  ghosts[0].x = 9; ghosts[0].y = 11; ghosts[0].dirX = 0; ghosts[0].dirY = -1;
  ghosts[1].x = 10; ghosts[1].y = 11; ghosts[1].dirX = 0; ghosts[1].dirY = 1;
  ghosts[2].x = 9; ghosts[2].y = 12; ghosts[2].dirX = 1; ghosts[2].dirY = 0;
  ghosts[3].x = 10; ghosts[3].y = 12; ghosts[3].dirX = -1; ghosts[3].dirY = 0;
  gameOver = false;
}

// Game loop
function gameLoop() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  if (gameOver) {
    ctx.fillStyle = 'black';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = 'red';
    ctx.font = '40px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('GAME OVER', canvas.width / 2, canvas.height / 2);
    ctx.font = '20px Arial';
    ctx.fillText('Press R to Restart', canvas.width / 2, canvas.height / 2 + 40);
    return;
  }

  drawMap();
  movePacman();
  moveGhosts();
  drawPacman();
  drawGhosts();
  requestAnimationFrame(gameLoop);
}

gameLoop();
