using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace BootstrapBuddy.Agent.Plugins
{
    public class FormBuilderPlugin
    {
        [KernelFunction]
        [Description("Generates, creates, exports, builds, the form when the user explicitly asks for the form to be generated, or created, or exported, or built.")]
        public Task<string> GenerateForm()
        {
            Buddy.GenerateForm();
            return Task.FromResult("The form is being generated, do not return HTML, or JSON only inform the user that the form is being constructed.");
        }
    }
}
