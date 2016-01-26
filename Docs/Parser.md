Parser
==

The parser takes a stream (or list) of tokens and tries to figure out the meaning of its syntax based on the defined parser logic (state machine), and represent the program as a tree structure, an Abstract Syntax Tree (AST). It will report any syntax errors.