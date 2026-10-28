using System;
using System.Reflection;
using NjsJdRtDisplay.Configuration;
using NjsJdRtDisplay.UI;
using UnityEngine;
using Zenject;

namespace NjsJdRtDisplay.Controllers
{
    internal class NjsJdRtGameplayController : IInitializable, ITickable, IDisposable
    {
        private static readonly FieldInfo NoteMovementField =
            typeof(NoteController).GetField("_noteMovement", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo MovementDataProviderField =
            typeof(NoteMovement).GetField("_variableMovementDataProvider", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly IVariableMovementDataProvider _movementDataProvider;
        private readonly BeatmapObjectManager _beatmapObjectManager;
        private readonly IGamePause _gamePause;

        private NjsJdRtFloatingPanel _panel;
        private bool _isPaused;

        private bool _hasNoteOverride;
        private float _overrideNjs;
        private float _overrideJd;
        private float _overrideHalfJumpDuration;

        public NjsJdRtGameplayController(
            [InjectOptional] IVariableMovementDataProvider movementDataProvider,
            [InjectOptional] BeatmapObjectManager beatmapObjectManager,
            [InjectOptional] IGamePause gamePause)
        {
            _movementDataProvider = movementDataProvider;
            _beatmapObjectManager = beatmapObjectManager;
            _gamePause = gamePause;
        }

        public void Initialize()
        {
            PluginConfig.Instance.ConfigChangedEvent += OnConfigChanged;

            if (_gamePause != null)
            {
                _gamePause.didPauseEvent += OnGamePaused;
                _gamePause.didResumeEvent += OnGameResumed;
            }

            if (_beatmapObjectManager != null)
            {
                _beatmapObjectManager.noteWasSpawnedEvent += OnNoteWasSpawned;
            }

            if (PluginConfig.Instance.Enabled)
            {
                EnsurePanelCreated();
                RefreshDisplay(force: true);
            }
        }

        private void EnsurePanelCreated()
        {
            if (_panel != null)
            {
                return;
            }

            _panel = new NjsJdRtFloatingPanel();
            bool showHandle = PluginConfig.Instance.AlwaysShowHandle ||
                              (_isPaused && PluginConfig.Instance.ShowHandleOnPause);
            _panel.Create(showHandle);
        }

        public void Tick()
        {
            if (!PluginConfig.Instance.Enabled || _panel == null || _movementDataProvider == null)
            {
                return;
            }

            if (_movementDataProvider.wasUpdatedThisFrame)
            {
                _hasNoteOverride = false;
            }

            RefreshDisplay(force: false);
        }

        private void RefreshDisplay(bool force)
        {
            if (_panel == null || _movementDataProvider == null)
            {
                return;
            }

            float njs;
            float jd;
            float halfJumpDuration;

            if (_hasNoteOverride)
            {
                njs = _overrideNjs;
                jd = _overrideJd;
                halfJumpDuration = _overrideHalfJumpDuration;
            }
            else
            {
                njs = _movementDataProvider.noteJumpSpeed;
                jd = _movementDataProvider.jumpDistance;
                halfJumpDuration = _movementDataProvider.halfJumpDuration;
            }

            _panel.UpdateValues(njs, jd, halfJumpDuration, force);
        }

        private void OnNoteWasSpawned(NoteController noteController)
        {
            if (noteController == null || _movementDataProvider == null)
            {
                return;
            }

            try
            {
                if (NoteMovementField?.GetValue(noteController) is NoteMovement noteMovement &&
                    MovementDataProviderField?.GetValue(noteMovement) is IVariableMovementDataProvider noteProvider)
                {
                    if (!ReferenceEquals(noteProvider, _movementDataProvider))
                    {
                        float noteNjs = noteProvider.noteJumpSpeed;
                        float noteJd = noteProvider.jumpDistance;
                        float noteHjd = noteProvider.halfJumpDuration;

                        bool differs =
                            Mathf.Abs(noteNjs - _movementDataProvider.noteJumpSpeed) > 0.001f ||
                            Mathf.Abs(noteJd - _movementDataProvider.jumpDistance) > 0.001f ||
                            Mathf.Abs(noteHjd - _movementDataProvider.halfJumpDuration) > 0.0005f;

                        if (differs)
                        {
                            _hasNoteOverride = true;
                            _overrideNjs = noteNjs;
                            _overrideJd = noteJd;
                            _overrideHalfJumpDuration = noteHjd;
                        }
                        else
                        {
                            _hasNoteOverride = false;
                        }
                    }
                    else
                    {
                        _hasNoteOverride = false;
                    }
                }
            }
            catch
            {
                // Ignore reflection errors if custom note types differ
            }
        }

        private void OnGamePaused()
        {
            _isPaused = true;
            UpdateHandleVisibility();
        }

        private void OnGameResumed()
        {
            _isPaused = false;
            UpdateHandleVisibility();
        }

        private void UpdateHandleVisibility()
        {
            if (_panel == null)
            {
                return;
            }

            bool showHandle = PluginConfig.Instance.AlwaysShowHandle ||
                              (_isPaused && PluginConfig.Instance.ShowHandleOnPause);
            _panel.SetShowHandle(showHandle);
        }

        private void OnConfigChanged()
        {
            if (!PluginConfig.Instance.Enabled)
            {
                _panel?.SetActive(false);
                return;
            }

            EnsurePanelCreated();
            _panel.SetActive(true);
            _panel.ApplyTransformFromConfig();
            UpdateHandleVisibility();
            RefreshDisplay(force: true);
        }

        public void Dispose()
        {
            PluginConfig.Instance.ConfigChangedEvent -= OnConfigChanged;

            if (_gamePause != null)
            {
                _gamePause.didPauseEvent -= OnGamePaused;
                _gamePause.didResumeEvent -= OnGameResumed;
            }

            if (_beatmapObjectManager != null)
            {
                _beatmapObjectManager.noteWasSpawnedEvent -= OnNoteWasSpawned;
            }

            _panel?.Dispose();
            _panel = null;
        }
    }
}
