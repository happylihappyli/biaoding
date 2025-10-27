from common.C_Node import C_Node

import cv2

import torch
import torchvision
import torchvision.transforms as transforms
import torch.optim as optim

import json
import os 
import torch.nn as nn
import torch.nn.functional as F
import matplotlib.pyplot as plt
import numpy as np

from rdp import rdp
from PIL import Image
from urllib.parse import urlparse
from pathlib import Path
from torch.autograd import Variable
from io import BytesIO
from json import JSONEncoder
from segment_anything import SamPredictor, sam_model_registry
from segment_anything import SamAutomaticMaskGenerator, sam_model_registry


class NumpyArrayEncoder(JSONEncoder):
    def default(self, obj):
        if isinstance(obj, np.ndarray):
            return obj.tolist()
        return JSONEncoder.default(self, obj)
        
        
        
#https://dl.fbaipublicfiles.com/segment_anything/sam_vit_h_4b8939.pth


class py_model_sam(C_Node):
    def __init__(self,Name,space_parent,space,window=None):
        super().__init__(space_parent,space)
        self.Name=Name
        self.file=""
        self.path=""
        self.key_img=""
        print("py_model_sam __init__")
        
        
    def init(self):
        self.sam = sam_model_registry["vit_h"](checkpoint=self.file)
        self.sam.to(device="cuda") #device = "cuda"
        print("py_model_sam init")
        
    def show_anns(self,anns):

        if len(anns) == 0:
            return
        sorted_anns = sorted(anns, key=(lambda x: x['area']), reverse=True)
        #ax = plt.gca()
        #ax.set_autoscale_on(False)

        h=sorted_anns[0]['segmentation'].shape[0]
        print("h="+str(h))
        w=sorted_anns[0]['segmentation'].shape[1]
        print("w="+str(w))

        img = np.ones((h, w, 4))
        img[:,:,3] = 0

        index=0
        arr=[]
        for ann in sorted_anns:
            img2 = np.ones((h, w, 3))
            index+=1
            m = ann['segmentation']
            img2[m] = [255,0,0]
            
            img2 = Image.fromarray(img2.astype('uint8')).convert('RGB')
            arr.append(img2)

        return (index,arr)
        
    def run_sub(self):
        mask_generator = SamAutomaticMaskGenerator(self.sam)

        image = self.read_var(self.key_img)  # cv2.imread(file_name)
        image2 = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)

        print("计算mask") 
        masks = mask_generator.generate(image2)
        print("mask计算完毕") 

        arr_json=self.generate_mask_json(masks)
        result=",".join(arr_json)
        self.save_var(self.key_save,"string",result)
        
        self.call_next()
        

    def zzz(self,arr):
        count_pic=0
        for index in range(len(arr)):
            img2=arr[index]
            index2=index+1
            img2.save(self.path+str(count_pic)+"_"+str(index2)+".png")

    def generate_mask_json(self,anns):
        if len(anns) == 0:
            return "[]"
        sorted_anns = sorted(anns, key=(lambda x: x['area']), reverse=True)
        #ax = plt.gca()
        #ax.set_autoscale_on(False)

        h=sorted_anns[0]['segmentation'].shape[0]
        print("h="+str(h))
        w=sorted_anns[0]['segmentation'].shape[1]
        print("w="+str(w))

        img = np.ones((h, w, 4))
        img[:,:,3] = 0

        arr_json=[]
        index=0
        for ann in sorted_anns:
            img2 = np.ones((h, w, 3))
            index+=1
            m = ann['segmentation']
            
            h=m.shape[0]
            w=m.shape[1]
            print("===================="+str(index))
            
            img2[m] = (255,255,255)
            
            img3 = Image.fromarray(img2.astype('uint8')).convert('RGB')

            (str_json,area)=self.convert_json(img2.astype('uint8'))
            
            #print("===json===")
            #if area>9000 and area<250000:
            arr_json.append(str_json)

        return arr_json
    
    def polygon_area(self,polygon):
        """
        计算面积
        """
        area = 0
        q = polygon[-1]
        for p in polygon:
            area += p[0] * q[1] - p[1] * q[0]
            q = p
        return abs(area) / 2.0
        

    def convert_json(self,img3):#file):
        global h_min
        global h_max
        #img3 = cv2.imread(file)
        hulls=[]
        #在连线完的图片上重新寻找最外层轮廓
        gray2 = cv2.cvtColor(img3, cv2.COLOR_BGR2GRAY)
        ret2, binary2 = cv2.threshold(gray2, 50, 255, cv2.THRESH_BINARY)
        contours2, heriachy2 = cv2.findContours(binary2, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)
        
        max=0
        index=0
        for i in range(len(contours2)):
            item=contours2[i]
            if (item.shape[0]>max):
                max=item.shape[0]
                index=i
            #print(item.shape)


        hull2 = cv2.convexHull(contours2[index])
        #print(hull2.shape)
        #print(hull2)
        threshold=2
        arr=rdp(hull2.reshape(-1, 2),threshold)
        #print(arr.shape)
        #print(arr)

        
        rect = cv2.minAreaRect(arr) # 得到最小外接矩形的（中心(x,y), (宽,高), 旋转角度）

        dx=rect[1][0]
        dy=rect[1][1]

        numpyData = {
            "label": "box_intact","points": arr,"group_id": None,
            "description": "",
            "shape_type": "polygon",
            "flags": {}
        }
        encodedNumpyData = json.dumps(numpyData, cls=NumpyArrayEncoder)  # use dump() to write array into file
        #print("Printing JSON serialized NumPy array")

        hulls.append(arr)
        area = self.polygon_area(arr)
        print("area="+str(area))
        
        #draw_hulls = cv2.drawContours(img3, hulls, -1, (255, 0, 0), 2) #最后一个参数-1表示填充

        return (encodedNumpyData,area)