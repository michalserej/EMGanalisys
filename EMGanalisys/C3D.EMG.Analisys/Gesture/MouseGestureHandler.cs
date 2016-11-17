using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace C3D.EMG.Analisys.Gesture
{
    public class MouseGestureHandler : Component
    {
        private Control _control;
        private Boolean _autoRegister;
        private Boolean _isRegistered;
        private Boolean _isCaptured;
        private Int32 _lastX;
        private Int32 _lastY;
        private MouseButtons _supportButton;
        private MouseEventHandler _mouseDownHandler;
        private MouseEventHandler _mouseUpHandler;
        private MouseEventHandler _mouseMoveHandler;

        public event MouseGestureToLeft OnMouseGestureToLeft;
        public event MouseGestureToRight OnMouseGestureToRight;
        public event MouseGestureToTop OnMouseGestureToTop;
        public event MouseGestureToBottom OnMouseGestureToBottom;
        public event MouseEventHandler OnMouseGestureUp;

        [Browsable(false)]
        public Boolean IsRegistered
        {
            get { return this._isRegistered; }
        }

        public Boolean AutoRegister
        {
            get { return this._autoRegister; }
            set { this._autoRegister = value; }
        }

        public Control BaseControl
        {
            get { return this._control; }
            set
            {
                this.UnRegistreHandler();
                this._control = value;
                if (this._autoRegister) this.RegisterHandler();
            }
        }

        public MouseButtons SupportButton
        {
            get { return this._supportButton; }
            set { this._supportButton = value; }
        }

        public MouseGestureHandler()
        {
            this._autoRegister = true;
            this._isRegistered = false;
            this._supportButton = MouseButtons.Left;

            this._mouseDownHandler = new MouseEventHandler(this.control_MouseDown);
            this._mouseUpHandler = new MouseEventHandler(this.control_MouseUp);
            this._mouseMoveHandler = new MouseEventHandler(this.control_MouseMove);
        }

        public void RegisterHandler()
        {
            if (this._control != null && !this._isRegistered)
            {
                this._isRegistered = true;

                this._control.MouseDown += this._mouseDownHandler;
                this._control.MouseUp += this._mouseUpHandler;
                this._control.MouseMove += this._mouseMoveHandler;
            }
        }

        public void UnRegistreHandler()
        {
            if (this._control != null && this._isRegistered)
            {
                this._control.MouseDown -= this._mouseDownHandler;
                this._control.MouseUp -= this._mouseUpHandler;
                this._control.MouseMove -= this._mouseMoveHandler;

                this._isRegistered = false;
            }
        }

        private void control_MouseDown(object sender, MouseEventArgs e)
        {
            if (!this._isCaptured && e.Button == this._supportButton)
            {
                this._isCaptured = true;

                Point p = (this._control.Parent == null ? e.Location : this._control.Parent.PointToClient(this._control.PointToScreen(e.Location)));
                this._lastX = p.X;
                this._lastY = p.Y;
            }
        }

        private void control_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._isCaptured && e.Button == this._supportButton)
            {
                this._isCaptured = false;

                if (this.OnMouseGestureUp != null)
                {
                    this.OnMouseGestureUp(this, e);
                }
            }
        }

        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._isCaptured && e.Button == this._supportButton)
            {
                Point p = (this._control.Parent == null ? e.Location : this._control.Parent.PointToClient(this._control.PointToScreen(e.Location)));

                this.DoMouseGesture(p.X, p.Y);
                this._lastX = p.X;
                this._lastY = p.Y;
            }
        }

        private void DoMouseGesture(Int32 x, Int32 y)
        {
            MouseGesturedDirection direction = MouseGesturedDirection.None;
            Double dx = x - this._lastX;
            Double dy = y - this._lastY;
            Double delta = Math.Abs(dy / dx);

            if (delta < 0.50)
            {
                direction = (dx < 0 ? MouseGesturedDirection.Left : MouseGesturedDirection.Right);
            }
            else if (delta > 0.86)
            {
                direction = (dy < 0 ? MouseGesturedDirection.Top : MouseGesturedDirection.Bottom);
            }

            if (direction == MouseGesturedDirection.Left && this.OnMouseGestureToLeft != null)
            {
                this.OnMouseGestureToLeft(this, new MouseGestureEventArgs((Int32)dx));
            }
            else if (direction == MouseGesturedDirection.Right && this.OnMouseGestureToRight != null)
            {
                this.OnMouseGestureToRight(this, new MouseGestureEventArgs((Int32)dx));
            }
            else if (direction == MouseGesturedDirection.Top && this.OnMouseGestureToTop != null)
            {
                this.OnMouseGestureToTop(this, new MouseGestureEventArgs((Int32)dy));
            }
            else if (direction == MouseGesturedDirection.Bottom && this.OnMouseGestureToBottom != null)
            {
                this.OnMouseGestureToBottom(this, new MouseGestureEventArgs((Int32)dy));
            }
            else if (direction == MouseGesturedDirection.None && dx < 0 && dy < 0 && this.OnMouseGestureToLeft != null && this.OnMouseGestureToTop != null)//左上移动
            {
                this.OnMouseGestureToLeft(this, new MouseGestureEventArgs((Int32)dx));
                this.OnMouseGestureToTop(this, new MouseGestureEventArgs((Int32)dy));
            }
            else if (direction == MouseGesturedDirection.None && dx > 0 && dy > 0 && this.OnMouseGestureToRight != null && this.OnMouseGestureToBottom != null)//右下移动
            {
                this.OnMouseGestureToRight(this, new MouseGestureEventArgs((Int32)dx));
                this.OnMouseGestureToBottom(this, new MouseGestureEventArgs((Int32)dy));
            }
        }
    }
}