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
                .Select(c => new
                {
                    Command = c,
                    Index = c.IndexOf(commandPart, StringComparison.OrdinalIgnoreCase)
                })
                .Where(t => t.Index >= 0)
                .OrderBy(t => t.Index)
                .Select(t => t.Command)
                .ToArray();
        }

        public string[] GetVerbs(string verbPart)
        {
            return SpoonConfig.Primary.Verbs
                .Select(v => new
                {
                    Verb = v,
                    Index = v.IndexOf(verbPart, StringComparison.OrdinalIgnoreCase)
                })
                .Where(t => t.Index >= 0)
                .OrderBy(t => t.Index)
                .Select(t => t.Verb)
                .ToArray();
        }
    }
}
