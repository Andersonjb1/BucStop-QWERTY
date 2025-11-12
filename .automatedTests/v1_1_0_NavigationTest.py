from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import Select
import time

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


try:
    # 1) Navigates to the localhosted server and ensures website it loaded properly.
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

    
    
    # Navigates to the tetris.
    print("4) Navigating to tetris....")

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

    
    
    # Navigates to the pong.
    print("5) Navigating to pong....")

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

    
    
    # Navigates to snapshots.
    print("6) Navigating to Snapshots....")

    games_link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.LINK_TEXT, "Snapshots"))
    )
    games_link.click()

    time.sleep(1)
    headers = driver.find_elements(By.XPATH, "//h1[text()='System Snapshots']")
    print(f"Headers: {headers}")
    assert len(headers) > 0, f"Couldn't find the h1 tag with the text 'System Snapshots' on this page"

    
    
    # Navigates to create a snapshots.
    print("7) Navigating to Create New Snapshot....")

    create_btn = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, "a.btn.btn-create"))
    )
    create_btn.click()

    time.sleep(1)
    headers = driver.find_elements(By.XPATH, "//h1[text()='Create New Snapshot']")
    print(f"Headers: {headers}")
    assert len(headers) > 0, f"Couldn't find the h1 tag with the text 'Create New Snapshot' on this page"

    
    
    # Navigates to about.
    print("8) Navigating to About....")

    games_link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.LINK_TEXT, "Games"))
    )
    games_link.click()



    # Navigates to admin.
    print("9) Navigating to Admin....")
    print("Work In Progress: Admin page has been removed for now.")


    
    # Navigates to add your game.
    print("10) Navigating to Add Your Game....")

    add_link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.LINK_TEXT, "Add Your Game!"))
    )
    add_link.click()

    time.sleep(1)
    #headers = driver.find_elements(By.XPATH, "//h1[text()='Game Suggestion Guide']")
    headers = driver.find_elements(By.XPATH, "//h1[text()='Submissions Temporarily Unavailable']")
    print(f"Headers: {headers}")
    assert len(headers) > 0, f"Couldn't find the h1 tag with the text 'Submissions Temporarily Unavailable' on this page"
    #assert len(headers) > 0, f"Couldn't find the h1 tag with the text 'Game Suggestion Guide' on this page"

finally:
    print("Test Complete")
    driver.quit()
