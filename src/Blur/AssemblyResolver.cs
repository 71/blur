using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

#if NET_CORE
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
#endif

namespace Blur.Processing
{
    /// <summary>
    /// This class provides a method meant to help resolve
    /// assemblies on runtime.
    /// </summary>
    /// <remarks>
    /// Thanks to Jb Evain for this work on Mono.Cecil.
    /// <conceptualLink target="https://github.com/jbevain/cecil/blob/master/Mono.Cecil/BaseAssemblyResolver.cs"/>
    /// </remarks>
    public static class AssemblyResolver
    {
        private static readonly Lazy<string[]> _gacPaths = new Lazy<string[]>(GetGacPaths);

        #region Properties
        /// <summary>
        /// Gets whether or not the current program is running on Unix.
        /// </summary>
        public static bool IsRunningOnUnix
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Unix:
                    case PlatformID.MacOSX:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets whether or not the current program is running on Windows.
        /// </summary>
        public static bool IsRunningOnWindows
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.Win32NT:
                    case PlatformID.WinCE:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Gets whether or not the current program is running on Mono.
        /// </summary>
        public static bool IsRunningOnMono => Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Gets or sets a list of all the references of the target assembly.
        /// </summary>
        public static string[] References { get; set; }

        /// <summary>
        /// Gets a list of all the paths of the GAC.
        /// </summary>
        public static string[] GacPaths => _gacPaths.Value;
        #endregion

        /// <inheritdoc cref="Resolve(AssemblyName)" select="summary"/>
        public static Assembly Resolve(string name) => Resolve(new AssemblyName(name));

        /// <summary>
        /// Attempts to resolve an assembly given its name.
        /// </summary>
        public static Assembly Resolve(AssemblyName an)
        {
            Assembly assembly;

            // Try loaded assemblies
            //if (an.Name == "Blur.Library")
            //    return Processor.BlurLibrary;

#if NET_CORE
            assembly = ResolveNetCore(an);
            if (assembly != null)
                return assembly;
#endif

            // Try references.
            if (References != null)
            {
                for (int i = 0; i < References.Length; i++)
                {
                    string reference = References[i];

                    if (Path.GetFileNameWithoutExtension(reference)?
                            .Equals(an.Name, StringComparison.OrdinalIgnoreCase) ?? false)
                    {
                        return LoadAssembly(reference);
                    }
                }
            }

            // Try mscorlib.
            if (an.Name == "mscorlib")
            {
                assembly = ResolveMscorlib(an);
                if (assembly != null)
                    return assembly;
            }

            // Try current directory.
            assembly = SearchDirectory(Directory.GetCurrentDirectory(), an.Name);
            if (assembly != null)
                return assembly;

            // Try GAC.
            assembly = ResolveGAC(an);
            if (assembly != null)
                return assembly;

            // Try Nuget cache.
            return ResolveNugetCache(an);
        }

