using BaboonAPI.Hooks.Initializer;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using TootTallyCore.Utils.TootTallyModules;
using TootTallySettings;
using UnityEngine;

namespace JudgementCounter
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("TootTallyCore", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("TootTallySettings", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin, ITootTallyModule
    {
        public static Plugin Instance;

        private const string CONFIG_NAME = "JudgementCounter.cfg";
        private Harmony _harmony;
        public ConfigEntry<bool> ModuleConfigEnabled { get; set; }
        public bool IsConfigInitialized { get; set; }

        //Config
        public ConfigEntry<string> DisplayPosition { get; set; }
        public ConfigEntry<string> ColorPerfect { get; set; }
        public ConfigEntry<string> ColorNice { get; set; }
        public ConfigEntry<string> ColorOk { get; set; }
        public ConfigEntry<string> ColorMeh { get; set; }
        public ConfigEntry<string> ColorNasty { get; set; }

        public string Name { get => PluginInfo.PLUGIN_NAME; set => Name = value; }

        public static TootTallySettingPage settingPage;

        public static void LogInfo(string msg) => Instance.Logger.LogInfo(msg);
        public static void LogError(string msg) => Instance.Logger.LogError(msg);

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            _harmony = new Harmony(Info.Metadata.GUID);

            GameInitializationEvent.Register(Info, TryInitialize);
        }

        private void TryInitialize()
        {
            ModuleConfigEnabled = TootTallyCore.Plugin.Instance.Config.Bind("Modules", "Judgement Counter", true, "Displays a live hit note window overlay during gameplay.");
            TootTallyModuleManager.AddModule(this);
            TootTallySettings.Plugin.Instance.AddModuleToSettingPage(this);
        }

        public void LoadModule()
        {
            string configPath = Path.Combine(Paths.BepInExRootPath, "config/");
            ConfigFile config = new ConfigFile(configPath + CONFIG_NAME, true) { SaveOnConfigSet = true };

            DisplayPosition = config.Bind("Settings", "DisplayPosition", "Top Left", "Judgement Display Position.");
            ColorPerfect = config.Bind("Hex Colors", "Perfecto Color", "00BFFF", "Hex for Perfectos.");
            ColorNice = config.Bind("Hex Colors", "Nice Color", "00FF00", "Hex for Nices.");
            ColorOk = config.Bind("Hex Colors", "OK Color", "FFFF00", "Hex for OKs.");
            ColorMeh = config.Bind("Hex Colors", "Meh Color", "FF8800", "Hex for Mehs.");
            ColorNasty = config.Bind("Hex Colors", "Nasty Color", "FF0000", "Hex for Nastys.");

            settingPage = TootTallySettingsManager.AddNewPage("Judgement Counter", "Judgement Counter", 40f, new Color(0, 0, 0, 0));
            if (settingPage != null)
            {
                // Display Position Dropdown
                settingPage.AddLabel("Display Position");
                settingPage.AddDropdown("Display Position", DisplayPosition, new string[] { "Top Left", "Bottom Left" });

                // Judgement Hex Codes Headers and Fields
                settingPage.AddLabel("Perfecto Color Hex");
                settingPage.AddTextField("Perfecto Color Hex", ColorPerfect.Value, false, (val) => ColorPerfect.Value = val);

                settingPage.AddLabel("Nice Color Hex");
                settingPage.AddTextField("Nice Color Hex", ColorNice.Value, false, (val) => ColorNice.Value = val);

                settingPage.AddLabel("OK Color Hex");
                settingPage.AddTextField("OK Color Hex", ColorOk.Value, false, (val) => ColorOk.Value = val);

                settingPage.AddLabel("Meh Color Hex");
                settingPage.AddTextField("Meh Color Hex", ColorMeh.Value, false, (val) => ColorMeh.Value = val);

                settingPage.AddLabel("Nasty Color Hex");
                settingPage.AddTextField("Nasty Color Hex", ColorNasty.Value, false, (val) => ColorNasty.Value = val);
            }

            _harmony.PatchAll(typeof(JudgementCounterManager));
            LogInfo($"Module loaded!");
        }

        public void UnloadModule()
        {
            _harmony.UnpatchSelf();
            settingPage?.Remove();
            LogInfo($"Module unloaded!");
        }
    }
}