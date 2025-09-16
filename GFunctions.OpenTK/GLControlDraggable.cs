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
    /// <summary>
    /// Extends a <see cref="GLControl"/> with mouse drag (rotation, panning) and zoom functionality
    /// </summary>
    public class GLControlDraggable
    {
        private bool disposed = false; // Used when form is closing to prevent calls

        private double[] _currentTrans = [0, 0, 0]; // XYZ coords
        private double[] _currentRot = [0, 0]; // Z, X rotation

        private double[] _finalTrans = [0, 0, 0];
        private double[] _finalRot = [0, 0];
        private double _finalZoom = 1.0;

        private bool _mousePressed = false;
        private double _mouseDelta = 0; // Positive or negative depending on direction of wheel

        private double[] _mouseDown = [0, 0]; // X, Y
        private double[] _mouseCurrent = [0, 0]; // X, Y

        private double _rotMultiplier = 1;
        private double _transMultiplier = -0.1;
        private double _ZoomMultiplier = 0.02;

        //-------------- Public Properties ----------------
        
        /// <summary>
        /// The control managed by this class
        /// </summary>
        public GLControl SubControl { get; private set; }

        /// <summary>
        /// All objects that will be drawn by the GLControl
        /// </summary>
        public List<IGLDrawable> DrawnObjects = [];

        /// <summary>
        /// The mousebutton used for dragging/rotating the view
        /// </summary>
        public MouseButtons DragButton { get; set; } = MouseButtons.Left;

        /// <summary>
        /// The key that when held, switches the control to panning instead of rotation
        /// </summary>
        public Keys TranslateKey { get; set; } = Keys.Shift;

        //-------------- Events  ----------------

        /// <summary>
        /// Raised when the GLControl has been updated
        /// </summary>
        public event EventHandler<GLControlViewUpdatedEventArgs>? ViewUpdated;

        //-------------- Public subs  ----------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="control">The control to manage</param>
        public GLControlDraggable(GLControl control)
        {
            SubControl = control;

            // Add handlers for control objects
            control.MouseDown += GLControl_MouseDown;
            control.MouseMove += GLControl_MouseMove;
            control.MouseUp += GLControl_MouseUp;
            control.MouseWheel += GLControl_MouseWheel;
            control.Resize += GLControl_Resized;

            // Update background color of form and refresh to display any inital objects
            GL.ClearColor(Color.Black);
            Refresh();
        }

        /// <summary>
        /// Disposes this object and the underlying control
        /// </summary>
        public void Dispose()
        {
            disposed = true;
            SubControl.Dispose();
        }

        /// <summary>
        /// Used for re-drawing screen when orientation hasn't changed (new object added, etc)
        /// </summary>
        public void Refresh()
        {
            PrepCamera();

            if (!_mousePressed)
            {
                SetOrientation_Manual(_finalTrans, _finalRot, _finalZoom);
            }
            else
            {
                SetOrientation_Drag(); //makes refreshes smooth for animation when mouse is being held down
            }
        }

        /// <summary>
        /// Returns whether the mouse is inside the control
        /// </summary>
        /// <param name="control">The control to check</param>
        /// <returns>True if the mouse is inside the control</returns>
        public static bool MouseIsOverControl(GLControl control)
        {
            return control.ClientRectangle.Contains(control.PointToClient(Control.MousePosition));
        }

        //-------------- Private subs  ----------------

        // Mouse methods
        private void DragStarted(double X, double Y)
        {
            if (MouseIsOverControl(SubControl))
            {
                _mousePressed = true;
                _mouseDown[0] = X;
                _mouseDown[1] = Y;
            }
        }
        private void Dragged(double X, double Y)
        {
            if (_mousePressed)
            {
                _mouseCurrent[0] = X;
                _mouseCurrent[1] = Y;

                PrepCamera();
                SetOrientation_Drag();
            }
        }
        private void EndDrag()
        {
            _mousePressed = false;

            _finalTrans[0] += _currentTrans[0];
            _finalTrans[1] += _currentTrans[1];
            _finalTrans[2] += _currentTrans[2];
            _finalRot[0] += _currentRot[0];
            _finalRot[1] += _currentRot[1];
        }
        private void MouseWheeled(double Z)
        {
            if (MouseIsOverControl(SubControl))
            {
                _mouseDelta = Z;

                PrepCamera();
                SetOrientation_Drag();
            }
        }

        // Drawing methods
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

                GL.Viewport(0, 0, SubControl.Width, SubControl.Height); //Size of window
                GL.Enable(EnableCap.DepthTest); //Enable correct Z Drawings
                GL.DepthFunc(DepthFunction.Less); //Enable correct Z Drawings
            }
        } // Step 1 
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

                    if (Control.ModifierKeys == TranslateKey)
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

                // Prepare output event
                double[] trans = [_finalTrans[0] + _currentTrans[0], _finalTrans[1] + _currentTrans[1], _finalTrans[2] + _currentTrans[2]];
                double[] rot = [_finalRot[0] + _currentRot[0], _finalRot[1] + _currentRot[1]];

                ViewUpdated?.Invoke(this, new GLControlViewUpdatedEventArgs(trans, rot, _finalZoom));

                DrawObjects();
                Cleanup();
            }
        } // Step 2 option 1
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

                // Prepare output event
                ViewUpdated?.Invoke(this, new GLControlViewUpdatedEventArgs(_finalTrans, _finalRot, _finalZoom));

                DrawObjects();
                Cleanup();
            }
        } // Step 2 option 2
        private void DrawObjects()
        {
            foreach (IGLDrawable obj in DrawnObjects)
            {
                if (obj.IsDrawn)
                    obj.Draw();
            }
        } // Step 3
        private void Cleanup()
        {
            //Finally...
            if (SubControl.Context != null)
                SubControl.Context.SwapInterval = 1;
            SubControl.SwapBuffers(); //Takes from the 'GL' and puts into control 
        } // Step 4 this needs to be done after all objects are drawn


        //------------ Subs to listen for control events ------------------
        private void GLControl_MouseDown(object? Sender, MouseEventArgs e)
        {
            DragStarted(Cursor.Position.X, Cursor.Position.Y);
        }
        private void GLControl_MouseMove(object? Sender, MouseEventArgs e)
        {
            if (e.Button == DragButton)
            {
                Dragged(Cursor.Position.X, Cursor.Position.Y);
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
            Refresh();
        }
    }
}
