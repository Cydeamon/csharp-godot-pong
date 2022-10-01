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

    public PlayerRect PlayerNode;
    
    public Vector2 BallDirection = Vector2.Zero;
    public KinematicBody2D Ball;
    
    [Export] public int BallSpeed;
    [Export] public int PlayerSpeed;

    public override void _Ready()
    {
        PlayerNode = GetNode<PlayerRect>("Game/Player/Player");
        Ball = GetNode<KinematicBody2D>("Game/Ball");
        GameReset();
    }

    public override void _Process(float delta)
    {
        float moveDistance = Input.GetActionStrength("down");
        moveDistance -= Input.GetActionStrength("up");
        moveDistance *= delta;
        moveDistance *= PlayerSpeed;

        PlayerNode.Translate(new Vector2(0, moveDistance));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (GameStarted)
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
                else
                {
                    BallDirection.y = -BallDirection.y;
                }
            }
        }
    }

    private void BallBounceAfterPlayerHit(float hitHeightPercentPosition)
    { 
        float maxAngle = 90;
        float angle = (maxAngle * hitHeightPercentPosition / 100) - maxAngle / 2;

        BallDirection.x = (float) Math.Cos(angle);
        BallDirection.y = (float) Math.Sin(angle);

        if (IsPlayerTurn)
        {
            BallDirection.x = -BallDirection.x;
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

    public void OnLoseTriggerBodyEntered(Node body, string player)
    {
        if (player == "Player")
        {
            EnemyScore++;
            GetNode<Label>("Game/Enemy/Score").Text = EnemyScore.ToString();
        }

        if (player == "Enemy")
        {
            PlayerScore++;
            GetNode<Label>("Game/Player/Score").Text = PlayerScore.ToString();
        }
        
        GameReset();
    }
}