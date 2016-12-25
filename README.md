// Copyright (C) 2016 by Jonas Jermann <jjermann2@gmail.com>

Sharpgraph
==========

A C# program to parse and interactively process files in the DOT format from Graphviz.

See in particular also http://graphviz4net.codeplex.com/.
They already achieved a similar goal and some ideas from the project were used here.

The project started mainly as a way for me to get to know Antlr and WPF.
It's essentially written from scratch and still early work-in-progress.

I haven't decided on a specific license yet.

Prerequisites:
- VisualStudio 2015 / .NET Framework 4.6.2 / Resharper 2016
- http://www.graphviz.org/ (binary installation required for ExternalRunners)

  Make sure to set DotExecutablePath correctly in ExternalRunners (TODO: move this to the configuration file)
- https://marketplace.visualstudio.com/items?itemName=SamHarwell.ANTLRLanguageSupport (recommended for DotParser)
- http://www.antlr.org/ (Nuget package, used in DotParser, GraphParser)
- https://diffplex.codeplex.com/ (Nuget package, used in ConsoleProgram)

Here is a screenshot from 25.12.2016:

![sharpgraph 20161225](https://cloud.githubusercontent.com/assets/1377808/21469092/0a97e0f0-ca39-11e6-95a5-92e2536b1201.png)