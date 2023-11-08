import requests
from bs4 import BeautifulSoup

url = 'https://gongchausa.com/our-tea/'

# Send a GET request to the URL
response = requests.get(url)

if response.status_code == 200:
    soup = BeautifulSoup(response.content, 'html.parser')
    items = soup.find_all('img', class_='gc-itemimage')
    x = 0
    output = ""
    for item in items:
        x += 1
        name = item['alt']
        src = item['src']
        output += f"Name: {name}\nSrc: {src}\n\n"
    output += str(x) + " items"
    with open("gongcha_items.txt", 'w', encoding='utf-8') as file:
        file.write(output)
    print("Data written to output file(gongcha_items.txt)")
else:
    print("Failed to retrieve the website.")
