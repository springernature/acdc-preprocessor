'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : 
'MODULE NAME   : PDFINFO
'CREATED DATE  : 3RD JUNE 2013
'LAST MODIFIED : 14TH JUNE 2013
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
Imports System.Text.RegularExpressions
Public Class PDFINFO
    Dim Xdoc As New Xml.XmlDocument
    Public Sub New(ByVal Xxdoc As Xml.XmlDocument)
        Xdoc = Xxdoc
    End Sub
    Public Function OffPrintNdCreation(ByVal JournalName As String)
        'OffPrint_Data
        Try
            Dim OffPrint_Data As String = ""
            ' Dim JournalName As String = ""
            Dim DOI As String = ""
            Try
                ' JournalName = GetJournalName()
                DOI = Xdoc.SelectSingleNode(".//ArticleDOI").InnerText
            Catch ex As Exception

            End Try
            Try
                JournalName = "<JournalName>Journal: " + JournalName + "</JournalName>" + vbNewLine
            Catch ex As Exception

            End Try
            Dim corr_Auth As String = ""
            Try

                Dim Corr_Info As String = ""
                Dim Corr_Node As Xml.XmlNodeList = Xdoc.SelectNodes(".//Author[@CorrespondingAffiliationID and (not(.=''))]")
                If (IsNothing(Corr_Node) = False) Then
                    For k As Integer = 0 To Corr_Node.Count - 1
                        Try
                            If (IsNothing(Corr_Node(k).SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
                                corr_Auth += Corr_Node(k).SelectSingleNode(".//GivenName[not(.='')]").InnerText + " "
                            End If
                            If (IsNothing(Corr_Node(k).SelectSingleNode(".//Particle[not(.='')]")) = False) Then
                                corr_Auth += Corr_Node(k).SelectSingleNode(".//Particle[not(.='')]").InnerText + " "
                            End If
                            If (IsNothing(Corr_Node(k).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                                corr_Auth += Corr_Node(k).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
                            End If
                        Catch ex As Exception

                        End Try
                        If (corr_Auth <> "") Then
                            corr_Auth += Chr(13)
                        End If
                        Try
                            'aff
                            Dim Aff_ID As String = Corr_Node(k).Attributes.ItemOf("CorrespondingAffiliationID").Value
                            Dim AffilationNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Affiliation[@ID='" + Aff_ID + "']")
                            corr_Auth += AffilationNds(0).InnerText
                        Catch ex As Exception

                        End Try

                    Next
                    Try
                        corr_Auth = "<corr_Auth>" + corr_Auth + "</corr_Auth>"
                    Catch ex As Exception

                    End Try
                End If
            Catch ex As Exception

            End Try
            Try
                OffPrint_Data = "<OffPrintInfo>" + JournalName + "<DOI>" + DOI + "</DOI>" + corr_Auth + "</OffPrintInfo>"
                Return OffPrint_Data
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
    End Function
    Public Function ProofNdCreation(ByVal JournalName As String, ByVal Article_Lg As String)
        'create Proof node
        Dim PROOFINFO As String = ""
        Try
            Dim ProofNode As Xml.XmlNode = Xdoc.CreateElement("Proof_Tag")
            Dim ArticleTitle As String = ""
            Dim Authors As String = ""
            Dim DOI As String = ""
            Try
                DOI = Xdoc.SelectSingleNode(".//ArticleDOI").InnerText
            Catch ex As Exception

            End Try
            Try
                JournalName = "<JournalName>" + JournalName + " (" + DOI + ")" + "</JournalName>"
            Catch ex As Exception

            End Try
            Try
                ArticleTitle = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]").InnerText
                ' ArticleTitle = "ArticleTitle: " + ArticleTitle
                ArticleTitle = "<ArticleTitle>" + ArticleTitle + "</ArticleTitle>"
            Catch ex As Exception

            End Try
            Try
                Dim Authnodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorName[not(.='')]")
                If (Authnodes.Count > 0) Then
                    Try
                        For i As Integer = 0 To Authnodes.Count - 1
                            Try
                                Try
                                    If (IsNothing(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
                                        Authors += Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]").InnerText + " "
                                    End If
                                    If (IsNothing(Authnodes(i).SelectSingleNode(".//Particle[not(.='')]")) = False) Then
                                        Authors += Authnodes(i).SelectSingleNode(".//Particle[not(.='')]").InnerText + " "
                                    End If
                                    If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                                        Authors += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
                                    End If
                                Catch ex As Exception

                                End Try
                                'Authors += Authnodes(i).InnerText
                                If (i <> Authnodes.Count - 1) Then
                                    Authors += ChrW(10)
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
                ' Authors = "Author(s): " + Authors
                Authors = "<Authors>" + Authors + "</Authors>"
            Catch ex As Exception

            End Try
            Try
                'CSTNode.InnerXml = JournalName + ArticleTitle + Authors + "<AuthorsSignature>Author's signature: </AuthorsSignature><Date>Date: </Date>"
                ProofNode.InnerXml = JournalName + ArticleTitle + Authors
            Catch ex As Exception

            End Try
            Try
                PROOFINFO = ProofNode.OuterXml
                Return PROOFINFO
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
    End Function
    Public Function CTSNdCreation(ByVal JournalName As String, ByVal Article_Lg As String)
        'create CTS node
        Dim CTSINFO As String = ""
        Try
            Dim CSTNode As Xml.XmlNode = Xdoc.CreateElement("CTS_Tag")
            ' Dim JournalName As String = ""
            Dim ArticleTitle As String = ""
            Dim Authors As String = ""
            Try
                ' JournalName = GetJournalName()
                'JournalName = "JournalName: " + JournalName
                JournalName = "<JournalName>" + JournalName + "</JournalName>"
            Catch ex As Exception

            End Try
            Try
                ArticleTitle = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]").InnerText
                ' ArticleTitle = "ArticleTitle: " + ArticleTitle
                ArticleTitle = "<ArticleTitle>" + ArticleTitle + "</ArticleTitle>"
            Catch ex As Exception

            End Try
            Try
                Dim Authnodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorName[not(.='')]")
                If (Authnodes.Count > 0) Then
                    Try
                        For i As Integer = 0 To Authnodes.Count - 1
                            Try
                                Try
                                    If (IsNothing(Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
                                        Authors += Authnodes(i).SelectSingleNode(".//GivenName[not(.='')]").InnerText + " "
                                    End If
                                    If (IsNothing(Authnodes(i).SelectSingleNode(".//Particle[not(.='')]")) = False) Then
                                        Authors += Authnodes(i).SelectSingleNode(".//Particle[not(.='')]").InnerText + " "
                                    End If
                                    If (IsNothing(Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                                        Authors += Authnodes(i).SelectSingleNode(".//FamilyName[not(.='')]").InnerText
                                    End If
                                Catch ex As Exception

                                End Try
                                'Authors += Authnodes(i).InnerText
                                If (i <> Authnodes.Count - 1) Then
                                    Authors += ", "
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
                ' Authors = "Author(s): " + Authors
                Authors = "<Authors>" + Authors + "</Authors>"
            Catch ex As Exception

            End Try
            Try
                'CSTNode.InnerXml = JournalName + ArticleTitle + Authors + "<AuthorsSignature>Author's signature: </AuthorsSignature><Date>Date: </Date>"
                CSTNode.InnerXml = JournalName + ArticleTitle + Authors
            Catch ex As Exception

            End Try
            Try
                CTSINFO = CSTNode.OuterXml
                Return CTSINFO
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
    End Function
    Public Function MetaDataNdCreation(ByVal JournalName As String, ByVal Article_Lg As String, ByVal colorinprint As String)
        'create MetaDataNode
        Try
            Dim XMLM_Data As String = ""
            Dim ArticleTitle As String = ""
            Dim ArticleSubTitle As String = ""
            Dim ArticleCopyRightYear As String = ""
            'Dim JournalName As String = ""
            Dim AuthData As String = ""
            Dim Abstract As String = ""
            Dim Schedule As String = ""
            Dim Keywords As String = ""
            Dim FootNoteInfo As String = ""

            Try
                ArticleTitle = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]").InnerXml
                ArticleTitle = "<M_ArticleTitle type='ArticleTitle'>" + ArticleTitle + "</M_ArticleTitle>"
            Catch ex As Exception

            End Try

            Try
                Dim ArticleSubTitleNDs As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleInfo/ArticleSubTitle[@Language='" + Article_Lg + "' and not(.='') and position()=1]")
                If (IsNothing(ArticleSubTitleNDs) = False) Then
                    ArticleSubTitle = "<M_ArticleSubTitle type='Article Sub-Title'>" + ArticleSubTitleNDs.InnerXml + "</M_ArticleSubTitle>"
                End If
            Catch ex As Exception

            End Try

            Try
                If (IsNothing(Xdoc.SelectSingleNode(".//ArticleCopyright/CopyrightHolderName[not(.='')]")) = False) Then
                    ArticleCopyRightYear += Xdoc.SelectSingleNode(".//ArticleCopyright/CopyrightHolderName[not(.='')]").InnerXml
                End If

                If (IsNothing(Xdoc.SelectSingleNode(".//ArticleCopyright/CopyrightYear[not(.='')]")) = False) Then
                    ArticleCopyRightYear += " " + Xdoc.SelectSingleNode(".//ArticleCopyright/CopyrightYear[not(.='')]").InnerXml
                End If
                If (ArticleCopyRightYear <> "") Then
                    ArticleCopyRightYear = "<M_ArticleCopyRightYear type='Article CopyRight - Year'>" + ArticleCopyRightYear + "</M_ArticleCopyRightYear>"
                End If

            Catch ex As Exception

            End Try
            Try
                'JournalName = GetJournalName()
                JournalName = "<M_JournalName type='Journal Name'>" + JournalName + "</M_JournalName>"
            Catch ex As Exception

            End Try
            Try
                AuthData = CreateMetaDataNode()
            Catch ex As Exception

            End Try

            Try
                'schedule
                'received

                If (IsNothing(Xdoc.SelectSingleNode(".//Received[not(.='')]")) = False) Then
                    Dim Received_Dt As String = Format_Date(Xdoc.SelectSingleNode(".//Received[not(.='')]"), Article_Lg)
                    Schedule += "<M_Schedule type='Schedule'>" + "<M_Received type='Received'>" + Received_Dt + "</M_Received></M_Schedule>"
                Else
                    Schedule += "<M_Schedule type='Schedule'><M_Received type='Received'></M_Received></M_Schedule>"
                End If
                'revised
                If (IsNothing(Xdoc.SelectSingleNode(".//Revised[not(.='')]")) = False) Then
                    Dim Revised_Dt As String = Format_Date(Xdoc.SelectSingleNode(".//Revised[not(.='')]"), Article_Lg)
                    Schedule += "<M_Schedule type='Schedule'><M_Revised type='Revised'>" + Revised_Dt + "</M_Revised></M_Schedule>"
                Else
                    Schedule += "<M_Schedule type='Schedule'><M_Revised type='Revised'></M_Revised></M_Schedule>"
                End If
                'accepted
                If (IsNothing(Xdoc.SelectSingleNode(".//Accepted[not(.='')]")) = False) Then
                    Dim Accepted_Dt As String = Format_Date(Xdoc.SelectSingleNode(".//Accepted[not(.='')]"), Article_Lg)
                    Schedule += "<M_Schedule type='Schedule'><M_Accepted type='Accepted'>" + Accepted_Dt + "</M_Accepted></M_Schedule>"
                Else
                    Schedule += "<M_Schedule type='Schedule'><M_Accepted type='Accepted'></M_Accepted></M_Schedule>"
                End If
                ''   Schedule = "<M_Schedule type='Schedule'>" + Schedule + "</M_Schedule>"
            Catch ex As Exception

            End Try
   

            '===================================================================================================================
            '===================================================================================================================
            Dim Sec_Lang As String = ""
            Dim Sec_Abs As String = ""
            Dim Temp_AbstNodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//Abstract")
            If (IsNothing(Temp_AbstNodes) = False) Then
                Try
                    For Each abnode As Xml.XmlNode In Temp_AbstNodes
                        Sec_Lang = abnode.Attributes.ItemOf("Language").Value
                        Try
                            'abstract
                            If (IsNothing(Xdoc.SelectSingleNode(".//Abstract[@Language='" + Sec_Lang + "'  and not(.='')]")) = False) Then
                                Try
                                    Dim MainAbstract As Xml.XmlNode = Xdoc.SelectSingleNode(".//Abstract[@Language='" + Sec_Lang + "'  and not(.='')]")
                                    Dim DummyAbstract As Xml.XmlNode = Xdoc.CreateElement("dummyabstract")
                                    DummyAbstract.InnerXml = MainAbstract.InnerXml
                                    Try
s:                                      Dim cs_qNode As Xml.XmlNodeList = DummyAbstract.SelectNodes(".//cs_query")
                                        If (IsNothing(cs_qNode) = False And cs_qNode.Count > 0) Then
                                            For h As Integer = 0 To cs_qNode.Count - 1
                                                cs_qNode(h).ParentNode.RemoveChild(cs_qNode(h))
                                                GoTo s
                                            Next
                                        End If

                                    Catch ex As Exception

                                    End Try

                                    If (IsNothing(DummyAbstract) = False) Then
                                        For k As Integer = 0 To DummyAbstract.ChildNodes.Count - 2
                                            Dim node As Xml.XmlNode = DummyAbstract.ChildNodes(k)
                                            If (node.InnerText <> "") Then
                                                If (node.Name.ToLower <> "heading") Then
                                                    node.InnerXml = node.InnerXml + "<cs_text type='newline'>" + Chr(13) + "</cs_text>"
                                                End If
                                            End If
                                        Next
                                    End If
                                    ' DummyAbstract.InnerText = DummyAbstract.InnerText.Replace("<", "&#x3C;").Replace(">", "&#x3E;")
                                    ' DummyAbstract.InnerXml = DummyAbstract.InnerXml.Replace("<", "&lt;").Replace(">", "&gt;")
                                    Abstract = DummyAbstract.InnerXml
                                Catch ex As Exception
                                End Try
                                Try
                                    If (IsNothing(Xdoc.SelectSingleNode(".//Abstract[@Language='" + Sec_Lang + "'  and not(.='')]/Heading[not(.='')]")) = False) Then
                                        Try
                                            If (Abstract.StartsWith(Xdoc.SelectSingleNode(".//Abstract[@Language='" + Sec_Lang + "'  and not(.='')]/Heading[not(.='')]").InnerText)) Then
                                                Dim s As Regex
                                                s = New Regex(Xdoc.SelectSingleNode(".//Abstract[@Language='" + Sec_Lang + "'  and not(.='')]/Heading[not(.='')]").InnerText, RegexOptions.Singleline)
                                                Abstract = s.Replace(Abstract, "", 1)
                                            End If
                                        Catch ex As Exception

                                        End Try
                                    End If
                                Catch ex As Exception

                                End Try
                            End If
                            If (Sec_Lang.ToLower = "en") Then
                                Abstract = "<M_Abstract type='Abstract'>" + Abstract + "</M_Abstract>"
                            Else
                                If (Sec_Lang.ToLower = "de") Then
                                    Abstract = "<M_Abstract type='Zusammenfassung'>" + Abstract + "</M_Abstract>"
                                Else
                                    If (Sec_Lang.ToLower = "fr") Then
                                        Abstract = "<M_Abstract type='Abstract'>" + Abstract + "</M_Abstract>"
                                    End If
                                End If
                            End If

                            Sec_Abs += Abstract
                        Catch ex As Exception

                        End Try
                    Next
                Catch ex As Exception

                End Try
                Abstract = Sec_Abs
            End If
            '===================================================================================================================
            '===================================================================================================================
            Dim Sec_Language As String = ""
            Dim Sec_keywrds As String = ""
            Dim Temp_kwywrdsNodes As Xml.XmlNodeList = Xdoc.SelectNodes(".//KeywordGroup")
            If (IsNothing(Temp_kwywrdsNodes) = False) Then
                Try
                    Try
                        For Each abnode As Xml.XmlNode In Temp_kwywrdsNodes
                            Try
                                Sec_Language = abnode.Attributes.ItemOf("Language").Value
                                Try
                                    'keywords
                                    If (IsNothing(Xdoc.SelectSingleNode(".//KeywordGroup[@Language='" + Sec_Language + "'and not(.='')]")) = False) Then
                                        Try
                                            Dim MainKeywords As Xml.XmlNode = Xdoc.SelectSingleNode(".//KeywordGroup[@Language='" + Sec_Language + "'and not(.='')]")
                                            Dim DummyKeywords As Xml.XmlNode = Xdoc.CreateElement("dummykeywords")
                                            DummyKeywords.InnerXml = MainKeywords.InnerXml
                                            If (IsNothing(DummyKeywords) = False) Then
                                                Try
                                                    For k As Integer = 0 To DummyKeywords.ChildNodes.Count - 2
                                                        Dim node As Xml.XmlNode = DummyKeywords.ChildNodes(k)
                                                        Try
                                                            If (node.InnerText <> "") Then
                                                                If (node.Name.ToLower <> "heading") Then
                                                                    node.InnerXml = node.InnerXml + " &#8211; " '+ " - "
                                                                End If
                                                            End If
                                                        Catch ex As Exception

                                                        End Try
                                                    Next
                                                Catch ex As Exception

                                                End Try
                                            End If
                                            Keywords = DummyKeywords.InnerXml
                                        Catch ex As Exception

                                        End Try
                                        Try
                                            If (IsNothing(Xdoc.SelectSingleNode(".//KeywordGroup[@Language='" + Sec_Language + "'and not(.='')]/Heading[not(.='')]")) = False) Then
                                                Try
                                                    If (Keywords.StartsWith("<Heading>" + Xdoc.SelectSingleNode(".//KeywordGroup[@Language='" + Sec_Language + "'and not(.='')]/Heading[not(.='')]").InnerText + "</Heading>") = True) Then
                                                        Dim s As Regex
                                                        s = New Regex(Xdoc.SelectSingleNode(".//KeywordGroup[@Language='" + Sec_Language + "'and not(.='')]/Heading[not(.='')]").InnerText, RegexOptions.Singleline)
                                                        Keywords = s.Replace(Keywords, "", 1)
                                                        Keywords = Keywords.Replace("<Heading>", "").Replace("</Heading>", "")
                                                    End If
                                                Catch ex As Exception

                                                End Try

                                            End If
                                        Catch ex As Exception

                                        End Try
                                    End If
                                    'Keywords = "<M_Keywords type='Keywords(seperated by –)'>" + Keywords + "</M_Keywords>"
                                    If (Sec_Language.ToLower = "en") Then
                                        Keywords = "<M_Keywords type='Keywords(seperated by –)'>" + Keywords + "</M_Keywords>"
                                    Else
                                        If (Sec_Language.ToLower = "de") Then
                                            Keywords = "<M_Keywords type='Schlüsselwörter'>" + Keywords + "</M_Keywords>"
                                        Else
                                            If (Sec_Language.ToLower = "fr") Then
                                                Keywords = "<M_Keywords type='Keywords(seperated by –)'>" + Keywords + "</M_Keywords>"
                                            End If
                                        End If
                                    End If
                                    Sec_keywrds += Keywords

                                Catch ex As Exception

                                End Try
                            Catch ex As Exception

                            End Try
                        Next
                    Catch ex As Exception

                    End Try
                    Keywords = Sec_keywrds
                Catch ex As Exception

                End Try

            End If
            '===================================================================================================================
            '===================================================================================================================



          

            Try
                Try
                    ' Dim FootNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote[@Type='PresentedAt']|.//ArticleNote[@Type='PresentedBy']|.//ArticleNote[@Type='Dedication']|.//ArticleNote[@Type='CommunicatedBy']|.//ArticleNote[@Type='Misc']|.//ArticleNote[@Type='ESMHint']|.//ArticleNote[@Type='Motto']")
                    Dim FootNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote")
                    If (IsNothing(FootNd) = False) Then
                        Try
                            For i As Integer = 0 To FootNd.Count - 1
                                Try
                                  
                                    Try
                                        Dim Footnote As String = ""
                                        Dim MainArticleNote As Xml.XmlNode = FootNd(i)
                                        Dim DummyArticlenote As Xml.XmlNode = Xdoc.CreateElement("dummyarticlenote")
                                        DummyArticlenote.InnerXml = MainArticleNote.InnerXml
                                        If (IsNothing(DummyArticlenote) = False) Then
                                            For k As Integer = 0 To DummyArticlenote.ChildNodes.Count - 2
                                                Dim node As Xml.XmlNode = DummyArticlenote.ChildNodes(k)
                                                If (node.InnerText <> "") Then
                                                    node.InnerXml = node.InnerXml + "<cs_text type='newline'>" + Chr(13) + "</cs_text>"
                                                End If
                                            Next
                                        End If
                                        If (i = 0) Then
                                            Footnote += "<M_Footnote type='Footnote Information'>" + DummyArticlenote.InnerXml + "</M_Footnote>"
                                        Else
                                            Footnote += "<M_Footnote type=''>" + DummyArticlenote.InnerXml + "</M_Footnote>"
                                        End If
                                        FootNoteInfo += Footnote
                                    Catch ex As Exception

                                    End Try
                                Catch ex As Exception

                                End Try
                            Next
                        Catch ex As Exception

                        End Try
                    End If

                Catch ex As Exception

                End Try
            Catch ex As Exception

            End Try
            Try
                'combined all the node
                XMLM_Data = ArticleTitle + ArticleSubTitle + ArticleCopyRightYear + JournalName + AuthData + Schedule + Abstract + Keywords + FootNoteInfo
                Try
                    'create first and second mdata node
                    If (colorinprint = False) Then
                        If (Article_Lg.ToLower = "de") Then
                            XMLM_Data = "<M_PrintIncolor>Achtung: Farbabbildungen erscheinen in der Online-Version  in Farbe, werden jedoch schwarzweiß gedruckt.</M_PrintIncolor>" + XMLM_Data
                        Else
                            If (Article_Lg.ToLower = "en") Then
                                XMLM_Data = "<M_PrintIncolor>Please note: Images will appear in color online but will be printed in black and white.</M_PrintIncolor>" + XMLM_Data
                            End If
                        End If
                    End If
                    If (Article_Lg.ToLower = "de") Then
                        XMLM_Data = "<M_MDataTitle>Metadaten des Artikels, die online angezeigt werden</M_MDataTitle>" + XMLM_Data
                    Else
                        If (Article_Lg.ToLower = "en") Then
                            XMLM_Data = "<M_MDataTitle>Metadata of the article that will be visualized online</M_MDataTitle>" + XMLM_Data
                        End If
                    End If
                Catch ex As Exception

                End Try
                Return XMLM_Data
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try
    End Function
    Private Function Format_Date(ByVal nd As Xml.XmlNode, ByVal Article_Lg As String)
        Try
            Dim Mm As Integer = 0
            Dim Month As String = ""
            Dim Dd As Integer = 0
            Dim Yy As Integer = 0
            Try
                Mm = nd.SelectSingleNode(".//Month").InnerText
                Dd = nd.SelectSingleNode(".//Day").InnerText
                Yy = nd.SelectSingleNode(".//Year").InnerText
            Catch ex As Exception

            End Try
            If (Mm = "01" Or Mm = "1") Then
                If (Article_Lg = "De") Then
                    Month = "Januar"
                Else
                    If (Article_Lg = "En") Then
                        Month = "January"
                    End If
                End If
            End If
            If (Mm = "02" Or Mm = "2") Then
                If (Article_Lg = "De") Then
                    Month = "Februar"
                Else
                    If (Article_Lg = "En") Then
                        Month = "February"
                    End If
                End If
            End If
            If (Mm = "03" Or Mm = "3") Then
                If (Article_Lg = "De") Then
                    Month = "März"
                Else
                    If (Article_Lg = "En") Then
                        Month = "March"
                    End If
                End If
            End If
            If (Mm = "04" Or Mm = "4") Then
                If (Article_Lg = "De") Then
                    Month = "April"
                Else
                    If (Article_Lg = "En") Then
                        Month = "April"
                    End If
                End If
            End If
            If (Mm = "05" Or Mm = "5") Then
                If (Article_Lg = "De") Then
                    Month = "Mai"
                Else
                    If (Article_Lg = "En") Then
                        Month = "May"
                    End If
                End If
            End If
            If (Mm = "06" Or Mm = "6") Then
                If (Article_Lg = "De") Then
                    Month = "Juni"
                Else
                    If (Article_Lg = "En") Then
                        Month = "June"
                    End If
                End If
            End If
            If (Mm = "07" Or Mm = "7") Then
                If (Article_Lg = "De") Then
                    Month = "Juli"
                Else
                    If (Article_Lg = "En") Then
                        Month = "July"
                    End If
                End If
            End If
            If (Mm = "08" Or Mm = "8") Then
                If (Article_Lg = "De") Then
                    Month = "August"
                Else
                    If (Article_Lg = "En") Then
                        Month = "August"
                    End If
                End If
            End If
            If (Mm = "09" Or Mm = "9") Then
                If (Article_Lg = "De") Then
                    Month = "September"
                Else
                    If (Article_Lg = "En") Then
                        Month = "September"
                    End If
                End If
            End If
            If (Mm = "10" Or Mm = "10") Then
                If (Article_Lg = "De") Then
                    Month = "Oktober"
                Else
                    If (Article_Lg = "En") Then
                        Month = "October"
                    End If
                End If
            End If
            If (Mm = "11" Or Mm = "11") Then
                If (Article_Lg = "De") Then
                    Month = "November"
                Else
                    If (Article_Lg = "En") Then
                        Month = "November"
                    End If
                End If
            End If
            If (Mm = "12" Or Mm = "12") Then
                If (Article_Lg = "De") Then
                    Month = "Dezember"
                Else
                    If (Article_Lg = "En") Then
                        Month = "December"
                    End If
                End If
            End If

            Try
                Dim Dt_Date As String = Dd.ToString + " " + Month.ToString + " " + Yy.ToString
                Return Dt_Date
            Catch ex As Exception

            End Try
        Catch ex As Exception

        End Try

    End Function
    Public Function CreateMetaDataNode()
        Dim MainNode As String = "" '"<METADATA_Info>"
        Try
            Dim AuthorGroupNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//AuthorGroup[not(.='')]")
            If (IsNothing(AuthorGroupNd) = False) Then
                Try
                    'First find CorrespondingAuthor(1) its affilations(*) and pass this info for node creation
                    Try
                        Dim valid As String = ""
                        Dim Corr_Node As Xml.XmlNodeList = Xdoc.SelectNodes(".//Author[@CorrespondingAffiliationID and (not(.=''))]|.//InstitutionalAuthor[@CorrespondingAffiliationID and (not(.=''))]")  ''InstitutionalAuthor
                        If (IsNothing(Corr_Node) = False) Then
                            For i As Integer = 0 To Corr_Node.Count - 1
                                Try
                                    valid = Corr_Node(i).Attributes.ItemOf("CorrespondingAffiliationID").Value
                                    'Dim Aff_ID As String = Corr_Node(i).Attributes.ItemOf("CorrespondingAffiliationID").Value 'Orignal
                                    Dim Aff_ID As String = Corr_Node(i).Attributes.ItemOf("AffiliationIDS").Value
                                    If (Aff_ID.Split(" ").Length > 1) Then
                                        For j As Integer = 0 To Aff_ID.Split(" ").Length - 1
                                            ' If (valid = Aff_ID.Split(" ")(j)) Then
                                            Dim AffilationNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Affiliation[@ID='" + Aff_ID.Split(" ")(j) + "']")
                                            Dim MDATA As String = CreateMetaDataNd(Corr_Node(i), AffilationNds)
                                            MainNode += "<M_Author type='Corresponding Author'>" + MDATA + "</M_Author>"

                                            '  End If

                                        Next
                                    Else
                                        Dim AffilationNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Affiliation[@ID='" + Aff_ID + "']")
                                        Dim MDATA As String = CreateMetaDataNd(Corr_Node(i), AffilationNds)
                                        MainNode += "<M_Author type='Corresponding Author'>" + MDATA + "</M_Author>"
                                    End If

                                Catch ex As Exception

                                End Try
                            Next
                        End If
                    Catch ex As Exception

                    End Try
                    'create node for remaining author
                    Try
                        Dim WithoutCorr_Node As Xml.XmlNodeList = Xdoc.SelectNodes(".//Author[not(@CorrespondingAffiliationID) and (not(.=''))]")
                        If (IsNothing(WithoutCorr_Node) = False) Then
                            For i As Integer = 0 To WithoutCorr_Node.Count - 1
                                Try
                                    Dim Aff_ID As String = WithoutCorr_Node(i).Attributes.ItemOf("AffiliationIDS").Value
                                    If (Aff_ID.Split(" ").Length > 1) Then
                                        For j As Integer = 0 To Aff_ID.Split(" ").Length - 1
                                            Dim AffilationNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Affiliation[@ID='" + Aff_ID.Split(" ")(j) + "']")
                                            Dim MDATA As String = CreateMetaDataNd(WithoutCorr_Node(i), AffilationNds)
                                            MainNode += "<M_Author type='Author'>" + MDATA + "</M_Author>"
                                        Next
                                    Else
                                        Dim AffilationNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Affiliation[@ID='" + Aff_ID + "']")
                                        Dim MDATA As String = CreateMetaDataNd(WithoutCorr_Node(i), AffilationNds)
                                        MainNode += "<M_Author type='Author'>" + MDATA + "</M_Author>"
                                    End If
                                Catch ex As Exception

                                End Try
                            Next
                        End If
                    Catch ex As Exception

                    End Try

                Catch ex As Exception

                End Try
            End If
        Catch ex As Exception

        End Try
        ' MainNode += "</METADATA_Info>"
        Return MainNode
    End Function
    Public Function CreateMetaDataNd(ByVal author As Xml.XmlNode, ByVal Affiliation As Xml.XmlNodeList)
        Try
            Dim M_Author As String = ""
            Try
                'auth info
                If (IsNothing(author.SelectSingleNode(".//FamilyName[not(.='')]")) = False) Then
                    M_Author += "<M_FamilyName type='Family Name'>" + author.SelectSingleNode(".//FamilyName[not(.='')]").InnerXml + "</M_FamilyName>"
                Else
                    M_Author += "<M_FamilyName type='Family Name'></M_FamilyName>"
                End If
                If (IsNothing(author.SelectSingleNode(".//Particle[not(.='')]")) = False) Then
                    M_Author += "<M_Particle type='Particle'>" + author.SelectSingleNode(".//Particle[not(.='')]").InnerXml + "</M_Particle>"
                Else
                    M_Author += "<M_Particle type='Particle'></M_Particle>"
                End If

                '' suru added for ''InstitutionalAuthor
                If (IsNothing(author.SelectSingleNode(".//InstitutionalAuthorName[not(.='')]")) = False) Then


                    If (IsNothing(author.SelectSingleNode(".//InstitutionalAuthorName[not(.='')]")) = False) Then
                        M_Author += "<M_GivenName type='Given Name'>" + author.SelectSingleNode(".//InstitutionalAuthorName[not(.='')]").InnerXml + "</M_GivenName>"
                    Else
                        M_Author += "<M_GivenName type='Given Name'></M_GivenName>"
                    End If
                End If
                Try
                    If (author.SelectNodes(".//GivenName[not(.='')]").Count = 1) Then
                        If (IsNothing(author.SelectSingleNode(".//GivenName[not(.='')]")) = False) Then
                            M_Author += "<M_GivenName type='Given Name'>" + author.SelectSingleNode(".//GivenName[not(.='')]").InnerXml + "</M_GivenName>"
                        Else
                            M_Author += "<M_GivenName type='Given Name'></M_GivenName>"
                        End If
                    Else
                        M_Author += "<M_GivenName type='Given Name'>"
                        For j As Integer = 0 To author.SelectNodes(".//GivenName[not(.='')]").Count - 1
                            Try
                                M_Author += author.SelectNodes(".//GivenName[not(.='')]")(j).InnerXml
                                If (j <> author.SelectNodes(".//GivenName[not(.='')]").Count - 1) Then
                                    M_Author += " "
                                End If
                            Catch ex As Exception

                            End Try

                        Next
                        M_Author += "</M_GivenName>"
                    End If
                Catch ex As Exception

                End Try
                If (IsNothing(author.SelectSingleNode(".//Suffix[not(.='')]")) = False) Then
                    M_Author += "<M_Suffix type='Suffix'>" + author.SelectSingleNode(".//Suffix[not(.='')]").InnerXml + "</M_Suffix>"
                Else
                    M_Author += "<M_Suffix type='Suffix'></M_Suffix>"
                End If
            Catch ex As Exception

            End Try

            Try
                'aff info
                For k As Integer = 0 To Affiliation.Count - 1
                    Try
                        If (IsNothing(Affiliation(k).SelectSingleNode(".//OrgName[not(.='')]")) = False) Then
                            M_Author += "<M_Organization type='Organization'>" + Affiliation(k).SelectSingleNode(".//OrgName[not(.='')]").InnerXml + "</M_Organization>"
                        End If

                        If (IsNothing(Affiliation(k).SelectSingleNode(".//OrgAddress[not(.='')]")) = False) Then
                            ' M_Author += "<M_Address type='Address'>" + Affiliation(k).SelectSingleNode(".//OrgAddress[not(.='')]").InnerText + "</M_Address>"
                            Try
                                Dim force As Boolean = True
                                Dim MainAddress As Xml.XmlNode = Affiliation(k).SelectSingleNode(".//OrgAddress[not(.='')]")
                                Dim DummyAddress As Xml.XmlNode = Xdoc.CreateElement("dummyaddress")
                                DummyAddress.InnerXml = MainAddress.InnerXml
                                'Dim Temp_Nodes As Xml.XmlNodeList = DummyAddress.SelectNodes(".//State[following-sibling::*]|.//Postcode[following-sibling::*]|.//Street[following-sibling::*]|.//City[following-sibling::*]|.//Country[following-sibling::*]")
                                Dim Temp_Nodes As Xml.XmlNodeList = DummyAddress.SelectNodes(".//Postbox[following-sibling::*]|.//Street[following-sibling::*]")
                                If (IsNothing(Temp_Nodes) = False And Temp_Nodes.Count > 0) Then
                                    For i As Integer = 0 To Temp_Nodes.Count - 1
                                        Dim ReqNd As Xml.XmlNode = Temp_Nodes(i)
                                        Try
                                            Dim newNode As Xml.XmlElement = Xdoc.CreateElement("root")
                                            newNode.InnerXml = "<cs_text type='comma_space'>, </cs_text>"

                                            If ReqNd.InnerText.EndsWith(vbNewLine) = False Or force = True Then
                                                While (newNode.ChildNodes.Count > 0)
                                                    ReqNd.ParentNode.InsertAfter(newNode.LastChild, ReqNd)
                                                End While

                                            End If
                                        Catch ex As Exception

                                        End Try
                                    Next
                                End If
                                Try
                                    Dim str As String = DummyAddress.InnerXml
                                    str = str.Replace("</City><Postcode>", "</City> <Postcode>")
                                    str = str.Replace("</Postcode><State>", "</Postcode>, <State>")
                                    str = str.Replace("</State><Country", "</State>, <Country")
                                    str = str.Replace("</City><State>", "</City>, <State>")
                                    str = str.Replace("</State><Postcode>", "</State> <Postcode>")
                                    str = str.Replace("</Postcode><Country", "</Postcode>, <Country")
                                    str = str.Replace("</Postcode><City>", "</Postcode> <City>")
                                    str = str.Replace("</City><Country", "</City>, <Country")
                                    DummyAddress.InnerXml = str
                                    M_Author += "<M_Address type='Address'>" + DummyAddress.InnerXml + "</M_Address>"
                                Catch ex As Exception

                                End Try
                            Catch ex As Exception

                            End Try
                        End If

                        If (IsNothing(Affiliation(k).SelectSingleNode(".//OrgDivision[not(.='')]")) = False) Then
                            M_Author += "<M_Division type='Division'>" + Affiliation(k).SelectSingleNode(".//OrgDivision[not(.='')]").InnerXml + "</M_Division>"
                        End If
                    Catch ex As Exception

                    End Try
                Next
            Catch ex As Exception

            End Try

            Try
                'contact info
                If (IsNothing(author.SelectSingleNode(".//Contact/Email[not(.='')]")) = False) Then
                    M_Author += "<M_Email type='Email'>" + author.SelectSingleNode(".//Contact/Email[not(.='')]").InnerXml + "</M_Email>"
                End If
                If (IsNothing(author.SelectSingleNode(".//Contact/Phone[not(.='')]")) = False) Then
                    M_Author += "<M_Phone type='Phone'>" + author.SelectSingleNode(".//Contact/Phone[not(.='')]").InnerXml + "</M_Phone>"
                End If
                If (IsNothing(author.SelectSingleNode(".//Contact/Fax[not(.='')]")) = False) Then
                    M_Author += "<M_Fax type='Fax'>" + author.SelectSingleNode(".//Contact/Fax[not(.='')]").InnerXml + "</M_Fax>"
                End If
                If (IsNothing(author.SelectSingleNode(".//Contact/URL[not(.='')]")) = False) Then
                    M_Author += "<M_Url type='URL'>" + author.SelectSingleNode(".//Contact/URL[not(.='')]").InnerXml + "</M_Url>"
                End If
            Catch ex As Exception

            End Try
            Return M_Author
        Catch ex As Exception

        End Try
    End Function
End Class
