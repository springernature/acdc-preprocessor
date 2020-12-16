Imports System.Xml
Imports System.IO
Imports System.Text.RegularExpressions
Public Class ClsEntity


    Private _EntityXML_Path As String = String.Empty
    Private _InXML_Path As String = String.Empty
    Private _OutXML_Path As String = String.Empty


    Public Property EntityXML_Path() As String
        Get
            Return _EntityXML_Path
        End Get
        Set(ByVal value As String)
            _EntityXML_Path = value
        End Set
    End Property

    Public Property InXML_Path() As String
        Get
            Return _InXML_Path
        End Get
        Set(value As String)
            _InXML_Path = value
        End Set
    End Property
    Public Property OutXML_Path() As String
        Get
            Return _InXML_Path
        End Get
        Set(value As String)
            _InXML_Path = value
        End Set
    End Property
    Public Sub New()

    End Sub
    Public Sub New(ByVal InputXMl_Path As String, ByVal outXmlFile As String, ByVal EntityXMLPath As String)
        Try
            InXML_Path = InputXMl_Path
            EntityXML_Path = EntityXMLPath
            OutXML_Path = outXmlFile
            '  Dim val As Boolean = ConvertSymbolEntities(InputXMl_Path, OutXML_Path, EntityXML_Path)
        Catch ex As Exception
        End Try
    End Sub
    ''Public Function ConvertEntities(ByVal inXmlFile As String, ByVal outXmlFile As String, ByVal EntityXML_Path As String) As Boolean
    ''    Try
    ''        Dim inXmlString As String
    ''        Dim sr As New StreamReader(inXmlFile)
    ''        inXmlString = sr.ReadToEnd
    ''        sr.Close()

    ''        Dim Xdoc As New Xml.XmlDocument
    ''        Xdoc.PreserveWhitespace = True
    ''        Xdoc.Load(inXmlFile)
    ''        '==============================================
    ''        '1.Write regular expression to convert all unicode to xmlelement <cs_text type='unicode'>
    ''        Xdoc.InnerXml = Regex.Replace(Xdoc.InnerXml, "(&#x[A-Za-z0-9]{4};)", "<cs_unicode>$1</cs_unicode>", RegexOptions.Singleline)
    ''        '2.Find all cs_unicode xmlelement and chek its parent if emphasis then add cstyle='style'
    ''        Dim all_cs_unicodeNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//cs_unicode")
    ''        If (IsNothing(all_cs_unicodeNd) = False And all_cs_unicodeNd.Count > 0) Then
    ''            For i As Integer = 0 To all_cs_unicodeNd.Count - 1
    ''                Dim Nd As Xml.XmlNode = all_cs_unicodeNd(i)
    ''                Dim attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cstyle")
    ''                If (Nd.ParentNode.Name.ToLower = "emphasis") Then
    ''                    attr.Value = Nd.ParentNode.Attributes.ItemOf("Type").Value
    ''                Else
    ''                    attr.Value = "regular"
    ''                End If
    ''                Nd.Attributes.Append(attr)
    ''            Next
    ''        End If
    ''        '3.Now as per sequence of font Symbol|arial|calibri|Windings|Webdings find 
    ''        'current input is <cs_unicode cstyle="regular|bold|italic|bolditalic">...</cs_unicode>

    ''        '==============================================
    ''        Dim sw As New StreamWriter(outXmlFile, False, System.Text.Encoding.GetEncoding(1252))
    ''        sw.Write(Xdoc.OuterXml)
    ''        sw.Close()
    ''        Return True
    ''    Catch ex As Exception
    ''        Return False
    ''    End Try
    ''    Return False
    ''End Function
    Public Function ConvertSymbolEntities(ByVal XmlStr As String, ByVal EntityXML_Path As String) As String
        Try
            Dim inXmlString As String = XmlStr

            ''Load entity.xml document 
            Dim EntityXmlDoc As New XmlDocument
            Try
                EntityXmlDoc.Load(EntityXML_Path)
            Catch ex As Exception
                '  Return False
            End Try

            Dim sortstring As New ArrayList
            ''Check for Unicodevalues in XML
            Dim Reg As New Regex("&#x[A-Za-z0-9]{4};")
            Dim CharAttributes As MatchCollection = Reg.Matches(inXmlString)
            Dim lk As Integer = 0
            Try
                For Each CharAttribute As Match In CharAttributes
                    sortstring.Add(CharAttribute.Groups(0).Value.Replace("&amp;", "&"))
                Next
                sortstring.Sort()
                Dim count As Integer = sortstring.Count
                Dim ii As Integer
                For ii = count - 1 To 1 Step -1
                    If (sortstring(ii).ToString() = sortstring(ii - 1).ToString()) Then
                        sortstring.RemoveAt(ii)
                    End If
                Next ii
                For i As Integer = 0 To sortstring.Count - 1
                    Dim entityVal As String = sortstring(i)
                    Dim entityUnicode As String = entityVal.Replace("&#x", "").Replace(";", "")
                   
                    Dim EntityNd As XmlNode = EntityXmlDoc.SelectSingleNode(".//Entity[(IFont='Symbol') and (Emp='Normal') and (IDUni='" + entityUnicode + "')]")
                    If (IsNothing(EntityNd) = False) Then
                        Dim EntityCharCode As String = EntityNd.SelectSingleNode(".//WCC").InnerText.ToString.Trim
                        inXmlString = inXmlString.Replace("&#x" + entityUnicode + ";", "<cs_text type='cSYMBOL'>&#x" + EntityCharCode + ";</cs_text>")
                    End If

                Next
            Catch ex As Exception

            End Try
            'End General Preprocess

            'File Writing
            'Dim sw As New StreamWriter(outXmlFile)
            'sw.Write(inXmlString)
            'sw.Close()
            'Return True
           
            Return inXmlString
        Catch ex As Exception
            'Return False
        End Try
        ' Return False
    End Function

    ''Public Function ConvertEntities(ByVal inXmlFile As String, ByVal outXmlFile As String, ByVal EntityXML_Path As String) As Boolean
    ''    Try
    ''        Dim inXmlString As String
    ''        Dim sr As New StreamReader(inXmlFile)
    ''        inXmlString = sr.ReadToEnd
    ''        sr.Close()
    ''        ''Load entity.xml document 

    ''        Dim MEvalItalic As New MatchEvaluator(AddressOf MapItalicEntities)
    ''        inXmlString = Regex.Replace(inXmlString, "(<Emphasis Type=""([^>]+))"">(&#x[A-Za-z0-9]{4};)</Emphasis>", MEvalItalic)


    ''        ' ''Check for Unicodevalues in XML
    ''        'Dim Reg As New Regex("(<Emphasis Type=""([^>]+))"">&#x[a-z0-9]{4};</Emphasis>") 'All emphasis entity''''"&#x[a-z0-9]{4};"
    ''        'Dim CharAttributes As MatchCollection = Reg.Matches(inXmlString)
    ''        'Try
    ''        '    For Each CharAttribute As Match In CharAttributes
    ''        '        ''Dim entityVal As String = CharAttribute.Groups(0).Value.Replace("&amp;", "&")
    ''        '        ''Dim entityUnicode As String = entityVal.Replace("&", "").Replace("#", "").Replace(";", "")
    ''        '        ''Dim EntityNd As XmlNode = EntityXmlDoc.SelectSingleNode(".//Entity[@Unicode='" + entityUnicode + "']")
    ''        '        ''Dim CharStyle As String = EntityNd.Attributes.ItemOf("CharStyle").Value.ToString.Trim
    ''        '        ''Dim EntityName As String = EntityNd.Attributes.ItemOf("name").Value.ToString.Trim
    ''        '        ''Dim EntityCharCode As String = EntityNd.Attributes.ItemOf("CharCode").Value.ToString.Trim
    ''        '        ''inXmlString = inXmlString.Replace("&#" + entityUnicode + ";", "<cs_text type=""entity"" CharStyle=""" + CharStyle + """ name=""" + EntityName + """ WordCode=""&#" + entityUnicode + """>&#x" + EntityCharCode + ";</cs_text>")


    ''        '        'Dim entityVal As String = CharAttribute.Groups(0).Value.Replace("&amp;", "&")
    ''        '        'Dim entityUnicode As String = entityVal.Replace("&", "").Replace("#", "").Replace(";", "")
    ''        '        'Dim EntityNd As XmlNode = EntityXmlDoc.SelectSingleNode(".//Entity[IDUni['" + entityUnicode + "']]") '[@Unicode='" + entityUnicode + "']")
    ''        '        'Dim CharStyle As String = EntityNd.SelectSingleNode(".//IFont").InnerText.ToString.Trim
    ''        '        'Dim EntityName As String = EntityNd.SelectSingleNode(".//Name").InnerText.ToString.Trim
    ''        '        'Dim EntityCharCode As String = EntityNd.SelectSingleNode(".//WCC").InnerText.ToString.Trim
    ''        '        'inXmlString = inXmlString.Replace("&#" + entityUnicode + ";", "<Emphasis type='c" + CharStyle + "'>" + EntityCharCode + "</Emphasis>")
    ''        '        '   inXmlString = inXmlString.Replace("&#" + entityUnicode + ";", "<cs_text type=""entity"" CharStyle=""" + CharStyle + """ name=""" + EntityName + """ WordCode=""&#" + entityUnicode + """>&#x" + EntityCharCode + ";</cs_text>")

    ''        '    Next
    ''        'Catch ex As Exception

    ''        'End Try
    ''        'End General Preprocess

    ''        'File Writing
    ''        Dim sw As New StreamWriter(inXmlString)
    ''        sw.Write(outXmlFile)
    ''        sw.Close()
    ''        Return True
    ''    Catch ex As Exception
    ''        Return False
    ''    End Try
    ''    Return False
    ''End Function
    Public Function MapItalicEntities(ByVal m As Match) As String
        Dim WDEmp As String = m.Groups(2).Value
        Dim WDFont As String = "Symbol" 'm.Groups(2).Value
        Dim WDHexName As String = m.Groups(3).Value.Replace("&#x", "").Replace(";", "")
        'System.Configuration.ConfigurationSettings.AppSettings("EntityPath") '"D:\Suresh\EntWord2Indd\Entities.xml"

        Dim EntityXmlDoc As New XmlDocument
        Try
            EntityXmlDoc.Load(EntityXML_Path)
        Catch ex As Exception
            '  Return False
        End Try


        'Dim entityDoc As New Xml.XmlDocument
        'entityDoc.Load(EntityPathxml)
        Dim entities As Xml.XmlNode = EntityXmlDoc.SelectSingleNode(".//Entity[WFont='" + WDFont + "' and Emp='" + WDEmp + "' and WCC='" + WDHexName + "']")
        Try
            If (IsNothing(entities) = False) Then
                Dim EntityTag As String = "<Emphasis type=" + """" + "c" + entities.ChildNodes(5).InnerText + """" + ">"
                EntityTag += "&#x" + entities.ChildNodes(6).InnerText + ";</Emphasis>"
                Return EntityTag
            End If
        Catch ex As Exception

        End Try
    End Function
End Class
