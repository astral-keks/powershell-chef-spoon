using AstralKeks.ChefSpoon.Core;
using AstralKeks.PowerShell.Common;
using AstralKeks.PowerShell.Common.Attributes;
using System;
using System.Linq;
using System.Management.Automation;

namespace AstralKeks.ChefSpoon.Command
{
    [Cmdlet(VerbsCommon.Switch, Noun.SpoonTarget)]
    public class SwitchSpoonTargetCmdlet : DynamicCmdlet
    {
        [DynamicParameter(Position = 0, Mandatory = true)]
        [DynamicCompleter(nameof(GetTargetNames))]
        public string Name => Parameters.GetValue<string>(nameof(Name));

        protected override void ProcessRecord()
        {
            SpoonConfig.SelectSecondary(Name);
        }

        public string[] GetTargetNames(string targetNamePart)
        {
            return SpoonConfig.Secondary
                .Select(t => new
                {
                    Target = t,
                    Index = t.IndexOf(targetNamePart, StringComparison.OrdinalIgnoreCase)
                })
                .Where(t => t.Index >= 0)
                .OrderBy(t => t.Index)
                .Select(t => t.Target)
                .ToArray();
        }
    }
}
