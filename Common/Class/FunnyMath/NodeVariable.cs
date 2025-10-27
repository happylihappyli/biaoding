using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnyMath
{
    // Represents 代表表达式里的变量，比如: "2 * pi"
    public class NodeVariable : Node
    {
        public NodeVariable(string variableName)
        {
            _variableName = variableName;
        }

        string _variableName;

        public override double Eval(IContext ctx)
        {
            return ctx.ResolveVariable(_variableName);
        }
    }
}
