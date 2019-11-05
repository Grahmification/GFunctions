using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Drawing;

namespace GFunctions.Graphics
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

        public delegate void viewUpdatedDel(object sender, GLControlViewUpdatedEventArgs e);
        public event viewUpdatedDel ViewUpdated;

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
            GraphicsContext.CurrentContext.SwapInterval = 1;
            this.SubControl.SwapBuffers(); //Takes from the 'GL' and puts into control 
        } //step 4 this needs to be done after all objects are drawn


        //------------ Subs to listen for control events ------------------

        private void GLControl_MouseDown(object Sender, MouseEventArgs e)
        {
            this.DragStarted(Cursor.Position.X, Cursor.Position.Y);
        }
        private void GLControl_MouseMove(object Sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Dragged(Cursor.Position.X, Cursor.Position.Y);
            }
        }
        private void GLControl_MouseUp(object Sender, MouseEventArgs e)
        {
            EndDrag();
        }
        private void GLControl_MouseWheel(object Sender, MouseEventArgs e)
        {
            MouseWheeled(e.Delta);
        }

        private void GLControl_Resized(object Sender, EventArgs e)
        {
            this.Refresh();
        }



        private bool mouseIsOverControl(GLControl c)
        {
            return c.ClientRectangle.Contains(c.PointToClient(System.Windows.Forms.Control.MousePosition));
        }

    }
    public class GLControlViewUpdatedEventArgs
    {
        public double[] Translation { get; private set; }
        public double[] Rotation { get; private set; }
        public double Zoom { get; private set; }

        public GLControlViewUpdatedEventArgs(double[] translation, double[] rotation, double zoom)
        {
            this.Translation = translation;
            this.Rotation = rotation;
            this.Zoom = zoom;
        }
    }


    public interface IGLDrawable
    {
        bool IsDrawn { get; set; }
        void Draw();

        event EventHandler RedrawRequired;
    }
    public class GLObjects
    {
        public static void Cube(Color Clr, double[] Pos, double width)
        {
            GL.Color3(Clr);

            GL.Begin(PrimitiveType.Quads);
            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] + width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] - width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] - width / 2.0, Pos[2] - width / 2.0);
            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] + width / 2.0, Pos[2] - width / 2.0);

            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] + width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] - width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] - width / 2.0, Pos[2] - width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] + width / 2.0, Pos[2] - width / 2.0);

            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] + width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] - width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] - width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] + width / 2.0, Pos[2] + width / 2.0);

            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] + width / 2.0, Pos[2] - width / 2.0);
            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] - width / 2.0, Pos[2] - width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] - width / 2.0, Pos[2] - width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] + width / 2.0, Pos[2] - width / 2.0);

            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] + width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] + width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] + width / 2.0, Pos[2] - width / 2.0);
            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] + width / 2.0, Pos[2] - width / 2.0);

            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] - width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] - width / 2.0, Pos[2] + width / 2.0);
            GL.Vertex3(Pos[0] - width / 2.0, Pos[1] - width / 2.0, Pos[2] - width / 2.0);
            GL.Vertex3(Pos[0] + width / 2.0, Pos[1] - width / 2.0, Pos[2] - width / 2.0);


            GL.End();
        }
        public static void Line(Color Clr, double[] End1, double[] End2, double thickness = 1)
        {
            GL.Color3(Clr);

            GL.Begin(PrimitiveType.Lines);

            GL.LineWidth((float)thickness);
            GL.Vertex3(End1[0], End1[1], End1[2]);
            GL.Vertex3(End2[0], End2[1], End2[2]);
            GL.LineWidth((float)1.0); //reset to default

            GL.End();
        }
        public static void Arrow(Color Clr, double[] Pos, double[] Dir, double Length)
        {
            if (Length != 0)
            { //cant draw a force if its zero
                if (Dir[0] != 0 || Dir[1] != 0 || Dir[2] != 0)
                { //must have a direction

                    Vector3 vect = new Vector3((float)Dir[0], (float)Dir[1], (float)Dir[2]);
                    vect.Normalize();

                    Vector3 X = new Vector3(1f, 0f, 0f);
                    Vector3 Y = new Vector3(0f, 1f, 0f);

                    Vector3 normal1 = Vector3.Cross(vect, X);
                    normal1 = Vector3.Multiply(normal1, (float)(Length * 0.2));

                    Vector3 normal2 = Vector3.Cross(vect, Y);
                    normal2 = Vector3.Multiply(normal2, (float)(Length * 0.2));

                    vect = Vector3.Multiply(vect, (float)Length);


                    //draw line
                    GLObjects.Line(Clr, Pos, new double[] { Pos[0] + vect.X, Pos[1] + vect.Y, Pos[2] + vect.Z });

                    GL.Color3(Clr);
                    GL.Begin(PrimitiveType.Triangles);
                    Vector3 endPt = Vector3.Multiply(vect, (float)0.8);
                    GL.Vertex3(Pos[0] + vect.X, Pos[1] + vect.Y, Pos[2] + vect.Z);
                    GL.Vertex3(Pos[0] + endPt.X + normal1.X, Pos[1] + endPt.Y + normal1.Y, Pos[2] + endPt.Z + normal1.Z);
                    GL.Vertex3(Pos[0] + endPt.X + normal2.X, Pos[1] + endPt.Y + normal2.Y, Pos[2] + endPt.Z + normal2.Z);

                    GL.Vertex3(Pos[0] + vect.X, Pos[1] + vect.Y, Pos[2] + vect.Z);
                    GL.Vertex3(Pos[0] + endPt.X - normal1.X, Pos[1] + endPt.Y - normal1.Y, Pos[2] + endPt.Z - normal1.Z);
                    GL.Vertex3(Pos[0] + endPt.X - normal2.X, Pos[1] + endPt.Y - normal2.Y, Pos[2] + endPt.Z - normal2.Z);

                    GL.Vertex3(Pos[0] + vect.X, Pos[1] + vect.Y, Pos[2] + vect.Z);
                    GL.Vertex3(Pos[0] + endPt.X + normal1.X, Pos[1] + endPt.Y + normal1.Y, Pos[2] + endPt.Z + normal1.Z);
                    GL.Vertex3(Pos[0] + endPt.X - normal2.X, Pos[1] + endPt.Y - normal2.Y, Pos[2] + endPt.Z - normal2.Z);

                    GL.Vertex3(Pos[0] + vect.X, Pos[1] + vect.Y, Pos[2] + vect.Z);
                    GL.Vertex3(Pos[0] + endPt.X - normal1.X, Pos[1] + endPt.Y - normal1.Y, Pos[2] + endPt.Z - normal1.Z);
                    GL.Vertex3(Pos[0] + endPt.X + normal2.X, Pos[1] + endPt.Y + normal2.Y, Pos[2] + endPt.Z + normal2.Z);

                    GL.End();
                }
            }
        }

    }
}
