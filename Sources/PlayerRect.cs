using Godot;
using System;

using static Godot.GD;

public class PlayerRect : RigidBody2D
{
    public float LastHitHeightPositionPercent = 0;
    
    public void OnHit(Node2D hitNode)
    {
        float height = GetNode<Sprite>("Player").GetRect().Size.y;
        float hitHeight = hitNode.Position.y - (Position.y - height / 2);
        LastHitHeightPositionPercent = hitHeight * 100 / height;
    }
}
