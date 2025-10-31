#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
SVD项目编译和测试脚本
"""

import os
import subprocess
import sys
import time
from datetime import datetime

def print_line_utf8(message):
    """打印UTF-8编码的信息"""
    print(message)

def run_command(command, cwd=None):
    """运行命令并返回结果"""
    print_line_utf8(f"执行命令: {command}")
    
    try:
        result = subprocess.run(
            command, 
            shell=True, 
            cwd=cwd, 
            capture_output=True, 
            text=True, 
            encoding='utf-8'
        )
        
        if result.returncode == 0:
            print_line_utf8("命令执行成功")
            if result.stdout:
                print_line_utf8("输出:")
                print_line_utf8(result.stdout)
        else:
            print_line_utf8(f"命令执行失败，返回码: {result.returncode}")
            if result.stderr:
                print_line_utf8("错误信息:")
                print_line_utf8(result.stderr)
        
        return result.returncode == 0, result.stdout, result.stderr
    
    except Exception as e:
        print_line_utf8(f"执行命令时发生异常: {e}")
        return False, "", str(e)

def build_svd_algorithm():
    """编译SVD算法库"""
    print_line_utf8("\n=== 开始编译SVD算法库 ===")
    
    start_time = time.time()
    
    # 编译算法库
    success, stdout, stderr = run_command(
        "dotnet build SvdAlgorithm/SvdAlgorithm.csproj --configuration Release",
        cwd=os.getcwd()
    )
    
    end_time = time.time()
    print_line_utf8(f"编译耗时: {end_time - start_time:.2f}秒")
    
    if success:
        print_line_utf8("✓ SVD算法库编译成功")
    else:
        print_line_utf8("✗ SVD算法库编译失败")
    
    return success

def build_svd_test():
    """编译测试程序"""
    print_line_utf8("\n=== 开始编译测试程序 ===")
    
    start_time = time.time()
    
    # 编译测试程序
    success, stdout, stderr = run_command(
        "dotnet build SvdTest/SvdTest.csproj --configuration Release",
        cwd=os.getcwd()
    )
    
    end_time = time.time()
    print_line_utf8(f"编译耗时: {end_time - start_time:.2f}秒")
    
    if success:
        print_line_utf8("✓ 测试程序编译成功")
    else:
        print_line_utf8("✗ 测试程序编译失败")
    
    return success

def run_tests():
    """运行测试程序"""
    print_line_utf8("\n=== 开始运行测试 ===")
    
    start_time = time.time()
    
    # 运行测试程序
    success, stdout, stderr = run_command(
        "dotnet run --project SvdTest/SvdTest.csproj --configuration Release --no-build",
        cwd=os.getcwd()
    )
    
    end_time = time.time()
    print_line_utf8(f"测试耗时: {end_time - start_time:.2f}秒")
    
    if success:
        print_line_utf8("✓ 测试运行成功")
    else:
        print_line_utf8("✗ 测试运行失败")
    
    return success

def clean_build():
    """清理编译结果"""
    print_line_utf8("\n=== 开始清理编译结果 ===")
    
    # 清理算法库
    success1, _, _ = run_command(
        "dotnet clean SvdAlgorithm/SvdAlgorithm.csproj",
        cwd=os.getcwd()
    )
    
    # 清理测试程序
    success2, _, _ = run_command(
        "dotnet clean SvdTest/SvdTest.csproj",
        cwd=os.getcwd()
    )
    
    # 删除bin和obj目录
    for dir_name in ["bin", "obj"]:
        for project_dir in ["SvdAlgorithm", "SvdTest"]:
            dir_path = os.path.join(project_dir, dir_name)
            if os.path.exists(dir_path):
                try:
                    import shutil
                    shutil.rmtree(dir_path)
                    print_line_utf8(f"删除目录: {dir_path}")
                except Exception as e:
                    print_line_utf8(f"删除目录失败 {dir_path}: {e}")
    
    print_line_utf8("✓ 清理完成")
    return success1 and success2

def main():
    """主函数"""
    print_line_utf8("=" * 50)
    print_line_utf8("SVD算法项目编译和测试脚本")
    print_line_utf8(f"开始时间: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print_line_utf8("=" * 50)
    
    # 检查当前目录
    current_dir = os.getcwd()
    print_line_utf8(f"当前工作目录: {current_dir}")
    
    # 检查必要的文件
    required_files = [
        "SvdProject.sln",
        "SvdAlgorithm/SvdAlgorithm.csproj", 
        "SvdTest/SvdTest.csproj"
    ]
    
    for file_path in required_files:
        if not os.path.exists(file_path):
            print_line_utf8(f"错误: 找不到必要文件 {file_path}")
            print_line_utf8("请确保在项目根目录运行此脚本")
            return 1
    
    # 检查dotnet是否可用
    dotnet_available, _, _ = run_command("dotnet --version")
    if not dotnet_available:
        print_line_utf8("错误: 未找到dotnet命令，请安装.NET SDK")
        return 1
    
    # 处理命令行参数
    if len(sys.argv) > 1:
        if sys.argv[1] == "clean":
            return 0 if clean_build() else 1
        elif sys.argv[1] == "build":
            # 只编译不测试
            success1 = build_svd_algorithm()
            success2 = build_svd_test()
            return 0 if (success1 and success2) else 1
        elif sys.argv[1] == "test":
            # 只测试不编译
            return 0 if run_tests() else 1
        elif sys.argv[1] == "help":
            print_line_utf8("""
使用方法:
  python build_and_test.py [选项]

选项:
  clean    - 清理编译结果
  build    - 只编译不测试
  test     - 只测试不编译
  help     - 显示此帮助信息
  无参数   - 完整编译和测试
            """)
            return 0
    
    # 默认行为：完整编译和测试
    print_line_utf8("\n开始完整编译和测试流程...")
    
    # 编译算法库
    if not build_svd_algorithm():
        return 1
    
    # 编译测试程序
    if not build_svd_test():
        return 1
    
    # 运行测试
    if not run_tests():
        return 1
    
    print_line_utf8("\n" + "=" * 50)
    print_line_utf8("✓ 所有任务完成成功！")
    print_line_utf8(f"结束时间: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print_line_utf8("=" * 50)
    
    # 播放完成提示音（可选）
    try:
        import winsound
        winsound.PlaySound("SystemExclamation", winsound.SND_ALIAS)
        print_line_utf8("任务运行完毕，过来看看！")
    except:
        print_line_utf8("任务运行完毕！")
    
    return 0

if __name__ == "__main__":
    exit_code = main()
    sys.exit(exit_code)