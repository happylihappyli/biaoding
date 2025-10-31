# SVD算法独立项目

本项目从MathNet.Numerics库中提取了实数矩阵SVD分解算法的核心计算过程，并创建了一个独立的C#项目来测试和使用该算法。

## 项目结构

```
svd/
├── SvdProject.sln              # Visual Studio解决方案文件
├── SvdAlgorithm/               # SVD算法库项目
│   ├── SvdAlgorithm.csproj    # 算法库项目文件
│   ├── Matrix.cs              # 矩阵类实现
│   └── Svd.cs                 # SVD算法核心实现
├── SvdTest/                   # 测试项目
│   ├── SvdTest.csproj         # 测试项目文件
│   └── Program.cs             # 测试程序
└── README.md                  # 项目说明文件
```

## 算法特性

- **完整的SVD分解**：支持计算左奇异向量矩阵U、右奇异向量矩阵VT和奇异值向量S
- **矩阵操作**：包含基本的矩阵运算（转置、乘法、行列式等）
- **数值稳定性**：基于MathNet.Numerics的稳定算法实现
- **多种矩阵类型**：支持方阵和非方阵的SVD分解

## 编译和运行

### 使用Visual Studio
1. 打开 `SvdProject.sln`
2. 设置 `SvdTest` 为启动项目
3. 编译并运行

### 使用命令行
```bash
# 编译算法库
dotnet build SvdAlgorithm/SvdAlgorithm.csproj

# 编译测试程序
dotnet build SvdTest/SvdTest.csproj

# 运行测试
dotnet run --project SvdTest/SvdTest.csproj
```

## 使用示例

```csharp
using SvdAlgorithm;

// 创建矩阵
double[,] data = {
    {1.0, 2.0},
    {3.0, 4.0}
};
var matrix = Matrix.OfArray(data);

// 进行SVD分解
var svdResult = SvdAlgorithm.Compute(matrix);

// 获取结果
var u = svdResult.U;        // 左奇异向量矩阵
var vt = svdResult.VT;       // 右奇异向量矩阵的转置
var s = svdResult.S;         // 奇异值向量
var rank = svdResult.Rank;   // 矩阵的秩
```

## 测试内容

测试程序包含以下测试用例：

1. **2x2矩阵测试**：验证基本功能
2. **3x3矩阵测试**：测试更复杂的矩阵
3. **非方阵测试**：验证对非方阵的支持
4. **精度对比测试**：与已知理论结果对比验证算法精度

## 算法来源

本项目的SVD算法实现基于MathNet.Numerics库中的`UserSvd`类，该算法：

- 使用双对角化方法将矩阵约简
- 采用QR迭代算法计算奇异值
- 包含完整的数值稳定性处理
- 支持各种矩阵维度的分解

## 依赖项

- .NET 6.0或更高版本
- 无外部依赖（完全独立实现）

## 许可证

本项目代码基于MathNet.Numerics的算法实现，遵循相应的开源许可证。