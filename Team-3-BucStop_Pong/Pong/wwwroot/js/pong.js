/*
 * Ping-Pong Fever! (Extended + Mobile)
 * Base game created by straker on GitHub
 * Extended by Chris Seals, Jacob Klucher.
 * Later edited by ChatGPT and Joe Neglia.
 * Mobile/responsive & container-fit pass by ChatGPT.
 */

const canvas = document.getElementById('game');
const context = canvas.getContext('2d');

let dpr = window.devicePixelRatio || 1;

// Resize canvas to match CONTAINER, not screen.
function sizeCanvas() {
  // Fixed, non-responsive canvas size (matches Snake & Tetris constraint style)
  canvas.width = 400;
  canvas.height = 400;

  canvas.style.width = canvas.width + 'px';
  canvas.style.height = canvas.height + 'px';

  context.setTransform(1, 0, 0, 1, 0, 0);
}

// Helpers for readable code
function canvasWidth() { return canvas.width; }
function canvasHeight() { return canvas.height; }
function clamp(v, lo, hi) { return Math.max(lo, Math.min(hi, v)); }

// Game variables
const grid = 15;
const paddleWidth = grid * 5;
let paddleSpeed = 10;
let ballSpeed = 8;

let playerScore = 0;
let computerScore = 0;
let resetting = false;
let running = false;
let loopId = null;

// Objects
const topPaddle = { x: 0, y: grid * 2, width: paddleWidth, height: grid, dx: 0 };
const bottomPaddle = { x: 0, y: 0, width: paddleWidth, height: grid, dx: 0 };
const ball = { x: 0, y: 0, width: grid, height: grid, dx: 0, dy: 0 };

// Utility
function randSign() { return Math.random() < 0.5 ? -1 : 1; }
function randBetween(min, max) { return min + Math.random() * (max - min); }

function randomLaunch(speed) {
  const angle = randBetween(25, 45) * Math.PI / 180;
  return {
    dx: Math.cos(angle) * speed * randSign(),
    dy: Math.sin(angle) * speed * randSign()
  };
}

function collides(a, b) {
  return a.x < b.x + b.width &&
         a.x + a.width > b.x &&
         a.y < b.y + b.height &&
         a.y + a.height > b.y;
}

// Spin effect for depth
function addSpin(paddle) {
  const ballCenter = ball.x + ball.width / 2;
  const paddleCenter = paddle.x + paddle.width / 2;
  const offset = (ballCenter - paddleCenter) / (paddle.width / 2);
  ball.dx += offset * 2;

  const maxSpeed = 11;
  const speed = Math.hypot(ball.dx, ball.dy);
  if (speed > maxSpeed) {
    const k = maxSpeed / speed;
    ball.dx *= k;
    ball.dy *= k;
  }
}

// AI logic (adaptive, slightly human-like)
const aiPaddleBaseSpeed = 6;
let aiLagTimer = 0;
let targetOffset = 0;

function controlAIPaddle() {
  const interested = ball.dy < 0;

  if (aiLagTimer > 0) aiLagTimer--;
  else {
    aiLagTimer = Math.floor(randBetween(3, 7));
    if (interested) targetOffset = randBetween(-14, 14);
  }

  const targetX = (interested
    ? ball.x + ball.width / 2 + targetOffset
    : canvasWidth() / 2) - topPaddle.width / 2;

  const dx = targetX - topPaddle.x;

  if (Math.abs(dx) > 5)
    topPaddle.dx = Math.sign(dx) * Math.min(Math.abs(dx), aiPaddleBaseSpeed);
  else
    topPaddle.dx = 0;
}

function maxPaddleX() { return canvasWidth() - grid - paddleWidth; }

