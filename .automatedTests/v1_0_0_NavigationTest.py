from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import Select
import time

# pip install webdriver_manager
# pip install selenium
# python ./.automatedTests/v1_0_0_NavigationTest.py

chrome_options = Options()
#chrome_options.add_argument("--headless")
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

    # 2) Navigates to the login page after clicking the games page.
    print("2) Navigating to Login Page...")

    games_link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.LINK_TEXT, "Games"))
    )
    games_link.click()

    time.sleep(1)
    headers = driver.find_elements(By.XPATH, "//h1[text()='Login']")
    print(f"Headers: {headers}")
    assert len(headers) > 0, f"Couldn't find the h1 tag with the text 'Login' on this page"

    email_input = WebDriverWait(driver, 10).until(
        EC.presence_of_element_located((By.ID, "email"))
    )
    email_input.clear()
    email_input.send_keys("dummy@etsu.edu")

    login_button = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.XPATH, "//button[text()='Login']"))
    )
    login_button.click()

    time.sleep(1)
    headers2 = driver.find_elements(By.XPATH, "//h1[text()='top games']")
    print(f"Headers: {headers2}")
    assert len(headers2) > 0, f"Couldn't find the h1 tag with the text 'top games' on this page"

    games_link = WebDriverWait(driver, 10).until(
        EC.element_to_be_clickable((By.LINK_TEXT, "Games"))
    )
    games_link.click()

    time.sleep(1)
    headers3 = driver.find_elements(By.XPATH, "//h1[text()='List of Games']")
    print(f"Headers: {headers3}")
    assert len(headers3) > 0, f"Couldn't find the h1 tag with the text 'List of Games' on this page"

    time.sleep(3)

finally:
    print("Test Complete")
    driver.quit()
