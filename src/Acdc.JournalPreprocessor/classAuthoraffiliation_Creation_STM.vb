'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : SUVARNA RAUT
'CLASS NAME    : classAuthoraffiliation_Creation
'CREATED DATE  : 3RD JUNE 2013
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
Imports System.Text.RegularExpressions
Public Class classAuthoraffiliation_Creation_STM
    Dim Xdoc As New Xml.XmlDocument
    Dim affxdoc As New Xml.XmlDocument
    Dim Article_Lg As String = ""
    Public Sub New(ByVal Xxdoc As Xml.XmlDocument, ByVal Art_Lang As String)
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:New
        'PARAMETER    :Xxdoc,Art_Lang
        'AIM          :This is constructor of classAuthoraffiliation_Creation class.
        '=============================================================================================================
        '=============================================================================================================

        Article_Lg = Art_Lang
        Xdoc = Xxdoc
        Dim MainNode As Xml.XmlNode = Xdoc.SelectSingleNode(".//AuthorGroup")
        If (IsNothing(MainNode) = False) Then
            affxdoc.InnerXml = MainNode.OuterXml
            affxdoc.InnerXml = affxdoc.InnerXml.Replace("</", "</test_").Replace("<", "<test_").Replace("<test_/", "</").Replace("<test_!", "<!")
            affxdoc.InnerXml = affxdoc.InnerXml.Replace("_InstitutionalAuthor", "_Author")
            affxdoc.InnerXml = "<Root>" + affxdoc.OuterXml + "</Root>"
        End If
        Fn_Add_Corr_To_Aff()

        Fn_Add_PresentAdd_To_Aff()

        Fn_RearrangeNode()
        ' Fn_RearrangeAffNode()
        Fn_AddCorresplondAttr()
        Dim Finalxml As String = Fn_CreateAuthAffNodeFor_STM()
        '======================================================
        If (Finalxml <> "") Then
      
            '======================================================
            Dim p As Xml.XmlNode = Xxdoc.CreateElement("test")
            p.InnerXml = Finalxml
            Try
                Dim RootNda As Xml.XmlNode = Xxdoc.SelectSingleNode(".//Article")
                If (IsNothing(RootNda) = False) Then
                    RootNda.InsertBefore(p.FirstChild, RootNda.FirstChild)
                End If

            Catch ex As Exception

            End Try
        End If
        '====================================================END======================================================
        '=============================================================================================================
    End Sub
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
                Try
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

                Catch ex As Exception

                End Try
            Next
            If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
                GivenNameStr = GivenNameStr.Replace(". ", ".") + " "
            End If
            Return GivenNameStr

        Catch ex As Exception

        End Try
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Fn_GetAbbr_GivenName(ByVal Finalxml As String) As String
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_GetAbbr_GivenName
        'PARAMETER    :Finalxml
        'AIM          :This function return abbrivated name of given node.
        '=============================================================================================================
        '=============================================================================================================
        Dim b As New Xml.XmlDocument
        If (Finalxml <> "") Then
            b.LoadXml(Finalxml)
        End If
        Try

            Dim Nodes As Xml.XmlNodeList = b.SelectNodes(".//test_GivenName")
            If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
                For i As Integer = 0 To Nodes.Count - 1
                    Dim GivenName As Xml.XmlNode = Nodes(i)
                    If (GivenName.InnerText.Split(" ").Length > 1 Or GivenName.InnerText.Contains("-")) Then
                        GivenName.InnerText = GetAbbrGivenName(GivenName).Trim
                    Else
                        GivenName.InnerText = GivenName.InnerText(0) + "."
                    End If
                Next
            End If
        Catch ex As Exception

        End Try
        Return b.InnerXml
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Fn_CreateAuthAffNodeFor_STM() As String
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_CreateAuthAffNode
        'PARAMETER    :-
        'AIM          :This function create author affiliation node.
        '=============================================================================================================
        '=============================================================================================================
        Dim Finalxml As String = "" 'cs_PrePro_AuthAff
      

        Dim PresentedAtNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote[@Type='PresentedAt']")
        If (IsNothing(PresentedAtNd) = False And PresentedAtNd.Count > 0) Then
            For m As Integer = 0 To PresentedAtNd.Count - 1
                Finalxml += PresentedAtNd(m).OuterXml
            Next

        End If

        Dim PresentedByNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote[@Type='PresentedBy']")
        If (IsNothing(PresentedByNd) = False And PresentedByNd.Count > 0) Then
            For m As Integer = 0 To PresentedByNd.Count - 1
                Finalxml += PresentedByNd(m).OuterXml
            Next

        End If

        Dim DedicationNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote[@Type='Dedication']")
        If (IsNothing(DedicationNd) = False And DedicationNd.Count > 0) Then
            For m As Integer = 0 To DedicationNd.Count - 1
                Finalxml += DedicationNd(m).OuterXml
            Next
        End If


        Dim CommunicatedByNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote[@Type='CommunicatedBy']")
        If (IsNothing(CommunicatedByNd) = False And CommunicatedByNd.Count > 0) Then
            For m As Integer = 0 To CommunicatedByNd.Count - 1
                Finalxml += CommunicatedByNd(m).OuterXml
            Next
        End If

        Dim ArticleNote_Nd As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote[@Type='Misc' and ancestor::ArticleHeader]")
        If (IsNothing(ArticleNote_Nd) = False And ArticleNote_Nd.Count > 0) Then
            For m As Integer = 0 To ArticleNote_Nd.Count - 1
                Finalxml += ArticleNote_Nd(m).OuterXml
            Next
        End If

        Dim Corrigendum_Nd As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote[@Type='Corrigendum']")
        If (IsNothing(Corrigendum_Nd) = False And Corrigendum_Nd.Count > 0) Then
            For m As Integer = 0 To Corrigendum_Nd.Count - 1
                Finalxml += Corrigendum_Nd(m).OuterXml
            Next
        End If

        Dim ESMHintNd As Xml.XmlNodeList = Xdoc.SelectNodes(".//ArticleNote[@Type='ESMHint']")
        If (IsNothing(ESMHintNd) = False And ESMHintNd.Count > 0) Then
            For m As Integer = 0 To ESMHintNd.Count - 1
                Finalxml += ESMHintNd(m).OuterXml
            Next
        End If


        'Main code'
        Dim mystring As String = ""
        Dim AuthNodes As Xml.XmlNodeList = affxdoc.SelectNodes(".//test_AuthorGroup//test_Author")
        If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
            For k As Integer = 0 To AuthNodes.Count - 1
                If ((IsNothing(AuthNodes(k).SelectNodes(".//test_Email")) = False And AuthNodes(k).SelectNodes(".//test_Email").Count > 0) Or (IsNothing(AuthNodes(k).SelectNodes(".//test_URL")) = False And AuthNodes(k).SelectNodes(".//test_URL").Count > 0) Or (IsNothing(AuthNodes(k).Attributes.ItemOf("CorrespondingAffiliationID")) = False)) Then
                    mystring += AuthNodes(k).OuterXml
                End If

            Next
        End If
        Dim AffNodes As Xml.XmlNodeList = affxdoc.SelectNodes(".//test_AuthorGroup//test_Affiliation")
        If (IsNothing(AffNodes) = False And AffNodes.Count > 0) Then
            For k As Integer = 0 To AffNodes.Count - 1
                Dim temstr As String = ""
                If (IsNothing(AffNodes(k).Attributes.ItemOf("ID")) = False) Then
                    temstr = "<test_superscript>" + AffNodes(k).Attributes.ItemOf("ID").Value.Replace("Aff", "") + "</test_superscript>"
                End If
                If (IsNothing(AffNodes(k).Attributes.ItemOf("PresentAffiliationID")) = False) Then
                    AffNodes(k).InnerXml = "<test_PresentAffID>Present address:</test_PresentAffID>" + AffNodes(k).InnerXml
                End If
                mystring += "<test_Affiliation>" + temstr + AffNodes(k).InnerXml + "</test_Affiliation>"
            Next
        End If
        Dim mainstr As String = ""
        mainstr = Finalxml + mystring

        If (mainstr <> "") Then
            mainstr = "<cs_text type=""auth_aff_collections"">" + mainstr + "</cs_text>"
        End If
      
         
        '=============================================================================================================
        '=============================================================================================================



        '=============================================================================================================
        '=============================================================================================================

        Return mainstr
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Rearrange_Newxml_Str(ByVal Newxml_Str As String) As String
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Rearrange_Newxml_Str
        'PARAMETER    :Newxml_Str
        'AIM          :This function do modification and arragement in new created xml string.
        '=============================================================================================================
        '=============================================================================================================
        Dim Result_xml As String = ""

        Dim TempDom As New Xml.XmlDocument
        TempDom.InnerXml = Newxml_Str

