using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using PlayerInfoViewer.Configuration;
using System;
using System.Globalization;
using Zenject;

namespace PlayerInfoViewer.Views
{
    public class PlayerInfoSettingView : IInitializable, IDisposable
    {
        private bool _disposedValue;
        public static readonly string _buttonName = "PlayerInfoViewer";
        public string ResourceName => string.Join(".", this.GetType().Namespace, this.GetType().Name);
        public void Initialize()
        {
            BSMLSettings.instance.AddSettingsMenu(_buttonName, this.ResourceName, this);
        }
        public virtual void Dispose()
        {
            if (this._disposedValue)
                return;
            BSMLSettings.instance?.RemoveSettingsMenu(_buttonName);
            this._disposedValue = true;
        }
        [UIValue("DateChangeTime")]
        public int DateChangeTime
        {
            get => PluginConfig.Instance.DateChangeTime;
            set => PluginConfig.Instance.DateChangeTime = value;
        }
        [UIValue("IntervalTime")]
        public int IntervalTime
        {
            get => PluginConfig.Instance.IntervalTime;
            set => PluginConfig.Instance.IntervalTime = value;
        }
        [UIValue("ViewRankPP")]
        public bool ViewRankPP
        {
            get => PluginConfig.Instance.ViewRankPP;
            set => PluginConfig.Instance.ViewRankPP = value;
        }
        [UIValue("ViewPlayCount")]
        public bool ViewPlayCount
        {
            get => PluginConfig.Instance.ViewPlayCount;
            set => PluginConfig.Instance.ViewPlayCount = value;
        }
        [UIValue("ViewPlayerStatistics")]
        public bool ViewPlayerStatistics
        {
            get => PluginConfig.Instance.ViewPlayerStatistics;
            set => PluginConfig.Instance.ViewPlayerStatistics = value;
        }
        [UIValue("ViewFontSize")]
        public int ViewFontSize
        {
            get => (int)PluginConfig.Instance.ViewFontSize;
            set => PluginConfig.Instance.ViewFontSize = (float)value;
        }
        [UIValue("ViewYoffset")]
        public float ViewYoffset
        {
            get => PluginConfig.Instance.ViewYoffset;
            set => PluginConfig.Instance.ViewYoffset = value;
        }
        [UIAction("TimeFormatter")]
        private string TimeFormatter(int value)
        {
            return $"{value.ToString("F0", CultureInfo.InvariantCulture)} hour";
        }
    }
}
