﻿using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using PlayFab.Editor.EditorModels;

namespace PlayFab.Editor
{
    public class PlayFabEditorToolsMenu : UnityEditor.Editor
    {
        public static float buttonWidth = 200;
        public static Vector2 scrollPos = Vector2.zero;

        public static void DrawToolsPanel()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, PlayFabEditorHelper.uiStyle.GetStyle("gpStyleGray1"));
                buttonWidth = EditorGUIUtility.currentViewWidth > 400 ? EditorGUIUtility.currentViewWidth/2 : 200;


                    GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                        GUILayout.Label("CLOUD SCRIPT:", PlayFabEditorHelper.uiStyle.GetStyle("labelStyle"));
                        GUILayout.Space(10);
                        if (GUILayout.Button("IMPORT", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(30)))
                        {
                            ImportCloudScript();
                        }
                        GUILayout.Space(10);
                        if(File.Exists(PlayFabEditorDataService.envDetails.localCloudScriptPath))
                        {
                            if (GUILayout.Button("REMOVE", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(30)))
                            {
                                PlayFabEditorDataService.envDetails.localCloudScriptPath = string.Empty;
                                PlayFabEditorDataService.SaveEnvDetails();
                            }
                            GUILayout.Space(10);
                            if (GUILayout.Button("EDIT", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinHeight(30)))
                            {
                                EditorUtility.OpenWithDefaultApp(PlayFabEditorDataService.envDetails.localCloudScriptPath);
                            }

                        }
                    GUILayout.EndHorizontal();


                    if(File.Exists(PlayFabEditorDataService.envDetails.localCloudScriptPath))
                    {
                        var path = File.Exists(PlayFabEditorDataService.envDetails.localCloudScriptPath) ? PlayFabEditorDataService.envDetails.localCloudScriptPath : PlayFabEditorHelper.CLOUDSCRIPT_PATH;
                        var shortPath = "..." + path.Substring(path.LastIndexOf('/'));

                        GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));

                            GUILayout.FlexibleSpace();
 
                            if (GUILayout.Button(shortPath, PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinWidth(110), GUILayout.MinHeight(20)))
                            {
                                EditorUtility.RevealInFinder(path);
                            }
//                            GUILayout.Space(10);
//                            if (GUILayout.Button("EDIT LOCALLY", PlayFabEditorHelper.uiStyle.GetStyle("textButton"), GUILayout.MinWidth(90), GUILayout.MinHeight(20)))
//                            {
//                                EditorUtility.OpenWithDefaultApp(path);
//                            }
                            GUILayout.FlexibleSpace();

                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("SAVE TO PLAYFAB", PlayFabEditorHelper.uiStyle.GetStyle("Button"), GUILayout.MinHeight(32), GUILayout.Width(buttonWidth)))
                            {
                                if(EditorUtility.DisplayDialog("Deployment Confirmation", "This action will upload your local Cloud Script changes to PlayFab?", "Continue", "Cancel"))
                                {
                                    BeginCloudScriptUpload();
                                }
                            }
                            GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                   }
                   else
                   {
                        GUILayout.BeginHorizontal(PlayFabEditorHelper.uiStyle.GetStyle("gpStyleClear"));
                            GUILayout.FlexibleSpace();
                                GUILayout.Label("No Cloud Script files added. Import your file to get started.", PlayFabEditorHelper.uiStyle.GetStyle("orTxt"));
                            GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                   }
         
            GUILayout.EndScrollView();
        }



        public static void ImportCloudScript()
        {
            int dialogResponse = EditorUtility.DisplayDialogComplex("Selcet an Import Option", "What Cloud Script file do you want to import?", "Use my latest PlayFab revision", "Cancel", "Use my local file");
            switch(dialogResponse)
            {
                case 0:
                    // use PlayFab
                    GetCloudScriptRevision();
                break;

                case 1:
                    // cancel
                    return;

                case 2: 
                    //use local
                    SelectLocalFile();
                    break;
            }

        }

        public static void GetCloudScriptRevision()
        {
            // empty request object gets latest versions
            PlayFabEditorApi.GetCloudScriptRevision(new EditorModels.GetCloudScriptRevisionRequest(), (GetCloudScriptRevisionResult result) => {

                var csPath = PlayFabEditorHelper.CLOUDSCRIPT_PATH; 

                try
                {
                    System.IO.File.WriteAllText(csPath, result.Files.First().FileContents);
                    Debug.Log("CloudScript uploaded successfully!");
                    PlayFabEditorDataService.envDetails.localCloudScriptPath = csPath;
                    PlayFabEditorDataService.SaveEnvDetails();
                }
                catch (Exception ex)
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
                    return;
                }

            }, PlayFabEditorHelper.SharedErrorCallback);
        }



        public static void SelectLocalFile()
        {
            
            string starterPath = File.Exists(PlayFabEditorDataService.envDetails.localCloudScriptPath) ? Application.dataPath : PlayFabEditorDataService.envDetails.localCloudScriptPath; 
            string cloudScriptPath =string.Empty;

            try
            {
                cloudScriptPath = EditorUtility.OpenFilePanel("Select your Cloud Script file", starterPath, "js");
            }
            catch (Exception ex)
            {
               PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, ex.Message);
            }

            if(!string.IsNullOrEmpty(cloudScriptPath))
            {
                PlayFabEditorDataService.envDetails.localCloudScriptPath = cloudScriptPath;
                PlayFabEditorDataService.SaveEnvDetails();
            }
        }



        public static void BeginCloudScriptUpload()
        {
            string filePath = File.Exists(PlayFabEditorDataService.envDetails.localCloudScriptPath) ? PlayFabEditorDataService.envDetails.localCloudScriptPath : PlayFabEditorHelper.CLOUDSCRIPT_PATH;

            if(!File.Exists(PlayFabEditorDataService.envDetails.localCloudScriptPath) && !File.Exists(PlayFabEditorHelper.CLOUDSCRIPT_PATH))
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, "Cloud Script Upload Failed: null or corrupt file at path(" + filePath + ").");
                return;
            }

            StreamReader s = File.OpenText(filePath);
            string contents = s.ReadToEnd();
            s.Close();

            UpdateCloudScriptRequest request = new UpdateCloudScriptRequest();
            request.Publish = EditorUtility.DisplayDialog("Deployment Options", "Do you want to make this Cloud Script live after uploading?", "Yes", "No");
            request.Files = new List<CloudScriptFile>(){ 
                new CloudScriptFile() { 
                    Filename = PlayFabEditorHelper.CLOUDSCRIPT_FILENAME,
                    FileContents = contents
                }
            };

            PlayFabEditorApi.UpdateCloudScript(request, (UpdateCloudScriptResult result) => {
                PlayFabEditorDataService.envDetails.localCloudScriptPath = filePath;
                PlayFabEditorDataService.SaveEnvDetails();

                Debug.Log("CloudScript uploaded successfully!");

            }, PlayFabEditorHelper.SharedErrorCallback);

        }
    }





}
