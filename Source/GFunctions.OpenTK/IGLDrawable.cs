namespace GFunctions.OpenTK
{
    /// <summary>
    /// Generic definition of an object that can be drawn with a <see cref="GLControlDraggable"/>
    /// </summary>
    public interface IGLDrawable
    {
        /// <summary>
        /// If true, the object will be drawn, false and the object will not be drawn
        /// </summary>
        bool IsDrawn { get; set; }
        
        /// <summary>
        /// Draws the object
        /// </summary>
        void Draw();

        /// <summary>
        /// Fires if the object requires redrawing
        /// </summary>
        event EventHandler RedrawRequired;
    }
}
