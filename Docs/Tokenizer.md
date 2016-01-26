Tokenizer
==

Notable Methods
===

* GetNextToken() - Returns the next unread token either directly or from a saved state.

* PeekToken() - Peeks the next unread token.

Internals
===
The GetNextToken and PeekToken methods calls the internal scan-method that contains a state machine that reads from the inputstream character by character, and then produces a valid token.