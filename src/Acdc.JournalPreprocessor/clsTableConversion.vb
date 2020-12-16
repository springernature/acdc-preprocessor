'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : 
'MODULE NAME   : clsTableConversion
'CREATED DATE  : 3RD JUNE 2013
'LAST MODIFIED : 14TH JUNE 2013
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.IO
Public Class clsTableConversion
    Private XmlFile As String
    Private XmlStr As String
    Public XDoc As New Xml.XmlDocument
    Private NTable As Xml.NameTable
    Private NS As Xml.XmlNamespaceManager
    Private NSht As New Hashtable
    Private TableSettings As New Xml.XmlDocument

    Private TableStructureName As String
    Private tableTagName As String
    Private tableHeaderCellPath As String
    Private TableCaptionPath As String
    Private TableBodyCellPath As String
    Private TableFooterCellPath As String
    Private TableCapPath As String
    Private TableRowXPath As String
    Private TableColumnXPathWRTrow As String
    Private RowTagName As String
    Private CellTagName As String
    Private TableCoulumInformationXPath As String


    Private ColWidthAtt As String = "colwidth"
    Private ColnameAttr As String = "colname"
    Private ColnumAttr As String = "columnnum"
    Private mergedColStart As String = "namest"
    Private MergedColEnd As String = "nameend"
    Private MoreRowsArr As String = "morerows"
    Private oreq As clsPreprocMain
    Public Sub New(ByRef Xddoc As XmlDocument, ByVal nmtab As NameTable, ByVal NSm As XmlNamespaceManager, ByVal oreqq As clsPreprocMain)
        XDoc.PreserveWhitespace = True
        XDoc = Xddoc
        NTable = nmtab
        NS = NSm
        oreq = oreqq
    End Sub
    Private Sub GetTableSettings(ByVal TableSettingsXMl As String)
        TableSettings.LoadXml(TableSettingsXMl)

        TableStructureName = TableSettings.SelectSingleNode("TableIdentification/TableStructure/@names").Value
        tableTagName = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableTagNames").InnerText

        tableHeaderCellPath = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableHeaderCellPath").InnerText

        TableCaptionPath = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableCaptionPaths/TableCapPath").InnerText

        TableBodyCellPath = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableBodyCellPath").InnerText

        TableFooterCellPath = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableFooterCellPath").InnerText

        TableCapPath = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableCaptionPaths/TableCapPath").InnerText

        TableRowXPath = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableRowTagXpath").InnerText

        TableColumnXPathWRTrow = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableColumnTagXpathWRTrow").InnerText

        RowTagName = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableRowTagXpath/@tagname").Value

        CellTagName = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableColumnTagXpathWRTrow/@tagname").Value

        TableCoulumInformationXPath = TableSettings.SelectSingleNode("TableIdentification/TableStructure/TableCoulumInformationXPath").InnerText


    End Sub
    Public Sub AddRowIndex(ByVal currTableStructure As Xml.XmlNode)
        Dim TableHeadNd As Xml.XmlNodeList = currTableStructure.SelectNodes(".//tgroup/thead/row")
        Dim TableBodyNd As Xml.XmlNodeList = currTableStructure.SelectNodes(".//tgroup/tbody/row")
        For i As Integer = 0 To TableHeadNd.Count - 1
            Try
                Dim currHeadNode As XmlNode = TableHeadNd(i)

                Dim entryNd As Xml.XmlNodeList = currHeadNode.SelectNodes(".//entry")
                For Each entNd As Xml.XmlNode In entryNd
                    Dim NewAttr As Xml.XmlAttribute = XDoc.CreateAttribute("cs_rowindex")
                    NewAttr.Value = i + 1
                    entNd.Attributes.Append(NewAttr)
                Next
            Catch ex As Exception

            End Try
        Next
        For i As Integer = 0 To TableBodyNd.Count - 1
            Try
                Dim currBodyNode As XmlNode = TableBodyNd(i)

                Dim entryNd As Xml.XmlNodeList = currBodyNode.SelectNodes(".//entry")
                For Each entNd As Xml.XmlNode In entryNd
                    Dim NewAttr As Xml.XmlAttribute = XDoc.CreateAttribute("cs_rowindex")
                    NewAttr.Value = i + 1
                    entNd.Attributes.Append(NewAttr)
                Next

            Catch ex As Exception

            End Try
        Next



    End Sub
    Public Sub AddMergeColName(ByVal currTableStructure As Xml.XmlNode)

        Dim AllMergecols As Xml.XmlNodeList = currTableStructure.SelectNodes(".//Cell/entry[@namest]")
        For Each MergeNode As Xml.XmlNode In AllMergecols
            Dim Namest As String = ""
            Dim Nameend As String = ""
            Dim NamestVal As String = ""
            Dim NameendVal As String = ""
            Try
                Namest = MergeNode.Attributes.ItemOf("namest").Value
                Nameend = MergeNode.Attributes.ItemOf("nameend").Value
                NamestVal = Regex.Match(Namest, "c(\d+)", RegexOptions.Singleline).Groups(1).Value
                NameendVal = Regex.Match(Nameend, "c(\d+)", RegexOptions.Singleline).Groups(1).Value
                Dim NewAttr As Xml.XmlAttribute = XDoc.CreateAttribute("colname")
                If (NamestVal = 1) Then
                    NewAttr.Value = Namest
                    MergeNode.Attributes.Append(NewAttr)

                Else
                    NewAttr.Value = Nameend
                    MergeNode.Attributes.Append(NewAttr)
                End If

            Catch ex As Exception
            End Try
        Next
    End Sub
    Public Sub TableConversion(ByVal TableSettingsXMl As String)

        GetTableSettings(TableSettingsXMl)

        Dim TableStructureNodes As Xml.XmlNodeList = XDoc.SelectNodes(".//" + TableStructureName, NS)

        ' is their any possibility that any their could be previously table are having ID 
        ' if yes then get the heighest
        Dim AllNewTableNodes As New ArrayList
        For i As Integer = 0 To TableStructureNodes.Count - 1
            Try
                Try
                    Dim Nd As Xml.XmlNode = TableStructureNodes(i).SelectSingleNode(".//tbody/row[position()=1]")
                    Dim entryNds As Xml.XmlNodeList = Nd.SelectNodes(".//entry")
                    For k As Integer = 0 To entryNds.Count - 1
                        Dim atr As Xml.XmlAttribute = XDoc.CreateAttribute("rowposition")
                        atr.Value = "first"
                        entryNds(k).Attributes.Append(atr)
                    Next
                Catch ex As Exception

                End Try

                Try
                    Dim Nd As Xml.XmlNode = TableStructureNodes(i).SelectSingleNode(".//tbody/row[position()=last()]")
                    Dim entryNds As Xml.XmlNodeList = Nd.SelectNodes(".//entry")
                    For k As Integer = 0 To entryNds.Count - 1
                        Dim atr As Xml.XmlAttribute = XDoc.CreateAttribute("rowposition")
                        atr.Value = "last"
                        entryNds(k).Attributes.Append(atr)
                    Next
                Catch ex As Exception

                End Try

                Try
                    Dim Nd As Xml.XmlNode = TableStructureNodes(i).SelectSingleNode(".//thead/row[position()=last()]")
                    Dim entryNds As Xml.XmlNodeList = Nd.SelectNodes(".//entry")
                    For k As Integer = 0 To entryNds.Count - 1
                        Dim atr As Xml.XmlAttribute = XDoc.CreateAttribute("rowposition")
                        atr.Value = "last"
                        entryNds(k).Attributes.Append(atr)
                    Next
                Catch ex As Exception

                End Try

                Try
                    'find rowcount (cnt)
                    'goto each table head row 
                    'find row posi every time (pos)
                    'if (pos)+(Morerows)=(cnt)
                    Dim tb_cnt As Integer = 0
                    If (IsNothing(TableStructureNodes(i).SelectNodes(".//thead/row")) = False And TableStructureNodes(i).SelectNodes(".//thead/row").Count > 0) Then
                        tb_cnt = TableStructureNodes(i).SelectNodes(".//thead/row").Count


                        For g As Integer = 0 To TableStructureNodes(i).SelectNodes(".//thead/row").Count - 1
                            Dim Nd_tb As Xml.XmlNode = TableStructureNodes(i).SelectNodes(".//thead/row")(g)
                            Dim rowpos_tb As Integer = CInt(g + 1)
                            Dim entryNds As Xml.XmlNodeList = Nd_tb.SelectNodes(".//entry")
                            For kk As Integer = 0 To entryNds.Count - 1
                                If (IsNothing(entryNds(kk).Attributes.ItemOf("morerows")) = False) Then
                                    Dim leftval As Integer = CInt(entryNds(kk).Attributes.ItemOf("morerows").Value) + rowpos_tb
                                    Dim rightval As Integer = tb_cnt
                                    If (leftval = rightval) Then
                                        Dim atr As Xml.XmlAttribute = XDoc.CreateAttribute("rowposition")
                                        atr.Value = "last"
                                        entryNds(kk).Attributes.Append(atr)
                                    End If
                                End If
                            Next
                        Next
                    End If




                Catch ex As Exception

                End Try
                Try
                    'find rowcount (cnt)
                    'goto each table row 
                    'find row posi every time (pos)
                    'if (pos)+(Morerows)=(cnt)
                    Dim tb_cnt As Integer = 0
                    If (IsNothing(TableStructureNodes(i).SelectNodes(".//tbody/row")) = False And TableStructureNodes(i).SelectNodes(".//tbody/row").Count > 0) Then
                        tb_cnt = TableStructureNodes(i).SelectNodes(".//tbody/row").Count


                        For g As Integer = 0 To TableStructureNodes(i).SelectNodes(".//tbody/row").Count - 1
                            Dim Nd_tb As Xml.XmlNode = TableStructureNodes(i).SelectNodes(".//tbody/row")(g)
                            Dim rowpos_tb As Integer = CInt(g + 1)
                            Dim entryNds As Xml.XmlNodeList = Nd_tb.SelectNodes(".//entry")
                            For kk As Integer = 0 To entryNds.Count - 1
                                If (IsNothing(entryNds(kk).Attributes.ItemOf("morerows")) = False) Then
                                    Dim leftval As Integer = CInt(entryNds(kk).Attributes.ItemOf("morerows").Value) + rowpos_tb
                                    Dim rightval As Integer = tb_cnt
                                    If (leftval = rightval) Then
                                        Dim atr As Xml.XmlAttribute = XDoc.CreateAttribute("rowposition")
                                        atr.Value = "last"
                                        entryNds(kk).Attributes.Append(atr)
                                    End If
                                End If
                            Next
                        Next
                    End If




                Catch ex As Exception

                End Try
                Dim currTableStructure As XmlNode = TableStructureNodes(i)
                Dim TempCols As String = GetColumnCount(currTableStructure, TableRowXPath, TableColumnXPathWRTrow)
                GetColSpecWidth(currTableStructure, TempCols)

                AddRowIndex(currTableStructure)
                Dim NewTableNode As Xml.XmlNode = XDoc.CreateElement("cs:table", NSht("cs"))
                For Each atr As Xml.XmlAttribute In currTableStructure.Attributes
                    Dim NewAttr As Xml.XmlAttribute = XDoc.CreateAttribute(atr.Name)
                    NewAttr.Value = atr.Value
                    NewTableNode.Attributes.Append(NewAttr)
                Next
                '**************************************
                Dim TableNd As Xml.XmlNode = Nothing
                '*************************************

                If TableStructureName = tableTagName Then
                    TableNd = currTableStructure
                Else
                    'TableNd = currTableStructure.SelectSingleNode(".//Table[not(@Float='No')]", NS)
                    TableNd = currTableStructure.SelectSingleNode(".//" + tableTagName, NS)
                End If

                Dim Columns As String = GetColumnCount(TableNd, TableRowXPath, TableColumnXPathWRTrow)


                'Dim rows As String = CStr(CInt(GetRowCount(TableNd, RowTagName)) + 1 + 1)
                Dim rows As String = CStr(CInt(GetRowCount(TableNd, RowTagName)))
                Dim TableCaptionNode As Xml.XmlNode = currTableStructure.SelectSingleNode(TableCapPath)
                Dim TableCaptionOuterXml As String = ""
                Try
                    TableCaptionOuterXml = TableCaptionNode.OuterXml
                Catch ex As Exception
                End Try

                Dim TableCaption As String = ""
                If TableCaptionOuterXml <> "" Then
                    rows += 1
                    Columns = TableNd.SelectSingleNode(".//tgroup/@cols").Value
                    'TableCaption = "<cs:text type=""tablecaption"" req=""yes"" xmlns:cs=""http://www.crest-premedia.in"">" + TableCaptionOuterXml + "</cs:text>"
                    TableCaption = "<Cell aid:table=""cell"" aid:ccolwidth="""" aid:crows=""" + CStr(1) + """ aid:ccols=""" + Columns + """ cs_type=""tablecaption"" xmlns:cs=""http://www.crest-premedia.in""><entry>" + TableCaptionOuterXml + "</entry></Cell>"
                    '"<Cell " + "aid:table=""cell"" aid:ccolwidth=""" + CellProp(ColWidthAtt) + """ aid:crows=""" + CellProp(MoreRowsArr) + """ aid:ccols=""" + CellProp("ColCount") + """ cs_type=""" + xp + """>" + CellOuterXml + "</Cell>"
                End If

                ' Dim TableStart As String = "<Story><Table xmlns:aid=""http://ns.adobe.com/AdobeInDesign/4.0/"" aid:table=""table"" aid:trows=""" + rows + """ aid:tcols=""" + Columns + """>"

                Dim CellsXPAL As New ArrayList
                CellsXPAL.Add(tableHeaderCellPath)
                CellsXPAL.Add(TableBodyCellPath)
                'CellsXPAL.Add(TableFooterCellPath)
                Dim CellXml As String = GetCellXml(TableNd, CellsXPAL, CellTagName, currTableStructure, TableCoulumInformationXPath)


                Dim TableFooterNode As XmlNode = currTableStructure.SelectSingleNode(TableFooterCellPath)
                Dim TableFooterOuterXml As String = ""
                Try
                    TableFooterOuterXml = TableFooterNode.OuterXml
                Catch ex As Exception
                End Try

                Dim TableFooter As String = ""
                If TableFooterOuterXml <> "" Then
                    rows += 1
                    Columns = TableNd.SelectSingleNode(".//tgroup/@cols").Value
                    'TableFooter = "<cs:text type=""tablefooter"" req=""yes"" xmlns:cs=""http://www.crest-premedia.in"">" + TableFooterNode.OuterXml + "</cs:text>"
                    TableFooter = "<Cell aid:table=""cell"" aid:ccolwidth="""" aid:crows=""" + CStr(1) + """ aid:ccols=""" + Columns + """ cs_type=""tablefooter"" xmlns:cs=""http://www.crest-premedia.in""><entry>" + TableFooterOuterXml + "</entry></Cell>"
                End If

                ' Dim TableXml As String = TableCaption + TableStart + CellXml + "</Table></Story>" + TableFooter
                ' Dim TableXml As String = TableStart + TableCaption + CellXml + "</Table></Story>" + TableFooter
                Dim TableStart As String = "<Story><Table xmlns:aid=""http://ns.adobe.com/AdobeInDesign/4.0/"" xmlns:aid5=""http://ns.adobe.com/AdobeInDesign/5.0/"" aid:table=""table"" aid:trows=""" + rows + """ aid:tcols=""" + Columns + """>"
                Dim TableXml As String = TableStart + TableCaption + CellXml + TableFooter + "</Table></Story>"

                NewTableNode.InnerXml = TableXml
                AddMergeColName(NewTableNode)
                'Dim Float As String = NewTableNode.Attributes.ItemOf("Float").Value
                'If (Float = "Yes") Then
                currTableStructure.ParentNode.InsertAfter(NewTableNode, currTableStructure)
                currTableStructure.ParentNode.RemoveChild(currTableStructure)
                AllNewTableNodes.Add(NewTableNode)
                'End If

            Catch ex As Exception
            End Try
            'PrepareIndesignTable
            '*************************** <Table node *****************
        Next
        For ii As Integer = 0 To AllNewTableNodes.Count - 1
            Try
                Dim TabNd As XmlNode = AllNewTableNodes(ii)
                Dim XAttr As Xml.XmlAttribute = TabNd.SelectSingleNode(".//@ID[1]")
                Dim AttrIdVal As String = ""
                If (IsNothing(XAttr) = False) Then
                    AttrIdVal = XAttr.Value
                End If
                If (TabNd.Attributes.ItemOf("cs_tbltype").Value.ToLower = "normal") Then
                    oreq.Reposition(AllNewTableNodes(ii), XDoc.DocumentElement, RelationShip.LastChild, "table", AttrIdVal)
                End If
                'If (TabNd.Attributes.ItemOf("type").Value.ToLower = "anhang") Then
                '    oreq.Reposition(AllNewTableNodes(ii), XDoc.DocumentElement, RelationShip.LastChild, "anhangtable", AttrIdVal)
                'End If
            Catch ex As Exception
            End Try
        Next


    End Sub
    Private Function GetColSpecWidth(ByVal TableNd As XmlNode, ByVal Columns As String)
        Try
            'commented for table opt' Dim tempWidth As Double = TableNd.Attributes.ItemOf("Width").Value
            Dim colNds As Xml.XmlNodeList = TableNd.SelectNodes(".//colspec")
            'commented for table opt'  Dim style As String = TableNd.Attributes.ItemOf("Style").Value
            Dim requiredWidth As Double


            'If (style = "Style2") Then
            '    requiredWidth = 123
            'ElseIf (style = "Style4") Then
            '    If (Infilemame.Trim = "501") Then
            '        requiredWidth = 234.811
            '    Else
            '        requiredWidth = 238.4
            '    End If

            'ElseIf (style = "Style1" Or style = "Style5") Then
            '    requiredWidth = 82
            'Else
            '    requiredWidth = 170.0
            'End If


            'For i As Integer = 0 To colNds.Count - 1
            '    Try
            '        Dim tempcolWidth = colNds(i).Attributes.ItemOf("colwidth").Value
            '        colNds(i).Attributes.ItemOf("colwidth").Value = (colNds(i).Attributes.ItemOf("colwidth").Value * requiredWidth) / tempWidth

            '    Catch ex As Exception

            '    End Try
            'Next
            'commented below for table optimization
            ''For cnode As Integer = 0 To colNds.Count - 1
            ''    Try
            ''        Dim ColDigit As Double = 0.0
            ''        ColDigit = colNds(cnode).Attributes.ItemOf("colwidth").Value
            ''        Dim tmp As Integer = cnode + 1
            ''        Dim NewAttr As Xml.XmlAttribute = XDoc.CreateAttribute("cs_col" + tmp.ToString)
            ''        NewAttr.Value = ColDigit
            ''        TableNd.Attributes.Append(NewAttr)


            ''    Catch ex As Exception

            ''    End Try
            'commented above for table optimization
          
            ''  Next



            ' '' ''For Each Nd As Xml.XmlNode In colNds
            ' '' ''    Try
            ' '' ''        Dim ColDigit As Integer = 0
            ' '' ''        ColDigit = Nd.Attributes.ItemOf("colnum").Value
            ' '' ''        Dim newColNds As Xml.XmlNode = XDoc.CreateElement("cs_Col")
            ' '' ''        Dim colId As Xml.XmlAttribute = XDoc.CreateAttribute("id")
            ' '' ''        colId.Value = ColDigit
            ' '' ''        Dim colratio As Xml.XmlAttribute = XDoc.CreateAttribute("ratio")
            ' '' ''        colratio.Value = ""
            ' '' ''        newColNds.Attributes.Append(colId)
            ' '' ''        newColNds.Attributes.Append(colratio)
            ' '' ''    Catch ex As Exception

            ' '' ''    End Try
            ' '' ''Next
        Catch ex As Exception
        End Try
    End Function

    Private Function GetColumnCount(ByVal TableNode As XmlNode, ByVal RowXpath As String, ByVal ColXpathWRTRow As String) As String
        Dim AllRowNodes As XmlNodeList = Nothing
        Dim RetVal As Int16 = 0
        Dim nd As Xml.XmlNode = TableNode.SelectSingleNode(".//tgroup[@cols]")
        If (IsNothing(nd) = False) Then
            RetVal = nd.Attributes.ItemOf("cols").Value
        End If

        'AllRowNodes = TableNode.SelectNodes(RowXpath, NS)
        'Dim RetVal As Int16 = 0
        'If IsNothing(AllRowNodes) = False Then
        '    For i As Integer = 0 To AllRowNodes.Count - 1
        '        Dim currRow As XmlNode = AllRowNodes(i)
        '        Dim ColCount As Integer = currRow.SelectNodes(ColXpathWRTRow).Count
        '        If ColCount > RetVal Then
        '            RetVal = ColCount
        '        End If
        '    Next
        'End If
        Return RetVal.ToString
    End Function

    Private Function GetRowCount(ByVal TableNode As XmlNode, ByVal RowTagName As String) As String
        Dim AllRowNodes As XmlNodeList = TableNode.SelectNodes(".//" + RowTagName, NS)
        Return AllRowNodes.Count.ToString
    End Function

    Private Function GetCellXml(ByVal TableNode As XmlNode, ByVal XPathList As ArrayList, ByVal CellTagName As String, ByVal CurrTable As XmlNode, ByVal TableCoulumInformationXPath As String) As String
        'TableSettings can be used to get TaableSetting 
        ' Need to improve ,to remove hardcoding

        Dim retString As String = ""
        For Each xp As String In XPathList
            If xp <> "" Then
                Dim Cells As XmlNodeList = TableNode.SelectNodes(xp, NS)
                For i As Integer = 0 To Cells.Count - 1
                    Dim CellOuterXml As String = Cells(i).OuterXml
                    Dim CellProp As New Hashtable
                    CellProp.Add(MoreRowsArr, "1")
                    CellProp.Add(ColWidthAtt, "")
                    CellProp.Add("ColCount", "1")
                    'Computing ColWidth 
                    Dim RetHash As New Hashtable
                    Try
                        Select Case True
                            Case IsNothing(Cells(i).Attributes.ItemOf(ColnameAttr)) = False
                                RetHash = GetCellWidth(CurrTable, Cells(i), ColnameAttr)
                                CellProp(ColWidthAtt) = RetHash(ColWidthAtt)
                            Case IsNothing(Cells(i).Attributes.ItemOf(mergedColStart)) = False And IsNothing(Cells(i).Attributes.ItemOf(MergedColEnd)) = False
                                RetHash = GetCellWidth(CurrTable, Cells(i), mergedColStart, MergedColEnd)
                                CellProp(ColWidthAtt) = RetHash(ColWidthAtt)
                        End Select
                    Catch ex As Exception
                    End Try

                    CellProp(MoreRowsArr) = "1"
                    If IsNothing(Cells(i).Attributes.ItemOf(MoreRowsArr)) = False Then
                        Try
                            CellProp(MoreRowsArr) = CStr(CInt(CellProp(MoreRowsArr)) + CInt(Cells(i).Attributes.ItemOf(MoreRowsArr).Value.ToString))
                        Catch ex As Exception
                        End Try
                        'CellProp.Add(MoreRowsArr, Cells(i).Attributes.ItemOf(MoreRowsArr))
                    End If

                    CellProp("ColCount") = RetHash("ColCount")

                    CellOuterXml = "<Cell " + "aid:table=""cell"" aid:ccolwidth=""" + CellProp(ColWidthAtt) + """ aid:crows=""" + CellProp(MoreRowsArr) + """ aid:ccols=""" + CellProp("ColCount") + """ cs_type=""" + xp + """>" + CellOuterXml + "</Cell>"
                    'CellOuterXml = "<Cell " + "aid:table=""cell"" aid:ccolwidth=""100"" aid:crows=""" + CellProp(MoreRowsArr) + """ aid:ccols=""" + CellProp("ColCount") + """ cs_type=""" + xp + """>" + CellOuterXml + "</Cell>"
                    retString += CellOuterXml
                Next
            End If
        Next
        Return retString
    End Function

    'Private Function GetColWidth(ByVal CurrTable As XmlNode, ByVal CurrCell As XmlNode, ByVal colnameAttr As String, ByVal ColWidthAtt As String) As String
    '    Dim ColWidth As String = "5"
    '    Try
    '        Dim colName As String = CurrCell.Attributes.ItemOf(colnameAttr).Value
    '        Dim ColspecNode As XmlNode = CurrTable.SelectSingleNode(".//tgroup/colspec[@" + colnameAttr + "='" + colName + "']")
    '        ColWidth = Regex.Match(ColspecNode.OuterXml, ColWidthAtt + "=""([^\s]+?)""", RegexOptions.Singleline).Groups(1).Value
    '        ColWidth = Regex.Match(ColWidth, "(\d+)", RegexOptions.Singleline).Groups(1).Value
    '    Catch ex As Exception
    '    End Try
    '    Return ColWidth
    'End Function
    Private Function getColWidth(ByVal CurrTable As XmlNode, ByVal colnameAttr As String, ByVal ColNameVal As String, ByVal ColWidthAtt As String) As String
        Dim ColWidthVal As String = ""

        Try
            Dim ColspecNode As XmlNode = CurrTable.SelectSingleNode(TableCoulumInformationXPath + "[@" + colnameAttr + "='" + ColNameVal + "']", NS)
            ColWidthVal = Regex.Match(ColspecNode.OuterXml, ColWidthAtt + "=""([^\s]+?)""", RegexOptions.Singleline).Groups(1).Value
            ColWidthVal = Regex.Match(ColWidthVal, "(\d+)(.(\d+))?", RegexOptions.Singleline).Groups(0).Value
        Catch ex As Exception
        End Try

        Return ColWidthVal

    End Function
    Private Function GetCellWidth(ByVal CurrTable As XmlNode, ByVal CurrCell As XmlNode, ByVal StartcolnameAttr As String, Optional ByVal EndColNameAttr As String = "") As Hashtable
        Dim RetProp As New Hashtable
        Dim TotalCellWidth As Double = 0.0
        Dim StartColDigit As Integer = 0
        Dim StartColName As String = ""
        Dim kColIteratorMin As Int16 = 1
        Dim kColIteratorMax As Int16 = 1

        Dim ColNamesWhoseWidthNeedToKnow As New ArrayList
        Try
            StartColName = CurrCell.Attributes.ItemOf(StartcolnameAttr).Value
            StartColDigit = Integer.Parse(Regex.Match(StartColName, "(\d+)", RegexOptions.Singleline).Groups(1).Value)
        Catch ex As Exception
        End Try


        Dim EndColDigit As Integer = StartColDigit
        Dim EndColName As String = ""
        Try
            EndColName = CurrCell.Attributes.ItemOf(EndColNameAttr).Value
            EndColDigit = Integer.Parse(Regex.Match(EndColName, "(\d+)", RegexOptions.Singleline).Groups(1).Value)
        Catch ex As Exception
        End Try
        Dim ColCount As Integer = 0
        Dim PrefixOfColName As String = Regex.Match(StartColName, "([a-zA-Z]+)(\d+)", RegexOptions.Singleline).Groups(1).Value
        For i As Integer = StartColDigit To EndColDigit
            ColCount += 1
            Dim retval As String = ""
            retval = getColWidth(CurrTable, ColnameAttr, PrefixOfColName + i.ToString, ColWidthAtt)
            Try
                TotalCellWidth += Double.Parse(retval)
            Catch ex As Exception
            End Try
        Next
        If TotalCellWidth = 0.0 Then
            RetProp.Add(ColWidthAtt, "")
        Else
            RetProp.Add(ColWidthAtt, TotalCellWidth.ToString)
        End If

        RetProp.Add("ColCount", ColCount.ToString)
        Return RetProp
    End Function

End Class


