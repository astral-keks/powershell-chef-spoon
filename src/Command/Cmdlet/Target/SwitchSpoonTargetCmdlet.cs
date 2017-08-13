using AstralKeks.ChefSpoon.Core;
using AstralKeks.Workbench.PowerShell;
using AstralKeks.Workbench.PowerShell.Attributes;
using System.Linq;
using System.Management.Automation;

namespace AstralKeks.ChefSpoon.Command
{
    [Cmdlet(VerbsCommon.Switch, Noun.SpoonTarget)]
    public class SwitchSpoonTargetCmdlet : DynamicCmdlet
    {
        [DynamicParameter(Position = 0, Mandatory = true)]
        [DynamicCompleter(nameof(GetConfigurationNames))]
        public string Name => Parameters.GetValue<string>(nameof(Command));

        protected override void ProcessRecord()
        {
            SpoonConfig.SelectSecondary(Name);
        }

        private string[] GetConfigurationNames(string configurationNamePart)
        {
            return SpoonConfig.Secondary.ToArray();
        }
    }
}
