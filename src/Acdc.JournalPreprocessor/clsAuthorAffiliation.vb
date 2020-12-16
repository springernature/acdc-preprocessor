''====================================================================================================================
''====================================================================================================================
''====================================================================================================================
''====================================================================================================================
''PROJECT NAME  : JOURNAL PREPROCESSOR
''CREATED BY    : SUVARNA RAUT
''CLASS NAME   : clsAuthorAffiliation
''CREATED DATE  : 3RD JUNE 2013
''LAST MODIFIED : 14TH JUNE 2013
''===================================================================================================================
''===================================================================================================================
''===================================================================================================================
''==========================================================================s=========================================
'Public Class clsAuthorAffiliation
'    Dim Xdoc As New Xml.XmlDocument
'    Dim Article_Lg As String = ""
'    Public Sub New(ByVal Xxdoc As Xml.XmlDocument, ByVal Art_Lang As String)
'        Article_Lg = Art_Lang
'        Xdoc = Xxdoc
'    End Sub
'    Public Function Create_AuthAffiliationNode()
'        '=============================================================================================================
'        '=============================================================================================================
'        'FUNCTION NAME:Create_AuthAffiliationNode
'        'PARAMETER    :-
'        'AIM          :This function create author affiliation node
'        '=============================================================================================================
'        '=============================================================================================================
'        Try

'            Dim authGP As Xml.XmlNode = Xdoc.SelectSingleNode(".//Author[@CorrespondingAffiliationID]/AuthorName|.//InstitutionalAuthor[@CorrespondingAffiliationID]/InstitutionalAuthorName") ''' InstitutionalAuthor
'            If (IsNothing(authGP) = False) Then
'                Dim Attr As Xml.XmlAttribute = Xdoc.CreateAttribute("cs_type")
'                authGP.Attributes.Append(Attr)
'                Attr.Value = "correspond"
'            End If
'        Catch ex As Exception

'        End Try
'        'Create auth+affiliation node
'        Try
'            Dim AuthAffiliationNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'            Dim Attr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'            AuthAffiliationNode.Attributes.Append(Attr)
'            Attr.Value = "auth_aff_collections"
'            'Try
'            '    'Add History date information
'            '    Dim ArticleHistoryNds As Xml.XmlNode = GetHistoryNode()
'            '    If (ArticleHistoryNds.InnerText <> "") Then
'            '        ArticleHistoryNds.InnerXml = ArticleHistoryNds.InnerXml + vbNewLine
'            '        AuthAffiliationNode.AppendChild(ArticleHistoryNds)
'            '    End If
'            'Catch ex As Exception

'            'End Try
'            Try
'                Dim PresentedAtNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='PresentedAt']")
'                If (IsNothing(PresentedAtNd) = False) Then
'                    Dim xnode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                    Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                    atr.Value = PresentedAtNd.Attributes.ItemOf("Type").Value
'                    xnode.Attributes.Append(atr)
'                    xnode.InnerXml = PresentedAtNd.InnerXml
'                    AuthAffiliationNode.InnerXml += xnode.OuterXml
'                    ' AuthAffiliationNode.AppendChild(PresentedAtNd)
'                End If
'                Dim PresentedByNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='PresentedBy']")
'                If (IsNothing(PresentedByNd) = False) Then
'                    Dim xnode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                    Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                    atr.Value = PresentedByNd.Attributes.ItemOf("Type").Value
'                    xnode.Attributes.Append(atr)
'                    xnode.InnerXml = PresentedByNd.InnerXml
'                    AuthAffiliationNode.InnerXml += xnode.OuterXml
'                    'AuthAffiliationNode.AppendChild(PresentedByNd)
'                End If
'                Dim DedicationNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='Dedication']")
'                If (IsNothing(DedicationNd) = False) Then
'                    Dim xnode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                    Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                    atr.Value = DedicationNd.Attributes.ItemOf("Type").Value
'                    xnode.Attributes.Append(atr)
'                    xnode.InnerXml = DedicationNd.InnerXml
'                    AuthAffiliationNode.InnerXml += xnode.OuterXml
'                    'AuthAffiliationNode.AppendChild(DedicationNd)
'                End If
'                Dim CommunicatedByNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='CommunicatedBy']")
'                If (IsNothing(CommunicatedByNd) = False) Then
'                    Dim xnode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                    Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                    atr.Value = CommunicatedByNd.Attributes.ItemOf("Type").Value
'                    xnode.Attributes.Append(atr)
'                    xnode.InnerXml = CommunicatedByNd.InnerXml
'                    AuthAffiliationNode.InnerXml += xnode.OuterXml
'                    'AuthAffiliationNode.AppendChild(CommunicatedByNd)
'                End If

