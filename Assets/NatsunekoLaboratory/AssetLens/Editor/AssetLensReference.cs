// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using UnityEditor;

using UnityEngine;

namespace NatsunekoLaboratory.AssetLens
{
    internal class AssetLensReference
    {
        private readonly Object _asset;

        public GUIContent Content { get; }

        public AssetLensReference(string path)
        {
            _asset = AssetDatabase.LoadMainAssetAtPath(path);

            var icon = AssetDatabase.GetCachedIcon(path);
            var name = _asset.name;

            Content = new GUIContent(name, icon);
        }

        public void Ping()
        {
            EditorGUIUtility.PingObject(_asset);
        }
    }
}