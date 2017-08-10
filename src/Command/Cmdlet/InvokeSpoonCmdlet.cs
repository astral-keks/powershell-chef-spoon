using System.Management.Automation;
using AstralKeks.Workbench.PowerShell;
using AstralKeks.Workbench.PowerShell.Attributes;
using AstralKeks.ChefSpoon.Core;
using System.Linq;

namespace AstralKeks.ChefSpoon.Command
{
    [Cmdlet(VerbsLifecycle.Invoke, SpoonNoun)]
    [OutputType(typeof(string))]
    public class InvokeSpoonCmdlet : DynamicPSCmdlet
    {
        public const string SpoonNoun = "Spoon";

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
            return Spoon.Configuration().Commands.Where(c => c.Contains(commandPart)).ToArray();
        }

        public string[] GetVerbs(string verbPart)
        {
            return Spoon.Configuration().Verbs.Where(c => c.Contains(verbPart)).ToArray();
        }
    }
}
