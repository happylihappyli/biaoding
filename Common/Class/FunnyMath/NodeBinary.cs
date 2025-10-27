using System;

namespace FunnyMath
{
    //二元节点，代表加法，减法，等二元运算
    class NodeBinary : Node
    {
        // Constructor accepts the two nodes to be operated on and function
        // that performs the actual operation
        public NodeBinary(Node lhs, Node rhs, Func<double, double, double> op)
        {
            _lhs = lhs;
            _rhs = rhs;
            _op = op;
        }

        Node _lhs;                              // Left hand side of the operation
        Node _rhs;                              // Right hand side of the operation
        Func<double, double, double> _op;       // The callback operator

        public override double Eval(IContext ctx)
        {
            // Evaluate both sides
            var lhsVal = _lhs.Eval(ctx);
            var rhsVal = _rhs.Eval(ctx);

            // 计算，返回
            var result = _op(lhsVal, rhsVal);
            return result;
        }
    }
}
