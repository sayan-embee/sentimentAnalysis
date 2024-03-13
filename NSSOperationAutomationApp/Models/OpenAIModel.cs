using Newtonsoft.Json;

namespace NSSOperationAutomationApp.Models
{
    public class OpenAIModel
    {
        [JsonProperty("responseModel")]
        public ReturnMessageModel ResponseModel { get; set; }

        [JsonProperty("outputModel")]
        public SummaryModel? OutputModel { get; set; }
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
}
