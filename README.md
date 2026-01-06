# CodeMethodsSorter

## 📋 Описание проекта

**Code Methods Sorter** — это инструмент командной строки на C#, который автоматически сортирует методы и другие члены классов в файлах `.cs` по модификаторам доступа и алфавиту. Программа использует Microsoft Roslyn для точного парсинга и трансформации кода.

## 🎯 Возможности

### Сортировка членов класса:
- **Методы** (`public`, `protected`, `internal`, `private`)
- **Свойства** (Properties)
- **Поля** (Fields)
- **Конструкторы**
- **Перечисления** (Enums)

## 🚀 Быстрый старт

### 1. Сборка проекта

```bash
# Клонирование репозитория
git clone <repository-url>
cd CodeMethodsSorter

# Сборка
dotnet build

# Публикация как single-file приложение
dotnet publish -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true -o ./publish
```

### 2. Использование

```bash
# Сортировать один файл
./publish/CodeMethodsSorter --file MyClass.cs
```

## 🎮 Интеграция с VS Code

### 1. Настройка tasks.json

Создайте файл `.vscode/tasks.json` в вашем проекте:

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Sort C# Members",
            "type": "shell",
            "command": "{Путь к файлу}/RoslynMethodSorterComplete",
            "args": [
                "${file}"
            ],
            "group": {
                "kind": "build",
                "isDefault": false
            },
            "problemMatcher": [],
            "presentation": {
                "echo": true,
                "reveal": "always",
                "focus": false,
                "panel": "shared"
            }
        }
    ]
}
```

### 2. Настройка горячих клавиш

Добавьте в `.vscode/keybindings.json`:

```json
[
    {
        "key": "ctrl+alt+s",
        "command": "workbench.action.tasks.runTask",
        "args": "Sort C# Methods"
    }
]
```
## 🔧 Разработка

### Требования:
- .NET 9.0 SDK или выше
- Visual Studio Code / Visual Studio 2022 / Rider
- (Опционально) Unity для тестирования Unity-скриптов

### Установка зависимостей:

```bash
dotnet restore
```

## 📊 Пример работы

### До:
```csharp
public class Example
{
    private string _name;
    public int Age { get; set; }
    protected void MethodA() { }
    public Example() { }
    private static void Helper() { }
}
```

### После:
```csharp
public class Example
{
    public int Age { get; set; }
    public Example() { }
    protected void MethodA() { }
    private static void Helper() { }
    private string _name;
}
```

## 🌐 Кросс-платформенная сборка

Сборка для разных платформ:

```bash
# macOS (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true

# macOS (Intel)
dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true

# Windows
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Linux
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true
```

## 🤝 Вклад в проект

1. Форкните репозиторий
2. Создайте ветку для вашей фичи (`git checkout -b feature/amazing-feature`)
3. Закоммитьте изменения (`git commit -m 'Add amazing feature'`)
4. Запушьте в ветку (`git push origin feature/amazing-feature`)
5. Откройте Pull Request

## ⚠️ Ограничения

- Не обрабатывает partial классы, разделенные между файлами
- Может некорректно работать с очень сложными выражениями
- Не сохраняет порядок регионов (#region)

## 📞 Поддержка

Если вы нашли баг или у вас есть предложения:
1. Откройте Issue на GitHub
2. Опишите проблему с примером кода
3. Укажите версию .NET и ОС

## 🙏 Благодарности

- [Microsoft Roslyn](https://github.com/dotnet/roslyn) - за отличный API для работы с кодом
- Сообщество .NET за вдохновение

---

**Счастливой сортировки кода!** 🚀