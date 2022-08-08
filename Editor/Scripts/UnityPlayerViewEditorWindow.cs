﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
#if UNITY_2020_1_OR_NEWER
using UnityEngine.Networking.PlayerConnection;
using PlayerConnectionUtility = UnityEditor.Networking.PlayerConnection.PlayerConnectionGUIUtility;
using PlayerConnectionGUILayout = UnityEditor.Networking.PlayerConnection.PlayerConnectionGUILayout;
#else
    using UnityEngine.Experimental.Networking.PlayerConnection;
    using PlayerConnectionUtility = UnityEditor.Experimental.Networking.PlayerConnection.EditorGUIUtility;
    using PlayerConnectionGUILayout = UnityEditor.Experimental.Networking.PlayerConnection.EditorGUILayout;
#endif
using System.Security.AccessControl;
using System;
using UnityEditor.Android;


namespace UTJ.UnityPlayerView.Editor
{
    // UnityPlayerViewerKunのEditorEditor側の処理
    // Katsumasa Kimura

    public class UnityPlayerViewKunEditorWindow : EditorWindow
    {
        [System.Serializable]
        public class Style
        {
            public static Texture2D GAME_VIEW_ICON = (Texture2D)EditorGUIUtility.Load("d_UnityEditor.GameView");
            public static Texture2D MERTO_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.Merto");
            public static Texture2D LUMIN_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.Lumin");
            public static Texture2D SWITCH_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.Switch");
            public static Texture2D PS4_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.PS4");
            public static Texture2D WEBGL_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.WebGL");
            public static Texture2D TVOS_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.tvOS");
            public static Texture2D IPHONE_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.iPhone");
            public static Texture2D ANDROID_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.Android");
            public static Texture2D STANDALONE_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.Standalone");
            public static Texture2D XBOXONE_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.XboxOne");
            public static Texture2D STADIA_ICON = (Texture2D)EditorGUIUtility.Load("d_BuildSettings.Stadia");

            public static readonly GUIContent PlayContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_PlayButton On@2x"),"Play / Stop");
            public static readonly GUIContent RecOnContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_Record On@2x"),"Stop Record");
            public static readonly GUIContent RecOffContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_Record Off@2x"),"Start Record");
            public static readonly GUIContent FolderContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_OpenedFolder Icon"),"Set Save Location");
            public static readonly GUIContent CaptureContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_SceneViewCamera@2x"),"Capture Screen");


            public static readonly GUIContent TouchEvent = new GUIContent("Enabled TouchEvent","Enabled Touch Event.(Android only)");
        }


        /// <summary>
        /// 変数の定義 
        /// </summary>

        
        static UnityPlayerViewKunEditorWindow window;        
        IConnectionState attachProfilerState;                
        UnityPlayerView.EditorSendData editorSendData;
        Texture2D playerViewTexture;
        


        [SerializeField] string recordPath;
        [SerializeField] bool isRecord;
        [SerializeField] int recordMaxFrame;
        [SerializeField] int recordCount;
        [SerializeField] bool isPlay;
        [SerializeField] UnityPlayerViewPlayer.TextureHeader textureHeader;
        [SerializeField] List<int> m_players;
        [SerializeField] bool m_EnabledTouchEvent = false;

        /// <summary>
        /// 関数の定義 
        /// </summary>
        [MenuItem("Window/UTJ/UnityPlayer View")]
        static void Create()
        {
            if (window == null)
            {
                window = (UnityPlayerViewKunEditorWindow)EditorWindow.GetWindow(typeof(UnityPlayerViewKunEditorWindow));
            }
            
            window.titleContent = new GUIContent(new GUIContent("Player View", Style.GAME_VIEW_ICON));
            window.wantsMouseMove = true;
            window.autoRepaintOnSceneChange = true;
            window.Show();
        }


        //
        // 基底クラスの関数のオーバーライド
        //        
        private void Awake()
        {            
            playerViewTexture = new Texture2D(2960, 1140, TextureFormat.RGBA32, false);
            editorSendData.frameCount = 1;
            recordMaxFrame = 200;
            m_players = new List<int>();
        }
       
