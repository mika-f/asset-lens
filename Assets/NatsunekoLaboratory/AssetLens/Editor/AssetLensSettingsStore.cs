// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace NatsunekoLaboratory.AssetLens
{
    [Serializable]
    internal class AssetLensSettingsStore : ScriptableObject
    {
        private const string Path = "ProjectSettings/AssetLens.json";

        public static AssetLensSettingsStore Load()
        {
            if (File.Exists(Path))
            {
                var instance = CreateInstance<AssetLensSettingsStore>();
                JsonUtility.FromJsonOverwrite(File.ReadAllText(Path), instance);

                return instance;
            }

            var defaults = CreateInstance<AssetLensSettingsStore>();
            defaults.Excludes = new List<string>
            {
                // Script Files
                "*.cs",
                "*.vb",
                "*.js",
                "*.ts",
                "*.md",
                "*.shader",
                "*.hlsl",
                "*.hlslinc",
                "*.glsl",
                "*.glslinc",
                "*.cg",
                "*.cginc",
                "*.json",

                // Project Files
                "*.csproj",
                "*.vbproj",
                "*.sln",

                // Unity Cache Directories
                "Library",
                "obj",
                "ProjectSettings",
                "Temp"
            };

            return defaults;
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(this, true);
            File.WriteAllText(Path, json);
        }

        #region Excludes

        [SerializeField]
        private List<string> _excludes;


        public List<string> Excludes
        {
            get => _excludes;
            set => _excludes = value;
        }

        #endregion
    }
}