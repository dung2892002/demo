# Details

Date : 2025-04-09 14:59:15

Directory d:\\demo\\Back-End\\Cukcuk.Infrastructure

Total : 28 files,  14525 codes, 24 comments, 274 blanks, all 14823 lines

[Summary](results.md) / Details / [Diff Summary](diff.md) / [Diff Details](diff-details.md)

## Files
| filename | language | code | comment | blank | total |
| :--- | :--- | ---: | ---: | ---: | ---: |
| [Cukcuk.Infrastructure/Cukcuk.Infrastructure.csproj](/Cukcuk.Infrastructure/Cukcuk.Infrastructure.csproj) | XML | 23 | 0 | 5 | 28 |
| [Cukcuk.Infrastructure/Data/ApplicationDbContext.cs](/Cukcuk.Infrastructure/Data/ApplicationDbContext.cs) | C# | 210 | 0 | 44 | 254 |
| [Cukcuk.Infrastructure/Repositories/CustomerGroupRepository.cs](/Cukcuk.Infrastructure/Repositories/CustomerGroupRepository.cs) | C# | 35 | 0 | 7 | 42 |
| [Cukcuk.Infrastructure/Repositories/CustomerRepository.cs](/Cukcuk.Infrastructure/Repositories/CustomerRepository.cs) | C# | 195 | 0 | 37 | 232 |
| [Cukcuk.Infrastructure/Repositories/DepartmentRepository.cs](/Cukcuk.Infrastructure/Repositories/DepartmentRepository.cs) | C# | 40 | 0 | 8 | 48 |
| [Cukcuk.Infrastructure/Repositories/DocumentRepository.cs](/Cukcuk.Infrastructure/Repositories/DocumentRepository.cs) | C# | 250 | 0 | 68 | 318 |
| [Cukcuk.Infrastructure/Repositories/EmployeeRepository.cs](/Cukcuk.Infrastructure/Repositories/EmployeeRepository.cs) | C# | 215 | 0 | 37 | 252 |
| [Cukcuk.Infrastructure/Repositories/ImportRepository.cs](/Cukcuk.Infrastructure/Repositories/ImportRepository.cs) | C# | 52 | 0 | 11 | 63 |
| [Cukcuk.Infrastructure/Repositories/MenuRepository.cs](/Cukcuk.Infrastructure/Repositories/MenuRepository.cs) | C# | 40 | 0 | 8 | 48 |
| [Cukcuk.Infrastructure/Repositories/MessageRepository.cs](/Cukcuk.Infrastructure/Repositories/MessageRepository.cs) | C# | 80 | 0 | 16 | 96 |
| [Cukcuk.Infrastructure/Repositories/PermissionRepository.cs](/Cukcuk.Infrastructure/Repositories/PermissionRepository.cs) | C# | 46 | 0 | 9 | 55 |
| [Cukcuk.Infrastructure/Repositories/PositionRepository.cs](/Cukcuk.Infrastructure/Repositories/PositionRepository.cs) | C# | 39 | 0 | 7 | 46 |
| [Cukcuk.Infrastructure/bin/Debug/net8.0/Cukcuk.Infrastructure.deps.json](/Cukcuk.Infrastructure/bin/Debug/net8.0/Cukcuk.Infrastructure.deps.json) | JSON | 3,117 | 0 | 0 | 3,117 |
| [Cukcuk.Infrastructure/bin/Debug/net8.0/Cukcuk.Infrastructure.runtimeconfig.json](/Cukcuk.Infrastructure/bin/Debug/net8.0/Cukcuk.Infrastructure.runtimeconfig.json) | JSON | 19 | 0 | 0 | 19 |
| [Cukcuk.Infrastructure/obj/Cukcuk.Infrastructure.csproj.EntityFrameworkCore.targets](/Cukcuk.Infrastructure/obj/Cukcuk.Infrastructure.csproj.EntityFrameworkCore.targets) | XML | 28 | 0 | 1 | 29 |
| [Cukcuk.Infrastructure/obj/Cukcuk.Infrastructure.csproj.nuget.dgspec.json](/Cukcuk.Infrastructure/obj/Cukcuk.Infrastructure.csproj.nuget.dgspec.json) | JSON | 234 | 0 | 0 | 234 |
| [Cukcuk.Infrastructure/obj/Cukcuk.Infrastructure.csproj.nuget.g.props](/Cukcuk.Infrastructure/obj/Cukcuk.Infrastructure.csproj.nuget.g.props) | XML | 25 | 0 | 0 | 25 |
| [Cukcuk.Infrastructure/obj/Cukcuk.Infrastructure.csproj.nuget.g.targets](/Cukcuk.Infrastructure/obj/Cukcuk.Infrastructure.csproj.nuget.g.targets) | XML | 9 | 0 | 0 | 9 |
| [Cukcuk.Infrastructure/obj/Debug/net8.0/.NETCoreApp,Version=v8.0.AssemblyAttributes.cs](/Cukcuk.Infrastructure/obj/Debug/net8.0/.NETCoreApp,Version=v8.0.AssemblyAttributes.cs) | C# | 3 | 1 | 1 | 5 |
| [Cukcuk.Infrastructure/obj/Debug/net8.0/Cukcuk.Infrastructure.AssemblyInfo.cs](/Cukcuk.Infrastructure/obj/Debug/net8.0/Cukcuk.Infrastructure.AssemblyInfo.cs) | C# | 9 | 10 | 5 | 24 |
| [Cukcuk.Infrastructure/obj/Debug/net8.0/Cukcuk.Infrastructure.GeneratedMSBuildEditorConfig.editorconfig](/Cukcuk.Infrastructure/obj/Debug/net8.0/Cukcuk.Infrastructure.GeneratedMSBuildEditorConfig.editorconfig) | EditorConfig | 15 | 0 | 1 | 16 |
| [Cukcuk.Infrastructure/obj/Debug/net8.0/Cukcuk.Infrastructure.GlobalUsings.g.cs](/Cukcuk.Infrastructure/obj/Debug/net8.0/Cukcuk.Infrastructure.GlobalUsings.g.cs) | C# | 7 | 1 | 1 | 9 |
| [Cukcuk.Infrastructure/obj/Debug/net8.0/Cukcuk.Infrastructure.sourcelink.json](/Cukcuk.Infrastructure/obj/Debug/net8.0/Cukcuk.Infrastructure.sourcelink.json) | JSON | 1 | 0 | 0 | 1 |
| [Cukcuk.Infrastructure/obj/Release/net8.0/.NETCoreApp,Version=v8.0.AssemblyAttributes.cs](/Cukcuk.Infrastructure/obj/Release/net8.0/.NETCoreApp,Version=v8.0.AssemblyAttributes.cs) | C# | 3 | 1 | 1 | 5 |
| [Cukcuk.Infrastructure/obj/Release/net8.0/Cukcuk.Infrastructure.AssemblyInfo.cs](/Cukcuk.Infrastructure/obj/Release/net8.0/Cukcuk.Infrastructure.AssemblyInfo.cs) | C# | 9 | 10 | 5 | 24 |
| [Cukcuk.Infrastructure/obj/Release/net8.0/Cukcuk.Infrastructure.GeneratedMSBuildEditorConfig.editorconfig](/Cukcuk.Infrastructure/obj/Release/net8.0/Cukcuk.Infrastructure.GeneratedMSBuildEditorConfig.editorconfig) | EditorConfig | 13 | 0 | 1 | 14 |
| [Cukcuk.Infrastructure/obj/Release/net8.0/Cukcuk.Infrastructure.GlobalUsings.g.cs](/Cukcuk.Infrastructure/obj/Release/net8.0/Cukcuk.Infrastructure.GlobalUsings.g.cs) | C# | 7 | 1 | 1 | 9 |
| [Cukcuk.Infrastructure/obj/project.assets.json](/Cukcuk.Infrastructure/obj/project.assets.json) | JSON | 9,801 | 0 | 0 | 9,801 |

[Summary](results.md) / Details / [Diff Summary](diff.md) / [Diff Details](diff-details.md)