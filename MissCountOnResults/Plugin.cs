using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace MissCountOnResults
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        public static string PluginName => "MissCountOnResults";
        internal static Harmony harmony;
        string harmonyId = "com.github.rakkyo150.MissCountOnResults";

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger,Config cfgProvider)
        {
            Instance = this;
            Log = logger;
            PluginConfig.Instance = cfgProvider.Generated<PluginConfig>();
            BSMLSettings.instance.AddSettingsMenu("MissCountOnResults", $"MissCountOnResults.Settings.bsml", SettingsController.instance);
            MissCountOnResultsMethods.InitializeRecords();
            Log.Info("MissCountOnResults initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            harmony = new Harmony(harmonyId);
            Log.Debug("Patching harmony...");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

            BS_Utils.Utilities.BSEvents.menuSceneLoaded += OnMenuSceneLoaded;
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("UnPatching harmony");
            harmony.UnpatchAll(harmonyId);

            BS_Utils.Utilities.BSEvents.menuSceneLoaded -= OnMenuSceneLoaded;
        }

        private void OnMenuSceneLoaded()
        {
            MissCountOnResultsMethods.ScanNewJson();
        }
    }
}