'                Dim MiscNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='Misc']")
'                If (IsNothing(MiscNd) = False) Then
'                    Dim xnode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                    Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                    atr.Value = MiscNd.Attributes.ItemOf("Type").Value
'                    xnode.Attributes.Append(atr)
'                    xnode.InnerXml = MiscNd.InnerXml
'                    AuthAffiliationNode.InnerXml += xnode.OuterXml
'                    ' AuthAffiliationNode.AppendChild(MiscNd)
'                End If
'                Dim ESMHintNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//ArticleNote[@Type='ESMHint']")
'                If (IsNothing(ESMHintNd) = False) Then
'                    Dim xnode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                    Dim atr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                    atr.Value = ESMHintNd.Attributes.ItemOf("Type").Value
'                    xnode.Attributes.Append(atr)
'                    xnode.InnerXml = ESMHintNd.InnerXml
'                    AuthAffiliationNode.InnerXml += xnode.OuterXml
'                    ' AuthAffiliationNode.AppendChild(ESMHintNd)
'                End If
'            Catch ex As Exception

'            End Try
'            Dim AffiliationNds As Xml.XmlNodeList = Xdoc.SelectNodes(".//Affiliation[not(.='')]")
'            If (IsNothing(AffiliationNds) = False) Then
'                Dim Affiliaion_Cnt As Integer = AffiliationNds.Count
'                For i As Integer = 1 To Affiliaion_Cnt
'                    Try
'                        Dim AFF_ID As String = "Aff" + i.ToString
'                        Dim Author_Node As Xml.XmlNodeList = Xdoc.SelectNodes(".//InstitutionalAuthor|.//Author")
'                        Try
'                            Dim SubAuthAffiliationNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                            Dim subatr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                            SubAuthAffiliationNode.Attributes.Append(subatr)
'                            subatr.Value = "sub_auth_aff_collections"
'                            Dim Auth_CollectionNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                            Dim authoratr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                            Auth_CollectionNode.Attributes.Append(authoratr)
'                            authoratr.Value = "auth_collections"
'                            Dim Aff_CollectionNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                            Dim affatr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                            Aff_CollectionNode.Attributes.Append(affatr)
'                            affatr.Value = "aff_collections"
'                            Dim Contact_CollectionNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                            Dim contactatr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                            Contact_CollectionNode.Attributes.Append(contactatr)
'                            contactatr.Value = "contact_collections"
'                            Dim Role_CollectionNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                            Dim roleatr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                            Role_CollectionNode.Attributes.Append(roleatr)
'                            affatr.Value = "role_collections"
'                            '****