        /// <summary>
        /// Attempts to resolve the mscorlib.dll assembly.
        /// </summary>
        private static Assembly ResolveMscorlib(AssemblyName an)
        {
            // If the currently loaded assembly is good, return it instead.
            Version v = an.Version;
            Assembly currentCorlib = typeof(object).GetTypeInfo().Assembly;

            if (currentCorlib.GetName().Version == v)
                return currentCorlib;

            // Heh, load it manually.
            string path = Directory.GetParent(
                Directory.GetParent(typeof(object).Module.FullyQualifiedName).FullName
            ).FullName;

            if (IsRunningOnUnix)
            {
                switch (v.Major)
                {
                    case 1:
                        path = Path.Combine(path, "1.0");
                        break;
                    case 2:
                        path = Path.Combine(path, v.MajorRevision == 5 ? "2.1" : "2.0");
                        break;
                    case 4:
                        path = Path.Combine(path, "4.0");
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                switch (v.Major)
                {
                    case 1:
                        path = Path.Combine(path, v.MajorRevision == 3300 ? "v1.0.3705" : "v1.0.5000.0");
                        break;
                    case 2:
                        path = Path.Combine(path, "v2.0.50727");
                        break;
                    case 4:
                        path = Path.Combine(path, "v4.0.30319");
                        break;
                    default:
                        return null;
                }
            }

            path = Path.Combine(path, "mscorlib.dll");

            if (File.Exists(path))
                return LoadAssembly(path);

            return null;
        }

        /// <summary>
        /// Attempts to resolve an assembly in the Global Assembly Cache.
        /// </summary>
        private static Assembly ResolveGAC(AssemblyName an)
        {
            byte[] publicKeyToken = an.GetPublicKeyToken();

            if (publicKeyToken == null || publicKeyToken.Length == 0)
                return null;

            if (IsRunningOnMono)
            {
                for (int i = 0; i < GacPaths.Length; i++)
                {
                    string gacPath = GacPaths[i];
                    string file = GetGacAssemblyFile(an, publicKeyToken, string.Empty, gacPath);

                    if (File.Exists(file))
                        return LoadAssembly(file);
                }
            }
            else
            {
                string[] gacs = { "GAC_MSIL", "GAC_32", "GAC_64", "GAC" };
                string[] prefixes = { string.Empty, "v4.0_" };

                for (int i = 0; i < GacPaths.Length; i++)
                {
                    for (int o = 0; o < prefixes.Length; o++)
                    {
                        string gacPath = Path.Combine(GacPaths[i], gacs[o]);
                        string file = GetGacAssemblyFile(an, publicKeyToken, prefixes[i], gacPath);

                        if (File.Exists(file))
                            return LoadAssembly(file);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to resolve an assembly in the NuGet cache.
        /// </summary>
        private static Assembly ResolveNugetCache(AssemblyName an)
        {
            return ResolveNugetCache(Environment.GetEnvironmentVariable("NUGET_PACKAGES") ?? "", an)
                ?? ResolveNugetCache(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".nuget", "packages"), an)
                ?? ResolveNugetCache(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".nuget", "packages"), an
            );
        }

        /// <summary>
        /// Attempts to resolve an assembly in the NuGet cache.
        /// </summary>
        private static Assembly ResolveNugetCache(string nugetCache, AssemblyName an)
        {
            // Try the following path: %USERPROFILE%\.nuget\packages\[name]\[version]\lib
            string path = Path.Combine(nugetCache, an.Name);

            if (!Directory.Exists(path))
                return null;

            string vString = $"{an.Version.Major}.{an.Version.Minor}";
            path = Directory.EnumerateDirectories(path, vString + '*').LastOrDefault();

            if (path == null)
                return null;

            // Check if the file is at the root
            string file = Path.Combine(path, an.Name + ".dll");
            if (File.Exists(file))
                return LoadAssembly(file);

            file = Path.Combine(path, an.Name + ".exe");
            if (File.Exists(file))
                return LoadAssembly(file);

            // Find it in one of the folders...
            Version v = Environment.Version;
            int maxVersion;
            int currentVersion = 0;

            if (v.Major == 4)
            {
                if (v.Build != 30319)
                    maxVersion = 40;
                else if (v.Revision >= 42000)
                    maxVersion = 46;
                else if (v.Revision >= 1000)
                    maxVersion = 45;
                else
                    maxVersion = 40;
            }
            else if (v.Major > 1)
            {
                return null;
            }
            else
            {
                maxVersion = v.Major*10 + v.Minor;
            }

            file = null;

            foreach (string directory in Directory.EnumerateDirectories(path))
            {
                Match m = Regex.Match(directory, @"(?<=net)\d+");

                if (m.Success)
                {
                    int nbr = int.Parse(m.Value);

                    if (maxVersion <= currentVersion || nbr <= currentVersion)
                        break;

                    file = directory;
                    currentVersion = nbr;
                }
                else if (true) // TODO: Replace this by a .NET Core check.
                {
                    m = Regex.Match(directory, @"(?<=netstandard)(\d+)\.(\d+)");
                    
                    if (m.Success) // TODO: Replace this by a .NET Core check on current assembly.
                        file = directory;
                }
            }

            if (file != null)
            {
                file = Path.Combine(file, an.Name + ".dll");
                if (File.Exists(file))
                    return LoadAssembly(file);

                file = Path.Combine(file, an.Name + ".exe");
                if (File.Exists(file))
                    return LoadAssembly(file);
            }

            return null;
        }

#if NET_CORE
        /// <summary>
        /// Attempts to resolve an assembly given its name,
        /// using <see cref="DependencyContext"/>.
        /// </summary>
        private static Assembly ResolveNetCore(AssemblyName name)
        {
            foreach (CompilationLibrary lib in DependencyContext.Default.CompileLibraries)
            {
                if (lib.Name == name.Name)
                {
                    string path = lib.ResolveReferencePaths().FirstOrDefault();

                    if (string.IsNullOrEmpty(path))
                        continue;

                    return LoadAssembly(path);
                }
            }

            return null;
        }
#endif

        #region Utils
        /// <summary>
        /// Searches the given <paramref name="directory"/> for the assembly whose
        /// name matches <paramref name="assemblyName"/>.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Results are returned by System methods; they're guaranteed to be non-null.")]
        private static Assembly SearchDirectory(string directory, string assemblyName)
        {
            if (!Directory.Exists(directory))
                return null;

            // Search files directly.
            foreach (string file in Directory.EnumerateFiles(directory, assemblyName + ".*")
                                             .Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                                                      || x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)))
            {
                return LoadAssembly(file);
            }

            // Search directories.
            foreach (string dir in Directory.EnumerateDirectories(directory, assemblyName + '*'))
            {
                return SearchDirectory(dir, assemblyName);
            }

            return null;
        }

        /// <summary>
        /// Loads an assembly, given a path to its location.
        /// </summary>
        private static Assembly LoadAssembly(string filepath)
        {
#if NET_CORE
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(filepath);
#else
            return Assembly.LoadFrom(filepath);
#endif
        }

        /// <summary>
        /// Attempts to resolve an assembly in the Global Assembly Cache.
        /// </summary>
        private static string[] GetGacPaths()
        {
            List<string> paths = new List<string>();

            if (IsRunningOnMono)
            {
                // Find current GAC.
                string currentGac = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);

                if (currentGac != null)
                    currentGac = Path.Combine(Directory.GetParent(currentGac).FullName, "gac");

                if (currentGac != null && Directory.Exists(currentGac))
                    paths.Add(currentGac);

                // Find GAC with environement variable.
                string envGacs = Environment.GetEnvironmentVariable("MONO_GAC_PREFIX");

                if (string.IsNullOrEmpty(envGacs))
                    return paths.ToArray();

                string[] prefixes = envGacs.Split(Path.PathSeparator);

                foreach (string prefix in prefixes)
                {
                    if (string.IsNullOrEmpty(prefix))
                        continue;

                    string gac = Path.Combine(Path.Combine(Path.Combine(prefix, "lib"), "mono"), "gac");

                    if (Directory.Exists(gac) && !paths.Contains(gac))
                        paths.Add(gac);
                }
            }
            else
            {
                string windir = Environment.GetEnvironmentVariable("WINDIR");

                if (windir == null)
                    return paths.ToArray();

                string gacPath = Path.Combine(windir, "assembly");
                if (Directory.Exists(gacPath))
                    paths.Add(gacPath);

                gacPath = Path.Combine(windir, Path.Combine("Microsoft.NET", "assembly"));
                if (Directory.Exists(gacPath))
                    paths.Add(gacPath);
            }

            return paths.ToArray();
        }
        
        /// <summary>
        /// Gets an assembly file in the GAC given its name, public key token,
        /// an optional prefix and its containing folder.
        /// </summary>
        private static string GetGacAssemblyFile(AssemblyName an, byte[] publicKeyToken, string prefix, string gac)
        {
            var gac_folder = new StringBuilder()
                .Append(prefix)
                .Append(an.Version)
                .Append("__");

            for (int i = 0; i < publicKeyToken.Length; i++)
                gac_folder.Append(publicKeyToken[i].ToString("x2"));

            return Path.Combine(
                Path.Combine(Path.Combine(gac, an.Name), gac_folder.ToString()),
            an.Name + ".dll");
        }
        #endregion
    }
}
