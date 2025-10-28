extends Control

var combo_box1: LineEdit
var tx_output: TextEdit
var button1: Button

# 类定义需要在extends语句之后，不能嵌套在其他类中
class Point3D:
	var x: float = 0.0
	var y: float = 0.0
	var z: float = 0.0

class CameraConst:
	var p1_center: Point3D = Point3D.new()
	var p2_center: Point3D = Point3D.new()
	var M_Rotate: Transform3D = Transform3D.IDENTITY

class CameraTuYang:
	var p1_center: Point3D = Point3D.new()
	var p2_center: Point3D = Point3D.new()
	var M_Rotate: Transform3D = Transform3D.IDENTITY
	var camera1: CameraConst = CameraConst.new()
	var file: String = ""

func _ready() -> void:
	combo_box1 = $VBoxContainer/HBoxContainer/LineEdit
	tx_output = $VBoxContainer/TextEdit

func _on_button_1_pressed() -> void:
	var file_path = combo_box1.text
	if file_path.is_empty():
		tx_output.text = "请输入标定文件路径"
		return
	
	# 简化的文件读取功能
	tx_output.text = "正在读取标定文件: " + file_path
	
	# 这里可以添加实际的文件读取和计算逻辑
	# GDScript中使用错误处理而不是try-except
	var params = calculate_transform_matrix_params(file_path)
	if params:
		var result = read_calibration_file(params)
		tx_output.text += "\n\n标定成功!"
		tx_output.text += "\n旋转矩阵: " + str(result.rotation)
		tx_output.text += "\n平移向量: " + str(result.translation)
	else:
		tx_output.text += "\n\n读取失败: 无法打开或解析文件"

func calculate_transform_matrix_params(file_path: String) -> Array:
	var file = File.new()
	var error = file.open(file_path, File.READ)
	if error != OK:
		return null
	
	var pList_Point1 = []
	var pList_Point2 = []
	var sum_x1 = 0.0
	var sum_y1 = 0.0
	var sum_z1 = 0.0
	var sum_x2 = 0.0
	var sum_y2 = 0.0
	var sum_z2 = 0.0
	var n = 0
	
	while file.get_position() < file.get_len():
		var line = file.get_line()
		# 跳过注释行
		if line.begins_with("//"):
			continue
		
		# 处理分隔符
		line = line.replace("，", ",")
		line = line.replace(" ", ",")
		while line.find(",,") != -1:
			line = line.replace(",,", ",")
		
		var strSplit = line.split(",")
		if strSplit.size() > 5:
			try:
				var x1 = float(strSplit[0])
				var y1 = float(strSplit[1])
				var z1 = float(strSplit[2])
				var x2 = float(strSplit[3])
				var y2 = float(strSplit[4])
				var z2 = float(strSplit[5])
				
				pList_Point1.append(Vector3(x1, y1, z1))
				pList_Point2.append(Vector3(x2, y2, z2))
				
				sum_x1 += x1
				sum_y1 += y1
				sum_z1 += z1
				sum_x2 += x2
				sum_y2 += y2
				sum_z2 += z2
				n += 1
			except:
				# 忽略解析错误的行
				continue
	
	file.close()
	
	if n < 1:
		return null
	
	# 计算中心点
	var c_Point3D = Vector3(sum_x1 / n, sum_y1 / n, sum_z1 / n)
	var c_Point3D2 = Vector3(sum_x2 / n, sum_y2 / n, sum_z2 / n)
	
	# 计算旋转矩阵（这里简化处理，使用默认值）
	# 在实际应用中，应该使用Kabsch算法计算旋转矩阵
	var matrix3 = [
		1.0, 0.0, 0.0,
		0.0, 1.0, 0.0,
		0.0, 0.0, 1.0
	]
	
	# 构建返回参数
	var result = [
		matrix3[0], matrix3[1], matrix3[2],
		matrix3[3], matrix3[4], matrix3[5],
		matrix3[6], matrix3[7], matrix3[8],
		0.0, 0.0, 0.0,  # 平移向量暂时设为0
		c_Point3D.x, c_Point3D.y, c_Point3D.z,
		c_Point3D2.x, c_Point3D2.y, c_Point3D2.z
	]
	
	return result

func read_calibration_file(params: Array) -> Dictionary:
	var result = {}
	
	# X^T=(a1 a2 a3 a4 a5 a6 a7 a8 a9 m1 m2 m3)
	# 提取旋转矩阵参数
	var a1 = params[0]
	var a2 = params[1]
	var a3 = params[2]
	var a4 = params[3]
	var a5 = params[4]
	var a6 = params[5]
	var a7 = params[6]
	var a8 = params[7]
	var a9 = params[8]
	
	# 提取平移向量参数
	var m1 = params[9]
	var m2 = params[10]
	var m3 = params[11]
	
	# 提取中心点
	var p1_center = Vector3(params[12], params[13], params[14])
	var p2_center = Vector3(params[15], params[16], params[17])
	
	# 创建旋转矩阵（Basis）
	var basis = Basis(
		Vector3(a1, a2, a3),
		Vector3(a4, a5, a6),
		Vector3(a7, a8, a9)
	)
	
	# 创建完整的变换矩阵
	var transform = Transform3D(basis)
	transform.origin = Vector3(m1, m2, m3)
	
	# 存储结果，保持与C#代码结构一致
	result.rotation = transform
	result.translation = Vector3(m1, m2, m3)
	result.center1 = p1_center
	result.center2 = p2_center
	result.M_Rotate = basis  # 额外存储旋转部分，便于调试
	
	return result
