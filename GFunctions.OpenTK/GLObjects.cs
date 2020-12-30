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

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace GFunctions.OpenTK
{
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
