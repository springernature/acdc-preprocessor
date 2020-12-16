'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : SUVARNA RAUT
'MODULE NAME   : MDLFunction
'CREATED DATE  : 3RD JUNE 2013
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
Imports MySql.Data.MySqlClient
Imports System.Text.RegularExpressions
Module MDLFunction
    Public Xdoc As New Xml.XmlDocument
    Dim XDocString As String
    Dim NumberingType As String = ""
    Dim CID As Integer = 0
    Dim RunningHeadStyle As String = ""
    Dim JournalName As String = ""
    Dim OpenChoiceCopyrightNd As Xml.XmlNode
    Dim ignorehistroyframe As String = configDoc.SelectSingleNode(".//Ignorehistoryframe").InnerText ''System.Configuration.ConfigurationSettings.AppSettings("Ignorehistoryframe").ToString()
    Dim Hframe As String = ""
    Sub Main(ByVal Xxdoc As Xml.XmlDocument)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Main
        'PARAMETER    :Xxdoc
        'AIM          :This is main function
        '=============================================================================================================
        '=============================================================================================================
        Xdoc = Xxdoc
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function CheckLayoutname_ACDC(ByVal inxmlstring As String)
        LayoutnameDB = ""


        If (System.IO.File.Exists(ACDC_Jonsheetpath) = True) Then
            ACDCXdoc.Load(ACDC_Jonsheetpath)
        End If
        Dim templayout As String = ""
        If (IsNothing(ACDCXdoc.SelectSingleNode(".//Typesetting/@Layout").Value) = False) Then
            templayout = ACDCXdoc.SelectSingleNode(".//Typesetting/@Layout").Value
        End If
        Dim temDoc As New Xml.XmlDocument
        temDoc.LoadXml(FileOperations.GetFile(ConfigFilePaths.conFigPath).ToString())

        If (templayout.ToLower = "medium") Then
            LayoutnameDB = temDoc.SelectSingleNode(".//TypeSettingMediumLayout").InnerText
        End If
        If (templayout.ToLower = "smallextended") Then
            LayoutnameDB = temDoc.SelectSingleNode(".//TypeSettingSmallextendedLayout").InnerText
        End If
        If (templayout.ToLower = "large") Then
            LayoutnameDB = temDoc.SelectSingleNode(".//TypeSettingLargeLayout").InnerText
        End If
        If (templayout.ToLower = "smallcondensed") Then
            LayoutnameDB = temDoc.SelectSingleNode(".//TypeSettingSmallcondensedLayout").InnerText
        End If
    End Function

    Public Function AddInitialAttr()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:AddInitialAttr
        'PARAMETER    :-
        'AIM          :This function add attribute to xml
        '=============================================================================================================
        '=============================================================================================================
        Try
            oReq.AddAttribute(".//Table", "cs_tbltype", "normal")
            oReq.AddAttribute(".//Figure[@Float='no' or @Float='No']", "cs_figstyle", "Style3")
            oReq.AddAttribute(".//Biography", "cs_imageeixst", "no")
            oReq.AddAttribute(".//Equation", "cs_type", "Equation")
            oReq.AddAttribute(".//InlineEquation", "cs_type", "Equation")
            oReq.AddAttribute(".//InlineEquation", "cs_category", "Inline")
            oReq.AddAttribute(".//Equation", "cs_category", "Unnumbered")
            oReq.AddAttribute(".//Equation[EquationNumber]", "cs_category", "Numbered")

        Catch ex As Exception
            CLog.LogMessages("Error in AddInitialAttr()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Sub ApplyRowTableStyle(ByVal TblNd As Xml.XmlNode)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyRowTableStyle
        'PARAMETER    :TblNd
        'AIM          :This function give alternate fill to table row
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim totcol As String = TblNd.Attributes.ItemOf("aid:tcols").Value

            '********************************************Table caption**************************************************
            Dim qryCaption As String = ".//Cell[(@cs_type='tablecaption') ]"
            Dim captionNd As Xml.XmlNodeList = TblNd.SelectNodes(qryCaption, oReq.NS)
            For Each capNd As Xml.XmlNode In captionNd
                oReq.AddAttribute(capNd, "cs:cellstyleindex", "1")
            Next
            '***********************************************************************************************************


            '********************************************Table Head*****************************************************

            Dim qryHead As String = ".//Cell[(@cs_type='tgroup/thead/row/entry') and entry[@cs_rowindex mod 2!=0]]"
            Dim OddHeadNd As Xml.XmlNodeList = TblNd.SelectNodes(qryHead, oReq.NS)
            For Each nd As Xml.XmlNode In OddHeadNd
                oReq.AddAttribute(nd, "cs:cellstyleindex", "5")
            Next

            Dim queryHead As String = ".//Cell[(@cs_type='tgroup/thead/row/entry') and entry[@cs_rowindex mod 2=0]]"
            Dim EvenHeadNd As Xml.XmlNodeList = TblNd.SelectNodes(queryHead, oReq.NS)
            For Each nd As Xml.XmlNode In EvenHeadNd
                oReq.AddAttribute(nd, "cs:cellstyleindex", "8")
            Next

            Dim reqHeadNd As Xml.XmlNode = TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/thead/row/entry') and (entry[@colname='c" + totcol + "'])][last()]")
            If (IsNothing(reqHeadNd) = False) Then
                Dim InnerHeadNode As Xml.XmlNode = reqHeadNd.SelectSingleNode("./entry")
                Dim RowHeadCnt = InnerHeadNode.Attributes.ItemOf("cs_rowindex").Value
                For i As Int16 = 1 To CInt(RowHeadCnt)
                    If (i Mod 2 = 0) Then
                        oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/thead/row/entry') and (entry[@cs_rowindex='" + CStr(i) + "'])][last()]"), "cs:cellstyleindex", "9")
                        oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/thead/row/entry')  and (entry[@cs_rowindex='" + CStr(i) + "'])][1]"), "cs:cellstyleindex", "7")

                    Else
                        oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/thead/row/entry') and (entry[@cs_rowindex='" + CStr(i) + "'])][last()]"), "cs:cellstyleindex", "6")
                        oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/thead/row/entry')  and (entry[@cs_rowindex='" + CStr(i) + "'])][1]"), "cs:cellstyleindex", "4")

                    End If
                Next

            End If

            '***********************************************************************************************************
            oReq.AddAttribute(".//Cell[@cs_type='tgroup/thead/row/entry']", "aid:theader", "")
            '***********************************************************************************************************


            '********************************************Table Boby*****************************************************
            Dim qry As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and entry[@cs_rowindex mod 2!=0]]"
            Dim OddNd As Xml.XmlNodeList = TblNd.SelectNodes(qry, oReq.NS)
            For Each nd As Xml.XmlNode In OddNd
                oReq.AddAttribute(nd, "cs:cellstyleindex", "11")
            Next

            Dim query As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and entry[@cs_rowindex mod 2=0]]"
            Dim EvenNd As Xml.XmlNodeList = TblNd.SelectNodes(query, oReq.NS)
            For Each nd As Xml.XmlNode In EvenNd
                oReq.AddAttribute(nd, "cs:cellstyleindex", "14")
            Next


            Dim reqNd As Xml.XmlNode = TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + totcol + "'])][last()]")
            Dim InnerNode As Xml.XmlNode = reqNd.SelectSingleNode("./entry")
            Dim RowCnt = InnerNode.Attributes.ItemOf("cs_rowindex").Value
            For i As Int16 = 1 To CInt(RowCnt)
                If (i Mod 2 = 0) Then
                    oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@cs_rowindex='" + CStr(i) + "'])][last()]"), "cs:cellstyleindex", "15")
                    oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry')  and (entry[@cs_rowindex='" + CStr(i) + "'])][1]"), "cs:cellstyleindex", "13")

                Else
                    oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@cs_rowindex='" + CStr(i) + "'])][last()]"), "cs:cellstyleindex", "12")
                    oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry')  and (entry[@cs_rowindex='" + CStr(i) + "'])][1]"), "cs:cellstyleindex", "10")

                End If
            Next
            '*********************************************************************************************************

            '********************************************Table caption**************************************************

            Dim qryFooter As String = ".//Cell[(@cs_type='tablefooter') ]"
            Dim footerNd As Xml.XmlNodeList = TblNd.SelectNodes(qryFooter, oReq.NS)
            For Each footNd As Xml.XmlNode In footerNd
                oReq.AddAttribute(footNd, "cs:cellstyleindex", "19")
            Next
        Catch ex As Exception
            CLog.LogMessages("Error in ApplyRowTableStyle()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub TableStyles()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:TableStyles
        'PARAMETER    :-
        'AIM          :This function apply row table style and column table style.
        '=============================================================================================================
        '=============================================================================================================
        Try
            oReq.AddAttribute(".//table", "cs_land", "no")
            oReq.AddAttribute(".//table[@opm='land']", "cs_land", "yes")
            oReq.AddAttribute(".//table[@Float='no' or @Float='No']", "cs_Inline", "yes")
            oReq.AddAttribute(".//Table", "cs_alternate", "yes")
            Dim GetTables As Xml.XmlNodeList = Xdoc.SelectNodes(".//Cell/..", oReq.NS)
            For Each TblNd As Xml.XmlNode In GetTables
                If (TblNd.Attributes.ItemOf("cs_alternate").Value = "yes") Then
                    ApplyRowTableStyle(TblNd)
                Else
                    ApplyColumnTableStyle(TblNd)
                End If
            Next
        Catch ex As Exception
            CLog.LogMessages("Error in TableStyles()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function Fn_JSRAltRow()
        Dim GetTables As Xml.XmlNodeList = Xdoc.SelectNodes(".//Cell/..", oReq.NS)
        For Each TblNd As Xml.XmlNode In GetTables
            Dim totcol As String = TblNd.Attributes.ItemOf("aid:tcols").Value
            '==============================================================================================================================================================
            Dim Q_lefthead_M As String = ".//Cell[(@cs_type='tgroup/thead/row/entry')]"
            Dim Q_leftheadNd_M As Xml.XmlNodeList = TblNd.SelectNodes(Q_lefthead_M, oReq.NS)
            For Each nd As Xml.XmlNode In Q_leftheadNd_M
                If (IsNothing(nd.Attributes.ItemOf("cs_tblalt")) = False) Then
                    nd.Attributes.ItemOf("cs_tblalt").Value = "head_middle"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "head_middle"
                    nd.Attributes.Append(apt)
                End If
            Next
            '==============================================================================================================================================================
            Dim Q_lefthead As String = ".//Cell[(@cs_type='tgroup/thead/row/entry') and (entry[@colname='c1'])]"
            Dim Q_leftheadNd As Xml.XmlNodeList = TblNd.SelectNodes(Q_lefthead, oReq.NS)
            For Each nd As Xml.XmlNode In Q_leftheadNd
                If (IsNothing(nd.Attributes.ItemOf("cs_tblalt")) = False) Then
                    nd.Attributes.ItemOf("cs_tblalt").Value = "head_left"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "head_left"
                    nd.Attributes.Append(apt)
                End If
            Next
            '==============================================================================================================================================================
            Dim Q_righthead As String = ".//Cell[(@cs_type='tgroup/thead/row/entry') and (entry[@colname='c" + totcol + "'])]"
            Dim Q_rightheadNd As Xml.XmlNodeList = TblNd.SelectNodes(Q_righthead, oReq.NS)
            For Each nd As Xml.XmlNode In Q_rightheadNd
                If (IsNothing(nd.Attributes.ItemOf("cs_tblalt")) = False) Then
                    nd.Attributes.ItemOf("cs_tblalt").Value = "head_right"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "head_right"
                    nd.Attributes.Append(apt)
                End If
            Next
            '==============================================================================================================================================================
            Dim query_rowlast_M As String = ".//Cell[(@cs_type='tgroup/thead/row/entry') and (entry[@rowposition='last'])]"
            Dim EvenNd_rowlast_M As Xml.XmlNodeList = TblNd.SelectNodes(query_rowlast_M, oReq.NS)
            For Each nd As Xml.XmlNode In EvenNd_rowlast_M
                If (IsNothing(nd.Attributes.ItemOf("cs_tblalt")) = False) Then
                    nd.Attributes.ItemOf("cs_tblalt").Value = "last_head_middle"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "last_head_middle"
                    nd.Attributes.Append(apt)
                End If
            Next
            '==============================================================================================================================================================
            Dim left_last_M As Xml.XmlNode = TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/thead/row/entry') and (entry[@colname='c1' and @rowposition='last'])]")
            If (IsNothing(left_last_M) = False) Then
                If (IsNothing(left_last_M.Attributes.ItemOf("cs_tblalt")) = False) Then
                    left_last_M.Attributes.ItemOf("cs_tblalt").Value = "last_head_left"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "last_head_left"
                    left_last_M.Attributes.Append(apt)
                End If
            End If
            '==============================================================================================================================================================
            Dim Right_last_M As Xml.XmlNode = TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/thead/row/entry') and (entry[@colname='c" + totcol + "' and @rowposition='last'])]")
            If (IsNothing(Right_last_M) = False) Then
                If (IsNothing(Right_last_M.Attributes.ItemOf("cs_tblalt")) = False) Then
                    Right_last_M.Attributes.ItemOf("cs_tblalt").Value = "last_head_right"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "last_head_right"
                    Right_last_M.Attributes.Append(apt)
                End If
            End If
            '==============================================================================================================================================================

            '==============================================================================================================================================================
            '==============================================================================================================================================================
            Dim query As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@rowposition='first'])]"
            Dim EvenNd As Xml.XmlNodeList = TblNd.SelectNodes(query, oReq.NS)
            For Each nd As Xml.XmlNode In EvenNd
                If (IsNothing(nd.Attributes.ItemOf("cs_tblalt")) = False) Then
                    nd.Attributes.ItemOf("cs_tblalt").Value = "first_middle"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "first_middle"
                    nd.Attributes.Append(apt)
                End If
            Next
            '==============================================================================================================================================================
            Dim query_col As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c1'])]"
            Dim EvenNd_col As Xml.XmlNodeList = TblNd.SelectNodes(query_col, oReq.NS)
            For Each nd As Xml.XmlNode In EvenNd_col
                If (IsNothing(nd.Attributes.ItemOf("cs_tblalt")) = False) Then
                    nd.Attributes.ItemOf("cs_tblalt").Value = "first_col_middle"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "first_col_middle"
                    nd.Attributes.Append(apt)
                End If
            Next
            '==============================================================================================================================================================
            Dim query_collast As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + totcol + "'])]"
            Dim EvenNd_collast As Xml.XmlNodeList = TblNd.SelectNodes(query_collast, oReq.NS)
            For Each nd As Xml.XmlNode In EvenNd_collast
                If (IsNothing(nd.Attributes.ItemOf("cs_tblalt")) = False) Then
                    nd.Attributes.ItemOf("cs_tblalt").Value = "last_col_middle"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "last_col_middle"
                    nd.Attributes.Append(apt)
                End If

            Next
            '==============================================================================================================================================================
            Dim query_rowlast As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@rowposition='last'])]"
            Dim EvenNd_rowlast As Xml.XmlNodeList = TblNd.SelectNodes(query_rowlast, oReq.NS)
            For Each nd As Xml.XmlNode In EvenNd_rowlast
                If (IsNothing(nd.Attributes.ItemOf("cs_tblalt")) = False) Then
                    nd.Attributes.ItemOf("cs_tblalt").Value = "last_middle"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "last_middle"
                    nd.Attributes.Append(apt)
                End If
            Next
            '==============================================================================================================================================================
            Dim Left_First As Xml.XmlNode = TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c1' and @rowposition='first'])]")
            If (IsNothing(Left_First) = False) Then
                If (IsNothing(Left_First.Attributes.ItemOf("cs_tblalt")) = False) Then
                    Left_First.Attributes.ItemOf("cs_tblalt").Value = "first_left"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "first_left"
                    Left_First.Attributes.Append(apt)
                End If
            End If
            '==============================================================================================================================================================
            Dim Right_First As Xml.XmlNode = TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + totcol + "' and @rowposition='first'])]")
            If (IsNothing(Right_First) = False) Then
                If (IsNothing(Right_First.Attributes.ItemOf("cs_tblalt")) = False) Then
                    Right_First.Attributes.ItemOf("cs_tblalt").Value = "first_right"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "first_right"
                    Right_First.Attributes.Append(apt)
                End If
            End If
            '==============================================================================================================================================================
            Dim left_last As Xml.XmlNode = TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c1' and @rowposition='last'])]")
            If (IsNothing(left_last) = False) Then
                If (IsNothing(left_last.Attributes.ItemOf("cs_tblalt")) = False) Then
                    'left_last.Attributes.ItemOf("cs_tblalt").Value = "last_left"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "last_left"
                    left_last.Attributes.Append(apt)
                End If
            End If
            '==============================================================================================================================================================
            Dim Right_last As Xml.XmlNode = TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + totcol + "' and @rowposition='last'])]")
            If (IsNothing(Right_last) = False) Then
                If (IsNothing(Right_last.Attributes.ItemOf("cs_tblalt")) = False) Then
                    Right_last.Attributes.ItemOf("cs_tblalt").Value = "last_right"
                Else
                    Dim apt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblalt")
                    apt.Value = "last_right"
                    Right_last.Attributes.Append(apt)
                End If
            End If
            '==============================================================================================================================================================
            '==============================================================================================================================================================
        Next


        ''=============================================================================================================
        Dim myxdoc As New Xml.XmlDocument
        myxdoc.LoadXml(Locationxmlpath)
        Dim ApplyCelltyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyJSRTableCellstyle")
        If (IsNothing(ApplyCelltyleNd) = False) Then
            Dim nodes As Xml.XmlNodeList = ApplyCelltyleNd.SelectNodes(".//node")
            If (IsNothing(nodes) = False And nodes.Count > 0) Then
                Try
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid5:cellstyle", Nd.Attributes.ItemOf("cellstyle").Value)
                    Next
                Catch ex As Exception
                    CLog.LogMessages("Error in ApplyJSRTableCellstyle()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            End If
        End If


    End Function
    Public Sub ApplyTableCellStyle()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyTableCellStyle
        'PARAMETER    :-
        'AIM          :This function apply table style and table cell style.
        '=============================================================================================================
        '=============================================================================================================
        Dim myxdoc As New Xml.XmlDocument
        myxdoc.LoadXml(Locationxmlpath)
        Dim ApplyParagraphstyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyTablestyle")
        If (IsNothing(ApplyParagraphstyleNd) = False) Then
            Dim nodes As Xml.XmlNodeList = ApplyParagraphstyleNd.SelectNodes(".//node")
            If (IsNothing(nodes) = False And nodes.Count > 0) Then
                Try
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid5:tablestyle", Nd.Attributes.ItemOf("tblstyle").Value)
                    Next
                Catch ex As Exception
                    CLog.LogMessages("Error in ApplyTableCellStyle()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            End If
        End If

        Dim ApplyCelltyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyTableCellstyle")
        If (IsNothing(ApplyCelltyleNd) = False) Then
            Dim nodes As Xml.XmlNodeList = ApplyCelltyleNd.SelectNodes(".//node")
            If (IsNothing(nodes) = False And nodes.Count > 0) Then
                Try
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid5:cellstyle", Nd.Attributes.ItemOf("cellstyle").Value)
                    Next
                Catch ex As Exception
                    CLog.LogMessages("Error in ApplyTableCellStyle()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            End If
        End If

        'If (LayoutnameDB.ToLower.Contains("bsl_large_ns") = True) Then
        '    Fn_BSLAltRow()
        'End If
        Fn_JSRAltRow()
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function Fn_BSLAltRow()
        Dim GetTables As Xml.XmlNodeList = Xdoc.SelectNodes(".//Cell/..", oReq.NS)
        For Each TblNd As Xml.XmlNode In GetTables
            Dim totcol As String = TblNd.Attributes.ItemOf("aid:tcols").Value
            Dim qry As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and entry[@cs_rowindex mod 2!=0]]"
            Dim OddNd As Xml.XmlNodeList = TblNd.SelectNodes(qry, oReq.NS)
            For Each nd As Xml.XmlNode In OddNd
                oReq.AddAttribute(nd, "cs_tblalt", "first_middle")
            Next

            Dim query As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and entry[@cs_rowindex mod 2=0]]"
            Dim EvenNd As Xml.XmlNodeList = TblNd.SelectNodes(query, oReq.NS)
            For Each nd As Xml.XmlNode In EvenNd
                oReq.AddAttribute(nd, "cs_tblalt", "second_middle")
            Next


            Dim reqNd As Xml.XmlNode = TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + totcol + "'])][last()]")
            Dim InnerNode As Xml.XmlNode = reqNd.SelectSingleNode("./entry")
            Dim RowCnt = InnerNode.Attributes.ItemOf("cs_rowindex").Value
            For i As Int16 = 1 To CInt(RowCnt)
                If (i Mod 2 = 0) Then
                    oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@cs_rowindex='" + CStr(i) + "'])][last()]"), "cs_tblalt", "second_last")
                    oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry')  and (entry[@cs_rowindex='" + CStr(i) + "'])][1]"), "cs_tblalt", "second_first")

                Else
                    oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@cs_rowindex='" + CStr(i) + "'])][last()]"), "cs_tblalt", "first_last")
                    oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry')  and (entry[@cs_rowindex='" + CStr(i) + "'])][1]"), "cs_tblalt", "first_first")

                End If
            Next
        Next


        '=============================================================================================================
        Dim myxdoc As New Xml.XmlDocument
        myxdoc.Load(Locationxmlpath)
        Dim ApplyCelltyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyBSLTableCellstyle")
        If (IsNothing(ApplyCelltyleNd) = False) Then
            Dim nodes As Xml.XmlNodeList = ApplyCelltyleNd.SelectNodes(".//node")
            If (IsNothing(nodes) = False And nodes.Count > 0) Then
                Try
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid5:cellstyle", Nd.Attributes.ItemOf("cellstyle").Value)
                    Next
                Catch ex As Exception
                    CLog.LogMessages("Error in ApplyBSLTableCellstyle()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            End If
        End If


    End Function
    Public Sub AddTableAttr()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:AddTableAttr
        'PARAMETER    :-
        'AIM          :This function add required table attribute.
        '=============================================================================================================
        '=============================================================================================================
        Try
            'commented for table optimazation
            ''Dim GetTables As Xml.XmlNodeList = Xdoc.SelectNodes(".//Cell/..", oReq.NS)
            ''For Each TblNd As Xml.XmlNode In GetTables
            ''    Dim val As String = TblNd.ParentNode.ParentNode.Attributes.ItemOf("Style").Value
            ''    Dim TbNd As Xml.XmlNodeList = TblNd.SelectNodes(".//Cell", oReq.NS)
            ''    For Each tNd As Xml.XmlNode In TbNd
            ''        oReq.AddAttribute(tNd, "cs_tblstyle", val)
            ''    Next
            ''Next
            'commented for table optimazation
            Try
                Dim CellNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Cell")
                For Each Cnd As Xml.XmlNode In CellNds
                    Dim AttrName As String = Cnd.Attributes.ItemOf("cs_crows").Name
                    Dim AttrValue As String = Cnd.Attributes.ItemOf("cs_crows").Value
                    Cnd.Attributes.Remove(Cnd.Attributes.ItemOf("cs_crows"))
                    Dim crowattr As Xml.XmlAttribute = Xdoc.CreateAttribute(AttrName)
                    crowattr.Value = AttrValue
                    Cnd.Attributes.Append(crowattr)
                Next
            Catch ex As Exception

            End Try
            Dim myxdoc As New Xml.XmlDocument
            myxdoc.LoadXml(Locationxmlpath)
            Dim ApplyParagraphstyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyTableParagraphstyle")
            If (IsNothing(ApplyParagraphstyleNd) = False) Then
                Dim nodes As Xml.XmlNodeList = ApplyParagraphstyleNd.SelectNodes(".//node")
                If (IsNothing(nodes) = False And nodes.Count > 0) Then
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid:pstyle", Nd.Attributes.ItemOf("pstyle").Value)
                    Next
                End If
            End If


            Dim myxdoc1 As New Xml.XmlDocument
            myxdoc1.LoadXml(Locationxmlpath)
            Dim ApplyCharacterstyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyTableCharacterstyle")
            If (IsNothing(ApplyCharacterstyleNd) = False) Then
                Dim nodes As Xml.XmlNodeList = ApplyCharacterstyleNd.SelectNodes(".//node")
                If (IsNothing(nodes) = False And nodes.Count > 0) Then
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid:cstyle", Nd.Attributes.ItemOf("cstyle").Value)
                    Next
                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in AddTableAttr()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Private Function DecideEndnoteOrFootnote(ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:DecideEndnoteOrFootnote
        'PARAMETER    :inputxml
        'AIM          :This function check whether given footnote is footnote or endnote
        '=============================================================================================================
        '=============================================================================================================
        Dim Result As Boolean = False
        Try

            For k As Integer = 0 To ProcessFootNoteAsEndnote.Split("|").Length - 1
                Dim Str As String = ProcessFootNoteAsEndnote.Split("|")(k)
                Try
                    If (inputxml.Contains(Str + "_") = True) Then
                        Result = True
                        Return Result
                    End If
                Catch ex As Exception

                End Try
            Next

        Catch ex As Exception
            CLog.LogMessages("Error in DecideEndnoteOrFootnote()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        Return Result
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Sub ApplyFootnote(ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyFootnote
        'PARAMETER    :inputxml
        'AIM          :This function add the footnote
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim AllFootNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Footnote")
            If (IsNothing(AllFootNds) = False And AllFootNds.Count > 0) Then
                Dim ProcessEndNote As Boolean = False
                ProcessEndNote = DecideEndnoteOrFootnote(inputxml)
                If (ProcessEndNote = True) Then
                    Dim ftNode As Xml.XmlNodeList = Xdoc.SelectNodes(".//Footnote")
                    If (IsNothing(ftNode) = False And ftNode.Count > 0) Then
                        For i As Integer = 0 To ftNode.Count - 1
                            Dim InnerNode As Xml.XmlNode = ftNode(i)
                            Dim IntVal As Integer = Regex.Match(InnerNode.Attributes.ItemOf("ID").Value, "(\d+)", RegexOptions.Singleline).Groups(1).Value
                            If (ftNode.Count >= 10) Then
                                If (IntVal < 10) Then
                                    InnerNode.InnerXml = "<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type='footnotenumber'>" + IntVal.ToString + "</cs_text>" + ftNode(i).InnerXml
                                Else
                                    InnerNode.InnerXml = "<cs_text type='footnotenumber'>" + IntVal.ToString + "</cs_text>" + ftNode(i).InnerXml
                                End If
                            Else
                                InnerNode.InnerXml = "<cs_text type='footnotenumber'>" + IntVal.ToString + "</cs_text>" + ftNode(i).InnerXml
                            End If
                        Next
                    End If

                    Dim newNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                    Dim BodyNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//Body")
                    Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                    atr.Value = "footnotehead"
                    newNd.Attributes.Append(atr)
                    If (Article_Lg.ToLower = "de") Then
                        newNd.InnerXml = "<cs_text type='endnote'>Anmerkungen</cs_text><cs_text type='vbnewline'>" + vbNewLine + "</cs_text>"
                    Else
                        newNd.InnerXml = "<cs_text type='endnote'>Notes</cs_text><cs_text type='vbnewline'>" + vbNewLine + "</cs_text>"
                    End If
                    BodyNd.ParentNode.InsertAfter(newNd, BodyNd)

                    For ii As Integer = 0 To AllFootNds.Count - 1
                        oReq.FootnoteReposition(AllFootNds(ii), newNd, RelationShip.LastChild, "endnote")
                    Next
                Else
                    For ii As Integer = 0 To AllFootNds.Count - 1
                        oReq.FootnoteReposition(AllFootNds(ii), Xdoc.DocumentElement, RelationShip.LastChild, "footnote")
                    Next
                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in ApplyFootnote()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub ApplyColumnTableStyle(ByVal TblNd As Xml.XmlNode)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyColumnTableStyle
        'PARAMETER    :TblNd
        'AIM          :This function give alternate fill to each table cell
        '=============================================================================================================
        '=============================================================================================================
        Try
            oReq.AddAttribute(".//Cell[@cs_type='tgroup/thead/row/entry']", "cs:cellstyleindex", "1")
            Dim firstColQry As String = ".//Cell/entry[(substring(@colname,2,1)<ancestor::Table/@aid:tcols) and (../@cs_type='tgroup/tbody/row/entry')]/.."
            Dim firstRNd As Xml.XmlNodeList = TblNd.SelectNodes(firstColQry, oReq.NS)
            For Each nd As Xml.XmlNode In firstRNd
                oReq.AddAttribute(nd, "cs:cellstyleindex", "7")
            Next

            Dim lastColQry As String = ".//Cell/entry[(substring(@colname,2,1)>1) and (../@cs_type='tgroup/tbody/row/entry')]/.."
            Dim lastRNd As Xml.XmlNodeList = TblNd.SelectNodes(lastColQry, oReq.NS)
            For Each nd As Xml.XmlNode In lastRNd
                oReq.AddAttribute(nd, "cs:cellstyleindex", "9")
            Next

            'All body cell except 1'st col and the last col
            Dim qry As String = ".//Cell/entry[(substring(@colname,2,1)<ancestor::Table/@aid:tcols) and (substring(@colname,2,1)>1) and (../@cs_type='tgroup/tbody/row/entry')]/.."
            Dim rNd As Xml.XmlNodeList = TblNd.SelectNodes(qry, oReq.NS)
            For Each nd As Xml.XmlNode In rNd
                oReq.AddAttribute(nd, "cs:cellstyleindex", "8")
            Next

            'For the first 2,3, ... colcount-1 
            Dim totcol As String = TblNd.Attributes.ItemOf("aid:tcols").Value
            For i As Int16 = 2 To CInt(totcol) - 1
                Dim query As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + i.ToString + "'])][1]"
                Dim reqNd As Xml.XmlNode = TblNd.SelectSingleNode(query, oReq.NS)
                oReq.AddAttribute(reqNd, "cs:cellstyleindex", "5")
            Next

            Dim NotApply As Boolean = False
            Dim ColVal As String = ""
            For i As Int16 = 1 To CInt(totcol)
                Select Case i
                    Case 1
                        oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + CStr(1) + "'])][1]"), "cs:cellstyleindex", "4")
                        oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + CStr(1) + "'])][last()]"), "cs:cellstyleindex", "10")
                    Case CInt(totcol)
                        Dim MergeNd As Xml.XmlNode = TblNd.SelectSingleNode(".//entry", oReq.NS)
                        For Each atr As Xml.XmlAttribute In MergeNd.Attributes
                            If (atr.Name = "nameend") Then

                                ColVal = Regex.Match(atr.Value, "c(\d+)", RegexOptions.Singleline).Groups(1).Value
                                If ColVal <> totcol Then
                                    oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + totcol + "'])][1]"), "cs:cellstyleindex", "6")
                                Else
                                    NotApply = True
                                End If
                            End If
                        Next
                        If NotApply = False Then
                            oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + totcol + "'])][1]"), "cs:cellstyleindex", "6")
                        End If
                        oReq.AddAttribute(TblNd.SelectSingleNode(".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + totcol + "'])][last()]"), "cs:cellstyleindex", "12")
                    Case Else
                        Dim query As String = ".//Cell[(@cs_type='tgroup/tbody/row/entry') and (entry[@colname='c" + i.ToString + "'])][last()]"
                        Dim reqNd As Xml.XmlNode = TblNd.SelectSingleNode(query, oReq.NS)
                        oReq.AddAttribute(reqNd, "cs:cellstyleindex", "11")
                End Select

            Next
        Catch ex As Exception
            CLog.LogMessages("Error in ApplyColumnTableStyle()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub AddAdditionalInformation(ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:AddAdditionalInformation
        'PARAMETER    :inputxml
        'AIM          :This function add additional information for PGUtility in preprocessing instruction
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim AddInformation As New ArrayList

            'Article Language
            AddInformation.Add("<ArticleLang>" + Article_Lg + "</ArticleLang>")

            'Article Id
            Dim articleID As String = ""
            Dim articleNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article")
            If (IsNothing(articleNd) = False) Then
                If (articleNd.Attributes.ItemOf("ID").Value <> "") Then
                    articleID = "<CurrentArticleID>" + articleNd.Attributes.ItemOf("ID").Value + "</CurrentArticleID>"
                Else
                    articleID = "<CurrentArticleID>" + "" + "</CurrentArticleID>"
                End If
            End If

            AddInformation.Add(articleID)


            'ArticleType
            Dim articletype As String = ""
            Dim articletypeNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleInfo[@ArticleType]")
            If (IsNothing(articletypeNd) = False) Then
                If (articletypeNd.Attributes.ItemOf("ArticleType").Value <> "") Then
                    articletype = "<CurrentArticleType>" + articletypeNd.Attributes.ItemOf("ArticleType").Value + "</CurrentArticleType>"
                Else
                    articletype = "<CurrentArticleType>" + "" + "</CurrentArticleType>"
                End If
            End If

            AddInformation.Add(articletype)



            'Harddrivepath
            Dim HPathNode As String = ""
            Dim HPath As String = ""
            ''HPath = oReq.oFigObj.GetHarddrivepath(inputxml)
            HPath = HPath.Replace("\Graphics\pts\", "\pts\")
            If (HPath <> "") Then
                Dim dirInfo As New System.IO.DirectoryInfo(HPath)
                HPathNode = "<HardDrivePath>" + dirInfo.Parent.Parent.FullName + "</HardDrivePath>"
            Else
                HPathNode = "<HardDrivePath>" + "" + "</HardDrivePath>"
            End If
            AddInformation.Add(HPathNode)

            'Author type
            If (IsNothing(AuthorType) = False) Then
                AddInformation.Add("<AuthorType>" + AuthorType + "</AuthorType>")
            End If

            'Running head style
            RunningHeadStyle = "<RunningHeadStyle>" + RunningHeadStyle.Trim + "</RunningHeadStyle>"
            AddInformation.Add(RunningHeadStyle)

            'Metadata
            If (XMLM_Data <> "") Then
                XMLM_Data = "<METADATA_Info xmlns:aid=""http://ns.adobe.com/AdobeInDesign/4.0/"">" + XMLM_Data + "</METADATA_Info>"
                AddInformation.Add(XMLM_Data)
            End If

            'Cts
            AddInformation.Add(CTSINFO)

            'Offprint
            AddInformation.Add(OffPrint_Data)

            'Proof pdf info
            AddInformation.Add(PROOF_Info)

            oReq.AddInfoNode(AddInformation)
        Catch ex As Exception
            CLog.LogMessages("Error in AddAdditionalInformation()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub HideElement()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:HideElement
        'PARAMETER    :-
        'AIM          :This function hide the xml element which should not appear in out xml
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim HideList As New ArrayList
            Dim myxdoc As New Xml.XmlDocument
            myxdoc.LoadXml(Locationxmlpath)
            Dim ApplyParagraphstyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//HideElement")
            If (IsNothing(ApplyParagraphstyleNd) = False) Then
                Dim nodes As Xml.XmlNodeList = ApplyParagraphstyleNd.SelectNodes(".//node")
                If (IsNothing(nodes) = False And nodes.Count > 0) Then
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        HideList.Add(Nd.InnerText)
                    Next
                End If
            End If
            If JournalSubType = "A" Then 'Reason to add this here instead of special routine is it will not add unnecessary enter mark or any other which we have to delete again
                HideList.Add(".//cs_text[@type='auth_aff_collections']/cs_text[@type='sub_auth_aff_collections' and child::cs_text[@type='auth_collections' and child::AuthorName[position()=1 and not(@cs_type='correspond')]]]")
                HideList.Add(".//cs_text[@type='auth_aff_collections']/cs_text[@type='sub_auth_aff_collections']/cs_text[@type='auth_collections']/AuthorName[not(@cs_type='correspond')]")
            End If
            If (Infilemame = "11812") Then
                HideList.Add(".//cs_text[@type='auth_aff_collections']//ArticleNote[@Type='Misc']")
            End If
            oReq.HideNode(HideList)
        Catch ex As Exception
            CLog.LogMessages("Error in HideElement()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub RemoveAttr()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:RemoveAttr
        'PARAMETER    :-
        'AIM          :This function removed the specified attribute
        '=============================================================================================================
        '=============================================================================================================
        Dim myxdoc As New Xml.XmlDocument
        myxdoc.LoadXml(Locationxmlpath)
        Dim ApplyParagraphstyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//Removeattribute")
        If (IsNothing(ApplyParagraphstyleNd) = False) Then
            Dim nodes As Xml.XmlNodeList = ApplyParagraphstyleNd.SelectNodes(".//node")
            If (IsNothing(nodes) = False And nodes.Count > 0) Then
                Try
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.DeleteAttr(Nd.InnerText, Nd.Attributes.ItemOf("attrname").Value)
                    Next
                Catch ex As Exception
                    CLog.LogMessages("Error in RemoveAttr()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            End If
        End If
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub ApplyFrame()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyFrame
        'PARAMETER    :-
        'AIM          :This function add the xml node at the end of out xml
        '=============================================================================================================
        '=============================================================================================================
        Dim ArticleTitleFrame As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_text[@type='journaltitle']", oReq.NS)
        For ii As Integer = 0 To ArticleTitleFrame.Count - 1
            Try
                oReq.FrameReposition(ArticleTitleFrame(ii), Xdoc.DocumentElement, RelationShip.LastChild, "VSArticleTitleFrame")
            Catch ex As Exception
                CLog.LogMessages("Error in ApplyFrame()" + vbNewLine)
                CLog.LogMessages(ex.Message.ToString + vbNewLine)
                Throw
            End Try
        Next
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub ApplyCharacterStyleAbstract()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyCharacterStyle
        'PARAMETER    :-
        'AIM          :This function apply character style
        '=============================================================================================================
        '=============================================================================================================
        Try
            'Emphasis
            'oReq.AddAttribute(".//Emphasis[@Type='Bold']", "aid:cstyle", "ctest")
            'oReq.AddAttribute(".//Emphasis[@Type='Italic']", "aid:cstyle", "ctest")
            'oReq.AddAttribute(".//Emphasis[@Type='BoldItalic']", "aid:cstyle", "ctest")

            'Superscript
            oReq.AddAttribute(".//Superscript", "aid:cstyle", "superscript")
            'oReq.AddAttribute(".//Superscript[Emphasis[@Type='Bold']]", "aid:cstyle", "ctest")
            'oReq.AddAttribute(".//Superscript[Emphasis[@Type='Italic']]", "aid:cstyle", "ctest")
            'oReq.AddAttribute(".//Superscript[Emphasis[@Type='BoldItalic']]", "aid:cstyle", "ctest")

            'Subscript
            oReq.AddAttribute(".//Subscript", "aid:cstyle", "Subscript")
            'oReq.AddAttribute(".//Subscript[Emphasis[@Type='Bold']]", "aid:cstyle", "ctest")
            'oReq.AddAttribute(".//Subscript[Emphasis[@Type='Italic']]", "aid:cstyle", "ctest")
            'oReq.AddAttribute(".//Subscript[Emphasis[@Type='BoldItalic']]", "aid:cstyle", "ctest")


            'Figure
            ' oReq.AddAttribute(".//Figure/Caption/CaptionNumber", "aid:cstyle", "ctest")

            'Table
            '  oReq.AddAttribute(".//Table//CaptionNumber[not(.='')]", "aid:cstyle", "ctest")

        Catch ex As Exception
            CLog.LogMessages("Error in ApplyCharacterStyleAbstract()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub ApplyCharacterStyle()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyCharacterStyle
        'PARAMETER    :-
        'AIM          :This function apply character style
        '=============================================================================================================
        '=============================================================================================================
        Dim myxdoc As New Xml.XmlDocument
        myxdoc.LoadXml(Locationxmlpath)
        Dim ApplyParagraphstyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyCharacterstyle")
        If (IsNothing(ApplyParagraphstyleNd) = False) Then
            Dim nodes As Xml.XmlNodeList = ApplyParagraphstyleNd.SelectNodes(".//node")
            If (IsNothing(nodes) = False And nodes.Count > 0) Then
                Try
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid:cstyle", Nd.Attributes.ItemOf("cstyle").Value)
                    Next
                Catch ex As Exception
                    CLog.LogMessages("Error in ApplyCharacterStyle()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            End If
        End If
        'Entity
        ' ApplyStyleToEntity()
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function ApplyStyleToEntity()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyStyleToEntity
        'PARAMETER    :-
        'AIM          :This function apply character style to entity
        '=============================================================================================================
        '=============================================================================================================
        Dim myxdoc As New Xml.XmlDocument
        myxdoc.Load(Locationxmlpath)
        Dim ApplyParagraphstyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyEntityCharacterStyle")
        If (IsNothing(ApplyParagraphstyleNd) = False) Then
            Dim nodes As Xml.XmlNodeList = ApplyParagraphstyleNd.SelectNodes(".//node")
            If (IsNothing(nodes) = False And nodes.Count > 0) Then
                Try
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid:cstyle", Nd.Attributes.ItemOf("cstyle").Value)
                    Next
                Catch ex As Exception
                    CLog.LogMessages("Error in ApplyStyleToEntity()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            End If
        End If
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Sub ApplyParagraphStyleAbstract()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyParagraphStyleAbstract
        'PARAMETER    :-
        'AIM          :This function apply paragraph style
        '=============================================================================================================
        '=============================================================================================================
        Try
            oReq.AddAttribute(".//Section1/Heading", "aid:pstyle", "Article_Title")
            oReq.AddAttribute(".//Section1/Para", "aid:pstyle", "Head1")
            oReq.AddAttribute(".//Section2/Heading", "aid:pstyle", "Head2")
            oReq.AddAttribute(".//Section2/Para", "aid:pstyle", "Head3")
            oReq.AddAttribute(".//Section3/Heading", "aid:pstyle", "Authors")

            oReq.AddAttribute(".//Section3/Para", "aid:pstyle", "Affiliation")
            oReq.AddAttribute(".//Section3/Para[position()=last()]", "aid:pstyle", "Affiliation_Below Space")

            If (Article_Lg.ToLower = "en") Then
                oReq.AddAttribute(".//FormalPara/Para", "aid:pstyle", "Abstract_Structured_Text_EN")
            Else
                oReq.AddAttribute(".//FormalPara/Para", "aid:pstyle", "Abstract_Structured_Text_De")
            End If

            oReq.AddAttribute(".//FormalPara/Para[position()>1]", "aid:pstyle", "Body_Text")


            'Journal Title Frame
            oReq.AddAttribute(".//ArticleTitle", "aid:pstyle", "Article_Title") 'bydefault it will get apply
            oReq.AddAttribute(".//cs_text[@type='journaltitle']/ArticleTitle", "aid:pstyle", "Article_Title")
            oReq.AddAttribute(".//cs_text[@type='journaltitle']/ArticleSubTitle", "aid:pstyle", "Article_SubTitle")
        Catch ex As Exception
            CLog.LogMessages("Error in ApplyParagraphStyleAbstract()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub ApplyParagraphStyle()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyParagraphStyle
        'PARAMETER    :-
        'AIM          :This function apply paragraph style
        '=============================================================================================================
        '=============================================================================================================
        Dim myxdoc As New Xml.XmlDocument
        myxdoc.LoadXml(Locationxmlpath)
        Dim ApplyParagraphstyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyParagraphstyle")
        If (IsNothing(ApplyParagraphstyleNd) = False) Then
            Dim nodes As Xml.XmlNodeList = ApplyParagraphstyleNd.SelectNodes(".//node")
            If (IsNothing(nodes) = False And nodes.Count > 0) Then
                Try
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid:pstyle", Nd.Attributes.ItemOf("pstyle").Value)
                    Next
                    If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                        Fn_ApplyBSLParagraphStyle()
                    End If
                Catch ex As Exception
                    CLog.LogMessages("Error in ApplyParagraphStyle()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            End If
        End If

        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function Fn_ApplyBSLParagraphStyle()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ApplyParagraphStyle
        'PARAMETER    :-
        'AIM          :This function apply paragraph style
        '=============================================================================================================
        '=============================================================================================================
        Dim myxdoc As New Xml.XmlDocument
        myxdoc.Load(Locationxmlpath)
        Dim ApplyParagraphstyleNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//ApplyBSLParagraphstyle")
        If (IsNothing(ApplyParagraphstyleNd) = False) Then
            Dim nodes As Xml.XmlNodeList = ApplyParagraphstyleNd.SelectNodes(".//node")
            If (IsNothing(nodes) = False And nodes.Count > 0) Then
                Try
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        oReq.AddAttribute(Nd.InnerText, "aid:pstyle", Nd.Attributes.ItemOf("pstyle").Value)
                    Next
                Catch ex As Exception
                    CLog.LogMessages("Error in Fn_ApplyBSLParagraphStyle()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            End If
        End If

        '====================================================END======================================================
        '=============================================================================================================

    End Function
    Public Function ConvertSeperateChapterAQNode(ByVal XmlDoc As Xml.XmlDocument)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:ConvertSeperateChapterAQNode
        'PARAMETER    :XmlDoc
        'AIM          :This function convert author node into single tag.
        '=============================================================================================================
        '=============================================================================================================
        Try

            Dim QueryNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_query") '".//cs_query[not(ancestor::cs_text)]"
            Dim NewNd As Xml.XmlNode = Nothing 'Xdoc.CreateElement("AQChapter")

            For i As Integer = 0 To QueryNds.Count - 1
                Try

                    Dim nd As Xml.XmlNode = QueryNds(i)
                    If (IsNothing(nd.Attributes.ItemOf("repoID")) = True) Then
                        'MsgBox("Main Node")
                        If (i <> 0) Then
                            nd.ParentNode.InsertBefore(NewNd, nd)
                        End If
                        NewNd = Xdoc.CreateElement("AQChapter")
                        NewNd.AppendChild(nd)
                    Else
                        ' MsgBox("Sub Node")
                        If (i = 0) Then
                            NewNd = Xdoc.CreateElement("AQChapter")
                            NewNd.AppendChild(nd)
                        End If
                        If (i = (QueryNds.Count - 1)) Then
                            NewNd.AppendChild(nd)
                            'nd.ParentNode.InnerXml = nd.ParentNode.InnerXml + NewNd.OuterXml
                            Dim rootnd As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='AQCollection']")
                            rootnd.AppendChild(NewNd)

                        Else
                            NewNd.AppendChild(nd)
                        End If
                    End If
                Catch ex As Exception

                End Try
            Next
        Catch ex As Exception

        End Try
        '  oReq.AddTextorXml(".//AQChapter/cs_query[@aid:pstyle='AQ_entry']", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)

        Dim AddInformation As New ArrayList
        Dim articleNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='AQCollection']")
        Dim str As String = articleNd.OuterXml
        AddInformation.Add(str)
        oReq.AddInfoNode_2(AddInformation)

        oReq.AddTextorXml(".//cs_text[@type='AQCollection']", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsPreviousSibling, True)

        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Sub RemoveNodes()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:RemoveNodes
        'PARAMETER    :inputxml
        'AIM          :This function remove the node mentioned in database.
        '=============================================================================================================
        '=============================================================================================================
        Try
            oReq.DeleteNode(".//OrderedList/ListItem/ItemContent/Para[UnorderedList]/preceding-sibling::cs_text[position()=1 and @type='indentohere']")
            oReq.DeleteNode(".//OrderedList/ListItem/ItemContent/Para[UnorderedList]/preceding-sibling::cs_text[position()=1 and @type='enspace']")
            'oReq.DeleteNode(".//Ethics/FormalPara/Heading/following-sibling::cs_text[@type='emspace' and (position()=1)]/following-sibling::cs_text[@type='enspace' and (position()=1)]")
            If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                oReq.DeleteNode(".//FormalPara[@Type='Overview']/Heading/following-sibling::cs_text[position()=1 and @type='vbnewline']/following-sibling::cs_text[position()=1 and @type='enspace']")
                oReq.DeleteNode(".//FormalPara[@Type='Overview']/Para[position()=last()]/following-sibling::cs_text[@type='vbnewline']")
                oReq.DeleteNode(".//KeywordGroup/Heading/following-sibling::cs_text[@type='vbnewline']/following-sibling::cs_text[@type='enspace']")
                oReq.DeleteNode(".//FormalPara[@Type='Questionnaire']/Heading/following-sibling::cs_text[position()=1 and (@type='vbnewline')]/following-sibling::cs_text[position()=1 and (@type='enspace')]")
                oReq.DeleteNode(".//Abstract[@Language='De' or @Language='Nl' and not(child::AbstractSection)]/Heading/following-sibling::cs_text[position()=1 and (@type='vbnewline')]/following-sibling::cs_text[position()=1 and (@type='enspace')]")
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in RemoveNodes()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function HideAQuery()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:HideAQuery
        'PARAMETER    :-
        'AIM          :This function hide author query node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim AQStr As String = ""
            Dim AQNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_query") '".//cs_query[not(ancestor::cs_text)]"
            Dim HideAQ As Xml.XmlNode = Xdoc.CreateElement("cs_text")
            Dim HideAQAttr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            HideAQAttr.Value = "AQCollection"
            HideAQ.Attributes.Append(HideAQAttr)
            For i As Integer = 0 To AQNds.Count - 1
                HideAQ.AppendChild(AQNds(i))
            Next
            Dim Root As Xml.XmlNode = Xdoc.DocumentElement
            Root.AppendChild(HideAQ)
            '  oReq.AddTextorXml(".//cs_query[not(position()=last())]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
        Catch ex As Exception
            CLog.LogMessages("Error in HideAQuery()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function AuthorQuery(ByVal XmlDoc As Xml.XmlDocument)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:AuthorQuery
        'PARAMETER    :XmlDoc
        'AIM          :This function create author query.
        '==============================================================================================================
        '==============================================================================================================

        Try
            Dim XdocString As String = XmlDoc.OuterXml
            Xdoc.InnerXml = XdocString
            Dim HideAffQuery As New ArrayList
            HideAffQuery.Add(".//AuthorGroup")
            HideAffQuery.Add(".//Abstract[not(parent::cs_text[@type='abstract_keyword']) and (@Language='De' or @Language='En' or @Language='Fr' or @Language='Es' or @Language='Nl')]")
            HideAffQuery.Add(".//KeywordGroup[not(parent::cs_text) and (@Language='De' or @Language='En' or @Language='Fr' or @Language='Es' or @Language='--')]")
            oReq.HideNode(HideAffQuery)
            Dim QueryNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_query") '".//cs_query[not(ancestor::cs_text)]"
            For i As Integer = 0 To QueryNds.Count - 1
                Dim tempNds As Xml.XmlNode = QueryNds(i)
                If (IsNothing(tempNds) = False) Then
                    Dim InnerX As String = QueryNds(i).InnerXml
                    If (Regex.Match(InnerX, "Author: ", RegexOptions.Singleline).Success = True) Then
                        Dim s As Regex
                        s = New Regex("(Author: )", RegexOptions.Singleline)
                        InnerX = s.Replace(InnerX, "", 1)
                    End If
                    QueryNds(i).InnerXml = InnerX
                End If
                Dim AttrIdVal As String = "AQ" + QueryNds(i).Attributes.ItemOf("repoID").Value
                oReq.Reposition(QueryNds(i), Xdoc.DocumentElement, RelationShip.LastChild, "Query", AttrIdVal)
            Next
        Catch ex As Exception
            CLog.LogMessages("Error in AuthorQuery()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Sub Hyperlink()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Hyperlink
        'PARAMETER    :-
        'AIM          :This function create hyperlink.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim HyperlinkSrc As String = ""
            Dim HyperlinkDest As String = ""
            Dim Counter As Integer = 0
            Dim InterCount As Integer = 0
            Dim HyperNds As Xml.XmlNodeList = configDoc.SelectNodes(".//hyperlinks/hyperlink")
            If (IsNothing(HyperNds) = False And HyperNds.Count > 0) Then
                For k As Integer = 0 To HyperNds.Count - 1
                    LinkSource = HyperNds(k).SelectSingleNode(".//source/@nodexpath").Value
                    LinksourceAttr = HyperNds(k).SelectSingleNode(".//source/@nodeattr").Value
                    LinksourcePrefix = HyperNds(k).SelectSingleNode(".//source/@attrprefix").Value
                    LinkDestination = HyperNds(k).SelectSingleNode(".//destination/@nodexpath").Value
                    LinkdestinationAttr = HyperNds(k).SelectSingleNode(".//destination/@nodeattr").Value
                    LinkdestinationPrefix = HyperNds(k).SelectSingleNode(".//destination/@attrprefix").Value
                    Dim SourceNds As Xml.XmlNodeList = Xdoc.SelectNodes(LinkSource)
                    If (IsNothing(SourceNds) = False And SourceNds.Count > 0) Then
                        For i As Integer = 0 To SourceNds.Count - 1
                            InterCount += 1
                            Dim srattribute As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_destRef")
                            Dim SrcAttrstr As String = SourceNds(i).Attributes.ItemOf(LinksourceAttr).Value.Replace(LinksourcePrefix, "")
                            If (SourceNds(i).Attributes.ItemOf(LinksourceAttr).Value.Contains(LinksourcePrefix)) Then
                                'If (LinksourcePrefix.Contains(SourceNds(i).Attributes.ItemOf(LinksourceAttr).Value)) Then
                            Else
                                Continue For
                            End If
                            'srattribute.Value = SrcAttrstr
                            srattribute.Value = InterCount


                            If (LinksourcePrefix = "Fig" Or LinksourcePrefix = "Tab" Or LinksourcePrefix = "Sch" Or LinksourcePrefix = "Str") Then
                                SourceNds(i).ChildNodes(0).Attributes.Append(srattribute)
                            Else
                                SourceNds(i).Attributes.Append(srattribute)
                            End If

                            HyperlinkDest = LinkDestination + "[@" + LinkdestinationAttr + "='" + LinksourcePrefix + SrcAttrstr + "']"
                            Dim DestinationNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//" + HyperlinkDest)
                            If (IsNothing(DestinationNds) = False And DestinationNds.Count > 0) Then
                                For j As Integer = 0 To DestinationNds.Count - 1
                                    Dim destAttribute As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_srcRef")
                                    Dim DestAttrstr As String = DestinationNds(j).Attributes.ItemOf(LinkdestinationAttr).Value.Replace(LinkdestinationPrefix, "")
                                    'destAttribute.Value = DestAttrstr
                                    destAttribute.Value = InterCount
                                    DestinationNds(j).Attributes.Append(destAttribute)
                                Next
                            End If
                        Next
                    End If
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Hyperlink()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub SideBarConversion()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:SideBarConversion
        'PARAMETER    :-
        'AIM          :This function create sidebar.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim AllSideBars As Xml.XmlNodeList = Xdoc.SelectNodes(".//Sidebar")
            For ii As Integer = 0 To AllSideBars.Count - 1
                Try
                    Dim SBNd As Xml.XmlNode = AllSideBars(ii)
                    Dim XAttr As Xml.XmlAttribute = SBNd.SelectSingleNode(".//@ID[1]")
                    Dim AttrIdVal As String = ""
                    If (IsNothing(XAttr) = False) Then
                        AttrIdVal = XAttr.Value
                    End If
                    oReq.Reposition(AllSideBars(ii), Xdoc.DocumentElement, RelationShip.LastChild, "SideBox", AttrIdVal)
                Catch ex As Exception
                End Try
            Next
        Catch ex As Exception
            CLog.LogMessages("Error in SideBarConversion()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub SpecialRoutine()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:SpecialRoutine
        'PARAMETER    :-
        'AIM          :This function do all the previous functionality also add the new code for given conditions.
        '=============================================================================================================
        '=============================================================================================================

        Dim Nodes_url As Xml.XmlNodeList = Xdoc.SelectNodes(".//test_Contact/test_URL[position()=1]|.//test_Affiliation/test_URL[position()=1]")
        If (IsNothing(Nodes_url) = False And Nodes_url.Count > 0) Then
            For j As Integer = 0 To Nodes_url.Count - 1
                Nodes_url(j).InnerXml = "URL: " + Nodes_url(j).InnerXml
            Next
        End If


        ''''oReq.AddAttribute(".//Cell[not(@cs_tblalt='head_right')  and (@cs_type='tgroup/thead/row/entry') and (child::entry[not(@rowposition='last') and (@namest) and (@nameend)])]/entry", "aid:pstyle", "Table_Head_Bridge_Rule")
        Dim xnodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//Cell/@aid:ccolwidth", oReq.NS)
        If (IsNothing(xnodes) = False And xnodes.Count > 0) Then

            For m As Integer = 0 To xnodes.Count - 1
                xnodes(m).Value = "200"
            Next
        End If

        If (Infilemame.Trim.ToLower = "501") Then
            '--------------------------------------------------------------------------------------------------------------------
            oReq.DeleteNode(".//cs_text[@type='authname']/AuthorName[not(position()=last())]/FamilyName/following-sibling::cs_text[@type='nonbreakingspace' and position()=1]")
            oReq.DeleteNode(".//cs_text[@type='authname']/AuthorName[not(position()=last())]/FamilyName/following-sibling::cs_text[@type='middot' and position()=1]")
            oReq.DeleteNode(".//cs_text[@type='authname']/AuthorName[not(position()=last())]/FamilyName/following-sibling::cs_text[@type='space' and position()=1]")

            Dim Nds As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_text[@type='authname']/AuthorName")
            If (IsNothing(Nds) = False And Nds.Count > 0) Then
                For i As Integer = 0 To Nds.Count - 2
                    If (i = Nds.Count - 2) Then
                        Dim new_nd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                        Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                        If (Article_Lg.ToLower = "de") Then
                            atr.Value = " und "
                            new_nd.InnerText = " und "
                        ElseIf (Article_Lg.ToLower = "en") Then
                            atr.Value = " and "
                            new_nd.InnerText = " and "
                        End If
                        new_nd.Attributes.Append(atr)
                        Nds(i).ParentNode.InsertAfter(new_nd, Nds(i))
                    Else
                        Dim new_nd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                        Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                        atr.Value = "comma_space"
                        new_nd.InnerText = ", "
                        new_nd.Attributes.Append(atr)
                        Nds(i).ParentNode.InsertAfter(new_nd, Nds(i))
                    End If

                Next
            End If
            '--------------------------------------------------------------------------------------------------------------------
            'Move auth name below affiliation in title frame for 501...4.08.2015
            Dim MoveNd_First As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='journaltitle']/cs_text[@type='authname']")
            Dim MoveNd_Second As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='journaltitle']/cs_text[@type='articleaddress']")
            If ((IsNothing(MoveNd_First) = False) And (IsNothing(MoveNd_Second) = False)) Then
                MoveNd_First.ParentNode.InsertAfter(MoveNd_First, MoveNd_Second)
                MoveNd_Second.ParentNode.InsertAfter(MoveNd_Second, MoveNd_Second.ParentNode.LastChild)
            End If

            '--------------------------------------------------------------------------------------------------------------------
        End If
        Try
            If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                oReq.AddTextorXml(".//FormalPara[@Type='Overview']/Heading", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
                oReq.AddTextorXml(".//KeywordGroup/Heading", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
                oReq.AddTextorXml(".//FormalPara[@Type='Questionnaire']/Heading", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
                oReq.AddTextorXml(".//Abstract[@Language='De' or @Language='Nl' and not(child::AbstractSection)]/Heading", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)

                oReq.AddTextorXml(".//tfooter/SimplePara/Superscript", "<cs_text type='thinspace'>&#8201;</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)

                Dim Nodes_email As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_text[@type='contact_collections']/Contact/Email")
                If (IsNothing(Nodes_email) = False And Nodes_email.Count > 0) Then
                    For j As Integer = 0 To Nodes_email.Count - 1
                        Nodes_email(j).InnerXml = "e-mail: " + Nodes_email(j).InnerXml
                    Next
                End If

            End If
            If (LayoutnameDB.ToLower = "globaljournalsmallextended") Then
                oReq.DeleteNode(".//cs_text[@type='abstract_keyword' and child::ArticleTitle and child::ArticleSubTitle]/ArticleTitle/following-sibling::cs_text[@type='space' and position()=1]")
                oReq.DeleteNode(".//cs_text[@type='abstract_keyword' and child::ArticleTitle and child::ArticleSubTitle]/ArticleTitle/following-sibling::cs_text[@type='endash' and position()=1]")
                oReq.DeleteNode(".//cs_text[@type='abstract_keyword' and child::ArticleTitle and child::ArticleSubTitle]/ArticleTitle/following-sibling::cs_text[@type='space' and position()=1]")

                oReq.AddTextorXml(".//cs_text[@type='abstract_keyword' and child::ArticleTitle and child::ArticleSubTitle]/ArticleTitle", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)

                oReq.AddAttribute(".//cs_text[@type='abstract_keyword' and child::ArticleTitle and child::ArticleSubTitle]/ArticleSubTitle", "aid:pstyle", "Add_Lang_SubTitle")
                oReq.AddAttribute(".//cs_text[@type='abstract_keyword' and child::ArticleTitle and child::ArticleSubTitle]/ArticleSubTitle[@Language='De']", "aid:pstyle", "Add_Lang_SubTitle_De")
                oReq.AddAttribute(".//cs_text[@type='abstract_keyword' and child::ArticleTitle and child::ArticleSubTitle]/ArticleSubTitle[@Language='En']", "aid:pstyle", "Add_Lang_SubTitle_En")
                oReq.AddAttribute(".//cs_text[@type='abstract_keyword' and child::ArticleTitle and child::ArticleSubTitle]/ArticleSubTitle[@Language='Fr']", "aid:pstyle", "Add_Lang_SubTitle_Fr")
                oReq.AddAttribute(".//cs_text[@type='abstract_keyword' and child::ArticleTitle and child::ArticleSubTitle]/ArticleSubTitle[@Language='Es']", "aid:pstyle", "Add_Lang_SubTitle_Es")
                ' oReq.AddAttribute(".//Biography[1]/FormalPara/Para", "aid:pstyle", "Biography_Entry2")
                oReq.DeleteNode(".//Biography[following-sibling::Biography]/following-sibling::cs_text[@type='vbnewline']")
            End If
            oReq.AddAttribute(".//cs:reposition/table", "aid:pstyle", "Body_Text")
            'oReq.AddTextorXml(".//Biography/FormalPara/Heading[not(.='')]", "<cs_text type='comma'>,</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
            'oReq.AddTextorXml(".//cs:reposition[position()=1]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsPreviousSibling, True)
            oReq.AddAttribute(".//cs_repos[@cs_IsInline]", "aid:pstyle", "InlineImage")
            oReq.AddAttribute(".//*[@cs_IsInlineTable]", "aid:pstyle", "InlineTable")
            oReq.AddAttribute(".//Para[cs:reposition[@cs_IsInlineTable]]", "aid:pstyle", "InlineTable")
            oReq.DeleteAttr(".//Para[cs:reposition[@cs_IsInlineTable]]//table", "aid:pstyle")
            oReq.AddAttribute(".//Para[cs:reposition[@cs_IsInlineTable]]//table", "aid:pstyle", "InlineTable")

            oReq.AddTextorXml(".//Para[cs:reposition[@cs_IsInlineTable]]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)


            If (Infilemame = "11812") Then

                Dim HideList As New ArrayList
                HideList.Add(".//ArticleHeader/TocChapter")
                oReq.HideNode(HideList)



                oReq.AddAttribute(".//cs_text[@type='journaltitle']/cs_text[@type='articleaddress']/Affiliation", "aid:pstyle", "Affilation_Author_With Space")
                oReq.AddAttribute(".//cs_text[@type='journaltitle']/cs_text[@type='articleaddress']/Affiliation[1]", "aid:pstyle", "Affilation_Author_With Rule")

                'attach articlenote to title frame
                Dim Node As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='Misc']")
                If (IsNothing(Node) = False) Then
                    Dim SrcNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='journaltitle']//cs_text[@type='authname']")
                    If (IsNothing(SrcNd) = False) Then
                        SrcNd.ParentNode.InsertAfter(Node, SrcNd)
                        oReq.AddTextorXml(".//cs_text[@type='journaltitle']//cs_text[@type='authname']", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
                    End If
                End If

                'Move questionaries after ref
                Dim SrcNd_1 As Xml.XmlNode = Xdoc.SelectSingleNode(".//Bibliography")
                Dim DestNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//Appendix/child::*[@Type='Questionnaire']")
                If (IsNothing(DestNd) = False) Then
                    If (IsNothing(SrcNd_1) = False) Then
                        SrcNd_1.ParentNode.InsertAfter(DestNd, SrcNd_1)
                    End If

                End If
                'FMpageBreak
                oReq.AddTextorXml(".//*[@Type='Questionnaire']", "<cs_text type='FMpageBreak'></cs_text>", clsPreprocMain.ChildTypes.AsPreviousSibling, True)
                'change authaff node contents and add it after lit...meand end of body
prv:            Dim MainNode1 As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_text[@type='auth_aff_collections']|.//cs_text[@type='sub_auth_aff_collections']|.//cs_text[@type='auth_collections']|.//cs_text[@type='auth_collections']/AuthorName|.//cs_text[@type='role_collections']/Affiliation|.//cs_text[@type='role_collections']/Affiliation/OrgAddress|.//cs_text[@type='contact_collections']")
                If (IsNothing(MainNode1) = False And MainNode1.Count > 0) Then
                    For m As Integer = 0 To MainNode1.Count - 1
                        Dim MainNode As Xml.XmlNode = MainNode1(m)
                        If (MainNode.ChildNodes.Count > 0) Then
                            For i As Integer = 0 To MainNode.ChildNodes.Count - 1
                                Dim nd As Xml.XmlNode = MainNode.ChildNodes(i)
                                If (nd.Name.ToLower = "cs_text") Then
                                    If (nd.Attributes.ItemOf("type").Value <> "sub_auth_aff_collections" And nd.Attributes.ItemOf("type").Value <> "auth_collections" And nd.Attributes.ItemOf("type").Value <> "role_collections" And nd.Attributes.ItemOf("type").Value <> "contact_collections") Then
                                        nd.ParentNode.RemoveChild(nd)
                                        GoTo prv
                                    End If
                                End If

                            Next
                        End If
                    Next

                End If
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']//Prefix", "<cs_text type='space'> </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']//GivenName", "<cs_text type='space'> </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']//FamilyName", "<cs_text type='commaspace'>, </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']//OrgDivision", "<cs_text type='commaspace'>, </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']//OrgName", "<cs_text type='commaspace'>, </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']//Street", "<cs_text type='commaspace'>, </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']//Postcode", "<cs_text type='space'> </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']//City", "<cs_text type='commaspace'>, </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']//Country", "<cs_text type='commaspace'>, </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.AddAttribute(".//cs_text[@type='auth_aff_collections']//cs_text[@type='head']", "aid:pstyle", "Affilation")
                oReq.AddAttribute(".//cs_text[@type='auth_aff_collections']//AuthorName", "aid:pstyle", "Affilation")

                'add Korrespondenz or correspondence
                Dim corr_Nd As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='auth_aff_collections']")
                If (IsNothing(corr_Nd) = False) Then
                    Dim corHd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                    Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                    attr.Value = "head"
                    corHd.Attributes.Append(attr)
                    If (Article_Lg.ToLower = "de") Then
                        corHd.InnerText = "Korrespondenz: "
                    Else
                        corHd.InnerText = "Correspondence: "
                    End If
                    corr_Nd.InsertBefore(corHd, corr_Nd.FirstChild)
                End If

                'attach corresp after reference or body node
                If (IsNothing(corr_Nd) = False) Then
                    Dim srcNode As Xml.XmlNode = Xdoc.SelectSingleNode(".//Bibliography")
                    If (IsNothing(srcNode) = False) Then
                    Else
                        srcNode = Xdoc.SelectSingleNode(".//Body")
                    End If
                    srcNode.ParentNode.InsertAfter(corr_Nd, srcNode)
                End If
                oReq.AddTextorXml(".//cs_text[@type='auth_aff_collections']", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)


                'createTOC node
                Dim TOCNode As Xml.XmlNode = Xdoc.CreateElement("cs_tocnode")
                Dim nodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//Section1/Heading")
                If (IsNothing(nodes) = False And nodes.Count > 0) Then
                    Dim tchdNd As Xml.XmlNode = Xdoc.CreateElement("cs_tochead")
                    tchdNd.InnerText = "Inhalt" + vbNewLine
                    TOCNode.AppendChild(tchdNd)
                    For i As Integer = 0 To nodes.Count - 1
                        Dim srcattr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_srcid")
                        srcattr.Value = i + 1
                        nodes(i).Attributes.Append(srcattr)

                        Dim destnode As Xml.XmlElement = Xdoc.CreateElement("cs_tc")
                        Dim destattr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_destid")
                        destattr.Value = i + 1
                        destnode.Attributes.Append(destattr)
                        destnode.InnerText = nodes(i).InnerText
                        destnode.InnerXml = destnode.InnerXml + "<cs_text type='RightIndentTab'></cs_text><cs_text type='ctoc'></cs_text>" + vbNewLine
                        TOCNode.AppendChild(destnode)
                    Next
                    Dim rootnd As Xml.XmlNode = Xdoc.SelectSingleNode(".//Body")
                    If (IsNothing(rootnd) = False) Then
                        rootnd.ParentNode.InsertBefore(TOCNode, rootnd)
                    End If
                    oReq.AddAttribute(".//cs_tochead", "aid:pstyle", "TOC_Head")
                    oReq.AddAttribute(".//cs_tc", "aid:pstyle", "TOC_Heading1")
                End If

            End If
            '''''
            If (Article_Lg.ToLower = "en") Then
                oReq.AddAttribute(".//cs_text[@type='Misc']/SimplePara", "aid:pstyle", "Article_Note_En")

            End If
            If (LayoutnameDB.ToLower.Contains("vsandgabler") = True) Then
                oReq.AddAttribute(".//cs_text[@type='vbgabler_historydt']", "aid:pstyle", "DOI_Detail")
                oReq.AddAttribute(".//cs_text[@type='auth_aff_collections']/cs_text[@type='copyright']", "aid:pstyle", "Article_History")
            End If
            oReq.AddAttribute(".//cs_text[@type='superscript']", "aid:cstyle", "Superscript")
            oReq.AddTextorXml(".//cs:footnote[@cs_type='endnote']//cs_text[@type='footnotenumber']", "<cs_text type='vbtab'>" + vbTab + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            oReq.AddAttribute(".//cs:footnote[@cs_type='endnote']//Para", "aid:pstyle", "Endnote")

            Try
                'small utility for export xml
                'Add Cell[cs_type] attribute in its entry node
                Dim CellNodes As Xml.XmlNodeList = Nothing
                CellNodes = Xdoc.SelectNodes(".//Cell")
                If (IsNothing(CellNodes) = False And CellNodes.Count > 0) Then
                    For i As Integer = 0 To CellNodes.Count - 1
                        Dim Nd As Xml.XmlNode = CellNodes(i)

                        If (IsNothing(Nd.Attributes.ItemOf("cs_type")) = False) Then
                            Dim entryNds As Xml.XmlNodeList = Nd.SelectNodes(".//entry")
                            If (IsNothing(entryNds) = False) Then
                                For l As Integer = 0 To entryNds.Count - 1
                                    Dim enNd As Xml.XmlNode = entryNds(l)
                                    Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
                                    attr.Value = Nd.Attributes.ItemOf("cs_type").Value
                                    enNd.Attributes.Append(attr)
                                Next
                            End If
                        End If

                    Next
                End If

            Catch ex As Exception
                CLog.LogMessages("Error in SpecialRoutine()" + vbNewLine)
                CLog.LogMessages(ex.Message.ToString + vbNewLine)
                Throw
            End Try

            oReq.AddTextorXml(".//cs:footnote[position()=1 and @cs_type='footnote']", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsPreviousSibling, True)
            oReq.AddTextorXml(".//cs:footnote[position()=last() and @cs_type='footnote']", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)

            oReq.AddTextorXml(".//cs:footnote[@cs_type='endnote']", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
            Dim HideList1 As New ArrayList
            HideList1.Add(".//Biography//cs_repos")
            oReq.HideNode(HideList1)
            If JournalSubType = "A" Then
                Dim HideList As New ArrayList
                HideList.Add(".//cs_text[@type='auth_aff_collections']/cs_text[@type='sub_auth_aff_collections' and child::cs_text[@type='auth_collections' and child::AuthorName[position()=1 and not(@cs_type='correspond')]]]")
                HideList.Add(".//cs_text[@type='auth_aff_collections']/cs_text[@type='sub_auth_aff_collections']/cs_text[@type='auth_collections']/AuthorName[not(@cs_type='correspond')]")
                oReq.HideNode(HideList)
            End If
            Try
                'Remove last entermarks of auth aff node
                Try
                    Dim GetNode As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='auth_aff_collections']", oReq.NS)
                    If (IsNothing(GetNode) = False) Then
                        Dim Last_ChildNd As Xml.XmlNode = GetNode.LastChild
                        While (Last_ChildNd.Name = "HIDE")
                            Last_ChildNd = Last_ChildNd.PreviousSibling
                        End While

                        Try
                            While (Last_ChildNd.ChildNodes.Count > 0)
                                Try
                                    If Last_ChildNd.InnerText.Trim <> "" Or Last_ChildNd.OuterXml.Contains("<cs_text type=""vbnewline"">") Then
                                        If (AscW(Last_ChildNd.InnerText) = 13) Then
                                            Dim prcs As Boolean = True
                                            If (Last_ChildNd.Attributes.ItemOf("type").Value = "vbgabler_historydt") Then
                                                prcs = False
                                            End If
                                            If (prcs = True) Then
                                                Last_ChildNd.ParentNode.RemoveChild(Last_ChildNd)
                                                Exit While
                                            Else
                                                Exit While
                                            End If

                                        Else
                                            Last_ChildNd = Last_ChildNd.LastChild
                                        End If
                                    Else
                                        Exit While
                                    End If
                                Catch ex As Exception

                                End Try
                            End While
                        Catch ex As Exception

                        End Try
                    End If
                Catch ex As Exception
                    CLog.LogMessages("Error in SpecialRoutine()" + vbNewLine)
                    CLog.LogMessages(ex.Message.ToString + vbNewLine)
                    Throw
                End Try
            Catch ex As Exception
                CLog.LogMessages("Error in SpecialRoutine()" + vbNewLine)
                CLog.LogMessages(ex.Message.ToString + vbNewLine)
                Throw
            End Try

            '
            If (LayoutnameDB.ToLower = "springervienna") Then
                oReq.AddTextorXml(".//Acknowledgments/Heading[not(.='')]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
            Else
                oReq.AddTextorXml(".//Acknowledgments/Heading[not(.='')]", "<cs_text type='emspace'>&#8195;</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
            End If
            oReq.AddAttribute(".//cs_text[@type='endnote']", "aid:pstyle", "Head_1")
            If (LayoutnameDB.ToLower.Contains("large") = False) Then '    If (LayoutnameDB.ToLower = "springervienna" Or LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
            Else
                oReq.AddAttribute(".//cs_text[@type='abstract_keyword']/ArticleSubTitle", "aid:pstyle", "Add_Lang_SubTitle")
                oReq.AddAttribute(".//cs_text[@type='abstract_keyword']/ArticleSubTitle[@Language='De']", "aid:pstyle", "Add_Lang_SubTitle_De")
                oReq.AddAttribute(".//cs_text[@type='abstract_keyword']/ArticleSubTitle[@Language='En']", "aid:pstyle", "Add_Lang_SubTitle_En")
                oReq.AddAttribute(".//cs_text[@type='abstract_keyword']/ArticleSubTitle[@Language='Fr']", "aid:pstyle", "Add_Lang_SubTitle_Fr")
                oReq.AddAttribute(".//cs_text[@type='abstract_keyword']/ArticleSubTitle[@Language='Es']", "aid:pstyle", "Add_Lang_SubTitle_Es")

            End If
            If (LayoutnameDB.ToLower = "vs-verlag" Or LayoutnameDB.ToLower = "vsandgabler") Then
                oReq.AddAttribute("Formalpara_RuninHeading_Italic", "aid:pstyle", ".//FormalPara[@RenderingStyle='Style1']/Para")
                oReq.AddAttribute("Formalpara_RuninHeading_Bold", "aid:pstyle", ".//FormalPara[@RenderingStyle='Style2']/Para")
            End If
            Try
                Dim Nds As Xml.XmlNodeList = Xdoc.SelectNodes(".//FormalPara")
                If (IsNothing(Nds) = False) Then
                    For i As Integer = 0 To Nds.Count - 1
                        Dim Nd As Xml.XmlNode = Nds(i).SelectSingleNode(".//Heading")
                        If (IsNothing(Nd) = False) Then
                            If (Regex.Match(Nd.InnerText.ToLower, "(h\d+|hypothesis|[Tt]hese \d+)[: ]?", RegexOptions.Singleline).Success = True) Then
                                Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
                                attr.Value = "hypothesis"
                                Nd.ParentNode.Attributes.Append(attr)
                                Dim ParaNd As Xml.XmlNodeList = Nds(i).SelectNodes(".//Para")
                                If (IsNothing(ParaNd) = False And ParaNd.Count > 0) Then
                                    For k As Integer = 0 To ParaNd.Count - 1
                                        Dim pnode As Xml.XmlNode = ParaNd(k)
                                        pnode.Attributes.ItemOf("aid:pstyle").Value = "Hypothesis"
                                    Next
                                End If
                                Try
                                    If (Nd.ParentNode.NextSibling.Name.ToLower = "para") Then
                                        If (Nd.ParentNode.NextSibling.Attributes("aid:pstyle").Value = "Body_Text_Indent") Then
                                            Nd.ParentNode.NextSibling.Attributes("aid:pstyle").Value = "Body_Text"
                                        End If
                                    End If
                                Catch ex As Exception

                                End Try

                            End If

                        End If


                    Next
                    Dim Nodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//FormalPara[@cs_type='hypothesis']")
                    If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
                        For i As Integer = 0 To Nodes.Count - 1
                            Dim Nd As Xml.XmlNode = Nodes(i)
                            Dim position As String = ""
                            Try
                                If (IsNothing(Nd.NextSibling) = False And Nd.NextSibling.Name = "FormalPara") Then
                                    If (Nd.NextSibling.Name = "FormalPara" And IsNothing(Nd.NextSibling.Attributes.ItemOf("cs_type")) = False) Then

                                        If (IsNothing(Nd.PreviousSibling) = False) Then
                                            If (Nd.PreviousSibling.Name = "FormalPara" And IsNothing(Nd.PreviousSibling.Attributes.ItemOf("cs_type")) = False) Then
                                                position = "middle"
                                            Else
                                                position = "first"
                                            End If
                                        Else
                                            position = "first"
                                        End If
                                    Else
                                        position = "single"
                                    End If
                                Else
                                    If (IsNothing(Nd.PreviousSibling) = False) Then
                                        If (Nd.PreviousSibling.Name = "FormalPara" And IsNothing(Nd.PreviousSibling.Attributes.ItemOf("cs_type")) = False) Then

                                            If (IsNothing(Nd.NextSibling) = False) Then
                                                If (Nd.NextSibling.Name = "FormalPara" And IsNothing(Nd.NextSibling.Attributes.ItemOf("cs_type")) = False) Then
                                                    position = "middle"
                                                Else
                                                    position = "last"
                                                End If
                                            Else
                                                position = "last"
                                            End If
                                        End If
                                    Else
                                        position = "single"
                                    End If

                                End If

                            Catch ex As Exception
                                position = "single"
                            End Try
                            If (IsNothing(Nd.Attributes.ItemOf("cs_position")) = False) Then
                                Nd.Attributes.ItemOf("cs_position").Value = position
                            Else
                                Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_position")
                                atr.Value = position
                                Nd.Attributes.Append(atr)
                            End If

                        Next
                    End If
                    oReq.AddTextorXml(".//FormalPara[@cs_type='hypothesis']/Heading/following-sibling::cs_text[@type='enspace']", "<cs_text type='indentohere'></cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
                    oReq.AddAttribute(".//FormalPara[@cs_type='hypothesis' and @cs_position='first']/Para", "aid:pstyle", "Hypothesis_First")
                    oReq.AddAttribute(".//FormalPara[@cs_type='hypothesis' and @cs_position='middle']/Para", "aid:pstyle", "Hypothesis_Middle")
                    oReq.AddAttribute(".//FormalPara[@cs_type='hypothesis' and @cs_position='last']/Para", "aid:pstyle", "Hypothesis_Last")
                    oReq.AddAttribute(".//FormalPara[@cs_type='hypothesis' and @cs_position='single']/Para", "aid:pstyle", "Hypothesis")
                End If
            Catch ex As Exception
                CLog.LogMessages("Error in SpecialRoutine()" + vbNewLine)
                CLog.LogMessages(ex.Message.ToString + vbNewLine)
                Throw
            End Try
            If (LayoutnameDB.ToLower = "vs-verlag" Or Infilemame.Trim = "501" Or LayoutnameDB.ToLower = "vsandgabler") Then
                oReq.AddTextorXml(".//cs_text[@type='abstract_keyword']/Abstract[not(child::AbstractSection)]/Heading[not(.='')]", "<cs_text type='colon'>:</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
                oReq.AddTextorXml(".//cs_text[@type='abstract_keyword']/KeywordGroup/Heading[not(.='')]", "<cs_text type='colon'>:</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
            End If
            If (LayoutnameDB = "GlobalJournalLarge") Then ' If (Infilemame.Trim = "62" Or Infilemame.Trim = "16024") Then
                oReq.AddAttribute(".//Abstract[@Language='En']/AbstractSection[position()>1]/Para", "aid:pstyle", "Abstract_Structured_Text_En_NoIndent")
                oReq.AddAttribute(".//Abstract[@Language='De']/AbstractSection[position()>1]/Para", "aid:pstyle", "Abstract_Structured_Text_De_NoIndent")
                oReq.AddAttribute(".//Abstract/AbstractSection[position()>1 and (@OutputMedium='All')]/Para", "aid:pstyle", "Abstract_GA")
            End If
            If (LayoutnameDB.ToLower = "vs-verlag" Or LayoutnameDB.ToLower = "vsandgabler") Then
                oReq.AddTextorXml(".//Table//CaptionNumber[not(.='')]", "<cs_text type='enspace'>&#8194;</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
                oReq.AddTextorXml(".//Table//CaptionNumber[not(.='')]", "<cs_text type='colon'>:</cs_text>", clsPreprocMain.ChildTypes.AsLastChild, True)
                oReq.AddTextorXml(".//Figure//CaptionNumber[not(.='')]", "<cs_text type='colon'>:</cs_text>", clsPreprocMain.ChildTypes.AsLastChild, True)
            Else
                If (JournalSubType = "A" And Infilemame <> "11812") Then
                    oReq.AddTextorXml(".//Table//CaptionNumber[not(.='')]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
                    oReq.AddTextorXml(".//Table//CaptionNumber[not(.='')]", "<cs_text type='colon'>:</cs_text>", clsPreprocMain.ChildTypes.AsLastChild, True)
                    oReq.AddTextorXml(".//Figure//CaptionNumber[not(.='')]", "<cs_text type='colon'>:</cs_text>", clsPreprocMain.ChildTypes.AsLastChild, True)
                    oReq.AddAttribute(".//Table//CaptionNumber[not(.='')]", "aid:pstyle", "Table_Number")
                Else
                    oReq.AddTextorXml(".//Table//CaptionNumber[not(.='')]", "<cs_text type='enspace'>&#8194;</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling, True)
                End If
            End If
            If (LayoutnameDB.ToLower <> "springervienna") Then
                oReq.AddAttribute(".//cs_text[@type='doiframe']/cs_text[@type='secondlinedetail']", "aid:pstyle", "DOI_Detail_Rule_Below")
            End If
            If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                oReq.AddAttribute(".//cs_text[@type='doiframe']/cs_text[@type='firstlinedetail']", "aid:cstyle", "Italic")
                oReq.AddAttribute(".//cs_text[@type='doiframe']/cs_text[@type='firstlinedetail']", "aid:pstyle", "DOI_Detail_Recto")
                oReq.AddAttribute(".//cs_text[@type='doiframe']/cs_text[@type='secondlinedetail']", "aid:pstyle", "DOI_Detail_Recto")
                oReq.AddAttribute(".//cs_text[@type='doiframe']/cs_text[@type='thirdlinedetail']", "aid:pstyle", "DOI_Detail_Recto")

                'oReq.AddAttribute(".//Biography/FormalPara/Para", "aid:pstyle", "Biography_Entry2")
                'oReq.AddAttribute(".//Biography[1]/FormalPara/Para", "aid:pstyle", "Biography_Entry1")
                'oReq.AddAttribute(".//Biography/FormalPara/Para[position()>1]", "aid:pstyle", "Biography_Para")

            End If
            'Abstarct_keyword
            If (JournalSubType = "B") Then
                oReq.AddTextorXml(".//cs_text[@type='abstract_keyword']//KeywordGroup/Keyword[not(position()=last())]", "<cs_text type='nonbreakingspace'>&#160;</cs_text><cs_text type='middot'>&#183;</cs_text><cs_text type='space'> </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
                oReq.DeleteNode(".//cs_text[@type='authname']/FamilyName[not(position()=last())]/following-sibling::cs_text[@type='comma' and position()=1]")
                oReq.DeleteNode(".//cs_text[@type='authname']/FamilyName[not(position()=last())]/following-sibling::cs_text[@type='space' and position()=1]")
                oReq.AddTextorXml(".//cs_text[@type='authname']/FamilyName[not(position()=last())]", "<cs_text type='nonbreakingspace'>&#160;</cs_text><cs_text type='middot'>&#183;</cs_text><cs_text type='space'> </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            Else
                oReq.AddTextorXml(".//cs_text[@type='abstract_keyword']//KeywordGroup/Keyword[not(position()=last())]", "<cs_text type='commaspace'>, </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            End If
            If (JournalSubType = "A") Then
                oReq.DeleteNode(".//cs_text[@type='authname']/GivenName[position()=last()]/preceding-sibling::cs_text[@type='space' and position()=1]")
                oReq.DeleteNode(".//cs_text[@type='authname']/GivenName[position()=last()]/preceding-sibling::cs_text[@type='comma' and position()=1]")
                If (Article_Lg.ToLower = "de") Then
                    oReq.AddTextorXml(".//cs_text[@type='authname']/GivenName[position()=last() and not(position()=1)]", "<cs_text type='and'> und </cs_text>", clsPreprocMain.ChildTypes.AsPreviousSibling, True)
                Else
                    oReq.AddTextorXml(".//cs_text[@type='authname']/GivenName[position()=last() and not(position()=1)]", "<cs_text type='and'> and </cs_text>", clsPreprocMain.ChildTypes.AsPreviousSibling, True)
                End If
            End If
            If (Article_Lg.ToLower = "de") Then
                oReq.AddTextorXml(".//Email[not(.='') and (not(preceding-sibling::Email))]", "<cs_text type='email'>E-Mail: </cs_text>", clsPreprocMain.ChildTypes.AsFirstChild)
            End If
            If (Article_Lg.ToLower = "en") Then
                oReq.AddTextorXml(".//Email[not(.='') and (not(preceding-sibling::Email))]", "<cs_text type='email'>e-mail: </cs_text>", clsPreprocMain.ChildTypes.AsFirstChild)
            End If


        Catch ex As Exception
            CLog.LogMessages("Error in SpecialRoutine()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function CreateNodeElement(ByVal insertionelement As String) As String
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:CreateNodeElement
        'PARAMETER    :insertionelement
        'AIM          :This function add the given element node at given position.
        '=============================================================================================================
        '=============================================================================================================
        Dim returnNodestring As String = ""
        Dim arr() As String = insertionelement.Split("_")
        Try
            If (arr.Length > 0) Then
                For i As Integer = 0 To arr.Length - 1
                    Dim val As String = arr(i)
                    If (val.ToLower = "vbnewline") Then
                        returnNodestring += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>"
                    End If

                    If (val.ToLower = "nonbreakingspace") Then
                        returnNodestring += "<cs_text type='nonbreakingspace'>&#160;</cs_text>"
                    End If

                    If (val.ToLower = "semicolon") Then
                        returnNodestring += "<cs_text type='semicolon'>;</cs_text>"
                    End If

                    If (val.ToLower = "space") Then
                        returnNodestring += "<cs_text type='space'> </cs_text>"
                    End If

                    If (val.ToLower = "comma") Then
                        returnNodestring += "<cs_text type='comma'>,</cs_text>"
                    End If

                    If (val.ToLower = "ndash") Then
                        returnNodestring += "<cs_text type='ndash'>&#8211;</cs_text>"
                    End If

                    If (val.ToLower = "colon") Then
                        returnNodestring += "<cs_text type='colon'>:</cs_text>"
                    End If

                    If (val.ToLower = "enspace") Then
                        returnNodestring += "<cs_text type='enspace'>&#8194;</cs_text>"
                    End If
                    If (val.ToLower = "emspace") Then
                        returnNodestring += "<cs_text type='emspace'>&#8195;</cs_text>"
                    End If

                    If (val.ToLower = "vbtab") Then
                        returnNodestring += "<cs_text type='vbtab'>" + vbTab + "</cs_text>"
                    End If

                    If (val.ToLower = "indentohere") Then
                        returnNodestring += "<cs_text type='indentohere'></cs_text>"
                    End If

                    If (val.ToLower = "rightindenttab") Then
                        returnNodestring += "<cs_text type='RightIndentTab'></cs_text>"
                    End If

                    If (val.ToLower = "openingbracket") Then
                        returnNodestring += "<cs_text type='openingbracket'>(</cs_text>"
                    End If

                    If (val.ToLower = "closingbracket") Then
                        returnNodestring += "<cs_text type='closingbracket'>)</cs_text>"
                    End If

                    If (val.ToLower = "envelop") Then
                        returnNodestring += "<cs_text type=""envelop"">&#61482;</cs_text>"
                    End If

                    If (val.ToLower = "middot") Then
                        returnNodestring += "<cs_text type='middot'>&#183;</cs_text>"
                    End If

                    If (val.ToLower = "tel") Then
                        returnNodestring += "<cs_text type='tel'>Tel.: </cs_text>"

                    End If
                    If (val.ToLower = "fax") Then
                        returnNodestring += "<cs_text type='fax'>Fax: </cs_text>"
                    End If


                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in CreateNodeElement()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        Return returnNodestring
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Sub PlaceEnterAbstract()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:PlaceEnterAbstract
        'PARAMETER    :-
        'AIM          :This function add entermark, enspace, vbtab..etc
        '=============================================================================================================
        '=============================================================================================================
        Try
            'Para
            oReq.AddTextorXml(".//Para", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)

            'Section heading
            oReq.AddTextorXml(".//Section3/Heading[not(.='')]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            oReq.AddTextorXml(".//Section2/Heading[not(.='')]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            oReq.AddTextorXml(".//Section1/Heading[not(.='')]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            oReq.AddTextorXml(".//FormalPara", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            oReq.AddTextorXml(".//FormalPara/Heading", "<cs_text type='enspace'>&#8194;</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)

            'Journal title frame
            oReq.AddTextorXml(".//cs_text[@type='journaltitle']/ArticleTitle", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            oReq.AddTextorXml(".//cs_text[@type='journaltitle']/ArticleSubTitle[following-sibling::*]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            oReq.AddTextorXml(".//cs_text[@type='journaltitle']/cs_text[@type='authname' and following-sibling::*]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            oReq.AddTextorXml(".//cs_text[@type='authname']/GivenName", "<cs_text type='nonbreakingspace'>&#160;</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)
            oReq.AddTextorXml(".//cs_text[@type='authname']/FamilyName[not(position()=last())]", "<cs_text type='commaspace'>, </cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)

            'Table
            oReq.AddTextorXml(".//Table//CaptionNumber[not(.='')]", "<cs_text type='enspace'>&#8194;</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)

            'Figure
            oReq.AddTextorXml(".//Figure//CaptionNumber", "<cs_text type='enspace'>&#8194;</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)


            'Footnote
            oReq.AddTextorXml(".//Footnote/Para[not(position()=last())]", "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>", clsPreprocMain.ChildTypes.AsNextSibling)

        Catch ex As Exception
            CLog.LogMessages("Error in PlaceEnterAbstract()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub PlaceEnter()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:PlaceEnter
        'PARAMETER    :-
        'AIM          :This function add entermark, enspace, vbtab..etc
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim myxdoc As New Xml.XmlDocument
            myxdoc.LoadXml(Locationxmlpath)
            Dim PlaceEnterNd As Xml.XmlNode = myxdoc.SelectSingleNode(".//PlaceEnter")
            If (IsNothing(PlaceEnterNd) = False) Then
                Dim nodes As Xml.XmlNodeList = PlaceEnterNd.SelectNodes(".//node")
                If (IsNothing(nodes) = False And nodes.Count > 0) Then
                    For i As Integer = 0 To nodes.Count - 1
                        Dim Nd As Xml.XmlNode = nodes(i)
                        Dim InsertionString As String = ""
                        InsertionString = CreateNodeElement(Nd.Attributes.ItemOf("Insertionnode").Value)
                        If (Nd.Attributes.ItemOf("position").Value.ToLower = "after") Then
                            oReq.AddTextorXml(Nd.InnerText, InsertionString, clsPreprocMain.ChildTypes.AsNextSibling, Nd.Attributes.ItemOf("override").Value)
                        End If
                        If (Nd.Attributes.ItemOf("position").Value.ToLower = "before") Then
                            oReq.AddTextorXml(Nd.InnerText, InsertionString, clsPreprocMain.ChildTypes.AsPreviousSibling, Nd.Attributes.ItemOf("override").Value)
                        End If
                        If (Nd.Attributes.ItemOf("position").Value.ToLower = "first") Then
                            oReq.AddTextorXml(Nd.InnerText, InsertionString, clsPreprocMain.ChildTypes.AsFirstChild, Nd.Attributes.ItemOf("override").Value)
                        End If
                        If (Nd.Attributes.ItemOf("position").Value.ToLower = "last") Then
                            oReq.AddTextorXml(Nd.InnerText, InsertionString, clsPreprocMain.ChildTypes.AsLastChild, Nd.Attributes.ItemOf("override").Value)
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in PlaceEnter()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Private Function Fn_AddLangToBSL_Motto()

        Try
            Dim Art_Lang As String = Xdoc.SelectSingleNode(".//ArticleInfo/@Language").InnerText
            If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                Dim xnodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote[@Type='Motto']")
                If (IsNothing(xnodes) = False And xnodes.Count > 0) Then
                    For i As Integer = 0 To xnodes.Count - 1
                        Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_language")

                        If (Art_Lang.ToLower = "de") Then
                            attr.Value = "De"
                        End If
                        If (Art_Lang.ToLower = "en") Then
                            attr.Value = "En"
                        End If
                        If (Art_Lang.ToLower = "nl") Then
                            attr.Value = "Nl"
                        End If
                        xnodes(i).Attributes.Append(attr)
                    Next
                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddLangToBSL_Motto()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try

    End Function
    Private Function Fn_EndInlineEquation()
        Try

            Dim Nodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//InlineEquation")
            If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
                For i As Integer = 0 To Nodes.Count - 1
                    Dim entermark As Boolean = False
                    Dim InlineNode As Xml.XmlNode = Nodes(i)
                    If (IsNothing(InlineNode.ParentNode.NextSibling) = True) Then
                        If (IsNothing(InlineNode.NextSibling) = True) Then
                            Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_inlineEnter")
                            attr.Value = "true"
                            InlineNode.ParentNode.Attributes.Append(attr)
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_EndInlineEquation()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Private Function Fn_MakeCityStateCountry()
        Dim Nodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorGroup//City|.//AuthorGroup//State|.//AuthorGroup//Country")
        If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
            For m As Integer = 0 To Nodes.Count - 1
                Dim Nd As Xml.XmlNode = Nodes(m)
                Nd.InnerXml = Nd.InnerXml.Replace(" ", "&#160;")
            Next
        End If
    End Function
    Private Function Fn_ListMaking()
        '28 combination
        '1....OderedList
        '2....UnOrderedList

        'add aatr cs_level==first second third four
        '1/1
        Dim Nodes_1 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/OrderedList")
        If (IsNothing(Nodes_1) = False And Nodes_1.Count > 0) Then
            For i As Integer = 0 To Nodes_1.Count - 1
                Dim Nd As Xml.XmlNode = Nodes_1(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "second"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "second"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '1/2
        Dim Node_2 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_2) = False And Node_2.Count > 0) Then
            For i As Integer = 0 To Node_2.Count - 1
                Dim Nd As Xml.XmlNode = Node_2(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "second"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "second"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/1
        Dim Node_3 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/OrderedList")
        If (IsNothing(Node_3) = False And Node_3.Count > 0) Then
            For i As Integer = 0 To Node_3.Count - 1
                Dim Nd As Xml.XmlNode = Node_3(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "second"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "second"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/2
        Dim Node_4 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_4) = False And Node_4.Count > 0) Then
            For i As Integer = 0 To Node_4.Count - 1
                Dim Nd As Xml.XmlNode = Node_4(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "second"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "second"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If
        '1/1/1
        Dim Node_5 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList")
        If (IsNothing(Node_5) = False And Node_5.Count > 0) Then
            For i As Integer = 0 To Node_5.Count - 1
                Dim Nd As Xml.XmlNode = Node_5(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "third"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "third"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If
        '1/1/2
        Dim Node_6 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_6) = False And Node_6.Count > 0) Then
            For i As Integer = 0 To Node_6.Count - 1
                Dim Nd As Xml.XmlNode = Node_6(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "third"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "third"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If
        '1/2/1
        Dim Node_7 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList")
        If (IsNothing(Node_7) = False And Node_7.Count > 0) Then
            For i As Integer = 0 To Node_7.Count - 1
                Dim Nd As Xml.XmlNode = Node_7(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "third"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "third"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If
        '1/2/2
        Dim Node_8 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_8) = False And Node_8.Count > 0) Then
            For i As Integer = 0 To Node_8.Count - 1
                Dim Nd As Xml.XmlNode = Node_8(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "third"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "third"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If


        '2/1/1
        Dim Node_9 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList")
        If (IsNothing(Node_9) = False And Node_9.Count > 0) Then
            For i As Integer = 0 To Node_9.Count - 1
                Dim Nd As Xml.XmlNode = Node_9(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "third"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "third"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/1/2
        Dim Node_10 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_10) = False And Node_10.Count > 0) Then
            For i As Integer = 0 To Node_10.Count - 1
                Dim Nd As Xml.XmlNode = Node_10(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "third"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "third"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/2/1
        Dim Node_11 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList")
        If (IsNothing(Node_11) = False And Node_11.Count > 0) Then
            For i As Integer = 0 To Node_11.Count - 1
                Dim Nd As Xml.XmlNode = Node_11(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "third"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "third"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/2/2
        Dim Node_12 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_12) = False And Node_12.Count > 0) Then
            For i As Integer = 0 To Node_12.Count - 1
                Dim Nd As Xml.XmlNode = Node_12(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "third"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "third"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '1/1/1/1
        Dim Node_13 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList")
        If (IsNothing(Node_13) = False And Node_13.Count > 0) Then
            For i As Integer = 0 To Node_13.Count - 1
                Dim Nd As Xml.XmlNode = Node_13(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If


        '1/1/1/2
        Dim Node_14 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_14) = False And Node_14.Count > 0) Then
            For i As Integer = 0 To Node_14.Count - 1
                Dim Nd As Xml.XmlNode = Node_14(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If


        '1/1/2/1
        Dim Node_15 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList")
        If (IsNothing(Node_15) = False And Node_15.Count > 0) Then
            For i As Integer = 0 To Node_15.Count - 1
                Dim Nd As Xml.XmlNode = Node_15(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If



        '1/1/2/2
        Dim Node_16 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_16) = False And Node_16.Count > 0) Then
            For i As Integer = 0 To Node_16.Count - 1
                Dim Nd As Xml.XmlNode = Node_16(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If



        '1/2/1/1
        Dim Node_17 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList")
        If (IsNothing(Node_17) = False And Node_17.Count > 0) Then
            For i As Integer = 0 To Node_17.Count - 1
                Dim Nd As Xml.XmlNode = Node_17(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If


        '1/2/1/2
        Dim Node_18 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_18) = False And Node_18.Count > 0) Then
            For i As Integer = 0 To Node_18.Count - 1
                Dim Nd As Xml.XmlNode = Node_18(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If


        '1/2/2/1
        Dim Node_19 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList")
        If (IsNothing(Node_19) = False And Node_19.Count > 0) Then
            For i As Integer = 0 To Node_19.Count - 1
                Dim Nd As Xml.XmlNode = Node_19(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '1/2/2/2
        Dim Node_20 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_20) = False And Node_20.Count > 0) Then
            For i As Integer = 0 To Node_20.Count - 1
                Dim Nd As Xml.XmlNode = Node_20(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/1/1/1
        Dim Node_21 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList")
        If (IsNothing(Node_21) = False And Node_21.Count > 0) Then
            For i As Integer = 0 To Node_21.Count - 1
                Dim Nd As Xml.XmlNode = Node_21(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If


        '2/1/1/2
        Dim Node_22 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_22) = False And Node_22.Count > 0) Then
            For i As Integer = 0 To Node_22.Count - 1
                Dim Nd As Xml.XmlNode = Node_22(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/1/2/1
        Dim Node_23 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList")
        If (IsNothing(Node_23) = False And Node_23.Count > 0) Then
            For i As Integer = 0 To Node_23.Count - 1
                Dim Nd As Xml.XmlNode = Node_23(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If


        '2/1/2/2
        Dim Node_24 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_24) = False And Node_24.Count > 0) Then
            For i As Integer = 0 To Node_24.Count - 1
                Dim Nd As Xml.XmlNode = Node_24(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/2/1/1
        Dim Node_25 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/OrderedList")
        If (IsNothing(Node_25) = False And Node_25.Count > 0) Then
            For i As Integer = 0 To Node_25.Count - 1
                Dim Nd As Xml.XmlNode = Node_25(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/2/1/2
        Dim Node_26 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList/ListItem/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_26) = False And Node_26.Count > 0) Then
            For i As Integer = 0 To Node_26.Count - 1
                Dim Nd As Xml.XmlNode = Node_26(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        '2/2/2/1
        Dim Node_27 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/OrderedList")
        If (IsNothing(Node_27) = False And Node_27.Count > 0) Then
            For i As Integer = 0 To Node_27.Count - 1
                Dim Nd As Xml.XmlNode = Node_27(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If
        '2/2/2/2
        Dim Node_28 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList/ItemContent/Para/UnorderedList")
        If (IsNothing(Node_28) = False And Node_28.Count > 0) Then
            For i As Integer = 0 To Node_28.Count - 1
                Dim Nd As Xml.XmlNode = Node_28(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "four"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "four"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If

        'Not having attr orderedlist
        Dim Node_29 As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList[not(@cs_level)]")
        If (IsNothing(Node_29) = False And Node_29.Count > 0) Then
            For i As Integer = 0 To Node_29.Count - 1
                Dim Nd As Xml.XmlNode = Node_29(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "first"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "first"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If
        'Not having attr unorderedlist
        Dim Node_30 As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList[not(@cs_level)]")
        If (IsNothing(Node_30) = False And Node_30.Count > 0) Then
            For i As Integer = 0 To Node_30.Count - 1
                Dim Nd As Xml.XmlNode = Node_30(i)
                Dim attr As Xml.XmlAttribute = Nothing
                If (IsNothing(Nd.Attributes.ItemOf("cs_level")) = False) Then
                    attr = Nd.Attributes.ItemOf("cs_level")
                    attr.Value = "first"
                Else
                    attr = Xdoc.CreateAttribute("cs_level")
                    attr.Value = "first"
                    Nd.Attributes.Append(attr)
                End If
            Next
        End If
    End Function
    Private Function Fn_TableList()
        Dim TableNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//Table")
        If ((IsNothing(TableNd) = False) And TableNd.Count > 0) Then
            For i As Integer = 0 To TableNd.Count - 1 '"<cs_text type='nonbreakingspace'>&#160;</cs_text>"
                'TableNd(i).InnerXml = TableNd(i).InnerXml.Replace("<SimplePara>   ", "<SimplePara>" + "<cs_text type='tbllistnonbreakingspace'>&#160;&#160;&#160;&#160;&#160;&#160;</cs_text>")
                'TableNd(i).InnerXml = TableNd(i).InnerXml.Replace("<SimplePara>  ", "<SimplePara>" + "<cs_text type='tbllistnonbreakingspace'>&#160;&#160;&#160;&#160;</cs_text>")
                'TableNd(i).InnerXml = TableNd(i).InnerXml.Replace("<SimplePara> ", "<SimplePara>" + "<cs_text type='tbllistnonbreakingspace'>&#160;&#160;</cs_text>")

                TableNd(i).InnerXml = TableNd(i).InnerXml.Replace("<SimplePara>   ", "<SimplePara cs_tbllisttype='thirdlevel'>")
                TableNd(i).InnerXml = TableNd(i).InnerXml.Replace("<SimplePara>  ", "<SimplePara cs_tbllisttype='secondlevel'>")
                TableNd(i).InnerXml = TableNd(i).InnerXml.Replace("<SimplePara> ", "<SimplePara cs_tbllisttype='firstlevel'>")

            Next
        End If
    End Function
    Private Function Fn_CharAlign()
        Try
            Dim colspecNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//colspec[@char]")
            If ((IsNothing(colspecNds) = False) And colspecNds.Count > 0) Then
                For i As Integer = 0 To colspecNds.Count - 1
                    Dim atrname As String = ""
                    If (IsNothing(colspecNds(i).Attributes.ItemOf("colname")) = False) Then
                        atrname = colspecNds(i).Attributes.ItemOf("colname").Value
                        Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_charalign_" + atrname)
                        If (IsNothing(colspecNds(i).Attributes.ItemOf("char")) = False) Then
                            attr.Value = colspecNds(i).Attributes.ItemOf("char").Value
                            If (colspecNds(i).ParentNode.ParentNode.Name.ToLower = "table") Then
                                colspecNds(i).ParentNode.ParentNode.Attributes.Append(attr)
                            End If

                        End If
                    End If

                Next
            End If

        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CharAlign()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Private Function Fn_ParenthesisToEquation()
        Try
            Dim DisplEeNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Equation//EquationNumber")
            If ((IsNothing(DisplEeNds) = False) And DisplEeNds.Count > 0) Then
                For i As Integer = 0 To DisplEeNds.Count - 1
                    DisplEeNds(i).InnerXml = "(" + DisplEeNds(i).InnerXml + ")"
                Next
            End If

        Catch ex As Exception
            CLog.LogMessages("Error in Fn_ParenthesisToEquation()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Private Function Fn_AddDOIToArticle()
        Try
            Dim ArticleNode As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article")
            If (IsNothing(ArticleNode) = False) Then
                Dim DOInode As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleDOI")
                If (IsNothing(DOInode) = False) Then
                    Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_doi")
                    attr.Value = DOInode.InnerXml
                    ArticleNode.Attributes.Append(attr)
                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddDOIToArticle()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Private Function Fn_CitationNumberdot()
        Dim CitationNumberNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Bibliography//CitationNumber")
        If (IsNothing(CitationNumberNds) = False And CitationNumberNds.Count > 0) Then
            For i As Integer = 0 To CitationNumberNds.Count - 1
                Dim Nd As Xml.XmlNode = CitationNumberNds(i)
                Nd.InnerXml = Nd.InnerXml + "."
            Next
        End If
    End Function
    Private Function Fn_CheckAffID()
        Try
            Dim Nodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//Author[ancestor::AuthorGroup]|.//InstitutionalAuthor[ancestor::AuthorGroup]") ' Xdoc.SelectNodes(".//Author|.//InstitutionalAuthorName")
            If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
                For i As Integer = 0 To Nodes.Count - 1
                    Dim Nd As Xml.XmlNode = Nodes(i)
                    Dim str As String = Nd.Attributes.ItemOf("AffiliationIDS").Value
                Next
            End If
        Catch ex As Exception
            'CLog.LogMessages("Error in Fn_CheckAffID()" + vbNewLine)
            CLog.LogMessages("Affiliation ID is missing for Author or InstitutionalAuthor" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Private Function Fn_AuthorAffiliationClass()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AuthorAffiliationClass
        'PARAMETER    :-
        'AIM          :This function call author affiliation creation class.
        '=============================================================================================================
        '=============================================================================================================
        Try
            'If (Article_Lg.ToLower = "nl") Then
            '    Dim csnodeAuthAff As New classAuthoraffiliation_Creation(Xdoc, Article_Lg)
            'Else
            '    Dim csnodeAuthAff As New classAuthoraffiliation_Creation_STM(Xdoc, Article_Lg)
            'End If
            'If (LayoutnameDB.ToLower = "springervienna") Then
            '    Dim csnodeAuthAff As New classAuthoraffiliation_Creation(Xdoc, Article_Lg)
            'Else
            '    Dim csnodeAuthAff As New classAuthoraffiliation_Creation_STM(Xdoc, Article_Lg)
            'End If
            Dim csnodeAuthAff As New classAuthoraffiliation_Creation_STM(Xdoc, Article_Lg)
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AuthorAffiliationClass()" + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_Addcs_timeidintblfig()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_Addcs_timeidintblfig
        'PARAMETER    :-
        'AIM          :This function add cs_timeid attribute to all table and figure except biography figure.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim TabFigNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Figure[not(ancestor::Biography)]|.//Table")
            If (IsNothing(TabFigNds) = False And TabFigNds.Count > 0) Then
                For i As Integer = 0 To TabFigNds.Count - 1
                    Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_timeid")
                    Dim intval As Integer = i + 1
                    attr.Value = intval
                    TabFigNds(i).Attributes.Append(attr)
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_Addcs_timeidintblfig()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_AddFootnote_cs_type_attr()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddFootnote_cs_type_attr
        'PARAMETER    :-
        'AIM          :This function add "cs_type" attribute to footnote to differentiate style.
        '=============================================================================================================
        '=============================================================================================================
        Try
            'if footnote greater than 10 then for style identification below node added
            Dim Nodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//footnote")
            If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
                Dim atrval As String = ""
                If (Nodes.Count >= 10) Then
                    atrval = "double"
                Else
                    atrval = "single"
                End If
                For r As Integer = 0 To Nodes.Count - 1
                    Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
                    atr.Value = atrval
                    Nodes(r).Attributes.Append(atr)
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddFootnote_cs_type_attr()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_DisplaySymbol_Superscript()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_DisplaySymbol_Superscript
        'PARAMETER    :-
        'AIM          :This function do addition, movement, modification of node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            If (Infilemame.Trim <> "501" Or Infilemame.Trim <> "506") Then
                'display &#x00AE; in superscript[Registed symbol]
                XDocString = Xdoc.InnerXml
                XDocString = Regex.Replace(XDocString, "®", "<cs_text type='superscript'>®</cs_text>", RegexOptions.Singleline)
                XDocString = Regex.Replace(XDocString, "™", "<cs_text type='superscript'>™</cs_text>", RegexOptions.Singleline)
                Xdoc.InnerXml = XDocString
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_DisplaySymbol_Superscript()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_AddAttrtoElement()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddAttrtoElement
        'PARAMETER    :-
        'AIM          :This function add language attribute to Definitionlist.
        '=============================================================================================================
        '=============================================================================================================
        Try
            'Add language attribute in definitionlist
            XDocString = Xdoc.InnerXml
            XDocString = Regex.Replace(XDocString, "&#x00AE;", "<Superscript> &#x00AE;</Superscript>", RegexOptions.Singleline)
            XDocString = Regex.Replace(XDocString, "<Para><DefinitionList>", "<DefinitionList>", RegexOptions.Singleline)
            XDocString = Regex.Replace(XDocString, "</DefinitionList></Para>", "</DefinitionList>", RegexOptions.Singleline)
            XDocString = Regex.Replace(XDocString, "<DefinitionList><Heading>Abkürzungen</Heading>", "<DefinitionList Language='De'><Heading>Abkürzungen</Heading>", RegexOptions.Singleline)
            XDocString = Regex.Replace(XDocString, "<DefinitionList><Heading>Abbreviations</Heading>", "<DefinitionList Language='En'><Heading>Abbreviations</Heading>", RegexOptions.Singleline)

            Xdoc.InnerXml = XDocString
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddAttrtoElement()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try

        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_AddMspace_toCitationnumber()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddMspace_toCitationnumber
        'PARAMETER    :-
        'AIM          :This function add enspace attribute before citation number.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim CitationNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Citation/CitationNumber")
            If (IsNothing(CitationNds) = False And CitationNds.Count > 0) Then
                If (CitationNds.Count > 9) Then
                    For i As Integer = 0 To 8
                        CitationNds(i).InnerXml = "<cs_text type='enspace'>&#8194;</cs_text>" + CitationNds(i).InnerXml
                    Next
                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddMspace_toCitationnumber()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_CreateTableList()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CreateTableList
        'PARAMETER    :-
        'AIM          :This function create table list.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim TableListNode As Xml.XmlNodeList = Xdoc.SelectNodes(".//Table//SimplePara[not(.='')]")
            If (IsNothing(TableListNode) = False And TableListNode.Count > 0) Then
                For i As Integer = 0 To TableListNode.Count - 1
                    Dim InnerNode As Xml.XmlNode = TableListNode(i)
                    'Hyphen
                    If (Regex.Match(InnerNode.OuterXml, "<SimplePara>– ", RegexOptions.Singleline).Success = True) Then
                        If (InnerNode.InnerText(0) = "–") Then
                            Dim s As Regex
                            s = New Regex("(–) ", RegexOptions.Singleline)
                            InnerNode.InnerXml = s.Replace(InnerNode.InnerXml, "<cs_text type='hyphenlist'>" + "$1" + "<cs_text type='indentohere'></cs_text></cs_text>", 1)
                            Dim OUAttr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblhyphen")
                            OUAttr.Value = "HypLevel1"
                            InnerNode.Attributes.Append(OUAttr)
                        End If
                    End If
                    'Bullet
                    If (Regex.Match(InnerNode.OuterXml, "<SimplePara>•(\s)", RegexOptions.Singleline).Success = True) Then
                        If (InnerNode.InnerText(0) = "•") Then
                            Dim s As Regex
                            s = New Regex("(•)(\s|<)", RegexOptions.Singleline)
                            InnerNode.InnerXml = s.Replace(InnerNode.InnerXml, "<cs_text type='bulletlist'>" + "$1" + "<cs_text type='indentohere'></cs_text></cs_text>", 1)
                            Dim OUAttr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tblbullet")
                            OUAttr.Value = "BulLevel1"
                            InnerNode.Attributes.Append(OUAttr)
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CreateTableList()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_ConvertTableAsFigure()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_ConvertTableAsFigure
        'PARAMETER    :-
        'AIM          :This function convert table as image.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim TableNd_MdObjNode As Xml.XmlNodeList = Xdoc.SelectNodes(".//Table[child::MediaObject and not(descendant::entry)]")
            If (IsNothing(TableNd_MdObjNode) = False And TableNd_MdObjNode.Count > 0) Then
                For i As Integer = 0 To TableNd_MdObjNode.Count - 1
                    Dim TableFigStr As String = TableNd_MdObjNode(i).ParentNode.InnerXml
                    TableFigStr = Regex.Replace(TableFigStr, "<Table", "<Figure cs_tblfig='TabToFig' cs_tblmethod='ImageLeftBot_CapTop'")
                    TableFigStr = Regex.Replace(TableFigStr, "Table>", "Figure>")
                    TableNd_MdObjNode(i).ParentNode.InnerXml = TableFigStr
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_ConvertTableAsFigure()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_AddLangattrToCitaion()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddLangattrToCitaion
        'PARAMETER    :-
        'AIM          :This function add language attribute to bibliography citation element.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim Art_Lang As String = Xdoc.SelectSingleNode(".//ArticleInfo/@Language").InnerText
            'Citation greater than 100-identification attribute
            Dim CTND As Xml.XmlNodeList = Xdoc.SelectNodes(".//Bibliography//Citation")
            If (IsNothing(CTND) = False And CTND.Count > 0) Then
                For k As Integer = 0 To CTND.Count - 1
                    Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_Lang")
                    If (IsNothing(CTND(k).SelectSingleNode(".//*/@Language")) = False) Then
                        attr.Value = CTND(k).SelectSingleNode(".//*/@Language").Value
                    Else
                        attr.Value = Art_Lang
                    End If
                    CTND(k).Attributes.Append(attr)
                Next

            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddLangattrToCitaion()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_AddattrToCitaion()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddattrToCitaion
        'PARAMETER    :-
        'AIM          :This function add attribute to bibliography citation element.
        '=============================================================================================================
        '=============================================================================================================
        Try
            'Citation greater than 100-identification attribute
            Dim CTND As Xml.XmlNodeList = Xdoc.SelectNodes(".//Bibliography//Citation")
            If (IsNothing(CTND) = False And CTND.Count > 0) Then
                For k As Integer = 0 To CTND.Count - 1
                    Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                    If (CTND.Count <= 99) Then
                        attr.Value = "Literatur_Entry"
                    Else
                        If (CTND.Count >= 100) Then
                            If (k <= 98) Then
                                attr.Value = "Literatur_Entry_1-99"
                            Else
                                attr.Value = "Literatur_Entry"
                            End If
                        End If
                    End If
                    CTND(k).Attributes.Append(attr)
                Next

            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddattrToCitaion()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_Add_cs_tablefigure_totblfig()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_Add_cs_tablefigure_totblfig
        'PARAMETER    :-
        'AIM          :This function add cs_tablefigure attribute to table and figure.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim Nds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Table|.//Figure")
            If (IsNothing(Nds) = False And Nds.Count > 0) Then
                For i As Integer = 0 To Nds.Count - 1
                    Dim Nd As Xml.XmlNode = Nds(i)
                    Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_tablefigure")
                    If (Nd.Name.ToLower = "table") Then
                        If (Nd.ParentNode.OuterXml.Contains("<Para><Table") = True And Nd.ParentNode.OuterXml.Contains("</Table></Para>") = True) Then
                            attr.Value = "table"
                            Nd.ParentNode.Attributes.Append(attr)
                        End If
                    End If
                    If (Nd.Name.ToLower = "figure") Then
                        If (Nd.ParentNode.OuterXml.Contains("<Para><Figure") = True And Nd.ParentNode.OuterXml.Contains("</Figure></Para>") = True) Then
                            attr.Value = "figure"
                            Nd.ParentNode.Attributes.Append(attr)
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_Add_cs_tablefigure_totblfig()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_Removespace_Acknw()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_Removespace_Acknw
        'PARAMETER    :-
        'AIM          :This function do addition, movement, modification of node.
        '=============================================================================================================
        '=============================================================================================================
        'Try
        '    Dim Node As Xml.XmlNode = Xdoc.SelectSingleNode(".//Acknowledgments")
        '    If (IsNothing(Node) = False) Then
        '        Node.InnerXml = Regex.Replace(Node.InnerXml, ">\s+", ">", RegexOptions.Singleline)
        '    End If
        'Catch ex As Exception
        '    CLog.LogMessages("Error in Fn_Removespace_Acknw()" + vbNewLine)
        '    CLog.LogMessages(ex.Message.ToString + vbNewLine)
        '    Throw
        'End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_AddLangtoArticleInfo()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddLangtoArticleInfo
        'PARAMETER    :-
        'AIM          :This function add language attribute to ArticleInfo.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim ArticleInfoLg As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleInfo")
            If (IsNothing(ArticleInfoLg) = False) Then
                If (ArticleInfoLg.Attributes.ItemOf("Language").Value = "De") Then
                    Article_Lg = "De"
                End If
                If (ArticleInfoLg.Attributes.ItemOf("Language").Value = "En") Then
                    Article_Lg = "En"
                End If

                If (ArticleInfoLg.Attributes.ItemOf("Language").Value = "Nl") Then
                    Article_Lg = "Nl"
                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddLangtoArticleInfo()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_Add_followedbylist()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_Add_followedbylist
        'PARAMETER    :-
        'AIM          :This function add attribute to para which is coming after list in order to apply different 
        '              style to this para ' cs_position='followedbylist'.
        '=============================================================================================================
        '=============================================================================================================
        Try
            XDocString = Xdoc.InnerXml
            XDocString = Regex.Replace(XDocString, "</Heading><Para><OrderedList", "</Heading><Para cs_position='abovelist'><OrderedList", RegexOptions.Singleline)
            XDocString = Regex.Replace(XDocString, "</Heading><Para><UnorderedList", "</Heading><Para cs_position='abovelist'><UnorderedList", RegexOptions.Singleline)
            Xdoc.InnerXml = XDocString
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_Add_followedbylist()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Private Function Fn_GraphicalAbstract()
        Try
            Dim Abs_Nodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//AbstractSection[@OutputMedium='All']")
            If (IsNothing(Abs_Nodes) = False And Abs_Nodes.Count > 0) Then
                For m As Integer = 0 To Abs_Nodes.Count - 1
                    Dim Node As Xml.XmlNode = Abs_Nodes(m)
                    If (IsNothing(Node.SelectSingleNode(".//Heading")) = False) Then
                        Dim HdNd = Node.SelectSingleNode(".//Heading")
                        If (IsNothing(HdNd.NextSibling) = False) Then
                            If (HdNd.NextSibling.Name.ToLower = "para") Then
                                If (HdNd.NextSibling.FirstChild.Name.ToLower = "figure") Then
                                    Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
                                    attr.Value = "graphicalAbs"
                                    HdNd.Attributes.Append(attr)
                                End If
                            End If
                        End If
                    End If

                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_GraphicalAbstract()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function

    Private Function Fn_BibHeadIdentify()
        Try
            Dim Nodes As Xml.XmlNode = Xdoc.SelectSingleNode(".//Bibliography")
            If (IsNothing(Nodes) = False) Then
                If (Nodes.FirstChild.Name.ToLower() = "heading") Then
                    If (IsNothing(Nodes.FirstChild.NextSibling) = False) Then
                        Dim BibNd As Xml.XmlNode = Nodes.FirstChild.NextSibling
                        If (BibNd.Name.ToLower() = "bibsection") Then
                            If (BibNd.FirstChild.Name.ToLower() = "heading") Then
                                Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
                                attr.Value = "bibmainhd"
                                Nodes.FirstChild.Attributes.Append(attr)

                                Dim attr1 As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
                                attr1.Value = "bibsubhd"
                                BibNd.FirstChild.Attributes.Append(attr1)
                            End If

                        End If
                    End If

                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_BibHeadIdentify()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Private Function Fn_TableAfter_Head()
        Try

            ''XDocString = Xdoc.InnerXml
            ''XDocString = Regex.Replace(XDocString, "</Heading><Para><Table", "</Heading><Para cs_position='tb_afterHead'><Table", RegexOptions.Singleline)
            ''Xdoc.InnerXml = XDocString

            Dim Hnodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//Section1/Heading|.//Section2/Heading|.//Section3/Heading|.//Section4/Heading|.//Section5/Heading|.//Section6/Heading|.//Section7/Heading")

            If (IsNothing(Hnodes) = False And Hnodes.Count > 0) Then
                For m As Integer = 0 To Hnodes.Count - 1
                    Dim Nd As Xml.XmlNode = Hnodes(m)
                    If (Nd.NextSibling.Name.ToLower = "para") Then
                        If ((Nd.NextSibling.OuterXml.Contains("<Para><Table") = True) And Nd.NextSibling.OuterXml.Contains("/Table></Para>") = True) Then
                            Dim Tbnnode As Xml.XmlNode = Nd.NextSibling.SelectSingleNode(".//Table")
                            If (IsNothing(Tbnnode.Attributes.ItemOf("Float")) = False) Then
                                If (Tbnnode.Attributes.ItemOf("Float").Value.ToLower = "yes") Then
                                    If (IsNothing(Nd.NextSibling.NextSibling) = False And Nd.NextSibling.NextSibling.Name.ToLower = "para") Then
                                        Dim NoGet As Xml.XmlNode = Nd.NextSibling.NextSibling
                                        Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_position")
                                        attr.Value = "tb_afterHead"
                                        NoGet.Attributes.Append(attr)
                                    End If

                                End If
                            End If

                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_TableAfter_Head()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Private Function Fn_Add_abovelist()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_Add_abovelist
        'PARAMETER    :-
        'AIM          :If list is coming in section para e.g<Para><Order  .then add attribute for 
        '              identification 'cs_position='abovelist'
        '=============================================================================================================
        '=============================================================================================================
        Try
            XDocString = Xdoc.InnerXml
            XDocString = Regex.Replace(XDocString, "</Heading><Para><OrderedList", "</Heading><Para cs_position='abovelist'><OrderedList", RegexOptions.Singleline)
            XDocString = Regex.Replace(XDocString, "</Heading><Para><UnorderedList", "</Heading><Para cs_position='abovelist'><UnorderedList", RegexOptions.Singleline)
            Xdoc.InnerXml = XDocString
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_Add_abovelist()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_CoverttablFig()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CoverttablFig
        'PARAMETER    :-
        'AIM          :This function convert table as image.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim TableNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Table[child::MediaObject and not(descendant::entry)]")
            For i As Integer = 0 To TableNds.Count - 1
                Dim TableFigStr As String = TableNds(i).ParentNode.InnerXml
                TableFigStr = Regex.Replace(TableFigStr, "<Table", "<Figure cs_Type='TabToFig'")
                TableFigStr = Regex.Replace(TableFigStr, "Table>", "Figure>")
                TableNds(i).ParentNode.InnerXml = TableFigStr
            Next
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CoverttablFig()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_Add_order_unorder_newline()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_Add_order_unorder_newline
        'PARAMETER    :-
        'AIM          :This function add "order_unorder_newline" attribute to list.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim OrderUnOrderNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//UnorderedList|.//OrderedList")
            If (IsNothing(OrderUnOrderNd) = False And OrderUnOrderNd.Count > 0) Then
                For Each OrdNd As Xml.XmlNode In OrderUnOrderNd
                    If (OrdNd.ParentNode.OuterXml.StartsWith("<Para><UnorderedList") Or OrdNd.ParentNode.OuterXml.StartsWith("<Para><OrderedList")) Then
                    Else
                        Dim result As Boolean = False
                        'If (IsNothing(OrdNd.ParentNode.PreviousSibling) = False) Then
                        '    If (OrdNd.ParentNode.PreviousSibling.Name.ToLower = "heading") Then
                        '        result = False
                        '    Else
                        '        result = True
                        '    End If
                        'Else
                        '    result = True
                        'End If
                        result = True
                        If (result = True) Then
                            Dim OUNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                            Dim OUAttr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                            OUAttr.Value = "order_unorder_newline"
                            OUNd.Attributes.Append(OUAttr)
                            OUNd.InnerText = vbNewLine
                            OrdNd.ParentNode.InsertBefore(OUNd, OrdNd)
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_Add_order_unorder_newline()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Sub InIt()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:InIt
        'PARAMETER    :-
        'AIM          :This function do addition, movement, modification of node.
        '=============================================================================================================
        '=============================================================================================================

        Fn_GraphicalAbstract()

        Fn_BibHeadIdentify()

        Fn_TableAfter_Head()

        Fn_EndInlineEquation()

        Fn_MakeCityStateCountry()

        Fn_ListMaking()

        Fn_TableList()

        Fn_CharAlign()

        Fn_ParenthesisToEquation()

        Fn_AddLangToBSL_Motto()

        Fn_AddDOIToArticle()

        ''Fn_CitationNumberdot()

        ''Fn_CheckAffID()

        Fn_AuthorAffiliationClass()

        Fn_Addcs_timeidintblfig()

        Fn_AddFootnote_cs_type_attr()

        Fn_DisplaySymbol_Superscript()

        Fn_CoverttablFig()

        Fn_AddAttrtoElement()

        Fn_AddattrToCitaion()

        Fn_AddLangattrToCitaion()

        Fn_ListFollowedByComment()

        Fn_Add_cs_tablefigure_totblfig()

        Fn_Removespace_Acknw()

        Fn_AddLangtoArticleInfo()

        oReq.oFigObj.GetJobName(FigureConversionFile, INXMLName)

        Fn_CreateJournalTitleFrame()

        Fn_CreateRunningTitle()

        Fn_CreateDOIFrame()

        Fn_PlaceOpenChoiceNode_New()

        ''Hframe = "|" + Infilemame + "|"
        ''If (ignorehistroyframe.Contains(Hframe) = False) Then
        ''    If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = False) Then
        ''        Fn_CreateArticleHistoryFrame()
        ''    End If

        ''End If
        Fn_CreateArticleHistoryFrame()

        Fn_CreateRunningFooter()

        Fn_CreateArticleCategory()

        Fn_CreateAbstract_KeywordNode()

        Fn_AddTableCharAlign()

        Fn_Sec_HeadingafterHeading()

        Fn_FormPara_afterHeading()


        Fn_Add_order_unorder_newline()

        Fn_Add_followedbylist()

        Fn_Add_abovelist()

        Fn_ConvertTableAsFigure()

        Fn_CreateTableList()

        Fn_AddMspace_toCitationnumber()

        'Added below code for finding numbering style
        'NumberingType = oReq.oFigObj.GetNumberingStyle(FigureConversionFile, INXMLName)
        If (IsNothing(Xdoc.SelectSingleNode(".//ArticleInfo[@NumberingStyle]")) = False) Then
            NumberingType = Xdoc.SelectSingleNode(".//ArticleInfo/@NumberingStyle").Value
        End If

        CID = Xdoc.SelectSingleNode(".//ArticleID").InnerText
        Dim ObjSN As New ClsAutoSectionNumbering(Xdoc, CID, NumberingType)

        '''' Fn_AddNspaceInHead()

        Fn_AddImgatr_Biography()

        AddInfoForMatadataPDF()

        'Get running head style from database add this in information afterwords in AddInformation() function 
        RunningHeadStyle = oReq.oFigObj.GetRunningHeadStyle(FigureConversionFile, INXMLName)

        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Private Function Fn_AddTableCharAlign()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddTableCharAlign
        'PARAMETER    :-
        'AIM          :This function add attribute to table node which is required for character alignment.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim TableNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Table")
            If (TableNds.Count > 0 And IsNothing(TableNds) = False) Then
                For Each TbNd As Xml.XmlNode In TableNds
                    Dim TbColCnt As Integer = CInt(TbNd.SelectSingleNode(".//tgroup/@cols").InnerText)
                    For i As Integer = 0 To TbColCnt - 1
                        Dim TbCharentryNds As Xml.XmlNodeList = TbNd.SelectNodes(".//entry[@align='char' and @colname='c" + CStr(i + 1) + "']")
                        Dim entryCount As Integer = 0
                        Dim MaxCol As Integer = 0
                        Dim ContSize As Integer = 0
                        For Each charnd As Xml.XmlNode In TbCharentryNds
                            If (charnd.Attributes.ItemOf("char").Value = ".") Then
                                ContSize = charnd.InnerText.Split(" ")(0).Split(".")(0).Length
                            ElseIf (charnd.Attributes.ItemOf("char").Value = ",") Then
                                ContSize = charnd.InnerText.Split(" ")(0).Split(",")(0).Length
                            Else
                            End If
                            If (entryCount = 0) Then
                                MaxCol = ContSize
                            End If
                            If (ContSize > MaxCol) Then
                                MaxCol = ContSize
                            End If
                            entryCount += 1
                        Next
                        For Each Entrycharnd As Xml.XmlNode In TbCharentryNds
                            Dim CharCnt As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_charcount")
                            CharCnt.Value = MaxCol
                            Entrycharnd.Attributes.Append(CharCnt)
                        Next
                    Next
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddTableCharAlign()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function AddInfoForMatadataPDF()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:AddInfoForMatadataPDF
        'PARAMETER    :-
        'AIM          :This function collect all the metadata information to display in PDF.
        '=============================================================================================================
        '=============================================================================================================
        Dim COMPDFObj As New PDFINFO(Xdoc)
        JournalName = GetJournalName()
        Dim JournalFullName As String = GetJournalTitle()
        'Collecting Proof Procedure Information
        PROOF_Info = COMPDFObj.ProofNdCreation(JournalFullName, Article_Lg)
        'create cts node info
        CTSINFO = COMPDFObj.CTSNdCreation(JournalName, Article_Lg)
        'create offprint info
        OffPrint_Data = COMPDFObj.OffPrintNdCreation(JournalName)
        Try
            'create metadat info 
            Dim colorinprint As Boolean = False
            colorinprint = colorinPrintInfo()
            Dim xnodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//Article")
            Dim Str As String = ""
            Str = COMPDFObj.MetaDataNdCreation(JournalFullName, Article_Lg, colorinprint)
            Dim articleinfo As String = Editmetadata(Str, xnodes(0))
            XMLM_Data += articleinfo

            'Replace emphasis bold italic bolditalic super and sub
            XMLM_Data = XMLM_Data.Replace("<Emphasis Type=""Bold"">", "<Emphasis Type=""Bold"" aid:cstyle=""Bold"">")
            XMLM_Data = XMLM_Data.Replace("<Emphasis Type=""Italic"">", "<Emphasis Type=""Italic"" aid:cstyle=""Italic"">")
            XMLM_Data = XMLM_Data.Replace("<Emphasis Type=""BoldItalic"">", "<Emphasis Type=""BoldItalic"" aid:cstyle=""Bold_Italic"">")
            XMLM_Data = XMLM_Data.Replace("<Emphasis Type=""SmallCaps"">", "<Emphasis Type=""Bold"" aid:cstyle=""small caps"">")

            XMLM_Data = XMLM_Data.Replace("<Superscript>", "<Superscript aid:cstyle=""Superscript"">")
            XMLM_Data = XMLM_Data.Replace("<Subscript>", "<Subscript aid:cstyle=""Subscript"">")

            XMLM_Data = XMLM_Data.Replace("<cs_text type=""superscript"">", "<cs_text type=""superscript"" aid:cstyle=""Superscript"">")
            XMLM_Data = XMLM_Data.Replace("<cs_text type=""Subscript"">", "<cs_text type=""Subscript"" aid:cstyle=""Subscript"">")

            XMLM_Data = XMLM_Data.Replace("<Superscript><Emphasis Type=""Bold"">", "<Superscript><Emphasis Type=""Bold"" aid:cstyle=""Super_Bold"">")
            XMLM_Data = XMLM_Data.Replace("<Superscript><Emphasis Type=""Italic"">", "<Superscript><Emphasis Type=""Italic"" aid:cstyle=""Super_Italic"">")
            XMLM_Data = XMLM_Data.Replace("<Superscript><Emphasis Type=""BoldItalic"">", "<Superscript><Emphasis Type=""BoldItalic"" aid:cstyle=""Super_BoldItalic"">")

            XMLM_Data = XMLM_Data.Replace("<Subscript><Emphasis Type=""Bold"">", "<Subscript><Emphasis Type=""Bold"" aid:cstyle=""Sub_Bold"">")
            XMLM_Data = XMLM_Data.Replace("<Subscript><Emphasis Type=""Italic"">", "<Subscript><Emphasis Type=""Italic"" aid:cstyle=""Sub_Italic"">")
            XMLM_Data = XMLM_Data.Replace("<Subscript><Emphasis Type=""BoldItalic"">", "<Subscript><Emphasis Type=""BoldItalic"" aid:cstyle=""Sub_BoldItalic"">")

            XMLM_Data = XMLM_Data.Replace("<cs_text type=""cSYMBOL"">", "<cs_text type=""cSYMBOL"" aid:cstyle=""cSYMBOL"">")

            Dim bxdoc As New Xml.XmlDocument
            bxdoc.PreserveWhitespace = True

            bxdoc.LoadXml("<Temp xmlns:aid=""http://ns.adobe.com/AdobeInDesign/4.0/"">" + XMLM_Data + "</Temp>")
            Dim Nodes As Xml.XmlNodeList = bxdoc.SelectNodes(".//cs_query")

            For i As Integer = 0 To Nodes.Count - 1
                Try
                    Dim nd As Xml.XmlNode = Nodes(i)
                    If IsNothing(nd) = False Then
                        nd.ParentNode.RemoveChild(nd)
                    End If
                Catch ex As Exception
                End Try
            Next
            XMLM_Data = bxdoc.InnerXml
            XMLM_Data = XMLM_Data.Replace("<Temp xmlns:aid=""http://ns.adobe.com/AdobeInDesign/4.0/"">", "").Replace("</Temp>", "")
            If (Article_Lg.ToLower = "de") Then
                XMLM_Data = XMLM_Data.Replace("'ArticleTitle'", "'Titel des Artikels'").Replace("'Article Sub-Title'", "'Untertitel'").Replace("'Article CopyRight - Year'", "'Copyright'").Replace("'Journal Name'", "'Zeitschrift'").Replace("'Corresponding Author'", "'Korrespondenzautor'").Replace("'Particle'", "'Adelsprädikat'").Replace("'Given Name'", "'Vorname'").Replace("'Suffix'", "'Namenszusatz'").Replace("'Organization'", "'Organisation'").Replace("'Address'", "'Adresse'").Replace("'Division'", "'Abteilung'").Replace("'Email'", "'E-Mail'").Replace("'Phone'", "'Telefon'").Replace("'Received'", "'Eingegangen'").Replace("'Revised'", "'Revidiert'").Replace("'Accepted'", "'Angenommen'").Replace("'Footnote Information'", "'Fußnoten zur Titelseite'").Replace("Author", "Autor")
            End If

        Catch ex As Exception
            CLog.LogMessages("Error in AddInfoForMatadataPDF()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function GetAbbrGivenName(ByVal GNames As Xml.XmlNode) As String
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetAbbrGivenName
        'PARAMETER    :GNames
        'AIM          :This function return abbrivated name of given node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim GivenName As Xml.XmlNode = GNames
            Dim GivenNameStr As String = ""
            For i As Integer = 0 To GivenName.InnerText.Split(" ").Length - 1
                If (GivenName.InnerText.Split("-").Length > 1) Then
                    For j As Integer = 0 To GivenName.InnerText.Split("-").Length - 1
                        If (j = GivenName.InnerText.Split("-").Length - 1) Then
                            GivenNameStr += GivenName.InnerText.Split("-")(j)(0) + ". "
                        Else
                            GivenNameStr += GivenName.InnerText.Split("-")(j)(0) + ".-"
                        End If
                    Next
                Else
                    GivenNameStr += GivenName.InnerText.Split(" ")(i)(0) + ". "
                End If

            Next

            Return GivenNameStr
        Catch ex As Exception
            CLog.LogMessages("Error in GetAbbrGivenName()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function CreateFirstPdageOpenChoiceNode()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:CreateFirstPdageOpenChoiceNode
        'PARAMETER    :-
        'AIM          :This function create first page open chioce node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim Root As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article")
            If (IsNothing(Root) = False) Then
                OpenChoiceCopyrightNd = Xdoc.CreateElement("cs_OpenChoiceCopyright")
                Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                OpenChoiceCopyrightNd.Attributes.Append(atr)
                atr.Value = "openchoicecopyright"
                If (Article_Lg.ToLower = "de") Then
                    OpenChoiceCopyrightNd.InnerText = "Dieser Artikel ist auf Springerlink.com mit Open Access verfügbar."
                Else
                    If (Article_Lg.ToLower = "en") Then
                        OpenChoiceCopyrightNd.InnerText = "This article is published with open access at Springerlink.com"
                    End If
                End If
                Root.InsertBefore(OpenChoiceCopyrightNd, Root.FirstChild)
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in CreateFirstPdageOpenChoiceNode()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function

    Public Function CreateLastPageOpenChoiceNode(Optional ByVal Alreadyexist As Boolean = False)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:CreateLastPageOpenChoiceNode
        'PARAMETER    :Alreadyexist
        'AIM          :This function create last page open choice node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim XmlPath As String = ""
            XmlPath = oReq.oFigObj.GetQDrivexmlPath(FigureConversionFile, INXMLName)
            If (XmlPath <> "") Then
                Dim dirInfo As New System.IO.DirectoryInfo(XmlPath)
                dirInfo = dirInfo.Parent.Parent
                If (dirInfo.FullName.ToLower.Contains("discrete object")) Then
                    XmlPath = dirInfo.FullName.ToLower + "\xml\" + INXMLName.Split("\")(INXMLName.Split("\").Length - 1).ToLower
                Else
                    If (dirInfo.FullName.ToLower.Contains("article_stage")) Then
                        XmlPath = dirInfo.FullName.ToLower + "\dispatch\" + INXMLName.Split("\")(INXMLName.Split("\").Length - 1).ToLower
                    End If
                End If
            End If
            If (Alreadyexist = True) Then
                Dim ExistingNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//Acknowledgments/FormalPara")
                If (IsNothing(ExistingNd) = False) Then
                    For i As Integer = 0 To ExistingNd.Count - 1
                        Dim node As Xml.XmlNode = ExistingNd(i)
                        If (node.SelectSingleNode(".//Heading").InnerText = "Open Access") Then
                            If (Article_Lg.ToLower = "de") Then
                                node.SelectSingleNode(".//Para").InnerText = "Dieser Artikel unterliegt den Bedingungen der Creative Commons Attribution Noncommercial License. Dadurch sind die nichtkommerzielle Nutzung, Verteilung und Reproduktion erlaubt, sofern der/die Originalautor/en und die Quelle angegeben sind."
                            Else
                                If (Article_Lg.ToLower = "en") Then
                                    node.SelectSingleNode(".//Para").InnerText = "This article is distributed under the terms of the Creative Commons Attribution Noncommercial License which permits any noncommercial use, distribution, and reproduction in any medium, provided the original author(s) and source are credited."
                                End If
                            End If
                        End If
                    Next
                End If
            Else
                Dim OpenChoiceInfoNd As Xml.XmlNode = Xdoc.CreateElement("FormalPara")
                Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("RenderingStyle")
                OpenChoiceInfoNd.Attributes.Append(atr)
                atr.Value = "Style1"
                If (Article_Lg.ToLower = "de") Then
                    OpenChoiceInfoNd.InnerXml = "<Heading>Open Access</Heading><Para>Dieser Artikel unterliegt den Bedingungen der Creative Commons Attribution Noncommercial License. Dadurch sind die nichtkommerzielle Nutzung, Verteilung und Reproduktion erlaubt, sofern der/die Originalautor/en und die Quelle angegeben sind.</Para>"
                Else
                    If (Article_Lg.ToLower = "en") Then
                        OpenChoiceInfoNd.InnerXml = "<Heading>Open Access</Heading><Para>This article is distributed under the terms of the Creative Commons Attribution Noncommercial License which permits any noncommercial use, distribution, and reproduction in any medium, provided the original author(s) and source are credited.</Para>"
                    End If
                End If

                If (System.IO.File.Exists(XmlPath) = True) Then
                    ' ReplaceQDrivexml(OpenChoiceInfoNd.OuterXml, XmlPath)
                End If
                Dim Root As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleBackmatter/Acknowledgments")
                If (IsNothing(Root) = True) Then
                    Root = Xdoc.SelectSingleNode(".//Article/ArticleBackmatter")
                    If (IsNothing(Root) = True) Then
                        Root = Xdoc.SelectSingleNode(".//Article/Body")
                        OpenChoiceInfoNd.InnerXml = "<ArticleBackmatter><Acknowledgments>" + OpenChoiceInfoNd.InnerXml + "</Acknowledgments></ArticleBackmatter>"
                        Root.InsertAfter(OpenChoiceInfoNd, Root.NextSibling)
                    Else
                        OpenChoiceInfoNd.InnerXml = "<Acknowledgments>" + OpenChoiceInfoNd.InnerXml + "</Acknowledgments>"
                        Root.InsertBefore(OpenChoiceInfoNd, Root.FirstChild)
                    End If
                Else
                    Root.InsertAfter(OpenChoiceInfoNd, Root.LastChild)
                End If
            End If

        Catch ex As Exception
            CLog.LogMessages("Error in CreateLastPageOpenChoiceNode()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_AddImgatr_Biography()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddImgatr_Biography
        'PARAMETER    :-
        'AIM          :This function add cs_imageeixst attribute to biography node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            'If biography contain author potrait then add attribute cs_imageeixst value cs_imageeixst="true"
            Dim BiographyNodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//Biography[not(.='')]")
            If (IsNothing(BiographyNodes) = False And BiographyNodes.Count > 0) Then
                For i As Integer = 0 To BiographyNodes.Count - 1
                    Dim InnerNd As Xml.XmlNode = BiographyNodes(i)
                    Dim FigNode As Xml.XmlNode = BiographyNodes(i).SelectSingleNode(".//Figure")
                    If (IsNothing(FigNode) = False) Then
                        BiographyNodes(i).Attributes.ItemOf("cs_imageeixst").Value = "yes"
                        Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_Authorimagepath")
                        attr.Value = FigNode.SelectSingleNode(".//ImageObject/@FileRef").Value
                        BiographyNodes(i).Attributes.Append(attr)
                        Dim FigName As String = "Fig"
                    End If
                Next
            End If
            'Place biography at bottom of chapter
            Dim BiographyNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Author/Biography[not(.='')]")
            If (IsNothing(BiographyNds) = False) Then
                Dim Root As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleBackmatter")
                If (IsNothing(Root) = False) Then
                Else
                    Root = Xdoc.SelectSingleNode(".//Body")
                End If
                For i As Integer = 0 To BiographyNds.Count - 1
                    Try
                        Dim nd As Xml.XmlNode = BiographyNds(i)
                        Root.ParentNode.InsertBefore(nd, Root.ParentNode.LastChild)
                    Catch ex As Exception

                    End Try
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddImgatr_Biography()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Public Function Fn_AddNspaceInHead()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddNspaceInHead
        'PARAMETER    :-
        'AIM          :This function add enspace+indenttohere after all sections heading except section1 heading.
        '=============================================================================================================
        '=============================================================================================================
        Try
            If (NumberingType.ToLower.Trim = "contentonly" Or NumberingType.ToLower.Trim = "chaptercontent") Then
                Dim AllHeadings As Xml.XmlNodeList = Xdoc.SelectNodes(".//Section1/Heading[not(.='')]|.//Section2/Heading[not(.='')]|.//Section3/Heading[not(.='')]|.//Section4/Heading[not(.='')]")                   'Xdoc.SelectNodes(".//Heading")
                For Each heading As Xml.XmlNode In AllHeadings
                    Dim tempNds As Xml.XmlNode = heading
                    Try
                        If (IsNothing(tempNds) = False) Then
                            Dim InnerX As String = heading.InnerXml
                            If (IsNumeric(InnerX.Substring(0, 1).ToString)) Then
                                If (Regex.Match(InnerX, "(\d+)((\.)?(\d+)?(\.)?(\d+)?(\.)?)(\s|<)", RegexOptions.Singleline).Success = True) Then
                                    Dim s As Regex
                                    s = New Regex("(\d+)((\.)?(\d+)?(\.)?(\d+)?(\.)?)(\s|<)", RegexOptions.Singleline)
                                    InnerX = s.Replace(InnerX, "$1" + "$2" + "<cs_text type='enspace'>&#8194;</cs_text>" + "<cs_text type=""indentohere""></cs_text>", 1)
                                End If
                                heading.InnerXml = InnerX
                            End If
                        End If
                    Catch ex As Exception

                    End Try
                Next
            End If

        Catch ex As Exception
            CLog.LogMessages("Error in Fn_AddNspaceInHead()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Fn_FormPara_afterHeading()
        Try
            Dim HeadNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//FormalPara[@RenderingStyle='Style1']|.//FormalPara[@RenderingStyle='Style2']")
            If (IsNothing(HeadNds) = False And HeadNds.Count > 0) Then
                For i As Integer = 0 To HeadNds.Count - 1
                    Dim Nd As Xml.XmlNode = HeadNds(i)
                    If (IsNothing(Nd.PreviousSibling) = False) Then
                        If (Nd.PreviousSibling.Name.ToLower = "heading") Then
                            Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_formparapos")
                            atr.Value = "formparaaftrhead"
                            Nd.Attributes.Append(atr)
                        End If
                    End If
                Next
            End If
        Catch ex As Exception

        End Try
    End Function
    Public Function Fn_Sec_HeadingafterHeading()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_Sec_HeadingafterHeading
        'PARAMETER    :-
        'AIM          :This function add attribute if section heading is followed by other section heading.
        '=============================================================================================================
        Try
            '=============================================================================================================
            Dim Section1_HeadNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Section1/Heading")
            If (IsNothing(Section1_HeadNds) = False And Section1_HeadNds.Count > 0) Then
                Try
                    For i As Integer = 0 To Section1_HeadNds.Count - 1
                        Dim Nd As Xml.XmlNode = Section1_HeadNds(i)
                        Dim node As Xml.XmlNode = Nd
                        If (node.NextSibling.OuterXml.StartsWith("<!--") = True) Then
                            While (node.NextSibling.OuterXml.StartsWith("<!--"))
                                node = node.NextSibling
                            End While
                        End If


                        If (node.NextSibling.Name.ToLower = "section2") Then
                            If (node.NextSibling.FirstChild.Name.ToLower = "heading") Then
                                Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_headposition")
                                attr.Value = "h2afterh1"
                                node.NextSibling.FirstChild.Attributes.Append(attr)
                            End If
                        End If
                    Next
                Catch ex As Exception

                End Try
            End If
            Dim Section2_HeadNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Section2/Heading")
            If (IsNothing(Section2_HeadNds) = False And Section2_HeadNds.Count > 0) Then
                Try
                    For i As Integer = 0 To Section2_HeadNds.Count - 1
                        Dim Nd As Xml.XmlNode = Section2_HeadNds(i)
                        Dim node As Xml.XmlNode = Nd
                        If (node.NextSibling.OuterXml.StartsWith("<!--") = True) Then
                            While (node.NextSibling.OuterXml.StartsWith("<!--"))
                                node = node.NextSibling
                            End While
                        End If
                        If (node.NextSibling.Name.ToLower = "section3") Then
                            If (node.NextSibling.FirstChild.Name.ToLower = "heading") Then
                                Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_headposition")
                                attr.Value = "h3afterh2"
                                node.NextSibling.FirstChild.Attributes.Append(attr)
                            End If
                        End If
                    Next
                Catch ex As Exception

                End Try
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_Sec_HeadingafterHeading()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Fn_ACDC_GetOpenChoice()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetOpenChoice
        'PARAMETER    :FigureConversionFile,inputxml
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        BOpenChoice = ""
        Try
            Dim OpenChoiceNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleGrants[@Type='OpenChoice']")
            If (IsNothing(OpenChoiceNd) = False) Then
                BOpenChoice = "openchoice"
            End If
        Catch ex As Exception

        End Try
        Return BOpenChoice
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Fn_PlaceOpenChoiceNode_New()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_PlaceOpenChoiceNode_New
        'PARAMETER    :-
        'AIM          :This function first check whether articletype is "open choice" then create first and last page 
        '              node.
        '=============================================================================================================
        '=============================================================================================================
        BOpenChoice = Fn_ACDC_GetOpenChoice()
        Try

            Dim xmlPath As String = ""
            If (BOpenChoice <> "") Then
                If (BOpenChoice.ToLower = "openchoice") Then
                    'First create firstpg node and add it as first node of article tag

                    Fn_ACDC_CreateFirstPageOpenChoiceNode()

                    Fn_ACDC_CreateLastPageOpenChoiceNode()
                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_PlaceOpenChoiceNode_New()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Fn_ACDC_CreateLastPageOpenChoiceNode()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_ACDC_CreateLastPageOpenChoiceNode
        'PARAMETER    :Alreadyexist
        'AIM          :This function create last page open choice node.
        '=============================================================================================================
        '=============================================================================================================
        Try

            Dim cs_openChoiceNd As Xml.XmlNode = Xdoc.CreateElement("cs_openchoice")
            Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
            attr.Value = "openchoice"
            cs_openChoiceNd.Attributes.Append(attr)

            Dim Nd As Xml.XmlNode = Xdoc.SelectSingleNode(".//License[@Type='OpenAccess']")
            If (IsNothing(Nd) = False) Then
                cs_openChoiceNd.InnerXml = Nd.OuterXml
            End If

            If (cs_openChoiceNd.InnerXml <> "") Then
                Dim MainNd As Xml.XmlNode = Nothing
                If (IsNothing(Xdoc.SelectSingleNode(".//ArticleBackmatter/Appendix")) = False) Then
                    MainNd = Xdoc.SelectSingleNode(".//ArticleBackmatter/Appendix")
                    MainNd.ParentNode.InsertBefore(cs_openChoiceNd, MainNd)
                Else
                    If (IsNothing(Xdoc.SelectSingleNode(".//ArticleBackmatter/Bibliography")) = False) Then
                        MainNd = Xdoc.SelectSingleNode(".//ArticleBackmatter/Bibliography")
                        MainNd.ParentNode.InsertBefore(cs_openChoiceNd, MainNd)
                    Else
                        If (IsNothing(Xdoc.SelectSingleNode(".//ArticleBackmatter")) = False) Then
                            MainNd = Xdoc.SelectSingleNode(".//ArticleBackmatter")
                            MainNd.InsertAfter(cs_openChoiceNd, MainNd.LastChild)
                        Else
                            If (IsNothing(Xdoc.SelectSingleNode(".//Body")) = False) Then
                                MainNd = Xdoc.SelectSingleNode(".//Body")
                                MainNd.ParentNode.InsertAfter(cs_openChoiceNd, MainNd)
                            End If
                        End If
                    End If
                End If



            End If



            '' ''If (cs_openChoiceNd.InnerXml <> "") Then
            '' ''    Dim MainNd As Xml.XmlNode = Nothing
            '' ''    If (IsNothing(Xdoc.SelectSingleNode(".//ArticleBackmatter/Acknowledgments")) = False) Then
            '' ''        MainNd = Xdoc.SelectSingleNode(".//ArticleBackmatter/Acknowledgments")
            '' ''        MainNd.ParentNode.InsertAfter(cs_openChoiceNd, MainNd)
            '' ''    Else
            '' ''        If (IsNothing(Xdoc.SelectSingleNode(".//ArticleBackmatter")) = False) Then
            '' ''            MainNd = Xdoc.SelectSingleNode(".//ArticleBackmatter")
            '' ''            MainNd.InsertBefore(cs_openChoiceNd, MainNd.FirstChild)
            '' ''        Else
            '' ''            If (IsNothing(Xdoc.SelectSingleNode(".//Body")) = False) Then
            '' ''                MainNd = Xdoc.SelectSingleNode(".//Body")
            '' ''                MainNd.ParentNode.InsertAfter(cs_openChoiceNd, MainNd)
            '' ''            End If
            '' ''        End If
            '' ''    End If



            '' ''End If



        Catch ex As Exception
            CLog.LogMessages("Error in Fn_ACDC_CreateLastPageOpenChoiceNode" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Fn_ACDC_CreateFirstPageOpenChoiceNode()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_ACDC_CreateFirstPageOpenChoiceNode
        'PARAMETER    :-
        'AIM          :This function create first page open choice node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim Root As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article")
            If (IsNothing(Root) = False) Then
                OpenChoiceCopyrightNd = Xdoc.CreateElement("cs_OpenChoiceCopyright")
                Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                OpenChoiceCopyrightNd.Attributes.Append(atr)
                atr.Value = "openchoicecopyright"



                Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
                OpenChoiceCopyrightNd.Attributes.Append(attr)
                attr.Value = "acdc_trace"

                If (Article_Lg.ToLower = "de") Then
                    OpenChoiceCopyrightNd.InnerText = "Dieser Artikel ist auf Springerlink.com mit Open Access verfügbar."
                ElseIf (Article_Lg.ToLower = "en") Then
                    OpenChoiceCopyrightNd.InnerText = "This article is published with open access at Springerlink.com"
                ElseIf (Article_Lg.ToLower = "nl") Then
                    OpenChoiceCopyrightNd.InnerText = "Dit artikel is gepubliceerd met open toegang op Springerlink.com"
                End If

            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_ACDC_CreateFirstPageOpenChoiceNode()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    'Public Function Fn_PlaceOpenChoiceNode()
    '    '=============================================================================================================
    '    '=============================================================================================================
    '    'FUNCTION NAME:Fn_PlaceOpenChoiceNode
    '    'PARAMETER    :-
    '    'AIM          :This function first decide whether for given article "grandtype" value is null or openchoice
    '    '              If it is openchoice then it add openchoicefirstpage node and last page node
    '    '              First page node name OpenChoice
    '    '              Last page node name OpenChoiceInfo
    '    '=============================================================================================================
    '    '=============================================================================================================
    '    BOpenChoice = oReq.oFigObj.GetOpenChoice(FigureConversionFile, INXMLName)
    '    Try

    '        Dim xmlPath As String = ""
    '        BOpenChoice = oReq.oFigObj.GetOpenChoice(FigureConversionFile, INXMLName)
    '        If (BOpenChoice <> "") Then
    '            If (BOpenChoice.ToLower = "openchoice") Then
    '                'First create firstpg node and add it as first node of article tag

    '                CreateFirstPageOpenChoiceNode()

    '                'Then create second page node and add it before reference.if it is absent then add it as a last child of body node
    '                Try
    '                    Dim Alreadyexist As Boolean = False
    '                    Dim Node As Xml.XmlNodeList = Xdoc.SelectNodes(".//Acknowledgments//Heading[not(.='')]")
    '                    If (IsNothing(Node) = False) Then
    '                        For i As Integer = 0 To Node.Count - 1
    '                            Dim nd As Xml.XmlNode = Node(i)
    '                            If (nd.InnerText.ToLower = "open access") Then
    '                                Alreadyexist = True
    '                                CreateLastPageOpenChoiceNode(True)
    '                                Exit For
    '                            End If
    '                        Next
    '                    End If

    '                    If (Alreadyexist = False) Then
    '                        CreateLastPageOpenChoiceNode()
    '                    End If

    '                Catch ex As Exception

    '                End Try
    '            End If
    '        End If
    '    Catch ex As Exception
    '        CLog.LogMessages("Error in Fn_PlaceOpenChoiceNode()" + vbNewLine)
    '        CLog.LogMessages(ex.Message.ToString + vbNewLine)
    '        Throw
    '    End Try
    '    '====================================================END======================================================
    '    '=============================================================================================================
    'End Function
    Public Function CreateFirstPageOpenChoiceNode()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:CreateFirstPageOpenChoiceNode
        'PARAMETER    :-
        'AIM          :This function create first page open choice node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim Root As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article")
            If (IsNothing(Root) = False) Then
                OpenChoiceCopyrightNd = Xdoc.CreateElement("cs_OpenChoiceCopyright")
                Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                OpenChoiceCopyrightNd.Attributes.Append(atr)
                atr.Value = "openchoicecopyright"
                If (Article_Lg.ToLower = "de") Then
                    OpenChoiceCopyrightNd.InnerText = "Dieser Artikel ist auf Springerlink.com mit Open Access verfügbar."
                ElseIf (Article_Lg.ToLower = "en") Then
                    OpenChoiceCopyrightNd.InnerText = "This article is published with open access at Springerlink.com"
                ElseIf (Article_Lg.ToLower = "nl") Then
                    OpenChoiceCopyrightNd.InnerText = "Dit artikel is gepubliceerd met open toegang op Springerlink.com"
                End If
                ' Root.InsertBefore(OpenChoiceCopyrightNd, Root.FirstChild)
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in CreateFirstPageOpenChoiceNode()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_CreateAbstract_KeywordNode()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CreateAbstract_KeywordNode
        'PARAMETER    :-
        'AIM          :This function create abstract and keyword node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim Abstract_CombinedNds As Xml.XmlNode = Xdoc.CreateElement("cs_text")
            Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            Abstract_CombinedNds.Attributes.Append(atr)
            atr.Value = "abstract_keyword"

            'if article language is german then add 1.De_Abstract 2.De_Keyword 3.En_Title 4.En_SubTitle ....etc
            Dim RootNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleHeader")

            If (IsNothing(RootNd) = False) Then
                Dim ArticleInfo As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleInfo")
                If (IsNothing(ArticleInfo) = False) Then
                    If (Article_Lg.ToLower = "de") Then

                        Dim Abstract_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='De' and not(.='')]")
                        If (IsNothing(Abstract_De) = False) Then
                            Abstract_CombinedNds.InnerXml += Abstract_De.OuterXml
                        End If

                        Dim Keyword_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='De'and not(.='')]")
                        If (IsNothing(Keyword_De) = False) Then
                            Abstract_CombinedNds.InnerXml += Keyword_De.OuterXml
                        End If

                        'abbrevation
                        Dim DefinitionList_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//DefinitionList[@Language='De'and not(.='')]")
                        If (IsNothing(DefinitionList_De) = False) Then
                            Abstract_CombinedNds.AppendChild(DefinitionList_De)
                        End If

                        Dim ArticleTitle_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='En' and not(.='')]")
                        If (IsNothing(ArticleTitle_En) = False) Then
                            Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_En.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                        End If
                        Dim ArticleSubTitle_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='En' and not(.='')]")
                        If (IsNothing(ArticleSubTitle_En) = False) Then
                            If (LayoutnameDB.ToLower.Contains("large") = False And LayoutnameDB.ToLower.Contains("large") = False) Then ' If (LayoutnameDB.ToLower = "springervienna") Then
                                Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_En.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                            Else
                                If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                    Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_En.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                Else
                                    Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_En.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                End If
                            End If

                        Else
                            If (IsNothing(ArticleTitle_En) = False) Then
                                Abstract_CombinedNds.InnerXml += vbNewLine
                            End If
                        End If
                        Dim Abstract_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='En' and not(.='')]")
                        If (IsNothing(Abstract_En) = False) Then
                            Abstract_CombinedNds.InnerXml += Abstract_En.OuterXml
                        End If

                        Dim Keyword_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='En'and not(.='')]")
                        If (IsNothing(Keyword_En) = False) Then
                            Abstract_CombinedNds.InnerXml += Keyword_En.OuterXml
                        End If

                        'abbrevation
                        Dim DefinitionList_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//DefinitionList[@Language='En'and not(.='')]")
                        If (IsNothing(DefinitionList_En) = False) Then
                            Abstract_CombinedNds.AppendChild(DefinitionList_En)
                        End If

                        Dim ArticleTitle_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='Fr' and not(.='')]")
                        If (IsNothing(ArticleTitle_Fr) = False) Then
                            Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                        End If

                        Dim ArticleSubTitle_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='Fr' and not(.='')]")
                        If (IsNothing(ArticleSubTitle_Fr) = False) Then
                            If (LayoutnameDB.ToLower.Contains("large") = False And LayoutnameDB.ToLower.Contains("large") = False) Then ' If (LayoutnameDB.ToLower = "springervienna") Then
                                Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                            Else
                                If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                    Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                Else
                                    Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                End If
                            End If

                        Else
                            If (IsNothing(ArticleTitle_Fr) = False) Then
                                Abstract_CombinedNds.InnerXml += vbNewLine
                            End If
                        End If


                        Dim Abstract_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='Fr' and not(.='')]")
                        If (IsNothing(Abstract_Fr) = False) Then
                            Abstract_CombinedNds.InnerXml += Abstract_Fr.OuterXml

                        End If
                        Dim Keyword_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='Fr'and not(.='')]")
                        If (IsNothing(Keyword_Fr) = False) Then
                            Abstract_CombinedNds.InnerXml += Keyword_Fr.OuterXml
                        End If
                        'spanish
                        Dim ArticleTitle_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='Es' and not(.='')]")
                        If (IsNothing(ArticleTitle_Es) = False) Then
                            Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                        End If


                        Dim ArticleSubTitle_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='Es' and not(.='')]")
                        If (IsNothing(ArticleSubTitle_Es) = False) Then
                            If (LayoutnameDB.ToLower.Contains("large") = False And LayoutnameDB.ToLower.Contains("large") = False) Then ' If (LayoutnameDB.ToLower = "springervienna") Then
                                Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                            Else
                                If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                    Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                Else
                                    Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                End If
                            End If

                        Else
                            If (IsNothing(ArticleTitle_Es) = False) Then
                                Abstract_CombinedNds.InnerXml += vbNewLine
                            End If
                        End If


                        Dim Abstract_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='Es' and not(.='')]")
                        If (IsNothing(Abstract_Es) = False) Then
                            Abstract_CombinedNds.InnerXml += Abstract_Es.OuterXml

                        End If

                        Dim Keyword_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='Es'and not(.='')]")
                        If (IsNothing(Keyword_Es) = False) Then
                            Abstract_CombinedNds.InnerXml += Keyword_Es.OuterXml
                        End If

                        ''''~~~
                        Dim Keyword_dashdash As Xml.XmlNodeList = Xdoc.SelectNodes(".//KeywordGroup[@Language='--' and not(.='')]")
                        If (IsNothing(Keyword_dashdash) = False And Keyword_dashdash.Count > 0) Then
                            For j As Integer = 0 To Keyword_dashdash.Count - 1
                                Abstract_CombinedNds.InnerXml += Keyword_dashdash(j).OuterXml
                            Next
                        End If

                        ''''~~~
                        '==================================================
                        Dim csnode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                        Dim csnode_attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                        csnode_attr.Value = "undefined"
                        csnode.Attributes.Append(csnode_attr)
                        Dim ArticleTitle_Unknown As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleTitle[parent::ArticleInfo and not(@Language='De' or @Language='En' or @Language='Fr' or @Language='Es') and not(.='')]")
                        If (IsNothing(ArticleTitle_Unknown) = False And ArticleTitle_Unknown.Count > 0) Then
                            For i As Integer = 0 To ArticleTitle_Unknown.Count - 1
                                csnode.AppendChild(ArticleTitle_Unknown(i))
                                csnode.InnerXml += vbNewLine
                            Next
                        End If
                        Dim ArticleSubTitle_Unknown As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleSubTitle[parent::ArticleInfo and not(@Language='De' or @Language='En' or @Language='Fr' or @Language='Es') and not(.='')]")
                        If (IsNothing(ArticleSubTitle_Unknown) = False And ArticleSubTitle_Unknown.Count > 0) Then
                            For i As Integer = 0 To ArticleSubTitle_Unknown.Count - 1
                                csnode.AppendChild(ArticleSubTitle_Unknown(i))
                                csnode.InnerXml += vbNewLine
                            Next
                        End If
                        Dim Abstract_Es_Unknown As Xml.XmlNodeList = Xdoc.SelectNodes(".//Abstract[not(@Language='De' or @Language='En' or @Language='Fr' or @Language='Es') and not(.='')]")
                        If (IsNothing(Abstract_Es_Unknown) = False And Abstract_Es_Unknown.Count > 0) Then
                            For i As Integer = 0 To Abstract_Es_Unknown.Count - 1
                                csnode.AppendChild(Abstract_Es_Unknown(i))
                                csnode.InnerXml += vbNewLine
                            Next
                        End If
                        Dim Keyword_Unknown As Xml.XmlNodeList = Xdoc.SelectNodes(".//KeywordGroup[not(@Language='De' or @Language='En' or @Language='Fr' or @Language='Es' or @Language='--')and not(.='')]")
                        If (IsNothing(Keyword_Unknown) = False And Keyword_Unknown.Count > 0) Then
                            For i As Integer = 0 To Keyword_Unknown.Count - 1
                                csnode.AppendChild(Keyword_Unknown(i))
                                csnode.InnerXml += vbNewLine
                            Next
                        End If
                        If (csnode.InnerXml <> "") Then
                            Abstract_CombinedNds.InnerXml += vbNewLine
                            Abstract_CombinedNds.AppendChild(csnode)
                        End If
                        '==================================================

                    Else
                        If (Article_Lg.ToLower = "en") Then
                            Dim Abstract_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='En' and not(.='')]")
                            If (IsNothing(Abstract_En) = False) Then
                                Abstract_CombinedNds.InnerXml += Abstract_En.OuterXml
                            End If

                            Dim Keyword_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='En'and not(.='')]")
                            If (IsNothing(Keyword_En) = False) Then
                                Abstract_CombinedNds.InnerXml += Keyword_En.OuterXml
                            End If
                            'abbrevation
                            Dim DefinitionList_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//DefinitionList[@Language='En'and not(.='')]")
                            If (IsNothing(DefinitionList_En) = False) Then
                                Abstract_CombinedNds.AppendChild(DefinitionList_En)
                            End If


                            Dim ArticleTitle_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='De' and not(.='')]")
                            If (IsNothing(ArticleTitle_De) = False) Then
                                Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_De.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                            End If
                            Dim ArticleSubTitle_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='De' and not(.='')]")
                            If (IsNothing(ArticleSubTitle_De) = False) Then
                                If (LayoutnameDB.ToLower = "springervienna") Then
                                    Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_De.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                Else
                                    If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_De.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    Else
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_De.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    End If
                                End If

                            Else
                                If (IsNothing(ArticleTitle_De) = False) Then
                                    Abstract_CombinedNds.InnerXml += vbNewLine
                                End If
                            End If


                            Dim Abstract_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='De' and not(.='')]")
                            If (IsNothing(Abstract_De) = False) Then
                                Abstract_CombinedNds.InnerXml += Abstract_De.OuterXml
                            End If

                            Dim Keyword_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='De'and not(.='')]")
                            If (IsNothing(Keyword_De) = False) Then
                                Abstract_CombinedNds.InnerXml += Keyword_De.OuterXml
                            End If

                            'abbrevation
                            Dim DefinitionList_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//DefinitionList[@Language='De'and not(.='')]")
                            If (IsNothing(DefinitionList_De) = False) Then
                                Abstract_CombinedNds.AppendChild(DefinitionList_De)
                            End If

                            Dim ArticleTitle_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='Fr' and not(.='')]")
                            If (IsNothing(ArticleTitle_Fr) = False) Then
                                Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                            End If

                            Dim ArticleSubTitle_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='Fr' and not(.='')]")
                            If (IsNothing(ArticleSubTitle_Fr) = False) Then
                                If (LayoutnameDB.ToLower.Contains("large") = False And LayoutnameDB.ToLower.Contains("large") = False) Then ' If (LayoutnameDB.ToLower = "springervienna") Then
                                    Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                Else
                                    If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    Else
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    End If
                                End If

                            Else
                                If (IsNothing(ArticleTitle_Fr) = False) Then
                                    Abstract_CombinedNds.InnerXml += vbNewLine
                                End If
                            End If



                            Dim Abstract_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='Fr' and not(.='')]")
                            If (IsNothing(Abstract_Fr) = False) Then
                                Abstract_CombinedNds.InnerXml += Abstract_Fr.OuterXml

                            End If
                            Dim Keyword_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='Fr'and not(.='')]")
                            If (IsNothing(Keyword_Fr) = False) Then
                                Abstract_CombinedNds.InnerXml += Keyword_Fr.OuterXml
                            End If
                            'spanish
                            Dim ArticleTitle_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='Es' and not(.='')]")
                            If (IsNothing(ArticleTitle_Es) = False) Then
                                Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                            End If

                            Dim ArticleSubTitle_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='Es' and not(.='')]")
                            If (IsNothing(ArticleSubTitle_Es) = False) Then
                                If (LayoutnameDB.ToLower.Contains("large") = False And LayoutnameDB.ToLower.Contains("large") = False) Then ' If (LayoutnameDB.ToLower = "springervienna") Then
                                    Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                Else
                                    If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    Else
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    End If
                                End If

                            Else
                                If (IsNothing(ArticleTitle_Es) = False) Then
                                    Abstract_CombinedNds.InnerXml += vbNewLine
                                End If
                            End If



                            Dim Abstract_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='Es' and not(.='')]")
                            If (IsNothing(Abstract_Es) = False) Then
                                Abstract_CombinedNds.InnerXml += Abstract_Es.OuterXml

                            End If
                            Dim Keyword_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='Es'and not(.='')]")
                            If (IsNothing(Keyword_Es) = False) Then
                                Abstract_CombinedNds.InnerXml += Keyword_Es.OuterXml
                            End If
                            ''''~~~
                            Dim Keyword_dashdash As Xml.XmlNodeList = Xdoc.SelectNodes(".//KeywordGroup[@Language='--' and not(.='')]")
                            If (IsNothing(Keyword_dashdash) = False And Keyword_dashdash.Count > 0) Then
                                For j As Integer = 0 To Keyword_dashdash.Count - 1
                                    Abstract_CombinedNds.InnerXml += Keyword_dashdash(j).OuterXml
                                Next
                            End If
                            ''''~~~
                            '==================================================
                            Dim csnode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                            Dim csnode_attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                            csnode_attr.Value = "undefined"
                            csnode.Attributes.Append(csnode_attr)
                            Dim ArticleTitle_Unknown As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleTitle[parent::ArticleInfo and not(@Language='De' or @Language='En' or @Language='Fr' or @Language='Es') and not(.='')]")
                            If (IsNothing(ArticleTitle_Unknown) = False And ArticleTitle_Unknown.Count > 0) Then
                                For i As Integer = 0 To ArticleTitle_Unknown.Count - 1
                                    csnode.AppendChild(ArticleTitle_Unknown(i))
                                    csnode.InnerXml += vbNewLine
                                Next
                            End If
                            Dim ArticleSubTitle_Unknown As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleSubTitle[parent::ArticleInfo and not(@Language='De' or @Language='En' or @Language='Fr' or @Language='Es') and not(.='')]")
                            If (IsNothing(ArticleSubTitle_Unknown) = False And ArticleSubTitle_Unknown.Count > 0) Then
                                For i As Integer = 0 To ArticleSubTitle_Unknown.Count - 1
                                    csnode.AppendChild(ArticleSubTitle_Unknown(i))
                                    csnode.InnerXml += vbNewLine
                                Next
                            End If
                            Dim Abstract_Es_Unknown As Xml.XmlNodeList = Xdoc.SelectNodes(".//Abstract[not(@Language='De' or @Language='En' or @Language='Fr' or @Language='Es') and not(.='')]")
                            If (IsNothing(Abstract_Es_Unknown) = False And Abstract_Es_Unknown.Count > 0) Then
                                For i As Integer = 0 To Abstract_Es_Unknown.Count - 1
                                    csnode.AppendChild(Abstract_Es_Unknown(i))
                                    csnode.InnerXml += vbNewLine
                                Next
                            End If
                            Dim Keyword_Unknown As Xml.XmlNodeList = Xdoc.SelectNodes(".//KeywordGroup[not(@Language='De' or @Language='En' or @Language='Fr' or @Language='Es' or @Language='--')and not(.='')]")
                            If (IsNothing(Keyword_Unknown) = False And Keyword_Unknown.Count > 0) Then
                                For i As Integer = 0 To Keyword_Unknown.Count - 1
                                    csnode.AppendChild(Keyword_Unknown(i))
                                    csnode.InnerXml += vbNewLine
                                Next
                            End If
                            If (csnode.InnerXml <> "") Then
                                Abstract_CombinedNds.InnerXml += vbNewLine
                                Abstract_CombinedNds.AppendChild(csnode)
                            End If
                            '==================================================
                        Else
                            'ssss
                            If (Article_Lg.ToLower = "nl") Then
                                Dim Abstract_Nl As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='Nl' and not(.='')]")
                                If (IsNothing(Abstract_Nl) = False) Then
                                    Abstract_CombinedNds.InnerXml += Abstract_Nl.OuterXml
                                End If
                                Dim Keyword_Nl As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='Nl'and not(.='')]")
                                If (IsNothing(Keyword_Nl) = False) Then
                                    Abstract_CombinedNds.InnerXml += Keyword_Nl.OuterXml
                                End If

                                'abbrevation
                                Dim DefinitionList_Nl As Xml.XmlNode = Xdoc.SelectSingleNode(".//DefinitionList[@Language='Nl'and not(.='')]")
                                If (IsNothing(DefinitionList_Nl) = False) Then
                                    Abstract_CombinedNds.AppendChild(DefinitionList_Nl)
                                End If

                                Dim ArticleTitle_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='En' and not(.='')]")
                                If (IsNothing(ArticleTitle_En) = False) Then
                                    Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_En.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                End If
                                Dim ArticleSubTitle_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='En' and not(.='')]")
                                If (IsNothing(ArticleSubTitle_En) = False) Then
                                    If (LayoutnameDB.ToLower.Contains("large") = False And LayoutnameDB.ToLower.Contains("large") = False) Then ' If (LayoutnameDB.ToLower = "springervienna") Then
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_En.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    Else
                                        If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                            Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_En.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                        Else
                                            Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_En.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                        End If
                                    End If

                                Else
                                    If (IsNothing(ArticleTitle_En) = False) Then
                                        Abstract_CombinedNds.InnerXml += vbNewLine
                                    End If
                                End If
                                Dim Abstract_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='En' and not(.='')]")
                                If (IsNothing(Abstract_En) = False) Then
                                    Abstract_CombinedNds.InnerXml += Abstract_En.OuterXml

                                End If
                                Dim Keyword_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='En'and not(.='')]")
                                If (IsNothing(Keyword_En) = False) Then
                                    Abstract_CombinedNds.InnerXml += Keyword_En.OuterXml
                                End If

                                'abbrevation
                                Dim DefinitionList_En As Xml.XmlNode = Xdoc.SelectSingleNode(".//DefinitionList[@Language='En'and not(.='')]")
                                If (IsNothing(DefinitionList_En) = False) Then
                                    Abstract_CombinedNds.AppendChild(DefinitionList_En)
                                End If

                                Dim ArticleTitle_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='De' and not(.='')]")
                                If (IsNothing(ArticleTitle_De) = False) Then
                                    Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_De.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                End If
                                Dim ArticleSubTitle_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='De' and not(.='')]")
                                If (IsNothing(ArticleSubTitle_De) = False) Then
                                    If (LayoutnameDB.ToLower = "springervienna") Then
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_De.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    Else
                                        If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                            Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_De.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                        Else
                                            Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_De.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                        End If
                                    End If

                                Else
                                    If (IsNothing(ArticleTitle_De) = False) Then
                                        Abstract_CombinedNds.InnerXml += vbNewLine
                                    End If
                                End If


                                Dim Abstract_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='De' and not(.='')]")
                                If (IsNothing(Abstract_De) = False) Then
                                    Abstract_CombinedNds.InnerXml += Abstract_De.OuterXml
                                End If
                                Dim Keyword_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='De'and not(.='')]")
                                If (IsNothing(Keyword_De) = False) Then
                                    Abstract_CombinedNds.InnerXml += Keyword_De.OuterXml
                                End If

                                'abbrevation
                                Dim DefinitionList_De As Xml.XmlNode = Xdoc.SelectSingleNode(".//DefinitionList[@Language='De'and not(.='')]")
                                If (IsNothing(DefinitionList_De) = False) Then
                                    Abstract_CombinedNds.AppendChild(DefinitionList_De)
                                End If

                                Dim ArticleTitle_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='Fr' and not(.='')]")
                                If (IsNothing(ArticleTitle_Fr) = False) Then
                                    Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                End If

                                Dim ArticleSubTitle_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='Fr' and not(.='')]")
                                If (IsNothing(ArticleSubTitle_Fr) = False) Then
                                    If (LayoutnameDB.ToLower.Contains("large") = False And LayoutnameDB.ToLower.Contains("large") = False) Then ' If (LayoutnameDB.ToLower = "springervienna") Then
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    Else
                                        If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                            Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                        Else
                                            Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_Fr.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                        End If
                                    End If

                                Else
                                    If (IsNothing(ArticleTitle_Fr) = False) Then
                                        Abstract_CombinedNds.InnerXml += vbNewLine
                                    End If
                                End If



                                Dim Abstract_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='Fr' and not(.='')]")
                                If (IsNothing(Abstract_Fr) = False) Then
                                    Abstract_CombinedNds.InnerXml += Abstract_Fr.OuterXml
                                End If
                                Dim Keyword_Fr As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='Fr'and not(.='')]")
                                If (IsNothing(Keyword_Fr) = False) Then
                                    Abstract_CombinedNds.InnerXml += Keyword_Fr.OuterXml
                                End If
                                'spanish
                                Dim ArticleTitle_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleTitle[parent::ArticleInfo and @Language='Es' and not(.='')]")
                                If (IsNothing(ArticleTitle_Es) = False) Then
                                    Abstract_CombinedNds.InnerXml += Regex.Replace(ArticleTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                End If

                                Dim ArticleSubTitle_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleSubTitle[parent::ArticleInfo and @Language='Es' and not(.='')]")
                                If (IsNothing(ArticleSubTitle_Es) = False) Then
                                    If (LayoutnameDB.ToLower.Contains("large") = False And LayoutnameDB.ToLower.Contains("large") = False) Then ' If (LayoutnameDB.ToLower = "springervienna") Then
                                        Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                    Else
                                        If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                            Abstract_CombinedNds.InnerXml += "<cs_text type='space'> </cs_text><cs_text type='endash'>&#8211;</cs_text><cs_text type='space'> </cs_text>" + Regex.Replace(ArticleSubTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                        Else
                                            Abstract_CombinedNds.InnerXml += "<cs_text type='vbnewline'>" + vbNewLine + "</cs_text>" + Regex.Replace(ArticleSubTitle_Es.OuterXml, "\s+</", "</", RegexOptions.Singleline)
                                        End If
                                    End If

                                Else
                                    If (IsNothing(ArticleTitle_Es) = False) Then
                                        Abstract_CombinedNds.InnerXml += vbNewLine
                                    End If
                                End If



                                Dim Abstract_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='Es' and not(.='')]")
                                If (IsNothing(Abstract_Es) = False) Then
                                    Abstract_CombinedNds.InnerXml += Abstract_Es.OuterXml

                                End If
                                Dim Keyword_Es As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='Es'and not(.='')]")
                                If (IsNothing(Keyword_Es) = False) Then
                                    Abstract_CombinedNds.InnerXml += Keyword_Es.OuterXml
                                End If
                                Dim Keyword_dashdash As Xml.XmlNodeList = Xdoc.SelectNodes(".//KeywordGroup[@Language='--' and not(.='')]")
                                If (IsNothing(Keyword_dashdash) = False And Keyword_dashdash.Count > 0) Then
                                    For j As Integer = 0 To Keyword_dashdash.Count - 1
                                        Abstract_CombinedNds.InnerXml += Keyword_dashdash(j).OuterXml
                                    Next
                                End If
                            End If
                        End If

                    End If
                End If
            End If
            If (IsNothing(RootNd) = False) Then
                RootNd.InsertBefore(Abstract_CombinedNds, RootNd.FirstChild)
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CreateAbstract_KeywordNode()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Rechange_CorrspAuth()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Rechange_CorrspAuth
        'PARAMETER    :-
        'AIM          :This function add attribute to corresponding author for identifing that this node already added.
        '=============================================================================================================
        '=============================================================================================================
        Try
            'First add identify position attr to each author for xml export
            Dim AuthNode As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorGroup/Author")
            If (IsNothing(AuthNode) = False And AuthNode.Count > 0) Then
                Dim position = 1
                For i As Integer = 0 To AuthNode.Count - 1
                    Dim pos As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_actualposition")
                    pos.Value = position
                    AuthNode(i).Attributes.Append(pos)
                Next
            End If

            'Actual logic
            Dim AuthGPNode As Xml.XmlNode = Xdoc.SelectSingleNode(".//AuthorGroup")
            If (IsNothing(AuthGPNode) = False) Then
                Dim Nodes As Xml.XmlNodeList = AuthGPNode.SelectNodes(".//Author[@CorrespondingAffiliationID]") '<Author AffiliationIDS="Aff1" CorrespondingAffiliationID="Aff1">
                If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
                    Dim cnt As Integer = 1
                    For i As Integer = 0 To Nodes.Count - 1
                        Dim Nd As Xml.XmlNode = Nodes(i)
                        Dim Attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                        Attr.Value = "changepos_corr"

                        Dim aAttr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_pos")
                        aAttr.Value = cnt
                        Nd.Attributes.Append(Attr)
                        Nd.Attributes.Append(aAttr)
                        cnt = cnt + 1
                    Next
                    cnt = cnt - 1
                    While (cnt <> 0)
                        Dim Node As Xml.XmlNode = AuthGPNode.SelectSingleNode(".//Author[@cs_pos='" + cnt.ToString + "']")
                        If (IsNothing(Node) = False) Then
                            AuthGPNode.InsertBefore(Node, AuthGPNode.FirstChild)
                        End If
                        cnt = cnt - 1
                    End While

                End If
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Rechange_CorrspAuth()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    ''Private Function CreateAuth_AffNode()
    ''    '=============================================================================================================
    ''    '=============================================================================================================
    ''    'FUNCTION NAME:CreateAuth_AffNode
    ''    'PARAMETER    :-
    ''    'AIM          :This function create author affiliation node.
    ''    '=============================================================================================================
    ''    '=============================================================================================================
    ''    Try
    ''        Dim COMAuth_AffiliationObj As New clsAuthorAffiliation(Xdoc, Article_Lg)
    ''        COMAuth_AffiliationObj.Create_AuthAffiliationNode()
    ''        'Display only given name abbreviation in authaffiliation node
    ''        Dim Node_AuthAff As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_text[@type='auth_aff_collections']//AuthorName/GivenName")
    ''        If (IsNothing(Node_AuthAff) = False And Node_AuthAff.Count > 0) Then
    ''            For i As Integer = 0 To Node_AuthAff.Count - 1
    ''                Dim GivenName As Xml.XmlNode = Node_AuthAff(i)
    ''                If (GivenName.InnerText.Split(" ").Length > 1 Or GivenName.InnerText.Contains("-")) Then
    ''                    GivenName.InnerText = GetAbbrGivenName(GivenName).Trim
    ''                Else
    ''                    GivenName.InnerText = GivenName.InnerText(0) + "."
    ''                End If
    ''            Next
    ''        End If
    ''        Dim AuthNodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_text[@type='auth_aff_collections']")
    ''        Dim testcheck As Boolean = False
    ''        If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
    ''            Dim TestNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='auth_aff_collections']/cs_text[@type='sub_auth_aff_collections' and position()=1]")
    ''            If (IsNothing(TestNd) = False) Then
    ''                Dim Nd As Xml.XmlNode = TestNd.SelectSingleNode(".//cs_text[@type='auth_collections']/AuthorName[@cs_type='correspond']")
    ''                If (IsNothing(Nd) = False) Then
    ''                    testcheck = True
    ''                Else
    ''                    testcheck = False
    ''                End If
    ''                If (testcheck = False) Then
    ''                    Dim correNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='auth_aff_collections']/cs_text[@type='sub_auth_aff_collections'][cs_text[@type='auth_collections'][AuthorName[@cs_type='correspond']]]")
    ''                    If (IsNothing(correNd) = False) Then
    ''                        TestNd.ParentNode.InsertBefore(correNd, TestNd)
    ''                    End If
    ''                End If
    ''            End If

    ''        End If
    ''    Catch ex As Exception
    ''        CLog.LogMessages("Error in CreateAuth_AffNode()" + vbNewLine)
    ''        CLog.LogMessages(ex.Message.ToString + vbNewLine)
    ''        Throw
    ''    End Try
    ''    '====================================================END======================================================
    ''    '=============================================================================================================
    ''End Function
    Private Function Fn_CreateArticleHistoryFrame()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CreateArticleHistoryFrame
        'PARAMETER    :-
        'AIM          :This function create article history frame.
        '              If JournalSubType="A" then then ArticleHistory frame contain one line Date else if type B then
        '              it contain two line date+copyright
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim ReceivedDate As String = ""
            Dim RevisedDate As String = ""
            Dim AcceptedDate As String = ""
            Dim OnlineDate As String = ""
            Dim copyrightholdername As String = ""
            Dim CopyrightYear As String = ""

            Dim ArticleHistoryNds As Xml.XmlNode = Xdoc.CreateElement("cs_text")
            Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            ArticleHistoryNds.Attributes.Append(atr)
            atr.Value = "articlehistoryframe"

            Dim ReceivedNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//Received[not(.='')]")
            If (IsNothing(ReceivedNd) = False) Then
                If (Article_Lg.ToLower = "de") Then
                    ReceivedDate = GetGermanDate(ReceivedNd)
                Else
                    If (Article_Lg.ToLower = "nl") Then
                        ReceivedDate = GetGermanDate_Nl(ReceivedNd)
                    Else
                        ReceivedDate = GetDate(ReceivedNd)
                    End If

                End If
            End If

            Dim RevisedNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//Revised[not(.='')]")
            If (IsNothing(RevisedNd) = False) Then
                If (Article_Lg.ToLower = "de") Then
                    RevisedDate = GetGermanDate(RevisedNd)
                Else
                    If (Article_Lg.ToLower = "nl") Then
                        RevisedDate = GetGermanDate_Nl(ReceivedNd)
                    Else
                        RevisedDate = GetDate(RevisedNd)
                    End If

                End If
            End If

            Dim AcceptedNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//Accepted[not(.='')]")
            If (IsNothing(AcceptedNd) = False) Then
                If (Article_Lg.ToLower = "de") Then
                    AcceptedDate = GetGermanDate(AcceptedNd)
                Else
                    If (Article_Lg.ToLower = "nl") Then
                        AcceptedDate = GetGermanDate_Nl(ReceivedNd)
                    Else
                        AcceptedDate = GetDate(AcceptedNd)
                    End If

                End If
            End If

            Dim OnlineNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//OnlineDate[not(.='')]")
            If (IsNothing(OnlineNd) = False) Then
                'If (Article_Lg.ToLower = "de") Then
                '    ReceivedDate = GetGermanDate(OnlineNd)
                'Else
                '    If (Article_Lg.ToLower = "nl") Then
                '        ReceivedDate = GetGermanDate_Nl(ReceivedNd)
                '    Else
                '        ReceivedDate = GetDate(OnlineNd)
                '    End If

                'End If
            End If
            '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            If (Article_Lg.ToLower = "nl") Then
                If (ReceivedDate <> "") Then
                    If (JournalSubType = "A") Then
                        ArticleHistoryNds.InnerText = "Ontvangen op " + ReceivedDate
                    Else
                        ArticleHistoryNds.InnerText = "Ontvangen op: " + ReceivedDate
                    End If
                    If (AcceptedDate <> "") Then
                        If (JournalSubType = "A") Then
                            ArticleHistoryNds.InnerText += "; "
                        Else
                            ArticleHistoryNds.InnerText += " / "
                        End If

                    End If
                End If
                If (AcceptedDate <> "") Then
                    If (Article_Lg.ToLower = "nl") Then
                        ArticleHistoryNds.InnerText += "Geaccepteerd op: " + AcceptedDate
                    Else
                        If (JournalSubType = "A") Then
                            ArticleHistoryNds.InnerText += "Geaccepteerde op " + AcceptedDate
                        Else
                            ArticleHistoryNds.InnerText += "Geaccepteerde op: " + AcceptedDate
                        End If
                    End If


                End If
                Dim auth_aff_nd As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='auth_aff_collections']")
                Dim Nd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                attr.Value = "copyright"
                Nd.Attributes.Append(attr)

                If (JournalSubType = "B") Then
                    If (BOpenChoice.ToLower = "openchoice") Then
                        If (OpenChoiceCopyrightNd.InnerText <> "") Then
                            If (IsNothing(auth_aff_nd) = False And LayoutnameDB.ToLower.Contains("vsandgabler") = True) Then
                                'Nd.InnerXml = "© The Author(s) " + CStr(System.DateTime.Now.Year) + "." + " " + OpenChoiceCopyrightNd.InnerText + vbNewLine
                                Nd.InnerXml = "© " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName").InnerText + " " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear").InnerText + "." + " " + OpenChoiceCopyrightNd.InnerText + vbNewLine
                                auth_aff_nd.InsertBefore(Nd, auth_aff_nd.FirstChild)
                            Else
                                ArticleHistoryNds.InnerText += vbNewLine + "© " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName").InnerText + " " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear").InnerText + "." + " " + OpenChoiceCopyrightNd.InnerText
                            End If

                        End If
                    Else
                        Dim Articlecopyrightholdername As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName")
                        If (IsNothing(Articlecopyrightholdername) = False) Then
                            copyrightholdername = Articlecopyrightholdername.InnerText
                        End If
                        Dim ArticleCopyrightYear As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear")
                        If (IsNothing(ArticleCopyrightYear) = False) Then
                            CopyrightYear = ArticleCopyrightYear.InnerText
                        End If
                        If (copyrightholdername <> "" Or CopyrightYear <> "") Then
                            If (IsNothing(auth_aff_nd) = False And LayoutnameDB.ToLower.Contains("vsandgabler") = True) Then
                                ' Nd.InnerXml = "© " + copyrightholdername + " " + CopyrightYear + vbNewLine
                                Nd.InnerXml = "© " + copyrightholdername + " " + CopyrightYear + vbNewLine '5/09/2013
                                auth_aff_nd.InsertBefore(Nd, auth_aff_nd.FirstChild)

                                Dim DateNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                                Dim Aattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                                Aattr.Value = "vbgabler_historydt"
                                DateNd.Attributes.Append(Aattr)
                                DateNd.InnerText = ArticleHistoryNds.InnerXml + vbNewLine
                                ArticleHistoryNds.InnerXml = ""
                                auth_aff_nd.InsertBefore(DateNd, auth_aff_nd.FirstChild)

                            Else
                                ArticleHistoryNds.InnerXml += vbNewLine + "© " + copyrightholdername + " " + CopyrightYear

                                If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                    Dim auth_aff_nda As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                                    Dim authattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                                    authattr.Value = "auth_aff_collections"
                                    auth_aff_nda.Attributes.Append(authattr)


                                    Dim DateNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                                    Dim Aattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                                    Aattr.Value = "vbgabler_historydt"
                                    DateNd.Attributes.Append(Aattr)
                                    DateNd.InnerText = ArticleHistoryNds.InnerXml
                                    ArticleHistoryNds.InnerXml = ""
                                    auth_aff_nda.AppendChild(DateNd)
                                    Try
                                        Dim RootNda As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article")
                                        If (IsNothing(RootNda) = False) Then
                                            If (auth_aff_nda.InnerText <> "") Then
                                                RootNda.InsertBefore(auth_aff_nda, RootNda.FirstChild)
                                            End If
                                        End If
                                    Catch ex As Exception

                                    End Try

                                End If

                            End If
                        End If
                    End If
                End If
            End If
            If (Article_Lg.ToLower = "de") Then
                If (ReceivedDate <> "") Then
                    If (JournalSubType = "A") Then
                        ArticleHistoryNds.InnerText = "Eingegangen am " + ReceivedDate
                    Else
                        ArticleHistoryNds.InnerText = "Eingegangen: " + ReceivedDate
                    End If
                    If (AcceptedDate <> "") Then
                        If (JournalSubType = "A") Then
                            ArticleHistoryNds.InnerText += "; "
                        Else
                            ArticleHistoryNds.InnerText += " / "
                        End If

                    End If
                End If
                If (AcceptedDate <> "") Then
                    If (JournalSubType = "A") Then
                        ArticleHistoryNds.InnerText += "angenommen am " + AcceptedDate
                    Else
                        ArticleHistoryNds.InnerText += "Angenommen: " + AcceptedDate
                    End If

                End If
                Dim auth_aff_nd As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='auth_aff_collections']")
                Dim Nd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                attr.Value = "copyright"
                Nd.Attributes.Append(attr)

                If (JournalSubType = "B") Then
                    If (BOpenChoice.ToLower = "openchoice") Then
                        If (OpenChoiceCopyrightNd.InnerText <> "") Then
                            If (IsNothing(auth_aff_nd) = False And LayoutnameDB.ToLower.Contains("vsandgabler") = True) Then
                                Nd.InnerXml = "© " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName").InnerText + " " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear").InnerText + "." + " " + OpenChoiceCopyrightNd.InnerText + vbNewLine
                                auth_aff_nd.InsertBefore(Nd, auth_aff_nd.FirstChild)
                            Else
                                'ArticleHistoryNds.InnerText += vbNewLine + "© The Author(s) " + CStr(System.DateTime.Now.Year) + "." + " " + OpenChoiceCopyrightNd.InnerText
                                ArticleHistoryNds.InnerText += vbNewLine + "© " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName").InnerText + " " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear").InnerText + "." + " " + OpenChoiceCopyrightNd.InnerText
                            End If

                        End If
                    Else
                        Dim Articlecopyrightholdername As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName")
                        If (IsNothing(Articlecopyrightholdername) = False) Then
                            copyrightholdername = Articlecopyrightholdername.InnerText
                        End If
                        Dim ArticleCopyrightYear As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear")
                        If (IsNothing(ArticleCopyrightYear) = False) Then
                            CopyrightYear = ArticleCopyrightYear.InnerText
                        End If
                        If (copyrightholdername <> "" Or CopyrightYear <> "") Then
                            If (IsNothing(auth_aff_nd) = False And LayoutnameDB.ToLower.Contains("vsandgabler") = True) Then
                                ' Nd.InnerXml = "© " + copyrightholdername + " " + CopyrightYear + vbNewLine
                                Nd.InnerXml = "© " + copyrightholdername + " " + CopyrightYear + vbNewLine '5/09/2013
                                auth_aff_nd.InsertBefore(Nd, auth_aff_nd.FirstChild)

                                Dim DateNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                                Dim Aattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                                Aattr.Value = "vbgabler_historydt"
                                DateNd.Attributes.Append(Aattr)
                                DateNd.InnerText = ArticleHistoryNds.InnerXml + vbNewLine
                                ArticleHistoryNds.InnerXml = ""
                                auth_aff_nd.InsertBefore(DateNd, auth_aff_nd.FirstChild)

                            Else
                                ArticleHistoryNds.InnerXml += vbNewLine + "© " + copyrightholdername + " " + CopyrightYear

                                If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                    Dim auth_aff_nda As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                                    Dim authattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                                    authattr.Value = "auth_aff_collections"
                                    auth_aff_nda.Attributes.Append(authattr)


                                    Dim DateNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                                    Dim Aattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                                    Aattr.Value = "vbgabler_historydt"
                                    DateNd.Attributes.Append(Aattr)
                                    DateNd.InnerText = ArticleHistoryNds.InnerXml
                                    ArticleHistoryNds.InnerXml = ""
                                    auth_aff_nda.AppendChild(DateNd)
                                    Try
                                        Dim RootNda As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article")
                                        If (IsNothing(RootNda) = False) Then
                                            If (auth_aff_nda.InnerText <> "") Then
                                                RootNda.InsertBefore(auth_aff_nda, RootNda.FirstChild)
                                            End If
                                        End If
                                    Catch ex As Exception

                                    End Try

                                End If

                            End If
                        End If
                    End If
                End If
            End If

            If (Article_Lg.ToLower = "en") Then
                If (ReceivedDate <> "") Then
                    If (JournalSubType = "A") Then
                        ArticleHistoryNds.InnerText = "Received " + ReceivedDate
                    Else
                        ArticleHistoryNds.InnerText = "Received: " + ReceivedDate
                    End If
                End If
                If (RevisedDate <> "") Then
                    If (JournalSubType = "A") Then
                        ArticleHistoryNds.InnerText += "; " + "revised " + RevisedDate
                    Else
                        ArticleHistoryNds.InnerText += " / " + "Revised: " + RevisedDate
                    End If

                End If
                If (AcceptedDate <> "") Then
                    If (JournalSubType = "A") Then
                        ArticleHistoryNds.InnerText += "; " + "accepted " + AcceptedDate
                    Else
                        ArticleHistoryNds.InnerText += " / " + "Accepted: " + AcceptedDate
                    End If

                End If
                Dim auth_aff_nd As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_text[@type='auth_aff_collections']")
                Dim Nd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                attr.Value = "copyright"
                Nd.Attributes.Append(attr)

                If (JournalSubType = "B") Then
                    If (BOpenChoice.ToLower = "openchoice") Then
                        If (OpenChoiceCopyrightNd.InnerText <> "") Then
                            If (IsNothing(auth_aff_nd) = False And LayoutnameDB.ToLower.Contains("vsandgabler") = True) Then
                                ' Nd.InnerXml = "© The Author(s) " + CStr(System.DateTime.Now.Year) + "." + " " + OpenChoiceCopyrightNd.InnerText + vbNewLine
                                Nd.InnerXml = "© " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName").InnerText + " " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear").InnerText + "." + " " + OpenChoiceCopyrightNd.InnerText + vbNewLine
                                auth_aff_nd.InsertBefore(Nd, auth_aff_nd.FirstChild)
                            Else
                                '  ArticleHistoryNds.InnerText += vbNewLine + "© The Author(s) " + CStr(System.DateTime.Now.Year) + "." + " " + OpenChoiceCopyrightNd.InnerText
                                ArticleHistoryNds.InnerText += vbNewLine + "© " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName").InnerText + " " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear").InnerText + "." + " " + OpenChoiceCopyrightNd.InnerText
                            End If

                        End If
                    Else
                        Dim Articlecopyrightholdername As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName")
                        If (IsNothing(Articlecopyrightholdername) = False) Then
                            copyrightholdername = Articlecopyrightholdername.InnerText
                        End If
                        Dim ArticleCopyrightYear As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear")
                        If (IsNothing(ArticleCopyrightYear) = False) Then
                            CopyrightYear = ArticleCopyrightYear.InnerText
                        End If
                        If (copyrightholdername <> "" Or CopyrightYear <> "") Then
                            If (IsNothing(auth_aff_nd) = False And LayoutnameDB.ToLower.Contains("vsandgabler") = True) Then
                                Nd.InnerXml = "© " + copyrightholdername + " " + CopyrightYear + vbNewLine '5/09/2013
                                auth_aff_nd.InsertBefore(Nd, auth_aff_nd.FirstChild)
                                Dim DateNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                                Dim Aattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                                Aattr.Value = "vbgabler_historydt"
                                DateNd.Attributes.Append(Aattr)
                                DateNd.InnerText = ArticleHistoryNds.InnerXml + vbNewLine
                                ArticleHistoryNds.InnerXml = ""
                                auth_aff_nd.InsertBefore(DateNd, auth_aff_nd.FirstChild)
                            Else
                                ArticleHistoryNds.InnerXml += vbNewLine + "© " + copyrightholdername + " " + CopyrightYear
                                If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                    Dim auth_aff_nda As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                                    Dim authattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                                    authattr.Value = "auth_aff_collections"
                                    auth_aff_nda.Attributes.Append(authattr)
                                    Dim DateNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                                    Dim Aattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                                    Aattr.Value = "vbgabler_historydt"
                                    DateNd.Attributes.Append(Aattr)
                                    DateNd.InnerText = ArticleHistoryNds.InnerXml
                                    ArticleHistoryNds.InnerXml = ""
                                    auth_aff_nda.AppendChild(DateNd)
                                    Dim RootNda As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article")
                                    If (IsNothing(RootNda) = False) Then
                                        If (auth_aff_nda.InnerText <> "") Then
                                            RootNda.InsertBefore(auth_aff_nda, RootNda.FirstChild)
                                        End If
                                    End If
                                End If

                            End If

                        End If
                    End If
                End If
            End If
            ''++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'Dim GetNode As Xml.XmlNode = Xdoc.SelectSingleNode(".//cs_OpenChoiceCopyright[@type='openchoicecopyright']")
            'If (IsNothing(GetNode) = False) Then
            '    Dim Str As String = ""
            '    If (Article_Lg.ToLower = "en") Then
            '        Str = "©" + " The Author(s) " + CStr(System.DateTime.Now.Year)
            '    Else
            '        If (Article_Lg.ToLower = "de") Then
            '            Str = "©" + " Die Autor(en) " + CStr(System.DateTime.Now.Year)
            '        Else
            '            If (Article_Lg.ToLower = "fr") Then
            '                Str = "©" + " The Author(s) " + CStr(System.DateTime.Now.Year)
            '            End If

            '        End If
            '    End If
            '    If (copyrightholdername = "" And CopyrightYear = "") Then
            '        Dim PrevStr As String = "© " + "The Author(s) " + CStr(System.DateTime.Now.Year)
            '        ArticleHistoryNds.InnerText = ArticleHistoryNds.InnerText.Replace(PrevStr, Str)
            '    Else

            '        Dim PrevStr As String = "© " + copyrightholdername + " " + CopyrightYear
            '        ArticleHistoryNds.InnerText = ArticleHistoryNds.InnerText.Replace(PrevStr, Str)

            '    End If
            'End If
            ''++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim RootNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleBackmatter")
            If (IsNothing(RootNd) = False) Then
            Else
                RootNd = Xdoc.SelectSingleNode(".//Body")
            End If
            If (IsNothing(RootNd) = False) Then
                RootNd.ParentNode.InsertAfter(ArticleHistoryNds, RootNd.ParentNode.FirstChild)
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CreateArticleHistoryFrame()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Fn_ListFollowedByComment()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_ListFollowedByComment
        'PARAMETER    :-
        'AIM          :This function is written if comment is coming after list then it will add attribute 
        '              to last itemcontent.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim ListNode As Xml.XmlNodeList = Xdoc.SelectNodes(".//OrderedList|.//UnorderedList")
            If (IsNothing(ListNode) = False) Then
                For Each Node As Xml.XmlNode In ListNode
                    Dim Processnode As Xml.XmlNode = Node.ParentNode
                    If (IsNothing(Processnode.NextSibling) = False) Then
                        If (Processnode.NextSibling.Name = "#comment") Then
                            Dim attr As Xml.XmlNode = Xdoc.CreateAttribute("cs_listbeforehead")
                            attr.Value = "true"
                            If (Node.Name.ToLower = "orderedlist") Then
                                Node.LastChild.LastChild.Attributes.Append(attr)
                            Else
                                If (Node.Name.ToLower = "unorderedlist") Then
                                    Node.LastChild.LastChild.Attributes.Append(attr)
                                End If

                            End If
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_ListFollowedByComment()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
    End Function
    Public Function GetGermanDate(ByVal DateInfoNOde As Xml.XmlNode)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetGermanDate
        'PARAMETER    :-
        'AIM          :This function return the date in german format.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim ResultDate As String = ""
            If (IsNothing(DateInfoNOde) = False) Then
                Dim Yr As String = DateInfoNOde.SelectSingleNode(".//Year").InnerText
                Dim Dt As String = CStr(CInt(DateInfoNOde.SelectSingleNode(".//Day").InnerText))
                Dim monthno As String = DateInfoNOde.SelectSingleNode(".//Month").InnerText
                Dim mnth As String = ""
                For mnthCnt As Integer = 1 To monthno
                    If (mnthCnt = 1) Then
                        mnth = "Januar"
                    End If
                    If (mnthCnt = 2) Then
                        mnth = "Februar"
                    End If
                    If (mnthCnt = 3) Then
                        mnth = "März"
                    End If
                    If (mnthCnt = 4) Then
                        mnth = "April"
                    End If
                    If (mnthCnt = 5) Then
                        mnth = "Mai"
                    End If
                    If (mnthCnt = 6) Then
                        mnth = "Juni"
                    End If
                    If (mnthCnt = 7) Then
                        mnth = "Juli"
                    End If
                    If (mnthCnt = 8) Then
                        mnth = "August"
                    End If
                    If (mnthCnt = 9) Then
                        mnth = "September"
                    End If
                    If (mnthCnt = 1) Then
                        mnth = "Januar"
                    End If
                    If (mnthCnt = 10) Then
                        mnth = "Oktober"
                    End If
                    If (mnthCnt = 11) Then
                        mnth = "November"
                    End If
                    If (mnthCnt = 12) Then
                        mnth = "Dezember"
                    End If
                Next

                ResultDate = Dt + ". " + mnth + " " + Yr

                Return ResultDate
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in GetGermanDate()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function

    Public Function GetGermanDate_Nl(ByVal DateInfoNOde As Xml.XmlNode)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetGermanDate
        'PARAMETER    :-
        'AIM          :This function return the date in german format.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim ResultDate As String = ""
            If (IsNothing(DateInfoNOde) = False) Then
                Dim Yr As String = DateInfoNOde.SelectSingleNode(".//Year").InnerText
                Dim Dt As String = CStr(CInt(DateInfoNOde.SelectSingleNode(".//Day").InnerText))
                Dim monthno As String = DateInfoNOde.SelectSingleNode(".//Month").InnerText
                Dim mnth As String = ""
                For mnthCnt As Integer = 1 To monthno
                    If (mnthCnt = 1) Then
                        mnth = "januari"
                    End If
                    If (mnthCnt = 2) Then
                        mnth = "februari"
                    End If
                    If (mnthCnt = 3) Then
                        mnth = "maart"
                    End If
                    If (mnthCnt = 4) Then
                        mnth = "april"
                    End If
                    If (mnthCnt = 5) Then
                        mnth = "mei"
                    End If
                    If (mnthCnt = 6) Then
                        mnth = "juni"
                    End If
                    If (mnthCnt = 7) Then
                        mnth = "juli"
                    End If
                    If (mnthCnt = 8) Then
                        mnth = "augustus"
                    End If
                    If (mnthCnt = 9) Then
                        mnth = "september"
                    End If
                    If (mnthCnt = 10) Then
                        mnth = "october"
                    End If
                    If (mnthCnt = 11) Then
                        mnth = "november"
                    End If
                    If (mnthCnt = 12) Then
                        mnth = "december"
                    End If
                Next

                ResultDate = Dt + ". " + mnth + " " + Yr

                Return ResultDate
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in GetGermanDate()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function GetDate(ByVal DateInfoNOde As Xml.XmlNode)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetDate
        'PARAMETER    :DateInfoNOde
        'AIM          :This function return date in english format
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim ResultDate As String = ""
            If (IsNothing(DateInfoNOde) = False) Then
                Dim Yr As String = DateInfoNOde.SelectSingleNode(".//Year").InnerText
                Dim Dt As String = DateInfoNOde.SelectSingleNode(".//Day").InnerText
                Dim monthno As String = DateInfoNOde.SelectSingleNode(".//Month").InnerText
                Dim mnth As String = ""
                For mnthCnt As Integer = 1 To monthno
                    Try
                        If (mnthCnt = 1) Then
                            mnth = "January"
                        End If
                        If (mnthCnt = 2) Then
                            mnth = "February"
                        End If
                        If (mnthCnt = 3) Then
                            mnth = "March"
                        End If
                        If (mnthCnt = 4) Then
                            mnth = "April"
                        End If
                        If (mnthCnt = 5) Then
                            mnth = "May"
                        End If
                        If (mnthCnt = 6) Then
                            mnth = "June"
                        End If
                        If (mnthCnt = 7) Then
                            mnth = "July"
                        End If
                        If (mnthCnt = 8) Then
                            mnth = "August"
                        End If
                        If (mnthCnt = 9) Then
                            mnth = "September"
                        End If
                        If (mnthCnt = 10) Then
                            mnth = "October"
                        End If
                        If (mnthCnt = 11) Then
                            mnth = "November"
                        End If
                        If (mnthCnt = 12) Then
                            mnth = "December"
                        End If
                    Catch ex As Exception

                    End Try
                Next

                ResultDate = Dt + " " + mnth + " " + Yr

                Return ResultDate

            End If
        Catch ex As Exception
            CLog.LogMessages("Error in GetDate()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_CreateJournalTitleFrame()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CreateJournalTitleFrame
        'PARAMETER    :-
        'AIM          :This function create Journal Title Frame.
        '              If JournalSubType="A" then JournalTitle FM contain 1.Article Title 2.Authors 3.Affiliation
        '              else JournalTitle FM contain 1.Article Title 2.Authors
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim InstitutionalA As String = ""
            Dim newNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
            Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            atr.Value = "journaltitle"
            newNd.Attributes.Append(atr)
            Dim ArticleTitle As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]")


            If (IsNothing(ArticleTitle) = False) Then
                newNd.InnerXml += Regex.Replace(ArticleTitle.OuterXml, "\s+</", "</", RegexOptions.Singleline)
            End If
            Dim ArticleSubTitleNDs As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleSubTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]")
            If (IsNothing(ArticleSubTitleNDs) = False) Then
                newNd.InnerXml += Regex.Replace(ArticleSubTitleNDs.OuterXml, "\s+</", "</", RegexOptions.Singleline)
            End If

            Dim AuthNameNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
            Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            attr.Value = "authname"
            AuthNameNode.Attributes.Append(attr)

            'Dim AuthorNameNDs As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorName[not(.='') and GivenName[not(.='')] and FamilyName[not(.='')]  and not(ancestor::Body) and not(ancestor::cs_text)]")
            Dim AuthorNameNDs As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorName[not(.='') and FamilyName[not(.='')]  and not(ancestor::Body) and not(ancestor::cs_text)]")

            If (IsNothing(AuthorNameNDs) = False And AuthorNameNDs.Count > 0) Then
                Dim authCount As Integer = 0
                For i As Integer = 0 To AuthorNameNDs.Count - 1
                    authCount = authCount + 1
                    Dim GivenName As String = ""
                    If (IsNothing(AuthorNameNDs(i).SelectNodes(".//GivenName[not(.='')]")) = False And AuthorNameNDs(i).SelectNodes(".//GivenName[not(.='')]").Count > 0) Then
                        For val As Integer = 0 To AuthorNameNDs(i).SelectNodes(".//GivenName[not(.='')]").Count - 1
                            If (val = 0) Then
                                GivenName = AuthorNameNDs(i).SelectNodes(".//GivenName[not(.='')]")(val).InnerXml
                            Else
                                GivenName = GivenName + " " + AuthorNameNDs(i).SelectNodes(".//GivenName[not(.='')]")(val).InnerXml
                            End If
                        Next
                        GivenName = "<GivenName>" + GivenName + "</GivenName>"
                    End If

                    Dim FamilyName As String = AuthorNameNDs(i).SelectSingleNode(".//FamilyName[not(.='')]").OuterXml
                    Dim Particle As Xml.XmlNode = AuthorNameNDs(i).SelectSingleNode(".//Particle[not(.='')]")
                    Dim supscrtext As String = ""
                    Try
                        supscrtext = "<Superscript>" + AuthorNameNDs(i).ParentNode.Attributes.ItemOf("AffiliationIDS").Value.ToLower.Replace("aff", "").Replace(" ", ",") + "</Superscript>"
                    Catch ex As Exception

                    End Try


                    Dim suffix As Xml.XmlNode = AuthorNameNDs(i).SelectSingleNode(".//Suffix[not(.='')]")

                    If (IsNothing(suffix) = False) Then
                        FamilyName = FamilyName.Replace("</FamilyName>", suffix.OuterXml + "</FamilyName>")
                    End If
                    FamilyName = FamilyName.Replace("</FamilyName>", supscrtext + "</FamilyName>")
                    If (IsNothing(Particle) = False) Then
                        AuthNameNode.InnerXml += "<AuthorName cs_authid='" + authCount.ToString + "'>" + GivenName + Particle.OuterXml + FamilyName + "</AuthorName>"
                    Else
                        AuthNameNode.InnerXml += "<AuthorName cs_authid='" + authCount.ToString + "'>" + GivenName + FamilyName + "</AuthorName>"
                    End If
                Next
                Dim instu As Xml.XmlNodeList = Xdoc.SelectNodes(".//InstitutionalAuthor")
                If (IsNothing(instu) = False And instu.Count > 0) Then
                    For i As Integer = 0 To instu.Count - 1
                        If (instu(i).SelectNodes(".//InstitutionalAuthorName[not(.='')]").Count = 1) Then
                            InstitutionalA = instu(i).SelectSingleNode(".//InstitutionalAuthorName[not(.='')]").OuterXml
                            'If (Infilemame.Trim.ToLower = "501") Then
                            Dim supscrtext As String = ""
                            Try
                                supscrtext = "<Superscript>" + instu(i).Attributes.ItemOf("AffiliationIDS").Value.ToLower.Replace("aff", "") + "</Superscript>"
                            Catch ex As Exception

                            End Try
                            InstitutionalA = InstitutionalA.Replace("</InstitutionalAuthorName>", supscrtext + "</InstitutionalAuthorName>")
                            'End If
                            AuthNameNode.InnerXml += "<InstitutionalAuthorName cs_authid='" + authCount.ToString + "'>" + InstitutionalA + "</InstitutionalAuthorName>"
                        End If
                    Next
                End If

                Try

                    Dim authorNamesCharCount As Integer = AuthNameNode.InnerText.Length
                    If (authorNamesCharCount > 140) Then '
                        Dim AtFullWidth As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_FullWidth")
                        AtFullWidth.Value = "Yes"
                        AuthNameNode.Attributes.Append(AtFullWidth)
                    End If
                Catch ex As Exception

                End Try
                If (AuthNameNode.InnerXml <> "") Then
                    If (Infilemame = "11812") Then
                    Else
                        newNd.AppendChild(AuthNameNode)
                    End If

                End If
                If (JournalSubType = "A") Then
                    'Add AffiliationNd
                    Dim JournalTitle_AffAddrNd As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                    Dim atr_Aff As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                    atr_Aff.Value = "articleaddress"
                    JournalTitle_AffAddrNd.Attributes.Append(atr_Aff)
                    Dim AffiliationNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Affiliation[not(.='') and ancestor::AuthorGroup]")
                    If (IsNothing(AffiliationNds) = False And AffiliationNds.Count > 0) Then
                        For i As Integer = 0 To AffiliationNds.Count - 1
                            Dim AffNode As Xml.XmlElement = Xdoc.CreateElement(AffiliationNds(i).Name)
                            Dim superscNd As Xml.XmlNode = Xdoc.CreateElement("Superscript")
                            If (IsNothing(AffiliationNds(i).Attributes.ItemOf("ID")) = False) Then
                                Dim str As String = AffiliationNds(i).Attributes.ItemOf("ID").Value.ToLower.Replace("aff", "").ToString
                                superscNd.InnerText = str
                                AffNode.AppendChild(superscNd)
                            End If
                            If (IsNothing(AffiliationNds(i).SelectSingleNode(".//OrgDivision")) = False) Then
                                AffNode.InnerXml = AffNode.InnerXml + AffiliationNds(i).SelectSingleNode(".//OrgDivision").OuterXml
                            End If
                            If (IsNothing(AffiliationNds(i).SelectSingleNode(".//OrgName")) = False) Then
                                If (IsNothing(AffiliationNds(i).SelectSingleNode(".//OrgDivision")) = False) Then
                                    AffNode.InnerXml = AffNode.InnerXml + ", "
                                End If
                                AffNode.InnerXml = AffNode.InnerXml + AffiliationNds(i).SelectSingleNode(".//OrgName").OuterXml
                            End If
                            If (IsNothing(AffiliationNds(i).SelectSingleNode(".//City")) = False) Then
                                If (IsNothing(AffiliationNds(i).SelectSingleNode(".//OrgName")) = False Or IsNothing(AffiliationNds(i).SelectSingleNode(".//OrgDivision")) = False) Then
                                    AffNode.InnerXml = AffNode.InnerXml + ", "
                                End If
                                AffNode.InnerXml = AffNode.InnerXml + AffiliationNds(i).SelectSingleNode(".//City").OuterXml
                            End If
                            If (IsNothing(AffiliationNds(i).SelectSingleNode(".//Country")) = False) Then
                                If (IsNothing(AffiliationNds(i).SelectSingleNode(".//City")) = False Or IsNothing(AffiliationNds(i).SelectSingleNode(".//OrgName")) = False Or IsNothing(AffiliationNds(i).SelectSingleNode(".//OrgDivision")) = False) Then
                                    AffNode.InnerXml = AffNode.InnerXml + ", "
                                End If
                                AffNode.InnerXml = AffNode.InnerXml + AffiliationNds(i).SelectSingleNode(".//Country").OuterXml
                            End If
                            If (AffNode.InnerXml <> "") Then
                                JournalTitle_AffAddrNd.AppendChild(AffNode)
                            End If
                        Next
                    End If
                    If (JournalTitle_AffAddrNd.InnerXml <> "") Then
                        If (Infilemame = "11812") Then
                            Dim inst As String = newNd.InnerXml
                            newNd.InnerXml = ""
                            newNd.AppendChild(JournalTitle_AffAddrNd)
                            newNd.InnerXml = newNd.InnerXml + inst
                        Else
                            newNd.AppendChild(JournalTitle_AffAddrNd)
                        End If

                    End If
                    If (AuthNameNode.InnerXml <> "") Then
                        If (Infilemame = "11812") Then
                            Dim inxml As String = newNd.InnerXml
                            newNd.InnerXml = ""
                            newNd.AppendChild(AuthNameNode)
                            newNd.InnerXml = newNd.InnerXml + inxml
                        Else
                            newNd.AppendChild(AuthNameNode)
                        End If

                    End If
                End If

            End If
            Dim RootName As String = Xdoc.DocumentElement.Name
            Dim RootNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//" + RootName)
            If (IsNothing(RootNd) = False) Then
                RootNd.InsertBefore(newNd, RootNd.FirstChild)
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CreateJournalTitleFrame()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_CreateArticleCategory()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CreateArticleCategory
        'PARAMETER    :-
        'AIM          :This function create left_article_category and right_article_category node.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim Articlecategory As String = ""
            Try
                Articlecategory = Xdoc.SelectSingleNode(".//ArticleCategory[not(.='')]").InnerText
            Catch ex As Exception
            End Try
            If (Articlecategory <> "") Then
                Dim RightArticlecategory As Xml.XmlElement = Xdoc.CreateElement("cs_text")
                Dim rattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                rattr.Value = "right_article_category"
                RightArticlecategory.Attributes.Append(rattr)
                RightArticlecategory.InnerXml = Articlecategory.Replace("&", "&amp;")

                Dim leftArticlecategory As Xml.XmlElement = Xdoc.CreateElement("cs_text")
                Dim lattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                lattr.Value = "left_article_category"
                leftArticlecategory.Attributes.Append(lattr)
                leftArticlecategory.InnerXml = Articlecategory.Replace("&", "&amp;")
                Xdoc.DocumentElement.AppendChild(RightArticlecategory)
                Xdoc.DocumentElement.AppendChild(leftArticlecategory)
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CreateArticleCategory()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function

    Private Function CreateAuthNd_RunningTitle()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:CreateAuthNd_RunningTitle
        'PARAMETER    :-
        'AIM          :This function add author information in running title.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim Authnodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorName[not(.='') and not(parent::cs_text)]")
            If (Authnodes.Count > 0) Then
                For i As Integer = 0 To Authnodes.Count - 1
                    If (Authnodes.Count = 1) Then
                        If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                            authornamenew += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
                        End If
                    ElseIf (Authnodes.Count = 2) Then
                        If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                            authornamenew += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
                        End If
                        If (i <> Authnodes.Count - 1) Then
                            authornamenew += " & "
                        End If
                    Else
                        If (Authnodes.Count > 2) Then
                            If (i = 0) Then
                                If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                                    authornamenew = Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
                                End If
                                If (i <> Authnodes.Count - 1) Then
                                    authornamenew += " et al."
                                End If
                            End If

                        End If
                    End If
                Next
            End If
            If (authornamenew <> "") Then
                authornamenew = "<cs_text type='authornamenew'>" + authornamenew + "</cs_text>"
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in CreateAuthNd_RunningTitle()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function

    Private Function Fn_CreateRunningTitle()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CreateRunningTitle
        'PARAMETER    :-
        'AIM          :This function create running title node
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim STD_LeftRighthead As String = ""
            Dim OPT_Lefthead As String = ""
            Dim OPT_Righthead As String = ""

            STD_LeftRighthead = GetJournalName()



            OPT_Righthead = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]").InnerXml


            '  RightRunningTitle = "<cs_text type='right_running_title'>" + RightRunningTitle + "</cs_text>"

            'Left running head
            Dim Authnodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorName[not(.='') and not(ancestor::cs_text)]")
            If (IsNothing(Authnodes) = False And Authnodes.Count > 0) Then
                For i As Integer = 0 To Authnodes.Count - 1
                    If (Authnodes.Count = 1) Then
                        'If (IsNothing(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
                        '    OPT_Lefthead += GetAbbrGivenName(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]"))
                        If ((IsNothing(Authnodes(i).SelectNodes(".//GivenName[not(.='')]")) = False) And (Authnodes(i).SelectNodes(".//GivenName[not(.='')]").Count > 0)) Then
                            For g As Integer = 0 To Authnodes(i).SelectNodes(".//GivenName[not(.='')]").Count - 1
                                OPT_Lefthead += GetAbbrGivenName(Authnodes(i).SelectNodes(".//GivenName[not(.='')]")(g))

                            Next
                        End If
                        If (IsNothing(Authnodes(i).SelectSingleNode(".//Particle[not(.='')]")) = False) Then
                            OPT_Lefthead += Authnodes(i).SelectSingleNode(".//Particle[not(.='')]").InnerText + " "
                        End If
                        If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                            OPT_Lefthead += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
                        End If
                    ElseIf (Authnodes.Count = 2) Then
                        ''If (IsNothing(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
                        If ((IsNothing(Authnodes(i).SelectNodes(".//GivenName[not(.='')]")) = False) And (Authnodes(i).SelectNodes(".//GivenName[not(.='')]").Count > 0)) Then
                            For g As Integer = 0 To Authnodes(i).SelectNodes(".//GivenName[not(.='')]").Count - 1
                                OPT_Lefthead += GetAbbrGivenName(Authnodes(i).SelectNodes(".//GivenName[not(.='')]")(g))

                            Next
                            ''  OPT_Lefthead += GetAbbrGivenName(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]"))
                        End If
                        If (IsNothing(Authnodes(i).SelectSingleNode(".//Particle[not(.='')]")) = False) Then
                            OPT_Lefthead += Authnodes(i).SelectSingleNode(".//Particle[not(.='')]").InnerText + " "
                        End If
                        If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                            OPT_Lefthead += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
                        End If
                        If (i <> Authnodes.Count - 1) Then
                            If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
                                If (Article_Lg.ToLower = "de") Then
                                    OPT_Lefthead += " und "
                                Else
                                    OPT_Lefthead += " and "
                                End If

                            Else
                                OPT_Lefthead += ", "
                            End If

                        End If
                    Else
                        'If (IsNothing(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
                        'OPT_Lefthead += GetAbbrGivenName(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]"))
                        If ((IsNothing(Authnodes(i).SelectNodes(".//GivenName[not(.='')]")) = False) And (Authnodes(i).SelectNodes(".//GivenName[not(.='')]").Count > 0)) Then
                            For g As Integer = 0 To Authnodes(i).SelectNodes(".//GivenName[not(.='')]").Count - 1
                                OPT_Lefthead += GetAbbrGivenName(Authnodes(i).SelectNodes(".//GivenName[not(.='')]")(g))

                            Next
                        End If
                        If (IsNothing(Authnodes(i).SelectSingleNode(".//Particle[not(.='')]")) = False) Then
                            OPT_Lefthead += Authnodes(i).SelectSingleNode(".//Particle[not(.='')]").InnerText + " "
                        End If
                        If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                            OPT_Lefthead += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText + " et al."
                            Exit For
                        End If
                    End If
                Next

            End If

            '           LeftRunningTitle = "<cs_text type='left_running_title'>" + LeftRunningTitle + "</cs_text>"

            If (GlobalRunningOption.ToLower = "") Then
                LeftRunningTitle = "<cs_text type='left_running_title'>RunningHead Text Missing</cs_text>"
                RightRunningTitle = "<cs_text type='right_running_title'>RunningHead Text Missing</cs_text>"
            End If
            If (GlobalRunningOption.ToLower = "option" Or GlobalRunningOption.ToLower = "nonstandard") Then
                LeftRunningTitle = "<cs_text type='left_running_title'>" + OPT_Lefthead + "</cs_text>"
                RightRunningTitle = "<cs_text type='right_running_title'>" + OPT_Righthead + "</cs_text>"
            End If
            If (GlobalRunningOption.ToLower = "standard") Then
                LeftRunningTitle = "<cs_text type='left_running_title'>" + STD_LeftRighthead + "</cs_text>"
                RightRunningTitle = "<cs_text type='right_running_title'>" + STD_LeftRighthead + "</cs_text>"
            End If

            If (LeftRunningTitle <> "") Then
                Dim node As Xml.XmlNode = Xdoc.CreateElement("test")
                node.InnerXml = LeftRunningTitle
                Xdoc.DocumentElement.AppendChild(node.FirstChild)
            End If
            If (RightRunningTitle <> "") Then
                Dim node As Xml.XmlNode = Xdoc.CreateElement("test")
                node.InnerXml = RightRunningTitle
                Xdoc.DocumentElement.AppendChild(node.FirstChild)
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CreateRunningTitle()" + vbNewLine)
            CLog.LogMessages("ArticleInfo doesn't contain ArticleTitle node with language '" + Article_Lg + "' which is necessary for creating A")
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function


    ''Private Function Fn_CreateRunningTitle()
    ''    '=============================================================================================================
    ''    '=============================================================================================================
    ''    'FUNCTION NAME:Fn_CreateRunningTitle
    ''    'PARAMETER    :-
    ''    'AIM          :This function create running title node
    ''    '=============================================================================================================
    ''    '=============================================================================================================
    ''    Try
    ''        'Right running head
    ''        Dim BSLRunning_Title As String = ""
    ''        BSLRunning_Title = GetJournalName()
    ''        RightRunningTitle = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]").InnerXml

    ''        If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
    ''            RightRunningTitle = BSLRunning_Title
    ''        End If


    ''        RightRunningTitle = "<cs_text type='right_running_title'>" + RightRunningTitle + "</cs_text>"

    ''        'Left running head
    ''        Dim Authnodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorName[not(.='') and not(ancestor::cs_text)]")
    ''        If (IsNothing(Authnodes) = False And Authnodes.Count > 0) Then
    ''            For i As Integer = 0 To Authnodes.Count - 1
    ''                If (Authnodes.Count = 1) Then
    ''                    If (IsNothing(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
    ''                        LeftRunningTitle += GetAbbrGivenName(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) 'Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]").InnerText + " "
    ''                    End If
    ''                    If (IsNothing(Authnodes(i).SelectSingleNode(".//Particle[not(.='')]")) = False) Then
    ''                        LeftRunningTitle += Authnodes(i).SelectSingleNode(".//Particle[not(.='')]").InnerText + " "
    ''                    End If
    ''                    If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
    ''                        LeftRunningTitle += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
    ''                    End If
    ''                ElseIf (Authnodes.Count = 2) Then
    ''                    If (IsNothing(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
    ''                        LeftRunningTitle += GetAbbrGivenName(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) 'Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]").InnerText + " "
    ''                    End If
    ''                    If (IsNothing(Authnodes(i).SelectSingleNode(".//Particle[not(.='')]")) = False) Then
    ''                        LeftRunningTitle += Authnodes(i).SelectSingleNode(".//Particle[not(.='')]").InnerText + " "
    ''                    End If
    ''                    If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
    ''                        LeftRunningTitle += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
    ''                    End If
    ''                    If (i <> Authnodes.Count - 1) Then
    ''                        If (LayoutnameDB.ToLower = "vsandgabler" Or LayoutnameDB.ToLower = "vs-verlag") Then
    ''                            If (Article_Lg.ToLower = "de") Then
    ''                                LeftRunningTitle += " und "
    ''                            Else
    ''                                LeftRunningTitle += " and "
    ''                            End If

    ''                        Else
    ''                            LeftRunningTitle += ", "
    ''                        End If

    ''                    End If
    ''                Else
    ''                    If (IsNothing(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
    ''                        LeftRunningTitle += GetAbbrGivenName(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) 'Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]").InnerText + " "
    ''                    End If
    ''                    If (IsNothing(Authnodes(i).SelectSingleNode(".//Particle[not(.='')]")) = False) Then
    ''                        LeftRunningTitle += Authnodes(i).SelectSingleNode(".//Particle[not(.='')]").InnerText + " "
    ''                    End If
    ''                    If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
    ''                        LeftRunningTitle += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText + " et al."
    ''                        Exit For
    ''                    End If
    ''                End If
    ''            Next

    ''        End If

    ''        If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
    ''            LeftRunningTitle = BSLRunning_Title
    ''        End If

    ''        LeftRunningTitle = "<cs_text type='left_running_title'>" + LeftRunningTitle + "</cs_text>"


    ''        If (Infilemame.Trim.ToLower = "12471" Or Infilemame.Trim.ToLower = "767") Then
    ''            LeftRunningTitle = GetJournalName()
    ''            RightRunningTitle = LeftRunningTitle
    ''            LeftRunningTitle = "<cs_text type='left_running_title'>" + LeftRunningTitle + "</cs_text>"
    ''            RightRunningTitle = "<cs_text type='right_running_title'>" + RightRunningTitle + "</cs_text>"
    ''        End If

    ''        If (LeftRunningTitle <> "") Then
    ''            Dim node As Xml.XmlNode = Xdoc.CreateElement("test")
    ''            node.InnerXml = LeftRunningTitle
    ''            Xdoc.DocumentElement.AppendChild(node.FirstChild)
    ''        End If
    ''        If (RightRunningTitle <> "") Then
    ''            Dim node As Xml.XmlNode = Xdoc.CreateElement("test")
    ''            node.InnerXml = RightRunningTitle
    ''            Xdoc.DocumentElement.AppendChild(node.FirstChild)
    ''        End If
    ''    Catch ex As Exception
    ''        CLog.LogMessages("Error in Fn_CreateRunningTitle()" + vbNewLine)
    ''        CLog.LogMessages("ArticleInfo doesn't contain ArticleTitle node with language '" + Article_Lg + "' which is necessary for creating A")
    ''        CLog.LogMessages(ex.Message.ToString + vbNewLine)
    ''        Throw
    ''    End Try
    ''    '====================================================END======================================================
    ''    '=============================================================================================================
    ''End Function
    Private Function Fn_CreateRunningFooter()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CreateRunningFooter
        'PARAMETER    :-
        'AIM          :This function create running footer.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim journalnamenew As String = ""

            CreateAuthNd_RunningTitle()

            Dim FirstJournalname As Xml.XmlNode = Xdoc.CreateElement("cs_text")
            Dim att As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            att.Value = "firstJournalname"
            FirstJournalname.Attributes.Append(att)
            Dim JournalName As String = GetJournalName()
            If (JournalName = "") Then 'BHM, Jvn. Jg. (2013), Heft XX
                FirstJournalname.InnerXml = "<Emphasis Type='Bold'>JournalName</Emphasis>" + ",&#160;Jvn." + "&#160;" + "Jg." + "&#160;" + "(" + Xdoc.SelectSingleNode(".//Year").InnerText + ")" + ",&#160;" + "Heft XX"
            Else
                If (JournalName = "JournalName") Then
                    FirstJournalname.InnerXml = "<Emphasis Type='Bold'>JournalName</Emphasis>" + ",&#160;Jvn." + "&#160;" + "Jg." + "&#160;" + "(" + Xdoc.SelectSingleNode(".//Year").InnerText + ")" + ",&#160;" + "Heft XX"
                Else
                    FirstJournalname.InnerXml = "<Emphasis Type='Bold'>" + JournalName + "</Emphasis>" + ",&#160;Jvn." + "&#160;" + "Jg." + "&#160;" + "(" + Xdoc.SelectSingleNode(".//Year").InnerText + ")" + ",&#160;" + "Heft XX"
                End If
            End If
            journalnamenew = FirstJournalname.InnerXml
            If (journalnamenew <> "") Then
                journalnamenew = "<cs_text type='journalnamenew'>" + journalnamenew + "</cs_text>"
            End If

            LeftRunnignFooter = Xdoc.CreateElement("cs_text")
            Dim lattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            lattr.Value = "left_running_footer"
            LeftRunnignFooter.Attributes.Append(lattr)
            If (JournalSubType = "A") Then
                LeftRunnignFooter.InnerXml = authornamenew.Replace("&", "&amp;") + vbTab + "© Springer-Verlag Wien" + vbTab + journalnamenew.Replace("&", "&amp;")
                If (authornamenew = "") Then
                    LeftRunnignFooter.InnerXml = "© Springer-Verlag Wien" + vbTab + vbTab + journalnamenew.Replace("&", "&amp;")
                End If
            Else
                LeftRunnignFooter.InnerXml = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]").InnerXml ' authornamenew.Replace("&", "&amp;") + vbTab + "© Springer-Verlag" + vbTab + journalnamenew.Replace("&", "&amp;")
            End If
            RightRunnignFooter = Xdoc.CreateElement("cs_text")
            Dim rattr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            rattr.Value = "right_running_footer"
            RightRunnignFooter.Attributes.Append(rattr)
            If (JournalSubType = "A") Then
                RightRunnignFooter.InnerXml = journalnamenew.Replace("&", "&amp;") + vbTab + "© Springer-Verlag Wien" + vbTab + authornamenew.Replace("&", "&amp;")
                If (authornamenew = "") Then
                    RightRunnignFooter.InnerXml = journalnamenew.Replace("&", "&amp;") + vbTab + vbTab + "© Springer-Verlag Wien"
                End If
            Else
                RightRunnignFooter.InnerXml = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]").InnerXml 'journalnamenew.Replace("&", "&amp;") + vbTab + "© Springer-Verlag" + vbTab + authornamenew.Replace("&", "&amp;")
            End If
            Xdoc.DocumentElement.AppendChild(LeftRunnignFooter)
            Xdoc.DocumentElement.AppendChild(RightRunnignFooter)
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CreateRunningFooter()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_CreateDOIFrame()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CreateDOIFrame
        'PARAMETER    :-
        'AIM          :This function create DOI frame.
        '              If JournalSubType="A" then DOI frame contain two line else if type B then it contain three 
        '              line code.
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim DOIFrame As Xml.XmlNode = Xdoc.CreateElement("cs_text")
            Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            attr.Value = "doiframe"
            DOIFrame.Attributes.Append(attr)

            Dim FirstInnerFrame As Xml.XmlNode = Xdoc.CreateElement("cs_text")
            Dim attr1 As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            attr1.Value = "firstlinedetail"
            FirstInnerFrame.Attributes.Append(attr1)

            If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                Dim atr_italic As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
                atr_italic.Value = "italic"
                FirstInnerFrame.Attributes.Append(atr_italic)
            End If
            Dim JournalName As String = GetJournalName()
            If (JournalSubType = "A") Then
                If (JournalName = "") Then
                    FirstInnerFrame.InnerXml = "JournalName" + vbNewLine
                Else
                    FirstInnerFrame.InnerXml = JournalName + vbNewLine
                End If
            End If

            If (JournalSubType = "B") Then
                If (JournalName = "") Then
                    If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                        FirstInnerFrame.InnerXml = "JournalName, "
                    Else
                        FirstInnerFrame.InnerXml = "JournalName" + vbNewLine
                    End If

                Else
                    If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                        FirstInnerFrame.InnerXml = JournalName + ", "
                    Else
                        FirstInnerFrame.InnerXml = JournalName + vbNewLine ' + "&#160;" + "(" + Xdoc.SelectSingleNode(".//Year").InnerText + ")"  + "&#160;" + "[jvn]:[afp]" + "&#8211;" + "[alp]" + vbNewLine
                    End If

                    If (Infilemame = "16024") Then
                        'If (JournalName.ToLower = "hbscience") Then
                        '    FirstInnerFrame.InnerXml = ""
                        'End If
                        FirstInnerFrame.InnerXml = ""
                    End If
                End If
            End If

            DOIFrame.AppendChild(FirstInnerFrame)
            '=============================================================================
            Dim SecondInnerFrame As Xml.XmlNode = Xdoc.CreateElement("cs_text")
            Dim attr2 As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
            attr2.Value = "secondlinedetail"
            SecondInnerFrame.Attributes.Append(attr2)

            If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                SecondInnerFrame.InnerXml = "DOI: " + Xdoc.SelectSingleNode(".//ArticleDOI").InnerText
            Else
                SecondInnerFrame.InnerXml = "DOI " + Xdoc.SelectSingleNode(".//ArticleDOI").InnerText
            End If

            If (SecondInnerFrame.InnerXml <> "") Then
                DOIFrame.AppendChild(SecondInnerFrame)
            End If
            '=============================================================================
            If (JournalSubType = "A" Or LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                Dim ThirdInnerFrame As Xml.XmlNode = Xdoc.CreateElement("cs_text")
                Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
                ThirdInnerFrame.Attributes.Append(atr)
                atr.Value = "thirdlinedetail"
                Dim copyrightholdername As String = ""
                Dim CopyrightYear As String = ""
                If (BOpenChoice = "") Then
                    Dim Articlecopyrightholdername As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName")
                    If (IsNothing(Articlecopyrightholdername) = False) Then
                        copyrightholdername = Articlecopyrightholdername.InnerText
                    Else
                        copyrightholdername = ""
                    End If

                    Dim ArticleCopyrightYear As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear")
                    If (IsNothing(ArticleCopyrightYear) = False) Then
                        CopyrightYear = ArticleCopyrightYear.InnerText
                    Else
                        CopyrightYear = ""
                    End If

                    ThirdInnerFrame.InnerXml += "© " + copyrightholdername + " " + CopyrightYear

                Else
                    If (BOpenChoice.ToLower = "openchoice") Then
                        ' ThirdInnerFrame.InnerText += vbNewLine + "© The Author(s) " + CStr(System.DateTime.Now.Year) + "." + " " + OpenChoiceCopyrightNd.InnerText
                        ThirdInnerFrame.InnerText += vbNewLine + "© " + +Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName").InnerText + " " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear").InnerText + "." + " " + OpenChoiceCopyrightNd.InnerText
                    Else
                        'ThirdInnerFrame.InnerXml = "© " + "Springer-Verlag" + " " + CStr(System.DateTime.Now.Year)
                        ThirdInnerFrame.InnerXml = "© " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightHolderName").InnerText + " " + Xdoc.SelectSingleNode(".//Article/ArticleInfo/ArticleCopyright/CopyrightYear").InnerText
                    End If
                End If
                If (ThirdInnerFrame.InnerXml <> "") Then
                    ThirdInnerFrame.InnerXml = vbNewLine + ThirdInnerFrame.InnerXml
                    DOIFrame.AppendChild(ThirdInnerFrame)
                End If
            End If
            '=============================================================================
            If (DOIFrame.InnerXml <> "") Then
                Dim Root As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleBackmatter")
                If (IsNothing(Root) = False) Then
                Else
                    Root = Xdoc.SelectSingleNode(".//Body")
                End If
                Root.ParentNode.InsertAfter(DOIFrame, Root)
            End If
        Catch ex As Exception
            CLog.LogMessages("Error in Fn_CreateDOIFrame()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Editmetadata(ByVal str As String, ByVal chapterNds As Xml.XmlNode) As String
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Editmetadata
        'PARAMETER    :str,chapterNds
        'AIM          :This function update metadata node
        '=============================================================================================================
        '=============================================================================================================

        oReq.DeleteNode(".//Abstract//processing-instruction('HIDEINDEX')")
        Try
            Dim Article_Lg As String = ""
            Dim Abstract As String = ""
            Dim ArticleInfoLg As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article")
            If (IsNothing(Xdoc.SelectSingleNode(".//ArticleInfo/@Language")) = False) Then
                Article_Lg = Xdoc.SelectSingleNode(".//ArticleInfo/@Language").InnerText
            Else
                Article_Lg = Xdoc.SelectSingleNode(".//Article/@Language").InnerText
            End If

            If (IsNothing(chapterNds.SelectSingleNode(".//Abstract[@Language='" + Article_Lg + "'  and not(.='')]")) = False) Then
                Dim Articlenode As Xml.XmlNodeList = chapterNds.SelectNodes(".//Abstract")
                Dim str1 As String = ""
                Dim type As String = ""
                For i As Integer = 0 To Articlenode.Count - 1
                    '   type = Articlenode(i).ChildNodes(0).InnerText
                    type = Articlenode(i).SelectSingleNode(".//Heading").InnerText
                    str1 += "<M_Abstract type='" + type + "'>" + Articlenode(i).InnerXml + "</M_Abstract>"
                Next
                Abstract = str1

                If (IsNothing(chapterNds.SelectSingleNode(".//Abstract[@Language='" + Article_Lg + "'  and not(.='')]/Heading[not(.='')]")) = False) Then
                    Dim s As Regex
                    Dim Str_Abstract As String = chapterNds.SelectSingleNode(".//Abstract").InnerText

                    Str_Abstract = Str_Abstract.Replace("\", "\\")
                    Str_Abstract = Str_Abstract.Replace("[", "\[")
                    Str_Abstract = Str_Abstract.Replace("]", "\]")

                    Str_Abstract = Str_Abstract.Replace("(", "\(")
                    Str_Abstract = Str_Abstract.Replace(")", "\)")

                    'Str_Abstract = Str_Abstract.Replace("<", "\<")
                    'Str_Abstract = Str_Abstract.Replace(">", "\>")
                    'Str_Abstract = Str_Abstract.Replace("%", "\%")
                    'Str_Abstract = Str_Abstract.Replace(".", "\.")



                    s = New Regex(Str_Abstract, RegexOptions.Singleline)
                    Abstract = s.Replace(Abstract, "", 1)
                    s = New Regex("<cs_text> </cs_text>", RegexOptions.Singleline)
                    Abstract = s.Replace(Abstract, "", 1)
                    Abstract = Regex.Replace(Abstract, "Abstract</", "</", RegexOptions.Singleline)
                    Abstract = Regex.Replace(Abstract, "Zusammenfassung</", "</", RegexOptions.Singleline)
                    Abstract = Regex.Replace(Abstract, "Zusammenfassung <!--", " <!--", RegexOptions.Singleline) '' added by suvarna on 09.05.2012
                    Abstract = Regex.Replace(Abstract, "Summary</", "</", RegexOptions.Singleline)
                End If
            Else
                Dim Articlenode As Xml.XmlNodeList = chapterNds.SelectNodes(".//Abstract")
                Dim str1 As String = ""
                Dim type As String = ""
                For i As Integer = 0 To Articlenode.Count - 1
                    type = Articlenode(i).ChildNodes(0).InnerText
                    str1 += "<M_Abstract type='" + type + "'>" + Articlenode(i).InnerXml + "</M_Abstract>"
                Next
                Abstract = str1

                If (IsNothing(chapterNds.SelectSingleNode(".//Abstract[@Language='" + Article_Lg + "'  and not(.='')]/Heading[not(.='')]")) = False) Then
                    Dim s As Regex
                    s = New Regex(chapterNds.SelectSingleNode(".//Abstract").InnerText, RegexOptions.Singleline)
                    Abstract = s.Replace(Abstract, "", 1)
                    s = New Regex("<cs_text> </cs_text>", RegexOptions.Singleline)
                    Abstract = s.Replace(Abstract, "", 1)
                    Abstract = Regex.Replace(Abstract, "Abstract</", "</", RegexOptions.Singleline)
                    Abstract = Regex.Replace(Abstract, "Zusammenfassung</", "</", RegexOptions.Singleline)
                    Abstract = Regex.Replace(Abstract, "Zusammenfassung <!--", " <!--", RegexOptions.Singleline) '' added by suvarna on 09.05.2012
                    Abstract = Regex.Replace(Abstract, "Summary</", "</", RegexOptions.Singleline)
                End If

            End If

            If (Abstract <> "") Then
                If (Article_Lg.ToLower = "de") Then
                    str = Regex.Replace(str, "<M_Abstract([^>]*)>(.+)</M_Abstract>", Abstract, RegexOptions.Multiline)
                Else
                    str = Regex.Replace(str, "<M_Abstract([^>]*)>(.+)</M_Abstract>", Abstract, RegexOptions.Multiline)
                End If
            Else
                If (Article_Lg.ToLower = "de") Then
                    Abstract = "<M_Abstract type='Zusammenfassung'>" + "NA" + "</M_Abstract>"
                Else
                    Abstract = "<M_Abstract type='Abstract'>" + "NA" + "</M_Abstract>"
                End If
            End If
            XMLM_Data2 = str
        Catch ex As Exception
            CLog.LogMessages("Error in Editmetadata()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        Return XMLM_Data2
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function colorinPrintInfo()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:colorinPrintInfo
        'PARAMETER    :-
        'AIM          :This function return the color print information.
        '=============================================================================================================
        '=============================================================================================================
        Dim colorinPrint As Boolean = False
        Try
            colorinPrint = oReq.oFigObj.getcolorinprintInfo(FigureConversionFile, INXMLName)
        Catch ex As Exception
            CLog.LogMessages("Error in colorinPrintInfo()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        Return colorinPrint
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function GetJournalName()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetJournalName
        'PARAMETER    :-
        'AIM          :This function return the journal name to display in Article DOI Frame
        '=============================================================================================================
        '=============================================================================================================
        Dim JournalName As String = ""
        Try
            JournalName = oReq.oFigObj.GetJournalNamePath(FigureConversionFile, INXMLName)
            Return JournalName
        Catch ex As Exception
            CLog.LogMessages("Error in GetJournalName()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '===============================================Function End==================================================
    End Function
    Public Function GetJournalTitle()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetJournalTitle
        'PARAMETER    :-
        'AIM          :This function return the journal title to display in Article DOI Frame
        '=============================================================================================================
        '=============================================================================================================
        Dim JournalTitle As String = ""
        Try
            JournalTitle = oReq.oFigObj.GetJournalTitle(FigureConversionFile, INXMLName)
            Return JournalTitle
        Catch ex As Exception
            CLog.LogMessages("Error in GetJournalTitle()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            Throw
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
End Module

