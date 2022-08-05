﻿namespace UTJ.UnityPlayerView
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    // UnityPlayerViewewKunの共通クラス
    // Katsumasa Kimura
    public class UnityPlayerView
    {
        

        public enum Command
        {
            Play,
            Stop,
            Pause,
        }


        /// <summary>
        /// UnityPlaerViewerKunのEditorから送信されるData
        /// </summary>
        [SerializeField]
        public struct EditorSendData
        {
            
            public UnityPlayerView.Command command;
            public int frameCount;
            public bool isUseAsyncGPUReadback;

        }
        public static readonly System.Guid kMsgSendEditorToPlayer = new System.Guid("19bd0211c24b41cf83b68f8518ba4337");
        public static readonly System.Guid kMsgSendPlayerToEditor = new System.Guid("438b30091d444d979248d9db1a672272");
        public static readonly System.Guid kMsgSendPlayerToEditorHeader = new System.Guid("cc5014cc9b924611a01a68e0530c524c");

    }
}
