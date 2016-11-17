using System;

namespace C3D.EMG.Analisys.Gesture
{
    public class MouseGestureEventArgs : EventArgs
    {
        private Int32 _delta;

        public Int32 Delta
        {
            get { return this._delta; }
        }

        public MouseGestureEventArgs(Int32 delta)
        {
            this._delta = delta;
        }
    }
}