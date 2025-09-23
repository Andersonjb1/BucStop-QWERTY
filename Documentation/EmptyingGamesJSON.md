# Emptying games.json

After getting rid of snake and tetris from `games.json` and clearing the cache, the landing page changed to look like the following:

![Landing page after removing snake and tetris from games.json](images/image_1.png)

The arrows that are normally used switch between games essentially no longer function. I have tried multiple times to switch to "Pong" this way, as it should be the same, but it still stays in the state shown in above image.

Moving on to the Games list itself, Pong is now the only featured game. While hovering over the game, a teal drop down menu appears that shows play count and the description. Interestingly, the same drop down menu appears if you hover over where the other games should be on the right side.

![Games list showing only Pong with teal dropdown menu on hover](images/image_2.png)

I then decided to remove Pong from the `games.json` list.

I initially forgot to clear the cache after making the above change. After doing so, "Pong" has now been removed.

![Landing page after removing all games from games.json](images/image_3.png)

---

## Required Images:
* `images/image_1.png` - "Landing page after removing snake and tetris from games.json"
* `images/image_2.png` - "Games list showing only Pong with teal dropdown menu on hover"
* `images/image_3.png` - "Landing page after removing all games from games.json"