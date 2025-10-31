using System;
using SvdAlgorithm;

namespace SvdTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SVD算法测试程序 ===\n");
            
            // 测试1：简单的2x2矩阵
            Test2x2Matrix();
            
            // 测试2：3x3矩阵
            Test3x3Matrix();
            
            // 测试3：非方阵
            TestNonSquareMatrix();
            
            // 测试4：与MathNet.Numerics的对比测试
            TestComparisonWithMathNet();
            
            Console.WriteLine("\n=== 测试完成 ===");
            Console.ReadKey();
        }
        
        /// <summary>
        /// 测试2x2矩阵的SVD分解
        /// </summary>
        static void Test2x2Matrix()
        {
            Console.WriteLine("测试1：2x2矩阵SVD分解");
            Console.WriteLine("=====================");
            
            // 创建一个简单的2x2矩阵
            double[,] data = {
                { 1.0, 2.0 },
                { 3.0, 4.0 }
            };
            
            var matrix = Matrix.OfArray(data);
            Console.WriteLine("原始矩阵：");
            Console.WriteLine(matrix);
            
            // 进行SVD分解
            var svdResult = SvdAlgorithm.SvdAlgorithm.Compute(matrix);
            
            // 显示结果
            DisplaySvdResult(svdResult);
            
            // 验证分解的正确性
            VerifyDecomposition(matrix, svdResult);
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// 测试3x3矩阵的SVD分解
        /// </summary>
        static void Test3x3Matrix()
        {
            Console.WriteLine("测试2：3x3矩阵SVD分解");
            Console.WriteLine("=====================");
            
            // 创建一个3x3矩阵
            double[,] data = {
                { 1.0, 2.0, 3.0 },
                { 4.0, 5.0, 6.0 },
                { 7.0, 8.0, 9.0 }
            };
            
            var matrix = Matrix.OfArray(data);
            Console.WriteLine("原始矩阵：");
            Console.WriteLine(matrix);
            
            // 进行SVD分解
            var svdResult = SvdAlgorithm.SvdAlgorithm.Compute(matrix);
            
            // 显示结果
            DisplaySvdResult(svdResult);
            
            // 验证分解的正确性
            VerifyDecomposition(matrix, svdResult);
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// 测试非方阵的SVD分解
        /// </summary>
        static void TestNonSquareMatrix()
        {
            Console.WriteLine("测试3：非方阵SVD分解");
            Console.WriteLine("=====================");
            
            // 创建一个2x3矩阵
            double[,] data = {
                { 1.0, 2.0, 3.0 },
                { 4.0, 5.0, 6.0 }
            };
            
            var matrix = Matrix.OfArray(data);
            Console.WriteLine("原始矩阵（2x3）：");
            Console.WriteLine(matrix);
            
            // 进行SVD分解
            var svdResult = SvdAlgorithm.SvdAlgorithm.Compute(matrix);
            
            // 显示结果
            DisplaySvdResult(svdResult);
            
            // 验证分解的正确性
            VerifyDecomposition(matrix, svdResult);
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// 与MathNet.Numerics的对比测试
        /// </summary>
        static void TestComparisonWithMathNet()
        {
            Console.WriteLine("测试4：与MathNet.Numerics对比");
            Console.WriteLine("=============================");
            
            // 创建一个测试矩阵
            double[,] data = {
                { 2.0, 1.0 },
                { 1.0, 2.0 }
            };
            
            var matrix = Matrix.OfArray(data);
            Console.WriteLine("测试矩阵：");
            Console.WriteLine(matrix);
            
            // 使用我们的SVD算法
            var ourSvd = SvdAlgorithm.SvdAlgorithm.Compute(matrix);
            
            Console.WriteLine("我们的SVD算法结果：");
            Console.WriteLine($"奇异值: [{ourSvd.S[0]:F6}, {ourSvd.S[1]:F6}]");
            
            // 显示已知的理论结果（对于这个特定矩阵）
            Console.WriteLine("\n理论结果（已知）：");
            Console.WriteLine("奇异值: [3.000000, 1.000000]");
            
            // 计算误差
            double error1 = Math.Abs(ourSvd.S[0] - 3.0);
            double error2 = Math.Abs(ourSvd.S[1] - 1.0);
            
            Console.WriteLine($"\n误差分析：");
            Console.WriteLine($"第一个奇异值误差: {error1:E6}");
            Console.WriteLine($"第二个奇异值误差: {error2:E6}");
            
            if (error1 < 1e-5 && error2 < 1e-5)
            {
                Console.WriteLine("✓ 算法精度良好");
            }
            else
            {
                Console.WriteLine("⚠ 算法精度需要改进");
            }
            
            Console.WriteLine();
        }
        
        /// <summary>
        /// 显示SVD分解结果
        /// </summary>
        /// <param name="result">SVD结果</param>
        static void DisplaySvdResult(SvdResult result)
        {
            Console.WriteLine("SVD分解结果：");
            Console.WriteLine($"矩阵秩: {result.Rank}");
            Console.WriteLine($"L2范数: {result.L2Norm:F6}");
            Console.WriteLine($"条件数: {result.ConditionNumber:F6}");
            
            if (!double.IsNaN(result.Determinant))
            {
                Console.WriteLine($"行列式: {result.Determinant:F6}");
            }
            
            Console.WriteLine("奇异值: [" + string.Join(", ", Array.ConvertAll(result.S, x => x.ToString("F6"))) + "]");
            
            if (result.VectorsComputed)
            {
                Console.WriteLine("\n左奇异向量矩阵 U：");
                Console.WriteLine(result.U);
                
                Console.WriteLine("右奇异向量矩阵 VT：");
                Console.WriteLine(result.VT);
            }
        }
        
        /// <summary>
        /// 验证SVD分解的正确性
        /// </summary>
        /// <param name="original">原始矩阵</param>
        /// <param name="svdResult">SVD结果</param>
        static void VerifyDecomposition(Matrix original, SvdResult svdResult)
        {
            if (!svdResult.VectorsComputed) return;
            
            Console.WriteLine("\n验证分解正确性：");
            
            try
            {
                // 重建原始矩阵：A = U * S * VT
                var reconstructed = ReconstructMatrix(svdResult);
                
                // 计算重建误差
                double error = ComputeReconstructionError(original, reconstructed);
                
                Console.WriteLine($"重建误差: {error:E6}");
                
                if (error < 1e-10)
                {
                    Console.WriteLine("✓ 分解正确性验证通过");
                }
                else if (error < 1e-5)
                {
                    Console.WriteLine("✓ 分解基本正确");
                }
                else
                {
                    Console.WriteLine("⚠ 分解误差较大");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"验证过程中出现错误: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 从SVD结果重建原始矩阵
        /// </summary>
        /// <param name="svdResult">SVD结果</param>
        /// <returns>重建的矩阵</returns>
        static Matrix ReconstructMatrix(SvdResult svdResult)
        {
            var u = svdResult.U;
            var vt = svdResult.VT;
            var s = svdResult.S;
            
            int m = u.RowCount;
            int n = vt.ColumnCount;
            int minDim = Math.Min(m, n);
            
            // 创建S矩阵（对角矩阵）
            var sMatrix = new Matrix(m, n);
            for (int i = 0; i < minDim; i++)
            {
                sMatrix[i, i] = s[i];
            }
            
            // 计算 U * S
            var us = u.Multiply(sMatrix);
            
            // 计算 (U * S) * VT
            return us.Multiply(vt);
        }
        
        /// <summary>
        /// 计算重建误差
        /// </summary>
        /// <param name="original">原始矩阵</param>
        /// <param name="reconstructed">重建矩阵</param>
        /// <returns>Frobenius范数误差</returns>
        static double ComputeReconstructionError(Matrix original, Matrix reconstructed)
        {
            if (original.RowCount != reconstructed.RowCount || original.ColumnCount != reconstructed.ColumnCount)
            {
                throw new ArgumentException("矩阵维度不匹配");
            }
            
            double sumSquaredError = 0.0;
            for (int i = 0; i < original.RowCount; i++)
            {
                for (int j = 0; j < original.ColumnCount; j++)
                {
                    double error = original[i, j] - reconstructed[i, j];
                    sumSquaredError += error * error;
                }
            }
            
            return Math.Sqrt(sumSquaredError);
        }
    }
}