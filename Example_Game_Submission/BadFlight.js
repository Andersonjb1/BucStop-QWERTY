
const canvas = document.getElementById('game');
const ctx = canvas.getContext('2d');

const plane = {
    x: canvas.width / 2 - 15,
    y: canvas.height - 60,
    width: 30,
    height: 40,
    speed: 5
};

let obstacles = [];
let score = 0;
let gameOver = false;

// Key press state
const keys = {
    left: false,
    right: false,
    up: false,
    down: false
};

// Event listeners
document.addEventListener('keydown', e => {
    if(e.key === 'ArrowLeft') keys.left = true;
    if(e.key === 'ArrowRight') keys.right = true;
    if(e.key === 'ArrowUp') keys.up = true;
    if(e.key === 'ArrowDown') keys.down = true;

    // Reset game on Spacebar if game over
    if(e.key === ' ' && gameOver) resetGame();
});
document.addEventListener('keyup', e => {
    if(e.key === 'ArrowLeft') keys.left = false;
    if(e.key === 'ArrowRight') keys.right = false;
    if(e.key === 'ArrowUp') keys.up = false;
    if(e.key === 'ArrowDown') keys.down = false;
});

// Generate obstacles
function spawnObstacle() {
    const width = Math.random() * 50 + 20;
    const x = Math.random() * (canvas.width - width);
    obstacles.push({x: x, y: -50, width: width, height: 20, speed: 3});
}

// Update game state
function update() {
    if(gameOver) return;

    // Move plane
    if(keys.left && plane.x > 0) plane.x -= plane.speed;
    if(keys.right && plane.x + plane.width < canvas.width) plane.x += plane.speed;
    if(keys.up && plane.y > 0) plane.y -= plane.speed;
    if(keys.down && plane.y + plane.height < canvas.height) plane.y += plane.speed;

    // Move obstacles
    obstacles.forEach(o => o.y += o.speed);

    // Remove off-screen obstacles
    obstacles = obstacles.filter(o => o.y < canvas.height);

    // Check collisions
    obstacles.forEach(o => {
        if(plane.x < o.x + o.width &&
           plane.x + plane.width > o.x &&
           plane.y < o.y + o.height &&
           plane.y + plane.height > o.y) {
            gameOver = true;
        }
    });

    // Increase score
    score += 0.01;
}

// Draw everything
function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Draw plane
    ctx.fillStyle = 'blue';
    ctx.fillRect(plane.x, plane.y, plane.width, plane.height);

    // Draw obstacles
    ctx.fillStyle = 'white';
    obstacles.forEach(o => ctx.fillRect(o.x, o.y, o.width, o.height));

    // Draw score
    ctx.fillStyle = 'black';
    ctx.font = '16px Arial';
    ctx.fillText('Score: ' + Math.floor(score), 10, 20);

    // Game over
    if(gameOver){
        ctx.fillStyle = 'red';
        ctx.font = '30px Arial';
        ctx.fillText('GAME OVER', 80, canvas.height/2);
        ctx.font = '16px Arial';
        ctx.fillText('Press SPACE to restart', 100, canvas.height/2 + 30);
    }
}

// Reset game
function resetGame() {
    plane.x = canvas.width / 2 - 15;
    plane.y = canvas.height - 60;
    obstacles = [];
    score = 0;
    gameOver = false;
}

// Main game loop
function loop() {
    update();
    draw();
    requestAnimationFrame(loop);
}

// Spawn obstacles periodically
setInterval(() => {
    if(!gameOver) spawnObstacle();
}, 1500);

// Start the game
loop();
