import socket

host, port = "127.0.0.1", 25001
data = "1,2,3"

# SOCK_STREAM == TCP 
sock  = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
try:
    sock.connect((host, port))
    sock.sendall(data.encode("utf-8"))
    response = sock.recv(1024).decode("utf-8")
    print (response)

finally:
    sock.close()