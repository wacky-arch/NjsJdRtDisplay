using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.FloatingScreen;
using NjsJdRtDisplay.Configuration;
using TMPro;
using UnityEngine;

namespace NjsJdRtDisplay.UI
{
    internal class NjsJdRtFloatingPanel : IDisposable
    {
        private const string ResourceName = "NjsJdRtDisplay.UI.Views.NjsJdRtFloatingView.bsml";
        private const float BaseScale = 0.02f;
        private const float UniformFontSize = 6.5f;

        private static readonly Color NjsColor = Color.white;
        private static readonly Color JdGreenColor = new Color(0.25f, 1.0f, 0.40f, 1.0f);
        private static readonly Color RtBrightPurpleColor = new Color(0.85f, 0.47f, 1.0f, 1.0f);

        private FloatingScreen _floatingScreen;

        [UIComponent("njs-text")]
        private readonly TextMeshProUGUI _njsText = null;

        [UIComponent("jd-text")]
        private readonly TextMeshProUGUI _jdText = null;

        [UIComponent("rt-text")]
        private readonly TextMeshProUGUI _rtText = null;

        private float _lastDisplayedNjs = float.MinValue;
        private float _lastDisplayedJd = float.MinValue;
        private int _lastDisplayedRt = int.MinValue;
        private int _lastDecimalPlaces = -1;

        public FloatingScreen Screen => _floatingScreen;

        public void Create(bool showHandle)
        {
            if (_floatingScreen != null)
            {
                return;
            }

            var config = PluginConfig.Instance;
            _floatingScreen = FloatingScreen.CreateFloatingScreen(
                new Vector2(64f, 22f),
                createHandle: true,
                position: config.GetPosition(),
                rotation: config.GetRotation());

            _floatingScreen.gameObject.name = "NjsJdRtDisplayFloatingScreen";
            _floatingScreen.HandleSide = FloatingScreen.Side.Bottom;
            _floatingScreen.ShowHandle = showHandle;
            _floatingScreen.HighlightHandle = true;
            _floatingScreen.HandleReleased += OnHandleReleased;

            string bsmlContent = ReadEmbeddedResource(ResourceName);
            BSMLParser.Instance.Parse(bsmlContent, _floatingScreen.gameObject, this);

            ConfigureTextComponent(_njsText, TextAlignmentOptions.Center, NjsColor, 58f);
            ConfigureTextComponent(_jdText, TextAlignmentOptions.Right, JdGreenColor, 28f);
            ConfigureTextComponent(_rtText, TextAlignmentOptions.Left, RtBrightPurpleColor, 28f);

            ApplyTransformFromConfig();
        }

        private static void ConfigureTextComponent(TextMeshProUGUI text, TextAlignmentOptions alignment, Color color, float width)
        {
            if (text == null)
            {
                return;
            }

            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;
            text.alignment = alignment;
            text.fontSize = UniformFontSize;
            text.color = color;
            text.rectTransform.sizeDelta = new Vector2(width, 9.5f);
        }

        public void SetShowHandle(bool showHandle)
        {
            if (_floatingScreen != null)
            {
                _floatingScreen.ShowHandle = showHandle;
            }
        }

        public void SetActive(bool active)
        {
            if (_floatingScreen != null && _floatingScreen.gameObject != null)
            {
                _floatingScreen.gameObject.SetActive(active);
            }
        }

        public void ApplyTransformFromConfig()
        {
            if (_floatingScreen == null)
            {
                return;
            }

            var config = PluginConfig.Instance;
            _floatingScreen.ScreenPosition = config.GetPosition();
            _floatingScreen.ScreenRotation = config.GetRotation();

            float scale = BaseScale * Mathf.Clamp(config.Scale, 0.2f, 5.0f);
            _floatingScreen.transform.localScale = new Vector3(scale, scale, scale);
        }

        public void UpdateValues(float njs, float jd, float halfJumpDurationSeconds, bool force = false)
        {
            if (_floatingScreen == null)
            {
                return;
            }

            var config = PluginConfig.Instance;
            int decimals = Mathf.Clamp(config.DecimalPlaces, 0, 3);
            float factor = decimals == 0 ? 1f : (decimals == 1 ? 10f : (decimals == 2 ? 100f : 1000f));

            float roundedNjs = Mathf.Round(njs * factor) / factor;
            float roundedJd = Mathf.Round(jd * factor) / factor;

            float rtSeconds = halfJumpDurationSeconds > 0f
                ? halfJumpDurationSeconds
                : (njs > 0.001f ? jd / (2f * njs) : 0f);
            int rtMs = Mathf.RoundToInt(rtSeconds * 1000f);

            bool decimalsChanged = decimals != _lastDecimalPlaces;
            string format = decimals switch
            {
                0 => "0",
                1 => "0.0",
                2 => "0.00",
                _ => "0.000"
            };

            if (force || decimalsChanged || !Mathf.Approximately(roundedNjs, _lastDisplayedNjs))
            {
                _lastDisplayedNjs = roundedNjs;
                if (_njsText != null)
                {
                    _njsText.text = $"NJS {roundedNjs.ToString(format, CultureInfo.InvariantCulture)}";
                }
            }

            if (force || decimalsChanged || !Mathf.Approximately(roundedJd, _lastDisplayedJd))
            {
                _lastDisplayedJd = roundedJd;
                if (_jdText != null)
                {
                    _jdText.text = $"JD {roundedJd.ToString(format, CultureInfo.InvariantCulture)}";
                }
            }

            if (force || decimalsChanged || rtMs != _lastDisplayedRt)
            {
                _lastDisplayedRt = rtMs;
                if (_rtText != null)
                {
                    _rtText.text = $"RT {rtMs}";
                }
            }

            _lastDecimalPlaces = decimals;
        }

        private void OnHandleReleased(object sender, FloatingScreenHandleEventArgs args)
        {
            PluginConfig.Instance.SetTransform(args.Position, args.Rotation);
        }

        private static string ReadEmbeddedResource(string resourcePath)
        {
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                throw new InvalidOperationException($"Embedded resource not found: {resourcePath}");
            }

            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public void Dispose()
        {
            if (_floatingScreen != null)
            {
                _floatingScreen.HandleReleased -= OnHandleReleased;
                if (_floatingScreen.gameObject != null)
                {
                    UnityEngine.Object.Destroy(_floatingScreen.gameObject);
                }
                _floatingScreen = null;
            }
        }
    }
}
