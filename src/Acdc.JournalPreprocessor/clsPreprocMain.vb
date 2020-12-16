'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : SUVARNA RAUT
'MODULE NAME   : clsPreprocMain
'CREATED DATE  : 3RD JUNE 2013
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.IO
Imports System.Xml.Linq
Imports System.Text

Public Class clsPreprocMain
    Private XmlFile As String
    Private XmlStr As String
    Public XDoc As New Xml.XmlDocument
    Private NTable As Xml.NameTable
    Public Shared NS As Xml.XmlNamespaceManager
    Public Shared NSht As New Hashtable
    Public oTableObj As clsTableConversion
    Public oFigObj As clsFigureConversion

    Public Sub New(ByVal XFile As String)
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
        XDoc.PreserveWhitespace = True

        '' add in to VSS
        If File.Exists(XFile) Then

            XmlFile = XFile
            Try
                ''EquationCorrection(XmlFile)
            Catch ex As Exception

            End Try


            Dim sr As New StreamReader(XFile)

            XmlStr = sr.ReadToEnd
            XmlStr = Regex.Replace(XmlStr, "<!--<!DOCTYPE([^<>]+?)>-->", "", RegexOptions.Singleline)
            XmlStr = Regex.Replace(XmlStr, "<!-- <!DOCTYPE([^<>]+?)> -->", "", RegexOptions.Singleline)

            XmlStr = Regex.Replace(XmlStr, "<!DOCTYPE([^<>]+?)>", "<!--<!DOCTYPE -->", RegexOptions.Singleline) ''DD:17/08/2011 M.K


            XmlStr = RemoveIndentSpace(XmlStr)
            XmlStr = XmlStr.Replace(" ", " ")
            '' XmlStr = Regex.Replace(XmlStr, "\s{2,}", "", RegexOptions.Singleline)
            ' XmlStr = Regex.Replace(XmlStr, "\n", "", RegexOptions.Singleline)
            'XmlStr = Regex.Replace(XmlStr, "\r", "", RegexOptions.Singleline)
            'XmlStr = Regex.Replace(XmlStr, ChrW(13), "", RegexOptions.Singleline)
            'XmlStr = XmlStr.Replace(vbNewLine, "")

            'XmlStr = Regex.Replace(XmlStr, "\t", "", RegexOptions.Singleline)
            XmlStr = XmlStr.Replace(vbNewLine, "<!-- enter -->")
            XmlStr = Regex.Replace(XmlStr, "<\?xml([^<>]+?\?>)", "<!--<?xml\1\?> -->", RegexOptions.Singleline)

            XmlStr = Regex.Replace(XmlStr, "char=""&#x00D7;""", "char=""×""", RegexOptions.Singleline) 'multi
            XmlStr = Regex.Replace(XmlStr, "char=""&#x2013;""", "char=""–""", RegexOptions.Singleline)  'endash
            XmlStr = Regex.Replace(XmlStr, "char=""&#x00B1;""", "char=""±""", RegexOptions.Singleline)  'plusminus
            XmlStr = Regex.Replace(XmlStr, "char=""&#x2212;""", "char=""−""", RegexOptions.Singleline)  'minus
            XmlStr = Regex.Replace(XmlStr, "char=""&#X00B7;""", "char=""·""", RegexOptions.Singleline)  'middot

            XmlStr = Regex.Replace(XmlStr, "char=""&#x002B;""", "char=""+""", RegexOptions.Singleline)  'plus
            ''added by suru on 28.05.2012
            ' XmlStr = XmlStr.Replace("&#x03B2;", "<cs_text type=""entity"" xmlns:aid=""http://ns.adobe.com/AdobeInDesign/4.0/"" aid:pstyle=""Body_Text_Indent_italic"">" + "&#x03B2;" + "</cs_text>")


            Dim lang As String = getLanguage(XmlFile)


            If (lang.ToLower = "en") Then
                XmlStr = Regex.Replace(XmlStr, "(<!--\s*Query ID=""Q)(\d+)"" Text=""(([^>]*))""\s*-->", "<cs_query repoID=""" + "$2" + """>" + "AQ" + "$2.&#8194;" + "<cs_text type='indentohere'></cs_text>" + "$3" + vbNewLine + "</cs_query>")
            Else
                XmlStr = Regex.Replace(XmlStr, "(<!--\s*Query ID=""Q)(\d+)"" Text=""(([^>]*))""\s*-->", "<cs_query repoID=""" + "$2" + """>" + "FA" + "$2.&#8194;" + "<cs_text type='indentohere'></cs_text>" + "$3" + vbNewLine + "</cs_query>")
            End If

            '' by Suvarna
            XmlStr = XmlStr.Replace("&#x2502;", "<cs_text type=""entity"" xmlns:aid=""http://ns.adobe.com/AdobeInDesign/4.0/"" aid:cstyle=""Black Square"">" + "&#x2502;" + "</cs_text>")




            '<!DOCTYPE
            XmlStr = Regex.Replace(XmlStr, "<!--<!DOCTYPE([^<>]+?)>-->", "", RegexOptions.Singleline)
            ''XmlStr = Regex.Replace(XmlStr, "<!-- <!DOCTYPE([^<>]+?)> -->", "", RegexOptions.Singleline)
            ' ''XmlStr = Regex.Replace(XmlStr, "<!DOCTYPE([^<>]+?)>", "<!--<!DOCTYPE>-->", RegexOptions.Singleline)
            ''XmlStr = Regex.Replace(XmlStr, "<!DOCTYPE([^<>]+?)>", "<!--<!DOCTYPE -->", RegexOptions.Singleline) ''DD:17/08/2011 M.K

            'For Equal Symbol start
            XmlStr = Regex.Replace(XmlStr, "&#x2009;=&#x2009;", "&#x2009;<cs_text type='cSYMBOL'>=</cs_text>&#x2009;", RegexOptions.Singleline)
            XmlStr = Regex.Replace(XmlStr, "<Superscript>=</Superscript>", "<Superscript><cs_text type='cSYMBOL'>=</cs_text></Superscript>", RegexOptions.Singleline)
            XmlStr = Regex.Replace(XmlStr, "<Subscript>=</Subscript>", "<Subscript><cs_text type='cSYMBOL'>=</cs_text></Subscript>", RegexOptions.Singleline)
            XmlStr = Regex.Replace(XmlStr, "<Emphasis Type=""Bold"">=</Emphasis>", "<Emphasis Type='Bold'><cs_text type='cSYMBOL'>=</cs_text></Emphasis>", RegexOptions.Singleline)
            XmlStr = Regex.Replace(XmlStr, "<Emphasis Type=""Italic"">=</Emphasis>", "<Emphasis Type='Italic'><cs_text type='cSYMBOL'>=</cs_text></Emphasis>", RegexOptions.Singleline)
            XmlStr = Regex.Replace(XmlStr, "<Emphasis Type=""BoldItalic"">=</Emphasis>", "<Emphasis Type='BoldItalic'><cs_text type='cSYMBOL'>=</cs_text></Emphasis>", RegexOptions.Singleline)

            'For Equal Symbol end
            sr.Close()

            Dim sw As New StreamWriter("d:\tt.xml", False, System.Text.Encoding.GetEncoding(1252))
            sw.Write(XmlStr)
            sw.Close()
            NTable = New NameTable
            NS = New Xml.XmlNamespaceManager(NTable)

            NS.AddNamespace("cs", "http://www.crest-premedia.in")
            NS.AddNamespace("aid", "http://ns.adobe.com/AdobeInDesign/4.0/")
            NS.AddNamespace("aid5", "http://ns.adobe.com/AdobeInDesign/5.0/")

            NSht.Add("cs", "http://www.crest-premedia.in")
            NSht.Add("aid", "http://ns.adobe.com/AdobeInDesign/4.0/")
            NSht.Add("aid5", "http://ns.adobe.com/AdobeInDesign/5.0/")

            Try
                Dim obj As New ClsEntity()
                XmlStr = obj.ConvertSymbolEntities(XmlStr, EntityPathxml)
            Catch ex As Exception
                CLog.LogMessages("Error in EntityConversion()" + vbNewLine)
                CLog.LogMessages(ex.Message.ToString + vbNewLine)
                Throw
            End Try

            XDoc = CreateDom(XmlStr)


            ''''''''''''''''''''''CreateEntity start''''''''''''''''''''''''



            ''''''''''''''''''''''CreateEntity end''''''''''''''''''''''''

            'Added below code for log message
            'equation
            If (IsNothing(XDoc.SelectNodes(".//EquationSource")) = False And XDoc.SelectNodes(".//EquationSource").Count > 0) Then
                Dim EquationNds As Integer = XDoc.SelectNodes(".//EquationSource").Count
            End If

            ' figure
            If (IsNothing(XDoc.SelectNodes(".//Figure//CaptionNumber[not(.='')]")) = False And XDoc.SelectNodes(".//Figure//CaptionNumber[not(.='')]").Count > 0) Then
                Dim FigNodes As Xml.XmlNodeList = XDoc.SelectNodes(".//Figure//CaptionNumber[not(.='')]")
            End If

            ' table
            If (IsNothing(XDoc.SelectNodes(".//Table//CaptionNumber[not(.='')]")) = False And XDoc.SelectNodes(".//Table//CaptionNumber[not(.='')]").Count > 0) Then
                Dim TableNodes As Xml.XmlNodeList = XDoc.SelectNodes(".//Table//CaptionNumber[not(.='')]")
            End If

            oTableObj = New clsTableConversion(XDoc, NTable, NS, Me)
            oFigObj = New clsFigureConversion(XDoc, NTable, NS, Me)
        End If
    End Sub
    Private Function RemoveIndentSpace(ByVal XmlStr As String) As String

        Try

            Dim sxDoc As New XmlDocument

            sxDoc.PreserveWhitespace = False

            Dim xWriteSettings As New XmlWriterSettings()

            xWriteSettings.Indent = False

            Dim xWrite As XmlWriter = Nothing

            sxDoc.LoadXml(XmlStr)

            xWrite = XmlWriter.Create("d:\coversIndentXml.xml", xWriteSettings)

            sxDoc.WriteTo(xWrite)

            xWrite.Close()
            Dim finalxdoc As New XmlDocument
            finalxdoc.Load("d:\coversIndentXml.xml")


            Return finalxdoc.OuterXml

            ''   Debug.WriteLine("")

        Catch ex As Exception

            Return Nothing

        End Try

        Return Nothing

    End Function

    Private Function getLanguage(ByVal xmlFile As String) As String
        Dim lang As String = ""

        Try
            Dim articleInfo = From nodes In XElement.Load(xmlFile).Elements("ArticleInfo") Select nodes

            If Not articleInfo Is Nothing Then
                For Each nd In articleInfo
                    lang = nd.Attributes("Language")(0).Value.ToString()
                    Exit For
                Next



            End If
        Catch ex As Exception
            CLog.LogMessages("Error in getLanguage()" + vbNewLine)
            CLog.LogMessages("ex.message" + ex.Message.ToString)
            Throw
        End Try

        Return lang
    End Function

    'Private Sub EquationCorrection(ByVal XMLfullFileName As String)
    '    Dim proc As New Process
    '    proc.StartInfo.FileName = "perl.exe"
    '    proc.StartInfo.Arguments = """" & System.Windows.Forms.Application.StartupPath & "\eqncorrection.pl""" & Space(1) & XMLfullFileName
    '    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
    '    proc.Start()
    '    proc.WaitForExit()
    '    While (proc.HasExited <> True)
    '    End While
    '    proc.Close()
    'End Sub

    Public Function MapItalicEntities(ByVal m As Match) As String
        Dim WDEmp As String = m.Groups(2).Value
        Dim WDFont As String = m.Groups(3).Value
        Dim WDHexName As String = m.Groups(4).Value.Replace("&#x", "").Replace(";", "")
        'System.Configuration.ConfigurationSettings.AppSettings("EntityPath") '"D:\Suresh\EntWord2Indd\Entities.xml"
        Dim entityDoc As New Xml.XmlDocument
        entityDoc.Load(EntityPathxml)
        Dim entities As Xml.XmlNode = entityDoc.SelectSingleNode(".//Entity[WFont='" + WDFont + "' and Emp='" + WDEmp + "' and WCC='" + WDHexName + "']")
        Try
            If (IsNothing(entities) = False) Then
                Dim EntityTag As String = "<Emphasis type=" + """" + "c" + entities.ChildNodes(5).InnerText + """" + ">"
                EntityTag += "&#x" + entities.ChildNodes(6).InnerText + ";</Emphasis>"
                Return EntityTag
            End If
        Catch ex As Exception

        End Try
    End Function
    Public Function MapNormalEntities(ByVal m As Match) As String
        Dim WDEmp As String = "Normal"
        Dim WDFont As String = m.Groups(1).Value
        Dim WDHexName As String = m.Groups(2).Value.Replace("&#x", "").Replace(";", "")
        Dim EntityPath As String = "" 'System.Configuration.ConfigurationSettings.AppSettings("EntityPath") '"D:\Suresh\EntWord2Indd\Entities.xml"
        Dim entityDoc As New Xml.XmlDocument
        entityDoc.Load(EntityPathxml)
        Dim entities As Xml.XmlNode = entityDoc.SelectSingleNode(".//Entity[WFont='" + WDFont + "' and Emp='" + WDEmp + "' and WCC='" + WDHexName + "']")
        Try
            If (IsNothing(entities) = False) Then
                Dim EntityTag As String = "<Emphasis type=" + """" + "c" + entities.ChildNodes(5).InnerText + """" + ">"
                EntityTag += "&#x" + entities.ChildNodes(6).InnerText + ";</Emphasis>"
                Return EntityTag
            End If
        Catch ex As Exception

        End Try
    End Function
    Private Function ReplaceEntities(ByVal xmlstr As String) As String
        Try
            Dim finfo As New System.IO.FileInfo(Environment.GetCommandLineArgs(0))
            If System.IO.File.Exists(finfo.DirectoryName + "\" + "Config.xml") Then
                Dim ConfigDoc As New Xml.XmlDocument
                ConfigDoc.Load(finfo.DirectoryName + "\" + "Config.xml")
                Dim EntityPath As String = ConfigDoc.SelectSingleNode(".//Entitysettingsxml").InnerText
                Dim entityDoc As New Xml.XmlDocument
                entityDoc.Load(EntityPath)
                Dim entities As Xml.XmlNodeList = entityDoc.SelectNodes(".//Entities/entity")
                For Each entity As Xml.XmlNode In entities
                    Dim hexname As String = entity.Attributes.ItemOf("hexname").Value
                    hexname = "&#x?" + hexname + ";"
                    Dim FontName As String = entity.Attributes.ItemOf("font").Value
                    xmlstr = Regex.Replace(xmlstr, "(" + hexname + ")", "<cs_text type=""entity"" font=" + """" + FontName + """" + ">" + "$1" + "</cs_text>", RegexOptions.Singleline)
                Next
            End If
        Catch ex As Exception

        End Try
        Return xmlstr
    End Function
    Private Function CreateDom(ByVal XmlString As String) As Xml.XmlDocument

        Dim Xdoc As New Xml.XmlDocument
        Xdoc.PreserveWhitespace = True
        Try
            Xdoc.LoadXml(XmlString)
        Catch ex As Exception
            CLog.LogMessages("Error when creating DOM Object" + ex.Message)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages(vbNewLine + "STM Journal Preprocessor  :" + ex.Message.ToString + vbNewLine)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages(vbNewLine + XmlString)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
        End Try


        Return Xdoc
    End Function
    Public Function AddAttribute(ByVal XPath As String, ByVal AttrName As String, ByVal AttrVal As String)
        Dim NdList As Xml.XmlNodeList = XDoc.SelectNodes(XPath, NS)
        Dim prefix As String = ""
        Dim LocalName As String = ""
        prefix = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups(1).Value
        If prefix = "" Then
            LocalName = AttrName
        Else
            LocalName = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups(2).Value
        End If

        ' For all the nodes having the XPATH 
        For i As Integer = 0 To NdList.Count - 1
            Dim Nd As Xml.XmlNode = NdList(i)
            Dim AttributeExists As Boolean = False
            Try
                If (IsNothing(Nd.Attributes.ItemOf(AttrName)) = False) Then
                    Dim reqAttr As Xml.XmlAttribute = Nd.Attributes.ItemOf(AttrName)
                    reqAttr.Value = AttrVal
                    AttributeExists = True
                End If

            Catch ex As Exception

            End Try

            Try
                If AttributeExists = False Then
                    Dim newAttr As Xml.XmlAttribute
                    If prefix = "" Then
                        newAttr = XDoc.CreateAttribute(AttrName)
                    Else
                        newAttr = XDoc.CreateAttribute(AttrName, NSht(prefix))
                    End If
                    newAttr.Value = AttrVal
                    Nd.Attributes.Append(newAttr)
                End If
            Catch ex As Exception
            End Try
        Next

    End Function
    Public Function AddInfoNode_2(ByVal XmlFragStr As ArrayList)
        Dim AddPreProc As Xml.XmlProcessingInstruction = Nothing
        Try
            'If IsNothing((XDoc.SelectSingleNode(".//processing-instruction('ADDINFO')"))) Then
            AddPreProc = XDoc.CreateProcessingInstruction("ADDINFO", "")
            ' End If
        Catch ex As Exception
        End Try
        For Each str As String In XmlFragStr
            Try
                'XDoc.DocumentElement.Name 
                Dim DataPart As String = str
                DataPart = ClearDataPart(DataPart)
                AddPreProc.Data += DataPart
            Catch ex As Exception
            End Try
        Next

        Try
            Dim RootName As String = XDoc.DocumentElement.Name
            Dim RootNd As XmlNode = XDoc.SelectSingleNode(".//" + RootName)
            RootNd.AppendChild(AddPreProc)
        Catch ex As Exception
        End Try

    End Function

    Public Function AddAttribute(ByVal XNode As XmlNode, ByVal AttrName As String, ByVal AttrVal As String)
        Dim NdList As New ArrayList
        NdList.Add(XNode)

        Dim prefix As String = ""
        Dim LocalName As String = ""
        prefix = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups(1).Value
        If prefix = "" Then
            LocalName = AttrName
        Else
            LocalName = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups(2).Value
        End If

        ' For all the nodes having the XPATH 
        For i As Integer = 0 To NdList.Count - 1
            Dim Nd As Xml.XmlNode = NdList(i)
            Dim AttributeExists As Boolean = False
            Try
                If (IsNothing(Nd.Attributes.ItemOf(AttrName)) = False) Then
                    Dim reqAttr As Xml.XmlAttribute = Nd.Attributes.ItemOf(AttrName)
                    reqAttr.Value = AttrVal
                    AttributeExists = True
                End If

            Catch ex As Exception
            End Try

            Try
                If AttributeExists = False Then
                    Dim newAttr As Xml.XmlAttribute
                    If prefix = "" Then
                        newAttr = XDoc.CreateAttribute(AttrName)
                    Else
                        newAttr = XDoc.CreateAttribute(AttrName, NSht(prefix))
                    End If
                    newAttr.Value = AttrVal
                    Nd.Attributes.Append(newAttr)
                End If
            Catch ex As Exception
            End Try
        Next
    End Function

    ''Public Function AddTextorCharacter(ByVal ToNode As String, ByVal text As String, Optional ByVal before As Boolean = True, Optional ByVal type As String = "")
    ''    Dim XdocString As String = XDoc.InnerXml
    ''    If before Then
    ''        If type.Length = 0 Then
    ''            XdocString = Regex.Replace(XdocString, "<" + ToNode + "\s?>", "<cs:text xmlns:cs=""http://www.crest-premedia.in"">" + text + "</cs:text>" + "<" + ToNode + ">", RegexOptions.Singleline)
    ''        Else
    ''            XdocString = Regex.Replace(XdocString, "<" + ToNode + "\s?>", "<cs:text type=""newline"" xmlns:cs=""http://www.crest-premedia.in"">" + text + "</cs:text>" + "<" + ToNode + ">", RegexOptions.Singleline)
    ''        End If
    ''    Else
    ''        If type.Length = 0 Then
    ''            XdocString = Regex.Replace(XdocString, "</" + ToNode + ">", "</" + ToNode + ">" + "<cs:text xmlns:cs=""http://www.crest-premedia.in"">" + text + "</cs:text>", RegexOptions.Singleline)
    ''        Else
    ''            XdocString = Regex.Replace(XdocString, "</" + ToNode + ">", "</" + ToNode + ">" + "<cs:text type=""newline"" xmlns:cs=""http://www.crest-premedia.in"">" + text + "</cs:text>", RegexOptions.Singleline)
    ''        End If
    ''    End If



    ''    'XdocString = Regex.Replace(XdocString, EnterString + "</(.+?)>" + EnterString, EnterString, RegexOptions.Singleline)
    ''    Try
    ''        XDoc.InnerXml = XdocString
    ''    Catch ex As Exception
    ''    End Try

    ''End Function

    ''Public Function RemoveDoubleEnter()

    ''    Dim AllReqNodes As Xml.XmlNodeList = XDoc.SelectNodes(".//cs:text/following-sibling::cs:text", NS)

    ''    'For i As Integer = 0 To AllReqNodes.Count - 1
    ''    '    dim 

    ''    'Next
    ''    '''Dim XdocString As String = XDoc.InnerXml
    ''    ' ''Dim EnterString As String = "<cs:text type=""newline"" xmlns:cs=""http://www.crest-premedia.in"">(.)</cs:text>"
    ''    ' ''Dim DoubleEnter As MatchCollection = Regex.Matches(XdocString, EnterString + "</(.+?)>" + EnterString, RegexOptions.Singleline)

    ''    ' ''Dim meav As New MatchEvaluator(AddressOf eval)
    ''    ' ''XdocString = Regex.Replace(XdocString, EnterString + "</(.+?)>" + EnterString, meav)
    ''    ' ''Try
    ''    ' ''    XDoc.InnerXml = XdocString
    ''    ' ''Catch ex As Exception
    ''    ' ''End Try
    ''End Function

    ''Public Function eval(ByVal m As Match) As String
    ''    Dim EnterString As String = "<cs:text type=""newline"" xmlns:cs=""http://www.crest-premedia.in"">(.)</cs:text>"
    ''    If AscW(m.Groups(1).Value) = 13 Then
    ''        Return m.Groups(2).Value + EnterString
    ''    Else
    ''        Return m.Value
    ''    End If

    ''End Function

    Public Enum ChildTypes
        AsFirstChild = 1
        AsLastChild = 2
        AsPreviousSibling = 3
        AsNextSibling = 4
    End Enum

    Public Function AddTextorXml(ByVal ToNodeXpath As String, ByVal text As String, ByVal Position As ChildTypes, Optional ByVal force As Boolean = False)

        Dim AllReqNodes As Xml.XmlNodeList = XDoc.SelectNodes(ToNodeXpath, NS)
        For i As Integer = 0 To AllReqNodes.Count - 1
            Dim ReqNd As XmlNode = AllReqNodes(i)
            Select Case Position
                Case ChildTypes.AsFirstChild
                    ReqNd.InnerXml = text + ReqNd.InnerXml
                Case ChildTypes.AsLastChild
                    ReqNd.InnerXml = ReqNd.InnerXml + text
                Case ChildTypes.AsNextSibling
                    Try
                        'Dim newNode As XmlElement = XDoc.CreateElement("cs:text", NSht("cs"))
                        ''Dim temxDoc As XmlDocument = New XmlDocument
                        ''temxDoc.LoadXml(text)
                        ''Dim newNode As Xml.XmlNode = XDoc.ImportNode(temxDoc.FirstChild, True)

                        ''If ReqNd.InnerText.EndsWith(vbNewLine) = False Or force = True Then
                        ''    ReqNd.ParentNode.InsertAfter(newNode, ReqNd)

                        ''End If

                        Dim newNode As XmlElement = XDoc.CreateElement("root")
                        newNode.InnerXml = text

                        If ReqNd.InnerText.EndsWith(vbNewLine) = False Or force = True Then
                            While (newNode.ChildNodes.Count > 0)
                                ReqNd.ParentNode.InsertAfter(newNode.LastChild, ReqNd)
                            End While

                        End If
                    Catch ex As Exception

                    End Try

                Case ChildTypes.AsPreviousSibling
                    'Dim newNode As XmlElement = XDoc.CreateElement("cs:text", NSht("cs"))
                    Dim newNode As XmlElement = XDoc.CreateElement("root")
                    newNode.InnerXml = text

                    Try
                        If IsNothing(ReqNd.PreviousSibling) = False Then
                            While (newNode.ChildNodes.Count > 0)
                                ReqNd.ParentNode.InsertBefore(newNode.FirstChild, ReqNd)
                            End While
                        Else
                            While (newNode.ChildNodes.Count > 0)
                                ReqNd.ParentNode.InsertBefore(newNode.FirstChild, ReqNd)
                            End While
                        End If
                    Catch ex As Exception
                    End Try




            End Select
        Next
    End Function

    Public Sub DeleteAttr(ByVal Xpath As String, ByVal AttrName As String)
        Dim NdList As Xml.XmlNodeList = XDoc.SelectNodes(Xpath, NS)
        Dim prefix As String = ""
        Dim LocalName As String = ""
        prefix = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups(1).Value
        If prefix = "" Then
            LocalName = AttrName
        Else
            LocalName = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups(2).Value
        End If

        Dim AllReqNodes As Xml.XmlNodeList = XDoc.SelectNodes(Xpath, NS)
        For i As Integer = 0 To AllReqNodes.Count - 1
            Dim ReqNd As XmlNode = AllReqNodes(i)
            Try
                ReqNd.Attributes.Remove(ReqNd.Attributes.ItemOf(AttrName))
            Catch ex As Exception
            End Try
        Next
    End Sub
    Public Function HideLineNode(ByVal xPath As XmlNode)
        For Each nd As XmlNode In xPath
            Try
                Dim HideXmlPreProc As Xml.XmlProcessingInstruction = XDoc.CreateProcessingInstruction("HIDE", "")
                Dim DataPart As String = nd.OuterXml
                DataPart = ClearDataPart(DataPart)
                HideXmlPreProc.Data = DataPart
                nd.ParentNode.ReplaceChild(HideXmlPreProc, nd)

            Catch ex As Exception

            End Try
        Next

    End Function
    Public Function AddInfoNode(ByVal XmlFragStr As ArrayList)
        Dim AddPreProc As Xml.XmlProcessingInstruction = Nothing
        Try
            If IsNothing((XDoc.SelectSingleNode(".//processing-instruction('ADDHDINFO')"))) Then
                AddPreProc = XDoc.CreateProcessingInstruction("ADDHDINFO", "")
            End If
        Catch ex As Exception
        End Try
        For Each str As String In XmlFragStr
            Try
                'XDoc.DocumentElement.Name 
                Dim DataPart As String = str
                DataPart = ClearDataPart(DataPart)
                AddPreProc.Data += DataPart
            Catch ex As Exception
            End Try
        Next

        Try
            Dim RootName As String = XDoc.DocumentElement.Name
            Dim RootNd As XmlNode = XDoc.SelectSingleNode(".//" + RootName)
            RootNd.AppendChild(AddPreProc)
        Catch ex As Exception
        End Try

    End Function

    Public Function HideNode(ByVal XpathList As ArrayList)
        For Each XPathStr As String In XpathList
            Dim Nodes As Xml.XmlNodeList = XDoc.SelectNodes(XPathStr, NS)
            For Each nd As XmlNode In Nodes
                Try
                    Dim HideXmlPreProc As Xml.XmlProcessingInstruction = XDoc.CreateProcessingInstruction("HIDE", "")
                    Dim DataPart As String = nd.OuterXml
                    DataPart = ClearDataPart(DataPart)
                    HideXmlPreProc.Data = DataPart
                    nd.ParentNode.ReplaceChild(HideXmlPreProc, nd)
                Catch ex As Exception
                End Try
            Next
        Next
    End Function

    Public Function DeleteNode(ByVal DelXpath As String)
        Dim Nodes As Xml.XmlNodeList = XDoc.SelectNodes(DelXpath, NS)

        For i As Integer = 0 To Nodes.Count - 1
            Try
                DeleteNode(Nodes(i))
            Catch ex As Exception
            End Try
        Next

    End Function
    Public Function DeleteNode(ByVal nd As Xml.XmlNode)
        If IsNothing(nd) = False Then
            nd.ParentNode.RemoveChild(nd)
        End If
    End Function

    Private Function ClearDataPart(ByVal datapart As String) As String
        Return Regex.Replace(datapart, "<\?([^<>]+?)\?>", "<cs_preproc>$1</cd_preproc>", RegexOptions.Singleline)
    End Function


    'Public Sub Reposition(ByVal SourceNodeXpath As String, ByVal DestinationPlaceXpath As String, ByVal withRelation As RelationShip)
    '    Dim SrcNode As Xml.XmlNode = XDoc.SelectSingleNode(SourceNodeXpath, NS)

    '    Dim DestiPlaceNode As Xml.XmlNode = XDoc.SelectSingleNode(DestinationPlaceXpath, NS)

    '    If SrcNode Is Nothing Or DestiPlaceNode Is Nothing Then
    '        Exit Sub
    '    End If

    '    Dim RepositionNode As Xml.XmlNode = XDoc.CreateElement("cs:reposition", NSht("cs"))
    '    Select Case withRelation
    '        Case RelationShip.FirstChild
    '            If DestiPlaceNode.ChildNodes.Count > 0 Then
    '                Dim firstc As Xml.XmlNode = DestiPlaceNode.ChildNodes.Item(0)
    '                DestiPlaceNode.InsertBefore(RepositionNode, firstc)
    '            Else
    '                DestiPlaceNode.AppendChild(RepositionNode)
    '            End If
    '        Case RelationShip.LastChild
    '            DestiPlaceNode.AppendChild(RepositionNode)
    '        Case RelationShip.NextSibling
    '            DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode)
    '        Case RelationShip.PreviousSibling
    '            DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode)
    '    End Select


    '    RepositionNode.InnerXml = SrcNode.OuterXml

    '    Dim RepoId As Xml.XmlAttribute = XDoc.CreateAttribute("cs:repoID", NSht("cs"))
    '    RepositionNode.Attributes.Append(RepoId)
    '    RepoId.Value = clsReposition.GetRepositionID
    '    RepositionHide(SrcNode, RepoId.Value)

    'End Sub
    Public Sub FrameMove(ByVal SourceNodeXpath As Xml.XmlNode, ByVal DestinationPlaceXpath As Xml.XmlNode, ByVal withRelation As RelationShip)
        Dim SrcNode As Xml.XmlNode = SourceNodeXpath

        Dim DestiPlaceNode As Xml.XmlNode = DestinationPlaceXpath

        If SrcNode Is Nothing Or DestiPlaceNode Is Nothing Then
            Exit Sub
        End If

        Try
            Dim frameatt As Xml.XmlAttribute = XDoc.CreateAttribute("cs_moved")
            frameatt.Value = "1"
            SrcNode.Attributes.Append(frameatt)
        Catch ex As Exception
        End Try

        Dim RepositionNode As Xml.XmlNode = XDoc.CreateElement("cs:reposition", NSht("cs"))
        Select Case withRelation
            Case RelationShip.FirstChild
                If DestiPlaceNode.ChildNodes.Count > 0 Then
                    Dim firstc As Xml.XmlNode = DestiPlaceNode.ChildNodes.Item(0)
                    DestiPlaceNode.InsertBefore(RepositionNode, firstc)
                Else
                    DestiPlaceNode.AppendChild(RepositionNode)
                End If
            Case RelationShip.LastChild
                DestiPlaceNode.AppendChild(RepositionNode)
            Case RelationShip.NextSibling
                DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode)
            Case RelationShip.PreviousSibling
                DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode)
        End Select


        RepositionNode.InnerXml = SrcNode.OuterXml

        Dim RepoId As Xml.XmlAttribute = XDoc.CreateAttribute("cs:repoID", NSht("cs"))
        RepositionNode.Attributes.Append(RepoId)
        RepoId.Value = clsReposition.GetRepositionID

        Dim MovId As Xml.XmlAttribute = XDoc.CreateAttribute("cs_moved")
        RepositionNode.Attributes.Append(MovId)
        MovId.Value = "1"



        RepositionHide(SrcNode, RepoId.Value)


    End Sub
    Public Sub FootnoteReposition(ByVal SourceNodeXpath As Xml.XmlNode, ByVal DestinationPlaceXpath As Xml.XmlNode, ByVal withRelation As RelationShip, Optional ByVal typename As String = "")
        Dim SrcNode As Xml.XmlNode = SourceNodeXpath

        Dim DestiPlaceNode As Xml.XmlNode = DestinationPlaceXpath

        If SrcNode Is Nothing Or DestiPlaceNode Is Nothing Then
            Exit Sub
        End If

        Dim RepositionNode As Xml.XmlNode = XDoc.CreateElement("cs:footnote", NSht("cs"))
        Dim attr As Xml.XmlAttribute = XDoc.CreateAttribute("cs_type")
        attr.Value = typename
        RepositionNode.Attributes.Append(attr)
        Select Case withRelation
            Case RelationShip.FirstChild
                If DestiPlaceNode.ChildNodes.Count > 0 Then
                    Dim firstc As Xml.XmlNode = DestiPlaceNode.ChildNodes.Item(0)
                    DestiPlaceNode.InsertBefore(RepositionNode, firstc)
                Else
                    DestiPlaceNode.AppendChild(RepositionNode)
                End If
            Case RelationShip.LastChild
                DestiPlaceNode.AppendChild(RepositionNode)
            Case RelationShip.NextSibling
                DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode)
            Case RelationShip.PreviousSibling
                DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode)
        End Select


        RepositionNode.InnerXml = SrcNode.OuterXml

        Dim RepoId As Xml.XmlAttribute = XDoc.CreateAttribute("cs:repoID", NSht("cs"))
        RepositionNode.Attributes.Append(RepoId)
        RepoId.Value = clsReposition.GetRepositionID

        'Dim fType As Xml.XmlAttribute = XDoc.CreateAttribute("cs_type")
        'RepositionNode.Attributes.Append(fType)
        'fType.Value = "footnote"


        FootnoteRepositionHide(SrcNode, RepoId.Value, typename)

    End Sub

    Private Sub FootnoteRepositionHide(ByVal SrcNode As XmlNode, ByVal withRepoID As String, Optional ByVal typeName As String = "")
        'Dim RepoHide As Xml.XmlProcessingInstruction = XDoc.CreateProcessingInstruction("cs_RepositionHide", "<cs:repoID=" + withRepoID + "/>")
        Dim RepoHide As Xml.XmlNode = XDoc.CreateElement("cs_footnote")
        Dim attr As XmlAttribute = XDoc.CreateAttribute("repoID")
        attr.Value = withRepoID
        RepoHide.Attributes.Append(attr)
        Dim typeNameAttr As XmlAttribute = XDoc.CreateAttribute("typename")
        typeNameAttr.Value = typeName
        RepoHide.Attributes.Append(typeNameAttr)
        If (typeName.ToLower = "endnote") Then
            RepoHide.InnerXml = "<cs_text type=""superscript"">" + SrcNode.SelectSingleNode(".//cs_text[@type='footnotenumber']").InnerText + "</cs_text>" + "<cs_text forced=""true"">" + "</cs_text>"
        Else
            RepoHide.InnerXml = "<cs_text forced=""true"">" + "</cs_text>"
        End If
        'Dim datapart As String = SrcNode.OuterXml
        'datapart = ClearDataPart(datapart)
        'RepoHide.Data += datapart

        Try
            SrcNode.ParentNode.ReplaceChild(RepoHide, SrcNode)
        Catch ex As Exception
        End Try

    End Sub



    Public Sub FrameReposition(ByVal SourceNodeXpath As Xml.XmlNode, ByVal DestinationPlaceXpath As Xml.XmlNode, ByVal withRelation As RelationShip, Optional ByVal typename As String = "")
        Dim SrcNode As Xml.XmlNode = SourceNodeXpath

        Dim DestiPlaceNode As Xml.XmlNode = DestinationPlaceXpath

        If SrcNode Is Nothing Or DestiPlaceNode Is Nothing Then
            Exit Sub
        End If

        Dim RepositionNode As Xml.XmlNode = XDoc.CreateElement("cs:frame", NSht("cs"))
        Select Case withRelation
            Case RelationShip.FirstChild
                If DestiPlaceNode.ChildNodes.Count > 0 Then
                    Dim firstc As Xml.XmlNode = DestiPlaceNode.ChildNodes.Item(0)
                    DestiPlaceNode.InsertBefore(RepositionNode, firstc)
                Else
                    DestiPlaceNode.AppendChild(RepositionNode)
                End If
            Case RelationShip.LastChild
                DestiPlaceNode.AppendChild(RepositionNode)
            Case RelationShip.NextSibling
                DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode)
            Case RelationShip.PreviousSibling
                DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode)
        End Select


        RepositionNode.InnerXml = SrcNode.OuterXml

        Dim RepoId As Xml.XmlAttribute = XDoc.CreateAttribute("cs:repoID", NSht("cs"))
        RepositionNode.Attributes.Append(RepoId)
        RepoId.Value = clsReposition.GetRepositionID
        FrameRepositionHide(SrcNode, RepoId.Value, typename)

    End Sub
    Private Sub FrameRepositionHide(ByVal SrcNode As XmlNode, ByVal withRepoID As String, Optional ByVal typeName As String = "")
        'Dim RepoHide As Xml.XmlProcessingInstruction = XDoc.CreateProcessingInstruction("cs_RepositionHide", "<cs:repoID=" + withRepoID + "/>")
        Dim RepoHide As Xml.XmlNode = XDoc.CreateElement("cs_frame")
        Dim attr As XmlAttribute = XDoc.CreateAttribute("repoID")
        attr.Value = withRepoID
        RepoHide.Attributes.Append(attr)
        Dim typeNameAttr As XmlAttribute = XDoc.CreateAttribute("typename")
        typeNameAttr.Value = typeName
        RepoHide.Attributes.Append(typeNameAttr)
        RepoHide.InnerXml = "<cs_text forced=""true"">" + "</cs_text>"
        'Dim datapart As String = SrcNode.OuterXml
        'datapart = ClearDataPart(datapart)
        'RepoHide.Data += datapart

        Try
            SrcNode.ParentNode.ReplaceChild(RepoHide, SrcNode)
        Catch ex As Exception
        End Try

    End Sub
    Public Sub Reposition(ByVal SourceNodeXpath As Xml.XmlNode, ByVal DestinationPlaceXpath As Xml.XmlNode, ByVal withRelation As RelationShip, Optional ByVal typename As String = "", Optional ByVal TypeID As String = "")
        Dim SrcNode As Xml.XmlNode = SourceNodeXpath
        Dim isInlineTable As Boolean = False
        Dim DestiPlaceNode As Xml.XmlNode = DestinationPlaceXpath

        If SrcNode Is Nothing Or DestiPlaceNode Is Nothing Then
            Exit Sub
        End If
        Try
            If (SrcNode.Name.ToLower = "cs:table" And SrcNode.Attributes("Float").Value.ToString.ToLower = "no") Then
                isInlineTable = True
            Else
                isInlineTable = False
            End If
        Catch ex As Exception
            isInlineTable = False
        End Try
        Dim RepositionNode As Xml.XmlNode = XDoc.CreateElement("cs:reposition", NSht("cs"))
        If (isInlineTable = False) Then
            Select Case withRelation
                Case RelationShip.FirstChild
                    If DestiPlaceNode.ChildNodes.Count > 0 Then
                        Dim firstc As Xml.XmlNode = DestiPlaceNode.ChildNodes.Item(0)
                        DestiPlaceNode.InsertBefore(RepositionNode, firstc)
                    Else
                        DestiPlaceNode.AppendChild(RepositionNode)
                    End If
                Case RelationShip.LastChild
                    DestiPlaceNode.AppendChild(RepositionNode)
                Case RelationShip.NextSibling
                    DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode)
                Case RelationShip.PreviousSibling
                    DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode)
            End Select
        Else
            SrcNode.ParentNode.InsertAfter(RepositionNode, SrcNode)
            Dim pStyle As Xml.XmlAttribute = XDoc.CreateAttribute("cs_IsInlineTable")
            pStyle.Value = "Yes"
            RepositionNode.Attributes.Append(pStyle)
        End If



        RepositionNode.InnerXml = SrcNode.OuterXml

        'Dim RepoId As Xml.XmlAttribute = XDoc.CreateAttribute("cs:repoID", NSht("cs"))
        'RepositionNode.Attributes.Append(RepoId)
        'RepoId.Value = clsReposition.GetRepositionID
        'RepositionHide(SrcNode, RepoId.Value, typename)
        Dim RepoId As Xml.XmlAttribute = XDoc.CreateAttribute("cs:repoID", NSht("cs"))
        RepositionNode.Attributes.Append(RepoId)
        RepoId.Value = clsReposition.GetRepositionID
        Dim cs_ID As Xml.XmlAttribute = XDoc.CreateAttribute("cs_ID")
        cs_ID.Value = TypeID
        RepositionNode.Attributes.Append(cs_ID)
        RepositionHide(SrcNode, RepoId.Value, typename, TypeID)
    End Sub
    Private Sub RepositionHide(ByVal SrcNode As XmlNode, ByVal withRepoID As String, Optional ByVal typeName As String = "", Optional ByVal TypeIdVal As String = "")
        'Dim RepoHide As Xml.XmlProcessingInstruction = XDoc.CreateProcessingInstruction("cs_RepositionHide", "<cs:repoID=" + withRepoID + "/>")
        Dim RepoHide As Xml.XmlNode = XDoc.CreateElement("cs_repos")
        Dim attr As XmlAttribute = XDoc.CreateAttribute("repoID")
        Dim IsInline As Boolean = False
        Try
            If (SrcNode.Name.ToLower = "figure" And SrcNode.Attributes("Float").Value.ToLower = "no") Then
                IsInline = True
            Else
                IsInline = False
            End If
        Catch ex As Exception
            IsInline = False
        End Try


        attr.Value = withRepoID
        RepoHide.Attributes.Append(attr)
        Dim typeNameAttr As XmlAttribute = XDoc.CreateAttribute("typename")
        typeNameAttr.Value = typeName
        RepoHide.Attributes.Append(typeNameAttr)

        Dim cs_ID As Xml.XmlAttribute = XDoc.CreateAttribute("cs_ID")
        cs_ID.Value = TypeIdVal
        RepoHide.Attributes.Append(cs_ID)

        RepoHide.InnerXml = "<cs_text type=""" + TypeIdVal + """>" + "</cs_text>"

        'RepoHide.InnerXml = "<cs_text forced=""true"">" + "</cs_text>"
        'Dim datapart As String = SrcNode.OuterXml
        'datapart = ClearDataPart(datapart)
        'RepoHide.Data += datapart

        Try
            SrcNode.ParentNode.ReplaceChild(RepoHide, SrcNode)
            If (IsInline) Then
                Dim inline As Xml.XmlAttribute = XDoc.CreateAttribute("cs_IsInline")
                inline.Value = "Yes"
                RepoHide.Attributes.Append(inline)
                Dim EnterMark As XmlElement = XDoc.CreateElement("cs_text")
                Dim typeA As XmlAttribute = XDoc.CreateAttribute("type")
                typeA.Value = "EnterBeforeInlineFig"
                EnterMark.Attributes.Append(typeA)
                EnterMark.InnerXml = vbNewLine
                RepoHide.ParentNode.InsertAfter(EnterMark, RepoHide)
            End If
        Catch ex As Exception
        End Try

    End Sub


End Class
Public Enum RelationShip
    PreviousSibling = 1
    NextSibling = 2
    FirstChild = 3
    LastChild = 4
End Enum

