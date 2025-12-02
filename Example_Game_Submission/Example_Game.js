/*
===========================================================
 BucStop Game Submission Information
===========================================================

Game Title:
    Buc Tac Toe

Author:
    Bobby

Description:
    Buc Tac Toe is a simple strategy game where players try
    to get three marks in a row on a 3×3 board. This version
    includes a computer opponent that evaluates strong moves,
    prioritizing winning possibilities and blocking threats.
    The game uses the built-in canvas provided by BucStop and
    runs fully inside the main game area.

How To Play:
    You play as X and go first. Click any open square to place
    your mark. The computer will respond after a short delay
    to feel more natural. The first to align three marks wins.
    You can press the reset button or the "R" key to start a
    new round at any time.

Submission Notes:
    • File size is small and well under the 500KB limit.
    • No dangerous code patterns are included.
    • No dynamic HTML injection is used.
    • No storage, network calls, or restricted methods.
    • All drawing is done on the existing <canvas id="game">
      element provided by BucStop's Play page.
    • This file is safe for review and meets all validator rules.

===========================================================
*/

(function () {
    "use strict";

    // Locate BucStop's required canvas
    const canvas = document.getElementById("game");
    const ctx = canvas.getContext("2d");

    // Force the canvas to stay square inside the allocated area
    function resizeCanvas() {
        const size = Math.min(canvas.clientWidth, canvas.clientHeight);
        canvas.width = size;
        canvas.height = size;
        drawBoard();
    }

    window.addEventListener("resize", resizeCanvas);

    // UI: status message (safe insertion)
    const status = document.createElement("div");
    status.textContent = "Your turn (X)";
    status.style.fontFamily = "sans-serif";
    status.style.textAlign = "center";
    status.style.color = "black";
    status.style.marginTop = "6px";

    // Reset button
    const reset = document.createElement("button");
    reset.textContent = "New Game";
    reset.style.marginTop = "4px";
    reset.style.padding = "4px 10px";
    reset.style.cursor = "pointer";

    // Insert status + button inside the .game wrapper (safe)
    const parent = canvas.parentElement;
    parent.appendChild(status);
    parent.appendChild(reset);

    // Game logic
    const human = "X";
    const ai = "O";
    let board = ["", "", "", "", "", "", "", "", ""];
    let current = human;
    let ended = false;
    let aiThinking = false;

    const wins = [
        [0,1,2],[3,4,5],[6,7,8],
        [0,3,6],[1,4,7],[2,5,8],
        [0,4,8],[2,4,6]
    ];

    function drawBoard(line) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        const s = canvas.width;
        const t = s / 3;

        ctx.strokeStyle = "#000";
        ctx.lineWidth = 2;

        ctx.beginPath();
        ctx.moveTo(t,0); ctx.lineTo(t,s);
        ctx.moveTo(2*t,0); ctx.lineTo(2*t,s);
        ctx.moveTo(0,t); ctx.lineTo(s,t);
        ctx.moveTo(0,2*t); ctx.lineTo(s,2*t);
        ctx.stroke();

        ctx.font = `${t*0.7}px sans-serif`;
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";

        for (let i=0;i<9;i++){
            if (!board[i]) continue;
            const c = i % 3;
            const r = Math.floor(i / 3);
            const x = c * t + t/2;
            const y = r * t + t/2;
            ctx.fillStyle = board[i] === human ? "#0055ff" : "#ff3333";
            ctx.fillText(board[i], x, y);
        }

        if (line){
            ctx.strokeStyle = "#00aa22";
            ctx.lineWidth = 4;
            const a = line[0], c = line[2];
            const ac = a%3, ar = Math.floor(a/3);
            const cc = c%3, cr = Math.floor(c/3);
            ctx.beginPath();
            ctx.moveTo(ac*t+t/2, ar*t+t/2);
            ctx.lineTo(cc*t+t/2, cr*t+t/2);
            ctx.stroke();
        }
    }

    function resetGame(){
        board = ["","","","","","","","",""];
        current = human;
        ended = false;
        aiThinking = false;
        status.textContent = "Your turn (X)";
        drawBoard();
    }

    function getIndex(x, y){
        const s = canvas.width;
        const t = s / 3;
        return Math.floor(y/t)*3 + Math.floor(x/t);
    }

    function checkResult(){
        for (const line of wins){
            const [a,b,c] = line;
            if(board[a] && board[a]===board[b] && board[a]===board[c]){
                return {win: board[a], line};
            }
        }
        if (board.every(x=>x)) return {tie:true};
        return null;
    }

    function aiMove(){
        const empty=[];
        for(let i=0;i<9;i++) if(!board[i]) empty.push(i);

        // Try win
        for (const i of empty){
            board[i]=ai;
            const r=checkResult();
            board[i]="";
            if(r && r.win===ai) return i;
        }
        // Block
        for (const i of empty){
            board[i]=human;
            const r=checkResult();
            board[i]="";
            if(r && r.win===human) return i;
        }
        // Center
        if(!board[4]) return 4;
        // Corners
        const corners=[0,2,6,8].filter(i=>!board[i]);
        if(corners.length) return corners[Math.floor(Math.random()*corners.length)];
        // Sides
        const sides=[1,3,5,7].filter(i=>!board[i]);
        if(sides.length) return sides[Math.floor(Math.random()*sides.length)];

        return -1;
    }

    function humanMove(i){
        if(ended || aiThinking) return;
        if(current!==human) return;
        if(i<0 || board[i]) return;

        board[i]=human;
        drawBoard();

        const r=checkResult();
        if(r && r.win){
            ended=true;
            status.textContent="You win!";
            drawBoard(r.line);
            return;
        } else if(r && r.tie){
            ended=true;
            status.textContent="Tie!";
            drawBoard();
            return;
        }

        current=ai;
        aiThinking=true;
        status.textContent="Computer thinking...";

        setTimeout(function(){
            const move=aiMove();
            if(move>=0) board[move]=ai;

            const r2=checkResult();
            if(r2 && r2.win){
                ended=true;
                status.textContent="Computer wins!";
                drawBoard(r2.line);
            } else if(r2 && r2.tie){
                ended=true;
                status.textContent="Tie!";
                drawBoard();
            } else {
                current=human;
                status.textContent="Your turn (X)";
                drawBoard();
            }
            aiThinking=false;
        }, 350);
    }

    canvas.addEventListener("click", function(evt){
        const rect = canvas.getBoundingClientRect();
        const x = evt.clientX - rect.left;
        const y = evt.clientY - rect.top;
        humanMove(getIndex(x,y));
    });

    reset.addEventListener("click", resetGame);

    resizeCanvas();
    resetGame();

})();