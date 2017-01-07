// Copyright (C) 2017 by Jonas Jermann <jjermann2@gmail.com>

Sharpgraph
==========

A C# program to parse and interactively process files in the DOT format from Graphviz.

See in particular also http://graphviz4net.codeplex.com/.
They already achieved a similar goal and the project was inspired by it.

The project started mainly as a way for me to get to know Antlr and WPF.
It's essentially written from scratch.


License:
--------
MIT License, see LICENSE.md.


Other Prerequisites, Licenses, Links:
-------------------------------------
- VisualStudio 2015 / .NET Framework 4.6.2 / Resharper 2016

  Make sure the region settings have "." as the decimal separator.

- http://www.graphviz.org/ (binary installation required for ExternalRunners)
  License: Eclipse Public License V1.0, see http://www.graphviz.org/License.php

  Make sure to set GraphvizPath in the App.conf file(s).

  Default: C:\Program Files (x86)\Graphviz2.38\bin

- https://marketplace.visualstudio.com/items?itemName=SamHarwell.ANTLRLanguageSupport (not needed, useful for DotParser)

- http://www.antlr.org/ (Nuget package, used in DotParser, GraphParser)
  License: BSD License, see http://www.antlr.org/license.html

- https://diffplex.codeplex.com/ (Nuget package, used in ConsoleProgram)
  License: Apache License, see https://github.com/mmanela/diffplex/blob/master/License.txt

- http://graphviz4net.codeplex.com/ (not used/needed, at most some basic ideas)
  License: New BSD License, see http://graphviz4net.codeplex.com/license

- Other similar projects (not used/needed):
  http://graphsharp.codeplex.com/
  
  https://code.google.com/archive/p/graphviznet/
  
  http://dot2silverlight.codeplex.com/


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
