using System;
using System.Collections.Generic;

namespace C3D
{
    public sealed class C3DFrameCollection : List<C3DFrame>
    {
        public C3DPoint3DData Get3DPoint(Int32 frameIndex, Int32 pointIndex)
        {
            if (frameIndex < 0 || frameIndex >= this.Count)
            {
                throw new ArgumentOutOfRangeException("Frame index is INVALID!");
            }

            C3DFrame frame = this[frameIndex];

            if (pointIndex < 0 || pointIndex >= frame.Point3Ds.Length)
            {
                throw new ArgumentOutOfRangeException("Point index is INVALID!");
            }

            return frame.Point3Ds[pointIndex];
        }

        public C3DAnalogSamples GetAnalogSample(Int32 frameIndex, Int32 sampleIndex)
        {
            if (frameIndex < 0 || frameIndex >= this.Count)
            {
                throw new ArgumentOutOfRangeException("Frame index is INVALID!");
            }

            C3DFrame frame = this[frameIndex];

            if (sampleIndex < 0 || sampleIndex >= frame.AnalogSamples.Length)
            {
                throw new ArgumentOutOfRangeException("Sample index is INVALID!");
            }

            return frame.AnalogSamples[sampleIndex];
        }
    }
}