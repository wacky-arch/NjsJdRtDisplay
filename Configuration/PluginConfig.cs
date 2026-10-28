using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace NjsJdRtDisplay.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        public const float DefaultPosX = 0.0f;
        public const float DefaultPosY = 2.35f;
        public const float DefaultPosZ = 4.0f;
        public const float DefaultRotX = 0.0f;
        public const float DefaultRotY = 0.0f;
        public const float DefaultRotZ = 0.0f;
        public const float DefaultScale = 1.0f;

        public virtual bool Enabled { get; set; } = true;

        public virtual float PosX { get; set; } = DefaultPosX;
        public virtual float PosY { get; set; } = DefaultPosY;
        public virtual float PosZ { get; set; } = DefaultPosZ;

        public virtual float RotX { get; set; } = DefaultRotX;
        public virtual float RotY { get; set; } = DefaultRotY;
        public virtual float RotZ { get; set; } = DefaultRotZ;

        public virtual float Scale { get; set; } = DefaultScale;

        public virtual int DecimalPlaces { get; set; } = 1;

        public virtual bool ShowHandleOnPause { get; set; } = true;
        public virtual bool AlwaysShowHandle { get; set; } = false;

        public event Action ConfigChangedEvent;

        public Vector3 GetPosition() => new Vector3(PosX, PosY, PosZ);

        public Quaternion GetRotation() => Quaternion.Euler(RotX, RotY, RotZ);

        public void SetTransform(Vector3 position, Quaternion rotation)
        {
            PosX = (float)Math.Round(position.x, 2);
            PosY = (float)Math.Round(position.y, 2);
            PosZ = (float)Math.Round(position.z, 2);

            Vector3 euler = rotation.eulerAngles;
            RotX = (float)Math.Round(NormalizeAngle(euler.x), 1);
            RotY = (float)Math.Round(NormalizeAngle(euler.y), 1);
            RotZ = (float)Math.Round(NormalizeAngle(euler.z), 1);

            Changed();
        }

        public void ResetPosition()
        {
            PosX = DefaultPosX;
            PosY = DefaultPosY;
            PosZ = DefaultPosZ;
            RotX = DefaultRotX;
            RotY = DefaultRotY;
            RotZ = DefaultRotZ;
            Scale = DefaultScale;
            Changed();
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

        public virtual void OnReload()
        {
        }

        public virtual void Changed()
        {
            ConfigChangedEvent?.Invoke();
        }

        public virtual void CopyFrom(PluginConfig other)
        {
            Enabled = other.Enabled;
            PosX = other.PosX;
            PosY = other.PosY;
            PosZ = other.PosZ;
            RotX = other.RotX;
            RotY = other.RotY;
            RotZ = other.RotZ;
            Scale = other.Scale;
            DecimalPlaces = other.DecimalPlaces;
            ShowHandleOnPause = other.ShowHandleOnPause;
            AlwaysShowHandle = other.AlwaysShowHandle;
        }
    }
}
