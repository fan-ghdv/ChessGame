using System.Reflection;

string dllPath =
    Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile
        ),
        ".nuget",
        "packages",
        "svg.controls.skia.avalonia",
        "12.0.0",
        "lib",
        "net8.0",
        "Svg.Controls.Skia.Avalonia.dll"
    );

Console.WriteLine("Loading:");
Console.WriteLine(dllPath);
Console.WriteLine();

Assembly assembly =
    Assembly.LoadFrom(dllPath);

Type svgType =
    assembly.GetType("Avalonia.Svg.Skia.Svg")!;

Console.WriteLine(
    $"TYPE: {svgType.FullName}"
);

Console.WriteLine();
Console.WriteLine("CONSTRUCTORS:");
Console.WriteLine();

foreach (
    ConstructorInfo constructor
    in svgType.GetConstructors(
        BindingFlags.Public |
        BindingFlags.Instance |
        BindingFlags.NonPublic))
{
    Console.WriteLine(constructor);

    ParameterInfo[] parameters =
        constructor.GetParameters();

    foreach (ParameterInfo parameter in parameters)
    {
        Console.WriteLine(
            $"    PARAMETER: " +
            $"{parameter.ParameterType.FullName} " +
            $"{parameter.Name}"
        );
    }

    Console.WriteLine();
}