        private void OnDestroy()
        {
            if (playerViewTexture == null)
            {
                DestroyImmediate(playerViewTexture);
            }
            window = null;        
        }


        //
        private void OnEnable()
        {
            if (window == null)
            {
                window = (UnityPlayerViewKunEditorWindow)EditorWindow.GetWindow(typeof(UnityPlayerViewKunEditorWindow));
            }
            if (attachProfilerState == null)
            {
#if UNITY_2020_1_OR_NEWER
                attachProfilerState = PlayerConnectionUtility.GetConnectionState(this);
#else
                attachProfilerState = PlayerConnectionUtility.GetAttachToPlayerState(this);
#endif
            }
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Initialize();
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Register(UnityPlayerView.kMsgSendPlayerToEditor, OnMessageEvent);
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Register(UnityPlayerView.kMsgSendPlayerToEditorHeader, OnMessageEventHeader);
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.RegisterConnection(OnConnection);
        }


        //
        private void OnDisable()
        {
            attachProfilerState.Dispose();
            attachProfilerState = null;            
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.DisconnectAll();
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Unregister(UnityPlayerView.kMsgSendPlayerToEditor, OnMessageEvent);
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Unregister(UnityPlayerView.kMsgSendPlayerToEditorHeader, OnMessageEventHeader);
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.UnregisterConnection(OnConnection);
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.UnregisterDisconnection(OnDisConnection);
        }


        //
        private void OnGUI()
        {
            //ChangeTitleContent();
            GUILayoutConnect();
            UnityEditor.EditorGUILayout.Separator();
            var rect = GUILayoutPlayView();

            if (m_EnabledTouchEvent)
            {
                TouchEventExec(rect);
            }
        }

        

        private void TouchEventExec(Rect rect)
        {
            if (playerViewTexture != null)
            {
                float scale = 0f;
                if (playerViewTexture.width > playerViewTexture.height)
                {
                    scale = rect.width / playerViewTexture.width;
                }
                else
                {
                    scale = rect.height / playerViewTexture.height;
                }
                var v2 = Event.current.mousePosition;
                var ofstx = 0;
                var ofsty = 0;
                if (rect.width > playerViewTexture.width * scale)
                {
                    ofstx = (int)((float)rect.width - playerViewTexture.width * scale) / 2;
                }
                if (rect.height > playerViewTexture.height * scale)
                {
                    ofsty = (int)((float)rect.height - playerViewTexture.height * scale) / 2;
                }
                var x = v2.x - (rect.x + ofstx);
                var y = v2.y - (rect.y + ofsty);
                x = x / scale;
                y = y / scale;
                x = Mathf.Max(x, 0);
                y = Mathf.Max(y, 0);
                x = Mathf.Min(x, playerViewTexture.width);
                y = Mathf.Min(y, playerViewTexture.height);

                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        {
                            ADB.GetInstance().Run(new string[] { "shell", "input", "touchscreen", "motionevent", "DOWN", $"{x}", $"{y}" }, "UnityPlayerView");
                        }
                        break;

                    case EventType.MouseMove:
                    case EventType.MouseDrag:
                        {
                            ADB.GetInstance().Run(new string[] { "shell", "input", "touchscreen", "motionevent", "MOVE", $"{x}", $"{y}" }, "UnityPlayerView");
                        }
                        break;

                    case EventType.MouseUp:
                        {
                            ADB.GetInstance().Run(new string[] { "shell", "input", "touchscreen", "motionevent", "UP", $"{x}", $"{y}" }, "UnityPlayerView");
                        }
                        break;
                    
                }
                if (Event.current.isKey)
                {
                    Debug.Log($"{Event.current.keyCode}");

                    switch (Event.current.keyCode)
                    {                        

                        case KeyCode.Home:
                            {
                                ADB.GetInstance().Run(new string[] { "shell", "input", "keyboard", "keyevent", "3" }, "UnityPlayerSync");
                            }
                            break;

                        case KeyCode.Escape:
                            {
                                ADB.GetInstance().Run(new string[] { "shell","input","keyboard","keyevent","4" }, "UnityPlayerSync");
                            }
                            break;

                        case KeyCode.Space:
                            ADB.GetInstance().Run(new string[] { "shell", "input", "keyboard", "keyevent", "62" }, "UnityPlayerSync");
                            break;

                        case KeyCode.A:
                            ADB.GetInstance().Run(new string[] { "shell", "input", "keyboard", "keyevent", "29" }, "UnityPlayerSync");
                            break;

                        case KeyCode.D:
                            ADB.GetInstance().Run(new string[] { "shell", "input", "keyboard", "keyevent", "32" }, "UnityPlayerSync");
                            break;

                        case KeyCode.S:
                            ADB.GetInstance().Run(new string[] { "shell", "input", "keyboard", "keyevent", "47" }, "UnityPlayerSync");
                            break;

                        case KeyCode.W:
                            ADB.GetInstance().Run(new string[] { "shell", "input", "keyboard", "keyevent", "51" }, "UnityPlayerSync");
                            break;
                    }
                }
            }
        }



