using Godot;
using System;

public partial class DamageText : RichTextLabel
{

    [Export]
    public Timer Timer;

    Vector2 FinalPos;

    float InterpSpeed = 0.07f;
    float InterpDist = 150;
    int RandOffset = 50;


    public override void _Ready()
    {
        Timer.Timeout += Disappear;
    }
    public override void _EnterTree()
    {
        base._EnterTree();
        GD.Randomize();
        
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (Visible)
        {
            Interpolate();
        }
    }

    public void Disappear()
    {
        
        Hide();
        //Pooler.Stuff
    }

    public void ReAppear()
    {
        Timer.Start();
        Show();
    }

    public void SetText(Vector2 pos, int damage)
    {
        GlobalPosition = pos;

        FinalPos = pos + new Vector2(GD.RandRange(-RandOffset, RandOffset), -InterpDist);
        Text = "-" + damage.ToString();
    }

    public void Interpolate()
    {
        GlobalPosition += (FinalPos - GlobalPosition) * InterpSpeed;
    }

}
