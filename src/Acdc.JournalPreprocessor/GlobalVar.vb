'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'====================================================================================================================
'PROJECT NAME  : JOURNAL PREPROCESSOR
'CREATED BY    : SUVARNA RAUT
'MODULE NAME   : GlobalVar
'CREATED DATE  : 3RD JUNE 2013
'LAST MODIFIED : 14TH JUNE 2013
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
'===================================================================================================================
Module GlobalVar
    '====================================================START======================================================
    '=============================================================================================================
    Public CrestJobsheetid As String = ""
    Public ProductionInfoId As String = ""
    Public JobSheetID As String = ""
    Public AuthorInfoCapturePrefix As String = "No"
    Public Infilemame As String = ""
    Public xmlFileName As String = ""
    Public CLog As New ClsLogMessage()
    Public configDoc As New Xml.XmlDocument
    Public ErrorMessages As String = ""
    Public layoutname As String = ""
    Public LayoutnameDB As String = ""
    Public FigureConversionFile As String = ""
    Public ProcessFootNoteAsEndnote As String = ""
    Public LinkSource As String = ""
    Public LinkDestination As String = ""
    Public LinksourceAttr As String = ""
    Public LinkdestinationAttr As String = ""
    Public LinksourcePrefix As String = ""
    Public LinkdestinationPrefix As String = ""
    Public LinkSrcIndex As String = ""
    Public LinkdestIndex As String = ""
    Public TableConversionFile As String = ""
    Public EquationPath As String = ""
    Public ConnectionString As String = System.Configuration.ConfigurationSettings.AppSettings("ConnectionString")
    Public oReq As clsPreprocMain
    Public Article_Lg As String = ""
    Public INXMLName As String = ""
    Public PROOF_Info As String = ""
    Public CTSINFO As String = ""
    Public OffPrint_Data As String = ""
    Public XMLM_Data As String = ""
    Public XMLM_Data2 As String = ""
    Public JournalSubType = ""
    Public BOpenChoice As String = ""
    Public AuthorType As String = "author"
    Public LeftRunningTitle As String = ""
    Public RightRunningTitle As String = ""
    Public authornamenew As String = ""
    Public OutPath As String = ""
    Public LeftRunnignFooter As Xml.XmlElement = Nothing
    Public RightRunnignFooter As Xml.XmlElement = Nothing
    Public FirstRunnignFooter As Xml.XmlElement = Nothing
    Public Locationxmlpath As String
    Public NewGraphicsPath As String
    Public EntityPathxml As String = ""
    Public ACDCLayout As Boolean = False
    Public ACDC_Jonsheetpath As String = ""
    Public ACDC_Graphics As String = ""
    Public GlobalRunningOption As String = ""
    Public ACDCXdoc As New Xml.XmlDocument
    '====================================================END======================================================
    '=============================================================================================================
End Module
