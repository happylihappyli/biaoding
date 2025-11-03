# SVD测试脚本
extends SceneTree

# 测试SVD分解
func _ready():
	print("开始测试SVD分解...")
	
	# 创建一个测试矩阵
	var test_matrix = [
		[1.0, 2.0, 3.0],
		[4.0, 5.0, 6.0],
		[7.0, 8.0, 9.0]
	]
	
	print("测试矩阵:")
	_print_matrix(test_matrix)
	
	# 调用SVD分解
	var svd_result = _compute_svd_accurate(test_matrix)
	
	print("SVD分解结果:")
	print("U矩阵:")
	_print_matrix(svd_result["U"])
	print("S矩阵:")
	_print_matrix(svd_result["S"])
	print("V矩阵:")
	_print_matrix(svd_result["V"])
	print("奇异值:", svd_result["singular_values"])
	
	# 验证分解结果
	var error = _verify_svd(test_matrix, svd_result["U"], svd_result["S"], svd_result["V"])
	print("重构误差:", error)
	
	if error < 1e-5:
		print("✓ SVD分解验证成功!")
	else:
		print("⚠ SVD分解存在较大误差，需要改进")

# 打印矩阵
func _print_matrix(matrix: Array):
	for i in range(matrix.size()):
		var row_str = ""
		for j in range(matrix[i].size()):
			row_str += str(matrix[i][j]) + "\t"
		print(row_str)

# 精确实现SVD分解，参考C#中的Jacobi特征值分解方法
func _compute_svd_accurate(matrix: Array) -> Dictionary:
	# 参考E:\github\svd中的C#实现，使用Jacobi方法进行特征值分解
	# 确保与C#代码的结果完全一致
	
	var A = matrix
	
	# 计算ATA矩阵
	var At = _transpose_matrix(A)
	var ATA = _multiply_matrix(At, A)
	
	# 使用Jacobi方法计算ATA的特征值和特征向量
	var eigen_result = _jacobi_eigen(ATA)
	
	# 计算奇异值（特征值的平方根）
	var singular_values = []
	for i in range(eigen_result["eigenvalues"].size()):
		var val = eigen_result["eigenvalues"][i]
		singular_values.append(sqrt(max(0, val)))
	
	# 对奇异值进行降序排序
	var sorted_indices = _sort_eigenvalues_indices(singular_values)
	
	# 重新排列特征向量矩阵的列
	var V = _reorder_columns(eigen_result["eigenvectors"], sorted_indices)
	
	# 计算U矩阵
	var U = _compute_u_matrix(A, V, singular_values, sorted_indices)
	
	# 创建Σ矩阵（奇异值矩阵）
	var Sigma = _create_sigma_matrix(singular_values, sorted_indices, A.size(), V[0].size())
	
	return {
		"U": U,
		"S": Sigma,
		"V": V,
		"singular_values": singular_values
	}

# Jacobi特征值分解方法（参考C#实现）
func _jacobi_eigen(matrix: Array, max_iterations: int = 100, tolerance: float = 1e-10) -> Dictionary:
	var n = matrix.size()
	var A = _copy_matrix(matrix)
	var V = _identity_matrix(n)
	
	for iter in range(max_iterations):
		var max_off_diagonal = 0.0
		var p = 0
		var q = 0
		
		# 寻找最大的非对角线元素
		for i in range(n):
			for j in range(i + 1, n):
				var off_diagonal = abs(A[i][j])
				if off_diagonal > max_off_diagonal:
					max_off_diagonal = off_diagonal
					p = i
					q = j
		
		if max_off_diagonal < tolerance:
			break
		
		# 计算旋转角度
		var App = A[p][p]
		var Aqq = A[q][q]
		var Apq = A[p][q]
		
		var theta = 0.5 * atan2(2 * Apq, Aqq - App)
		var c = cos(theta)
		var s = sin(theta)
		
		# 应用旋转
		for i in range(n):
			if i != p and i != q:
				var Aip = A[i][p]
				var Aiq = A[i][q]
				A[i][p] = c * Aip - s * Aiq
				A[i][q] = s * Aip + c * Aiq
				A[p][i] = A[i][p]
				A[q][i] = A[i][q]
		
		# 更新p行和q行
		var App_new = c * c * App - 2 * c * s * Apq + s * s * Aqq
		var Aqq_new = s * s * App + 2 * c * s * Apq + c * c * Aqq
		var Apq_new = 0.0
		
		A[p][p] = App_new
		A[q][q] = Aqq_new
		A[p][q] = Apq_new
		A[q][p] = Apq_new
		
		# 更新特征向量矩阵
		for i in range(n):
			var Vip = V[i][p]
			var Viq = V[i][q]
			V[i][p] = c * Vip - s * Viq
			V[i][q] = s * Vip + c * Viq
	
	# 提取特征值（对角线元素）
	var eigenvalues = []
	for i in range(n):
		eigenvalues.append(A[i][i])
	
	return {
		"eigenvalues": eigenvalues,
		"eigenvectors": V
	}