'                            '****
'                            Dim secondNode As String = ""
'                            Dim SameAffDiffEmail_String As String = ""
'                            For j As Integer = 0 To Author_Node.Count - 1
'                                Try
'                                    If (Author_Node(j).Attributes.ItemOf("AffiliationIDS").Value.Contains(AFF_ID) = True) Then
'                                        'If (j = 0 Or ((j > 0) And (IsNothing(Author_Node(j).SelectSingleNode(".//Contact[not(.='')]")) = True))) Then
'                                        If (j >= 0) Then
'                                            'first time author node come so add all three information
'                                            Try
'                                                Try
'                                                    'add All three info
'                                                    Try
'                                                        Dim AuthNameNode As Xml.XmlNode = Author_Node(j).SelectSingleNode(".//AuthorName[not(.='')]|.//InstitutionalAuthorName")  ''InstitutionalAuthor
'                                                        Dim removeattr As Boolean = False
'                                                        Try
'                                                            If (IsNothing(AuthNameNode.ParentNode.Attributes.ItemOf("PresentAffiliationID")) = False) Then
'                                                                If (Article_Lg.ToLower = "de") Then
'                                                                    AuthNameNode.InnerXml += "<cs_presentedAff>" + "&#160;" + "(" + "<Emphasis Type='Italic'>" + "gegenwärtige Adresse" + "</Emphasis>" + ")" + "</cs_presentedAff>"
'                                                                Else
'                                                                    If (Article_Lg.ToLower = "en") Then
'                                                                        AuthNameNode.InnerXml += "<cs_presentedAff>" + "&#160;" + "(" + "<Emphasis Type='Italic'>" + "Present address" + "</Emphasis>" + ")" + "</cs_presentedAff>"
'                                                                    End If
'                                                                End If
'                                                            End If
'                                                        Catch ex As Exception

'                                                        End Try
'                                                        If (IsNothing(AuthNameNode) = False) Then
'                                                            Try
'                                                                If (IsNothing(AuthNameNode.ParentNode.Attributes.ItemOf("CorrespondingAffiliationID")) = False) Then
'                                                                    If (AuthNameNode.ParentNode.Attributes.ItemOf("CorrespondingAffiliationID").Value <> AFF_ID) Then
'                                                                        removeattr = True
'                                                                    End If
'                                                                End If
'                                                            Catch ex As Exception

'                                                            End Try
'                                                            If (removeattr = True) Then
'                                                                Dim temp_Str As String = AuthNameNode.OuterXml.Replace("type=""correspond""", "type=""noncorrespond""")
'                                                                Auth_CollectionNode.InnerXml += temp_Str
'                                                            Else
'                                                                If (AuthNameNode.OuterXml.Contains("cs_type") = True) Then
'                                                                    Auth_CollectionNode.InnerXml = AuthNameNode.OuterXml + Auth_CollectionNode.InnerXml
'                                                                Else
'                                                                    Auth_CollectionNode.InnerXml += AuthNameNode.OuterXml
'                                                                End If
'                                                                ' Auth_CollectionNode.InnerXml += AuthNameNode.OuterXml
'                                                            End If

'                                                        End If

'                                                    Catch ex As Exception

'                                                    End Try
'                                                    Try
'                                                        'Role
'                                                        Dim RoleNode As Xml.XmlNode = Author_Node(j).SelectSingleNode(".//Role[not(.='')]")
'                                                        If (IsNothing(RoleNode) = False) Then
'                                                            Role_CollectionNode.InnerXml += RoleNode.OuterXml
'                                                        End If
'                                                    Catch ex As Exception

'                                                    End Try
'                                                    Try
'                                                        'affiliation
'                                                        If (Aff_CollectionNode.InnerText = "") Then
'                                                            Dim Search_String As String = ".//Affiliation[@ID='" + AFF_ID + "']"
'                                                            Dim AffiliationNode As Xml.XmlNode = Xdoc.SelectSingleNode(Search_String)
'                                                            If (IsNothing(AffiliationNode) = False) Then
'                                                                Aff_CollectionNode.InnerXml += AffiliationNode.OuterXml
'                                                            End If
'                                                        End If
'                                                    Catch ex As Exception

