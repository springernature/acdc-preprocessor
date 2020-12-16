Imports System.IO
Imports System.Reflection
Imports System.Xml.Linq


Public Class FileOperations
        Protected Sub New()
        End Sub

        Private Shared Function GetAbsoluteFilePath(ByVal filePath As String) As String
            Return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filePath)
        End Function

        Public Shared Function GetFile(ByVal filePath As String) As XDocument
            filePath = GetAbsoluteFilePath(filePath)
            Dim doc As XDocument = XDocument.Load(filePath)
            Return doc
        End Function
    End Class
