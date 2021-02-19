using Acdc.JournalPreprocessor;
using Acdc.Preprocessor.CMS.Communicator;
using Acdc.Preprocessor.Logging;
using Newtonsoft.Json.Linq;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Acdc.Preprocessor.Core
{
    public class PreprocessorHelper
    {
        static CmsService _cmsService = new CmsService();
        protected PreprocessorHelper()
        { }

        public static (bool result, XmlAndFileName editedXml, XmlAndFileName jobsheetXml, string requestID) ValidateAndAssignBasicValue(JObject brokerMessage)
        {
            bool result = true;
            string editedXmlPath = string.Empty;
            XmlAndFileName editedXml = null;
            string jobsheetXmlPath = string.Empty;
            XmlAndFileName jobsheetXml = null;



            string requestID = BrokerMessageHelper.GetRequestId(brokerMessage);
            if (string.IsNullOrEmpty(requestID))
            {
                WriteErrorLog(brokerMessage, "Request ID field is missing in broker message.", "", Constants.invalidJson);
                return (false, editedXml, jobsheetXml, requestID);
            }


            jobsheetXmlPath = GetLinkFromBrokerMessage(brokerMessage, "jobsheet_template_s200", "GET");

            if (string.IsNullOrEmpty(jobsheetXmlPath))
            {
                WriteErrorLog(brokerMessage, "jobsheetXmlPath GET link is missing in broker message.", "", Constants.invalidJson);
                return (false, editedXml, jobsheetXml, requestID);
            }

            jobsheetXml = GetXmlAndFileName(brokerMessage, jobsheetXmlPath, Constants.StructureArticle);

            editedXmlPath = GetLinkFromBrokerMessage(brokerMessage, "edited_xml", "GET");

            if (string.IsNullOrEmpty(editedXmlPath))
            {
                WriteErrorLog(brokerMessage, "jobsheetXmlPath GET link is missing in broker message.", "", Constants.invalidJson);
                return (false, editedXml, jobsheetXml, requestID);
            }

            editedXml = GetXmlAndFileName(brokerMessage, editedXmlPath, Constants.StructureArticle);

            return (true, editedXml, jobsheetXml, requestID);
        }

        public static bool CopyImages(string folderPath, JObject brokerMessage)
        {
            bool isDownloadImages = false;
            isDownloadImages = GetImageLinkFromBrokerMessage(brokerMessage, "images_s200", "GET", folderPath);
            return isDownloadImages;

        }

        public static bool CreatePGXml(string inputxmlPath, string jobSheetXmlPath, string folderPath,JObject brokerMessage)
        {
            bool isPGXml = false;
            string finalString = string.Empty;

            XDocument jXDoc = XDocument.Load(jobSheetXmlPath);
            XDocument inputXml = XDocument.Load(inputxmlPath);

            

            (isPGXml, finalString)= GetLayoutPGInfo(finalString, jXDoc,brokerMessage);

            if (isPGXml)
            {
                isPGXml = PreprocessorHelper.SavePGXml(folderPath, finalString, Path.GetFileNameWithoutExtension(inputxmlPath));
            }


            return isPGXml;
        }

        private static (bool isPGXml, string finalString) GetLayoutPGInfo(string finalString, XDocument jXDoc,JObject brokerMessage)
        {
            bool isPGXml = true;

            XDocument temDoc = new XDocument();
            temDoc = XDocument.Parse(FileOperations.GetFile(ConfigFilePaths.JournalTemplateDetails).ToString());

            // < JournalID templatename = "Large_Template" column = "2 Column" prefix = "2-GL_11612" publisher = "Springer-Verlag" > 00061 </ JournalID >

            var journalNode = temDoc.Descendants("JournalID").FirstOrDefault(s => s.Value.ToLower().ToString().Equals(FormatJournalID(jXDoc).ToString()));
            if (journalNode == null)
            {
                isPGXml = false;
                LoggerCF.GetInstance().LogError("Error: journal ID info is not available in temmplate detail xml.");
            }


            if (jXDoc.Descendants("Typesetting").FirstOrDefault() != null && jXDoc.Descendants("Typesetting").FirstOrDefault().Attributes("Layout").Any())
            {
                var LayoutType = jXDoc.Descendants("Typesetting").FirstOrDefault().Attribute("Layout").Value;
                var layout = GetLayout(LayoutType);

                if (layout.ToString() != "")
                    finalString += "<Style Mandatory='Yes'>" + layout + "</Style>" + "\n";
                else
                {
                    isPGXml = false;
                    LoggerCF.GetInstance().LogError("Error: Mandatory Field Layout Name is not updated in configuration");
                    throw new PreprocessorException("Layout(Large/Medium/Smallcondensed/Smallextended) information is mandatory for PG xml Creating.Layout information is missing in jobsheet.");

                }
                finalString += "<HardDrivePath/>" + Environment.NewLine + "<UserHomeDrivePath/>" + Environment.NewLine + "<LogFileName/>";


                string publishetName = GetPublisherNameFromJobsheet(jXDoc);
                if(!string.IsNullOrEmpty(publishetName))
                {
                    finalString += "<Publisher>" + publishetName + "</Publisher>";
                }
                else
                {
                    finalString += "<Publisher>/Publisher>";
                }

                
                if (jXDoc.Descendants("JournalID").Any())
                    finalString += "<JournalId>" + FormatJournalID(jXDoc) + "</JournalId>";

                if (jXDoc.Descendants("ArticleId").Any())
                    finalString += jXDoc.Descendants("ArticleId").FirstOrDefault().Value;

                if (jXDoc.Descendants("ArticleCategory").Any())
                    finalString += "<ArticleCategory Mandatory='Yes'>" + jXDoc.Descendants("ArticleCategory").FirstOrDefault().Value + "</ArticleCategory>";
                else
                    finalString += "<ArticleCategory Mandatory='Yes'>Original Article</ArticleCategory>";

                if (journalNode.Attribute("column") != null)
                {
                    
                    finalString += "<Columns>" + journalNode.Attribute("column").Value + "</Columns>";
                }
                else
                {
                    finalString += "<Columns>/Columns>";
                }

                if (journalNode.Attribute("prefix") != null)
                {
                     finalString += "<Prefix Mandatory='Yes'>" + journalNode.Attribute("prefix").Value + "</Prefix>";
                }
                else
                {
                    finalString += "<Prefix Mandatory='Yes'>/Prefix>";
                }

                if (journalNode.Attribute("templatename") != null)
                {
                    finalString += "<TemplateName Mandatory='Yes'>" + journalNode.Attribute("templatename").Value + "</TemplateName>";
                }
                else
                {
                    finalString += "<TemplateName Mandatory='Yes'>/TemplateName>";
                }
                var workflow = BrokerMessageHelper.GetWorkflow(brokerMessage);
                if(!string.IsNullOrEmpty(workflow))
                {
                    finalString += "<StageiD>"+GetStageInfo(workflow) +"</StageiD>";
                }
                else
                {
                    finalString += "<StageiD>200</StageiD>";
                }

            }

            return (isPGXml, finalString);
        }

        private static string GetPublisherNameFromJobsheet(XDocument jXDoc)
        {
            string publisherName = string.Empty;
            if (jXDoc.Descendants("PublisherInfo").Any())
            {
                XElement publisherInfo = jXDoc.Descendants("PublisherInfo").FirstOrDefault();
                if (publisherInfo.Descendants("PublisherName").Any())
                {
                    publisherName = publisherInfo.Descendants("PublisherName").FirstOrDefault().Value;

                }

            }

            return publisherName;
        }

        private static string GetStageInfo(string workflow)
        {
            if (workflow.ToLower().Equals("acceptance-s200"))
                return "200";
            else
                return "300";
        }

        private static string FormatJournalID(XDocument jXDoc)
        {
            string formattedID = string.Empty;
            string journalID = jXDoc.Descendants("JournalID").FirstOrDefault().Value;
            if (journalID.Length == 2)
                formattedID = "000" + journalID;
            if (journalID.Length == 3)
                formattedID = "00" + journalID;
            if (journalID.Length == 4)
                formattedID = "0" + journalID;
            if (journalID.Length == 5)
                formattedID = journalID;


            return formattedID;
        }

        private static object GetLayout(string layoutType)
        {
            string LayoutnameDB = string.Empty;
            XDocument temDoc = new XDocument();
            temDoc = XDocument.Parse(FileOperations.GetFile(ConfigFilePaths.conFigPath).ToString());

            

            if (layoutType.ToLower() == "medium")
                LayoutnameDB = temDoc.Descendants("TypeSettingMediumLayout").FirstOrDefault().Value;

            if (layoutType.ToLower() == "smallextended")
                LayoutnameDB = temDoc.Descendants("TypeSettingSmallextendedLayout").FirstOrDefault().Value;

            if (layoutType.ToLower() == "large")
                LayoutnameDB = temDoc.Descendants("TypeSettingLargeLayout").FirstOrDefault().Value;

            if (layoutType.ToLower() == "smallcondensed")
                LayoutnameDB = temDoc.Descendants("TypeSettingSmallcondensedLayout").FirstOrDefault().Value;

            return LayoutnameDB;

        }

        public static string CopyJobsheetXml(string folderPath, XmlAndFileName jobsheetXml)
        {
            string filePath = Path.Combine(folderPath, jobsheetXml.FileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            jobsheetXml.FileXml.Save(filePath);
            return filePath;
        }

        public static string SetOutputXmlPath(string folderPath, XmlAndFileName editedXml)
        {
            return Path.Combine(folderPath, "OUTXML", editedXml.FileName);

        }

        public static string CopyEditedXml(string folderPath, XmlAndFileName editedXml)
        {
            string filePath = Path.Combine(folderPath, "InputXml", editedXml.FileName);
            editedXml.FileXml.Save(filePath);
            return filePath;
        }
        public static bool SavePGXml(string folderPath, string pgxmlString, string fileName)
        {
            bool isSaved = false;
            string filePath = Path.Combine(folderPath, fileName + "_PG.xml");
            StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
            sw.Write("<Zapprocess>" + pgxmlString + "</Zapprocess>");
            sw.Flush();
            sw.Close();
            isSaved = true;

            return isSaved;

        }

        public static string CreateFolderStructure(string workingFolder,string reuestID)
        {
            string folderPath = Path.Combine(workingFolder,reuestID);
            try
            {
                if (Directory.Exists(folderPath))
                    CleanFolder(folderPath);

                CreateFolder(folderPath);

                return folderPath;
            }
            catch (Exception e)
            {
                throw new PreprocessorException("Unable to created reuired folder structure:: "+ folderPath+". "+e.Message.ToString());
            }


        }

        private static void CreateFolder(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
            Directory.CreateDirectory(folderPath + @"\Pagination");
            Directory.CreateDirectory(folderPath + @"\Graphics");
            Directory.CreateDirectory(folderPath + @"\Graphics\WEB");
            Directory.CreateDirectory(folderPath + @"\Graphics\PRINT");
            Directory.CreateDirectory(folderPath + @"\Graphics\Equation");
            Directory.CreateDirectory(folderPath + @"\PDF");
            Directory.CreateDirectory(folderPath + @"\OUTXML");
            Directory.CreateDirectory(folderPath + @"\InputXml");
        }

        private static void CleanFolder(string folderPath)
        {
            string[] innerFiles = System.IO.Directory.GetFiles(folderPath);

            foreach (string file in innerFiles)
            {
                System.IO.File.Delete(file);
            }
            string[] InnerDirs = System.IO.Directory.GetDirectories(folderPath);

            foreach (var subfolder in InnerDirs)
            {
                CleanFolder(subfolder);
                //Directory.Delete(subfolder);
            }
            Directory.Delete(folderPath);
        }


        public static string GetLink(CmsRouteResource resources, string rel, string method)
        {
            try
            {
                return resources._links.FirstOrDefault(l => l.rel.Equals(rel) && l.method.Equals(method)).href;
            }
            catch
            {
                return null;
            }
        }

        public static XmlAndFileName GetXmlAndFileName(JObject brokerMessage, string filePath,
    string relation, bool throwException = true)
        {


            StreamAndFileName structureArticleXmlLink = GetStructureArticleXmlStream(brokerMessage, filePath, throwException);

            if (structureArticleXmlLink == null && throwException)
            {
                LoggerCF.GetInstance().LogError(relation + " xml is not found in structure article link.");
            }
            if (structureArticleXmlLink == null)
                return null;

            XDocument structuredXml = ParseXml(brokerMessage, structureArticleXmlLink.FileStream);



            if (structuredXml == null && throwException)
                throw new PreprocessorException("Xml is null");
            if (structuredXml == null)
                return null;

            return
              new XmlAndFileName
              {
                  FileName = structureArticleXmlLink.FileName,
                  FileXml = structuredXml
              };
        }
        private static XDocument ParseXml(JObject brokerMessage, Stream XmlData)
        {
            XDocument xDocument = null;

            try
            {
                xDocument = XDocument.Load(XmlData, LoadOptions.PreserveWhitespace);
            }
            catch (Exception ex)
            {
                var ravenClient = new RavenClient(GlobalAppSetting.appsetting.ACDC_SENTRY_CLIENT_KEY);
                ravenClient.Capture(new SentryEvent(ex));
                BrokerMessageHelper.SetError(brokerMessage, "Error in value added service, not able to parse xml: " + ex.Message,
                  ex.StackTrace, GlobalAppSetting.appsetting.ACDC_PREPROCESSOR_APP_NAME);
                LoggerCF.GetInstance().LogError($"Error in value added service, not able to parse xml: {ex.Message}",
                  brokerMessage);
            }

            return xDocument;
        }
        public static StreamAndFileName GetStructureArticleXmlStream(JObject brokerMessage, string structureArticleLink, bool throwException = true)
        {
            StreamAndFileName structureArticleXml = _cmsService.GetXml(structureArticleLink, throwException);

            if (structureArticleXml == null && throwException)
            {
                BrokerMessageHelper.SetError(brokerMessage, "structure article xml not exist. ", string.Empty,
                  GlobalAppSetting.appsetting.ACDC_PREPROCESSOR_APP_NAME);
                return null;
            }

            return structureArticleXml;
        }

        public static string GetLinkFromBrokerMessage(JObject brokerMessage, string documentName, string method, bool throwException = true)
        {
            string articleLinks = BrokerMessageHelper.GetDocumentLinkFromBrokerMessage(brokerMessage, documentName, method);
            if (articleLinks == string.Empty && throwException)
            {
                LoggerCF.GetInstance().LogError("Link is missing in broker Message for Rel:: " + documentName + " and Method:: " + method + "");
                return null;
            }
            return articleLinks;
        }

        public static bool GetImageLinkFromBrokerMessage(JObject brokerMessage, string documentName, string method, string folderPath, bool throwException = true)
        {
            bool result = false;
            List<string> articleLinks = new List<string>();
            articleLinks=BrokerMessageHelper.GetImagesLinkFromBrokerMessage(brokerMessage, documentName, method);
            if (articleLinks.Count>0)
            {
                DownloadImages(articleLinks, folderPath);
                result = true;
            }
            return result;
        }

        private static void DownloadImages(List<string> articleLinks, string folderPath)
        {
            folderPath += @"\Graphics\WEB\";
            foreach (var link in articleLinks)
            {
                StreamAndFileName structureArticleXml = _cmsService.GetXml(link, true);
                try
                {
                    Image image = Image.FromStream(structureArticleXml.FileStream);
                    image.Save(folderPath+ structureArticleXml.FileName);
                }
                catch (Exception ex)
                {

                    
                }
            }
        }

        public static string GetStructureArticleResources(JObject brokerMessage, string documentName, string linkRelation)
        {
            string articleLinksFromBrokerMessage = GetLinkFromBrokerMessage(brokerMessage, documentName, "GET");

            //LoggerCF.GetInstance().LogInfo("Getting structure article resources from " + articleLinksFromBrokerMessage);

            //var cmsRouteResource = _cmsService.GetCmsRouteResource(brokerMessage, articleLinksFromBrokerMessage);

            //LoggerCF.GetInstance().LogInfo("Successfully get structure article resources " + articleLinksFromBrokerMessage);
            //if (cmsRouteResource == null)
            //{
            //    return null;
            //}
            //return GetStructureArticleLink(brokerMessage, cmsRouteResource, linkRelation);
            return articleLinksFromBrokerMessage;
        }

        public static void WriteErrorLog(JObject brokerMessage, string exception, string stackTrace, string errorCode)
        {
            LoggerCF.GetInstance().LogError(exception);
            BrokerMessageHelper.SetError(brokerMessage, exception, stackTrace, GlobalAppSetting.appsetting.ACDC_PREPROCESSOR_APP_NAME);
            AuditLogHelper.alert_message.Add(new AlertMessage { code = errorCode, description = exception, elementref = "" });
        }
    }
}
