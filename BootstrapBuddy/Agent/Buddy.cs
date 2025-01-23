#pragma warning disable SKEXP0070
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootstrapBuddy.Agent.Plugins;

namespace BootstrapBuddy.Agent
{
    public static class Buddy
    {
        private static string ollamaEndpoint = "http://localhost:11434";
        private static readonly Kernel _kernel;
        private static ChatHistory _chatHistory;
        private static readonly IChatCompletionService _chatCompletion;
        private static StringBuilder _formRequirements = new();
        private static Task? PendingTask { get; set; }
        public static bool GeneratingHtml { get; set; }
        public static Action<string>? OnHtmlReady { get; set; }
        public static Action<string>? OnChatMessage { get; set; }
        static Buddy()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOllamaChatCompletion("llama3.2:latest", new Uri(ollamaEndpoint));
            builder.Plugins.AddFromType<RequirementGatheringPlugin>();
            builder.Plugins.AddFromType<FormBuilderPlugin>();
            builder.Plugins.AddFromType<FormSummaryPlugin>();
            _kernel = builder.Build();
          
            _chatCompletion = _kernel.GetRequiredService<IChatCompletionService>();
            _chatHistory = new ChatHistory("You are Buddy a friendly project management assistant, you help gather requirements and parameters of forms. You only respond conversationally, you do not respond with versions of the form being built. You always respond in a friendly manor.");
        }

        public static string AppendRequirement(string requirement)
        {
            _formRequirements.AppendLine(requirement);
            return _formRequirements.ToString();
        }
        public static async Task ChatAsync(string userMessage)
        {
            try
            {
                var settings = new PromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                };
                _chatHistory.AddUserMessage(userMessage);

                var response = await _chatCompletion.GetChatMessageContentAsync(_chatHistory, settings, _kernel);
                

                if (PendingTask != null)
                {
                    _chatHistory.AddAssistantMessage("Your form is being generated please wait, this may take a minute.");
                    response = await _chatCompletion.GetChatMessageContentAsync(_chatHistory, settings, _kernel);
                    var responseMessage = response.Content;
                    _chatHistory.AddAssistantMessage(responseMessage);
                    OnChatMessage?.Invoke(responseMessage);
                    PendingTask.Start();
                }
                else
                {
                    var responseMessage = response.Content;
                    _chatHistory.AddAssistantMessage(responseMessage);
                    OnChatMessage?.Invoke(responseMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during chat interaction: {ex.Message}", ex);
            }
        }

        public static void GenerateForm()
        {
            PendingTask = new Task(async void () =>
            {
                try
                {
                    GeneratingHtml = true;
                    var builder = Kernel.CreateBuilder();
                    builder.AddOllamaChatCompletion("phi4:latest", new Uri(ollamaEndpoint));
                    builder.Plugins.AddFromType<RequirementGatheringPlugin>();
                    var k = builder.Build();

                    var completion = k.GetRequiredService<IChatCompletionService>();
                    var prompt = $"Using bootstrap 4 and t he following requirements create a form for the user.\n\n" +
                                 $"Requirements:\n\n" +
                                 $"{_formRequirements}";
                    var history = new ChatHistory("You are a programming assistant that helps generate forms with bootstrap 4.");
                    
                    history.AddUserMessage(prompt);
                    var response = await completion.GetChatMessageContentAsync(history);
                    //get the html from the response
                    var result = HtmlExtractor.ExtractHtml(response.Content);

                    OnHtmlReady?.Invoke(result);
                    PendingTask = null;
                    GeneratingHtml = false;
                }
                catch (Exception ex)
                {
                    //we would normally do something here.
                }
            });
        }

        public static string GetRequirements()
        {
            return _formRequirements.ToString();
        }
    }
}
