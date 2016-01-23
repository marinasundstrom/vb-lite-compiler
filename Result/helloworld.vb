'This is a comment

Imports System

Namespace ConsoleApplication1

    Public Module Module1
        Private x As Integer = 2

        Sub Main()
            Dim foo As Integer = Subtract(Add(2, 3), 5)
        End Sub

        Public Function Add(ByVal a As Integer, ByVal b As Integer) As Integer
            Dim x As Integer = a
            Return b + x
        End Function

        Public Function Subtract(ByVal a As Integer, ByVal b As Integer) As Integer
            Return a - b
        End Function
    End Module

    Module Module2

    End Module

End Namespace