// Copyright (C) 2017 by Jonas Jermann <jjermann2@gmail.com>

Sharpgraph
==========

A C# program to parse and interactively process files in the DOT format from Graphviz.

See in particular also http://graphviz4net.codeplex.com/.
They already achieved a similar goal and some ideas from the project were used here.

The project started mainly as a way for me to get to know Antlr and WPF.
It's essentially written from scratch.

I haven't decided on a specific license yet.

Prerequisites:
--------------
- VisualStudio 2015 / .NET Framework 4.6.2 / Resharper 2016

  Make sure the region settings have "." as the decimal separator.
- http://www.graphviz.org/ (binary installation required for ExternalRunners)

  Make sure to set DotExecutablePath correctly in ExternalRunners (TODO: move this to the configuration file).
- https://marketplace.visualstudio.com/items?itemName=SamHarwell.ANTLRLanguageSupport (recommended for DotParser, not needed)
- http://www.antlr.org/ (Nuget package, used in DotParser, GraphParser)
- https://diffplex.codeplex.com/ (Nuget package, used in ConsoleProgram)

Screenshots:
------------

Screenshot from 01.01.2017:

![sharpgraph 20151228](https://cloud.githubusercontent.com/assets/1377808/21582909/59e80cc6-d069-11e6-8c41-53943c27dcd1.png)

Screenshot from 28.12.2016:

![sharpgraph 20151228](https://cloud.githubusercontent.com/assets/1377808/21525059/e97322ce-cd1a-11e6-815e-a271bcb4aa9f.png)

Screenshot from 26.12.2016:

![sharpgraph 20161226](https://cloud.githubusercontent.com/assets/1377808/21485976/43a80bee-cbac-11e6-9cb3-64855dd6286c.png)

Screenshot from 25.12.2016:

![sharpgraph 20161225](https://cloud.githubusercontent.com/assets/1377808/21469092/0a97e0f0-ca39-11e6-95a5-92e2536b1201.png)
