import requests
from bs4 import BeautifulSoup

url = 'https://gongchausa.com/our-tea/'

# Send a GET request to the URL
response = requests.get(url)

if response.status_code == 200:
    soup = BeautifulSoup(response.content, 'html.parser')
    items = soup.find_all('img', class_='gc-itemimage')
    x = 0
    for item in items:
        x += 1
        name = item['alt']
        src = item['src']
        print(f"Name: {name}\nSrc: {src}\n")
    print(str(x) + " items")
else:
    print("Failed to retrieve the website.")