        private void OnConnection(int playerId)
        {
            if (m_players.Contains(playerId) == false)
            {
                m_players.Add(playerId);
                Debug.Log("connected " + playerId);
            }
        }


        private void OnDisConnection(int playerId)
        {
            if (m_players.Contains(playerId) == true)
            {
                m_players.Remove(playerId);
                Debug.Log("disconnected " + playerId);
            }
        }



        //=========================================================================================
        // ユニーク関数の定義
        //=========================================================================================

        // ----------------------------------------------------------------------------------------
        // <summary>接続先のデバイスアイコンに切り替える </summary>
        // ----------------------------------------------------------------------------------------
        private void ChangeTitleContent()
        {                          
            window.titleContent = new GUIContent(new GUIContent("Player View", Style.GAME_VIEW_ICON));
        }


        private void OnMessageEventHeader(UnityEngine.Networking.PlayerConnection.MessageEventArgs args)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream(args.data);
            textureHeader = (UnityPlayerViewPlayer.TextureHeader)bf.Deserialize(ms);

        }


        // ----------------------------------------------------------------------------------------
        // <summary> メッセージ受信時のCB </summary>
        // ----------------------------------------------------------------------------------------
        private void OnMessageEvent(UnityEngine.Networking.PlayerConnection.MessageEventArgs args)
        {
            if (textureHeader != null)
            {
                if ((playerViewTexture == null) ||
                    (playerViewTexture.width != textureHeader.width) ||
                    (playerViewTexture.height != textureHeader.height) ||
                    (playerViewTexture.format != textureHeader.textureFormat)
                )
                {
                    if (playerViewTexture != null)
                    {
                        DestroyImmediate(playerViewTexture);
                    }
                    playerViewTexture = new Texture2D(
                        textureHeader.width,
                        textureHeader.height,
                        textureHeader.textureFormat,
                        textureHeader.mipChain);
                }

                byte[] raw;
                int slide = textureHeader.width * 4;
                // 画像データが上下反転しているケースがある
                if (textureHeader.flip)
                {
                    raw = new byte[args.data.Length];
                    for (var y = 0; y < textureHeader.height; y++)
                    {
                        var i1 = (textureHeader.height - (y + 1)) * slide;
                        var i2 = y * slide;
                        System.Array.Copy(args.data, i1, raw, i2, slide);
                    }
                }
                else
                {
                    raw = args.data;
                }
                playerViewTexture.LoadRawTextureData(raw);
                playerViewTexture.Apply();
                // EditorWidowを再描画
                if (window != null)
                {
                    window.Repaint();
                }
            }
        }


        // ----------------------------------------------------------------------------------------
        // <summary> UnityPlayerViewerKunPlayerへデータを送信する </summary>
        // ----------------------------------------------------------------------------------------
        private void SendMessage(object obj)
        {
            Debug.Log("SendMessage");
            var json = JsonUtility.ToJson(obj);
            var bytes = System.Text.Encoding.ASCII.GetBytes(json);
            EditorConnection.instance.Send(UnityPlayerView.kMsgSendEditorToPlayer, bytes);
        }


        // ----------------------------------------------------------------------------------------
        // <summary> 接続先選択GUIの描画
        // ----------------------------------------------------------------------------------------
        private void GUILayoutConnect()
        {
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.LabelField("Connect To", UnityEngine.GUILayout.ExpandWidth(false));
#if UNITY_2020_1_OR_NEWER
            PlayerConnectionGUILayout.ConnectionTargetSelectionDropdown(attachProfilerState, EditorStyles.toolbarDropDown);
#else
            PlayerConnectionGUILayout.AttachToPlayerDropdown(attachProfilerState, EditorStyles.toolbarDropDown);
#endif
            
            // Play
            {
                var contentSize = EditorStyles.label.CalcSize(Style.PlayContents);
                UnityEditor.EditorGUI.BeginChangeCheck();
                isPlay = UnityEngine.GUILayout.Toggle(isPlay, Style.PlayContents, EditorStyles.toolbarButton, UnityEngine.GUILayout.MaxWidth(contentSize.x + 10));
                if (UnityEditor.EditorGUI.EndChangeCheck())
                {
                    if (isPlay)
                    {
                        editorSendData.command = UnityPlayerView.Command.Play;                        
                    } 
                    else
                    {
                        editorSendData.command = UnityPlayerView.Command.Stop;
                    }
                    SendMessage(editorSendData);
                }
            }
            
            // Rec
            {
                UnityEditor.EditorGUI.BeginDisabledGroup(recordPath == null || recordPath.Length == 0);
                var contentSize = EditorStyles.label.CalcSize(Style.RecOnContents);
                UnityEditor.EditorGUI.BeginChangeCheck();
                isRecord = UnityEngine.GUILayout.Toggle(isRecord, isRecord ? Style.RecOnContents: Style.RecOffContents, EditorStyles.toolbarButton, UnityEngine.GUILayout.MaxWidth(contentSize.x + 10));
                if (UnityEditor.EditorGUI.EndChangeCheck())
                {
                    if (isRecord)
                    {
                        recordCount = 0;
                    }
                }
                UnityEditor.EditorGUI.EndDisabledGroup();
            }

            // Capture
            {                
                var contentSize = EditorStyles.label.CalcSize(Style.RecOnContents);
                if (UnityEngine.GUILayout.Button(Style.CaptureContents, EditorStyles.toolbarButton, UnityEngine.GUILayout.MaxWidth(contentSize.x + 10)))
                {
                    System.DateTime dt = System.DateTime.Now;
                    string result = dt.ToString("yyyyMMddHHmmss");
                    var path = EditorUtility.SaveFilePanel(
                        "Save texture as PNG",
                        "",
                        result + ".png",
                        "png");
                    if (path.Length != 0)
                    {
                        var pngData = playerViewTexture.EncodeToPNG();
                        if (pngData != null)
                            System.IO.File.WriteAllBytes(path, pngData);
                    }
                }             
            }

            // Save Folder
            {
                if (UnityEngine.GUILayout.Button(Style.FolderContents, EditorStyles.toolbarButton, UnityEngine.GUILayout.ExpandWidth(false)))
                {
                    if (isRecord == false)
                    {
                        recordPath = EditorUtility.SaveFolderPanel("Save textures to folder", "", "");
                    }
                }
            }

            UnityEngine.GUILayout.FlexibleSpace();
            UnityEditor.EditorGUILayout.EndHorizontal();            

            var playerCount = EditorConnection.instance.ConnectedPlayers.Count;
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine(string.Format("{0} players connected.", playerCount));
            int i = 0;
            foreach (var p in EditorConnection.instance.ConnectedPlayers)
            {
                builder.AppendLine(string.Format("[{0}] - {1} {2}", i++, p.name, p.playerId));
            }
            UnityEditor.EditorGUILayout.HelpBox(builder.ToString(), MessageType.Info);
        }


        // ----------------------------------------------------------------------------------------
        // <summary> PlayView GUIの描画 </summary>
        // ----------------------------------------------------------------------------------------
        private Rect GUILayoutPlayView()
        {
            if (recordPath == null || recordPath.Length == 0)
            {
                UnityEditor.EditorGUILayout.HelpBox("Please Set Record Folder.", MessageType.Info);
            }

#if UNITY_2019_1_OR_NEWER
            UnityEditor.EditorGUI.BeginDisabledGroup(isPlay);
            editorSendData.isUseAsyncGPUReadback = UnityEditor.EditorGUILayout.ToggleLeft("Enable Async GPU Readback", editorSendData.isUseAsyncGPUReadback);
            UnityEditor.EditorGUI.EndDisabledGroup();
#endif
            m_EnabledTouchEvent = EditorGUILayout.ToggleLeft(Style.TouchEvent, m_EnabledTouchEvent);
            UnityEditor.EditorGUI.BeginDisabledGroup(isPlay);
            editorSendData.frameCount = UnityEditor.EditorGUILayout.IntField("Reflesh Interval", editorSendData.frameCount, UnityEngine.GUILayout.ExpandWidth(false));
            editorSendData.frameCount = Math.Max(0, editorSendData.frameCount);
            UnityEditor.EditorGUI.EndDisabledGroup();
            UnityEditor.EditorGUILayout.LabelField(new GUIContent("Record Folder","Record and Capture Folder"),new GUIContent(recordPath));                        
            
            

            // ここでキャプチャ処理
            if (playerViewTexture != null && recordPath != null && recordPath.Length != 0 && recordCount < recordMaxFrame　&& isRecord){
                var pngData = playerViewTexture.EncodeToPNG();
                if (pngData != null){
                    var path = recordPath + "/" + recordCount.ToString("D4") + ".png";
                    System.IO.File.WriteAllBytes(path, pngData);
                    recordCount++;
                }
                if(recordCount >= recordMaxFrame){
                    isRecord = false;
                }
            }

            UnityEditor.EditorGUI.BeginDisabledGroup(isRecord);
            var tmp = UnityEditor.EditorGUILayout.IntField("Record Count Max", recordMaxFrame, UnityEngine.GUILayout.ExpandWidth(false));
            {
                recordMaxFrame = Math.Max(0, tmp);
                recordMaxFrame = Math.Min(99999, recordMaxFrame);
            }
            UnityEditor.EditorGUI.EndDisabledGroup();

            UnityEditor.EditorGUI.BeginDisabledGroup(isRecord);
            UnityEditor.EditorGUILayout.BeginHorizontal();
            UnityEditor.EditorGUI.BeginChangeCheck();
            var tmpFrame = UnityEditor.EditorGUILayout.IntSlider("Record Count", recordCount, 1, recordMaxFrame);
            if (UnityEditor.EditorGUI.EndChangeCheck())
            {
                if (isRecord == false && playerViewTexture != null)
                {
                    var fpath = recordPath + "/" + tmpFrame.ToString("D4") + ".png";
                    if (System.IO.File.Exists(fpath))
                    {
                        recordCount = tmpFrame;
                        var bytes = System.IO.File.ReadAllBytes(fpath);
                        playerViewTexture.LoadImage(bytes);
                    }
                }
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
            UnityEditor.EditorGUI.EndDisabledGroup();

            var r1 = UnityEditor.EditorGUILayout.GetControlRect();
            var r2 = new Rect(r1.x, r1.y, r1.width, position.height - (r1.y + r1.height) - 30.0f);
            // 描画
            if (playerViewTexture != null)
            {
                

                UnityEditor.EditorGUI.DrawPreviewTexture(
                    r2,
                    playerViewTexture,
                    null,
                    ScaleMode.ScaleToFit
                    );
               // GUILayoutUtility.GetRect(r2.width, r2.height);
            }
            return r2;
        }


        // ----------------------------------------------------------------------------------------
        // <summary> Int32をbyte arrayの途中に挿入する </summary>
        // ----------------------------------------------------------------------------------------
        int SetInt32ToBytes(int value, in byte[] dsts, int idx)
        {
            var bytes = System.BitConverter.GetBytes(value);
            for (var i = 0; i < bytes.Length; i++)
            {
                dsts[idx + i] = bytes[i];
            }
            return bytes.Length;
        }


#if !UNITY_2019_1_OR_NEWER
        public static bool HasOpenInstances<T>() where T : UnityEditor.EditorWindow
        {
            UnityEngine.Object[] wins = Resources.FindObjectsOfTypeAll(typeof(T));
            return wins != null && wins.Length > 0;
        }
#endif

    }
}