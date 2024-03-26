using Newtonsoft.Json;

namespace NSSOperationAutomationApp.Models
{
    public class OpenAIModel
    {
        [JsonProperty("responseModel")]
        public ReturnMessageModel ResponseModel { get; set; }

        [JsonProperty("outputModel")]
        public SummaryModel? OutputModel { get; set; }

        [JsonProperty("fileOutputModel")]
        public BlobFileUploadModel? FileOutputModel { get; set; }
    }

    public class SpeakerModel
    {
        [JsonProperty("transcriptionSpeaker")]
        public string TranscriptionSpeaker { get; set; }
    }

    public class SummaryModel
    {
        [JsonProperty("summaryText")]
        public string SummaryText { get; set; }

        [JsonProperty("sentiment")]
        public string Sentiment { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("transcribeText")]
        public string TranscribeText { get; set; }
    }

    public class AudioOutputModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("transcriptionSpeakerList")]
        public List<SpeakerModel> TranscriptionSpeakerList { get; set; }

        [JsonProperty("extractedText")]
        public string ExtractedText { get; set; }

        [JsonProperty("summary")]
        public SummaryModel Summary { get; set; }
    }

    public class BlobFileUploadModel
    {
        [JsonProperty("fileId")]
        public long FileId { get; set; }

        [JsonProperty("refId")]
        public string RefId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileInternalName")]
        public string FileInternalName { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        //[JsonProperty("uniqueFileKey")]
        //public string UniqueFileKey { get; set; }
    }

    public class GetSentimentAnalysisModel
    {
        [JsonProperty("autoId")]
        public int? AutoId { get; set; }

        [JsonProperty("summaryText")]
        public string SummaryText { get; set; }

        [JsonProperty("sentiment")]
        public string Sentiment { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("transcribeText")]
        public string TranscribeText { get; set; }


        [JsonProperty("fileRefId")]
        public string FileRefId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileInternalName")]
        public string FileInternalName { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("createdOnIST")]
        public DateTime? CreatedOnIST { get; set; }

    }
}
