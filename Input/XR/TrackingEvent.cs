﻿using System;

namespace UnityEngine.InputNew
{
    public class TrackingEvent : InputEvent
    {
        // must mirror UnityVRTrackedNodeFields
        [Flags]
        public enum Flags
        {
            None = 0x00000000,
            PositionAvailable = 0x00000001,
            OrientationAvailable = 0x00000002,
            VelocityAvailable = 0x00000004,
            AngularVelocityAvailable = 0x00000008,
            AccelerationAvailable = 0x00000010,
            AngularAccelerationAvailable = 0x00000020
        };

        public int nodeId { get; set; }
        public Vector3 localPosition { get; set; }
        public Quaternion localRotation { get; set; }

        public Vector3 velocity { get; set; }
        public Vector3 angularVelocity { get; set; }

        public Vector3 acceleration { get; set; }
        public Vector3 angularAcceleration { get; set; }

        public Flags availableFields { get; set; }
    }
}
