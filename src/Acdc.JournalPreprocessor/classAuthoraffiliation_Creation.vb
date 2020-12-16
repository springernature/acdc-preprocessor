'' ''====================================================================================================================
'' ''====================================================================================================================
'' ''====================================================================================================================
'' ''====================================================================================================================
'' ''PROJECT NAME  : JOURNAL PREPROCESSOR
'' ''CREATED BY    : SUVARNA RAUT
'' ''CLASS NAME    : classAuthoraffiliation_Creation
'' ''CREATED DATE  : 3RD JUNE 2013
'' ''====================================================================================================================
'' ''====================================================================================================================
'' ''====================================================================================================================
'' ''====================================================================================================================
' ''Imports System.Text.RegularExpressions
' ''Public Class classAuthoraffiliation_Creation
' ''    Dim Xdoc As New Xml.XmlDocument
' ''    Dim affxdoc As New Xml.XmlDocument
' ''    Dim Article_Lg As String = ""
' ''    Public Sub New(ByVal Xxdoc As Xml.XmlDocument, ByVal Art_Lang As String)
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        'FUNCTION NAME:New
' ''        'PARAMETER    :Xxdoc,Art_Lang
' ''        'AIM          :This is constructor of classAuthoraffiliation_Creation class.
' ''        '=============================================================================================================
' ''        '=============================================================================================================

' ''        Article_Lg = Art_Lang
' ''        Xdoc = Xxdoc
' ''        Dim MainNode As Xml.XmlNode = Xdoc.SelectSingleNode(".//AuthorGroup")
' ''        If (IsNothing(MainNode) = False) Then
' ''            affxdoc.InnerXml = MainNode.OuterXml
' ''            affxdoc.InnerXml = affxdoc.InnerXml.Replace("</", "</test_").Replace("<", "<test_").Replace("<test_/", "</").Replace("<test_!", "<!")
' ''            affxdoc.InnerXml = affxdoc.InnerXml.Replace("_InstitutionalAuthor", "_Author")
' ''            affxdoc.InnerXml = "<Root>" + affxdoc.OuterXml + "</Root>"
' ''        End If
' ''        Fn_Add_Corr_To_Aff()
' ''        Fn_RearrangeNode()
' ''        Fn_RearrangeAffNode()
' ''        Fn_AddCorresplondAttr()
' ''        Dim Finalxml As String = Fn_CreateAuthAffNode()
' ''        Finalxml = Fn_GetAbbr_GivenName(Finalxml)

' ''        '======================================================
' ''        If (Finalxml <> "") Then
' ''            Dim asas As String = Finalxml
' ''            asas = Regex.Replace(asas, "<test_", "<", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "</test_", "</", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "<cs_PrePro_AuthAff>", "<cs_text type=""auth_aff_collections"">", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "</cs_PrePro_AuthAff>", "</cs_text>", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "<cs_Sub_PrePro_AuthAff>", "<cs_text type=""sub_auth_aff_collections"">", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "</cs_Sub_PrePro_AuthAff>", "</cs_text>", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "<AuthorName", "<cs_text type=""auth_collections""><AuthorName", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "</AuthorName>", "</AuthorName></cs_text>", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "</AuthorName></cs_text><cs_text type=""auth_collections"">", "</AuthorName>", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "<Role>", "<cs_text type=""role_collections""><Role>", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "</Role>", "</Role></cs_text>", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "<Affiliation", "<cs_text type=""aff_collections""><Affiliation", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "</Affiliation>", "</Affiliation></cs_text>", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "<Email", "<cs_text type=""contact_collections""><Email", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "</Email>", "</Email></cs_text>", RegexOptions.Singleline)

' ''            asas = Regex.Replace(asas, "<Email", "<Contact><Email", RegexOptions.Singleline)
' ''            asas = Regex.Replace(asas, "</Email>", "</Email></Contact>", RegexOptions.Singleline)
' ''            Finalxml = asas
' ''            '======================================================
' ''            Dim p As Xml.XmlNode = Xxdoc.CreateElement("test")
' ''            p.InnerXml = Finalxml
' ''            Try
' ''                Dim RootNda As Xml.XmlNode = Xxdoc.SelectSingleNode(".//Article")
' ''                If (IsNothing(RootNda) = False) Then
' ''                    RootNda.InsertBefore(p.FirstChild, RootNda.FirstChild)
' ''                End If

