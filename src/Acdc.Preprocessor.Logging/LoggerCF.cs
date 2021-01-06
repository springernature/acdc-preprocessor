using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Acdc.Preprocessor.Settings;
using Serilog;
using Microsoft.Extensions.Configuration;
using Serilog.Formatting.Compact;


namespace Acdc.Preprocessor.Logging
{
  public sealed class LoggerCF
  {
    private LoggerCF()
    { }

    private static Serilog.Core.Logger _logger;

    private static LoggerCF LoggerInstance { get; set; }

    public string ACDC_Id { get; set; }
    public string APP_Name { get; set; }
    public string Manuscript_ID { get; set; }
    public string ProductionTask_ID { get; set; }
    public string Journal_ID { get; set; }
    public string Journal_Code { get; set; }
    public string Package_Name { get; set; }
    public string Global_MS_ID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public static LoggerCF GetInstance()
    {
      LoggerInstance = LoggerInstance ?? new LoggerCF();

      return LoggerInstance;
    }
    public static void SetElkConfiguration(IConfiguration configuration)
    {
      _logger = new LoggerConfiguration().Enrich.WithProperty("service", configuration["ACDC_PREPROCESSOR_APP_NAME"])
      .Enrich.WithProperty("module_name", configuration["ACDC_PREPROCESSOR_APP_NAME"])
      .Enrich.WithProperty("environment", configuration["ACDC_ENVIRONMENT"])
      .WriteTo.Sentry($"{configuration["ACDC_SENTRY_CLIENT_KEY"]}", string.Empty, $"{configuration["ACDC_ENVIRONMENT"]}")
      .WriteTo.Console(new RenderedCompactJsonFormatter())
      .CreateLogger();
    }
    public static string GetMessageId(JObject message)
    {
      const string ID = "id";
      return message[ID] != null ? message[ID].ToString() : string.Empty;
    }
    public void LogStartService(JObject brokerMessage)
    {
      StartTime = DateTime.Now;
     
      var log = new AcdcLogDetails
      {
        APP_Name = APP_Name,
        ACDC_Id = GetMessageId(brokerMessage),
        Broker_Message = brokerMessage.ToString(),
        Message = "acdc-metadata-sync started for ID: " + brokerMessage["id"].ToString() + " at: " + StartTime.ToString(),
        Status = MessageStatus.Inprocess,
        HostName = Environment.MachineName,
        Manuscript_Id = Manuscript_ID,
        Production_Task_Id = ProductionTask_ID,
        Journal_Id = Journal_ID,
        Journal_Code = Journal_Code,
        Package_Name = Package_Name,
        Global_Ms_Id = Global_MS_ID,
        Elapsed_Time_In_Seconds = 0,
      };

      _logger.Information("{app_name}{acdc_id}{message}{broker_message}{status}{host}{manuscript_id}{production_task_id}{journal_id}{journal_code}{package_name}{global_ms_id}{elapsed_time_in_seconds}",
        log.APP_Name,
        log.ACDC_Id,
        log.Message,
        log.Broker_Message,
        log.Status,
        log.HostName,
        log.Manuscript_Id,
        log.Production_Task_Id,
        log.Journal_Id,
        log.Journal_Code,
        log.Package_Name,
        log.Global_Ms_Id,
        log.Elapsed_Time_In_Seconds
       );

    }
    public void LogServiceCompleted(JObject brokerMessage)
    {
      EndTime = DateTime.Now;

      var log = new AcdcLogDetails
      {
        APP_Name = APP_Name,
        ACDC_Id = GetMessageId(brokerMessage),
        Broker_Message = brokerMessage.ToString(),
        Message = "acdc-metadata-sync completed for ID: " + brokerMessage["id"].ToString() + " at: " + EndTime.ToString(),
        Status = MessageStatus.Completed,
        HostName = Environment.MachineName,
        Manuscript_Id = Manuscript_ID,
        Production_Task_Id = ProductionTask_ID,
        Journal_Id = Journal_ID,
        Journal_Code = Journal_Code,
        Package_Name = Package_Name,
        Global_Ms_Id = Global_MS_ID,
        Elapsed_Time_In_Seconds = (EndTime - StartTime).TotalSeconds
              };

      _logger.Information("{app_name}{acdc_id}{message}{broker_message}{status}{host}{manuscript_id}{production_task_id}{journal_id}{journal_code}{package_name}{global_ms_id}{elapsed_time_in_seconds}",
        log.APP_Name,
        log.ACDC_Id,
        log.Message,
        log.Broker_Message,
        log.Status,
        log.HostName,
        log.Manuscript_Id,
        log.Production_Task_Id,
        log.Journal_Id,
        log.Journal_Code,
        log.Package_Name,
        log.Global_Ms_Id,
        log.Elapsed_Time_In_Seconds
      );
    }

