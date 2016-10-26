﻿namespace PlayFab.Editor
{
    using UnityEditor;
    using UnityEngine;
    using System.Collections;


    public class TitleInternalDataEditor : EditorWindow {
        public string key = string.Empty;
        public string Value = string.Empty;

        public string displayTitle = "";
        public Vector2 scrollPos = Vector2.zero;

        
        void OnGUI () {
            // The actual window code goes here
            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                GUILayout.Label(string.Format("Editing: {0}", key), PlayFabEditorHelper.uiStyle.GetStyle("orTitle"), GUILayout.MinWidth(EditorGUIUtility.currentViewWidth));
            EditorGUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
            Value = GUILayout.TextArea(Value, PlayFabEditorHelper.uiStyle.GetStyle("editTxt"));
            GUILayout.EndScrollView();


            EditorGUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Save",  PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MaxWidth(200)))
                {
                    for(int z = 0; z < PlayFabEditorDataMenu.tdInternalViewer.items.Count; z++)
                    {
                        if(PlayFabEditorDataMenu.tdInternalViewer.items[z].Key == key)
                        {
                        PlayFabEditorDataMenu.tdInternalViewer.items[z].Value = Value;
                        PlayFabEditorDataMenu.tdInternalViewer.items[z].isDirty = true;
                        }
                    }
                    Close();

                }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            Repaint();
        }

        public void LoadData(string k, string v)
        {
            key = k;
            Value = v;
        }

    }
}