import socket
from PIL import Image
import cv2
from matplotlib import cm


#SERVER
host, port = "127.0.0.1", 25001
filename = "../../Heightmaps/0003_h_4.png"

image = cv2.imread(filename, cv2.IMREAD_UNCHANGED)
im = Image.fromarray(np.uint8(cm.gist_earth(image)*255))
print(type(image))


'''
with Image.open(filename) as img:
    img.load()
    img.save

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind((host, port))
server.listen(5)

while True:
    coms_socket, addr = server.accept()
    print(f"Connected to {addr}")
    msg = coms_socket.recv(1024)
    print(f"Message from client is: {msg}")
    coms_socket.send(f"Got message!") 
    coms_socket.close()
    print(f"Connection with  {addr} ended.")
'''
