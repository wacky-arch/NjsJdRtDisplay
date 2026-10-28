using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;
using NjsJdRtDisplay.Configuration;
using Zenject;

namespace NjsJdRtDisplay.UI
{
    internal class NjsJdRtSettingsController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private const string ResourceName = "NjsJdRtDisplay.UI.Views.NjsJdRtSettingsView.bsml";
        private const string TabName = "NJS/JD/RT";

        private NjsJdRtFloatingPanel _previewPanel;
        private bool _menuPreview;

        public event PropertyChangedEventHandler PropertyChanged;

        [UIValue("Enabled")]
        public bool Enabled
        {
            get => PluginConfig.Instance.Enabled;
            set
            {
                if (PluginConfig.Instance.Enabled != value)
                {
                    PluginConfig.Instance.Enabled = value;
                    PluginConfig.Instance.Changed();
                    NotifyPropertyChanged();
                }
            }
        }

        [UIValue("MenuPreview")]
        public bool MenuPreview
        {
            get => _menuPreview;
            set
            {
                if (_menuPreview != value)
                {
                    _menuPreview = value;
                    UpdatePreviewPanel();
                    NotifyPropertyChanged();
                }
            }
        }

        [UIValue("ShowHandleOnPause")]
        public bool ShowHandleOnPause
        {
            get => PluginConfig.Instance.ShowHandleOnPause;
            set
            {
                if (PluginConfig.Instance.ShowHandleOnPause != value)
                {
                    PluginConfig.Instance.ShowHandleOnPause = value;
                    PluginConfig.Instance.Changed();
                    NotifyPropertyChanged();
                }
            }
        }

        [UIValue("Scale")]
        public float Scale
        {
            get => PluginConfig.Instance.Scale;
            set
            {
                if (Math.Abs(PluginConfig.Instance.Scale - value) > 0.0001f)
                {
                    PluginConfig.Instance.Scale = value;
                    PluginConfig.Instance.Changed();
                    NotifyPropertyChanged();
                }
            }
        }

        [UIValue("DecimalPlaces")]
        public int DecimalPlaces
        {
            get => PluginConfig.Instance.DecimalPlaces;
            set
            {
                if (PluginConfig.Instance.DecimalPlaces != value)
                {
                    PluginConfig.Instance.DecimalPlaces = value;
                    PluginConfig.Instance.Changed();
                    NotifyPropertyChanged();
                }
            }
        }

        [UIValue("PosX")]
        public float PosX
        {
            get => PluginConfig.Instance.PosX;
            set
            {
                if (Math.Abs(PluginConfig.Instance.PosX - value) > 0.0001f)
                {
                    PluginConfig.Instance.PosX = value;
                    PluginConfig.Instance.Changed();
                    NotifyPropertyChanged();
                }
            }
        }

        [UIValue("PosY")]
        public float PosY
        {
            get => PluginConfig.Instance.PosY;
            set
            {
                if (Math.Abs(PluginConfig.Instance.PosY - value) > 0.0001f)
                {
                    PluginConfig.Instance.PosY = value;
                    PluginConfig.Instance.Changed();
                    NotifyPropertyChanged();
                }
            }
        }

        [UIValue("PosZ")]
        public float PosZ
        {
            get => PluginConfig.Instance.PosZ;
            set
            {
                if (Math.Abs(PluginConfig.Instance.PosZ - value) > 0.0001f)
                {
                    PluginConfig.Instance.PosZ = value;
                    PluginConfig.Instance.Changed();
                    NotifyPropertyChanged();
                }
            }
        }

        [UIAction("ResetPosition")]
        public void ResetPosition()
        {
            PluginConfig.Instance.ResetPosition();
        }

        public void Initialize()
        {
            PluginConfig.Instance.ConfigChangedEvent += OnConfigChanged;
            GameplaySetup.Instance.AddTab(TabName, ResourceName, this, MenuType.All);
            BSMLSettings.Instance.AddSettingsMenu(TabName, ResourceName, this);
        }

        private void UpdatePreviewPanel()
        {
            if (_menuPreview)
            {
                if (_previewPanel == null)
                {
                    _previewPanel = new NjsJdRtFloatingPanel();
                    _previewPanel.Create(showHandle: true);
                }

                _previewPanel.SetActive(true);
                _previewPanel.SetShowHandle(true);
                _previewPanel.ApplyTransformFromConfig();
                _previewPanel.UpdateValues(20.0f, 17.5f, 0.4375f, force: true);
            }
            else
            {
                _previewPanel?.Dispose();
                _previewPanel = null;
            }
        }

        private void OnConfigChanged()
        {
            if (_previewPanel != null)
            {
                _previewPanel.ApplyTransformFromConfig();
                _previewPanel.UpdateValues(20.0f, 17.5f, 0.4375f, force: true);
            }

            NotifyPropertyChanged(nameof(Enabled));
            NotifyPropertyChanged(nameof(ShowHandleOnPause));
            NotifyPropertyChanged(nameof(Scale));
            NotifyPropertyChanged(nameof(DecimalPlaces));
            NotifyPropertyChanged(nameof(PosX));
            NotifyPropertyChanged(nameof(PosY));
            NotifyPropertyChanged(nameof(PosZ));
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            PluginConfig.Instance.ConfigChangedEvent -= OnConfigChanged;

            if (GameplaySetup.Instance != null)
            {
                GameplaySetup.Instance.RemoveTab(TabName);
            }

            if (BSMLSettings.Instance != null)
            {
                BSMLSettings.Instance.RemoveSettingsMenu(this);
            }

            _previewPanel?.Dispose();
            _previewPanel = null;
        }
    }
}
