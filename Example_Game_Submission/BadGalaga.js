
const canvas = document.getElementById('game');
const ctx = canvas.getContext('2d');

const WIDTH = canvas.width;
const HEIGHT = canvas.height;

let score = 0;
let gameOver = false;

// Player
const player = {
    x: WIDTH / 2 - 15,
    y: HEIGHT - 40,
    width: 30,
    height: 20,
    speed: 5,
    bullets: []
};

// Enemies
let enemies = [];
let enemySpeed = 1;

// Controls
const keys = {};

// Event listeners
document.addEventListener('keydown', e => {
    keys[e.code] = true;
    if(e.code === 'Space') {
        if(!gameOver) shoot();
        else resetGame(); // restart on space after game over
    }
});
document.addEventListener('keyup', e => keys[e.code] = false);

// Shoot function
function shoot() {
    player.bullets.push({
        x: player.x + player.width/2 - 2,
        y: player.y,
        width: 4,
        height: 10,
        speed: 7
    });
}

// Spawn enemies
function spawnEnemies() {
    const rowCount = 3;
    const colCount = 6;
    const padding = 10;
    const enemyWidth = 30;
    const enemyHeight = 20;
    enemies = [];
    for(let r = 0; r < rowCount; r++) {
        for(let c = 0; c < colCount; c++) {
            enemies.push({
                x: c * (enemyWidth + padding) + 20,
                y: r * (enemyHeight + padding) + 20,
                width: enemyWidth,
                height: enemyHeight,
                dir: 1,  // moving right initially
                charging: false,
                chargeDir: 1 // 1=right, -1=left for horizontal wrap
            });
        }
    }
}

// Reset game
function resetGame() {
    score = 0;
    enemySpeed = 1;
    gameOver = false;
    player.x = WIDTH / 2 - 15;
    player.bullets = [];
    spawnEnemies();
    gameLoop();
}

// Update game state
function update() {
    if(gameOver) return;

    // Move player
    if(keys['ArrowLeft'] && player.x > 0) player.x -= player.speed;
    if(keys['ArrowRight'] && player.x + player.width < WIDTH) player.x += player.speed;

    // Move bullets
    player.bullets.forEach((b, i) => {
        b.y -= b.speed;
        if(b.y + b.height < 0) player.bullets.splice(i,1);
    });

    // Random enemy charge toward player
    if(Math.random() < 0.01 && enemies.length > 0) {
        const randomEnemy = enemies[Math.floor(Math.random() * enemies.length)];
        randomEnemy.charging = true;
        randomEnemy.chargeDir = Math.random() < 0.5 ? 1 : -1; // random horizontal direction
    }

    // Move enemies
    let changeDir = false;
    enemies.forEach(e => {
        if(e.charging) {
            // Move down toward player and horizontally
            e.y += 3;
            e.x += e.chargeDir * 2; // horizontal movement

            // Wrap around horizontally
            if(e.x > WIDTH) e.x = -e.width;
            if(e.x + e.width < 0) e.x = WIDTH;
        } else {
            e.x += enemySpeed * e.dir;
            if(e.x + e.width > WIDTH || e.x < 0) changeDir = true;
        }
    });
    if(changeDir) enemies.forEach(e => {
        if(!e.charging) {
            e.dir *= -1;
            e.y += 10;
        }
    });

    // Check collisions: bullets vs enemies
    player.bullets.forEach((b, i) => {
        enemies.forEach((e,j) => {
            if(b.x < e.x + e.width &&
               b.x + b.width > e.x &&
               b.y < e.y + e.height &&
               b.y + b.height > e.y) {
                   player.bullets.splice(i,1);
                   enemies.splice(j,1);
                   score += 10;
            }
        });
    });

    // Check collisions: enemies vs player
    enemies.forEach(e => {
        if(player.x < e.x + e.width &&
           player.x + player.width > e.x &&
           player.y < e.y + e.height &&
           player.y + player.height > e.y) {
               gameOver = true;
        }
    });

    // Spawn new wave if all enemies gone
    if(enemies.length === 0 && !gameOver) {
        enemySpeed += 0.2; // increase speed gradually
        spawnEnemies();
    }
}

// Draw everything
function draw() {
    ctx.clearRect(0,0,WIDTH,HEIGHT);

    // Draw player
    ctx.fillStyle = 'green';
    ctx.fillRect(player.x, player.y, player.width, player.height);

    // Draw bullets
    ctx.fillStyle = 'yellow';
    player.bullets.forEach(b => ctx.fillRect(b.x, b.y, b.width, b.height));

    // Draw enemies
    enemies.forEach(e => {
        ctx.fillStyle = e.charging ? 'orange' : 'red';
        ctx.fillRect(e.x, e.y, e.width, e.height);
    });

    // Draw score
    ctx.fillStyle = 'white';
    ctx.font = '16px Arial';
    ctx.fillText('Score: ' + score, 10, 20);

    if(gameOver) {
        ctx.fillStyle = 'white';
        ctx.font = '30px Arial';
        ctx.fillText('GAME OVER', WIDTH/2 - 90, HEIGHT/2);
        ctx.font = '16px Arial';
        ctx.fillText('Press SPACE to Restart', WIDTH/2 - 90, HEIGHT/2 + 30);
    }
}

// Game loop
function gameLoop() {
    update();
    draw();
    if(!gameOver) requestAnimationFrame(gameLoop);
}

// Start game
spawnEnemies();
gameLoop();
