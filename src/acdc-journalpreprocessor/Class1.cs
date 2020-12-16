// ====================================================================================================================
// ====================================================================================================================
// ====================================================================================================================
// ====================================================================================================================
// PROJECT NAME  : JOURNAL PREPROCESSOR
// CREATED BY    : SUVARNA RAUT
// MODULE NAME   : clsPreprocMain
// CREATED DATE  : 3RD JUNE 2013
// ====================================================================================================================
// ====================================================================================================================
// ====================================================================================================================
// ====================================================================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
namespace Acdc.Journalpreprocessor
{
    public class clsPreprocMain
    {
        private string XmlFile;
        private string XmlStr;
        public System.Xml.XmlDocument XDoc = new System.Xml.XmlDocument();
        private System.Xml.NameTable NTable;
        public static System.Xml.XmlNamespaceManager NS;
        public static Hashtable NSht = new Hashtable();
        public clsTableConversion oTableObj;
        public clsFigureConversion oFigObj;

        public clsPreprocMain(string XFile)
        {
            XDoc.PreserveWhitespace = true;

            // ' add in to VSS
            if (File.Exists(XFile))
            {
                XmlFile = XFile;
                try
                {
                    EquationCorrection(XmlFile);
                }
                catch (Exception ex)
                {
                }

                
                StreamReader sr = new StreamReader(XFile);

                XmlStr = sr.ReadToEnd();
                XmlStr = Regex.Replace(XmlStr, "<!--<!DOCTYPE([^<>]+?)>-->", "", RegexOptions.Singleline);
                XmlStr = Regex.Replace(XmlStr, "<!-- <!DOCTYPE([^<>]+?)> -->", "", RegexOptions.Singleline);

                XmlStr = Regex.Replace(XmlStr, "<!DOCTYPE([^<>]+?)>", "<!--<!DOCTYPE -->", RegexOptions.Singleline); // 'DD:17/08/2011 M.K


                XmlStr = RemoveIndentSpace(XmlStr);
                XmlStr = XmlStr.Replace(" ", " ");
                
                XmlStr = XmlStr.Replace("\n", "<!-- enter -->");
                XmlStr = Regex.Replace(XmlStr, @"<\?xml([^<>]+?\?>)", @"<!--<?xml\1\?> -->", RegexOptions.Singleline);

                XmlStr = Regex.Replace(XmlStr, "char=\"&#x00D7;\"", "char=\"×\"", RegexOptions.Singleline); // multi
                XmlStr = Regex.Replace(XmlStr, "char=\"&#x2013;\"", "char=\"–\"", RegexOptions.Singleline);  // endash
                XmlStr = Regex.Replace(XmlStr, "char=\"&#x00B1;\"", "char=\"±\"", RegexOptions.Singleline);  // plusminus
                XmlStr = Regex.Replace(XmlStr, "char=\"&#x2212;\"", "char=\"−\"", RegexOptions.Singleline);  // minus
                XmlStr = Regex.Replace(XmlStr, "char=\"&#X00B7;\"", "char=\"·\"", RegexOptions.Singleline);  // middot

                XmlStr = Regex.Replace(XmlStr, "char=\"&#x002B;\"", "char=\"+\"", RegexOptions.Singleline);  // plus
                                                                                                             // 'added by suru on 28.05.2012
                                                                                                             // XmlStr = XmlStr.Replace("&#x03B2;", "<cs_text type=""entity"" xmlns:aid=""http://ns.adobe.com/AdobeInDesign/4.0/"" aid:pstyle=""Body_Text_Indent_italic"">" + "&#x03B2;" + "</cs_text>")


                string lang = getLanguage(XmlFile);


                if ((lang.ToLower() == "en"))
                    XmlStr = Regex.Replace(XmlStr, @"(<!--\s*Query ID=""Q)(\d+)"" Text=""(([^>]*))""\s*-->", "<cs_query repoID=\"" + "$2" + "\">" + "AQ" + "$2.&#8194;" + "<cs_text type='indentohere'></cs_text>" + "$3" + Constants.vbNewLine + "</cs_query>");
                else
                    XmlStr = Regex.Replace(XmlStr, @"(<!--\s*Query ID=""Q)(\d+)"" Text=""(([^>]*))""\s*-->", "<cs_query repoID=\"" + "$2" + "\">" + "FA" + "$2.&#8194;" + "<cs_text type='indentohere'></cs_text>" + "$3" + Constants.vbNewLine + "</cs_query>");

              
                XmlStr = XmlStr.Replace("&#x2502;", "<cs_text type=\"entity\" xmlns:aid=\"http://ns.adobe.com/AdobeInDesign/4.0/\" aid:cstyle=\"Black Square\">" + "&#x2502;" + "</cs_text>");




                
                XmlStr = Regex.Replace(XmlStr, "<!--<!DOCTYPE([^<>]+?)>-->", "", RegexOptions.Singleline);
                // For Equal Symbol start
                XmlStr = Regex.Replace(XmlStr, "&#x2009;=&#x2009;", "&#x2009;<cs_text type='cSYMBOL'>=</cs_text>&#x2009;", RegexOptions.Singleline);
                XmlStr = Regex.Replace(XmlStr, "<Superscript>=</Superscript>", "<Superscript><cs_text type='cSYMBOL'>=</cs_text></Superscript>", RegexOptions.Singleline);
                XmlStr = Regex.Replace(XmlStr, "<Subscript>=</Subscript>", "<Subscript><cs_text type='cSYMBOL'>=</cs_text></Subscript>", RegexOptions.Singleline);
                XmlStr = Regex.Replace(XmlStr, "<Emphasis Type=\"Bold\">=</Emphasis>", "<Emphasis Type='Bold'><cs_text type='cSYMBOL'>=</cs_text></Emphasis>", RegexOptions.Singleline);
                XmlStr = Regex.Replace(XmlStr, "<Emphasis Type=\"Italic\">=</Emphasis>", "<Emphasis Type='Italic'><cs_text type='cSYMBOL'>=</cs_text></Emphasis>", RegexOptions.Singleline);
                XmlStr = Regex.Replace(XmlStr, "<Emphasis Type=\"BoldItalic\">=</Emphasis>", "<Emphasis Type='BoldItalic'><cs_text type='cSYMBOL'>=</cs_text></Emphasis>", RegexOptions.Singleline);

                // For Equal Symbol end
                sr.Close();

                StreamWriter sw = new StreamWriter(@"d:\tt.xml", false, System.Text.Encoding.GetEncoding(1252));
                sw.Write(XmlStr);
                sw.Close();
                NTable = new NameTable();
                NS = new System.Xml.XmlNamespaceManager(NTable);

                NS.AddNamespace("cs", "http://www.crest-premedia.in");
                NS.AddNamespace("aid", "http://ns.adobe.com/AdobeInDesign/4.0/");
                NS.AddNamespace("aid5", "http://ns.adobe.com/AdobeInDesign/5.0/");

                NSht.Add("cs", "http://www.crest-premedia.in");
                NSht.Add("aid", "http://ns.adobe.com/AdobeInDesign/4.0/");
                NSht.Add("aid5", "http://ns.adobe.com/AdobeInDesign/5.0/");

                try
                {
                    ClsEntity obj = new ClsEntity();
                    XmlStr = obj.ConvertSymbolEntities(XmlStr, EntityPathxml);
                }
                catch (Exception ex)
                {
                    CLog.LogMessages("Error in EntityConversion()" + Constants.vbNewLine);
                    CLog.LogMessages(ex.Message.ToString() + Constants.vbNewLine);
                    throw;
                }

                XDoc = CreateDom(XmlStr);


                // '''''''''''''''''''''CreateEntity start''''''''''''''''''''''''



                // '''''''''''''''''''''CreateEntity end''''''''''''''''''''''''

                // Added below code for log message
                // equation
                if ((Information.IsNothing(XDoc.SelectNodes(".//EquationSource")) == false & XDoc.SelectNodes(".//EquationSource").Count > 0))
                    int EquationNds = XDoc.SelectNodes(".//EquationSource").Count;

                // figure
                if ((Information.IsNothing(XDoc.SelectNodes(".//Figure//CaptionNumber[not(.='')]")) == false & XDoc.SelectNodes(".//Figure//CaptionNumber[not(.='')]").Count > 0))
                    System.Xml.XmlNodeList FigNodes = XDoc.SelectNodes(".//Figure//CaptionNumber[not(.='')]");

                // table
                if ((Information.IsNothing(XDoc.SelectNodes(".//Table//CaptionNumber[not(.='')]")) == false & XDoc.SelectNodes(".//Table//CaptionNumber[not(.='')]").Count > 0))
                    System.Xml.XmlNodeList TableNodes = XDoc.SelectNodes(".//Table//CaptionNumber[not(.='')]");

                oTableObj = new clsTableConversion(XDoc, NTable, NS, this);
                oFigObj = new clsFigureConversion(XDoc, NTable, NS, this);
            }
        }
        private string RemoveIndentSpace(string XmlStr)
        {
            try
            {
                XmlDocument sxDoc = new XmlDocument();

                sxDoc.PreserveWhitespace = false;

                XmlWriterSettings xWriteSettings = new XmlWriterSettings();

                xWriteSettings.Indent = false;

                XmlWriter xWrite = null;

                sxDoc.LoadXml(XmlStr);

                xWrite = XmlWriter.Create(@"d:\coversIndentXml.xml", xWriteSettings);

                sxDoc.WriteTo(xWrite);

                xWrite.Close();
                XmlDocument finalxdoc = new XmlDocument();
                finalxdoc.Load(@"d:\coversIndentXml.xml");


                return finalxdoc.OuterXml;
            }

            // '   Debug.WriteLine("")

            catch (Exception ex)
            {
                return null;
            }

            return null;
        }

