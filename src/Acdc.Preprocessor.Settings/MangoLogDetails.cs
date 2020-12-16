using System;
using System.Collections.Generic;

namespace Acdc.Preprocessor.Settings
{
  public class AcdcLogDetails
  {
    public string ACDC_Id { get; set; }
    public string Broker_Message { get; set; }
    public string Message { get; set; }
    public Exception Exception { get; set; }
    public Dictionary<string, object> AdditionalInfo { get; set; }
    public MessageStatus Status { get; set; }
    public string APP_Name { get; set; }
    public string HostName { get; set; }
    public string Manuscript_Id { get; set; }
    public string Production_Task_Id { get; set; }
    public string Journal_Id { get; set; }
    public string Journal_Code { get; set; }
    public string Package_Name { get; set; }
    public string Global_Ms_Id { get; set; }
    public double? Elapsed_Time_In_Seconds { get; set; }
      
  }
}