ss:     Dim Nodes As Xml.XmlNodeList = TempDom.SelectNodes(".//cs_Sub_PrePro_AuthAff[child::test_Email[@cs_status='onlyemail']]")
        If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
            For m As Integer = 0 To Nodes.Count - 1
                Dim EmailNd As Xml.XmlNode = Nodes(m).SelectSingleNode(".//test_Email[@cs_status='onlyemail']")
                ''Dim atr As Xml.XmlAttribute = affxdoc.CreateAttribute("cs_underprocess")
                ''atr.Value = True
                ''EmailNd.Attributes.Append(atr)
                Dim emailtext As String = EmailNd.InnerXml
                Dim otherNd As Xml.XmlNode = TempDom.SelectSingleNode(".//cs_Sub_PrePro_AuthAff[child::test_Email[not(@cs_status) and .='" + emailtext + "']]")
                If (IsNothing(otherNd) = False) Then
                    'delete main node
                    '  MsgBox(Nodes(m).OuterXml)
                    Nodes(m).ParentNode.RemoveChild(Nodes(m))
                    GoTo ss
                End If
            Next
        End If
        Result_xml = TempDom.OuterXml
        Return (Result_xml)
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Public Function Fn_AddCorresplondAttr()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_AddCorresplondAttr
        'PARAMETER    :-
        'AIM          :This function add cs_type='correspond' attribute to test_AuthorName node.
        '=============================================================================================================
        '=============================================================================================================
        Dim AuthNodes As Xml.XmlNodeList = affxdoc.SelectNodes(".//test_Author") '[@CorrespondingAffiliationID]
        If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
            For i As Integer = 0 To AuthNodes.Count - 1
                Dim attr As Xml.XmlAttribute = affxdoc.CreateAttribute("cs_type")
                If (IsNothing(AuthNodes(i).SelectSingleNode(".//test_AuthorName").ParentNode.Attributes.ItemOf("CorrespondingAffiliationID")) = False) Then
                    attr.Value = "correspond"
                    AuthNodes(i).SelectSingleNode(".//test_AuthorName").Attributes.Append(attr)
                    ' Else
                    'attr.Value = "noncorrespond"
                End If


            Next
        End If
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_Add_Corr_To_Aff()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_Add_Corr_To_Aff
        'PARAMETER    :-
        'AIM          :This function add 'CorrespondingAffiliationID' attribute to test_Affiliation node.
        '=============================================================================================================
        '=============================================================================================================
        Dim AuthNodes As Xml.XmlNodeList = affxdoc.SelectNodes(".//test_AuthorGroup//test_Author[@CorrespondingAffiliationID]")
        If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
            For i As Integer = 0 To AuthNodes.Count - 1
                Dim atr_Val As String = AuthNodes(i).Attributes.ItemOf("CorrespondingAffiliationID").Value
                ' Dim alreadyExist As Boolean = False
                Dim AffNd As Xml.XmlNode = affxdoc.SelectSingleNode(".//test_AuthorGroup//test_Affiliation[@ID='" + atr_Val + "']")
                If (IsNothing(AffNd) = False) Then
                    If (IsNothing(AffNd.Attributes.ItemOf("CorrespondingAffiliationID")) = False) Then
                    Else
                        Dim New_atr As Xml.XmlAttribute = affxdoc.CreateAttribute("CorrespondingAffiliationID")
                        New_atr.Value = atr_Val
                        AffNd.Attributes.Append(New_atr)
                    End If
                End If
            Next
        End If
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_Add_PresentAdd_To_Aff()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_Add_PresentAdd_To_Aff
        'PARAMETER    :-
        'AIM          :This function add 'PresentAffiliationID' attribute to test_Affiliation node.
        '=============================================================================================================
        '=============================================================================================================
        Dim AuthNodes As Xml.XmlNodeList = affxdoc.SelectNodes(".//test_AuthorGroup//test_Author[@PresentAffiliationID]")
        If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
            For i As Integer = 0 To AuthNodes.Count - 1
                Dim atr_Val As String = AuthNodes(i).Attributes.ItemOf("PresentAffiliationID").Value
                ' Dim alreadyExist As Boolean = False
                Dim AffNd As Xml.XmlNode = affxdoc.SelectSingleNode(".//test_AuthorGroup//test_Affiliation[@ID='" + atr_Val + "']")
                If (IsNothing(AffNd) = False) Then
                    If (IsNothing(AffNd.Attributes.ItemOf("PresentAffiliationID")) = False) Then
                    Else
                        Dim New_atr As Xml.XmlAttribute = affxdoc.CreateAttribute("PresentAffiliationID")
                        New_atr.Value = atr_Val
                        AffNd.Attributes.Append(New_atr)
                    End If
                End If
            Next
        End If
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    Private Function Fn_RearrangeAffNode()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_RearrangeAffNode
        'PARAMETER    :-
        'AIM          :This function rearrange author affiliation element.
        '=============================================================================================================
        '=============================================================================================================
        Dim Nodes As Xml.XmlNode = affxdoc.SelectSingleNode(".//test_AuthorGroup")
        If (IsNothing(Nodes) = False) Then
            'Rearrange author node
            'Rearrange affiliation node
            Dim AuthNodes As Xml.XmlNodeList = Nodes.SelectNodes(".//test_Affiliation[@CorrespondingAffiliationID]")
            If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
                Dim ParentNode As Xml.XmlNode = Nothing
                Dim First_serchNd As Xml.XmlNode = Nothing
                For i As Integer = 0 To AuthNodes.Count - 1
                    First_serchNd = Nodes.FirstChild
