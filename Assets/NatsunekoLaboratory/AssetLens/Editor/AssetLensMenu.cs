// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using UnityEditor;

namespace NatsunekoLaboratory.AssetLens
{
    public static class AssetLensMenu
    {
        private static AssetLensSettingsStore _settings;

        private static void LoadAssetLensSettings()
        {
            _settings = AssetLensSettingsStore.Load();
        }

        [MenuItem("Assets/Find References in Project", true)]
        [UsedImplicitly]
        private static bool IsEnableFindReferencesInProject()
        {
            return Selection.assetGUIDs != null && Selection.assetGUIDs.Length > 0;
        }

        [MenuItem("Assets/Find References in Project", false, 25)]
        [UsedImplicitly]
        private static void FindReferencesInProject()
        {
            if (_settings == null)
                LoadAssetLensSettings();

            var result = AssetLensProcessor.FindReferences(Selection.assetGUIDs.ToList(), _settings?.Excludes ?? new List<string>());
            if (result.Count > 0)
                AssetLensWindow.ShowWindow(result);
        }
    }
}