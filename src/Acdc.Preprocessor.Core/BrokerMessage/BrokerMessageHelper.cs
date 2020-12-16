using Acdc.Preprocessor.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acdc.Preprocessor.Core
{
   public class BrokerMessageHelper
    {
        protected BrokerMessageHelper()
        { }

        public static string GetRequestId(JObject message)
        {
            return message[Broker.RequestId] != null ? message[Broker.RequestId].ToString() : string.Empty;
        }
        public static string GetDocumentLinkFromBrokerMessage(JObject message, string documentName, string method)
        {
            var documents = message["documents_s200"];

            if(documents!=null)
            {
                foreach (var item in documents)
                {
                    if(item["document"].ToString() == documentName)
                    {
                        return GetLink(item, method);
                    }
                }
            }
            foreach (var document in documents)
            {
             
                //foreach (var item in document)
                //{
                //    if (document[Broker.Linkcloud].ToString() == documentName)
                //    {
                //        return GetLink(document, method);
                //    }
                //}
                
            }
            return string.Empty;
        }
        public static JObject GetMessage(JObject brokerMessage, bool isSuccess, AppSettings appSettings,string tempstoragePath=null)
        {
            if (brokerMessage[Broker.Rmq] == null) return brokerMessage;

            brokerMessage[Broker.Rmq][Broker.ModuleName] = appSettings.ACDC_PREPROCESSOR_APP_NAME;
            var messageStatus = isSuccess ? MessageStatus.success.ToString() : MessageStatus.fail.ToString();
            brokerMessage[Broker.Rmq][Broker.Status] = messageStatus;

            if(tempstoragePath!=null && isSuccess)
            {
                brokerMessage["temp_storage"] = tempstoragePath;
            }
            return brokerMessage;
        }
        public static string GetLink(JToken document, string method)
        {
            foreach (var link in document[Broker.Linkcloud])
            {
                if (link[Broker.Method].ToString() == method)
                    return link[Broker.Href].ToString();
            }
            return string.Empty;
        }
        private static void AddErrors(string exceptionMessage, string stackTrace, JArray errors)
        {
            var source = new JObject(
              new JProperty(Broker.Module, Broker.AcdcMetaSync),
              new JProperty(Broker.HostName, System.Environment.MachineName)
            );

            var detail = new JObject(
              new JProperty(Broker.Message, exceptionMessage),
              new JProperty(Broker.Trace, stackTrace),
              new JProperty(Broker.Source, source)
            );
            errors.Add(new JObject(new JProperty(Broker.Level, ""), new JProperty(Broker.Detail, detail)));
        }

        public static JArray SetError(JObject message, string exceptionMessage, string stackTrace, string moduleName)
        {
            var errors = new JArray();
            if (message[Broker.Rmq] != null && message[Broker.Rmq][Broker.Errors] != null &&
                message[Broker.Rmq][Broker.Errors] is JArray)
            {
                errors = (JArray)message[Broker.Rmq][Broker.Errors];
                AddErrors(exceptionMessage, stackTrace, errors);
            }
            else
            {
                AddErrors(exceptionMessage, stackTrace, errors);
            }
            return errors;
        }

    }
}
