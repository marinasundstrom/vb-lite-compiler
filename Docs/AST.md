Abstract Syntax Tree (AST)
==

The Abstract Syntax Tree (AST) is an hierarchical data structure that represents the structure of the program. Each of its nodes represent parts of the actual code; classes, methods, statements and expressions - the relations between them.

This tree is traversed by the Code Generator.

Every AST-node has methods that perform certain actions during the compilations process.

See Passes.md.