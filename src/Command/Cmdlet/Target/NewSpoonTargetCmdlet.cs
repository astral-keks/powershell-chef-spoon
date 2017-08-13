using AstralKeks.Workbench.PowerShell;
using AstralKeks.Workbench.PowerShell.Attributes;
using System.Management.Automation;
using AstralKeks.ChefSpoon.Core;

namespace AstralKeks.ChefSpoon.Command
{
    [Cmdlet(VerbsCommon.New, Noun.SpoonTarget)]
    public class NewSpoonTargetCmdlet : DynamicCmdlet
    {
        [DynamicParameter(Position = 0, Mandatory = true)]
        public string Name => Parameters.GetValue<string>(nameof(Name));

        [DynamicParameter(Position = 1, Mandatory = true)]
        public string ChefServerUrl => Parameters.GetValue<string>(nameof(ChefServerUrl));

        [DynamicParameter(Position = 2, Mandatory = true)]
        public string ChefUserNodeName => Parameters.GetValue<string>(nameof(ChefUserNodeName));

        [DynamicParameter(Position = 3, Mandatory = true)]
        public string ChefValidationNodeName => Parameters.GetValue<string>(nameof(ChefValidationNodeName));

        protected override void ProcessRecord()
        {
            SpoonConfig.CreateSecondary(Name, ChefServerUrl, ChefUserNodeName, ChefValidationNodeName);
        }
    }
}
