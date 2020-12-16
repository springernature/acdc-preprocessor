'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : SUVARNA RAUT
'CLASS NAME   : ClsAutoSectionNumbering
'CREATED DATE  : 3RD JUNE 2013
'LAST MODIFIED : 14TH JUNE 2013
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
Public Class ClsAutoSectionNumbering
    Public Xdoc As New Xml.XmlDocument
    Dim ChapterID As String = ""
    Public Sub New(ByVal xmldoc As Xml.XmlDocument, ByVal CID As String, ByVal NumberingType As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:New
        'PARAMETER    :xmldoc,CID,NumberingType
        'AIM          :This is constructor.It initailise the data member
        '=============================================================================================================
        '=============================================================================================================
        Xdoc = xmldoc
        ChapterID = CID
        SectionNumbering(NumberingType)
        '====================================================END======================================================
        '========================================================   =====================================================
    End Sub
    Private Function SectionNumbering(ByVal NumberingType As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:SectionNumbering
        'PARAMETER    :NumberingType
        'AIM          :This function will add the Section and Chapter Number according to the NumberingStyle
        '=============================================================================================================
        '=============================================================================================================
        NumberingType = NumberingType.ToLower
        Try
            If (NumberingType.ToLower.Trim = "chaptercontent") Then
                NumberingType = "ContentOnly"
            End If
        Catch ex As Exception

        End Try
        Try
            If (NumberingType.ToLower.Trim = "contentonly" Or NumberingType.ToLower.Trim = "chaptercontent") Then
                Try
                    'Section1/Heading
                    Dim Sec1Count As Integer = 0
                    Dim GetAllSection1_Heading As Xml.XmlNodeList = Xdoc.SelectNodes(".//Section1/Heading[not(.='') and not(.='')]")
                    If (IsNothing(GetAllSection1_Heading) = False) Then
                        Try
                            For i As Integer = 0 To GetAllSection1_Heading.Count - 1
                                Try
                                    Dim Head1_No As String = ""
                                    Dim Sec1_Head As Xml.XmlNode = GetAllSection1_Heading(i)
                                    Dim Sec2Exist As Boolean = False
                                    If (Sec1_Head.InnerText = " ") Then
                                        Dim s2Nds As Xml.XmlNodeList = Sec1_Head.SelectNodes(".//Section2/Heading")
                                        For j As Integer = 0 To s2Nds.Count - 1
                                            If (s2Nds(j).InnerText <> " " Or s2Nds(j).InnerText <> "") Then
                                                Sec2Exist = True
                                            End If
                                        Next
                                    End If
                                    If (Sec2Exist Or (Sec1_Head.InnerText <> "" And Sec1_Head.InnerText <> " ")) Then
                                        Try
                                            Sec1Count += 1
                                            Head1_No = Sec1Count 'CInt(i + 1)
                                            Dim head1_atr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_headno")
                                            head1_atr.Value = Head1_No.ToString
                                            Sec1_Head.ParentNode.Attributes.Append(head1_atr)
                                        Catch ex As Exception

                                        End Try
                                        Try
                                            If (Sec1_Head.InnerText <> " ") Then
                                                If (NumberingType = "chaptercontent") Then
                                                    If (Infilemame = "506") Then
                                                        Sec1_Head.InnerXml = ChapterID + "." + Head1_No.ToString + ".<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec1_Head.InnerXml
                                                    Else
                                                        Sec1_Head.InnerXml = ChapterID + "." + Head1_No.ToString + "<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec1_Head.InnerXml
                                                    End If
                                                Else
                                                    ' Sec1_Head.InnerXml = Head1_No.ToString + ". " + Sec1_Head.InnerXml
                                                    If (Infilemame = "501" Or Infilemame = "506") Then
                                                        Sec1_Head.InnerXml = Head1_No.ToString + ".<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec1_Head.InnerXml
                                                    Else
                                                        Sec1_Head.InnerXml = Head1_No.ToString + "<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec1_Head.InnerXml

                                                    End If
                                                End If

                                            End If
                                        Catch ex As Exception

                                        End Try
                                    End If

                                Catch ex As Exception

                                End Try
                            Next
                        Catch ex As Exception

                        End Try
                    End If
                Catch ex As Exception

                End Try

                Try
                    'Section2/Heading
                    Dim GetAllSection2_Heading As Xml.XmlNodeList = Xdoc.SelectNodes(".//Section2/Heading[not(.='') and not(.='')]")
                    If (IsNothing(GetAllSection2_Heading) = False) Then
                        Try
                            Dim Previous_sec1Head As String = ""
                            Dim No_h1 As Integer = 0
                            For i As Integer = 0 To GetAllSection2_Heading.Count - 1
                                Try
                                    Dim Sec2_Head As Xml.XmlNode = GetAllSection2_Heading(i)
                                    Dim Head2_No As String = ""
                                    Dim Sec2_Parent As Xml.XmlNode = Sec2_Head.ParentNode.ParentNode
                                    Try
                                        If (Previous_sec1Head = Sec2_Parent.Attributes.ItemOf("cs_headno").Value.ToString) Then
                                            No_h1 = No_h1 + 1
                                        Else
                                            No_h1 = 1
                                        End If
                                    Catch ex As Exception

                                    End Try
                                    Head2_No = Sec2_Parent.Attributes.ItemOf("cs_headno").Value.ToString + "." + CInt(No_h1).ToString
                                    Try
                                        Dim head2_atr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_headno")
                                        head2_atr.Value = Head2_No.ToString
                                        Sec2_Head.ParentNode.Attributes.Append(head2_atr)
                                    Catch ex As Exception

                                    End Try
                                    Try
                                        If (Sec2_Head.InnerText <> " ") Then
                                            If (NumberingType = "chaptercontent") Then
                                                If (Infilemame = "506") Then
                                                    Sec2_Head.InnerXml = ChapterID + "." + Head2_No.ToString + ".<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec2_Head.InnerXml
                                                Else
                                                    Sec2_Head.InnerXml = ChapterID + "." + Head2_No.ToString + "<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec2_Head.InnerXml
                                                End If

                                            Else
                                                If (Infilemame = "506") Then
                                                    Sec2_Head.InnerXml = Head2_No.ToString + ".<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec2_Head.InnerXml
                                                Else
                                                    Sec2_Head.InnerXml = Head2_No.ToString + "<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec2_Head.InnerXml
                                                End If

                                            End If
                                        End If

                                    Catch ex As Exception

                                    End Try
                                    Previous_sec1Head = Sec2_Parent.Attributes.ItemOf("cs_headno").Value.ToString
                                Catch ex As Exception

                                End Try

                            Next
                        Catch ex As Exception

                        End Try
                    End If
                Catch ex As Exception

                End Try

                Try
                    'Section3/Heading
                    Dim GetAllSection3_Heading As Xml.XmlNodeList = Xdoc.SelectNodes(".//Section3/Heading[not(.='') and not(.='')]")
                    If (IsNothing(GetAllSection3_Heading) = False) Then
                        Try
                            Dim Previous_sec2Head As String = ""
                            Dim No_h2 As Integer = 0
                            For i As Integer = 0 To GetAllSection3_Heading.Count - 1
                                Try
                                    Dim Sec3_Head As Xml.XmlNode = GetAllSection3_Heading(i)
                                    Dim Head3_No As String = ""
                                    Dim Sec3_Parent As Xml.XmlNode = Sec3_Head.ParentNode.ParentNode
                                    Try
                                        If (Previous_sec2Head = Sec3_Parent.Attributes.ItemOf("cs_headno").Value.ToString) Then
                                            No_h2 = No_h2 + 1
                                        Else
                                            No_h2 = 1
                                        End If
                                    Catch ex As Exception

                                    End Try
                                    Head3_No = Sec3_Parent.Attributes.ItemOf("cs_headno").Value.ToString + "." + CInt(No_h2).ToString
                                    Try
                                        Dim head3_atr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_headno")
                                        head3_atr.Value = Head3_No.ToString
                                        Sec3_Head.ParentNode.Attributes.Append(head3_atr)
                                    Catch ex As Exception

                                    End Try
                                    Try
                                        If (Sec3_Head.InnerText <> " ") Then
                                            If (NumberingType = "chaptercontent") Then
                                                If (Infilemame = "506") Then
                                                    Sec3_Head.InnerXml = ChapterID + "." + Head3_No.ToString + ".<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec3_Head.InnerXml
                                                Else
                                                    Sec3_Head.InnerXml = ChapterID + "." + Head3_No.ToString + "<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec3_Head.InnerXml
                                                End If

                                            Else
                                                If (Infilemame = "506") Then
                                                    Sec3_Head.InnerXml = Head3_No.ToString + ".<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec3_Head.InnerXml
                                                Else
                                                    Sec3_Head.InnerXml = Head3_No.ToString + "<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>" + Sec3_Head.InnerXml
                                                End If

                                            End If
                                        End If
                                    Catch ex As Exception

                                    End Try
                                    Previous_sec2Head = Sec3_Parent.Attributes.ItemOf("cs_headno").Value.ToString
                                Catch ex As Exception

                                End Try

                            Next
                        Catch ex As Exception

                        End Try
                    End If
                Catch ex As Exception

                End Try
            End If
        Catch ex As Exception

        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    '====================================================END======================================================
    '=============================================================================================================
End Class
