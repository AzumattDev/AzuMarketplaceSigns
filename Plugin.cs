using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using PieceManager;
using ServerSync;
using UnityEngine;
using UnityEngine.Rendering;

namespace AzuMarketplaceSigns
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class AzuMarketplaceSignsPlugin : BaseUnityPlugin

    {
        internal const string ModName = "AzuMarketplaceSigns";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "Azumatt";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        
        private readonly Harmony _harmony = new(ModGUID);
        
        public static readonly ManualLogSource AzuMarketplaceSignsLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);
        
        private static readonly ConfigSync ConfigSync = new(ModGUID)
            { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        private void Awake()
        {
            _serverConfigLocked = config("General", "Force Server Config", true, "Force Server Config");
            
            _alchemy_sign_hover = config("HoverText", "Alchemy Sign Hover Text", "Alchemy Store", "Alchemy Sign Hover Text");
            _blacksmith_sign_hover = config("HoverText", "Blacksmith Sign Hover Text", "Blacksmith", "Blacksmith Sign Hover Text");
            _docking_sign_hover = config("HoverText", "Dock Sign Hover Text", "Dock", "Dock Sign Hover Text");
            _generalstore_sign_hover = config("HoverText", "General Store Sign Hover Text", "General Store", "General Store Sign Hover Text");
            _tavern_sign_hover = config("HoverText", "Tavern Sign Hover Text", "Tavern", "Tavern Sign Hover Text");
            _inn_sign_hover = config("HoverText", "Inn Sign Hover Text", "Inn", "Inn Sign Hover Text");
            
            
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            BuildPiece alchemy_sign = new("marketplacesigns", "Alchemy_sign");

            alchemy_sign.Name.English("Alchemy Sign"); 
            alchemy_sign.Description.English("Alchemy Sign");
            alchemy_sign.Category.Add(BuildPieceCategory.Furniture);
            alchemy_sign.RequiredItems.Add("FineWood", 5, true); 
            alchemy_sign.RequiredItems.Add("Iron", 5, true);
            alchemy_sign.RequiredItems.Add("Coal", 2, true);
            HoverText? alchemy_sign_text = alchemy_sign.Prefab.AddComponent<HoverText>();
            alchemy_sign_text.m_text = _alchemy_sign_hover.Value;

            
            BuildPiece blacksmith_sign = new("marketplacesigns", "Blacksmith_sign");

            blacksmith_sign.Name.English("Blacksmith Sign"); 
            blacksmith_sign.Description.English("Blacksmith Sign");
            blacksmith_sign.Category.Add(BuildPieceCategory.Furniture);
            blacksmith_sign.RequiredItems.Add("FineWood", 5, true); 
            blacksmith_sign.RequiredItems.Add("Iron", 5, true);
            blacksmith_sign.RequiredItems.Add("Coal", 2, true);
            HoverText? blacksmith_sign_text = blacksmith_sign.Prefab.AddComponent<HoverText>();
            blacksmith_sign_text.m_text = _blacksmith_sign_hover.Value;

            BuildPiece dock_sign = new("marketplacesigns", "Docking_sign");

            dock_sign.Name.English("Docking Sign"); 
            dock_sign.Description.English("Docking Sign");
            dock_sign.Category.Add(BuildPieceCategory.Furniture);
            dock_sign.RequiredItems.Add("FineWood", 5, true); 
            dock_sign.RequiredItems.Add("Iron", 5, true);
            dock_sign.RequiredItems.Add("Coal", 2, true);
            HoverText? dock_sign_text = dock_sign.Prefab.AddComponent<HoverText>();
            dock_sign_text.m_text = _docking_sign_hover.Value;

            
            BuildPiece generalstore_sign = new("marketplacesigns", "GeneralStore_sign");

            generalstore_sign.Name.English("General Store Sign"); 
            generalstore_sign.Description.English("General Store Sign");
            generalstore_sign.Category.Add(BuildPieceCategory.Furniture);
            generalstore_sign.RequiredItems.Add("FineWood", 5, true); 
            generalstore_sign.RequiredItems.Add("Iron", 5, true);
            generalstore_sign.RequiredItems.Add("Coal", 2, true);
            HoverText? generalstore_sign_text = generalstore_sign.Prefab.AddComponent<HoverText>();
            generalstore_sign_text.m_text = _generalstore_sign_hover.Value;

            
            BuildPiece tavern_sign = new("marketplacesigns", "Tavern_sign");

            tavern_sign.Name.English("Tavern Sign"); 
            tavern_sign.Description.English("Tavern Sign");
            tavern_sign.Category.Add(BuildPieceCategory.Furniture);
            tavern_sign.RequiredItems.Add("FineWood", 5, true); 
            tavern_sign.RequiredItems.Add("Iron", 5, true);
            tavern_sign.RequiredItems.Add("Coal", 2, true);
            HoverText? tavern_sign_text = tavern_sign.Prefab.AddComponent<HoverText>();
            tavern_sign_text.m_text = _tavern_sign_hover.Value;
            
            BuildPiece inn_sign = new("marketplacesigns", "Inn_sign");

            inn_sign.Name.English("Inn Sign"); 
            inn_sign.Description.English("Inn Sign");
            inn_sign.Category.Add(BuildPieceCategory.Furniture);
            inn_sign.RequiredItems.Add("FineWood", 5, true); 
            inn_sign.RequiredItems.Add("Iron", 5, true);
            inn_sign.RequiredItems.Add("Coal", 2, true);
            HoverText? inn_sign_text = inn_sign.Prefab.AddComponent<HoverText>();
            inn_sign_text.m_text = _inn_sign_hover.Value;




            _harmony.PatchAll();
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
        }
        
        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                AzuMarketplaceSignsLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                AzuMarketplaceSignsLogger.LogError($"There was an issue loading your {ConfigFileName}");
                AzuMarketplaceSignsLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<bool>? _serverConfigLocked;
        private static ConfigEntry<string>? _alchemy_sign_hover;
        private static ConfigEntry<string>? _blacksmith_sign_hover;
        private static ConfigEntry<string>? _docking_sign_hover;
        private static ConfigEntry<string>? _generalstore_sign_hover;
        private static ConfigEntry<string>? _tavern_sign_hover;
        private static ConfigEntry<string>? _inn_sign_hover;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            public bool? Browsable = false;
        }
        
        class AcceptableShortcuts : AcceptableValueBase  // Used for KeyboardShortcut Configs 
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", KeyboardShortcut.AllKeyCodes);
        }

        #endregion
    }
}   