'                                                    End Try
'                                                    Try
'                                                        'contact
'                                                        Dim ContactNode As Xml.XmlNode = Author_Node(j).SelectSingleNode(".//Contact[not(.='')]")
'                                                        If (IsNothing(ContactNode) = False) Then
'                                                            '  Contact_CollectionNode.InnerXml += ContactNode.OuterXml
'                                                            If (j = 0) Then
'                                                                Contact_CollectionNode.InnerXml += ContactNode.OuterXml
'                                                            Else
'                                                                If (Contact_CollectionNode.InnerXml = "") Then
'                                                                    Contact_CollectionNode.InnerXml += ContactNode.OuterXml
'                                                                Else
'                                                                    ' Contact_CollectionNode.InnerXml = Contact_CollectionNode.InnerXml.Replace("</Email></Contact>", "; " + ContactNode.InnerText + "</Email></Contact>")
'                                                                    Try
'                                                                        Dim Temp_str As String = ""
'                                                                        Temp_str = "<cs_text type='auth_collections'>" + Author_Node(j).SelectSingleNode(".//AuthorName[not(.='')]").OuterXml + "</cs_text>" + "<cs_text type='contact_collections'>" + ContactNode.OuterXml + "</cs_text>"
'                                                                        If (Temp_str <> "") Then
'                                                                            SameAffDiffEmail_String += "<cs_text type='sub_auth_aff_collections'>" + Temp_str + "</cs_text>"
'                                                                        End If
'                                                                    Catch ex As Exception

'                                                                    End Try
'                                                                End If
'                                                            End If
'                                                        End If
'                                                    Catch ex As Exception

'                                                    End Try
'                                                Catch ex As Exception

'                                                End Try
'                                            Catch ex As Exception

'                                            End Try

'                                        Else
'                                            'MsgBox("ss")
'                                            'create new subauth node and attatched at the ens of main node
'                                            Dim sec_SubAuthAffiliationNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                                            Dim sec_subatr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                                            sec_SubAuthAffiliationNode.Attributes.Append(sec_subatr)
'                                            sec_subatr.Value = "sub_auth_aff_collections"
'                                            Dim sec_Auth_CollectionNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                                            Dim sec_authoratr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                                            sec_Auth_CollectionNode.Attributes.Append(sec_authoratr)
'                                            sec_authoratr.Value = "auth_collections"
'                                            Dim sec_Aff_CollectionNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                                            Dim sec_affatr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                                            sec_Aff_CollectionNode.Attributes.Append(sec_affatr)
'                                            sec_affatr.Value = "aff_collections"
'                                            Dim sec_Contact_CollectionNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                                            Dim sec_contactatr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                                            sec_Contact_CollectionNode.Attributes.Append(sec_contactatr)
'                                            sec_contactatr.Value = "contact_collections"
'                                            Dim sec_Role_CollectionNode As Xml.XmlNode = Xdoc.CreateElement("cs_text")
'                                            Dim sec_roleatr As Xml.XmlAttribute = Xdoc.CreateAttribute("type")
'                                            sec_Role_CollectionNode.Attributes.Append(sec_roleatr)
'                                            sec_roleatr.Value = "role_collections"
'                                            Try
'                                                Try
'                                                    Try
'                                                        'add All three info
'                                                        Try
'                                                            Dim sec_AuthNameNode As Xml.XmlNode = Author_Node(j).SelectSingleNode(".//AuthorName[not(.='')]")
'                                                            If (IsNothing(sec_AuthNameNode) = False) Then
'                                                                sec_Auth_CollectionNode.InnerXml += sec_AuthNameNode.OuterXml
'                                                            End If
'                                                        Catch ex As Exception

'                                                        End Try
'                                                        Try
'                                                            'Role
'                                                            Dim sec_RoleNode As Xml.XmlNode = Author_Node(j).SelectSingleNode(".//Role[not(.='')]")
'                                                            If (IsNothing(sec_RoleNode) = False) Then
'                                                                sec_Role_CollectionNode.InnerXml += sec_RoleNode.OuterXml
'                                                            End If
'                                                        Catch ex As Exception

'                                                        End Try
'                                                        Try
'                                                            'affiliation
'                                                            Dim Search_String As String = ".//Affiliation[@ID='" + AFF_ID + "']"
'                                                            Dim sec_AffiliationNode As Xml.XmlNode = Xdoc.SelectSingleNode(Search_String)
'                                                            If (IsNothing(sec_AffiliationNode) = False) Then
'                                                                sec_Aff_CollectionNode.InnerXml += sec_AffiliationNode.OuterXml
'                                                            End If
'                                                        Catch ex As Exception

