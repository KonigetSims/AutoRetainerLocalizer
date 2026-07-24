using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Localizer; 
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        string rootPath = Environment.CurrentDirectory;
        
        if (!Directory.Exists(Path.Combine(rootPath, "AutoRetainer")))
        {
            rootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
        }

        string sourcePath = Path.Combine(rootPath, "AutoRetainer", "AutoRetainer", "UI");
        string dictPath = Path.Combine(rootPath, "zh-CN.json");

        Console.WriteLine($"[信息] 当前工作目录: {Environment.CurrentDirectory}");
        Console.WriteLine($"[信息] 预计源码路径: {sourcePath}");

        if (!Directory.Exists(sourcePath))
        {
            Console.WriteLine($"[错误] 找不到 AutoRetainer 文件夹！");
            return;
        }

        // 读取现有的字典
        var dictionary = File.Exists(dictPath)
            ? JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(dictPath))
            : new Dictionary<string, string>();

        var files = Directory.GetFiles(sourcePath, "*.cs", SearchOption.AllDirectories);
        Console.WriteLine($"找到 {files.Length} 个文件，准备开始扫描...");

        // 1: 建立一个持久的 rewriter 实例
        // 这样 MissingTranslations 可以在处理所有文件时持续累积
        var rewriter = new TranslationRewriter(dictionary ?? new(), dictPath);

        foreach (var file in files)
        {
            string code = File.ReadAllText(file);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            // 2: 使用同一个 rewriter 进行 Visit
            var result = rewriter.Visit(root);

            if (result != root)
            {
                File.WriteAllText(file, result.ToFullString());
                Console.WriteLine($"[已更新] {Path.GetRelativePath(rootPath, file)}");
            }
        }

        // 3: 处理完所有文件后，一次性写入未翻译字符串
        Console.WriteLine("正在检查是否有新发现的字符串需写入字典...");
        rewriter.SaveMissingTranslations();

        Console.WriteLine("中文化处理与字典更新完成！");
    }
}
