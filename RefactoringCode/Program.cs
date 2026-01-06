using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RoslynMethodSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Использование: RoslynMethodSorter.exe <путь_к_файлу.cs>");
                Console.WriteLine("Пример: RoslynMethodSorter.exe Program.cs");
                return;
            }

            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Файл не найден: {filePath}");
                return;
            }

            try
            {
                SortFileWithRoslyn(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        static void SortFileWithRoslyn(string filePath)
        {
            Console.WriteLine($"Обработка файла: {filePath}");
            
            string sourceCode = File.ReadAllText(filePath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = tree.GetRoot() as CompilationUnitSyntax;
            
            var rewriter = new MethodSortingRewriter();
            var newRoot = rewriter.Visit(root);
            
            if (!newRoot.IsEquivalentTo(root))
            {
                string newSourceCode = newRoot.ToFullString();
                File.WriteAllText(filePath, newSourceCode);
                Console.WriteLine("Файл успешно отсортирован и сохранен!");
                
                Console.WriteLine("\nСтатистика:");
                Console.WriteLine($"Перемещено методов: {rewriter.MethodsMoved}");
                Console.WriteLine($"Обработано классов: {rewriter.ClassesProcessed}");
            }
            else
            {
                Console.WriteLine("Изменений не требуется.");
            }
        }
    }

    class MethodSortingRewriter : CSharpSyntaxRewriter
    {
        public int MethodsMoved { get; private set; }
        public int ClassesProcessed { get; private set; }
        
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            ClassesProcessed++;
            Console.WriteLine($"\nОбработка класса: {node.Identifier.Text}");
            
            var methods = node.Members.OfType<MethodDeclarationSyntax>().ToList();
            var otherMembers = node.Members.Where(m => !(m is MethodDeclarationSyntax)).ToList();
            
            if (methods.Count == 0)
            {
                Console.WriteLine("  Нет методов для сортировки");
                return base.VisitClassDeclaration(node);
            }
            
            Console.WriteLine($"  Найдено методов: {methods.Count}");
            
            // Сортируем методы
            var sortedMethods = SortMethods(methods);
            
            // Проверяем, изменился ли порядок
            bool orderChanged = !methods.SequenceEqual(sortedMethods);
            
            if (orderChanged)
            {
                MethodsMoved += methods.Count;
                
                // Создаем новый список членов
                var newMembers = new List<MemberDeclarationSyntax>();
                
                // Добавляем все кроме методов в исходном порядке
                foreach (var member in node.Members)
                {
                    if (!(member is MethodDeclarationSyntax))
                    {
                        newMembers.Add(member);
                    }
                }
                
                // Добавляем отсортированные методы
                newMembers.AddRange(sortedMethods);
                
                // Создаем новый узел класса
                var newNode = node.WithMembers(SyntaxFactory.List(newMembers));
                
                Console.WriteLine("  Методы отсортированы!");
                
                // Выводим новый порядок
                Console.WriteLine("  Новый порядок методов:");
                foreach (var method in sortedMethods)
                {
                    var mods = method.Modifiers.Select(m => m.Text);
                    Console.WriteLine($"    [{string.Join(" ", mods)}] {method.Identifier.Text}");
                }
                
                return base.VisitClassDeclaration(newNode);
            }
            else
            {
                Console.WriteLine("  Порядок методов не изменился");
                return base.VisitClassDeclaration(node);
            }
        }
        
        private List<MethodDeclarationSyntax> SortMethods(List<MethodDeclarationSyntax> methods)
        {
            // Порядок модификаторов доступа
            var accessOrder = new Dictionary<string, int>
            {
                ["public"] = 0,
                ["protected internal"] = 1,
                ["protected"] = 2,
                ["internal"] = 3,
                ["private protected"] = 4,
                ["private"] = 5
            };
            
            return methods
                .OrderBy(m => GetAccessModifierPriority(m.Modifiers, accessOrder))
                .ThenBy(m => m.Identifier.Text)
                .ThenBy(m => m.ParameterList.Parameters.Count)
                .ToList();
        }
        
        private int GetAccessModifierPriority(SyntaxTokenList modifiers, Dictionary<string, int> order)
        {
            var modifierTexts = modifiers.Select(m => m.Text).ToList();
            
            // Проверяем комбинированные модификаторы
            if (modifierTexts.Contains("protected") && modifierTexts.Contains("internal"))
                return order["protected internal"];
            if (modifierTexts.Contains("private") && modifierTexts.Contains("protected"))
                return order["private protected"];
            
            // Проверяем обычные модификаторы
            foreach (var modifier in modifierTexts)
            {
                if (order.TryGetValue(modifier, out int priority))
                    return priority;
            }
            
            // Если модификатор не указан (по умолчанию private)
            return order["private"];
        }
    }
}