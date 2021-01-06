using Acdc.Preprocessor.Settings;
using Newtonsoft.Json.Linq;
using SharpRaven;
using System;
using Acdc.Preprocessor.Logging;
using SharpRaven.Data;
using System.IO;

namespace Acdc.Preprocessor.Core
{
    public class PreprocessorService
    {
        private readonly AppSettings _appSettings;
        private readonly RavenClient _ravenClient;

        public PreprocessorService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _ravenClient = new RavenClient(_appSettings.ACDC_SENTRY_CLIENT_KEY);
            GlobalAppSetting.appsetting = _appSettings;
          
        }

        public (bool, JObject) Start(JObject brokerMessage)
        {
            JArray errors = null;
            bool isSuccess = false;
            string tempstorage = null;
            try
            {
                var logger = LoggerCF.GetInstance();
                SetLogFields(brokerMessage);
                logger.LogStartService(brokerMessage);

                (isSuccess, tempstorage) = new XmlToPreProcessedXml().Process(brokerMessage);


                
                logger.LogServiceCompleted(brokerMessage);
            }
            catch (Exception ex)
            {
                _ravenClient.Capture(new SentryEvent(ex));
                LoggerCF.GetInstance().LogError(ex, brokerMessage);
                errors = BrokerMessageHelper.SetError(brokerMessage, "Error in acdc-preprocessor service: " + ex.Message, ex.StackTrace, _appSettings.ACDC_PREPROCESSOR_APP_NAME);
                AuditLogHelper.alert_message.Add(new AlertMessage
                {
                    code = Constants.technicalException,
                    description = ex.Message,
                    elementref = ex.StackTrace
                });
            }
            brokerMessage = BrokerMessageHelper.GetMessage(brokerMessage, isSuccess, _appSettings, tempstorage);
            if (!isSuccess)
            {
                DeleteFolder(tempstorage);
            }
            return (isSuccess, brokerMessage);
        }
        private void DeleteFolder(string tempstorage)
        {
            if (Directory.Exists(tempstorage))
            {
                Directory.Delete(tempstorage, true);
            }
        }
        private void SetLogFields(JObject brokerMessage)
        {
            var logger = LoggerCF.GetInstance();
            logger.APP_Name = _appSettings.ACDC_PREPROCESSOR_APP_NAME;
            //// logger.ACDC_Id = BrokerMessageHelper.GetMessageId(brokerMessage);
            //logger.Manuscript_ID = BrokerMessageHelper.GetManuscriptId(brokerMessage);
            //logger.ProductionTask_ID = BrokerMessageHelper.GetProductionTaskId(brokerMessage);
            //logger.Journal_ID = BrokerMessageHelper.GetJournalId(brokerMessage);
            //logger.Global_MS_ID = BrokerMessageHelper.GetGlobalMsId(brokerMessage);
            //logger.Journal_Code = BrokerMessageHelper.GetJournalCode(brokerMessage);
            //logger.Package_Name = BrokerMessageHelper.GetPackageName(brokerMessage);
        }
    }
}
