﻿'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : 
'MODULE NAME   : clsReposition
'CREATED DATE  : 3RD JUNE 2013
'LAST MODIFIED : 14TH JUNE 2013
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
Public Class clsReposition
    Private Shared repositionNo As Integer = 0
    Public Shared ReadOnly Property GetRepositionID() As String
        Get
            repositionNo += 1
            Return CStr(repositionNo)
        End Get
    End Property

End Class