' ''            Catch ex As Exception

' ''            End Try
' ''        End If
' ''        '====================================================END======================================================
' ''        '=============================================================================================================
' ''    End Sub
' ''    Private Function GetAbbrGivenName(ByVal GNames As Xml.XmlNode) As String
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        'FUNCTION NAME:GetAbbrGivenName
' ''        'PARAMETER    :GNames
' ''        'AIM          :This function return abbrivated name of given node.
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        Try
' ''            Dim GivenName As Xml.XmlNode = GNames
' ''            Dim GivenNameStr As String = ""
' ''            For i As Integer = 0 To GivenName.InnerText.Split(" ").Length - 1
' ''                Try
' ''                    If (GivenName.InnerText.Split("-").Length > 1) Then
' ''                        For j As Integer = 0 To GivenName.InnerText.Split("-").Length - 1
' ''                            If (j = GivenName.InnerText.Split("-").Length - 1) Then
' ''                                GivenNameStr += GivenName.InnerText.Split("-")(j)(0) + ". "
' ''                            Else
' ''                                GivenNameStr += GivenName.InnerText.Split("-")(j)(0) + ".-"
' ''                            End If
' ''                        Next
' ''                    Else
' ''                        GivenNameStr += GivenName.InnerText.Split(" ")(i)(0) + ". "
' ''                    End If

' ''                Catch ex As Exception

' ''                End Try
' ''            Next
' ''            If (LayoutnameDB.ToLower.Contains("bsl_t1_grey") = True) Then
' ''                GivenNameStr = GivenNameStr.Replace(". ", ".") + " "
' ''            End If
' ''            Return GivenNameStr

' ''        Catch ex As Exception

' ''        End Try
' ''        '====================================================END======================================================
' ''        '=============================================================================================================
' ''    End Function
' ''    Public Function Fn_GetAbbr_GivenName(ByVal Finalxml As String) As String
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        'FUNCTION NAME:Fn_GetAbbr_GivenName
' ''        'PARAMETER    :Finalxml
' ''        'AIM          :This function return abbrivated name of given node.
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        Dim b As New Xml.XmlDocument
' ''        If (Finalxml <> "") Then
' ''            b.LoadXml(Finalxml)
' ''        End If
' ''        Try

' ''            Dim Nodes As Xml.XmlNodeList = b.SelectNodes(".//test_GivenName")
' ''            If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
' ''                For i As Integer = 0 To Nodes.Count - 1
' ''                    Dim GivenName As Xml.XmlNode = Nodes(i)
' ''                    If (GivenName.InnerText.Split(" ").Length > 1 Or GivenName.InnerText.Contains("-")) Then
' ''                        GivenName.InnerText = GetAbbrGivenName(GivenName).Trim
' ''                    Else
' ''                        GivenName.InnerText = GivenName.InnerText(0) + "."
' ''                    End If
' ''                Next
' ''            End If
' ''        Catch ex As Exception

' ''        End Try
' ''        Return b.InnerXml
' ''        '====================================================END======================================================
' ''        '=============================================================================================================
' ''    End Function
' ''    Public Function Fn_CreateAuthAffNode() As String
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        'FUNCTION NAME:Fn_CreateAuthAffNode
' ''        'PARAMETER    :-
' ''        'AIM          :This function create author affiliation node.
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        Dim Finalxml As String = "" 'cs_PrePro_AuthAff
' ''        Dim FinalSubStraing As String = "" 'cs_Sub_PrePro_AuthAff
' ''        Dim Final_2ndlevelString As String = "" 'cs_Sub_PrePro_AuthAff

' ''        'Add ArticleNote


' ''        Dim PresentedAtNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='PresentedAt']")
' ''        If (IsNothing(PresentedAtNd) = False) Then
' ''            Finalxml += PresentedAtNd.OuterXml
' ''        End If

' ''        Dim PresentedByNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='PresentedBy']")
' ''        If (IsNothing(PresentedByNd) = False) Then
' ''            Finalxml += PresentedByNd.OuterXml
' ''        End If

' ''        Dim DedicationNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='Dedication']")
' ''        If (IsNothing(DedicationNd) = False) Then
' ''            Finalxml += DedicationNd.OuterXml
' ''        End If


