using AstralKeks.ChefManagement.Core.Resources;
using AstralKeks.Workbench.Common.Context;
using AstralKeks.Workbench.Common.Json;
using AstralKeks.Workbench.Common.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AstralKeks.ChefSpoon.Core
{
    public class Spoon
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager(typeof(Spoon));

        public static SpoonConfig Configuration()
        {
            var locations = new[] { Location.Workspace(), Location.Userspace(Directories.Spoon) };
            var configResource = _resourceManager.CreateResource(locations, Directories.Config, Files.Spoon);
            var config = configResource.Read<SpoonConfig>();
            return config;
        }

        public static object Execute(string command, string verb, string[] arguments, SpoonConfig config = null)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            command = command.Trim();
            verb = (verb ?? string.Empty).Trim();
            arguments = arguments ?? new string[0];
            config = config ?? Configuration();

            var knife = BootstrapKnife(command, verb, arguments, config);
            return ExecuteKnife(knife.Exe, knife.Args);
        }
        
        private static (string Exe, string Args) BootstrapKnife(string command, string verb, string[] arguments, SpoonConfig config)
        {
            var passedArguments = arguments.ToList();
            if (config.Arguments.ContainsKey($"{command}.{verb}"))
            {
                var defaultArguments = config.Arguments[$"{command}.{verb}"].Where(a => !arguments.Contains(a));
                passedArguments.AddRange(defaultArguments);
            }
            arguments = passedArguments.Select(a => a?.ToLower()).ToArray();

            var knifeExe = Path.IsPathRooted(config.KnifeExe) ? config.KnifeExe : Path.Combine(Location.Workspace(), config.KnifeExe);
            var knifeConfig = Path.IsPathRooted(config.KnifeConfig) ? config.KnifeConfig : Path.Combine(Location.Workspace(), config.KnifeConfig);
            var knifeArgs = $"{command.ToLower()} {verb.ToLower()} {string.Join(" ", arguments)} -c '{knifeConfig}' -F json";

            var locations = new[] { Location.Workspace(), Location.Userspace(Directories.Spoon) };
            _resourceManager.CreateResource(locations, Directories.Config, Files.Knife);

            return (knifeExe, knifeArgs);
        }

        private static object ExecuteKnife(string knifeExe, string knifeArgs)
        {
            var processInfo = new ProcessStartInfo(knifeExe, knifeArgs);
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;

            using (var process = Process.Start(processInfo))
            {
                var content = process?.StandardOutput.ReadToEnd();
                process?.WaitForExit();

                if (!string.IsNullOrEmpty(content))
                {
                    var obj = new StringJsonObject(content);
                    return obj.Read<JToken>().ToObject(obj);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
