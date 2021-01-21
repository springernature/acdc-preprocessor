﻿'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : SUVARNA RAUT
'CLASS NAME    : LogMessages
'CREATED DATE  : 3RD JUNE 2013
'LAST MODIFIED : 14TH JUNE 2013
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
Public Class ClsLogMessage
    Public Sub LogMessages(ByVal Msg As String, Optional ByVal str As Boolean = False)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:LogMessages
        'PARAMETER    :Msg
        'AIM          :This function write message to the given file
        '=============================================================================================================
        '=============================================================================================================
        Dim sw As New System.IO.StreamWriter("d:\JNLPGUtility.log", True)
        If (str = True) Then
            sw.Write(vbNewLine + Msg)
        Else
            sw.Write(vbNewLine + "JNLPGUtility                          :→  " + Msg)
        End If
        sw.Close()
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    '====================================================END======================================================
    '=============================================================================================================
End Class