    public void LogInfo(string infoText, JObject brokerMessage)
    {
      var log = new AcdcLogDetails
      {
        APP_Name = APP_Name,
        ACDC_Id = GetMessageId(brokerMessage),
        Broker_Message = brokerMessage.ToString(),
        Message = infoText,
        Status = MessageStatus.Inprocess,
        HostName = Environment.MachineName,
        Manuscript_Id = Manuscript_ID,
        Production_Task_Id = ProductionTask_ID,
        Journal_Id = Journal_ID,
        Journal_Code = Journal_Code,
        Package_Name = Package_Name,
        Global_Ms_Id = Global_MS_ID,
        Elapsed_Time_In_Seconds = 0,
             };
      if (_logger != null)
      {
        _logger.Information("{app_name}{acdc_id}{message}{broker_message}{status}{host}{manuscript_id}{production_task_id}{journal_id}{journal_code}{package_name}{global_ms_id}{elapsed_time_in_seconds}",
          log.APP_Name,
          log.ACDC_Id,
          log.Message,
          log.Broker_Message,
          log.Status,
          log.HostName,
          log.Manuscript_Id,
          log.Production_Task_Id,
          log.Journal_Id,
          log.Journal_Code,
          log.Package_Name,
          log.Global_Ms_Id,
          log.Elapsed_Time_In_Seconds
        );
      }
    }

    public void LogInfo(string infoText)
    {
      var log = new AcdcLogDetails
      {
        APP_Name = APP_Name,
        ACDC_Id = "",
        Message = infoText,
        Status = MessageStatus.Inprocess,
        HostName = Environment.MachineName,
        Manuscript_Id = Manuscript_ID,
        Production_Task_Id = ProductionTask_ID,
        Journal_Id = Journal_ID,
        Journal_Code = Journal_Code,
        Package_Name = Package_Name,
        Global_Ms_Id = Global_MS_ID,
        Elapsed_Time_In_Seconds = 0,
      };
      if (_logger != null)
      {
        _logger.Information("{app_name}{acdc_id}{message}{status}{host}{manuscript_id}{production_task_id}{journal_id}{journal_code}{package_name}{global_ms_id}{elapsed_time_in_seconds}",
        log.APP_Name,
        log.ACDC_Id,
        log.Message,
        log.Status,
        log.HostName,
        log.Manuscript_Id,
        log.Production_Task_Id,
        log.Journal_Id,
        log.Journal_Code,
        log.Package_Name,
        log.Global_Ms_Id,
        log.Elapsed_Time_In_Seconds
      );
      }
    }
    public void LogError(string error)
    {
      var log = new AcdcLogDetails
      {
        APP_Name = APP_Name,
        ACDC_Id ="",
        Message = error,
        Status = MessageStatus.Error,
        HostName = Environment.MachineName,
        Manuscript_Id = Manuscript_ID,
        Production_Task_Id = ProductionTask_ID,
        Journal_Id = Journal_ID,
        Journal_Code = Journal_Code,
        Package_Name = Package_Name,
        Global_Ms_Id = Global_MS_ID,
        Elapsed_Time_In_Seconds = 0
        
      };
      if (_logger != null)
      {
        _logger.Error("{app_name}{acdc_id}{message}{status}{host}{manuscript_id}{production_task_id}{journal_id}{journal_code}{package_name}{global_ms_id}{elapsed_time_in_seconds}",
          log.APP_Name,
          log.ACDC_Id,
          log.Message,
          log.Status,
          log.HostName,
          log.Manuscript_Id,
          log.Production_Task_Id,
          log.Journal_Id,
          log.Journal_Code,
          log.Package_Name,
          log.Global_Ms_Id,
          log.Elapsed_Time_In_Seconds

        );
      }
    }

