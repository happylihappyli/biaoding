
from common.C_Train import C_Train
import random

from enum import Enum
from common.C_Container import C_Container

class State_Next(Enum):
    STrue = 1
    SFalse = 2
    SNone = 0


Train_ID_Count=0

class C_Node:
    
    def __init__(self,space_parent,space):
        global Train_ID_Count
        
        print("C_Node init ")
        
        self.Name=""
        self.train_from=""
        
        self.space_parent=space_parent
        self.space=space
        
        self.active_input_count=0
        
        self.all_active_mode=True
        self.auto_active=True
        
        self.pTrain=None
        self.e_Clear_Active=None
        self.step_index=-1   #计算的时候的步骤数组下标
        
        
        
        self.pre_nodes = [None,None,None,None,None,None]   #当前节点之前的节点
        
        self.pNext_True=[]
        self.pNext_False=[]
        
        self.input_nodes=[]
        self.active_now=[0,0,0,0,0,0,0,0,0,0]
        Train_ID_Count+=1
        self.Train_ID=Train_ID_Count #random.randint(1, 999)
        self.Next_Step = State_Next.STrue
        

    def read_vars(self,file):
        file2=file
        if isinstance(file2, str):
            if (file2.startswith("@")):
                count=len(file2)
                file2=self.space.vars.read_vars(self.pTrain,file2[1:count],"")
        return file2

    def Get_Train_From_Input(self):# C_Train
        if (self.input_nodes == None or len(self.input_nodes) == 0):
            return self.pTrain;

        for i in range(self.active_input_count):
            if (self.active_now[i] == 1):
                pState = self.input_nodes[i]
                return pState.pTrain

        if (len(self.input_nodes) > 0):
            pState = self.input_nodes[0]
            return pState.pTrain
        return None
   
   
    '''
     清除所有的激活状态
    '''
    def clear_active(self):
        for i in range(self.active_input_count):
            self.active_now[i] = 0
        
        
    def Run(self,pTrain_Input):
        if pTrain_Input != None:
            self.pTrain = pTrain_Input
            
        if (self.Name=="PF:2D卷积层3-2"):
            print("debug")
        #actives_Nodes = self.space.vars.actives_Nodes

        if (self.train_from != ""):
            if (self.space.vars.vars_step.ContainsKey(self.train_from)):
                self.pTrain = self.space.vars.vars_step[self.train_from].pTrain
        else:
            self.pTrain = self.Get_Train_From_Input()


        if (self.pTrain == None):
            self.pTrain = self.Get_Train_From_Input()

            if (self.pTrain == None):
                print("★★★S " + self.Name)
                self.pTrain = self.Get_Train_From_Input()
            if (self.pTrain == None):
                self.pTrain = C_Train(self.Train_ID)

        if (self.pTrain == None):
            print("错误，Train = None ：" + self.Name + " ")
            return

        self.clear_active()
        print(self.Name+" 开始运行 Train.ID="+str(self.pTrain.ID))
        self.pTrain.ran_step.append(self)
        self.run_sub() #//调用子程序

        

    def init_input(self, pNode, true_false = True, index=""):
        self.input_nodes.append(pNode)
        #如果有了就不添加了
        bExists = False

        if (true_false):
            for k in range(len(pNode.pNext_True)):
                if (pNode.pNext_True[k] == self):
                    bExists = True
        else:
            for k in range(len(pNode.pNext_False)):
                if (pNode.pNext_False[k] == self):
                    bExists = True
        if (bExists == False):
            if (true_false):
                pNode.pNext_True.append(self)
            else:
                pNode.pNext_False.append(self)
        self.active_input_count = len(self.input_nodes)
        
        index2=0
        if (index!=""):
            index2=int(index)
        print("init_input="+str(index2))
        print("pNode="+pNode.Name)
        self.pre_nodes[index2]=pNode
        
    def save_var(self,key,s_type,obj):
        self.space.vars.save_vars(self.pTrain,key,s_type,obj)

    def read_var(self,key):
        return self.space.vars.read_vars(self.pTrain,key,"")

    def read_string(self,value):
        result=value
        if (result.startswith("@")):
            count=len(result)
            result=self.read_var(result[1:count])
        return result
    
    def Add_Next(self,pTrain,actives_Nodes):
        if (self.Next_Step == State_Next.STrue):
            print("Next>>>True")
            for i in range(len(self.pNext_True)):
                pNode = self.pNext_True[i]
                pNode.set_active(self)
                if (pNode.check_active()):
                    self.space.console.WriteLine(self.Name + " 准备：" + pNode.Name)
                    actives_Nodes.append(pNode)
                    
        elif (self.Next_Step == State_Next.SFalse):
            print("Next>>>False")
            for i in range(len(self.pNext_False)):
                pNode = self.pNext_False[i]
                pNode.set_active(self)
                if (pNode.check_active()):
                    self.space.console.WriteLine(self.Name + " 准备启动模块：" + pNode.Name)
                    actives_Nodes.append(pNode)
        else:
            print("Next>>>none")
        
    def call_next(self):
        if (self.Name=="PF:上采样5"):
            print("debug")
        

        if (self.auto_active):
            self.Add_Next(self.pTrain, self.space.vars.actives_Nodes)
            #print("Add_Next")
            #print("step.count="+str(len(self.space.vars.actives_Nodes)))
            
            
    def set_active(self,pState):
        if (self.active_input_count == 1):
            self.set_active_sub(0)
            return

        for i in range(self.active_input_count):
            if (self.input_nodes[i] is pState):
                self.set_active_sub(i)
    
    
    def set_active_sub(self,index):
        self.active_now[index] = 1


    # 检查这个节点是否激活
    def check_active(self):
        if (self.all_active_mode):
            for i in range(self.active_input_count):
                self.space.console.WriteLine(self.Name + " check=" + str(i) + "=" + str(self.active_now[i]) + " from=" + self.input_nodes[i].Name)
                if (self.active_now[i] == 0):
                    return False
            return True
        else:
            for i in range(self.active_input_count):
                if (self.active_now[i] == 1):
                    return True
            return False

