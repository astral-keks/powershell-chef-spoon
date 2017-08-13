using AstralKeks.ChefSpoon.Core.Resources;
using AstralKeks.Workbench.Common.Context;
using AstralKeks.Workbench.Common.FileSystem;
using AstralKeks.Workbench.Common.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;

namespace AstralKeks.ChefSpoon.Core
{
    public class Spoon
    {
        public static object Execute(string command, string verb, string[] arguments, SpoonConfig config = null)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));
            command = command.Trim();
            verb = (verb ?? string.Empty).Trim();
            arguments = arguments ?? new string[0];
            config = config ?? SpoonConfig.Primary;
            if (string.IsNullOrWhiteSpace(config.KnifeConfig))
                throw new InvalidOperationException("Target is missing");

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

            var knifeExe = FsPath.Absolute(Location.Workspace(), config.KnifeExe);
            var knifeConfig = FsPath.Absolute(Location.Workspace(), Directories.Config, config.KnifeConfig);
            var knifeArgs = $"{command.ToLower()} {verb.ToLower()} {string.Join(" ", arguments)} -c '{knifeConfig}' -F json";

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

                try
                {
                    var obj = new StringJsonObject(content);
                    return obj.Read<JToken>().ToObject(obj);
                }
                catch (JsonReaderException)
                {
                    Console.WriteLine(content);
                    return null;
                }
            }
        }
    }
}
