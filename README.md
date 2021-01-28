<a href="https://skyhop.org"><img src="https://skyhop.org/assets/images/skyhop.png" width=200 alt="skyhop logo" /></a>

----

# Igc

This project contains an IGC file parser and generator based on prior work in the [Turbo87/igc-parser repository](https://github.com/Turbo87/igc-parser). The parser in this project is a C# adaption of the code found in that project.

## Installation

The library can be added to your project in a number of ways:

### NuGet

The `Skyhop.Igc` library is available through NuGet and can be installed as follows:

    Install-Package Skyhop.Igc

### .NET CLI

To install the library through the .NET CLI you can use the following command:

    dotnet add package Skyhop.Igc

### Building From Source

You can always grab the source on [GitHub](https://github.com/SkyHop/Igc) and build the code yourself!

## Usage

Using the code is as easy as passing the contents of an IGC file to the parser;

```csharp
string fileContents = File.ReadAllText(filePath);

IgcFile file = Skyhop.Igc.Parser.Parse(fileContents);
```
