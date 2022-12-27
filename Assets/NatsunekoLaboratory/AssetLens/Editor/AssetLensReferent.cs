// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace NatsunekoLaboratory.AssetLens
{
    internal class AssetLensReferent
    {
        private readonly Object _asset;
        private readonly Texture _icon;
        private readonly string _name;
        private readonly List<AssetLensReference> _references;

        public GUIContent Content { get; private set; }

        public bool IsShowFoldOut { get; set; }

        public IReadOnlyCollection<AssetLensReference> References => _references.AsReadOnly();

        public bool HasReferences => References.Count > 0;

        public AssetLensReferent(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            _asset = AssetDatabase.LoadMainAssetAtPath(path);
            _icon = AssetDatabase.GetCachedIcon(path);
            _name = _asset.name;

            _references = new List<AssetLensReference>();
            Content = new GUIContent($"{_name} <i>(No References)</i>", _icon);
        }

        public void AddRange(List<AssetLensReference> references)
        {
            _references.AddRange(references);

            Content = new GUIContent($"{_name} <i>({_references.Count} References)</i>", _icon);
        }

        public void Ping()
        {
            EditorGUIUtility.PingObject(_asset);
        }
    }
}