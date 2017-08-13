using AstralKeks.ChefSpoon.Core.Resources;
using AstralKeks.Workbench.Common.Context;
using AstralKeks.Workbench.Common.FileSystem;
using AstralKeks.Workbench.Common.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AstralKeks.ChefSpoon.Core
{
    public class SpoonConfig
    {
        public string KnifeExe { get; set; }

        public string KnifeConfig { get; set; }

        public string[] Commands { get; set; }

        public string[] Verbs { get; set; }

        public Dictionary<string, string[]> Arguments { get; set; }



        public static SpoonConfig Primary
        {
            get
            {
                var resourceManager = new ResourceManager(typeof(Spoon));
                var primaryResource = GetResource(resourceManager, GetResourceName());
                var primaryConfig = primaryResource.Read<SpoonConfig>();
                return primaryConfig;
            }
        }

        public static IEnumerable<string> Secondary
        {
            get
            {
                var directory = FsPath.Absolute(Location.Workspace(), Directories.Config);
                return Directory.EnumerateFiles(directory)
                  .Where(f => f.StartsWith(Files.Knife, StringComparison.OrdinalIgnoreCase))
                  .Select(f => Path.GetFileNameWithoutExtension(f).Replace(Files.Knife, string.Empty));
            }
        }

        public static void CreateSecondary(string secondaryName, 
            string chefServerUrl, string chefUserNodeName, string chefValidationNodeName)
        {
            if (string.IsNullOrWhiteSpace(secondaryName))
                throw new ArgumentNullException(nameof(secondaryName));
            if (string.IsNullOrWhiteSpace(chefServerUrl))
                throw new ArgumentNullException(nameof(chefServerUrl));
            if (string.IsNullOrWhiteSpace(chefUserNodeName))
                throw new ArgumentNullException(nameof(chefUserNodeName));
            if (string.IsNullOrWhiteSpace(chefValidationNodeName))
                throw new ArgumentNullException(nameof(chefValidationNodeName));
            if (Secondary.Contains(secondaryName, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException($"Configuration {secondaryName} already exists");

            var resourceManager = new ResourceManager(typeof(Spoon));
            var secondaryResourceName = GetResourceName(secondaryName);
            var secondaryResource = GetResource(resourceManager, secondaryResourceName);

            var secondaryConfig = secondaryResource.Read<string>();
            secondaryConfig = secondaryConfig.Replace(Patterns.ChefServer, Patterns.ChefServer + chefServerUrl);
            secondaryConfig = secondaryConfig.Replace(Patterns.UserNode, Patterns.UserNode + chefUserNodeName);
            secondaryConfig = secondaryConfig.Replace(Patterns.ValidationNode, Patterns.ValidationNode + chefValidationNodeName);
            secondaryResource.Write(secondaryConfig);

            var primaryResource = GetResource(resourceManager, GetResourceName());
            var primaryConfig = primaryResource.Read<SpoonConfig>();
            if (string.IsNullOrWhiteSpace(primaryConfig.KnifeConfig))
            {
                primaryConfig.KnifeConfig = secondaryResourceName;
                primaryResource.Write(primaryConfig);
            }
        }

        public static void SelectSecondary(string secondaryName)
        {
            if (string.IsNullOrWhiteSpace(secondaryName))
                throw new ArgumentNullException(nameof(secondaryName));

            var resourceManager = new ResourceManager(typeof(Spoon));
            var secondaryResourceName = GetResourceName(secondaryName);
            var primaryResource = GetResource(resourceManager, secondaryResourceName);
            var primaryConfig = primaryResource.Read<SpoonConfig>();
            primaryConfig.KnifeConfig = secondaryResourceName;
            primaryResource.Write(primaryConfig);
        }

        private static string GetResourceName(string secondaryName = null)
        {
            return secondaryName != null ? Files.Knife + secondaryName + Files.RbExtension : Files.Spoon;
        }

        private static Resource GetResource(ResourceManager resourceManager, string resourceName)
        {
            var locations = new[] { Location.Workspace(), Location.Userspace(Directories.Spoon) };
            return resourceManager.CreateResource(locations, Directories.Config, resourceName);
        }

    }
}