' ''        Dim CommunicatedByNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='CommunicatedBy']")
' ''        If (IsNothing(CommunicatedByNd) = False) Then
' ''            Finalxml += CommunicatedByNd.OuterXml
' ''        End If

' ''        Dim ArticleNote_Nd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='Misc']")
' ''        If (IsNothing(ArticleNote_Nd) = False) Then
' ''            Finalxml += ArticleNote_Nd.OuterXml
' ''        End If

' ''        Dim ESMHintNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='ESMHint']")
' ''        If (IsNothing(ESMHintNd) = False) Then
' ''            Finalxml += ESMHintNd.OuterXml
' ''        End If



' ''        Dim FinalAuthStr As String = ""
' ''        Dim FinalAffStr As String = ""
' ''        Dim FinalRoleStr As String = ""
' ''        Dim FinalEmailStr As String = ""
' ''        Dim Aff_Nodes As Xml.XmlNodeList = affxdoc.SelectNodes(".//test_Affiliation")
' ''        If (IsNothing(Aff_Nodes) = False And Aff_Nodes.Count > 0) Then
' ''            For afx As Integer = 0 To Aff_Nodes.Count - 1
' ''                FinalAuthStr = ""
' ''                FinalAffStr = ""
' ''                FinalRoleStr = ""
' ''                FinalEmailStr = ""
' ''                Final_2ndlevelString = ""
' ''                Dim Current_Aff_ID As String = Aff_Nodes(afx).Attributes.ItemOf("ID").Value
' ''                '======================================================================================auth start  
' ''                Dim Auth_Nodes As Xml.XmlNodeList = affxdoc.SelectNodes(".//test_Author")
' ''                If (IsNothing(Auth_Nodes) = False And Auth_Nodes.Count > 0) Then
' ''                    Dim AuthStatus As Boolean = False
' ''                    Dim hect As Boolean = False
' ''                    For ssr As Integer = 0 To Auth_Nodes.Count - 1
' ''                        If (Auth_Nodes(ssr).Attributes.ItemOf("AffiliationIDS").Value.Contains(Current_Aff_ID) = True) Then

' ''                            If (IsNothing(Auth_Nodes(ssr).Attributes.ItemOf("cs_added")) = False) Then
' ''                                AuthStatus = True
' ''                            End If

' ''                            If (AuthStatus = True) Then
' ''                                If (hect = False) Then
' ''                                    FinalAuthStr = Auth_Nodes(ssr).SelectSingleNode(".//test_AuthorName").OuterXml + FinalAuthStr
' ''                                    If (IsNothing(Auth_Nodes(ssr).Attributes.ItemOf("cs_added")) = True And AuthStatus = True) Then
' ''                                        For twt As Integer = 0 To Auth_Nodes(ssr).SelectNodes(".//test_Contact/test_Email").Count - 1
' ''                                            FinalEmailStr += Auth_Nodes(ssr).SelectNodes(".//test_Contact/test_Email")(twt).OuterXml
' ''                                        Next
' ''                                        hect = True
' ''                                    End If

' ''                                Else
' ''                                    FinalAuthStr = Auth_Nodes(ssr).SelectSingleNode(".//test_AuthorName").OuterXml + FinalAuthStr
' ''                                    If (FinalEmailStr = "") Then
' ''                                        If (IsNothing(Auth_Nodes(ssr).SelectSingleNode(".//test_Contact/test_Email")) = False) Then
' ''                                            Dim attr As Xml.XmlAttribute = affxdoc.CreateAttribute("cs_added")
' ''                                            attr.Value = "yes"
' ''                                            If (IsNothing(Auth_Nodes(ssr).Attributes.ItemOf("cs_added")) = False) Then
' ''                                                Auth_Nodes(ssr).Attributes.Append(attr)
' ''                                            End If
' ''                                            For twt As Integer = 0 To Auth_Nodes(ssr).SelectNodes(".//test_Contact/test_Email").Count - 1
' ''                                                FinalEmailStr += Auth_Nodes(ssr).SelectNodes(".//test_Contact/test_Email")(twt).OuterXml
' ''                                            Next
' ''                                        End If
' ''                                    End If
' ''                                    AuthStatus = False
' ''                                    hect = False
' ''                                End If

