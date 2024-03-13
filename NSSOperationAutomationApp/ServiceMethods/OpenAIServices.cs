using Azure.AI.OpenAI;
using Azure;
using Newtonsoft.Json;
using NSSOperationAutomationApp.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public class OpenAIServices : IOpenAIServices
    {
        private readonly IConfiguration _configuration;
        public OpenAIServices(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public async Task<(ReturnMessageModel, AudioOutputModel?)> TranscribeAudioFile(IFormFile formFile)
        {
            string deploymentName = this._configuration.GetSection("AzureOpenAI:SpeechModelType").Value.ToString();
            string endpoint = this._configuration.GetSection("AzureOpenAI:Endpoint").Value.ToString();
            string apiKey = this._configuration.GetSection("AzureOpenAI:Key").Value.ToString();
            string apiVersion = this._configuration.GetSection("AzureOpenAI:SpeechModelAPIVersion").Value.ToString();

            float temperature = float.Parse(this._configuration.GetSection("AzureOpenAI:Temperature").Value);

            if (string.IsNullOrEmpty(deploymentName) 
                || string.IsNullOrEmpty(endpoint)
                || string.IsNullOrEmpty(apiKey)
                || string.IsNullOrEmpty(apiVersion)
                || temperature < 0)
            {
                return (new ReturnMessageModel { Status = 0, ErrorMessage = "Invalid App-Settings!" }, null);
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("api-key", apiKey);

                    using (var content = new MultipartFormDataContent())
                    {
                        // Add deploymentName as a string content
                        content.Add(new StringContent(deploymentName), "deploymentName");

                        // Add the file stream directly from IFormFile
                        content.Add(new StreamContent(formFile.OpenReadStream()), "file", formFile.FileName);

                        // Add language and temperature as string content
                        content.Add(new StringContent("en"), "language");
                        content.Add(new StringContent(temperature.ToString()), "temperature");

                        using (var response = await client.PostAsync($"{endpoint}/openai/deployments/{deploymentName}/audio/transcriptions?{apiVersion}", content))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var result = await response.Content.ReadAsStringAsync();
                                if (result != null)
                                {
                                    var outputModel = new AudioOutputModel
                                    {
                                        Id = new Guid().ToString()
                                    };

                                    // Extract transcribed audio in text format

                                    // Deserialize the JSON string
                                    JsonDocument jsonDocument = JsonDocument.Parse(result);

                                    // Access the root element
                                    JsonElement root = jsonDocument.RootElement;

                                    // Extract the "text" property value
                                    if (root.TryGetProperty("text", out JsonElement textElement))
                                    {
                                        string textValue = textElement.GetString() ?? string.Empty;

                                        outputModel.ExtractedText = textValue;

                                        if (string.IsNullOrEmpty(textValue))
                                        {
                                            return (new ReturnMessageModel { Status = 0, ErrorMessage = "Transcription failed: Unable to extract 'text' property from JSON output of transcribed audio file!" }, outputModel);
                                        }                                      
                                    }
                                    else
                                    {
                                        return (new ReturnMessageModel { Status = 0, ErrorMessage = "Transcription failed: The 'text' property is not present in the JSON output of transcribed audio file!" }, outputModel);
                                    }

                                    return (new ReturnMessageModel { Status = 1, Message = "Audio file has been transcribed & summary has been generated successfully!" }, outputModel);
                                }
                            }
                            else
                            {
                                return (new ReturnMessageModel { Status = 0, ErrorMessage = $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}!" }, null);
                            }
                        }
                    }
                }

                return (new ReturnMessageModel { Status = 0, ErrorMessage = "Unexpected Error!" }, null);
            }
            catch (Exception ex)
            {
                return (new ReturnMessageModel { Status = 0, ErrorMessage = ex.Message.ToString() }, null);
            }
        }

        public async Task<(ReturnMessageModel, string)> GetChatAsync(string inputText, int maxTokens = 0)
        {
            try
            {
                string endpoint = this._configuration.GetSection("AzureOpenAI:Endpoint").Value.ToString();
                string key = this._configuration.GetSection("AzureOpenAI:Key").Value.ToString();

                if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
                {
                    return (new ReturnMessageModel { Status = 0, ErrorMessage = "Invalid App-Settings!" }, String.Empty);
                }

                string deploymentId = this._configuration.GetSection("AzureOpenAI:DeploymentModelId").Value.ToString();
                float temperature = float.Parse(this._configuration.GetSection("AzureOpenAI:Temperature").Value);
                float nucleusSamplingFactor = float.Parse(this._configuration.GetSection("AzureOpenAI:NucleusSamplingFactor").Value);
                float frequencyPenalty = float.Parse(this._configuration.GetSection("AzureOpenAI:FrequencyPenalty").Value);
                float presencePenalty = float.Parse(this._configuration.GetSection("AzureOpenAI:PresencePenalty").Value);
                string systemMessage = this._configuration.GetSection("AzureOpenAI:SystemMessage").Value.ToString();

                if (maxTokens == 0)
                {
                    maxTokens = int.Parse(this._configuration.GetSection("AzureOpenAI:MaxTokens").Value);
                }

                if (temperature < 0
                    || nucleusSamplingFactor < 0
                    || frequencyPenalty < 0
                    || presencePenalty < 0
                    || maxTokens < 0
                    )
                {
                    return (new ReturnMessageModel { Status = 0, ErrorMessage = "Invalid App-Settings!" }, String.Empty);
                }

                if (string.IsNullOrEmpty(systemMessage))
                {
                    return (new ReturnMessageModel { Status = 0, ErrorMessage = "Invalid App-Settings!" }, String.Empty);
                }

                OpenAIClient client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

                var stringBuilder = new StringBuilder();

                var query = @"Prompt: - Provide the summary in 3-4 sentences        
- Provide sentiment of the transcript as positive, negative or neutral       
- Provide the reason why the sentiment is positive, negative or neutral       
- Bind the whole output in the following JSON format : {""SummaryText"":""Summary of the transcript"",""Sentiment"":""Positive"", ""Reason"":""reason of the sentiment""}";

                stringBuilder.Append(query);
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine("");
                stringBuilder.Append(inputText);

                string prompt = stringBuilder.ToString();

                var chatCompletionOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = deploymentId,
                    Temperature = temperature,
                    MaxTokens = maxTokens,
                    NucleusSamplingFactor = nucleusSamplingFactor,
                    FrequencyPenalty = frequencyPenalty,
                    PresencePenalty = presencePenalty,
                    Messages =
                    {
                        new ChatRequestSystemMessage($"{systemMessage}"),
                        new ChatRequestUserMessage(prompt)
                    }
                };

                // If streaming is not selected
                Response<ChatCompletions> completionsResponse = await client.GetChatCompletionsAsync(chatCompletionOptions);

                if (completionsResponse != null)
                {
                    ChatCompletions completions = completionsResponse.Value;

                    if (completions != null)
                    {
                        var outputText = completions.Choices[0]?.Message?.Content.ToString() ?? string.Empty;

                        //var outputText = @"Azure OpenAI Service is a REST API that provides access to OpenAI's language models, including GPT-4, GPT-4 Turbo with Vision, GPT-3.5-Turbo, and Embeddings model series. These models can be adapted to specific tasks such as content generation, summarization, image understanding, semantic search, and natural language to code translation. Microsoft has made significant investments to ensure responsible AI use, including content filters, guidance, and principles. Access to Azure OpenAI is currently limited to customers with an existing partnership with Microsoft, lower risk use cases, and those committed to incorporating mitigations. Azure OpenAI offers the same models as OpenAI with the added security capabilities of Microsoft Azure and responsible AI content filtering.";

                        return (new ReturnMessageModel { Status = 1, Message = "Execution Successful"}, outputText);
                    }
                }

                return (new ReturnMessageModel { Status = 0, ErrorMessage = "Execution Failed" }, string.Empty);
            }
            catch (Exception ex)
            {
                return (new ReturnMessageModel { Status = 0, ErrorMessage = ex.Message.ToString() }, string.Empty);
            }
        }


    }
}
