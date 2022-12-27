// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using UnityEditor;

using UnityEngine;

using Debug = UnityEngine.Debug;

namespace NatsunekoLaboratory.AssetLens
{
    internal static class AssetLensProcessor
    {
        private const string ExecutableRootGuid = "9274170124102d04896a0ea15d541cf6";

        public static List<AssetLensReferent> FindReferences(List<string> assets, List<string> excludes)
        {
            var executable = GetExecutablePath();
            if (!File.Exists(executable))
                throw new FileNotFoundException(executable);

            var result = new List<AssetLensReferent>();

            foreach (var (asset, i) in assets.Select((w, i) => (w, i)))
            {
                var title = $"Searching {i + 1} of {assets.Count} references";
                if (EditorUtility.DisplayCancelableProgressBar(title, title, i / (float)assets.Count))
                    break;

                var references = RunExecutable(executable, asset, excludes);
                var referent = new AssetLensReferent(asset);
                referent.AddRange(references);
                result.Add(referent);
            }

            EditorUtility.ClearProgressBar();

            return result;
        }

        private static List<AssetLensReference> RunExecutable(string path, string asset, List<string> excludes)
        {
            var references = new List<AssetLensReference>();

            try
            {
                using (var process = new Process())
                {
                    var arguments = string.Join(" ", BuildCommandArguments(asset, "./", excludes));
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = arguments,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        WorkingDirectory = Directory.GetCurrentDirectory()
                    };

                    process.Start();

                    var sr = process.StandardOutput;
                    var lines = sr.ReadToEnd().Split('\n').Select(w => w.Trim());
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var reference = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), line));
                        if (File.Exists(reference))
                            references.Add(new AssetLensReference(line.Substring(2)));
                    }

                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return references;
        }

        private static List<string> BuildCommandArguments(string asset, string root, List<string> excludes)
        {
            var arguments = new List<string>
            {
                $"\"guid: {asset},\"", // reference only (exclude others)
                root, // root directory
                "-l" // output filepath only
            };

            foreach (var exclude in excludes)
                arguments.Add($"-g \"!{exclude}\""); // excludes

            return arguments;
        }

        private static string GetExecutablePath()
        {
            var root = AssetDatabase.GUIDToAssetPath(ExecutableRootGuid);
            return Path.Combine(Directory.GetCurrentDirectory(), root, GetExecutableArch(), GetExecutablePlatform(), $"rg{GetExecutableExtension()}");
        }

        private static string GetExecutableArch()
        {
            // Windows x86, macOS x86, Linux x86 is discontinued
            return Environment.Is64BitOperatingSystem ? "x86_64" : "aarch64";
        }

        private static string GetExecutablePlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                    return "apple-darwin";

                case RuntimePlatform.WindowsEditor:
                    return "windows";

                case RuntimePlatform.LinuxEditor:
                    return "linux-musl";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetExecutableExtension()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                    return "";

                case RuntimePlatform.WindowsEditor:
                    return ".exe";

                case RuntimePlatform.LinuxEditor:
                    return "";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}