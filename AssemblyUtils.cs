namespace FLUtils
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    public class AssemblyUtils
    {
        public static string Name(bool executingAssembly) => executingAssembly ? Assembly.GetExecutingAssembly().FullName : Assembly.GetCallingAssembly().FullName;
        public static Version Version(bool executingAssembly) => executingAssembly ? Assembly.GetExecutingAssembly().GetName().Version : Assembly.GetCallingAssembly().GetName().Version;
        public static string Company(bool executingAssembly) =>
            executingAssembly
                ? FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).CompanyName
                : FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location).CompanyName;

        public static string Description(bool executingAssembly) =>
            executingAssembly
                ? FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileDescription
                : FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location).FileDescription;

        public static string Copyright(bool executingAssembly) =>
            executingAssembly
                ? FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).LegalCopyright
                : FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location).LegalCopyright;
    }
}
