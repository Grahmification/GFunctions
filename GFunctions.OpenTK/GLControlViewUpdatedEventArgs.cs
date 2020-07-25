namespace GFunctions.OpenTK
{
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
}
