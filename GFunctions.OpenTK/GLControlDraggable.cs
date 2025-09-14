/*Licence Declaration:

This code uses the OpenTK library under the MIT License:

MIT License

Copyright (c) 2006-2019 Stefanos Apostolopoulos for the Open Toolkit project.

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software 
is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using OpenTK.Graphics.OpenGL;
using OpenTK.GLControl;
using OpenTK.Mathematics;

namespace GFunctions.OpenTK
{
    public class GLControlDraggable
    {
        private bool disposed = false; //used when form is closing to prevent calls

        private double[] _currentTrans = { 0, 0, 0 }; //XYZ coords
        private double[] _currentRot = { 0, 0 }; //Z, X rotation

        private double[] _finalTrans = { 0, 0, 0 };
        private double[] _finalRot = { 0, 0 };
        private double _finalZoom = 1.0;

        private bool _mousePressed = false;
        private double _mouseDelta = 0; //positive or negative depending on direction of wheel

        private double[] _mouseDown = { 0, 0 }; //X, Y
        private double[] _mouseCurrent = { 0, 0 }; //X, Y

        private double _rotMultiplier = 1;
        private double _transMultiplier = -0.1;
        private double _ZoomMultiplier = 0.02;

        //-------------- Public Properties ----------------
        public GLControl SubControl { get; private set; }

        public List<IGLDrawable> DrawnObjects = new List<IGLDrawable>();

        //-------------- Events  ----------------

        public delegate void viewUpdatedDel(object? sender, GLControlViewUpdatedEventArgs e);
        public event viewUpdatedDel? ViewUpdated;

        //-------------- Public subs  ----------------

        public GLControlDraggable(GLControl C)
        {
            this.SubControl = C;

            //add handlers for control objects

            C.MouseDown += this.GLControl_MouseDown;
            C.MouseMove += this.GLControl_MouseMove;
            C.MouseUp += this.GLControl_MouseUp;
            C.MouseWheel += this.GLControl_MouseWheel;
            C.Resize += this.GLControl_Resized;

            //update background color of form and refresh to display any inital objects

            GL.ClearColor(System.Drawing.Color.Black);
            this.Refresh();
        }

        public void Dispose()
        {
            this.disposed = true;
            SubControl.Dispose();
        }


        public void DragStarted(double X, double Y)
        {
            if (mouseIsOverControl(this.SubControl))
            {
                _mousePressed = true;
                _mouseDown[0] = X;
                _mouseDown[1] = Y;
            }
        }
        public void Dragged(double X, double Y)
        {
            if (_mousePressed)
            {
                _mouseCurrent[0] = X;
                _mouseCurrent[1] = Y;

                PrepCamera();
                SetOrientation_Drag();
            }
        }
        public void EndDrag()
        {
            _mousePressed = false;

            _finalTrans[0] += _currentTrans[0];
            _finalTrans[1] += _currentTrans[1];
            _finalTrans[2] += _currentTrans[2];
            _finalRot[0] += _currentRot[0];
            _finalRot[1] += _currentRot[1];
        }
        public void MouseWheeled(double Z)
        {
            if (mouseIsOverControl(this.SubControl))
            {
                _mouseDelta = Z;

                PrepCamera();
                SetOrientation_Drag();
            }
        }


        public void Refresh()
        {
            PrepCamera();

            if (_mousePressed == false)
            {
                SetOrientation_Manual(_finalTrans, _finalRot, _finalZoom);
            }
            else
            {
                SetOrientation_Drag(); //makes refreshes smooth for animation when mouse is being held down
            }

        } //used for re-drawing screen when orientation hasn't changed (new object added, etc)


        private void PrepCamera()
        {
            if (!disposed)
            {
                //First Clear Buffers
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Clear(ClearBufferMask.DepthBufferBit);

                //Basic Setup for viewing
                Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView((float)1.1, (float)(4 / 3.0), (float)1, (float)10000); // Perspective
                GL.MatrixMode(MatrixMode.Projection); //Load Perspective
                GL.LoadIdentity();
                GL.LoadMatrix(ref perspective);

                Matrix4 lookat = Matrix4.LookAt(100, 0, 0, 0, 0, 0, 0, 0, 1); //Setup camera

                GL.MatrixMode(MatrixMode.Modelview); //Load Camera
                GL.LoadIdentity();
                GL.LoadMatrix(ref lookat);

                GL.Viewport(0, 0, this.SubControl.Width, this.SubControl.Height); //Size of window
                GL.Enable(EnableCap.DepthTest); //Enable correct Z Drawings
                GL.DepthFunc(DepthFunction.Less); //Enable correct Z Drawings
            }
        } //step 1 
        private void SetOrientation_Drag()
        {
            if (!disposed)
            {
                _currentRot[0] = 0;
                _currentRot[1] = 0;
                _currentTrans[0] = 0;
                _currentTrans[1] = 0;
                _currentTrans[2] = 0;

                //------------- handle mousewheel ---------------

                if (_mouseDelta != 0)
                {
                    if (_mouseDelta > 0)
                    {
                        _finalZoom += _ZoomMultiplier;
                    }
                    else if (_mouseDelta < 0 & _finalZoom != _ZoomMultiplier)
                    {
                        _finalZoom -= _ZoomMultiplier;
                    }
                    _mouseDelta = 0; //actual zoom value updated below

                    if (_finalZoom <= 0)
                        _finalZoom = _ZoomMultiplier; //don't let zoom go negative
                }


                //------------- handle mousebutton ---------------

                if (_mousePressed)
                {
                    double deltaX = _mouseCurrent[0] - _mouseDown[0];
                    double deltaY = _mouseCurrent[1] - _mouseDown[1];

                    if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift)
                    {
                        //translate mode
                        _currentTrans[0] = deltaX * _transMultiplier;
                        _currentTrans[1] = deltaY * _transMultiplier;
                    }
                    else
                    {
                        //rotation mode
                        _currentRot[0] = deltaX * _rotMultiplier;
                        _currentRot[1] = deltaY * _rotMultiplier;
                    }
                }

                //perform actual orientation.. if nothing is updated stays at last position

                GL.Translate(0, -1 * (_finalTrans[0] + _currentTrans[0]), (_finalTrans[1] + _currentTrans[1]));
                GL.Rotate(_finalRot[0] + _currentRot[0], 0, 0, 1);
                GL.Rotate(_finalRot[1] + _currentRot[1], 0, 1, 0);
                GL.Scale(_finalZoom, _finalZoom, _finalZoom);

                //prepare output event
                double[] trans = { _finalTrans[0] + _currentTrans[0], _finalTrans[1] + _currentTrans[1], _finalTrans[2] + _currentTrans[2] };
                double[] rot = { _finalRot[0] + _currentRot[0], _finalRot[1] + _currentRot[1] };

                if (ViewUpdated != null) //raise event if there are subscribers
                    ViewUpdated(this, new GLControlViewUpdatedEventArgs(trans, rot, _finalZoom));

                DrawObjects();
                Cleanup();
            }
        } //step 2 option 1
        private void SetOrientation_Manual(double[] Trans, double[] Rot, double Zoom)
        {
            if (!disposed)
            {
                _finalTrans[0] = Trans[0];
                _finalTrans[1] = Trans[1];

                _finalRot[0] = Rot[0];
                _finalRot[1] = Rot[1];

                _finalZoom = Zoom;


                GL.Translate(0, -1 * _finalTrans[0], _finalTrans[1]);
                GL.Rotate(_finalRot[0], 0, 0, 1);
                GL.Rotate(_finalRot[1], 0, 1, 0);
                GL.Scale(_finalZoom, _finalZoom, _finalZoom);

                //prepare output event

                if (ViewUpdated != null) //raise event if there are subscribers
                    ViewUpdated(this, new GLControlViewUpdatedEventArgs(_finalTrans, _finalRot, _finalZoom));

                DrawObjects();
                Cleanup();
            }
        } //step 2 option 2
        private void DrawObjects()
        {
            foreach (IGLDrawable obj in this.DrawnObjects)
            {
                if (obj.IsDrawn)
                    obj.Draw();
            }
        } //step 3       
        private void Cleanup()
        {
            //Finally...
            if (SubControl.Context != null)
                SubControl.Context.SwapInterval = 1;
            this.SubControl.SwapBuffers(); //Takes from the 'GL' and puts into control 
        } //step 4 this needs to be done after all objects are drawn


        //------------ Subs to listen for control events ------------------

        private void GLControl_MouseDown(object? Sender, MouseEventArgs e)
        {
            this.DragStarted(Cursor.Position.X, Cursor.Position.Y);
        }
        private void GLControl_MouseMove(object? Sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Dragged(Cursor.Position.X, Cursor.Position.Y);
            }
        }
        private void GLControl_MouseUp(object? Sender, MouseEventArgs e)
        {
            EndDrag();
        }
        private void GLControl_MouseWheel(object? Sender, MouseEventArgs e)
        {
            MouseWheeled(e.Delta);
        }

        private void GLControl_Resized(object? Sender, EventArgs e)
        {
            this.Refresh();
        }



        private bool mouseIsOverControl(GLControl c)
        {
            return c.ClientRectangle.Contains(c.PointToClient(System.Windows.Forms.Control.MousePosition));
        }

    }
}
