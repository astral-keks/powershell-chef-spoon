using AstralKeks.Workbench.Common.Context;

namespace AstralKeks.ChefSpoon.Core.Resources
{
    internal class Paths
    {
        public static string WorkspaceDirectory => Location.Workspace();

        public static string UserspaceDirectory => Location.Userspace(Directories.Spoon);
    }
}