// Reset positions
function resetGame() {
  ball.x = canvasWidth() / 2 - grid / 2;
  ball.y = canvasHeight() / 2 - grid / 2;

  const v = randomLaunch(ballSpeed + randBetween(-1, 1.5));
  ball.dx = v.dx;
  ball.dy = v.dy;

  topPaddle.x = canvasWidth() / 2 - paddleWidth / 2;
  bottomPaddle.x = canvasWidth() / 2 - paddleWidth / 2;
  bottomPaddle.y = canvasHeight() - grid * 3;

  resetting = false;
}

function endGame() {
  running = false;
  cancelAnimationFrame(loopId);

  context.clearRect(0, 0, canvasWidth(), canvasHeight());
  context.textAlign = 'center';
  context.font = '36px Arial';
  context.fillText(`${playerScore === 7 ? "Player" : "Computer"} wins!`, canvasWidth()/2, canvasHeight()/2);
  context.font = '22px Arial';
  context.fillText('Press Space or Tap to Restart', canvasWidth()/2, canvasHeight()/2 + 40);
}

function showStartScreen() {
  context.textAlign = 'center';
  context.font = '32px Arial';
  context.fillText('Press Space or Tap to Start', canvasWidth()/2, canvasHeight()/2);
}

// Game Loop
function loop() {
  loopId = requestAnimationFrame(loop);

  context.clearRect(0, 0, canvasWidth(), canvasHeight());
  controlAIPaddle();

  topPaddle.x = clamp(topPaddle.x + topPaddle.dx, grid, maxPaddleX());
  bottomPaddle.x = clamp(bottomPaddle.x + bottomPaddle.dx, grid, maxPaddleX());

  context.fillRect(topPaddle.x, topPaddle.y, paddleWidth, grid);
  context.fillRect(bottomPaddle.x, bottomPaddle.y, paddleWidth, grid);

  ball.x += ball.dx;
  ball.y += ball.dy;

  if (ball.x < grid || ball.x + grid > canvasWidth() - grid) ball.dx *= -1;

  if (ball.y > canvasHeight() && !resetting) {
    computerScore++;
    return computerScore === 7 ? endGame() : (resetting = true, setTimeout(resetGame, 700));
  }
  if (ball.y < 0 && !resetting) {
    playerScore++;
    return playerScore === 7 ? endGame() : (resetting = true, setTimeout(resetGame, 700));
  }

  if (collides(ball, topPaddle)) { ball.dy = Math.abs(ball.dy); addSpin(topPaddle); }
  if (collides(ball, bottomPaddle)) { ball.dy = -Math.abs(ball.dy); addSpin(bottomPaddle); }

  context.fillRect(ball.x, ball.y, grid, grid);

  context.font = '24px Arial';
  context.textAlign = 'left';
  context.fillText(`Player: ${playerScore}`, 20, 30);
  context.textAlign = 'right';
  context.fillText(`Computer: ${computerScore}`, canvasWidth()-20, 30);
}

// Controls
document.addEventListener('keydown', e => {
  if (e.key === 'ArrowLeft') bottomPaddle.dx = -paddleSpeed;
  else if (e.key === 'ArrowRight') bottomPaddle.dx = paddleSpeed;
  else if (e.code === 'Space') requestStart();
});
document.addEventListener('keyup', e => {
  if (e.key === 'ArrowLeft' || e.key === 'ArrowRight') bottomPaddle.dx = 0;
});

// Touch support
canvas.addEventListener('pointerdown', e => { requestStart(); movePaddle(e); dragging = true; });
canvas.addEventListener('pointermove', e => dragging && movePaddle(e));
canvas.addEventListener('pointerup', () => dragging = false);

function movePaddle(e) {
  const rect = canvas.getBoundingClientRect();
  const x = (e.clientX - rect.left) - paddleWidth/2;
  bottomPaddle.x = clamp(x, grid, maxPaddleX());
}

// Start Handler
function requestStart() {
  if (running) return;
  if (playerScore === 7 || computerScore === 7) { playerScore = 0; computerScore = 0; }
  running = true;
  resetGame();
  loop();
}

// Init
sizeCanvas();
resetGame();
showStartScreen();
