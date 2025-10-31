using System;
using SvdAlgorithm;

class SimpleTest
{
    static void Main()
    {
        Console.WriteLine("简单测试程序启动");
        
        // 创建一个简单的2x2矩阵
        double[,] data = {
            { 1.0, 2.0 },
            { 3.0, 4.0 }
        };
        
        var matrix = new Matrix(data);
        Console.WriteLine("矩阵创建成功");
        
        // 尝试调用SVD算法
        try
        {
            var result = SvdAlgorithm.Compute(matrix);
            Console.WriteLine("SVD计算成功");
            Console.WriteLine("奇异值: [" + string.Join(", ", result.S) + "]");
        }
        catch (Exception ex)
        {
            Console.WriteLine("错误: " + ex.Message);
        }
        
        Console.WriteLine("测试完成");
    }
}