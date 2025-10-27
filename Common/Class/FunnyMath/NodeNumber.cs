namespace FunnyMath
{
    // NodeNumber 代表表达式里的数字
    class NodeNumber : Node
    {
        double _number;             // The number
        public NodeNumber(double number)
        {
            _number = number;
        }


        public override double Eval(IContext ctx)
        {
            // Just return it.  Too easy.
            return _number;
        }
    }
}
