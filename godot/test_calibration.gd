# 简单的标定测试脚本
# 用于验证标定计算结果与C#参考值的一致性

func _init():
	print("开始验证标定计算结果...")
	
	# 直接导入CalibrationPanel并运行测试
	var calibration_panel = load("res://CalibrationPanel.gd").new()
	calibration_panel.test_calibration_accuracy()
	
	print("验证完成!")

# 如果作为主脚本运行，则执行测试
if Engine.get_main_loop() == null:
	var test = _init()