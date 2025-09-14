namespace GFunctions.OpenTK
{
    public interface IGLDrawable
    {
        bool IsDrawn { get; set; }
        void Draw();

        event EventHandler RedrawRequired;
    }
}
