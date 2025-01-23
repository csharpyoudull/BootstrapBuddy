using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootstrapBuddy.Models
{
    internal class Message
    {
        public string Text { get; set; } = null!;
        public bool FromAgent { get; set; }
    }
}
