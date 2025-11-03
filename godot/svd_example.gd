extends Node

# SVD功能使用示例脚本

func _ready():
	print("=== SVD功能使用示例 ===")
	
	# 示例1：基本矩阵操作
	test_basic_matrix_operations()
	
	# 示例2：SVD分解
	test_svd_decomposition()
	
	# 示例3：矩阵重构验证
	test_matrix_reconstruction()
	
	print("=== SVD示例测试完成 ===")

# 测试基本矩阵操作
func test_basic_matrix_operations():
	print("\n1. 基本矩阵操作测试")
	
	# 创建3x3矩阵
	var matrix = Matrix.create(3, 3)
	
	# 设置矩阵值
	matrix.set_element(0, 0, 1.0)
	matrix.set_element(0, 1, 2.0)
	matrix.set_element(0, 2, 3.0)
	matrix.set_element(1, 0, 4.0)
	matrix.set_element(1, 1, 5.0)
	matrix.set_element(1, 2, 6.0)
	matrix.set_element(2, 0, 7.0)
	matrix.set_element(2, 1, 8.0)
	matrix.set_element(2, 2, 9.0)
	
	print("原始矩阵:")
	print(matrix.matrix_to_string())
	
	# 矩阵转置
	var transposed = matrix.transpose()
	print("转置矩阵:")
	print(transposed.matrix_to_string())
	
	# 矩阵乘法
	var multiplied = matrix.multiply(transposed)
	print("矩阵乘积:")
	print(multiplied.matrix_to_string())
	
	# 矩阵复制
	var copied = matrix.copy()
	print("复制矩阵验证:")
	print("复制成功: " + str(copied.get_element(0, 0) == matrix.get_element(0, 0)))

# 测试SVD分解
func test_svd_decomposition():
	print("\n2. SVD分解测试")
	
	# 创建测试矩阵
	var matrix = Matrix.create(2, 2)
	matrix.set_element(0, 0, 1.0)
	matrix.set_element(0, 1, 2.0)
	matrix.set_element(1, 0, 2.0)
	matrix.set_element(1, 1, 1.0)
	
	print("输入矩阵:")
	print(matrix.matrix_to_string())
	
	# 执行SVD分解
	var svd_result = SVD.decompose(matrix, 100, 1e-10)
	
	if svd_result:
		print("SVD分解成功!")
		
		# 获取分解结果
		var U = svd_result.get_u()
		var S = svd_result.get_s()
		var V = svd_result.get_v()
		var singular_values = svd_result.get_singular_values()
		
		print("左奇异向量矩阵 U:")
		print(U.matrix_to_string())
		
		print("奇异值矩阵 S:")
		print(S.matrix_to_string())
		
		print("右奇异向量矩阵 V:")
		print(V.matrix_to_string())
		
		print("奇异值数组: " + str(singular_values))
		
		# 验证分解结果
		var error = svd_result.verify(matrix)
		print("重构误差: " + str(error))
		
		# 显示SVD结果摘要
		print("SVD结果摘要:")
		print(svd_result.result_to_string())
	else:
		print("SVD分解失败!")

# 测试矩阵重构
func test_matrix_reconstruction():
	print("\n3. 矩阵重构验证")
	
	# 创建对称矩阵进行测试
	var matrix = Matrix.create(2, 2)
	matrix.set_element(0, 0, 4.0)
	matrix.set_element(0, 1, 1.0)
	matrix.set_element(1, 0, 1.0)
	matrix.set_element(1, 1, 3.0)
	
	print("原始矩阵:")
	print(matrix.matrix_to_string())
	
	# 执行SVD分解
	var svd_result = SVD.decompose(matrix)
	
	if svd_result:
		var U = svd_result.get_u()
		var S = svd_result.get_s()
		var V = svd_result.get_v()
		
		# 手动重构矩阵: matrix ≈ U * S * V^T
		var V_transposed = V.transpose()
		var temp = U.multiply(S)
		var reconstructed = temp.multiply(V_transposed)
		
		print("重构矩阵:")
		print(reconstructed.matrix_to_string())
		
		# 计算重构误差
		var error = 0.0
		for i in range(matrix.get_rows()):
			for j in range(matrix.get_cols()):
				var diff = matrix.get_element(i, j) - reconstructed.get_element(i, j)
				error += diff * diff
		
		error = sqrt(error)
		print("手动重构误差: " + str(error))
		
		# 使用内置验证方法
		var builtin_error = svd_result.verify(matrix)
		print("内置验证误差: " + str(builtin_error))

# 辅助函数：计算平方根
func sqrt(value):
	return pow(value, 0.5)