'                                                        End Try
'                                                        Try
'                                                            'contact
'                                                            Dim sec_ContactNode As Xml.XmlNode = Author_Node(j).SelectSingleNode(".//Contact[not(.='')]")
'                                                            If (IsNothing(sec_ContactNode) = False) Then
'                                                                sec_Contact_CollectionNode.InnerXml += sec_ContactNode.OuterXml
'                                                            End If
'                                                        Catch ex As Exception

'                                                        End Try
'                                                    Catch ex As Exception

'                                                    End Try
'                                                Catch ex As Exception

'                                                End Try
'                                                If (sec_Auth_CollectionNode.InnerText <> "") Then
'                                                    sec_SubAuthAffiliationNode.AppendChild(sec_Auth_CollectionNode)
'                                                End If
'                                                If (sec_Role_CollectionNode.InnerText <> "") Then
'                                                    sec_SubAuthAffiliationNode.AppendChild(sec_Role_CollectionNode)
'                                                End If
'                                                If (sec_Aff_CollectionNode.InnerText <> "") Then
'                                                    sec_SubAuthAffiliationNode.AppendChild(sec_Aff_CollectionNode)
'                                                End If
'                                                If (sec_Contact_CollectionNode.InnerText <> "") Then
'                                                    sec_SubAuthAffiliationNode.AppendChild(sec_Contact_CollectionNode)
'                                                End If
'                                                Try
'                                                    If (sec_SubAuthAffiliationNode.InnerText <> "") Then
'                                                        secondNode += sec_SubAuthAffiliationNode.OuterXml
'                                                    End If
'                                                Catch ex As Exception

'                                                End Try
'                                            Catch ex As Exception

'                                            End Try

'                                        End If
'                                    End If

'                                Catch ex As Exception

'                                End Try
'                            Next
'                            Try
'                                Try
'                                    'Append to main node
'                                    If (Auth_CollectionNode.InnerText <> "") Then
'                                        SubAuthAffiliationNode.AppendChild(Auth_CollectionNode)
'                                    End If
'                                    If (Role_CollectionNode.InnerText <> "") Then
'                                        SubAuthAffiliationNode.AppendChild(Role_CollectionNode)
'                                    End If
'                                    If (Aff_CollectionNode.InnerText <> "") Then
'                                        SubAuthAffiliationNode.AppendChild(Aff_CollectionNode)
'                                    End If
'                                    If (Contact_CollectionNode.InnerText <> "") Then
'                                        SubAuthAffiliationNode.AppendChild(Contact_CollectionNode)
'                                    End If
'                                Catch ex As Exception

'                                End Try
'                                Try
'                                    If (SubAuthAffiliationNode.InnerText <> "") Then
'                                        AuthAffiliationNode.AppendChild(SubAuthAffiliationNode)
'                                    End If
'                                Catch ex As Exception

'                                End Try
'                                If (secondNode <> "") Then
'                                    AuthAffiliationNode.InnerXml += secondNode
'                                End If
'                                Try
'                                    If (SameAffDiffEmail_String <> "") Then
'                                        AuthAffiliationNode.InnerXml += SameAffDiffEmail_String
'                                    End If
'                                Catch ex As Exception

'                                End Try
'                            Catch ex As Exception

'                            End Try


'                        Catch ex As Exception

'                        End Try
'                    Catch ex As Exception

'                    End Try
'                Next

'            End If
'            Try
'                Dim RootNd As Xml.XmlNode = Xdoc.SelectSingleNode(".//Article/ArticleHeader")
'                If (IsNothing(RootNd) = False) Then
'                    If (AuthAffiliationNode.InnerText <> "") Then
'                        RootNd.InsertBefore(AuthAffiliationNode, RootNd.FirstChild)
'                    End If
'                End If
'            Catch ex As Exception

'            End Try
'        Catch ex As Exception

'        End Try
'        'Author -GivenName FamilyName Prefix Sufix Particle
'        'Affiliation -Orgdivision,Name,Address=Street,postcode,city,country
'        'Role
'        '====================================================END======================================================
'        '=============================================================================================================
'    End Function
'    '====================================================END======================================================
'    '=============================================================================================================
'End Class
