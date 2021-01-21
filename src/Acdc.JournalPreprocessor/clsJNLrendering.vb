'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : SUVARNA RAUT
'CLASS NAME    : clsJNLrendering
'CREATED DATE  : 3RD JUNE 2013,
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
Public Class clsJNLrendering
    Dim Xdoc As New Xml.XmlDocument

    Public Sub New(ByVal inputxml As String, ByVal outputxml As String, ByVal acdc As String, ByVal jobsheetPath As String, ByVal share_p As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:New
        'PARAMETER    :inputxml,outputxml
        'AIM          :This is constructor.It construct the value of variable
        '=============================================================================================================
        '=============================================================================================================

        ACDCLayout = True
        INXMLName = inputxml
        OutPath = outputxml


        Try
            If (System.IO.Path.GetFileName(inputxml).Contains("-")) Then
                CLog.LogMessages("XML Filenaming convention is wrong. Use the following: JournalID_Year_ArticleID_Article.xml")
                ''End
            End If
        Catch ex As Exception

        End Try
        '=========================================================================================================
        xmlFileName = inputxml.Split("\")(inputxml.Split("\").Length - 1).Replace(".indd", ".xml")
        '=========================================================================================================
        Try

            configDoc.LoadXml(FileOperations.GetFile(ConfigFilePaths.conFigPath).ToString())
            Dim str As String = configDoc.OuterXml
            TableConversionFile = FileOperations.GetFile(ConfigFilePaths.tableSettingsPath).ToString() '' configDoc.SelectSingleNode(".//tablesettingxml").InnerText
            FigureConversionFile = FileOperations.GetFile(ConfigFilePaths.figureSettingsPath).ToString() ''configDoc.SelectSingleNode(".//figuresettingsxml").InnerText
            EquationPath = configDoc.SelectSingleNode(".//EquationPath").InnerText
            ProcessFootNoteAsEndnote = configDoc.SelectSingleNode(".//Endnote/@JournalID").Value
            layoutname = configDoc.SelectSingleNode(".//Layoutname").InnerText
            Infilemame = System.IO.Path.GetFileName(inputxml).Split("_")(0)
            Dim typea As String = configDoc.SelectSingleNode(".//JournalSubTypeA").InnerText ''System.Configura'rationSettings.AppSettings("JournalSubTypeA")
            Dim typeb As String = configDoc.SelectSingleNode(".//JournalSubTypeB").InnerText 'System.Configuration.ConfigurationSettings.AppSettings("JournalSubTypeB")

            ACDC_Jonsheetpath = share_p
            ACDC_Graphics = share_p

            EntityPathxml = FileOperations.GetFile(ConfigFilePaths.entitiesPath).ToString() '' configDoc.SelectSingleNode(".//Entitysettingsxml").InnerText





            Locationxmlpath = FileOperations.GetFile(ConfigFilePaths.jounalPathLocationPath).ToString() '' System.Configuration.ConfigurationSettings.AppSettings("JournalLocationdetail")

            Dim tempstr As String = "|" + Infilemame + "|"
            If (typea.Contains(tempstr) = True) Then
                JournalSubType = "A"
            End If
            If (typeb.Contains(tempstr) = True) Then
                JournalSubType = "B"
            Else
                If (JournalSubType = "") Then
                    JournalSubType = "B"
                End If
            End If
            ACDC_Jonsheetpath = jobsheetPath

            ACDC_Graphics = ACDC_Graphics + "\" + inputxml.Split("\")(inputxml.Split("\").Length - 1).Replace(".xml", "")
            CheckLayoutname_ACDC(inputxml)



            LayoutnameDB = LayoutnameDB.Trim
            If (layoutname.Trim.Contains(LayoutnameDB.Trim) = True And LayoutnameDB <> "") Then
            Else
                CLog.LogMessages("Layout information is wrong.")

            End If
            'Else
            '    CLog.LogMessages("NOT Exists(""config.xml"")")
            'End If

        Catch ex As Exception
            CLog.LogMessages("Error in clsJNLrendering()" + vbNewLine)
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
        End Try
        '=========================================================================================================
        Try
            Dim ArticleCategory As String = ""
            oReq = New clsPreprocMain(inputxml)
            Xdoc.PreserveWhitespace = True
            Xdoc = oReq.XDoc
            MDLFunction.Main(Xdoc)

            If (IsNothing(Xdoc.SelectSingleNode(".//ArticleCategory")) = False) Then
                ArticleCategory = Xdoc.SelectSingleNode(".//ArticleCategory").InnerText.ToLower
            End If
            AddInitialAttr()

            ''RemoveStyleAttr()

            InIt()

            AuthorQuery(Xdoc)

            Hyperlink()
            HideElement()
            If (ArticleCategory.ToLower = "abstracts") Then
                PlaceEnterAbstract()
                ApplyParagraphStyleAbstract()
                ApplyCharacterStyleAbstract()
            Else
                PlaceEnter()
                ApplyParagraphStyle()
                ApplyCharacterStyle()
            End If
            ApplyFrame()
            ApplyFootnote(inputxml)
            RemoveAttr()
            SideBarConversion()
            oReq.oTableObj.TableConversion(TableConversionFile)
            oReq.oFigObj.FigureConversion(FigureConversionFile, inputxml)
            AddAdditionalInformation(inputxml)
            TableStyles()
            AddTableAttr()
            ApplyTableCellStyle()
            SpecialRoutine()
            HideAQuery()
            RemoveNodes()
            ConvertSeperateChapterAQNode(Xdoc)
            Try
                Dim XmlStr As String
                XmlStr = Xdoc.OuterXml
                Dim Xdoc1 As New Xml.XmlDocument
                Xdoc1.PreserveWhitespace = True
                Xdoc1.LoadXml(XmlStr)
            Catch ex As Exception
                CLog.LogMessages(ex.Message.ToString + vbNewLine)
                CLog.LogMessages("====================================================================================================================================", True)
                CLog.LogMessages("====================================================================================================================================", True)
                CLog.LogMessages("====================================================================================================================================", True)
                CLog.LogMessages("====================================================================================================================================", True)
                CLog.LogMessages(Xdoc.OuterXml)
                CLog.LogMessages("====================================================================================================================================", True)
                CLog.LogMessages("====================================================================================================================================", True)
                CLog.LogMessages("====================================================================================================================================", True)
                CLog.LogMessages("====================================================================================================================================", True)
                GoTo p
            End Try
            oReq.XDoc.Save(outputxml)
            System.Threading.Thread.Sleep(3000)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("Successfully created outputxml")
            CLog.LogMessages("====================================================================================================================================", True)
        Catch ex As Exception
            CLog.LogMessages(ex.Message.ToString + vbNewLine)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages(Xdoc.OuterXml)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            CLog.LogMessages("====================================================================================================================================", True)
            GoTo p
        End Try
p:
        Try
            Dim ersw As New System.IO.StreamWriter("d:\JNLPGUtility.txt")
            ersw.Write("-----STM Journal Preprocessor ------ : " + Now.ToString + vbNewLine + ErrorMessages + vbNewLine)
            ersw.Close()
        Catch ex As Exception
        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Sub

    Private Sub RemoveStyleAttr()
        Try

            Dim ndLst As Xml.XmlNodeList = Xdoc.SelectNodes(".//Table")

            If Not ndLst Is Nothing Then
                For Each ndTlb As Xml.XmlNode In ndLst
                    If Not ndTlb.Attributes("Style") Is Nothing Then
                        ndTlb.Attributes.Remove(ndTlb.Attributes("Style"))
                    End If
                Next
            End If

        Catch ex As Exception

        End Try
    End Sub

    '====================================================END======================================================
    '=============================================================================================================
End Class
