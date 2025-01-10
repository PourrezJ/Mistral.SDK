using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Mistral.SDK.DTOs
{
    public partial class AgentsEndpoint : EndpointBase
    {
        protected override string Endpoint => "agents/completions";
        public AgentsEndpoint(MistralClient client) : base(client) { }

        /// <summary>
        /// Makes a non-streaming call to the completion API. Be sure to set stream to false in <param name="parameters"></param>.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        public async Task<ChatCompletionResponse> GetCompletionAsync(AgentCompletionRequest request, CancellationToken cancellationToken = default)
        {
            request.Stream = false;

            var response = await HttpRequest(Url, HttpMethod.Post, request, cancellationToken).ConfigureAwait(false);

            var toolCalls = new List<Common.Function>();
            foreach (var message in response.Choices)
            {
                if (message.Message.ToolCalls is null) continue;
                foreach (var returned_tool in message.Message.ToolCalls)
                {
                    var tool = request.Tools?.FirstOrDefault(t => t.Function.Name == returned_tool.Function.Name);
                    if (tool != null)
                    {
                        tool.Function.Arguments = returned_tool.Function.Arguments;
                        tool.Function.Id = returned_tool.Id;
                        toolCalls.Add(tool.Function);
                    }
                }
            }
            response.ToolCalls = toolCalls;



            return response;
        }

        /// <summary>
        /// Makes a streaming call to the completion API using an IAsyncEnumerable. Be sure to set stream to true in <param name="request"></param>.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        public async IAsyncEnumerable<ChatCompletionResponse> StreamCompletionAsync(AgentCompletionRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            request.Stream = true;
            await foreach (var result in HttpStreamingRequest(Url, HttpMethod.Post, request, cancellationToken).WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var toolCalls = new List<Common.Function>();
                foreach (var message in result.Choices)
                {
                    if (message.Delta.ToolCalls is null) continue;
                    foreach (var returned_tool in message.Delta.ToolCalls)
                    {
                        var tool = request.Tools?.FirstOrDefault(t => t.Function.Name == returned_tool.Function.Name);
                        if (tool != null)
                        {
                            tool.Function.Arguments = returned_tool.Function.Arguments;
                            tool.Function.Id = returned_tool.Id;
                            toolCalls.Add(tool.Function);
                        }
                    }
                }

                result.Choices.First().Delta.Role = ChatMessage.RoleEnum.Assistant;
                result.ToolCalls = toolCalls;
                yield return result;
            }
        }
    }
}
