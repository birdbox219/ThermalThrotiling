using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

using System.Linq;



public class Player

{

    public int X { get; set; }

    public int Y { get; set; }

    public char Symbol { get; set; }



    public Player(int x, int y, char symbol)

    {

        X = x;

        Y = y;

        Symbol = symbol;

    }



    public virtual void Draw()

    {

        Console.SetCursorPosition(X, Y);

        Console.Write(Symbol);

    }

}



public class PlayerShip : Player

{

    public int Score { get; set; }

    private int health = 5;



    public PlayerShip(int x, int y) : base(x, y, '^') { }



    public void MoveLeft()

    {

        if (X > 0)

        {

            X--;

        }

    }



    public void MoveRight(int maxWidth)

    {

        if (X < maxWidth - 1)

        {

            X++;

        }

    }



    public void AddScore()

    {

        Score++;

    }



    public int GetScore()

    {

        return Score;

    }



    public int GetHealth()

    {

        return health;

    }



    public void SetHealth(int newHealth)

    {

        health = newHealth;

    }



    public void LoseHealth()

    {

        health--;

    }

}



public class EnemyShip : Player

{

    public EnemyShip(int x, int y) : base(x, y, 'v') { }



    public void MoveDown()

    {

        Y++;

    }

}



public class Bullet

{

    public int X { get; set; }

    public int Y { get; set; }



    public Bullet(int x, int y)

    {

        X = x;

        Y = y;

    }



    public void MoveUp()

    {

        Y--;

    }



    public void Draw()

    {

        Console.SetCursorPosition(X, Y);

        Console.Write("|");

    }

}



public class Game

{

    int width = Console.WindowWidth;

    int height = Console.WindowHeight-2;



    PlayerShip playerShip;

    List<EnemyShip> enemyShips = new List<EnemyShip>();

    List<Bullet> bullets = new List<Bullet>();



    Random random = new Random();



    bool gameOver = false;

    int enemySpeed = 0;



    double difficultyMultiplier  = 1.0;

    

    public Game()

    {

        playerShip = new PlayerShip(width / 2, height - 1);

    }



    public void Run()

    {

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        playerShip.SetHealth(5);

        Console.CursorVisible = false;



        while (!gameOver)

        {

            HandleInput();

            Update();

            Draw();

            Thread.Sleep(50);

        }



        Console.Clear();



        string[] gameOverArt = new string[]

        {

            "  ██████   █████  ███    ███ ███████      ██████  ██    ██ ███████ ██████  ",

            " ██       ██   ██ ████  ████ ██          ██    ██ ██    ██ ██      ██   ██ ",

            " ██   ███ ███████ ██ ████ ██ █████       ██    ██ ██    ██ █████   ██████  ",

            " ██    ██ ██   ██ ██  ██  ██ ██          ██    ██  ██  ██  ██      ██   ██ ",

            "  ██████  ██   ██ ██      ██ ███████      ██████    ████   ███████ ██   ██ "

        };



        int startY = (Console.WindowHeight / 2) - 4;

        int startX = (Console.WindowWidth / 2) - (gameOverArt[0].Length / 2);



        Console.ForegroundColor = ConsoleColor.Red;

        for (int i = 0; i < gameOverArt.Length; i++)

        {

            Console.SetCursorPosition(startX, startY + i);

            Console.WriteLine(gameOverArt[i]);

        }

        Console.ResetColor();



        string scoreText = "Total score: " + playerShip.GetScore();

        Console.SetCursorPosition((Console.WindowWidth - scoreText.Length) / 2, startY + gameOverArt.Length + 2);

        Console.WriteLine(scoreText);



        string playAgainText = "Play Again? (Y/N)";

        Console.SetCursorPosition((Console.WindowWidth - playAgainText.Length) / 2, startY + gameOverArt.Length + 4);

        Console.WriteLine(playAgainText);



        ConsoleKeyInfo key = Console.ReadKey(true);

        if (key.Key == ConsoleKey.Y)

        {

            Game newGame = new Game();

            newGame.Run();

        }

    }



    void HandleInput()

    {

        while (Console.KeyAvailable)

        {

            var key = Console.ReadKey(true).Key;

            switch (key)

            {

                case ConsoleKey.LeftArrow:

                    playerShip.MoveLeft();

                    break;

                case ConsoleKey.RightArrow:

                    playerShip.MoveRight(width);

                    break;

                case ConsoleKey.Spacebar:

                    bullets.Add(new Bullet(playerShip.X, playerShip.Y - 1));

                    break;

                case ConsoleKey.Escape:

                    gameOver = true;

                    break;

            }

        }

    }



    void Update()

    {

        enemySpeed++;



        int spawnChance = (int)(2 * difficultyMultiplier);

        if (random.Next(0, 10) < spawnChance)

        {

            enemyShips.Add(new EnemyShip(random.Next(0, width), 0));

        }



        foreach (var b in bullets)

        {

            b.MoveUp();

        }

        bullets.RemoveAll(b => b.Y < 0);

        

        if (enemySpeed % 3 == 0)

        {

            foreach (var e in enemyShips)

            {

                e.MoveDown();

            }

        }

        enemyShips.RemoveAll(e => e.Y >= height);



        for (int i = bullets.Count - 1; i >= 0; i--)

        {

            var b = bullets[i];

            for (int j = enemyShips.Count - 1; j >= 0; j--)

            {

                var e = enemyShips[j];

                if (b.X == e.X && Math.Abs(b.Y - e.Y) <= 1)

                {

                    bullets.RemoveAt(i);

                    enemyShips.RemoveAt(j);

                    playerShip.AddScore();

                    if (playerShip.GetScore() % 10 == 0 && playerShip.GetScore() != 0)

                    {

                        difficultyMultiplier += 0.1;

                    }

                    break;

                }

            }

        }



        for (int i = enemyShips.Count - 1; i >= 0; i--)

        {

            var e = enemyShips[i];

            if (playerShip.X == e.X && playerShip.Y == e.Y)

            {

                playerShip.LoseHealth();

                enemyShips.RemoveAt(i);

                if (playerShip.GetHealth() < 1)

                {

                    gameOver = true;

                }

            }

        }

    }

    

    void Draw()

    {

        Console.SetCursorPosition(0, 0);

        for (int i = 0; i < height; i++)

        {

            Console.Write(new string(' ', width));

        }



        playerShip.Draw();



        foreach (var b in bullets)

        {

            b.Draw();

        }



        foreach (var e in enemyShips)

        {

            e.Draw();

        }



        Console.SetCursorPosition(0, height);

        Console.Write("Score: " + playerShip.Score + "   ");

        Console.Write("\n");

        string hearts = "";

        for (int i = 0; i < playerShip.GetHealth(); i++)

        {

            hearts += "❤️ ";

        }

        Console.Write(hearts + "     ");



    }



}



class SpaceShooterRun

{

    static void Main(string[] args)

    {

        Game game = new Game();

        game.Run();



        return;

    }

}

