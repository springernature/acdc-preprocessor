using System;
using Microsoft.Extensions.Options;
using Acdc.Preprocessor.Settings;
using Newtonsoft.Json.Linq;
//using Acdc.MetadataSyncService.Core;
using System.Diagnostics;
//using Acdc.MetadataSyncService.Core.Helpers;
using System.Collections.Generic;
using Acdc.Preprocessor.Logging;
using Acdc.Preprocessor.Core;

namespace Acdc.Preprocessor.RMQ.Communicator
{
  public class Connector
  {
    private Sender _sender;
    private RmqConfiguration _configuration;
    private readonly AppSettings _appSettings;
    private string _receivedTime;
   private readonly Core.PreprocessorService preprocessorService;

    public Connector(IOptions<AppSettings> appSettings)
    {
      _appSettings = appSettings.Value;
      preprocessorService = new Core.PreprocessorService(_appSettings);
    }

    public void Connect()
    {
      InitializeConfiguration();
      InitializeSender();
      InitializeReceiver();
    }

    public void InitializeConfiguration()
    {
      _configuration = new RmqConfiguration
      {
        AppId = _appSettings.ACDC_PREPROCESSOR_APP_NAME,
        UserName = _appSettings.ACDC_RMQ_USER_NAME,
        Password = _appSettings.ACDC_RMQ_USER_PASSWORD,
        VirtualHost = _appSettings.ACDC_RMQ_VIRTUAL_HOST,
        PrefetchCount = Convert.ToUInt16(_appSettings.ACDC_RMQ_PREFETCH_COUNT),
        Timeout = Convert.ToUInt16(_appSettings.ACDC_RMQ_TIMEOUT),
        PersistentMessages = _appSettings.ACDC_RMQ_IS_PERSISTENT_MESSAGES,
        ExchangeName = _appSettings.ACDC_RMQ_EXCHANGE_NAME
      };

      _configuration.SetHostNames(_appSettings.ACDC_RMQ_HOST_NAMES);
    }

    public void InitializeSender()
    {
      _sender = new Sender(_configuration);
    }

    private void InitializeReceiver()
    {
      Receiver _receiver = new Receiver(_configuration);
      _receiver.StartMonitoring(_appSettings.ACDC_PREPROCESSOR_QUEUE_NAME, _appSettings.ACDC_RMQ_EXCHANGE_NAME, MessageHandler);
    }

    protected void MessageHandler(string queueMessage, byte queuePriority)
    {
      bool isProcessed;
      float elapsedSeconds = 0;
      var sw = new Stopwatch();
      sw.Start();

      var brokerMessage = JObject.Parse(queueMessage);

      //AuditLogHelper.alert_message = new List<AlertMessage>();
      //AuditLogHelper.alert_message.Clear();
      //LoggerCF.GetInstance().LogInfo("Queue Priority received :: " + queuePriority.ToString());

            //   SendReceivedMsgToAuditrail(brokerMessage, queuePriority);
     (isProcessed,brokerMessage) = preprocessorService.Start(brokerMessage);

     elapsedSeconds = (float)sw.ElapsedMilliseconds / 1000;

 //     SendResultMsgToAuditrail(brokerMessage, elapsedSeconds, queuePriority, isProcessed);

      SendToRMQ(brokerMessage, queuePriority);
    }

    private void SendToRMQ(JObject brokerMessage, byte queuePriority)
    {
      LoggerCF.GetInstance().LogInfo("Queue Priority send :: " + queuePriority.ToString());
      _sender.Send(_appSettings.ACDC_FLUX_ROUTING_KEY, brokerMessage, queuePriority);
    }

    //public void SendReceivedMsgToAuditrail(JObject _brokerMessage, byte queuePriority)
    //{

