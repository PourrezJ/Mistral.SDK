using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class AgentCompletionRequest
    {
        /// <summary>
        /// The maximum number of tokens to generate in the completion.
        /// </summary>
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// Whether to stream back partial progress.
        /// </summary>
        [JsonPropertyName("stream")]
        public bool? Stream { get; set; }

        /// <summary>
        /// A sequence of tokens at which the generation will stop.
        /// </summary>
        [JsonPropertyName("stop")]
        public string Stop { get; set; }

        /// <summary>
        /// The seed to use for random sampling.
        /// </summary>
        [JsonPropertyName("random_seed")]
        public int? RandomSeed { get; set; }

        /// <summary>
        /// List of chat messages for completion.
        /// </summary>
        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; }

        /// <summary>
        /// Response format type.
        /// </summary>
        [JsonPropertyName("response_format")]
        public ResponseFormat ResponseFormat { get; set; }

        /// <summary>
        /// List of tools.
        /// </summary>
        [JsonPropertyName("tools")]
        public List<Common.Tool> Tools { get; set; }

        /// <summary>
        /// Tool choice type (e.g., "auto").
        /// </summary>
        [JsonPropertyName("tool_choice")]
        public string ToolChoice { get; set; }

        /// <summary>
        /// Presence penalty.
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        public double PresencePenalty { get; set; }

        /// <summary>
        /// Frequency penalty.
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        public double FrequencyPenalty { get; set; }

        /// <summary>
        /// Number of completions to generate.
        /// </summary>
        [JsonPropertyName("n")]
        public int N { get; set; }

        /// <summary>
        /// The agent ID.
        /// </summary>
        [JsonPropertyName("agent_id")]
        public string AgentId { get; set; }

        /// <summary>
        /// Constructor for the `CompletionRequest` class.
        /// </summary>
        public AgentCompletionRequest(
            int? maxTokens = default(int?),
            bool? stream = false,
            string stop = null,
            int? randomSeed = default(int?),
            List<ChatMessage> messages = default(List<ChatMessage>),
            ResponseFormat responseFormat = default,
            List<Common.Tool> tools = default,
            string toolChoice = "none",
            double presencePenalty = 0.0,
            double frequencyPenalty = 0.0,
            int n = 1,
            string agentId = null)
        {
            // to ensure "messages" is required (not null)
            if (messages == null)
            {
                throw new ArgumentNullException("messages is a required property for ChatCompletionRequest and cannot be null");
            }

            MaxTokens = maxTokens;
            Stream = stream ?? false;
            Stop = stop;
            RandomSeed = randomSeed;
            Messages = messages;
            ResponseFormat = responseFormat;
            Tools = tools ?? new List<Common.Tool>();
            ToolChoice = toolChoice;
            PresencePenalty = presencePenalty;
            FrequencyPenalty = frequencyPenalty;
            N = n;
            AgentId = agentId;
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        public IEnumerable<ValidationResult> Validate()
        {
            if (MaxTokens < 0)
            {
                yield return new ValidationResult("Invalid value for MaxTokens, must be a value greater than or equal to 0.", new[] { "MaxTokens" });
            }

            if (PresencePenalty < 0.0 || PresencePenalty > 1.0)
            {
                yield return new ValidationResult("PresencePenalty must be between 0.0 and 1.0.", new[] { "PresencePenalty" });
            }

            if (FrequencyPenalty < 0.0 || FrequencyPenalty > 1.0)
            {
                yield return new ValidationResult("FrequencyPenalty must be between 0.0 and 1.0.", new[] { "FrequencyPenalty" });
            }

            yield break;
        }
    }
}
