from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import Select
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.common.keys import Keys
import time, hashlib

# Chrome Options
chrome_options = Options()
chrome_options.add_argument("--headless")
chrome_options.add_argument("--window-size=1920,1080")
chrome_options.add_argument("--no-sandbox")
chrome_options.add_argument("--disable-dev-shm-usage")
chrome_options.add_argument("--disable-gpu")
chrome_options.add_argument("--force-device-scale-factor=1")
chrome_options.add_argument('--ignore-certificate-errors-spki-list')
chrome_options.add_argument('--ignore-ssl-errors')

driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=chrome_options)

# Function to get a hash of the canvas content
def get_canvas_hash(driver):
    canvas = WebDriverWait(driver, 10).until(
        EC.presence_of_element_located((By.ID, "game"))
    )
    data_url = driver.execute_script("return arguments[0].toDataURL('image/png');", canvas)
    return hashlib.md5(data_url.encode()).hexdigest()

# Function to start the game and verify canvas updates
def start_game_test():
    actions = ActionChains(driver)
    element = driver.find_element(By.ID, "game")

    hash1 = get_canvas_hash(driver)

    actions.click(element).key_down(Keys.SPACE).key_up(Keys.SPACE).perform()
    time.sleep(1)

    hash2 = get_canvas_hash(driver)

    assert hash1 != hash2, "Canvas did not update — game might not have started."
    print("Canvas updated successfully after SPACE key press.")

try:
    # Navigates to the localhosted server and ensures website it loaded properly.
    print("1) Connecting to Website...")
    
    # This may need to change to whatever is setup in GitHub Actions
    driver.get("http://localhost:8080/")
    # ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    page_title = driver.title
    print(f"Current Page: {page_title}")
    assert page_title == "Home Page - BucStop", f"Expected title to be 'Home Page - BucStop', but got {page_title}"
    
   
    # Navigates to the games page.
    print("2) Navigating to Games....")

    games_link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.LINK_TEXT, "Games"))
    )
    games_link.click()

    time.sleep(1)
    headers = driver.find_elements(By.XPATH, "//h1[text()='List of Games']")
    print(f"Headers: {headers}")
    assert len(headers) > 0, f"Couldn't find the h1 tag with the text 'List of Games' on this page"

    
    
    # Navigates to the snake.
    print("3) Navigating to snake....")
    link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, "a[href='/Games/Play/1']"))
    )
    link.click()

    time.sleep(1)
    element = WebDriverWait(driver, 10).until(
        EC.presence_of_element_located((By.XPATH, "//div[@class='card-header' and text()='Snake']"))
    )
    
    print(f"Game Title: {element.text.strip()}")
    assert element.text.strip() == "Snake", f"Expected game title to be 'Snake', but got {element.text.strip()}"

    
    
    # Plays Snake to 1 point.
    print("4) Starting Snake....")
    start_game_test()

    
    
    # Navigates to the tetris.
    print("5) Navigating to tetris....")

    games_link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.LINK_TEXT, "Games"))
    )
    games_link.click()

    link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, "a[href='/Games/Play/2']"))
    )
    link.click()

    time.sleep(1)
    element = WebDriverWait(driver, 10).until(
        EC.presence_of_element_located((By.XPATH, "//div[@class='card-header' and text()='Tetris']"))
    )
    
    print(f"Game Title: {element.text.strip()}")
    assert element.text.strip() == "Tetris", f"Expected game title to be 'Tetris', but got {element.text.strip()}"

    
    
    # Plays Tetris to Gameover.
    print("6) Starting Tetris....")
    start_game_test()

    
    
    # Navigates to the pong.
    print("7) Navigating to pong....")

    games_link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.LINK_TEXT, "Games"))
    )
    games_link.click()

    link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, "a[href='/Games/Play/3']"))
    )
    link.click()

    time.sleep(1)
    element = WebDriverWait(driver, 10).until(
        EC.presence_of_element_located((By.XPATH, "//div[@class='card-header' and text()='Pong']"))
    )
    
    print(f"Game Title: {element.text.strip()}")
    assert element.text.strip() == "Pong", f"Expected game title to be 'Pong', but got {element.text.strip()}"

    
    
    print("8) Starting Pong....")
    start_game_test()

    
    
finally:
    print("Test Complete")
    driver.quit()
