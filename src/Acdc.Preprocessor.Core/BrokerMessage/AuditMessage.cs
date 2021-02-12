using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acdc.Preprocessor.Core
{
    public class AuditMessage
    {
        [JsonProperty("id")]
        public string AcdcId { get; set; }

        [JsonProperty("primary_id")]
        public string PrimaryID { get; set; }

        [JsonProperty("secondary_id")]
        public string SecondaryID { get; set; }

        [JsonProperty("request_id")]
        public string RequestID { get; set; }

        [JsonProperty("consumer_id")]
        public string ConsumerID { get; set; }

        [JsonProperty("tenant")]
        public string Tenant { get; set; }

        [JsonProperty("production_task_id")]
        public string ProductionTaskID { get; set; }

        [JsonProperty("journal_id")]
        public string JournalID { get; set; }

        [JsonProperty("revision")]
        public string Revision { get; set; }

        [JsonProperty("workflow")]
        public string Workflow { get; set; }

        [JsonProperty("journal_code")]
        public string JournalCode { get; set; }

        [JsonProperty("package_name")]
        public string PackageName { get; set; }

        [JsonProperty("app_name")]
        public string AppName { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("received_timestamp")]
        public string ReceivedTimestamp { get; set; }

        [JsonProperty("processed_timestamp")]
        public string ProcessedTimestamp { get; set; }

        [JsonProperty("elapsed_time_in_seconds")]
        public double ElapsedTime { get; set; }

        [JsonProperty("alert_message")]
        public JArray AlertMessage { get; set; }

        [JsonProperty("errors")]
        public JArray Errors { get; set; }
    }
}
