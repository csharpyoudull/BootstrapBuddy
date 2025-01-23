using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace BootstrapBuddy.Agent.Plugins
{
    public class RequirementGatheringPlugin
    {
        [KernelFunction]
        [Description("Adds requirements / parameters for building the form")]
        public Task<string> AddRequirements([Description("The requirements or parameters for the form")] string requirements)
        {
            var result = Buddy.AppendRequirement(requirements);
            var output =
                $@"New requirements have been added form the form. 
                The current requirements are:
                {result}
                
                Suggest possible requirements to add to the form or if the form is ready to generate.";
            return Task.FromResult(output);
        }
    }
}
