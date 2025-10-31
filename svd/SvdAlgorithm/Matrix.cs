using System;

namespace SvdAlgorithm
{
    /// <summary>
    /// 表示一个实数矩阵
    /// </summary>
    public class Matrix
    {
        private readonly double[,] _data;
        
        /// <summary>
        /// 获取矩阵的行数
        /// </summary>
        public int RowCount { get; }
        
        /// <summary>
        /// 获取矩阵的列数
        /// </summary>
        public int ColumnCount { get; }
        
        /// <summary>
        /// 使用指定的行数和列数创建新矩阵
        /// </summary>
        /// <param name="rows">行数</param>
        /// <param name="columns">列数</param>
        public Matrix(int rows, int columns)
        {
            RowCount = rows;
            ColumnCount = columns;
            _data = new double[rows, columns];
        }
        
        /// <summary>
        /// 使用二维数组创建矩阵
        /// </summary>
        /// <param name="data">二维数组数据</param>
        public Matrix(double[,] data)
        {
            RowCount = data.GetLength(0);
            ColumnCount = data.GetLength(1);
            _data = (double[,])data.Clone();
        }
        
        /// <summary>
        /// 获取或设置指定位置的矩阵元素
        /// </summary>
        /// <param name="row">行索引</param>
        /// <param name="column">列索引</param>
        /// <returns>矩阵元素值</returns>
        public double this[int row, int column]
        {
            get => _data[row, column];
            set => _data[row, column] = value;
        }
        
        /// <summary>
        /// 获取指定位置的矩阵元素
        /// </summary>
        /// <param name="row">行索引</param>
        /// <param name="column">列索引</param>
        /// <returns>矩阵元素值</returns>
        public double At(int row, int column)
        {
            return _data[row, column];
        }
        
        /// <summary>
        /// 设置指定位置的矩阵元素
        /// </summary>
        /// <param name="row">行索引</param>
        /// <param name="column">列索引</param>
        /// <param name="value">要设置的值</param>
        public void At(int row, int column, double value)
        {
            _data[row, column] = value;
        }
        
        /// <summary>
        /// 创建矩阵的转置
        /// </summary>
        /// <returns>转置矩阵</returns>
        public Matrix Transpose()
        {
            var result = new Matrix(ColumnCount, RowCount);
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    result[j, i] = _data[i, j];
                }
            }
            return result;
        }
        
        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="other">另一个矩阵</param>
        /// <returns>乘积矩阵</returns>
        public Matrix Multiply(Matrix other)
        {
            if (ColumnCount != other.RowCount)
                throw new ArgumentException("矩阵维度不匹配");
                
            var result = new Matrix(RowCount, other.ColumnCount);
            
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < other.ColumnCount; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < ColumnCount; k++)
                    {
                        sum += _data[i, k] * other[k, j];
                    }
                    result[i, j] = sum;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 创建单位矩阵
        /// </summary>
        /// <param name="size">矩阵大小</param>
        /// <returns>单位矩阵</returns>
        public static Matrix Identity(int size)
        {
            var result = new Matrix(size, size);
            for (int i = 0; i < size; i++)
            {
                result[i, i] = 1.0;
            }
            return result;
        }
        
        /// <summary>
        /// 从二维数组创建矩阵
        /// </summary>
        /// <param name="data">二维数组</param>
        /// <returns>矩阵对象</returns>
        public static Matrix OfArray(double[,] data)
        {
            return new Matrix(data);
        }
        
        /// <summary>
        /// 获取矩阵的行列式（仅适用于方阵）
        /// </summary>
        /// <returns>行列式值</returns>
        public double Determinant()
        {
            if (RowCount != ColumnCount)
                throw new InvalidOperationException("只有方阵才有行列式");
                
            return DeterminantInternal(_data, RowCount);
        }
        
        private static double DeterminantInternal(double[,] matrix, int n)
        {
            if (n == 1) return matrix[0, 0];
            if (n == 2) return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            
            double det = 0;
            for (int j = 0; j < n; j++)
            {
                double[,] minor = GetMinor(matrix, 0, j, n);
                det += (j % 2 == 0 ? 1 : -1) * matrix[0, j] * DeterminantInternal(minor, n - 1);
            }
            return det;
        }
        
        private static double[,] GetMinor(double[,] matrix, int row, int col, int n)
        {
            double[,] minor = new double[n - 1, n - 1];
            int minorRow = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == row) continue;
                int minorCol = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == col) continue;
                    minor[minorRow, minorCol] = matrix[i, j];
                    minorCol++;
                }
                minorRow++;
            }
            return minor;
        }
        
        /// <summary>
        /// 返回矩阵的字符串表示
        /// </summary>
        /// <returns>矩阵字符串</returns>
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < RowCount; i++)
            {
                sb.Append("[");
                for (int j = 0; j < ColumnCount; j++)
                {
                    sb.Append(_data[i, j].ToString("F6"));
                    if (j < ColumnCount - 1) sb.Append(", ");
                }
                sb.Append("]\n");
            }
            return sb.ToString();
        }
    }
}