    //  _receivedTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
    //  var auditMessage = new Core.AuditMessage
    //  {
    //    AcdcId = !string.IsNullOrEmpty(_brokerMessage["id"].ToString()) ? _brokerMessage["id"].ToString() : null,
    //    PrimaryID = !string.IsNullOrEmpty(BrokerMessageHelper.GetGlobalMsId(_brokerMessage)) ? BrokerMessageHelper.GetGlobalMsId(_brokerMessage) : null,
    //    SecondaryID = !string.IsNullOrEmpty(BrokerMessageHelper.GetManuscriptId(_brokerMessage)) ? BrokerMessageHelper.GetManuscriptId(_brokerMessage) : null,
    //    RequestID = !string.IsNullOrEmpty(BrokerMessageHelper.GetRequestId(_brokerMessage)) ? BrokerMessageHelper.GetRequestId(_brokerMessage) : null,
    //    ConsumerID = !string.IsNullOrEmpty(BrokerMessageHelper.GetConsumerId(_brokerMessage)) ? BrokerMessageHelper.GetConsumerId(_brokerMessage) : null,
    //    ProductionTaskID = !string.IsNullOrEmpty(BrokerMessageHelper.GetProductionTaskId(_brokerMessage)) ? BrokerMessageHelper.GetProductionTaskId(_brokerMessage) : null,
    //    JournalID = !string.IsNullOrEmpty(BrokerMessageHelper.GetJournalId(_brokerMessage)) ? BrokerMessageHelper.GetJournalId(_brokerMessage) : null,
    //    Revision = !string.IsNullOrEmpty(BrokerMessageHelper.GetRevision(_brokerMessage)) ? BrokerMessageHelper.GetRevision(_brokerMessage) : null,
    //    Workflow = !string.IsNullOrEmpty(BrokerMessageHelper.GetWorkFlow(_brokerMessage)) ? BrokerMessageHelper.GetWorkFlow(_brokerMessage) : null,
    //    JournalCode = !string.IsNullOrEmpty(BrokerMessageHelper.GetJournalCode(_brokerMessage)) ? BrokerMessageHelper.GetJournalCode(_brokerMessage) : null,
    //    PackageName = !string.IsNullOrEmpty(BrokerMessageHelper.GetPackageName(_brokerMessage)) ? BrokerMessageHelper.GetPackageName(_brokerMessage) : null,
    //    AppName = _appSettings.ACDC_PREPROCESSOR_APP_NAME,
    //    status = "received",
    //    ReceivedTimestamp = _receivedTime,
    //    ProcessedTimestamp = null,
    //    ElapsedTime = 0.0,
    //    AlertMessage = AuditLogHelper.alert_message.Count != 0 ? JArray.FromObject(AuditLogHelper.alert_message) : new JArray { }

    //  };
    //  _sender.Send(_appSettings.ACDC_AUDIT_TRAIL_ROUTING_KEY, auditMessage, queuePriority);

    //}

    //public void SendResultMsgToAuditrail(JObject _brokerMessage, float elapsedSeconds, byte queuePriority, bool isSuccess)
    //{
    //  if (isSuccess)
    //  {
    //    SendSuccessMsgToAuditrail(_brokerMessage, elapsedSeconds, queuePriority);
    //  }
    //  else
    //  {
    //    SendFailMsgToAuditrail(_brokerMessage, queuePriority);
    //  }
    //}

    //public void SendFailMsgToAuditrail(JObject _brokerMessage, byte queuePriority)
    //{
    //  JArray errors = new JArray();

    //  if (_brokerMessage["rmq"] != null && _brokerMessage["rmq"]["errors"] != null && _brokerMessage["rmq"]["errors"] is JArray)
    //    errors.Add(_brokerMessage["rmq"]["errors"]);

    //  var auditMessage = new Core.AuditMessage
    //  {
    //    AcdcId = !string.IsNullOrEmpty(_brokerMessage["id"].ToString()) ? _brokerMessage["id"].ToString() : null,
    //    PrimaryID = !string.IsNullOrEmpty(BrokerMessageHelper.GetGlobalMsId(_brokerMessage)) ? BrokerMessageHelper.GetGlobalMsId(_brokerMessage) : null,
    //    SecondaryID = !string.IsNullOrEmpty(BrokerMessageHelper.GetManuscriptId(_brokerMessage)) ? BrokerMessageHelper.GetManuscriptId(_brokerMessage) : null,
    //    RequestID = !string.IsNullOrEmpty(BrokerMessageHelper.GetRequestId(_brokerMessage)) ? BrokerMessageHelper.GetRequestId(_brokerMessage) : null,
    //    ConsumerID = !string.IsNullOrEmpty(BrokerMessageHelper.GetConsumerId(_brokerMessage)) ? BrokerMessageHelper.GetConsumerId(_brokerMessage) : null,
    //    ProductionTaskID = !string.IsNullOrEmpty(BrokerMessageHelper.GetProductionTaskId(_brokerMessage)) ? BrokerMessageHelper.GetProductionTaskId(_brokerMessage) : null,
    //    JournalID = !string.IsNullOrEmpty(BrokerMessageHelper.GetJournalId(_brokerMessage)) ? BrokerMessageHelper.GetJournalId(_brokerMessage) : null,
    //    Revision = !string.IsNullOrEmpty(BrokerMessageHelper.GetRevision(_brokerMessage)) ? BrokerMessageHelper.GetRevision(_brokerMessage) : null,
    //    Workflow = !string.IsNullOrEmpty(BrokerMessageHelper.GetWorkFlow(_brokerMessage)) ? BrokerMessageHelper.GetWorkFlow(_brokerMessage) : null,
    //    JournalCode = !string.IsNullOrEmpty(BrokerMessageHelper.GetJournalCode(_brokerMessage)) ? BrokerMessageHelper.GetJournalCode(_brokerMessage) : null,
    //    PackageName = !string.IsNullOrEmpty(BrokerMessageHelper.GetPackageName(_brokerMessage)) ? BrokerMessageHelper.GetPackageName(_brokerMessage) : null,
    //    AppName = _appSettings.ACDC_METADATASYNC_APP_NAME,
    //    status = "failed",
    //    ReceivedTimestamp = _receivedTime,
    //    ProcessedTimestamp = null,
    //    ElapsedTime = 0.0,
    //    AlertMessage = JArray.FromObject(AuditLogHelper.alert_message)
    //  };
    //  _sender.Send(_appSettings.ACDC_AUDIT_TRAIL_ROUTING_KEY, auditMessage, queuePriority);
    //}

