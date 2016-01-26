' Test program

Imports System

Namespace ConsoleApplication1

    ' A module (an implicitly shared (static) class)
    Public Module Module1
        ' Fields
        Private x1 As Integer = Fun1(2, 3)
        Private x2 As Integer = Fun1(x1, 3) smart hello

        ' An Entry Point.
        Sub Main()
            Dim foo As Integer = Fun2(Fun1(2, 3), 5)

            ' Show that types can be imported from another namespace 
            ' and then be used locally. Prototype and proof-of-concept implementation.
            Dim rand As Random ' Uninitialized. Initialization not supported yet.
        End Sub

        ' Fun1: Shows local variable declaration and assignation. (ByVal is useless)
        Public Function Fun1(ByVal a As Integer, ByVal b As Integer) As Integer
            Dim x As Integer = a
            Return b + x
        End Function

        ' Fun2: Shows a call to another user-defined function.
        Public Function Fun2(a As Integer, b As Integer) As Integer
            Return a - Fun1(a, b)
        End Function

        ' Foo: Shows basic flow control. (If-statement to be revised).
        Public Function Foo(ByVal x As Integer) As Integer
            Dim y As Integer = 3
            Dim z As Integer = 1
            While y < 10
                z = y * z
                y = y + 1
            End While
            Return z
        End Function
    End Module

End Namespace