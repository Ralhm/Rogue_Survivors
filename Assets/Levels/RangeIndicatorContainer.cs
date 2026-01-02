using Godot;
using System;

public partial class RangeIndicatorContainer : Node
{
    [Export]
    public Node2D[] RangeIndices;


    public void SetRangeIndices(float Range, Vector2 Position)
    {
        RangeIndices[0].Position = Position + (new Vector2(0, 1) * Range);
        RangeIndices[1].Position = Position + (new Vector2(0, -1) * Range);
        RangeIndices[2].Position = Position + (new Vector2(1, 0) * Range);
        RangeIndices[3].Position = Position + (new Vector2(-1, 0) * Range);
        RangeIndices[4].Position = Position + (new Vector2(0.707f, 0.707f) * Range);
        RangeIndices[5].Position = Position + (new Vector2(-0.707f, -0.707f) * Range);
        RangeIndices[6].Position = Position + (new Vector2(-0.707f, 0.707f) * Range);
        RangeIndices[7].Position = Position + (new Vector2(0.707f, -0.707f) * Range);
    }

    public void Hide()
    {
        
    }

    public void Show()
    {

    }

}