' ''                            Else
' ''                                If (IsNothing(Auth_Nodes(ssr).SelectSingleNode(".//test_AuthorName").Attributes.ItemOf("cs_type")) = False) Then
' ''                                    FinalAuthStr += "<test_AuthorName cs_type='correspond'>" + Auth_Nodes(ssr).SelectSingleNode(".//test_AuthorName").InnerXml + "</test_AuthorName>"
' ''                                    Auth_Nodes(ssr).SelectSingleNode(".//test_AuthorName").Attributes.Remove(Auth_Nodes(ssr).SelectSingleNode(".//test_AuthorName").Attributes.ItemOf("cs_type"))
' ''                                Else
' ''                                    FinalAuthStr += "<test_AuthorName>" + Auth_Nodes(ssr).SelectSingleNode(".//test_AuthorName").InnerXml + "</test_AuthorName>"
' ''                                End If

' ''                                If (FinalEmailStr = "") Then
' ''                                    If (IsNothing(Auth_Nodes(ssr).SelectSingleNode(".//test_Contact/test_Email")) = False) Then
' ''                                        Dim attr As Xml.XmlAttribute = affxdoc.CreateAttribute("cs_added")
' ''                                        attr.Value = "yes"
' ''                                        'If (IsNothing(Auth_Nodes(ssr).Attributes.ItemOf("cs_added")) = False) Then
' ''                                        '    Auth_Nodes(ssr).Attributes.Append(attr)
' ''                                        'End If
' ''                                        Auth_Nodes(ssr).Attributes.Append(attr)
' ''                                        For twt As Integer = 0 To Auth_Nodes(ssr).SelectNodes(".//test_Contact/test_Email").Count - 1
' ''                                            FinalEmailStr += Auth_Nodes(ssr).SelectNodes(".//test_Contact/test_Email")(twt).OuterXml
' ''                                        Next
' ''                                    End If
' ''                                End If
' ''                                If (ssr > 0) Then
' ''                                    'sub whole string
' ''                                    If (IsNothing(Auth_Nodes(ssr).SelectSingleNode(".//test_Contact/test_Email")) = False) Then
' ''                                        Final_2ndlevelString += "<cs_Sub_PrePro_AuthAff>" + Auth_Nodes(ssr).SelectSingleNode(".//test_AuthorName").OuterXml
' ''                                        For twt As Integer = 0 To Auth_Nodes(ssr).SelectNodes(".//test_Contact/test_Email").Count - 1
' ''                                            Final_2ndlevelString += "<test_Email cs_status='onlyemail'>" + Auth_Nodes(ssr).SelectNodes(".//test_Contact/test_Email")(twt).InnerXml + "</test_Email>"
' ''                                        Next
' ''                                        Final_2ndlevelString += "</cs_Sub_PrePro_AuthAff>"
' ''                                    End If
' ''                                End If

' ''                            End If


' ''                        End If

' ''                        '======================================================================================Role start
' ''                        'Role
' ''                        If (FinalRoleStr = "") Then
' ''                            If (IsNothing(Auth_Nodes(ssr).SelectSingleNode(".//test_Role")) = False) Then
' ''                                FinalRoleStr += Auth_Nodes(ssr).SelectSingleNode(".//test_Role").OuterXml
' ''                            End If
' ''                        End If
' ''                        '======================================================================================Role end

' ''                    Next
' ''                End If
' ''                '======================================================================================auth end

' ''                '======================================================================================Aff start
' ''                FinalAffStr += Aff_Nodes(afx).OuterXml
' ''                '======================================================================================Aff end
' ''                FinalSubStraing = "<cs_Sub_PrePro_AuthAff>" + FinalAuthStr + FinalRoleStr + FinalAffStr + FinalEmailStr + "</cs_Sub_PrePro_AuthAff>"
' ''                If (FinalSubStraing <> "") Then
' ''                    Finalxml += FinalSubStraing
' ''                End If
' ''                If (Final_2ndlevelString <> "") Then
' ''                    Finalxml += Final_2ndlevelString
' ''                End If
' ''            Next

