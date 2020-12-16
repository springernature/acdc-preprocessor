﻿'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : SUVARNA RAUT
'CLASS NAME    : clsFigureConversion
'CREATED DATE  : 3RD JUNE 2013
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
Imports System.Xml
Imports System.Text.RegularExpressions
Imports MySql.Data.MySqlClient
Public Class clsFigureConversion
    Private FigureSettingXmlPath As String = ""
    Private FigureNodeName As String
    Private FigureIdName As String
    Private FigureRefNodeName As String
    Private FigureRefId As String
    Private FigureRefIdPattern As String
    Private FigureCaption As String
    Private FigureCaptionNode As String
    Private FigurePathXPath As String
    Private DatabaseString As String
    Private FigureServerPath As String
    Private BaseDirectory As String
    Private FigureXMLPath As String
    Public XDoc As Xml.XmlDocument
    Private NTable As Xml.NameTable
    Private NS As Xml.XmlNamespaceManager
    Private NSht As New Hashtable
    Private FigureSettings As New Xml.XmlDocument
    Private oreq As clsPreprocMain
    Private EquationPathXpath As String
    Dim FInputXml As String = ""
    Public Sub New(ByRef Xddoc As XmlDocument, ByVal nmtab As NameTable, ByVal NSm As XmlNamespaceManager, ByVal oreqq As clsPreprocMain)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:New
        'PARAMETER    :Xddoc,nmtab,NSm,oreqq
        'AIM          :This is constructor.It initailise the data member
        '=============================================================================================================
        '=============================================================================================================
        XDoc = Xddoc
        NTable = nmtab
        NS = NSm
        oreq = oreqq
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Sub FigureConversion(ByVal FigureSettingsXMl As String, ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:FigureConversion
        'PARAMETER    :Xddoc,nmtab,NSm,oreqq
        'AIM          :This function call the another function used to do figure conversion
        '=============================================================================================================
        '=============================================================================================================
        FInputXml = inputxml
        FigureSettingXmlPath = FigureSettingsXMl
        Initialize(FigureSettingXmlPath)
        DownLoadImages(inputxml)
        DefineBiographyFigurePath(inputxml)
        DownLoadEquation(inputxml)
        Convert()
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Private Sub Initialize(ByVal FromXml)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Initialize
        'PARAMETER    :FromXml
        'AIM          :This function initialize the variables.

        '=============================================================================================================
        '=============================================================================================================
        FigureSettings.LoadXml(FromXml)
        Dim FigureSettingNode As Xml.XmlNode = FigureSettings.SelectSingleNode(".//FigureSettings")
        BaseDirectory = FigureSettingNode.SelectSingleNode(".//BaseDirectory").InnerText
        FigureNodeName = FigureSettingNode.SelectSingleNode(".//FigureElementName").InnerText
        FigureIdName = FigureSettingNode.SelectSingleNode(".//FigureElementName/@idAttribute").Value
        FigureRefNodeName = FigureSettingNode.SelectSingleNode(".//FigureRefElementName").InnerText
        FigureRefId = FigureSettingNode.SelectSingleNode(".//FigureRefElementName/@idAttribute").Value
        FigureRefIdPattern = FigureSettingNode.SelectSingleNode(".//FigureRefElementName/@IDpattern").Value
        FigureCaption = FigureSettingNode.SelectSingleNode(".//FigureCaptionElement").InnerText
        FigurePathXPath = FigureSettingNode.SelectSingleNode(".//FigurePathXpath").InnerText
        FigureServerPath = FigureSettingNode.SelectSingleNode(".//FigureServerPath").InnerText
        FigureXMLPath = FigureSettingNode.SelectSingleNode(".//FigureXMLPath").InnerText
        DatabaseString = FigureSettingNode.SelectSingleNode(".//DatabaseString").InnerText
        EquationPathXpath = FigureSettingNode.SelectSingleNode(".//EquationPathXpath").InnerText
        Try
            Dim UpdateStr As Boolean = False
            UpdateStr = UpdateDataBaseString(DatabaseString)
            Try
                If (UpdateStr = True) Then
                    DatabaseString = DatabaseString.Replace("dbcrest", "newdbcrest")
                End If
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function DefineBiographyFigurePath(ByVal inputxml As String)
        Try
            Dim BiographyNd As Xml.XmlNodeList = XDoc.SelectNodes(".//Biography[@cs_imageeixst='yes']")
            If (IsNothing(BiographyNd) = False) Then
                Try
                    'Dim hPath As String = BaseDirectory + "\" + FigurePathXPath
                    Dim hPath As String
                    Try
                        'get image path from hard drive if its null then get path on mentioned location
                        '  hPath = GetPathFromHardDrivePath(inputxml)


                        If (ACDCLayout = True) Then
                            hPath = ACDC_Graphics
                        Else
                            hPath = GetPathFromHardDrivePath(inputxml)
                        End If
                    Catch ex As Exception

                    End Try
                    If hPath <> "" Then
                        'hPath = hPath + "\Print"
                        hPath = hPath + "\Web"
                    End If
                    For i As Integer = 0 To BiographyNd.Count - 1
                        Try
                            Dim InnerNode As Xml.XmlNode = BiographyNd(i)
                            Dim AllNewImages() As String = System.IO.Directory.GetFiles(hPath)
                            For Each fl As String In AllNewImages
                                Dim finf As New System.IO.FileInfo(fl)
                                Dim str As String = "" 'XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
                                Dim initial_letter As String = BiographyNd(i).Attributes.ItemOf("cs_Authorimagepath").Value
                                str = initial_letter.ToLower
                                str = str.Split(".")(0)
                                If (finf.Name.ToLower.Contains(str + ".eps") Or finf.Name.ToLower.Contains(str + ".tif") Or finf.Name.ToLower.Contains(str + ".jpg") Or finf.Name.ToLower.Contains(str + ".jpeg")) Then
                                    BiographyNd(i).Attributes.ItemOf("cs_Authorimagepath").Value = fl
                                End If
                            Next
                        Catch ex As Exception

                        End Try
                        'Added condition on 170610
                        If (BiographyNd(i).Attributes.ItemOf("cs_Authorimagepath").Value.Contains("\") = False) Then
                            ''BiographyNd(i).Attributes.ItemOf("cs_Authorimagepath").Value = "C:\FigNotFound.jpg"
                            BiographyNd(i).Attributes.ItemOf("cs_Authorimagepath").Value = "d:\FigNotFoundNew.jpg"
                        End If
                    Next
                Catch ex As Exception

                End Try
            End If
        Catch ex As Exception

        End Try
    End Function
    Private Function getEquationImagePath(ByVal HiresWorkingArea As String, ByVal figIndex As String, ByVal prefix As String) As String
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:getEquationImagePath
        'PARAMETER    :HiresWorkingArea,figIndex,prefix
        'AIM          :This function gives the path
        '=============================================================================================================
        '=============================================================================================================
        Dim strFilePath As String = ""

        Dim AllNewImages() As String = System.IO.Directory.GetFiles(HiresWorkingArea)
        For Each fl As String In AllNewImages
            Try
                Dim finf As New System.IO.FileInfo(fl)
                If finf.Name.ToLower().Contains(prefix.ToLower() + figIndex.ToLower() + ".eps") Or finf.Name.ToLower().Contains(prefix.ToLower() + figIndex.ToLower() + ".tif") Or finf.Name.ToLower().Contains(prefix.ToLower() + figIndex.ToLower() + ".jpg") Or finf.Name.ToLower().Contains(prefix.ToLower() + figIndex.ToLower() + ".jpeg") Then
                    strFilePath = fl
                    Exit For
                End If
            Catch ex As Exception
            End Try
        Next

        Return strFilePath
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Sub DownLoadEquation(ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:DownLoadEquation
        'PARAMETER    :inputxml
        'AIM          :This function download the equations
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim destPath As String = BaseDirectory + "\" + EquationPathXpath + "\" + System.IO.Path.GetFileNameWithoutExtension(OutPath) + "\Graphics\Equation"
            Dim GetFigPath As String = ""
            Dim HiresPath As String = ""
            Dim LowersPath As String = ""
            Dim EqPath As String = ""

            Try
                Dim finfo As New System.IO.DirectoryInfo(inputxml)
                Dim EquationPath As String = finfo.Parent.Parent.FullName
                GetFigPath = EquationPath + "\Graphics\Equation"
            Catch ex As Exception

            End Try
            Try
                Dim AllFigs As XmlNodeList = XDoc.SelectNodes(".//Equation|.//InlineEquation")
                For Each FigNd As XmlNode In AllFigs
                    Try
                        Try
                            Dim sourcePath As String = ""
                            Dim FigName As String = "Equ"
                            Try
                                If (FigNd.Name = "InlineEquation") Then
                                    FigName = FigNd.Attributes.ItemOf("ID").Value
                                Else
                                    If (Regex.Match(FigNd.Attributes.ItemOf("ID").Value, "Equ", RegexOptions.Singleline).Success = True) Then
                                        FigName += Regex.Match(FigNd.Attributes.ItemOf("ID").Value, "Equ(.+)", RegexOptions.Singleline).Groups(1).Value.ToString
                                        If (FigNd.Name = "InlineEquation") Then
                                            FigName = "I" + FigName
                                        End If
                                    End If

                                End If

                                Dim FigPathforXML As String = getEquationImagePath(destPath, FigName, "")
                                Dim EqAttr As Xml.XmlAttribute = XDoc.CreateAttribute("cs_EqPath")
                                EqAttr.Value = FigPathforXML
                                'FigNd.SelectSingleNode(FigureXMLPath).Value = FigPathforXML
                                FigNd.Attributes.Append(EqAttr)
                            Catch ex As Exception
                            End Try
                            'Check whether directory exist or not
                        Catch ex As Exception
                        End Try
                    Catch ex As Exception

                    End Try
                Next
            Catch ex As Exception

            End Try

        Catch ex As Exception

        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    ''Public Function GetJournalNamePath(ByVal FigureConversionFile As String, ByVal inputxml As String)
    ''    '=============================================================================================================
    ''    '=============================================================================================================
    ''    'FUNCTION NAME:GetJournalNamePath
    ''    'PARAMETER    :FigureConversionFile,inputxml
    ''    'AIM          :This function return journal name from database
    ''    '=============================================================================================================
    ''    '=============================================================================================================
    ''    Dim fsConn As String = ""
    ''    Dim JobSheetID As String = ""
    ''    Dim JournalName As String = "JournalName"
    ''    Try
    ''        Initialize(FigureConversionFile)
    ''        fsConn = DatabaseString
    ''    Catch ex As Exception

    ''    End Try
    ''    Try
    ''        Dim mconn As New MySqlConnection(fsConn)
    ''        Try
    ''            mconn.Open()
    ''            Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
    ''            Dim cdm As New MySqlCommand("select JobSheetid from jobsheetinfo where DOI = '" + str + "'", mconn)
    ''            Dim reader As MySqlDataReader
    ''            reader = cdm.ExecuteReader
    ''            If reader.HasRows = True Then
    ''                While reader.Read()
    ''                    JobSheetID = reader("JobSheetid").ToString()
    ''                End While
    ''            End If
    ''            mconn.Close()
    ''            mconn.Dispose()
    ''        Catch ex As Exception

    ''        End Try
    ''    Catch ex As Exception

    ''    End Try
    ''    Try
    ''        Dim mconn As New MySqlConnection(fsConn)
    ''        mconn.Open()
    ''        Dim cdm As New MySqlCommand("select AbbreviatedTitle from tlbbookjournalseriessubseries where Id = '" + JobSheetID + "'", mconn)
    ''        Dim reader As MySqlDataReader
    ''        reader = cdm.ExecuteReader
    ''        If reader.HasRows = True Then
    ''            While reader.Read()
    ''                JournalName = reader("AbbreviatedTitle").ToString()
    ''            End While
    ''        End If
    ''        mconn.Close()
    ''        mconn.Dispose()
    ''    Catch ex As Exception

    ''    End Try
    ''    Try
    ''        If (ACDCLayout = True) Then
    ''            JournalName = ""
    ''            If (IsNothing(ACDCXdoc.SelectSingleNode(".//JournalAbbreviatedTitle")) = False) Then
    ''                JournalName = ACDCXdoc.SelectSingleNode(".//JournalAbbreviatedTitle").InnerXml
    ''            End If

    ''        End If
    ''    Catch ex As Exception

    ''    End Try
    ''    Return JournalName
    ''    '====================================================END======================================================
    ''    '=============================================================================================================
    ''End Function
    Public Function GetJournalNamePath(ByVal FigureConversionFile As String, ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetJournalNamePath
        'PARAMETER    :FigureConversionFile,inputxml
        'AIM          :This function return journal name from database
        '=============================================================================================================
        '=============================================================================================================
        Dim fsConn As String = ""
        Dim JobSheetID As String = ""
        Dim JournalName As String = "JournalName"
        'Below code is commented because currently we are taking abbr title from latest jobsheet xml as per requirement 26.08.2015
        Dim jobsheetxmlpath As String = ACDC_Jonsheetpath
        'Try
        '    jobsheetxmlpath = GetHarddrivepath(inputxml)
        'Catch ex As Exception

        'End Try

        Try
            'jobsheetxmlpath = jobsheetxmlpath.Replace("\Graphics\", "\")
            'Dim stage_temp As String = ""
            'Dim dirInfo As New System.IO.DirectoryInfo(jobsheetxmlpath)
            'jobsheetxmlpath = dirInfo.Parent.Parent.FullName + "\Manuscript_300"

            'If (System.IO.Directory.Exists(jobsheetxmlpath)) Then
            '    stage_temp = "300"
            'Else
            '    jobsheetxmlpath = dirInfo.Parent.Parent.FullName + "\Manuscript_200"
            '    stage_temp = "200"
            'End If

            'Try
            '    If (System.IO.Directory.Exists(jobsheetxmlpath + "\Revision 5") = True) Then
            '        jobsheetxmlpath = jobsheetxmlpath + "\Revision 5\" + inputxml.Split("\")(inputxml.Split("\").Length - 1).Replace("_Article.xml", "_JobSheet_" + stage_temp + ".xml")
            '    ElseIf (System.IO.Directory.Exists(jobsheetxmlpath + "\Revision 4") = True) Then
            '        jobsheetxmlpath = jobsheetxmlpath + "\Revision 4\" + inputxml.Split("\")(inputxml.Split("\").Length - 1).Replace("_Article.xml", "_JobSheet_" + stage_temp + ".xml")
            '    ElseIf (System.IO.Directory.Exists(jobsheetxmlpath + "\Revision 3") = True) Then
            '        jobsheetxmlpath = jobsheetxmlpath + "\Revision 3\" + inputxml.Split("\")(inputxml.Split("\").Length - 1).Replace("_Article.xml", "_JobSheet_" + stage_temp + ".xml")
            '    ElseIf (System.IO.Directory.Exists(jobsheetxmlpath + "\Revision 2") = True) Then
            '        jobsheetxmlpath = jobsheetxmlpath + "\Revision 2\" + inputxml.Split("\")(inputxml.Split("\").Length - 1).Replace("_Article.xml", "_JobSheet_" + stage_temp + ".xml")
            '    ElseIf (System.IO.Directory.Exists(jobsheetxmlpath + "\Revision 1") = True) Then
            '        jobsheetxmlpath = jobsheetxmlpath + "\Revision 1\" + inputxml.Split("\")(inputxml.Split("\").Length - 1).Replace("_Article.xml", "_JobSheet_" + stage_temp + ".xml")
            '    ElseIf (System.IO.Directory.Exists(jobsheetxmlpath + "\Fresh") = True) Then
            '        jobsheetxmlpath = jobsheetxmlpath + "\Fresh\" + inputxml.Split("\")(inputxml.Split("\").Length - 1).Replace("_Article.xml", "_JobSheet_" + stage_temp + ".xml")
            '    End If
            'Catch ex As Exception

            'End Try
            If (System.IO.File.Exists(jobsheetxmlpath)) Then
                Dim jobsheetxdoc As New Xml.XmlDocument
                jobsheetxdoc.Load(jobsheetxmlpath)
                If (IsNothing(jobsheetxdoc.SelectSingleNode(".//JournalAbbreviatedTitle")) = False) Then
                    JournalName = jobsheetxdoc.SelectSingleNode(".//JournalAbbreviatedTitle").InnerXml
                    GlobalRunningOption = ""
                    If (IsNothing(jobsheetxdoc.SelectSingleNode(".//Typesetting/@RunningHead")) = False) Then
                        GlobalRunningOption = jobsheetxdoc.SelectSingleNode(".//Typesetting/@RunningHead").Value
                    End If
                End If

            End If
        Catch ex As Exception

        End Try

        Try
            If (ACDCLayout = True) Then
                JournalName = ""
                If (IsNothing(ACDCXdoc.SelectSingleNode(".//JournalAbbreviatedTitle")) = False) Then
                    JournalName = ACDCXdoc.SelectSingleNode(".//JournalAbbreviatedTitle").InnerXml
                    GlobalRunningOption = ""
                    If (IsNothing(ACDCXdoc.SelectSingleNode(".//Typesetting/@RunningHead")) = False) Then
                        GlobalRunningOption = ACDCXdoc.SelectSingleNode(".//Typesetting/@RunningHead").Value
                    End If
                End If
            End If
        Catch ex As Exception

        End Try
        Return JournalName
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Sub DownLoadImages(ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:DownLoadImages
        'PARAMETER    :inputxml
        'AIM          :This function set figure image path in xml
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim chpaterNd As Xml.XmlNode = XDoc.SelectSingleNode(".//Chapter") 'chpaterNd.Attributes.Itemof("ID").Value
            Dim ChapterType As Xml.XmlNode = XDoc.SelectSingleNode(".//ChapterInfo")
            Dim destPath As String = ""
            Dim WebDestPath As String = ""
            Dim GetFigPath As String = ""
            Dim HiresPath As String = ""
            Dim LowersPath As String = ""
            Try
                'get image path from hard drive if its null then get path on mentioned location
                If (ACDCLayout = True) Then
                    GetFigPath = ACDC_Graphics
                Else
                    GetFigPath = GetPathFromHardDrivePath(inputxml)
                End If

            Catch ex As Exception

            End Try
            If GetFigPath <> "" Then
                ' destPath = GetFigPath + "\Print"
                destPath = GetFigPath + "\Web"
                WebDestPath = GetFigPath + "\Web"
            End If

            Try
                Dim AllFigs As XmlNodeList = XDoc.SelectNodes(".//" + FigureNodeName)
                For Each FigNd As XmlNode In AllFigs
                    Try
                        Try
                            Dim sourcePath As String = ""
                            Dim FigName As String = "" ''''"Fig"
                            Dim FigSubIndex As String = ""
                            Try
                                If (Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "_HTML", RegexOptions.Singleline).Success = True) Then
                                    Try
                                        Dim figref_attr As Xml.XmlAttribute = XDoc.CreateAttribute("cs_FileRef")
                                        figref_attr.Value = FigNd.SelectSingleNode(FigureXMLPath).Value
                                        FigNd.Attributes.Append(figref_attr)
                                    Catch ex As Exception

                                    End Try
                                    ''                                    Try
                                    ''                                        If (FigNd.SelectSingleNode(FigureXMLPath).Value.Contains("Tab")) Then
                                    ''                                            FigName = "Tab" + Integer.Parse(Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "(\d+)([a-zA-Z]?(-[a-zA-Z])?)_HTML.(gif|jpg|eps|tif)", RegexOptions.Singleline).Groups(1).Value).ToString ' FigNd.SelectSingleNode(FigurePathXPath).Value
                                    ''                                        Else
                                    ''                                            FigName += Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "_Fig?(\d+)?([a-zA-Z]?[a-zA-Z]?)_HTML.(gif|jpg)", RegexOptions.Singleline).Groups(1).Value
                                    ''                                            If (FigName = "Fig") Then
                                    ''                                                FigName =
                                    ''""
                                    ''                                                FigName += Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "_Sch?(\d+)?([a-zA-Z]?[a-zA-Z]?)_HTML.(gif|jpg)", RegexOptions.Singleline).Groups(1).Value
                                    ''                                                If (FigName = "Sch") Then
                                    ''                                                    FigName += Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "_Sch?(\d+)?([a-zA-Z]?[a-zA-Z]?(\d+))_HTML.(gif|jpg)", RegexOptions.Singleline).Groups(1).Value
                                    ''                                                End If
                                    ''                                                FigSubIndex = Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "_Sch?(\d+)?([a-zA-Z]?[a-zA-Z]?)_HTML.(gif|jpg)", RegexOptions.Singleline).Groups(2).Value
                                    ''                                                If (FigSubIndex = "") Then
                                    ''                                                    FigSubIndex = Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "_Sch?(\d+)?([a-zA-Z]?[a-zA-Z]?(\d+)?)_HTML.(gif|jpg)", RegexOptions.Singleline).Groups(2).Value
                                    ''                                                End If
                                    ''                                            End If

                                    ''                                        End If
                                    ''                                    Catch ex As Exception

                                    ''                                    End Try

                                    ''                                    FigSubIndex = Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "(\d+)?([a-zA-Z]?)_HTML.(gif|jpg)", RegexOptions.Singleline).Groups(2).Value
                                    ''                                    FigName += FigSubIndex

                                    Try
                                        If (FigNd.SelectSingleNode(FigureXMLPath).Value.Contains("Tab")) Then
                                            FigName = "Tab"
                                        End If
                                        If (FigNd.SelectSingleNode(FigureXMLPath).Value.Contains("Fig")) Then
                                            FigName = "Fig"
                                        End If
                                        If (FigNd.SelectSingleNode(FigureXMLPath).Value.Contains("Sch")) Then
                                            FigName = "Sch"
                                        End If
                                        If (FigNd.SelectSingleNode(FigureXMLPath).Value.Contains("Str")) Then
                                            FigName = "Str"
                                        End If
                                        ''If (FigNd.SelectSingleNode(FigureXMLPath).Value.Contains("Equ")) Then
                                        ''    FigName = "Equ"
                                        ''End If
                                        ''If (FigNd.SelectSingleNode(FigureXMLPath).Value.Contains("IEq")) Then
                                        ''    FigName = "IEq"
                                        ''End If
                                    Catch ex As Exception

                                    End Try
                                    FigName += Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "(\d+)?([a-zA-Z]?[a-zA-Z]?)_HTML.(gif|jpg)", RegexOptions.Singleline).Groups(1).Value
                                    FigSubIndex = Regex.Match(FigNd.SelectSingleNode(FigureXMLPath).Value, "(\d+)?([a-zA-Z]?)_HTML.(gif|jpg)", RegexOptions.Singleline).Groups(2).Value
                                    FigName += FigSubIndex
                                End If
                                Dim FigPathforXML As String = getHiresImagePath(destPath, FigName, "")
                                FigNd.SelectSingleNode(FigureXMLPath).Value = FigPathforXML
                            Catch ex As Exception
                            End Try
                            'Check whether directory exist or not
                        Catch ex As Exception
                        End Try
                    Catch ex As Exception

                    End Try
                Next
            Catch ex As Exception

            End Try

        Catch ex As Exception

        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function GetPath(ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetPath
        'PARAMETER    :inputxml
        'AIM          :This function return harddrive path
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim figurepath As String = ""
            Dim harddrivepath As String = ""
            Dim fsConn As String = ""
            fsConn = DatabaseString
            Dim mconn As New MySqlConnection(fsConn)
            Try
                mconn.Open()
                Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
                Dim cdm As New MySqlCommand("select HardDrivePath from jobsheetinfo where DOI = '" + str + "'", mconn)
                Dim reader As MySqlDataReader
                reader = cdm.ExecuteReader
                If reader.HasRows = True Then
                    While reader.Read()
                        harddrivepath = reader("HardDrivePath").ToString()
                        Return harddrivepath
                    End While
                End If
                mconn.Close()
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function GetHarddrivepath(ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetHarddrivepath
        'PARAMETER    :inputxml
        'AIM          :This function return harddrive path
        '=============================================================================================================
        '=============================================================================================================
        Dim fsConn As String = ""
        fsConn = DatabaseString
        Dim HardDrivePath As String = ""
        Dim mconn As New MySqlConnection(fsConn)
        Try
            mconn.Open()
            Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
            Dim cdm As New MySqlCommand("select HardDrivePath from jobsheetinfo where DOI = '" + str + "'", mconn)
            Dim reader As MySqlDataReader
            reader = cdm.ExecuteReader
            If reader.HasRows = True Then
                While reader.Read()
                    HardDrivePath = reader("HardDrivePath").ToString()

                End While
            End If
            mconn.Close()
            mconn.Dispose()
        Catch ex As Exception

        End Try

        Dim OldDrive As String = System.Configuration.ConfigurationSettings.AppSettings("OldDrive").ToString()
        'EntityPathxml = "" 'System.Configuration.ConfigurationSettings.AppSettings("NewGraphicsPath").ToString()

        HardDrivePath = HardDrivePath.Replace(OldDrive, NewGraphicsPath)
        Return HardDrivePath
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function GetJobName(ByVal FigureConversionFile As String, ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetJobName
        'PARAMETER    :FigureConversionFile,inputxml
        'AIM          :This function return jobsheet id
        '=============================================================================================================
        '=============================================================================================================
        Dim fsConn As String = ""
        Try
            Initialize(FigureConversionFile)
            fsConn = DatabaseString
        Catch ex As Exception

        End Try
        Dim HardDrivePath As String = ""
        Dim mconn As New MySqlConnection(fsConn)
        Try
            mconn.Open()
            Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
            Dim cdm As New MySqlCommand("select jobsheetid from jobsheetinfo where JobType='Journal' and DOI = '" + str + "'", mconn)
            Dim reader As MySqlDataReader
            reader = cdm.ExecuteReader
            If reader.HasRows = True Then
                While reader.Read()
                    JobSheetID = reader("jobsheetid").ToString()
                End While
            End If
            mconn.Close()
            mconn.Dispose()
        Catch ex As Exception

        End Try
        Return JobSheetID
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function GetPathFromHardDrivePath(ByVal inputxml As String) As String
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetPathFromHardDrivePath
        'PARAMETER    :inputxml
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        Try
            Dim figurepath As String = ""
            Dim harddrivepath As String = ""
            Try
                harddrivepath = GetHarddrivepath(inputxml)
            Catch ex As Exception

            End Try

            Try
                Dim dirInfo As New System.IO.DirectoryInfo(harddrivepath)
                figurepath = dirInfo.Parent.Parent.FullName + "\Graphics"

                If (System.IO.Directory.Exists(figurepath)) Then
                    If (System.IO.Directory.Exists(figurepath)) Then
                        Return figurepath
                    End If
                End If
            Catch ex As Exception

            End Try

        Catch ex As Exception

        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function getHiresImagePath(ByVal HiresWorkingArea As String, ByVal figIndex As String, ByVal prefix As String) As String
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:getHiresImagePath
        'PARAMETER    :HiresWorkingArea,figIndex,prefix
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        Dim strFilePath As String = ""

        Dim AllNewImages() As String = System.IO.Directory.GetFiles(HiresWorkingArea)
        For Each fl As String In AllNewImages
            Try
                Dim finf As New System.IO.FileInfo(fl)
                'If finf.Name.ToLower.Contains(prefix.ToLower() + figIndex.ToLower() + "_print" + ".eps") Or finf.Name.ToLower.Contains(prefix.ToLower() + figIndex.ToLower() + "_print" + ".tif") Or finf.Name.ToLower.Contains(prefix.ToLower() + figIndex.ToLower() + "_print" + ".jpg") Or finf.Name.ToLower.Contains(prefix.ToLower() + figIndex.ToLower() + "_print" + ".jpeg") Then
                If finf.Name.ToLower.Contains(prefix.ToLower() + figIndex.ToLower() + "_html" + ".eps") Or finf.Name.ToLower.Contains(prefix.ToLower() + figIndex.ToLower() + "_html" + ".tif") Or finf.Name.ToLower.Contains(prefix.ToLower() + figIndex.ToLower() + "_html" + ".jpg") Or finf.Name.ToLower.Contains(prefix.ToLower() + figIndex.ToLower() + "_html" + ".jpeg") Then
                    strFilePath = fl
                    Exit For
                End If
            Catch ex As Exception
            End Try
        Next

        Return strFilePath
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Sub Convert()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Convert
        'PARAMETER    :-
        'AIM          :This function do the figure conversion
        '=============================================================================================================
        '=============================================================================================================
        'add figure identifiactions
        FigureNodeName += "[not(ancestor::Biography)]"
        Dim AllFigs As XmlNodeList = XDoc.SelectNodes(".//" + FigureNodeName)
        Dim i As Integer = 0
        Dim Figal As New ArrayList
        Dim replaceProc As New ArrayList
        For Each FigNd As XmlNode In AllFigs
            Try

                Dim FigureIdentiAttr As XmlAttribute = XDoc.CreateAttribute("cs_Fignode")
                'assuming figpath will be in some attribute
                Dim sepra As String = "\"
                If BaseDirectory = "" Then
                    sepra = ""
                End If
                Dim FigName As String = ""
                Try
                    FigName = FigNd.SelectSingleNode(FigureXMLPath).Value
                Catch ex As Exception
                End Try

                Dim AllImages() As String = Nothing

                'Figure Alogorithm
                If System.IO.File.Exists(FigName) Then
                    FigureIdentiAttr.Value = FigName
                Else
                    If IsNothing(AllImages) = False Then
                        If FigName.EndsWith(".eps") = False And FigName.EndsWith(".tif") = False And FigName.EndsWith(".jpg") = False Then
                            Try
                                Dim FigID As String = ""
                                FigID = FigNd.Attributes.ItemOf(FigureIdName).Value
                                Dim IDNum As String = Regex.Match(FigID, "\d+", RegexOptions.Singleline).Value
                                For Each strF As String In AllImages
                                    Dim fignum As String = Regex.Match(strF, "\d+", RegexOptions.RightToLeft).Value
                                    If fignum = IDNum Then
                                        Dim Finf As New System.IO.FileInfo(strF)
                                        FigureIdentiAttr.Value = BaseDirectory + sepra + FigName + "\" + Finf.Name
                                        Exit For
                                    End If
                                Next
                            Catch ex As Exception
                            End Try
                        End If
                    Else
                        FigureIdentiAttr.Value = "D:\FigNotFound.jpg"
                        Try
                            If (FigNd.Attributes.ItemOf("Float").Value.ToLower = "no") Then
                                FigureIdentiAttr.Value = "d:\FigNotFoundNew.jpg"
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                End If

                FigNd.Attributes.Append(FigureIdentiAttr)
                Try
                    FigNd.SelectSingleNode(".//ImageObject").Attributes.ItemOf("FileRef").Value = FigNd.Attributes.ItemOf("cs_FileRef").Value
                    FigNd.Attributes.Remove(FigNd.Attributes.ItemOf("cs_FileRef"))
                Catch ex As Exception

                End Try

                'Identifying Caption 
                Dim FigureCapIdentity As XmlAttribute = XDoc.CreateAttribute("cs_FigCaption")

                FigureCapIdentity.Value = FigurePathXPath
                Dim CaptionNodes As XmlNodeList = FigNd.SelectNodes(".//" + FigureCaption)
                Dim newCaptionWrapper As Xml.XmlElement = XDoc.CreateElement("cs_CaptionWrapper")

                For Each CapNd As XmlNode In CaptionNodes
                    CapNd.Attributes.Append(FigureCapIdentity)
                    newCaptionWrapper.InnerXml = CapNd.OuterXml
                Next
                Try
                    CaptionNodes(0).ParentNode.InsertBefore(newCaptionWrapper, CaptionNodes(0))
                Catch ex As Exception
                End Try
                For Each CapNd As XmlNode In CaptionNodes
                    CapNd.ParentNode.RemoveChild(CapNd)
                Next
            Catch ex As Exception
            End Try
        Next
        For ii As Integer = 0 To AllFigs.Count - 1
            oreq.Reposition(AllFigs(ii), XDoc.DocumentElement, RelationShip.LastChild, "figure")
        Next
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
    Public Function UpdateDataBaseString(ByVal DatabaseString As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:UpdateDataBaseString
        'PARAMETER    :DatabaseString
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        Dim ChapterDoi As String = XDoc.SelectSingleNode(".//Article/@ID").Value
        Dim kkChapterID = ""
        Dim IsbnOrBookid As String = ""
        Try
            Dim XmlName As String = System.IO.Path.GetFileName(FInputXml)
            XmlName = XmlName.Replace(".xml", "")
            XmlName = XmlName.Replace("_Chapter", "")
            IsbnOrBookid = Mid(XmlName, 1, XmlName.LastIndexOf("_"))
            kkChapterID = "_" + XmlName.Split("_")(XmlName.Split("_").Length - 1)
        Catch ex As Exception

        End Try
        Try
            Dim UpdateStr As Boolean = False
            Dim JobSheetID As String = ""
            Try
                Dim fsConn As String = ""
                fsConn = DatabaseString
                Dim mconn As New MySqlConnection(fsConn)
                Try
                    mconn.Open()
                    Dim str As String = ""
                    If (IsNothing(XDoc.SelectSingleNode(".//Article[@ID]")) = False) Then
                        str = XDoc.SelectSingleNode(".//Article/@ID").Value
                    End If
                    If (IsNothing(XDoc.SelectSingleNode(".//ChapterDOI")) = False) Then
                        'str = XDoc.SelectSingleNode(".//ChapterDOI").InnerText
                        'str = str.Replace("10.1007/", "")
                    ElseIf (True) Then
                        'str = XDoc.SelectSingleNode(".//ChapterDOI").InnerText
                        'str = str.Replace("10.1007/", "")
                        'ChapterDOI
                    End If


                    'Dim cdm As New MySqlCommand("select HardDrivePath from jobsheetinfo where DOI = '" + IsbnOrBookid + "' OR Concat(JobSheetID,'_','')='" + IsbnOrBookid + kkChapterID + "' Or (JobSheetID='" + IsbnOrBookid + "' AND ChapterID='" + kkChapterID.replace("_", "") + "')", mconn)
                    Dim cdm As New MySqlCommand("select HardDrivePath from jobsheetinfo where DOI = '" + ChapterDoi + "'", mconn)
                    Dim reader As MySqlDataReader
                    reader = cdm.ExecuteReader
                    If reader.HasRows = True Then
                        While reader.Read()
                            JobSheetID = reader("JobSheetid").ToString()
                        End While
                    End If
                    mconn.Close()
                    mconn.Dispose()
                Catch ex As Exception

                End Try
            Catch ex As Exception

            End Try
            Try
                If (JobSheetID = "") Then
                    UpdateStr = True
                End If
            Catch ex As Exception

            End Try
            Return UpdateStr
        Catch ex As Exception

        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Public Function GetNumberingStyle(ByVal FigureConversionFile As String, ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetNumberingStyle
        'PARAMETER    :FigureConversionFile,inputxml
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        Dim fsConn As String = ""
        Dim JobSheetID As String = ""
        Dim JournalName As String = "JournalName"
        Try
            Initialize(FigureConversionFile)
            fsConn = DatabaseString
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            Try
                mconn.Open()
                Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
                Dim cdm As New MySqlCommand("select JobSheetid from jobsheetinfo where DOI = '" + str + "'", mconn)
                Dim reader As MySqlDataReader
                reader = cdm.ExecuteReader
                If reader.HasRows = True Then
                    While reader.Read()
                        JobSheetID = reader("JobSheetid").ToString()
                    End While
                End If
                mconn.Close()
                mconn.Dispose()
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            mconn.Open()
            Dim str1 As String = System.IO.Path.GetFileName(inputxml).Split("_")(2)
            Dim cdm As New MySqlCommand("select NumberingStyle from tlbarticlechapterpartsubpart where parentid = '" + JobSheetID + "' and id='" + str1 + "'", mconn)
            Dim reader As MySqlDataReader
            reader = cdm.ExecuteReader
            If reader.HasRows = True Then
                While reader.Read()
                    JournalName = reader("NumberingStyle").ToString()
                End While
            End If
            mconn.Close()
            mconn.Dispose()
        Catch ex As Exception

        End Try
        Return JournalName
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function GetRunningHeadStyle(ByVal FigureConversionFile As String, ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetRunningHeadStyle
        'PARAMETER    :FigureConversionFile,inputxml
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        Dim fsConn As String = ""
        Dim JobSheetID As String = ""
        Dim RHStyle As String = ""
        Try
            Initialize(FigureConversionFile)
            fsConn = DatabaseString
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            mconn.Open()
            Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
            Dim cdm As New MySqlCommand("SELECT * FROM tlbtechnicalinfo as t1 INNER JOIN tlbproductionInfo as t2 INNER JOIN jobsheetinfo as t3 WHERE t1.productioninfoid=t2.ID AND t2.Jobsheetid=t3.Jobid AND t3.DOI='" + str + "'", mconn)
            Dim reader As MySqlDataReader
            reader = cdm.ExecuteReader
            If reader.HasRows = True Then
                While reader.Read()
                    RHStyle = reader("TypesettingRunningHead").ToString()
                End While
            End If
            mconn.Close()
            mconn.Dispose()
        Catch ex As Exception

        End Try
        Return RHStyle
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function GetJournalTitle(ByVal FigureConversionFile As String, ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetJournalTitle
        'PARAMETER    :FigureConversionFile,inputxml
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        Dim fsConn As String = ""
        Dim JobSheetID As String = ""
        Dim JournalTitle As String = "JournalName"
        Try
            Initialize(FigureConversionFile)
            fsConn = DatabaseString
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            Try
                mconn.Open()
                Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
                Dim cdm As New MySqlCommand("select JobSheetid from jobsheetinfo where DOI = '" + str + "'", mconn)
                Dim reader As MySqlDataReader
                reader = cdm.ExecuteReader
                If reader.HasRows = True Then
                    While reader.Read()
                        JobSheetID = reader("JobSheetid").ToString()
                    End While
                End If
                mconn.Close()
                mconn.Dispose()
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            mconn.Open()
            Dim cdm As New MySqlCommand("select JournalFullName from tlbjournalinfo where JournalID = '" + JobSheetID + "'", mconn)
            Dim reader As MySqlDataReader
            reader = cdm.ExecuteReader
            If reader.HasRows = True Then
                While reader.Read()
                    JournalTitle = reader("JournalFullName").ToString()
                End While
            End If
            mconn.Close()
            mconn.Dispose()
        Catch ex As Exception

        End Try
        Try
            If (ACDCLayout = True) Then
                JournalTitle = ""
                If (IsNothing(ACDCXdoc.SelectSingleNode(".//JournalTitle")) = False) Then
                    JournalTitle = ACDCXdoc.SelectSingleNode(".//JournalTitle").InnerXml
                End If
            End If
        Catch ex As Exception

        End Try
        Return JournalTitle
        '====================================================END======================================================
        '=============================================================================================================
    End Function
   
    'Public Function GetOpenChoice(ByVal FigureConversionFile As String, ByVal inputxml As String)
    '    '=============================================================================================================
    '    '=============================================================================================================
    '    'FUNCTION NAME:GetOpenChoice
    '    'PARAMETER    :FigureConversionFile,inputxml
    '    'AIM          :
    '    '=============================================================================================================
    '    '=============================================================================================================
    '    Dim fsConn As String = ""
    '    Dim JobSheetID As String = ""
    '    Dim BOpenChoice As String = ""
    '    Try
    '        Initialize(FigureConversionFile)
    '        fsConn = DatabaseString
    '    Catch ex As Exception

    '    End Try
    '    Try
    '        Dim mconn As New MySqlConnection(fsConn)
    '        Try
    '            mconn.Open()
    '            Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
    '            Dim cdm As New MySqlCommand("select CopyrightHoldername from tlbarticlechapterpartsubpart where DOI = '" + str + "'", mconn)
    '            Dim reader As MySqlDataReader
    '            reader = cdm.ExecuteReader
    '            If reader.HasRows = True Then
    '                While reader.Read()
    '                    BOpenChoice = reader("CopyrightHoldername").ToString()
    '                    If (BOpenChoice.ToLower = "the author(s)") Then
    '                        BOpenChoice = "openchoice"
    '                    End If
    '                End While
    '            End If
    '            mconn.Close()
    '            mconn.Dispose()
    '        Catch ex As Exception

    '        End Try
    '    Catch ex As Exception

    '    End Try
    '    Return BOpenChoice
    '    '====================================================END======================================================
    '    '=============================================================================================================
    'End Function
    Public Function GetQDrivexmlPath(ByVal FigureConversionFile As String, ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetQDrivexmlPath
        'PARAMETER    :FigureConversionFile,inputxml
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        Dim fsConn As String = ""
        Dim JobSheetID As String = ""
        Dim xmlPath As String = ""
        Try
            Initialize(FigureConversionFile)
            fsConn = DatabaseString
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            Try
                mconn.Open()
                Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
                Dim cdm As New MySqlCommand("select HardDrivePath from jobsheetinfo where DOI = '" + str + "'", mconn)
                Dim reader As MySqlDataReader
                reader = cdm.ExecuteReader
                If reader.HasRows = True Then
                    While reader.Read()
                        xmlPath = reader("HardDrivePath").ToString()
                        If (xmlPath <> "") Then
                            Return xmlPath
                        End If
                    End While
                End If
                mconn.Close()
                mconn.Dispose()
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
        Return xmlPath
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function getcolorinprintInfo(ByVal FigureConversionFile As String, ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:getcolorinprintInfo
        'PARAMETER    :FigureConversionFile,inputxml
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        Dim fsConn As String = ""
        Dim jobid As String = ""
        Dim ProdID As String = ""
        Dim colorinprint As Boolean = False
        Try
            Initialize(FigureConversionFile)
            fsConn = DatabaseString
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            Try
                mconn.Open()
                Dim str As String = XDoc.SelectSingleNode(".//Article[@ID]").Attributes.ItemOf("ID").Value
                Dim cdm As New MySqlCommand("select jobid from jobsheetinfo where DOI = '" + str + "'", mconn)
                Dim reader As MySqlDataReader
                reader = cdm.ExecuteReader
                If reader.HasRows = True Then
                    While reader.Read()
                        jobid = reader("jobid").ToString()
                    End While
                End If
                reader.Close()
                mconn.Close()
                mconn.Dispose()
            Catch ex As Exception
            Finally
                mconn.Close()
                mconn.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            Try
                mconn.Open()
                Dim cdm As New MySqlCommand("select id from tlbproductioninfo where jobsheetid = '" + jobid + "'", mconn)
                Dim reader As MySqlDataReader
                reader = cdm.ExecuteReader
                If reader.HasRows = True Then
                    While reader.Read()
                        ProdID = reader("id").ToString()
                    End While
                End If
                reader.Close()
                mconn.Close()
                mconn.Dispose()
            Catch ex As Exception
            Finally
                mconn.Close()
                mconn.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            Try
                mconn.Open()
                Dim cdm As New MySqlCommand("select colorinprint from tlbdiscreteobjecttechnicalinfo where prodinfoid = '" + ProdID + "'", mconn)
                Dim reader As MySqlDataReader
                reader = cdm.ExecuteReader
                If reader.HasRows = True Then
                    While reader.Read()
                        If (reader("colorinprint").ToString().ToLower = "yes") Then
                            colorinprint = True
                            Exit While
                        Else
                            colorinprint = False
                            Exit While
                        End If
                    End While
                End If
                reader.Close()
                mconn.Close()
                mconn.Dispose()
            Catch ex As Exception
            Finally
                mconn.Close()
                mconn.Dispose()
            End Try
        Catch ex As Exception

        End Try
        Return colorinprint
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function GetTechnicalPrefixInfo(ByVal FigureConversionFile As String, ByVal inputxml As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:GetTechnicalPrefixInfo
        'PARAMETER    :FigureConversionFile,inputxml
        'AIM          :
        '=============================================================================================================
        '=============================================================================================================
        Dim fsConn As String = ""
        Try
            Initialize(FigureConversionFile)
            fsConn = DatabaseString
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            mconn.Open()
            Dim cdm As New MySqlCommand("select Id from tlbproductioninfo where jobsheetid = '" + CrestJobsheetid + "'", mconn)
            Dim reader As MySqlDataReader
            reader = cdm.ExecuteReader
            If reader.HasRows = True Then
                While reader.Read()
                    ProductionInfoId = reader("Id").ToString
                End While
            End If
            mconn.Close()
            mconn.Dispose()
        Catch ex As Exception

        End Try
        Try
            Dim mconn As New MySqlConnection(fsConn)
            mconn.Open()
            Dim cdm As New MySqlCommand("select AuthorInfoCapturePrefix from tlbtechnicalinfo where ProductionInfoId = '" + ProductionInfoId + "'", mconn)
            Dim reader As MySqlDataReader
            reader = cdm.ExecuteReader
            If reader.HasRows = True Then
                While reader.Read()
                    AuthorInfoCapturePrefix = reader("AuthorInfoCapturePrefix").ToString
                End While
            End If
            mconn.Close()
            mconn.Dispose()
        Catch ex As Exception

        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    '====================================================END======================================================
    '=============================================================================================================
End Class

