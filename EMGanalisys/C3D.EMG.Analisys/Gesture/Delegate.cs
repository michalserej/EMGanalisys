using System;

namespace C3D.EMG.Analisys.Gesture
{
    public delegate void MouseGestureToLeft(Object sender, MouseGestureEventArgs e);

    public delegate void MouseGestureToRight(Object sender, MouseGestureEventArgs e);

    public delegate void MouseGestureToTop(Object sender, MouseGestureEventArgs e);

    public delegate void MouseGestureToBottom(Object sender, MouseGestureEventArgs e);
}