# 重新排列矩阵列
func _reorder_columns(matrix: Array, indices: Array) -> Array:
	var n = matrix[0].size()
	var result = []
	
	# 初始化结果矩阵
	for i in range(matrix.size()):
		result.append([])
		for j in range(n):
			result[i].append(0.0)
	
	for j in range(n):
		var new_col = indices[j]
		for i in range(matrix.size()):
			result[i][j] = matrix[i][new_col]
	
	return result

# 计算U矩阵
func _compute_u_matrix(A: Array, V: Array, singular_values: Array, sorted_indices: Array) -> Array:
	var m = A.size()
	var n = V[0].size()
	var min_dim = min(m, n)
	var U = []
	
	# 初始化U矩阵
	for i in range(m):
		U.append([])
		for j in range(min_dim):
			U[i].append(0.0)
	
	for j in range(min_dim):
		var sigma = singular_values[sorted_indices[j]]
		if sigma > 1e-10:
			for i in range(m):
				var sum_val = 0.0
				for k in range(n):
					sum_val += A[i][k] * V[k][j]
				U[i][j] = sum_val / sigma
		else:
			# 对于零奇异值，设置对应的U列为零
			for i in range(m):
				U[i][j] = 0.0
	
	return U

# 创建Σ矩阵（奇异值矩阵）
func _create_sigma_matrix(singular_values: Array, sorted_indices: Array, m: int, n: int) -> Array:
	var min_dim = min(m, n)
	var Sigma = []
	
	# 初始化Σ矩阵
	for i in range(m):
		Sigma.append([])
		for j in range(n):
			Sigma[i].append(0.0)
	
	for i in range(min_dim):
		Sigma[i][i] = singular_values[sorted_indices[i]]
	
	return Sigma

# 复制矩阵
func _copy_matrix(matrix: Array) -> Array:
	var result = []
	for i in range(matrix.size()):
		result.append([])
		for j in range(matrix[i].size()):
			result[i].append(matrix[i][j])
	return result

# 创建单位矩阵
func _identity_matrix(n: int) -> Array:
	var result = []
	for i in range(n):
		result.append([])
		for j in range(n):
			if i == j:
				result[i].append(1.0)
			else:
				result[i].append(0.0)
	return result

# 验证SVD分解结果
func _verify_svd(A: Array, U: Array, S: Array, V: Array) -> float:
	# 计算重构矩阵：A_reconstructed = U * S * V^T
	var S_Vt = _multiply_matrix(S, _transpose_matrix(V))
	var A_reconstructed = _multiply_matrix(U, S_Vt)
	
	# 计算重构误差
	var error = 0.0
	for i in range(A.size()):
		for j in range(A[0].size()):
			error += abs(A[i][j] - A_reconstructed[i][j])
	
	return error

# 矩阵转置
func _transpose_matrix(matrix: Array) -> Array:
	var rows = matrix.size()
	var cols = matrix[0].size()
	var result = []
	
	for j in range(cols):
		result.append([])
		for i in range(rows):
			result[j].append(matrix[i][j])
	
	return result

# 矩阵乘法
func _multiply_matrix(A: Array, B: Array) -> Array:
	var rows_A = A.size()
	var cols_A = A[0].size()
	var cols_B = B[0].size()
	var result = []
	
	for i in range(rows_A):
		result.append([])
		for j in range(cols_B):
			var sum_val = 0.0
			for k in range(cols_A):
				sum_val += A[i][k] * B[k][j]
			result[i].append(sum_val)
	
	return result

# 特征值排序
func _sort_eigenvalues_indices(eigenvalues: Array) -> Array:
	var indices = []
	for i in range(eigenvalues.size()):
		indices.append(i)
	
	# 降序排序
	for i in range(eigenvalues.size()):
		for j in range(i + 1, eigenvalues.size()):
			if eigenvalues[i] < eigenvalues[j]:
				var temp_val = eigenvalues[i]
				eigenvalues[i] = eigenvalues[j]
				eigenvalues[j] = temp_val
				
				var temp_idx = indices[i]
				indices[i] = indices[j]
				indices[j] = temp_idx
	
	return indices