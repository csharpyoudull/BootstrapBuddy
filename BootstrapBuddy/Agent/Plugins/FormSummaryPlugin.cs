using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace BootstrapBuddy.Agent.Plugins
{
    public class FormSummaryPlugin
    {
        [KernelFunction]
        [Description("Provides a summary of the forms requirements.")]
        public Task<string> GetSummary()
        {
            var result = Buddy.GetRequirements();
            var output =
                $"The following requirements have been added to the form: {result}";
            return Task.FromResult(output);
        }
    }
}
