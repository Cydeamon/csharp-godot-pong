using Godot;
using System;
using System.Drawing.Printing;
using static Godot.GD;

public class MainScene : Node2D
{
    public int PlayerScore = 0;
    public int EnemyScore = 0;
    
    public bool GameStarted;
    public bool IsPlayerTurn; 

    public Vector2 BallDirection = Vector2.Zero;
    public KinematicBody2D Ball;
    [Export] public int BallSpeed;

    public override void _Ready()
    {
        Ball = GetNode<KinematicBody2D>("Game/Ball");
        GameReset();
    }

    public override void _PhysicsProcess(float delta)
    {
        KinematicCollision2D result = Ball.MoveAndCollide(BallDirection * BallSpeed * delta);

        if (result != null)
        {
            Node collider = (Node) result.Collider;

            if (collider.Name == "Player")
            {
                PlayerRect player = (PlayerRect) collider;
                player.OnHit(Ball);
                BallBounceAfterPlayerHit(player.LastHitHeightPositionPercent);
                IsPlayerTurn = !IsPlayerTurn;
            }
        }
    }

    private void BallBounceAfterPlayerHit(float hitHeightPercentPosition)
    { 
        float maxAngle = 140;
        float angle = (maxAngle * hitHeightPercentPosition / 100) - maxAngle / 2;

        BallDirection.x = (float) Math.Cos(angle);
        BallDirection.y = (float) Math.Sin(angle);

        if (IsPlayerTurn)
        {
            BallDirection.x = -BallDirection.x;
            BallDirection.y = -BallDirection.y;
        }
        
    }

    private void GameStart()
    {
        GameStarted = true;
    }

    // <summary> Reset game state</summary>
    private void GameReset()
    {
        Ball.Position = GetViewport().Size / 2;
        GetNode<Timer>("Game/GameStartTimer").Start();
        PickRandomBallDirection();
        GameStarted = false;
    }

    private void PickRandomBallDirection()
    {
        BallDirection = Vector2.Right;
        IsPlayerTurn = true;
    }

    public void OnGameStartTimerTimeout()
    {
        GameStart();
    }

    public void OnLoseTriggerBodyEntered(string player)
    {
        if (player == "player")
        {
            PlayerScore++;
            GetNode<Label>("Game/Player/Score").Text = PlayerScore.ToString();
        }

        if (player == "enemy")
        {
            EnemyScore++;
            GetNode<Label>("Game/Player/Score").Text = EnemyScore.ToString();
        }
        
        GameReset();
    }
}