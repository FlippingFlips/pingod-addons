---
title: "PinGodGame - Godot 4"
date: 2022-12-26T15:26:15Z
draft: false
weight: 15
---
---
# Godot 4 .
---
## Download
---
https://downloads.tuxfamily.org/godotengine/4.0/beta10/mono/

latest: https://downloads.tuxfamily.org/godotengine/4.0/
---

- Use the 64bit Godot.
- Rename to `godot4.exe` add environment path
---
## Visual Debugging
---
Need to create a `Properties/launchSettings.json` file in the project under Properties. This contains profiles to launch.

Replace paths to the Godot executable and working directory to the project, in this case is `.` to launch this.

Now can debug direct from launching in Visual Studio.

```
{
  "profiles": {
    "Godot (native debugging)": {
      "commandName": "Executable",
      "executablePath": "C:\\Godot4\\godot4.exe",
      "commandLineArgs": "--path . --verbose",
      "workingDirectory": ".",
      "environmentVariables": {
        "VisualBasicDesignTimeTargetsPath": "$(VisualStudioXamlRulesDir)Microsoft.VisualBasic.DesignTime.targets",
        "FSharpDesignTimeTargetsPath": "$(VisualStudioXamlRulesDir)Microsoft.FSharp.DesignTime.targets",
        "CSharpDesignTimeTargetsPath": "$(VisualStudioXamlRulesDir)Microsoft.CSharp.DesignTime.targets",
        "CPS_DiagnosticRuntime": "1",
        "CPS_MetricsCollection": "1"
      },
      "nativeDebugging": true
    }
  }
}
```

---
## Default dotnet project output for reference
---

Default project output:

```
<Project Sdk="Godot.NET.Sdk/4.0.0-beta1">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>
</Project>
```

Default solution output

```
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 2012
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UnnamedProject", "UnnamedProject.csproj", "{A4A5ECBD-C9CB-4907-BB8C-49C9DEFD1EBC}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
	Debug|Any CPU = Debug|Any CPU
	ExportDebug|Any CPU = ExportDebug|Any CPU
	ExportRelease|Any CPU = ExportRelease|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{A4A5ECBD-C9CB-4907-BB8C-49C9DEFD1EBC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{A4A5ECBD-C9CB-4907-BB8C-49C9DEFD1EBC}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{A4A5ECBD-C9CB-4907-BB8C-49C9DEFD1EBC}.ExportDebug|Any CPU.ActiveCfg = ExportDebug|Any CPU
		{A4A5ECBD-C9CB-4907-BB8C-49C9DEFD1EBC}.ExportDebug|Any CPU.Build.0 = ExportDebug|Any CPU
		{A4A5ECBD-C9CB-4907-BB8C-49C9DEFD1EBC}.ExportRelease|Any CPU.ActiveCfg = ExportRelease|Any CPU
		{A4A5ECBD-C9CB-4907-BB8C-49C9DEFD1EBC}.ExportRelease|Any CPU.Build.0 = ExportRelease|Any CPU
	EndGlobalSection
EndGlobal
```
