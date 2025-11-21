# How to Run Selenium Tests Locally

## Prerequisites
pip install webdriver_manager

pip install selenium


## Run Script

python ./.automatedTests/(NAME_OF_SCRIPT)


## Additional DEBUG Help

Remove or comment out the following line of code so that a physical chrome tab opens:

chrome_options.add_argument("--headless")

# Additional Selenium Information
- Currently within this solution there is two different selenium tests that was created in Fall 2025. 
- The first being NavigationTest which tests if you can go to each page within the system, alot of the ways it ensures that each page is accessible is by checking the first header of each page so if these are modified it will break.
- The second is a PlayBaseGames test where it see's if each base game is playable, this is achieved by going to each game page and pressing space on the canvas to see if it updates since we can't check specific elements of the canvas without checking it's pixels so this is the most effective way to check.
