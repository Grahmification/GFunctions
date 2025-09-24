#### Description

GFunctions is a collection of miscellaneous useful C# code collected from a bunch of different projects. The code focuses on mathematics, data processing, gui building and file read/write tasks.

The goal of this project is to promote modular, re-usable code, and speed up the development of future projects. Any code which is deemed to be sufficently modular and potentially useful for future tasks gets added here.

#### License 

![GitHub](https://img.shields.io/github/license/Grahmification/GFunctions) GFunctions is available for free under the MIT license.

GFunctions also licenses the following packages:
- [Mathnet Numerics](https://numerics.mathdotnet.com/) is licensed under the MIT License. Copyright (c) 2002-2022 Math.NET.
- [OpenTK](https://opentk.net/) is licensed under the MIT License. Copyright (c) 2006-2019 Stefanos Apostolopoulos for the Open Toolkit project.
- [Oxyplot](https://oxyplot.github.io/) is licensed under the MIT License. Copyright (c) 2014 OxyPlot contributors.

#### Projects

Projects are sorted by their key dependencies, so additional .net packages do not have to be added to a project utilizing GFunctions unless necessary. 

1. GFunctions: Base project. Requires only standard .net dependencies.
1. GFunctions.Mathnet: Sub-project for mathematical operations utilizing the [Mathnet Numerics](https://numerics.mathdotnet.com/) library.
2. GFunctions.OpenTk: Sub-project for rendering 3D objects utilizing the [OpenTK](https://opentk.net/) library.
3. GFunctions.Plotting: Sub-project for 2D plotting data utilizing the [Oxyplot](https://oxyplot.github.io/) library.
4. GFunctions.Winforms: Sub-project for creating Winforms GUIs utilizing the [System.Windows.Forms](https://en.wikipedia.org/wiki/Windows_Forms) library.

Some of the sub-projects reference the base project as a dependency.

#### Changelog

**V1.1.0.0**
- Updated all projects to .net 8.0
- Updated all dependencies to latest versions
- Minor renaming of a couple functions for clarity
- Code formatting and cleanup