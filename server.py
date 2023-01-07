#!/usr/bin/env python

import cv2
import socket, threading
import math
import struct
import time

ip_srvr = "127.0.0.1"
port_srvr = 12345

client = [0]

global_lock = threading.Lock()


# recv threading
def recv_msg(sock_srvr, client):
    while True:
        try :
            seg, client[0] = sock_srvr.recvfrom(FrameSegment.MAX_DGRAM)
            print(seg[0], client[0])
        except Exception as e:
            pass
        

class FrameSegment(object):
    """ 
    Object to break down image frame segment
    if the size of image exceed maximum datagram size 
    """

    MAX_DGRAM = 2**15
    #MAX_DGRAM = 2**16
    MAX_IMAGE_DGRAM = MAX_DGRAM - 64 # extract 64 bytes in case UDP frame overflown
    
    def __init__(self, sock, port, addr="127.0.0.1"):
        self.sock_srvr = sock
        self.port = port
        self.addr = addr
    
    def udp_frame(self, sock_srvr, img):

        if client[0] != 0 : 
            dat = img.flatten().tostring()
            size = len(dat)
            num_of_segments = math.ceil(size/(self.MAX_IMAGE_DGRAM))
            array_pos_start = 0

            while num_of_segments:
                array_pos_end = min(size, array_pos_start + self.MAX_IMAGE_DGRAM)

                #print(7 - num_of_segments, len(dat[array_pos_start:array_pos_end]))
                
                sock_srvr.sendto(
                            struct.pack('B', (7 - num_of_segments)) +
                            dat[array_pos_start:array_pos_end], 
                            (client[0])
                            )
                time.sleep(0.003)
                array_pos_start = array_pos_end
                num_of_segments -= 1

def main():

    """ Top level main function """
    # Set up UDP socket
    size = 256  

    sock_srvr = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  
    sock_srvr.bind((ip_srvr, port_srvr))

    fs = FrameSegment(sock_srvr, port_srvr)

    cap = cv2.VideoCapture(0)

    # 가로, 세로 고정값 안됨
    #cap.set(cv2.CAP_PROP_FRAME_WIDTH, size)
    #cap.set(cv2.CAP_PROP_FRAME_HEIGHT, size)

    #print(cap.get(cv2.CAP_PROP_FRAME_WIDTH))
    #print(cap.get(cv2.CAP_PROP_FRAME_HEIGHT))

    print("cam is opened : ", cap.isOpened())
    thred_recv = threading.Thread(target=recv_msg, args=(sock_srvr, client))
    thred_recv.setDaemon(True)
    thred_recv.start()


    is_ready = False
    
    while (cap.isOpened()):

        _, frame = cap.read()

        # 영상 확인 용
        '''
        cv2.imshow('test', frame)

        key = cv2.waitKey(1)

        if key == 27:
            break
        '''
        
        try : 
          frame = cv2.resize(frame, dsize=(size, size), interpolation=cv2.INTER_LINEAR)
          frame = cv2.cvtColor(frame, cv2.COLOR_RGB2BGR)
          fs.udp_frame(sock_srvr, frame)
        except Exception as e :
            print(e)
        
        
        
    cap.release()
    cv2.destroyAllWindows()
    sock_srvr.close()

if __name__ == "__main__":
    main()