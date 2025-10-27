namespace FunnyMath
{
    // 代表计算种的一个节点
    public abstract class Node
    {
        public abstract double Eval(IContext ctx);
    }
}
