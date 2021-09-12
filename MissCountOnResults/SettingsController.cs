using BeatSaberMarkupLanguage.Attributes;

namespace MissCountOnResults
{
    public class SettingsController : PersistentSingleton<SettingsController>
    {
        [UIValue("enable")]
        public bool Enable
        {
            get => PluginConfig.Instance.Enable;
            set => PluginConfig.Instance.Enable = value;
        }
    }
}
