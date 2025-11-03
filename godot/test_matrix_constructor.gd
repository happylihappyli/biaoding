extends Node

# 测试Matrix构造函数修复

func _ready():
	print("=== 测试Matrix构造函数修复 ===")
	
	# 测试1：无参数的Matrix.new()
	test_matrix_new()
	
	# 测试2：带参数的Matrix.create()
	test_matrix_create()
	
	# 测试3：Matrix.from_array()
	test_matrix_from_array()
	
	print("=== Matrix构造函数测试完成 ===")

# 测试无参数的Matrix.new()
func test_matrix_new():
	print("\n1. 测试Matrix.new() - 无参数构造函数")
	
	var matrix = Matrix.new()
	print("Matrix.new() 创建成功")
	print("矩阵维度: " + str(matrix.get_rows()) + "x" + str(matrix.get_cols()))
	
	# 验证默认矩阵是0x0
	if matrix.get_rows() == 0 and matrix.get_cols() == 0:
		print("✓ Matrix.new() 测试通过 - 创建了默认的0x0矩阵")
	else:
		print("✗ Matrix.new() 测试失败 - 期望0x0矩阵，实际得到" + str(matrix.get_rows()) + "x" + str(matrix.get_cols()))

# 测试带参数的Matrix.create()
func test_matrix_create():
	print("\n2. 测试Matrix.create(3, 3) - 带参数构造函数")
	
	var matrix = Matrix.create(3, 3)
	print("Matrix.create(3, 3) 创建成功")
	print("矩阵维度: " + str(matrix.get_rows()) + "x" + str(matrix.get_cols()))
	
	# 设置一些值
	matrix.set_element(0, 0, 1.0)
	matrix.set_element(1, 1, 2.0)
	matrix.set_element(2, 2, 3.0)
	
	# 验证矩阵维度
	if matrix.get_rows() == 3 and matrix.get_cols() == 3:
		print("✓ Matrix.create(3, 3) 维度测试通过")
	else:
		print("✗ Matrix.create(3, 3) 维度测试失败")
	
	# 验证设置的值
	if matrix.get_element(0, 0) == 1.0 and matrix.get_element(1, 1) == 2.0 and matrix.get_element(2, 2) == 3.0:
		print("✓ Matrix.create(3, 3) 值设置测试通过")
	else:
		print("✗ Matrix.create(3, 3) 值设置测试失败")

# 测试Matrix.from_array()
func test_matrix_from_array():
	print("\n3. 测试Matrix.from_array() - 从数组创建矩阵")
	
	var array_data = [
		[1.0, 2.0, 3.0],
		[4.0, 5.0, 6.0],
		[7.0, 8.0, 9.0]
	]
	
	var matrix = Matrix.from_array(array_data)
	print("Matrix.from_array() 创建成功")
	print("矩阵维度: " + str(matrix.get_rows()) + "x" + str(matrix.get_cols()))
	
	# 验证矩阵维度
	if matrix.get_rows() == 3 and matrix.get_cols() == 3:
		print("✓ Matrix.from_array() 维度测试通过")
	else:
		print("✗ Matrix.from_array() 维度测试失败")
	
	# 验证矩阵值
	if matrix.get_element(0, 0) == 1.0 and matrix.get_element(1, 1) == 5.0 and matrix.get_element(2, 2) == 9.0:
		print("✓ Matrix.from_array() 值验证测试通过")
	else:
		print("✗ Matrix.from_array() 值验证测试失败")
	
	print("矩阵内容:")
	print(matrix.matrix_to_string())
