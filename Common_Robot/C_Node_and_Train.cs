using Common_Robot2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_Robot2
{
    public class C_Node_and_Train
    {
        
        public C_Node? pNode = null;

        public I_Train? pTrain = null;


        public C_Node_and_Train(C_Node pNode, I_Train pTrain)
        {
            this.pNode = pNode;
            this.pTrain = pTrain;
        }

    }
}
