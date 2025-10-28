import re

# 读取文件内容
with open(r'e:\github\biaoding\godot\CalibrationPanel.gd', 'r', encoding='utf-8') as f:
    content = f.read()

# 查找所有函数定义
function_pattern = re.compile(r'func\s+(\w+)\s*\([^)]*\)\s*(->\s*\w+\s*)?\:.*?(?=func\s|$)', re.DOTALL)
functions = function_pattern.findall(content)

# 检查每个函数中的变量重复声明
for func_name, _ in functions:
    # 提取函数体
    func_body_match = re.search(r'func\s+' + func_name + r'\s*\([^)]*\)\s*(->\s*\w+\s*)?\:\s*(.*?)(?=\s*func\s|$)', content, re.DOTALL)
    if func_body_match:
        func_body = func_body_match.group(2)
        # 查找所有变量声明
        vars = re.findall(r'var\s+(\w+)\s*=', func_body)
        # 检查重复变量
        duplicates = []
        seen = set()
        for var in vars:
            if var in seen:
                if var not in duplicates:
                    duplicates.append(var)
            else:
                seen.add(var)
        if duplicates:
            print(f'函数 {func_name} 中的重复变量: {duplicates}')

print("检查完成")