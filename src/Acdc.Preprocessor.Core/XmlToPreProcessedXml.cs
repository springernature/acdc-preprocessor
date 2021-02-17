using Acdc.JournalPreprocessor;
using Acdc.Preprocessor.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Acdc.Preprocessor.Core
{
  public  class XmlToPreProcessedXml
    {
        public XmlToPreProcessedXml()
        { }
        public (bool,string) Process(JObject brokerMessage)
        {
            bool result = false;
            XmlAndFileName editedXml = null;
            XmlAndFileName jobsheetXml = null;
            string reuestID = string.Empty;
            string workingFolder = @"\\SWINRONEPRD0061\Work";//"s:\Breeze-Pagination\Breeze\IN\Work";
            string folderPath = null;
            try
            {
                LoggerCF.GetInstance().LogInfo("Preprocessor Service Started..", brokerMessage);
                (result, editedXml, jobsheetXml, reuestID) = PreprocessorHelper.ValidateAndAssignBasicValue(brokerMessage);

                if (!result)
                    throw new PreprocessorException("Preprocessor Pre validation gets failed.Unable To Process..");

               folderPath= PreprocessorHelper.CreateFolderStructure(workingFolder, editedXml.FileName.Replace(".xml", "") + "_" + DateTime.UtcNow.ToString("yyyy-MM-ddTHHmmss"));

               string inputxmlPath= PreprocessorHelper.CopyEditedXml(folderPath,editedXml);


               string jobSheetXmlPath = PreprocessorHelper.CopyJobsheetXml(folderPath, jobsheetXml);

               bool isImagedownloaded = PreprocessorHelper.CopyImages(folderPath, brokerMessage);

                string outxmlPath= PreprocessorHelper.SetOutputXmlPath(folderPath, editedXml);

               new clsJNLrendering(inputxmlPath, outxmlPath, "true", jobSheetXmlPath, "");
              

             bool isPGXml=  PreprocessorHelper.CreatePGXml(inputxmlPath, jobSheetXmlPath, folderPath,brokerMessage);
                if (System.IO.File.Exists(outxmlPath) == false)
                {
                    LoggerCF.GetInstance().LogInfo("Failed to create outxml: " + outxmlPath, brokerMessage);
                    throw new PreprocessorException("Failed to create outxml: " + outxmlPath);
                }
                if (isPGXml == false)
                {
                    LoggerCF.GetInstance().LogInfo("Failed to create pg xml: " + outxmlPath, brokerMessage);
                    throw new PreprocessorException("Failed to create pg xml: " + outxmlPath);
                }

                LoggerCF.GetInstance().LogInfo("Preprocessor Service Completed..", brokerMessage);

                result = true;

            }
            catch (Exception ex)
            {
                LoggerCF.GetInstance().LogError("Preprocessor service get failed." + "\n" + ex.Message.ToString());
                BrokerMessageHelper.SetError(brokerMessage, ex.Message, ex.StackTrace, GlobalAppSetting.appsetting.ACDC_PREPROCESSOR_APP_NAME);
                AuditLogHelper.alert_message.Add(new AlertMessage { code = Constants.technicalException, description = ex.Message, elementref = ex.StackTrace });
                result = false;
                DeleteFolder(folderPath);
                throw;
            }
            return (result, folderPath);
        }
        private void DeleteFolder(string tempstorage)
        {
            if (Directory.Exists(tempstorage))
            {
                Directory.Delete(tempstorage, true);
            }
        }

    }
}
