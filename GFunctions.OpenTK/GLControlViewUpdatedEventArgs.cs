namespace GFunctions.OpenTK
{
    /// <summary>
    /// Arguments for when a <see cref="GLControlDraggable"/> view is updated
    /// </summary>
    /// <param name="translation">Translation of the view [X,Y,Z]</param>
    /// <param name="rotation">Rotation of the view [Z,X]</param>
    /// <param name="zoom">Zoom of the view</param>
    public class GLControlViewUpdatedEventArgs(double[] translation, double[] rotation, double zoom)
    {
        /// <summary>
        /// Translation of the view [X,Y,Z]
        /// </summary>
        public double[] Translation { get; private set; } = translation;

        /// <summary>
        /// Rotation of the view [Z,X]
        /// </summary>
        public double[] Rotation { get; private set; } = rotation;

        /// <summary>
        /// Zoom of the view
        /// </summary>
        public double Zoom { get; private set; } = zoom;
    }
}