l:                  If (First_serchNd.OuterXml <> AuthNodes(i).OuterXml) Then
                        If (IsNothing(First_serchNd.Attributes.ItemOf("CorrespondingAffiliationID")) = False) Then
                            'First_serchNd.InsertBefore(AuthNodes(i), First_serchNd)
                            First_serchNd = First_serchNd.NextSibling
                            GoTo l
                        Else
                            First_serchNd.ParentNode.InsertBefore(AuthNodes(i), First_serchNd)

                        End If

                    End If
                Next
            End If
            '====================================================END======================================================
            '=============================================================================================================
        End If
    End Function
    Private Function Fn_RearrangeNode()
        '=============================================================================================================
        '=============================================================================================================
        'FUNCTION NAME:Fn_RearrangeNode
        'PARAMETER    :-
        'AIM          :This function rearrange author affiliation element.
        '=============================================================================================================
        '=============================================================================================================
        Dim Nodes As Xml.XmlNode = affxdoc.SelectSingleNode(".//test_AuthorGroup")
        If (IsNothing(Nodes) = False) Then
            'Rearrange author node
            'Rearrange affiliation node
            Dim AuthNodes As Xml.XmlNodeList = Nodes.SelectNodes(".//test_Author[@CorrespondingAffiliationID]")
            If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
                Dim ParentNode As Xml.XmlNode = Nothing
                Dim First_serchNd As Xml.XmlNode = Nothing
                For i As Integer = 0 To AuthNodes.Count - 1
                    First_serchNd = Nodes.FirstChild
l:                  If (First_serchNd.OuterXml <> AuthNodes(i).OuterXml) Then
                        If (IsNothing(First_serchNd.Attributes.ItemOf("CorrespondingAffiliationID")) = False) Then
                            'First_serchNd.InsertBefore(AuthNodes(i), First_serchNd)
                            First_serchNd = First_serchNd.NextSibling
                            GoTo l
                        Else
                            First_serchNd.ParentNode.InsertBefore(AuthNodes(i), First_serchNd)
                        End If

                    End If
                Next
            End If
        End If
        '====================================================END======================================================
        '=============================================================================================================
    End Function
    '====================================================END======================================================
    '=============================================================================================================
End Class
