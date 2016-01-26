Passes
==

The compiler works in these steps:

# Scan for tokens in input.
# Parse the tokenstream and produce an Abstract Syntax Tree (AST) representing the structure of your program.
# Perform semantic analysis on the AST (Not implemented)
# Generate an executable assembly from the AST by traversing it (calling the methods defined on each node type)
    # Define Assembly and Module. (.NET concepts)
    # Define Type Builders.
    # Define Member Builders.
    # Emit method bodies in IL.
    # Close all builders and save Assembly.
    
The result is an executable .NET assembly.
