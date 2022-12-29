// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;

using JetBrains.Annotations;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.AssetLens
{
    internal class AssetLensSettingsProvider : SettingsProvider
    {
        private ReorderableList _list;
        private AssetLensSettingsStore _settings;

        public AssetLensSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        [SettingsProvider]
        [UsedImplicitly]
        public static SettingsProvider CreateAssetLensSettingsProvider()
        {
            return new AssetLensSettingsProvider("Project/Editor/Asset Lens", SettingsScope.Project, new[] { "Assets", "Search", "References" });
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _settings = AssetLensSettingsStore.Load();

            var excludes = new List<string>(_settings.Excludes ?? new List<string>());

            _list = new ReorderableList(excludes, typeof(string), true, false, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Exclude file and/or directory patterns by glob"),
                drawNoneElementCallback = rect => EditorGUI.LabelField(rect, "No exclude directories/files in Asset Lens"),
                drawElementCallback = (rect, index, active, focus) =>
                {
                    // ReSharper disable once LocalVariableHidesMember
                    using (var scope = new EditorGUI.ChangeCheckScope())
                    {
                        var item = excludes[index];

                        const int controlGap = 4;

                        rect.width -= controlGap;

                        item = EditorGUI.TextField(rect, item);

                        if (scope.changed)
                        {
                            excludes[index] = item;

                            _settings.Excludes = excludes;
                            _settings.Save();
                        }
                    }
                },
                onAddCallback = _ => excludes.Add(""),
                onChangedCallback = _ =>
                {
                    _settings.Excludes = excludes;
                    _settings.Save();
                }
            };
        }

        public override void OnDeactivate()
        {
            _settings?.Save();
        }

        public override void OnGUI(string searchContext)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.inspectorFullWidthMargins))
            {
                EditorGUILayout.Space();

                _list.DoLayoutList();
            }
        }
    }
}