    public void LogError(Exception ex, JObject brokerMessage)
    {
      var log = new AcdcLogDetails
      {
        APP_Name = APP_Name,
        ACDC_Id = GetMessageId(brokerMessage),
        Broker_Message = brokerMessage.ToString(),
        Message = "Error in acdc-metadata-sync. " + ex.Message,
        Status = MessageStatus.Error,
        Exception = ex,
        HostName = Environment.MachineName,
        Manuscript_Id = Manuscript_ID,
        Production_Task_Id = ProductionTask_ID,
        Journal_Id = Journal_ID,
        Journal_Code = Journal_Code,
        Package_Name = Package_Name,
        Global_Ms_Id = Global_MS_ID,
        Elapsed_Time_In_Seconds = 0
        
      };

      _logger.Error("{app_name}{acdc_id}{message}{broker_message}{exception}{status}{host}{manuscript_id}{production_task_id}{journal_id}{journal_code}{package_name}{global_ms_id}{elapsed_time_in_seconds}",
        log.APP_Name,
        log.ACDC_Id,
        log.Message,
        log.Broker_Message,
        log.Exception,
        log.Status,
        log.HostName,
        log.Manuscript_Id,
        log.Production_Task_Id,
        log.Journal_Id,
        log.Journal_Code,
        log.Package_Name,
        log.Global_Ms_Id,
        log.Elapsed_Time_In_Seconds

      );

    }

    public void LogError(string error, JObject brokerMessage)
    {
      var log = new AcdcLogDetails
      {
        APP_Name = APP_Name,
        ACDC_Id = ACDC_Id,
        Broker_Message = brokerMessage.ToString(),
        Message = error,
        Status = MessageStatus.Error,
        HostName = Environment.MachineName,
        Manuscript_Id = Manuscript_ID,
        Production_Task_Id = ProductionTask_ID,
        Journal_Id = Journal_ID,
        Journal_Code = Journal_Code,
        Package_Name = Package_Name,
        Global_Ms_Id = Global_MS_ID,
        Elapsed_Time_In_Seconds = 0
        
      };

      _logger.Error(
        "{app_name}{acdc_id}{message}{broker_message}{status}{host}{manuscript_id}{production_task_id}{journal_id}{journal_code}{package_name}{global_ms_id}{elapsed_time_in_seconds}",
        log.APP_Name,
        log.ACDC_Id,
        log.Message,
        log.Broker_Message,
        log.Status,
        log.HostName,
        log.Manuscript_Id,
        log.Production_Task_Id,
        log.Journal_Id,
        log.Journal_Code,
        log.Package_Name,
        log.Global_Ms_Id,
        log.Elapsed_Time_In_Seconds
       
      );
    }

    public void LogRulesError(JObject brokerMessage, IEnumerable<string> errors)
    {
      foreach (var error in errors)
      {
        var log = new AcdcLogDetails
        {
          APP_Name = APP_Name,
          ACDC_Id = ACDC_Id,
          Broker_Message = brokerMessage.ToString(),
          Message = error,
          Status = MessageStatus.Inprocess,
          HostName = Environment.MachineName,
          Manuscript_Id = Manuscript_ID,
          Production_Task_Id = ProductionTask_ID,
          Journal_Id = Journal_ID,
          Journal_Code = Journal_Code,
          Package_Name = Package_Name,
          Global_Ms_Id = Global_MS_ID,
          Elapsed_Time_In_Seconds = 0
          
        };

        _logger.Information(
          "{app_name}{acdc_id}{message}{broker_message}{status}{host}{manuscript_id}{production_task_id}{journal_id}{journal_code}{package_name}{global_ms_id}{elapsed_time_in_seconds}",
          log.APP_Name,
          log.ACDC_Id,
          log.Message,
          log.Broker_Message,
          log.Status,
          log.HostName,
          log.Manuscript_Id,
          log.Production_Task_Id,
          log.Journal_Id,
          log.Journal_Code,
          log.Package_Name,
          log.Global_Ms_Id,
          log.Elapsed_Time_In_Seconds
          
        );
      }
    }

    public void LogRetryMechanism(string link)
    {
      var log = new AcdcLogDetails
      {
        Message = "Not able to get response, retrying to call service: " + link,
        Status = MessageStatus.Inprocess
      };

      _logger.Information(
        "{message}{status}",
        log.Message,
        log.Status
      );
    }
  }
}