' ''            If (Finalxml <> "") Then
' ''                Finalxml = "<cs_PrePro_AuthAff>" + Finalxml + "</cs_PrePro_AuthAff>"
' ''            End If
' ''            Finalxml = Rearrange_Newxml_Str(Finalxml)

' ''        End If
' ''        Return Finalxml
' ''        '====================================================END======================================================
' ''        '=============================================================================================================
' ''    End Function
' ''    Public Function Rearrange_Newxml_Str(ByVal Newxml_Str As String) As String
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        'FUNCTION NAME:Rearrange_Newxml_Str
' ''        'PARAMETER    :Newxml_Str
' ''        'AIM          :This function do modification and arragement in new created xml string.
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        Dim Result_xml As String = ""

' ''        Dim TempDom As New Xml.XmlDocument
' ''        TempDom.InnerXml = Newxml_Str

' ''ss:     Dim Nodes As Xml.XmlNodeList = TempDom.SelectNodes(".//cs_Sub_PrePro_AuthAff[child::test_Email[@cs_status='onlyemail']]")
' ''        If (IsNothing(Nodes) = False And Nodes.Count > 0) Then
' ''            For m As Integer = 0 To Nodes.Count - 1
' ''                Dim EmailNd As Xml.XmlNode = Nodes(m).SelectSingleNode(".//test_Email[@cs_status='onlyemail']")
' ''                ''Dim atr As Xml.XmlAttribute = affxdoc.CreateAttribute("cs_underprocess")
' ''                ''atr.Value = True
' ''                ''EmailNd.Attributes.Append(atr)
' ''                Dim emailtext As String = EmailNd.InnerXml
' ''                Dim otherNd As Xml.XmlNode = TempDom.SelectSingleNode(".//cs_Sub_PrePro_AuthAff[child::test_Email[not(@cs_status) and .='" + emailtext + "']]")
' ''                If (IsNothing(otherNd) = False) Then
' ''                    'delete main node
' ''                    '  MsgBox(Nodes(m).OuterXml)
' ''                    Nodes(m).ParentNode.RemoveChild(Nodes(m))
' ''                    GoTo ss
' ''                End If
' ''            Next
' ''        End If
' ''        Result_xml = TempDom.OuterXml
' ''        Return (Result_xml)
' ''        '====================================================END======================================================
' ''        '=============================================================================================================
' ''    End Function
' ''    Public Function Fn_AddCorresplondAttr()
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        'FUNCTION NAME:Fn_AddCorresplondAttr
' ''        'PARAMETER    :-
' ''        'AIM          :This function add cs_type='correspond' attribute to test_AuthorName node.
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        Dim AuthNodes As Xml.XmlNodeList = affxdoc.SelectNodes(".//test_Author") '[@CorrespondingAffiliationID]
' ''        If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
' ''            For i As Integer = 0 To AuthNodes.Count - 1
' ''                Dim attr As Xml.XmlAttribute = affxdoc.CreateAttribute("cs_type")
' ''                If (IsNothing(AuthNodes(i).SelectSingleNode(".//test_AuthorName").ParentNode.Attributes.ItemOf("CorrespondingAffiliationID")) = False) Then
' ''                    attr.Value = "correspond"
' ''                    AuthNodes(i).SelectSingleNode(".//test_AuthorName").Attributes.Append(attr)
' ''                    ' Else
' ''                    'attr.Value = "noncorrespond"
' ''                End If


