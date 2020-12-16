using System;
using Microsoft.Extensions.Configuration;

namespace Acdc.Preprocessor.Settings
{  public class AppSettings
  {
    public string ACDC_RMQ_HOST_NAMES { get; set; }
    public string ACDC_RMQ_USER_NAME { get; set; }
    public string ACDC_RMQ_USER_PASSWORD { get; set; }
    public string ACDC_RMQ_VIRTUAL_HOST { get; set; }
    public string ACDC_RMQ_PREFETCH_COUNT { get; set; }
    public string ACDC_RMQ_TIMEOUT { get; set; }
    public bool ACDC_RMQ_IS_PERSISTENT_MESSAGES { get; set; }
    public string ACDC_RMQ_EXCHANGE_NAME { get; set; }
    public string ACDC_PREPROCESSOR_APP_NAME { get; set; }
    public string ACDC_PREPROCESSOR_QUEUE_NAME { get; set; }
    public string ACDC_ENVIRONMENT { get; set; }
    public string ACDC_FLUX_ROUTING_KEY { get; set; }
    public string ACDC_SENTRY_CLIENT_KEY { get; set; }
    public string ACDC_AUDIT_TRAIL_ROUTING_KEY { get; set; }
    public string ACDC_DB_CONNECTION { get; set; }





    public string EM_TO_ACDC_TITLE { get; set; }
    public string EM_TO_ACDC_ABSTRACT { get; set; }
    public string EM_TO_ACDC_KEYWORD { get; set; }
    public string EM_TO_ACDC_AUTHOR { get; set; }
    public string EM_TO_ACDC_AFFILIATION { get; set; }
    public string ACDC_TO_EM_TITLE { get; set; }
    public string ACDC_TO_EM_ABSTRACT { get; set; }
    public string ACDC_TO_EM_KEYWORD { get; set; }
    public string ACDC_TO_EM_AUTHOR { get; set; }
    public string ACDC_TO_EM_AFFILIATION { get; set; }


  }
}

