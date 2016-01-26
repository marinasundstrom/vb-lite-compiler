Passes
==

The compiler works in these steps:

1. Scan for tokens in input.
2. Parse the tokenstream and produce an Abstract Syntax Tree (AST) representing the structure of your program.
3. Perform semantic analysis on the AST (Not implemented)
4. Generate an executable assembly from the AST by traversing it (calling the methods defined on each node type)
  4.1 Define Assembly and Module. (.NET concepts)
  4.2 Define Type Builders.
  4.3 Define Member Builders.
  4.4 Emit method bodies in IL.
  4.5 Close all builders and save Assembly.
    
The result is an executable .NET assembly.
