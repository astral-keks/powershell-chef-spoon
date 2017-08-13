using System.Management.Automation;
using AstralKeks.Workbench.PowerShell;
using AstralKeks.Workbench.PowerShell.Attributes;
using AstralKeks.ChefSpoon.Core;
using System.Linq;
using System;

namespace AstralKeks.ChefSpoon.Command
{
    [Cmdlet(VerbsLifecycle.Invoke, Noun.Spoon)]
    [OutputType(typeof(string))]
    public class InvokeSpoonCmdlet : DynamicPSCmdlet
    {
        [DynamicParameter(Position = 0, Mandatory = true)]
        [DynamicCompleter(nameof(GetCommands))]
        public string Command => Parameters.GetValue<string>(nameof(Command));

        [DynamicParameter(Position = 1, Mandatory = true)]
        [DynamicCompleter(nameof(GetVerbs))]
        public string Verb => Parameters.GetValue<string>(nameof(Verb));

        [Parameter(Position = 2, ValueFromRemainingArguments = true)]
        public string[] Arguments { get; set; }

        protected override void ProcessRecord()
        {
            var result = Spoon.Execute(Command, Verb, Arguments);
            WriteObject(result);
        }

        public string[] GetCommands(string commandPart)
        {
            return SpoonConfig.Primary.Commands
                .Where(c => c.IndexOf(commandPart, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();
        }

        public string[] GetVerbs(string verbPart)
        {
            return SpoonConfig.Primary.Verbs
                .Where(c => c.IndexOf(verbPart, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();
        }
    }
}
