using System.Collections.Generic;
using System;
using Demo.UI;

namespace Haare.Util.Prefab
{
    public class PrefabPath
    {
        //Canvas
        public static string CORE_CANVAS = "Prefabs/CoreCanvas";
        // DEMO Panel
        private static string DEBUG_PANEL = "Prefabs/HaareDebugPannel";
        private static string DEMO_TITLE_PANEL = "Prefabs/Demo_TitlePanel";
        private static string DEMO_LOADING_PANEL = "Prefabs/Demo_LoadingPanel";
        private static string DEMO_LOADINGFADE_PANEL = "Prefabs/Demo_LoadingFadePanel";
  
        // Panel
        private static string LOBBY_BASE_PANEL = "Prefabs/Lobby/LobbyBasePanel.prefab";
        private static string LOBBY_PANEL = "Prefabs/Lobby/LobbyPanel.prefab";
        private static string ITEM_PANEL = "Prefabs/Lobby/ItemPanel.prefab";

        
        
        public static Dictionary<Type, PrefabParam> PrefabDict = 
        new Dictionary<Type, PrefabParam>
        {
            { typeof(DebugPanel), new PrefabParam(DEBUG_PANEL) },
            { typeof(TitlePanel), new PrefabParam(DEMO_TITLE_PANEL) },
            { typeof(LoadingPanel), new PrefabParam(DEMO_LOADING_PANEL) },
            { typeof(LoadingFadePanel), new PrefabParam(DEMO_LOADINGFADE_PANEL) },
            
        };

        public static string SCENE_PATH = "Assets/Haare/Demo/Scene/";
        public static string SCENE_EXT = ".unity";
    }
}