' ''            Next
' ''        End If
' ''        '====================================================END======================================================
' ''        '=============================================================================================================
' ''    End Function
' ''    Private Function Fn_Add_Corr_To_Aff()
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        'FUNCTION NAME:Fn_Add_Corr_To_Aff
' ''        'PARAMETER    :-
' ''        'AIM          :This function add 'CorrespondingAffiliationID' attribute to test_Affiliation node.
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        Dim AuthNodes As Xml.XmlNodeList = affxdoc.SelectNodes(".//test_AuthorGroup//test_Author[@CorrespondingAffiliationID]")
' ''        If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
' ''            For i As Integer = 0 To AuthNodes.Count - 1
' ''                Dim atr_Val As String = AuthNodes(i).Attributes.ItemOf("CorrespondingAffiliationID").Value
' ''                ' Dim alreadyExist As Boolean = False
' ''                Dim AffNd As Xml.XmlNode = affxdoc.SelectSingleNode(".//test_AuthorGroup//test_Affiliation[@ID='" + atr_Val + "']")
' ''                If (IsNothing(AffNd) = False) Then
' ''                    If (IsNothing(AffNd.Attributes.ItemOf("CorrespondingAffiliationID")) = False) Then
' ''                    Else
' ''                        Dim New_atr As Xml.XmlAttribute = affxdoc.CreateAttribute("CorrespondingAffiliationID")
' ''                        New_atr.Value = atr_Val
' ''                        AffNd.Attributes.Append(New_atr)
' ''                    End If
' ''                End If
' ''            Next
' ''        End If
' ''        '====================================================END======================================================
' ''        '=============================================================================================================
' ''    End Function
' ''    Private Function Fn_RearrangeAffNode()
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        'FUNCTION NAME:Fn_RearrangeAffNode
' ''        'PARAMETER    :-
' ''        'AIM          :This function rearrange author affiliation element.
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        Dim Nodes As Xml.XmlNode = affxdoc.SelectSingleNode(".//test_AuthorGroup")
' ''        If (IsNothing(Nodes) = False) Then
' ''            'Rearrange author node
' ''            'Rearrange affiliation node
' ''            Dim AuthNodes As Xml.XmlNodeList = Nodes.SelectNodes(".//test_Affiliation[@CorrespondingAffiliationID]")
' ''            If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
' ''                Dim ParentNode As Xml.XmlNode = Nothing
' ''                Dim First_serchNd As Xml.XmlNode = Nothing
' ''                For i As Integer = 0 To AuthNodes.Count - 1
' ''                    First_serchNd = Nodes.FirstChild
' ''l:                  If (First_serchNd.OuterXml <> AuthNodes(i).OuterXml) Then
' ''                        If (IsNothing(First_serchNd.Attributes.ItemOf("CorrespondingAffiliationID")) = False) Then
' ''                            'First_serchNd.InsertBefore(AuthNodes(i), First_serchNd)
' ''                            First_serchNd = First_serchNd.NextSibling
' ''                            GoTo l
' ''                        Else
' ''                            First_serchNd.ParentNode.InsertBefore(AuthNodes(i), First_serchNd)

' ''                        End If

' ''                    End If
' ''                Next
' ''            End If
' ''            '====================================================END======================================================
' ''            '=============================================================================================================
' ''        End If
' ''    End Function
' ''    Private Function Fn_RearrangeNode()
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        'FUNCTION NAME:Fn_RearrangeNode
' ''        'PARAMETER    :-
' ''        'AIM          :This function rearrange author affiliation element.
' ''        '=============================================================================================================
' ''        '=============================================================================================================
' ''        Dim Nodes As Xml.XmlNode = affxdoc.SelectSingleNode(".//test_AuthorGroup")
' ''        If (IsNothing(Nodes) = False) Then
' ''            'Rearrange author node
' ''            'Rearrange affiliation node
' ''            Dim AuthNodes As Xml.XmlNodeList = Nodes.SelectNodes(".//test_Author[@CorrespondingAffiliationID]")
' ''            If (IsNothing(AuthNodes) = False And AuthNodes.Count > 0) Then
' ''                Dim ParentNode As Xml.XmlNode = Nothing
' ''                Dim First_serchNd As Xml.XmlNode = Nothing
' ''                For i As Integer = 0 To AuthNodes.Count - 1
' ''                    First_serchNd = Nodes.FirstChild
' ''l:                  If (First_serchNd.OuterXml <> AuthNodes(i).OuterXml) Then
' ''                        If (IsNothing(First_serchNd.Attributes.ItemOf("CorrespondingAffiliationID")) = False) Then
' ''                            'First_serchNd.InsertBefore(AuthNodes(i), First_serchNd)
' ''                            First_serchNd = First_serchNd.NextSibling
' ''                            GoTo l
' ''                        Else
' ''                            First_serchNd.ParentNode.InsertBefore(AuthNodes(i), First_serchNd)
' ''                        End If

' ''                    End If
' ''                Next
' ''            End If
' ''        End If
' ''        '====================================================END======================================================
' ''        '=============================================================================================================
' ''    End Function
' ''    '====================================================END======================================================
' ''    '=============================================================================================================
' ''End Class
