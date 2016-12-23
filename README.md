// Copyright (C) 2016 by Jonas Jermann <jjermann2@gmail.com>

Sharpgraph
==========

A C# program to parse and interactively process files in the DOT format from Graphviz.

See in particular also http://graphviz4net.codeplex.com/.
They achieve a similar goal and some basic ideas are based on it.
The project started mainly as a way for me to get to know Antlr and WPF.
It's essentially written from scratch and still work-in-progress.
I haven't decided on a specific license yet.

Prerequisites:
- VisualStudio 2015 / .NET Framework 4.6.2
- https://marketplace.visualstudio.com/items?itemName=SamHarwell.ANTLRLanguageSupport (recommended for DotParser)
- http://www.antlr.org/ (Nuget package, used in DotParser, GraphParser)
- http://www.graphviz.org/ (binary installation required for ExternalRunners)
  Make sure to set DotExecutablePath correctly in ExternalRunners (TODO: move this to the configuration file)
- https://diffplex.codeplex.com/ (Nuget package, used in ConsoleProgram)
