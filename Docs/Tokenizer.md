Tokenizer
==

The Tokenizer reads the input stream and creates Tokens for every significant symbol that is being identified. Thes Tokens is then to be parsed by the Parser.

Notable Methods
===

* GetNextToken() - Reads the next unread token.

* PeekToken() - Peeks the next unread token.

Internals
===
The GetNextToken and PeekToken methods are calling the internal scan-method that consists of a state-machine that reads from the input stream, character by character, and then produces a valid token.

Internally, the Tokenizer also holds the state for the peek-token functionality.