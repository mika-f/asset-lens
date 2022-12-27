// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace NatsunekoLaboratory.AssetLens
{
    internal class AssetLensWindow : EditorWindow
    {
        private List<AssetLensReferent> _result;
        private Vector2 _scrollPosition = Vector2.zero;
        private GUIStyle _buttonStyle;
        private GUIStyle _foldoutStyle;

        public static void ShowWindow(List<AssetLensReferent> result)
        {
            var window = GetWindow<AssetLensWindow>("Asset Lens Result");
            window.SetResult(result);
        }

        private void SetResult(List<AssetLensReferent> result)
        {
            _result = result;
            _scrollPosition = Vector2.zero;

            _buttonStyle = new GUIStyle(EditorStyles.label)
            {
                fixedHeight = 18f,
                richText = true
            };

            _foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                richText = true
            };

            Repaint();
        }

        private void OnGUI()
        {
            if (_result == null || _result.Count == 0)
                return;

            var prev = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(Vector2.one * 16f);

            EditorGUILayout.LabelField("Results of references by selected assets");
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var referent in _result)
                DrawReference(referent);

            EditorGUILayout.EndScrollView();

            EditorGUIUtility.SetIconSize(prev);
        }

        private void DrawReference(AssetLensReferent referent)
        {
            if (referent.HasReferences)
            {
                referent.IsShowFoldOut = EditorGUILayout.Foldout(referent.IsShowFoldOut, referent.Content, _foldoutStyle);
                if (referent.IsShowFoldOut)
                    foreach (var reference in referent.References)
                        DrawReference(reference);
            }
            else
            {
                if (GUILayout.Button(referent.Content, _buttonStyle))
                    referent.Ping();
            }
        }

        private void DrawReference(AssetLensReference reference)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(28f);

                if (GUILayout.Button(reference.Content, _buttonStyle))
                    reference.Ping();
            }
        }
    }
}