    //public void SendSuccessMsgToAuditrail(JObject _brokerMessage, float elapsedSeconds, byte queuePriority)
    //{
    //  var auditMessage = new Core.AuditMessage
    //  {
    //    AcdcId = !string.IsNullOrEmpty(_brokerMessage["id"].ToString()) ? _brokerMessage["id"].ToString() : null,
    //    PrimaryID = !string.IsNullOrEmpty(BrokerMessageHelper.GetGlobalMsId(_brokerMessage)) ? BrokerMessageHelper.GetGlobalMsId(_brokerMessage) : null,
    //    SecondaryID = !string.IsNullOrEmpty(BrokerMessageHelper.GetManuscriptId(_brokerMessage)) ? BrokerMessageHelper.GetManuscriptId(_brokerMessage) : null,
    //    RequestID = !string.IsNullOrEmpty(BrokerMessageHelper.GetRequestId(_brokerMessage)) ? BrokerMessageHelper.GetRequestId(_brokerMessage) : null,
    //    ConsumerID = !string.IsNullOrEmpty(BrokerMessageHelper.GetConsumerId(_brokerMessage)) ? BrokerMessageHelper.GetConsumerId(_brokerMessage) : null,
    //    ProductionTaskID = !string.IsNullOrEmpty(BrokerMessageHelper.GetProductionTaskId(_brokerMessage)) ? BrokerMessageHelper.GetProductionTaskId(_brokerMessage) : null,
    //    JournalID = !string.IsNullOrEmpty(BrokerMessageHelper.GetJournalId(_brokerMessage)) ? BrokerMessageHelper.GetJournalId(_brokerMessage) : null,
    //    Revision = !string.IsNullOrEmpty(BrokerMessageHelper.GetRevision(_brokerMessage)) ? BrokerMessageHelper.GetRevision(_brokerMessage) : null,
    //    Workflow = !string.IsNullOrEmpty(BrokerMessageHelper.GetWorkFlow(_brokerMessage)) ? BrokerMessageHelper.GetWorkFlow(_brokerMessage) : null,
    //    JournalCode = !string.IsNullOrEmpty(BrokerMessageHelper.GetJournalCode(_brokerMessage)) ? BrokerMessageHelper.GetJournalCode(_brokerMessage) : null,
    //    PackageName = !string.IsNullOrEmpty(BrokerMessageHelper.GetPackageName(_brokerMessage)) ? BrokerMessageHelper.GetPackageName(_brokerMessage) : null,
    //    AppName = _appSettings.ACDC_METADATASYNC_APP_NAME,
    //    status = "success",
    //    ReceivedTimestamp = _receivedTime,
    //    ProcessedTimestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
    //    ElapsedTime = elapsedSeconds,
    //    AlertMessage = AuditLogHelper.alert_message.Count != 0 ? JArray.FromObject(AuditLogHelper.alert_message) : new JArray { }
    //  };
    //  _sender.Send(_appSettings.ACDC_AUDIT_TRAIL_ROUTING_KEY, auditMessage, queuePriority);

    //}
  }
}
