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
        public ConfigEntry<JudgementCounterManager.DisplayPosition> DisplayPosition { get; set; }
        public ConfigEntry<Color> ColorPerfect { get; set; }
        public ConfigEntry<Color> ColorNice { get; set; }
        public ConfigEntry<Color> ColorOk { get; set; }
        public ConfigEntry<Color> ColorMeh { get; set; }
        public ConfigEntry<Color> ColorNasty { get; set; }
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

            DisplayPosition = config.Bind("Settings", "DisplayPosition", JudgementCounterManager.DisplayPosition.TopLeft, "Judgement Display Position.");
            ColorPerfect = config.Bind("Hex Colors", "Perfecto Color", new Color(0, 1, 1), "Hex for Perfectos.");
            ColorNice = config.Bind("Hex Colors", "Nice Color", new Color(0, 1, 0), "Hex for Nices.");
            ColorOk = config.Bind("Hex Colors", "OK Color", new Color(1, 1, 0), "Hex for OKs.");
            ColorMeh = config.Bind("Hex Colors", "Meh Color", new Color(1, .5f, 0), "Hex for Mehs.");
            ColorNasty = config.Bind("Hex Colors", "Nasty Color", new Color(1, 0, 0), "Hex for Nastys.");

            settingPage = TootTallySettingsManager.AddNewPage("Judgement Counter", "Judgement Counter", 40f, new Color(0, 0, 0, 0));
            if (settingPage != null)
            {
                // Display Position Dropdown
                settingPage.AddLabel("Display Position");
                settingPage.AddDropdown("Display Position", DisplayPosition);

                // Judgement Hex Codes Headers and Fields
                settingPage.AddLabel("Perfecto Color Hex");
                settingPage.AddColorSliders("Perfecto Color", "Perfecto Color", ColorPerfect);
                settingPage.AddLabel("Nice Color Hex");
                settingPage.AddColorSliders("Nice Color Hex", "Nice Color", ColorNice);
                settingPage.AddLabel("OK Color Hex");
                settingPage.AddColorSliders("OK Color Hex", "OK Color", ColorOk);
                settingPage.AddLabel("Meh Color Hex");
                settingPage.AddColorSliders("Meh Color Hex", "Meh Color", ColorMeh);
                settingPage.AddLabel("Nasty Color Hex");
                settingPage.AddColorSliders("Nasty Color Hex", "Nasty Color", ColorNasty);
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