        private string getLanguage(string xmlFile)
        {
            string lang = "";

            try
            {
                var articleInfo = from nodes in XElement.Load(xmlFile).Elements("ArticleInfo")
                                  select nodes;

                if (!articleInfo == null)
                {
                    foreach (var nd in articleInfo)
                    {
                        lang = nd.Attributes("Language")(0).Value.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                CLog.LogMessages("Error in getLanguage()" + Constants.vbNewLine);
                CLog.LogMessages("ex.message" + ex.Message.ToString());
                throw;
            }

            return lang;
        }

        private void EquationCorrection(string XMLfullFileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "perl.exe";
            proc.StartInfo.Arguments = "\"" + System.Windows.Forms.Application.StartupPath + @"\eqncorrection.pl""" + Strings.Space(1) + XMLfullFileName;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
            while ((proc.HasExited != true))
            {
            }
            proc.Close();
        }

        public string MapItalicEntities(Match m)
        {
            string WDEmp = m.Groups[2].Value;
            string WDFont = m.Groups[3].Value;
            string WDHexName = m.Groups[4].Value.Replace("&#x", "").Replace(";", "");
            // System.Configuration.ConfigurationSettings.AppSettings("EntityPath") '"D:\Suresh\EntWord2Indd\Entities.xml"
            System.Xml.XmlDocument entityDoc = new System.Xml.XmlDocument();
            entityDoc.Load(EntityPathxml);
            System.Xml.XmlNode entities = entityDoc.SelectSingleNode(".//Entity[WFont='" + WDFont + "' and Emp='" + WDEmp + "' and WCC='" + WDHexName + "']");
            try
            {
                if ((Information.IsNothing(entities) == false))
                {
                    string EntityTag = "<Emphasis type=" + "\"" + "c" + entities.ChildNodes[5].InnerText + "\"" + ">";
                    EntityTag += "&#x" + entities.ChildNodes[6].InnerText + ";</Emphasis>";
                    return EntityTag;
                }
            }
            catch (Exception ex)
            {
            }
        }
        public string MapNormalEntities(Match m)
        {
            string WDEmp = "Normal";
            string WDFont = m.Groups[1].Value;
            string WDHexName = m.Groups[2].Value.Replace("&#x", "").Replace(";", "");
            string EntityPath = ""; // System.Configuration.ConfigurationSettings.AppSettings("EntityPath") '"D:\Suresh\EntWord2Indd\Entities.xml"
            System.Xml.XmlDocument entityDoc = new System.Xml.XmlDocument();
            entityDoc.Load(EntityPathxml);
            System.Xml.XmlNode entities = entityDoc.SelectSingleNode(".//Entity[WFont='" + WDFont + "' and Emp='" + WDEmp + "' and WCC='" + WDHexName + "']");
            try
            {
                if ((Information.IsNothing(entities) == false))
                {
                    string EntityTag = "<Emphasis type=" + "\"" + "c" + entities.ChildNodes[5].InnerText + "\"" + ">";
                    EntityTag += "&#x" + entities.ChildNodes[6].InnerText + ";</Emphasis>";
                    return EntityTag;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private string ReplaceEntities(string xmlstr)
        {
            try
            {
                System.IO.FileInfo finfo = new System.IO.FileInfo(Environment.GetCommandLineArgs(0));
                if (System.IO.File.Exists(finfo.DirectoryName + @"\" + "Config.xml"))
                {
                    System.Xml.XmlDocument ConfigDoc = new System.Xml.XmlDocument();
                    ConfigDoc.Load(finfo.DirectoryName + @"\" + "Config.xml");
                    string EntityPath = ConfigDoc.SelectSingleNode(".//Entitysettingsxml").InnerText;
                    System.Xml.XmlDocument entityDoc = new System.Xml.XmlDocument();
                    entityDoc.Load(EntityPath);
                    System.Xml.XmlNodeList entities = entityDoc.SelectNodes(".//Entities/entity");
                    foreach (System.Xml.XmlNode entity in entities)
                    {
                        string hexname = entity.Attributes.ItemOf["hexname"].Value;
                        hexname = "&#x?" + hexname + ";";
                        string FontName = entity.Attributes.ItemOf["font"].Value;
                        xmlstr = Regex.Replace(xmlstr, "(" + hexname + ")", "<cs_text type=\"entity\" font=" + "\"" + FontName + "\"" + ">" + "$1" + "</cs_text>", RegexOptions.Singleline);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return xmlstr;
        }
        private System.Xml.XmlDocument CreateDom(string XmlString)
        {
            System.Xml.XmlDocument Xdoc = new System.Xml.XmlDocument();
            Xdoc.PreserveWhitespace = true;
            try
            {
                Xdoc.LoadXml(XmlString);
            }
            catch (Exception ex)
            {
                CLog.LogMessages("Error when creating DOM Object" + ex.Message);
                CLog.LogMessages("====================================================================================================================================", true);
                CLog.LogMessages(Constants.vbNewLine + "STM Journal Preprocessor  :" + ex.Message.ToString() + Constants.vbNewLine);
                CLog.LogMessages("====================================================================================================================================", true);
                CLog.LogMessages("====================================================================================================================================", true);
                CLog.LogMessages("====================================================================================================================================", true);
                CLog.LogMessages("====================================================================================================================================", true);
                CLog.LogMessages(Constants.vbNewLine + XmlString);
                CLog.LogMessages("====================================================================================================================================", true);
                CLog.LogMessages("====================================================================================================================================", true);
                CLog.LogMessages("====================================================================================================================================", true);
                CLog.LogMessages("====================================================================================================================================", true);
            }


            return Xdoc;
        }
        public void AddAttribute(string XPath, string AttrName, string AttrVal)
        {
            System.Xml.XmlNodeList NdList = XDoc.SelectNodes(XPath, NS);
            string prefix = "";
            string LocalName = "";
            prefix = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups[1].Value;
            if (prefix == "")
                LocalName = AttrName;
            else
                LocalName = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups[2].Value;

            // For all the nodes having the XPATH 
            for (int i = 0; i <= NdList.Count - 1; i++)
            {
                System.Xml.XmlNode Nd = NdList[i];
                bool AttributeExists = false;
                try
                {
                    if ((Information.IsNothing(Nd.Attributes.ItemOf[AttrName]) == false))
                    {
                        System.Xml.XmlAttribute reqAttr = Nd.Attributes.ItemOf[AttrName];
                        reqAttr.Value = AttrVal;
                        AttributeExists = true;
                    }
                }
                catch (Exception ex)
                {
                }

                try
                {
                    if (AttributeExists == false)
                    {
                        System.Xml.XmlAttribute newAttr;
                        if (prefix == "")
                            newAttr = XDoc.CreateAttribute(AttrName);
                        else
                            newAttr = XDoc.CreateAttribute(AttrName, NSht(prefix));
                        newAttr.Value = AttrVal;
                        Nd.Attributes.Append(newAttr);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        public void AddInfoNode_2(ArrayList XmlFragStr)
        {
            System.Xml.XmlProcessingInstruction AddPreProc = null;
            try
            {
                // If IsNothing((XDoc.SelectSingleNode(".//processing-instruction('ADDINFO')"))) Then
                AddPreProc = XDoc.CreateProcessingInstruction("ADDINFO", "");
            }
            // End If
            catch (Exception ex)
            {
            }
            foreach (string str in XmlFragStr)
            {
                try
                {
                    // XDoc.DocumentElement.Name 
                    string DataPart = str;
                    DataPart = ClearDataPart(DataPart);
                    AddPreProc.Data += DataPart;
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                string RootName = XDoc.DocumentElement.Name;
                XmlNode RootNd = XDoc.SelectSingleNode(".//" + RootName);
                RootNd.AppendChild(AddPreProc);
            }
            catch (Exception ex)
            {
            }
        }

        public void AddAttribute(XmlNode XNode, string AttrName, string AttrVal)
        {
            ArrayList NdList = new ArrayList();
            NdList.Add(XNode);

            string prefix = "";
            string LocalName = "";
            prefix = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups[1].Value;
            if (prefix == "")
                LocalName = AttrName;
            else
                LocalName = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups[2].Value;

            // For all the nodes having the XPATH 
            for (int i = 0; i <= NdList.Count - 1; i++)
            {
                System.Xml.XmlNode Nd = NdList(i);
                bool AttributeExists = false;
                try
                {
                    if ((Information.IsNothing(Nd.Attributes.ItemOf[AttrName]) == false))
                    {
                        System.Xml.XmlAttribute reqAttr = Nd.Attributes.ItemOf[AttrName];
                        reqAttr.Value = AttrVal;
                        AttributeExists = true;
                    }
                }
                catch (Exception ex)
                {
                }

                try
                {
                    if (AttributeExists == false)
                    {
                        System.Xml.XmlAttribute newAttr;
                        if (prefix == "")
                            newAttr = XDoc.CreateAttribute(AttrName);
                        else
                            newAttr = XDoc.CreateAttribute(AttrName, NSht(prefix));
                        newAttr.Value = AttrVal;
                        Nd.Attributes.Append(newAttr);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        // 'Public Function AddTextorCharacter(ByVal ToNode As String, ByVal text As String, Optional ByVal before As Boolean = True, Optional ByVal type As String = "")
        // '    Dim XdocString As String = XDoc.InnerXml
        // '    If before Then
        // '        If type.Length = 0 Then
        // '            XdocString = Regex.Replace(XdocString, "<" + ToNode + "\s?>", "<cs:text xmlns:cs=""http://www.crest-premedia.in"">" + text + "</cs:text>" + "<" + ToNode + ">", RegexOptions.Singleline)
        // '        Else
        // '            XdocString = Regex.Replace(XdocString, "<" + ToNode + "\s?>", "<cs:text type=""newline"" xmlns:cs=""http://www.crest-premedia.in"">" + text + "</cs:text>" + "<" + ToNode + ">", RegexOptions.Singleline)
        // '        End If
        // '    Else
        // '        If type.Length = 0 Then
        // '            XdocString = Regex.Replace(XdocString, "</" + ToNode + ">", "</" + ToNode + ">" + "<cs:text xmlns:cs=""http://www.crest-premedia.in"">" + text + "</cs:text>", RegexOptions.Singleline)
        // '        Else
        // '            XdocString = Regex.Replace(XdocString, "</" + ToNode + ">", "</" + ToNode + ">" + "<cs:text type=""newline"" xmlns:cs=""http://www.crest-premedia.in"">" + text + "</cs:text>", RegexOptions.Singleline)
        // '        End If
        // '    End If



        // '    'XdocString = Regex.Replace(XdocString, EnterString + "</(.+?)>" + EnterString, EnterString, RegexOptions.Singleline)
        // '    Try
        // '        XDoc.InnerXml = XdocString
        // '    Catch ex As Exception
        // '    End Try

        // 'End Function

        // 'Public Function RemoveDoubleEnter()

        // '    Dim AllReqNodes As Xml.XmlNodeList = XDoc.SelectNodes(".//cs:text/following-sibling::cs:text", NS)

        // '    'For i As Integer = 0 To AllReqNodes.Count - 1
        // '    '    dim 

        // '    'Next
        // '    '''Dim XdocString As String = XDoc.InnerXml
        // '    ' ''Dim EnterString As String = "<cs:text type=""newline"" xmlns:cs=""http://www.crest-premedia.in"">(.)</cs:text>"
        // '    ' ''Dim DoubleEnter As MatchCollection = Regex.Matches(XdocString, EnterString + "</(.+?)>" + EnterString, RegexOptions.Singleline)

        // '    ' ''Dim meav As New MatchEvaluator(AddressOf eval)
        // '    ' ''XdocString = Regex.Replace(XdocString, EnterString + "</(.+?)>" + EnterString, meav)
        // '    ' ''Try
        // '    ' ''    XDoc.InnerXml = XdocString
        // '    ' ''Catch ex As Exception
        // '    ' ''End Try
        // 'End Function

        // 'Public Function eval(ByVal m As Match) As String
        // '    Dim EnterString As String = "<cs:text type=""newline"" xmlns:cs=""http://www.crest-premedia.in"">(.)</cs:text>"
        // '    If AscW(m.Groups(1).Value) = 13 Then
        // '        Return m.Groups(2).Value + EnterString
        // '    Else
        // '        Return m.Value
        // '    End If

        // 'End Function

        public enum ChildTypes
        {
            AsFirstChild = 1,
            AsLastChild = 2,
            AsPreviousSibling = 3,
            AsNextSibling = 4
        }

        public void AddTextorXml(string ToNodeXpath, string text, ChildTypes Position, bool force = false)
        {
            System.Xml.XmlNodeList AllReqNodes = XDoc.SelectNodes(ToNodeXpath, NS);
            for (int i = 0; i <= AllReqNodes.Count - 1; i++)
            {
                XmlNode ReqNd = AllReqNodes[i];
                switch (Position)
                {
                    case ChildTypes.AsFirstChild:
                        {
                            ReqNd.InnerXml = text + ReqNd.InnerXml;
                            break;
                        }

                    case ChildTypes.AsLastChild:
                        {
                            ReqNd.InnerXml = ReqNd.InnerXml + text;
                            break;
                        }

                    case ChildTypes.AsNextSibling:
                        {
                            try
                            {
                                // Dim newNode As XmlElement = XDoc.CreateElement("cs:text", NSht("cs"))
                                // 'Dim temxDoc As XmlDocument = New XmlDocument
                                // 'temxDoc.LoadXml(text)
                                // 'Dim newNode As Xml.XmlNode = XDoc.ImportNode(temxDoc.FirstChild, True)

                                // 'If ReqNd.InnerText.EndsWith(vbNewLine) = False Or force = True Then
                                // '    ReqNd.ParentNode.InsertAfter(newNode, ReqNd)

                                // 'End If

                                XmlElement newNode = XDoc.CreateElement("root");
                                newNode.InnerXml = text;

                                if (ReqNd.InnerText.EndsWith(Constants.vbNewLine) == false | force == true)
                                {
                                    while ((newNode.ChildNodes.Count > 0))
                                        ReqNd.ParentNode.InsertAfter(newNode.LastChild, ReqNd);
                                }
                            }
                            catch (Exception ex)
                            {
                            }

                            break;
                        }

                    case ChildTypes.AsPreviousSibling:
                        {
                            // Dim newNode As XmlElement = XDoc.CreateElement("cs:text", NSht("cs"))
                            XmlElement newNode = XDoc.CreateElement("root");
                            newNode.InnerXml = text;

                            try
                            {
                                if (Information.IsNothing(ReqNd.PreviousSibling) == false)
                                {
                                    while ((newNode.ChildNodes.Count > 0))
                                        ReqNd.ParentNode.InsertBefore(newNode.FirstChild, ReqNd);
                                }
                                else
                                    while ((newNode.ChildNodes.Count > 0))
                                        ReqNd.ParentNode.InsertBefore(newNode.FirstChild, ReqNd);
                            }
                            catch (Exception ex)
                            {
                            }

                            break;
                        }
                }
            }
        }

        public void DeleteAttr(string Xpath, string AttrName)
        {
            System.Xml.XmlNodeList NdList = XDoc.SelectNodes(Xpath, NS);
            string prefix = "";
            string LocalName = "";
            prefix = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups[1].Value;
            if (prefix == "")
                LocalName = AttrName;
            else
                LocalName = Regex.Match(AttrName, "^(.+?):(.+?)$", RegexOptions.Singleline).Groups[2].Value;

            System.Xml.XmlNodeList AllReqNodes = XDoc.SelectNodes(Xpath, NS);
            for (int i = 0; i <= AllReqNodes.Count - 1; i++)
            {
                XmlNode ReqNd = AllReqNodes[i];
                try
                {
                    ReqNd.Attributes.Remove(ReqNd.Attributes.ItemOf[AttrName]);
                }
                catch (Exception ex)
                {
                }
            }
        }
        public void HideLineNode(XmlNode xPath)
        {
            foreach (XmlNode nd in xPath)
            {
                try
                {
                    System.Xml.XmlProcessingInstruction HideXmlPreProc = XDoc.CreateProcessingInstruction("HIDE", "");
                    string DataPart = nd.OuterXml;
                    DataPart = ClearDataPart(DataPart);
                    HideXmlPreProc.Data = DataPart;
                    nd.ParentNode.ReplaceChild(HideXmlPreProc, nd);
                }
                catch (Exception ex)
                {
                }
            }
        }
        public void AddInfoNode(ArrayList XmlFragStr)
        {
            System.Xml.XmlProcessingInstruction AddPreProc = null;
            try
            {
                if (Information.IsNothing((XDoc.SelectSingleNode(".//processing-instruction('ADDHDINFO')"))))
                    AddPreProc = XDoc.CreateProcessingInstruction("ADDHDINFO", "");
            }
            catch (Exception ex)
            {
            }
            foreach (string str in XmlFragStr)
            {
                try
                {
                    // XDoc.DocumentElement.Name 
                    string DataPart = str;
                    DataPart = ClearDataPart(DataPart);
                    AddPreProc.Data += DataPart;
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                string RootName = XDoc.DocumentElement.Name;
                XmlNode RootNd = XDoc.SelectSingleNode(".//" + RootName);
                RootNd.AppendChild(AddPreProc);
            }
            catch (Exception ex)
            {
            }
        }

        public void HideNode(ArrayList XpathList)
        {
            foreach (string XPathStr in XpathList)
            {
                System.Xml.XmlNodeList Nodes = XDoc.SelectNodes(XPathStr, NS);
                foreach (XmlNode nd in Nodes)
                {
                    try
                    {
                        System.Xml.XmlProcessingInstruction HideXmlPreProc = XDoc.CreateProcessingInstruction("HIDE", "");
                        string DataPart = nd.OuterXml;
                        DataPart = ClearDataPart(DataPart);
                        HideXmlPreProc.Data = DataPart;
                        nd.ParentNode.ReplaceChild(HideXmlPreProc, nd);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public void DeleteNode(string DelXpath)
        {
            System.Xml.XmlNodeList Nodes = XDoc.SelectNodes(DelXpath, NS);

            for (int i = 0; i <= Nodes.Count - 1; i++)
            {
                try
                {
                    DeleteNode(Nodes[i]);
                }
                catch (Exception ex)
                {
                }
            }
        }
        public void DeleteNode(System.Xml.XmlNode nd)
        {
            if (Information.IsNothing(nd) == false)
                nd.ParentNode.RemoveChild(nd);
        }

        private string ClearDataPart(string datapart)
        {
            return Regex.Replace(datapart, @"<\?([^<>]+?)\?>", "<cs_preproc>$1</cd_preproc>", RegexOptions.Singleline);
        }


        // Public Sub Reposition(ByVal SourceNodeXpath As String, ByVal DestinationPlaceXpath As String, ByVal withRelation As RelationShip)
        // Dim SrcNode As Xml.XmlNode = XDoc.SelectSingleNode(SourceNodeXpath, NS)

        // Dim DestiPlaceNode As Xml.XmlNode = XDoc.SelectSingleNode(DestinationPlaceXpath, NS)

        // If SrcNode Is Nothing Or DestiPlaceNode Is Nothing Then
        // Exit Sub
        // End If

        // Dim RepositionNode As Xml.XmlNode = XDoc.CreateElement("cs:reposition", NSht("cs"))
        // Select Case withRelation
        // Case RelationShip.FirstChild
        // If DestiPlaceNode.ChildNodes.Count > 0 Then
        // Dim firstc As Xml.XmlNode = DestiPlaceNode.ChildNodes.Item(0)
        // DestiPlaceNode.InsertBefore(RepositionNode, firstc)
        // Else
        // DestiPlaceNode.AppendChild(RepositionNode)
        // End If
        // Case RelationShip.LastChild
        // DestiPlaceNode.AppendChild(RepositionNode)
        // Case RelationShip.NextSibling
        // DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode)
        // Case RelationShip.PreviousSibling
        // DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode)
        // End Select


        // RepositionNode.InnerXml = SrcNode.OuterXml

        // Dim RepoId As Xml.XmlAttribute = XDoc.CreateAttribute("cs:repoID", NSht("cs"))
        // RepositionNode.Attributes.Append(RepoId)
        // RepoId.Value = clsReposition.GetRepositionID
        // RepositionHide(SrcNode, RepoId.Value)

        // End Sub
        public void FrameMove(System.Xml.XmlNode SourceNodeXpath, System.Xml.XmlNode DestinationPlaceXpath, RelationShip withRelation)
        {
            System.Xml.XmlNode SrcNode = SourceNodeXpath;

            System.Xml.XmlNode DestiPlaceNode = DestinationPlaceXpath;

            if (SrcNode == null | DestiPlaceNode == null)
                return;

            try
            {
                System.Xml.XmlAttribute frameatt = XDoc.CreateAttribute("cs_moved");
                frameatt.Value = "1";
                SrcNode.Attributes.Append(frameatt);
            }
            catch (Exception ex)
            {
            }

            System.Xml.XmlNode RepositionNode = XDoc.CreateElement("cs:reposition", NSht("cs"));
            switch (withRelation)
            {
                case RelationShip.FirstChild:
                    {
                        if (DestiPlaceNode.ChildNodes.Count > 0)
                        {
                            System.Xml.XmlNode firstc = DestiPlaceNode.ChildNodes.Item(0);
                            DestiPlaceNode.InsertBefore(RepositionNode, firstc);
                        }
                        else
                            DestiPlaceNode.AppendChild(RepositionNode);
                        break;
                    }

                case RelationShip.LastChild:
                    {
                        DestiPlaceNode.AppendChild(RepositionNode);
                        break;
                    }

                case RelationShip.NextSibling:
                    {
                        DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode);
                        break;
                    }

                case RelationShip.PreviousSibling:
                    {
                        DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode);
                        break;
                    }
            }


            RepositionNode.InnerXml = SrcNode.OuterXml;

            System.Xml.XmlAttribute RepoId = XDoc.CreateAttribute("cs:repoID", NSht("cs"));
            RepositionNode.Attributes.Append(RepoId);
            RepoId.Value = clsReposition.GetRepositionID;

            System.Xml.XmlAttribute MovId = XDoc.CreateAttribute("cs_moved");
            RepositionNode.Attributes.Append(MovId);
            MovId.Value = "1";



            RepositionHide(SrcNode, RepoId.Value);
        }
        public void FootnoteReposition(System.Xml.XmlNode SourceNodeXpath, System.Xml.XmlNode DestinationPlaceXpath, RelationShip withRelation, string typename = "")
        {
            System.Xml.XmlNode SrcNode = SourceNodeXpath;

            System.Xml.XmlNode DestiPlaceNode = DestinationPlaceXpath;

            if (SrcNode == null | DestiPlaceNode == null)
                return;

            System.Xml.XmlNode RepositionNode = XDoc.CreateElement("cs:footnote", NSht("cs"));
            System.Xml.XmlAttribute attr = XDoc.CreateAttribute("cs_type");
            attr.Value = typename;
            RepositionNode.Attributes.Append(attr);
            switch (withRelation)
            {
                case RelationShip.FirstChild:
                    {
                        if (DestiPlaceNode.ChildNodes.Count > 0)
                        {
                            System.Xml.XmlNode firstc = DestiPlaceNode.ChildNodes.Item(0);
                            DestiPlaceNode.InsertBefore(RepositionNode, firstc);
                        }
                        else
                            DestiPlaceNode.AppendChild(RepositionNode);
                        break;
                    }

                case RelationShip.LastChild:
                    {
                        DestiPlaceNode.AppendChild(RepositionNode);
                        break;
                    }

                case RelationShip.NextSibling:
                    {
                        DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode);
                        break;
                    }

                case RelationShip.PreviousSibling:
                    {
                        DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode);
                        break;
                    }
            }


            RepositionNode.InnerXml = SrcNode.OuterXml;

            System.Xml.XmlAttribute RepoId = XDoc.CreateAttribute("cs:repoID", NSht("cs"));
            RepositionNode.Attributes.Append(RepoId);
            RepoId.Value = clsReposition.GetRepositionID;

            // Dim fType As Xml.XmlAttribute = XDoc.CreateAttribute("cs_type")
            // RepositionNode.Attributes.Append(fType)
            // fType.Value = "footnote"


            FootnoteRepositionHide(SrcNode, RepoId.Value, typename);
        }

        private void FootnoteRepositionHide(XmlNode SrcNode, string withRepoID, string typeName = "")
        {
            // Dim RepoHide As Xml.XmlProcessingInstruction = XDoc.CreateProcessingInstruction("cs_RepositionHide", "<cs:repoID=" + withRepoID + "/>")
            System.Xml.XmlNode RepoHide = XDoc.CreateElement("cs_footnote");
            XmlAttribute attr = XDoc.CreateAttribute("repoID");
            attr.Value = withRepoID;
            RepoHide.Attributes.Append(attr);
            XmlAttribute typeNameAttr = XDoc.CreateAttribute("typename");
            typeNameAttr.Value = typeName;
            RepoHide.Attributes.Append(typeNameAttr);
            if ((typeName.ToLower() == "endnote"))
                RepoHide.InnerXml = "<cs_text type=\"superscript\">" + SrcNode.SelectSingleNode(".//cs_text[@type='footnotenumber']").InnerText + "</cs_text>" + "<cs_text forced=\"true\">" + "</cs_text>";
            else
                RepoHide.InnerXml = "<cs_text forced=\"true\">" + "</cs_text>";
            // Dim datapart As String = SrcNode.OuterXml
            // datapart = ClearDataPart(datapart)
            // RepoHide.Data += datapart

            try
            {
                SrcNode.ParentNode.ReplaceChild(RepoHide, SrcNode);
            }
            catch (Exception ex)
            {
            }
        }



        public void FrameReposition(System.Xml.XmlNode SourceNodeXpath, System.Xml.XmlNode DestinationPlaceXpath, RelationShip withRelation, string typename = "")
        {
            System.Xml.XmlNode SrcNode = SourceNodeXpath;

            System.Xml.XmlNode DestiPlaceNode = DestinationPlaceXpath;

            if (SrcNode == null | DestiPlaceNode == null)
                return;

            System.Xml.XmlNode RepositionNode = XDoc.CreateElement("cs:frame", NSht("cs"));
            switch (withRelation)
            {
                case RelationShip.FirstChild:
                    {
                        if (DestiPlaceNode.ChildNodes.Count > 0)
                        {
                            System.Xml.XmlNode firstc = DestiPlaceNode.ChildNodes.Item(0);
                            DestiPlaceNode.InsertBefore(RepositionNode, firstc);
                        }
                        else
                            DestiPlaceNode.AppendChild(RepositionNode);
                        break;
                    }

                case RelationShip.LastChild:
                    {
                        DestiPlaceNode.AppendChild(RepositionNode);
                        break;
                    }

                case RelationShip.NextSibling:
                    {
                        DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode);
                        break;
                    }

                case RelationShip.PreviousSibling:
                    {
                        DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode);
                        break;
                    }
            }


            RepositionNode.InnerXml = SrcNode.OuterXml;

            System.Xml.XmlAttribute RepoId = XDoc.CreateAttribute("cs:repoID", NSht("cs"));
            RepositionNode.Attributes.Append(RepoId);
            RepoId.Value = clsReposition.GetRepositionID;
            FrameRepositionHide(SrcNode, RepoId.Value, typename);
        }
        private void FrameRepositionHide(XmlNode SrcNode, string withRepoID, string typeName = "")
        {
            // Dim RepoHide As Xml.XmlProcessingInstruction = XDoc.CreateProcessingInstruction("cs_RepositionHide", "<cs:repoID=" + withRepoID + "/>")
            System.Xml.XmlNode RepoHide = XDoc.CreateElement("cs_frame");
            XmlAttribute attr = XDoc.CreateAttribute("repoID");
            attr.Value = withRepoID;
            RepoHide.Attributes.Append(attr);
            XmlAttribute typeNameAttr = XDoc.CreateAttribute("typename");
            typeNameAttr.Value = typeName;
            RepoHide.Attributes.Append(typeNameAttr);
            RepoHide.InnerXml = "<cs_text forced=\"true\">" + "</cs_text>";
            // Dim datapart As String = SrcNode.OuterXml
            // datapart = ClearDataPart(datapart)
            // RepoHide.Data += datapart

            try
            {
                SrcNode.ParentNode.ReplaceChild(RepoHide, SrcNode);
            }
            catch (Exception ex)
            {
            }
        }
        public void Reposition(System.Xml.XmlNode SourceNodeXpath, System.Xml.XmlNode DestinationPlaceXpath, RelationShip withRelation, string typename = "", string TypeID = "")
        {
            System.Xml.XmlNode SrcNode = SourceNodeXpath;
            bool isInlineTable = false;
            System.Xml.XmlNode DestiPlaceNode = DestinationPlaceXpath;

            if (SrcNode == null | DestiPlaceNode == null)
                return;
            try
            {
                if ((SrcNode.Name.ToLower() == "cs:table" & SrcNode.Attributes["Float"].Value.ToString().ToLower() == "no"))
                    isInlineTable = true;
                else
                    isInlineTable = false;
            }
            catch (Exception ex)
            {
                isInlineTable = false;
            }
            System.Xml.XmlNode RepositionNode = XDoc.CreateElement("cs:reposition", NSht("cs"));
            if ((isInlineTable == false))
            {
                switch (withRelation)
                {
                    case RelationShip.FirstChild:
                        {
                            if (DestiPlaceNode.ChildNodes.Count > 0)
                            {
                                System.Xml.XmlNode firstc = DestiPlaceNode.ChildNodes.Item(0);
                                DestiPlaceNode.InsertBefore(RepositionNode, firstc);
                            }
                            else
                                DestiPlaceNode.AppendChild(RepositionNode);
                            break;
                        }

                    case RelationShip.LastChild:
                        {
                            DestiPlaceNode.AppendChild(RepositionNode);
                            break;
                        }

                    case RelationShip.NextSibling:
                        {
                            DestiPlaceNode.ParentNode.InsertAfter(RepositionNode, DestiPlaceNode);
                            break;
                        }

                    case RelationShip.PreviousSibling:
                        {
                            DestiPlaceNode.ParentNode.InsertBefore(RepositionNode, DestiPlaceNode);
                            break;
                        }
                }
            }
            else
            {
                SrcNode.ParentNode.InsertAfter(RepositionNode, SrcNode);
                System.Xml.XmlAttribute pStyle = XDoc.CreateAttribute("cs_IsInlineTable");
                pStyle.Value = "Yes";
                RepositionNode.Attributes.Append(pStyle);
            }



            RepositionNode.InnerXml = SrcNode.OuterXml;

            // Dim RepoId As Xml.XmlAttribute = XDoc.CreateAttribute("cs:repoID", NSht("cs"))
            // RepositionNode.Attributes.Append(RepoId)
            // RepoId.Value = clsReposition.GetRepositionID
            // RepositionHide(SrcNode, RepoId.Value, typename)
            System.Xml.XmlAttribute RepoId = XDoc.CreateAttribute("cs:repoID", NSht("cs"));
            RepositionNode.Attributes.Append(RepoId);
            RepoId.Value = clsReposition.GetRepositionID;
            System.Xml.XmlAttribute cs_ID = XDoc.CreateAttribute("cs_ID");
            cs_ID.Value = TypeID;
            RepositionNode.Attributes.Append(cs_ID);
            RepositionHide(SrcNode, RepoId.Value, typename, TypeID);
        }
        private void RepositionHide(XmlNode SrcNode, string withRepoID, string typeName = "", string TypeIdVal = "")
        {
            // Dim RepoHide As Xml.XmlProcessingInstruction = XDoc.CreateProcessingInstruction("cs_RepositionHide", "<cs:repoID=" + withRepoID + "/>")
            System.Xml.XmlNode RepoHide = XDoc.CreateElement("cs_repos");
            XmlAttribute attr = XDoc.CreateAttribute("repoID");
            bool IsInline = false;
            try
            {
                if ((SrcNode.Name.ToLower() == "figure" & SrcNode.Attributes["Float"].Value.ToLower() == "no"))
                    IsInline = true;
                else
                    IsInline = false;
            }
            catch (Exception ex)
            {
                IsInline = false;
            }


            attr.Value = withRepoID;
            RepoHide.Attributes.Append(attr);
            XmlAttribute typeNameAttr = XDoc.CreateAttribute("typename");
            typeNameAttr.Value = typeName;
            RepoHide.Attributes.Append(typeNameAttr);

            System.Xml.XmlAttribute cs_ID = XDoc.CreateAttribute("cs_ID");
            cs_ID.Value = TypeIdVal;
            RepoHide.Attributes.Append(cs_ID);

            RepoHide.InnerXml = "<cs_text type=\"" + TypeIdVal + "\">" + "</cs_text>";

            // RepoHide.InnerXml = "<cs_text forced=""true"">" + "</cs_text>"
            // Dim datapart As String = SrcNode.OuterXml
            // datapart = ClearDataPart(datapart)
            // RepoHide.Data += datapart

            try
            {
                SrcNode.ParentNode.ReplaceChild(RepoHide, SrcNode);
                if ((IsInline))
                {
                    System.Xml.XmlAttribute inline = XDoc.CreateAttribute("cs_IsInline");
                    inline.Value = "Yes";
                    RepoHide.Attributes.Append(inline);
                    XmlElement EnterMark = XDoc.CreateElement("cs_text");
                    XmlAttribute typeA = XDoc.CreateAttribute("type");
                    typeA.Value = "EnterBeforeInlineFig";
                    EnterMark.Attributes.Append(typeA);
                    EnterMark.InnerXml = Constants.vbNewLine;
                    RepoHide.ParentNode.InsertAfter(EnterMark, RepoHide);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

    public enum RelationShip
    {
        PreviousSibling = 1,
        NextSibling = 2,
        FirstChild = 3,
        LastChild = 4
    }
}