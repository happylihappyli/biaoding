extends Control

func _ready() -> void:
    # 连接按钮信号
    $VBoxContainer/Button.pressed.connect(_on_open_calibration_panel)

func _on_open_calibration_panel() -> void:
    # 加载并实例化标定面板场景
    var calibration_panel_scene = load("res://CalibrationPanel.tscn")
    var calibration_panel = calibration_panel_scene.instantiate()
    
    # 添加到场景树
    add_child(calibration_panel)
    
    # 居中显示标定面板
    calibration_panel.rect_position = Vector2(
        (rect_size.x - calibration_panel.rect_size.x) / 2,
        (rect_size.y - calibration_panel.rect_size.y) / 2
    )

func _unhandled_key_input(event: InputEvent) -> void:
    if event is InputEventKey and event.pressed and event.keycode == KEY_ESCAPE:
        # 处理ESC键退出
        get_tree().quit()