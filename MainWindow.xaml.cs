using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CometM;
using CometF;
using PlayerM;

namespace AsteroidsAtari
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    // Creating to access from other classes
    public static Canvas CanvasMain;
    public static Image Player;
    public static Image PlayerBoost;
    public static Image Saucer;
    public static MainWindow Instance { get; private set; }

    List<bool> stages = new List<bool>() { true, false, false, false };

    Stopwatch sw = new Stopwatch();
    public static Stopwatch sw2 = new Stopwatch();
    public static Stopwatch sw3 = new Stopwatch();
    public static Stopwatch sw4 = new Stopwatch();
    public static Stopwatch sw5 = new Stopwatch();
    static int temp = 0;
    static int temp2 = 0;

    static int gameDifficulty = 3;
    public static bool isPlayerReset = false;
    public static bool isRoundStarted = true;
    public static bool isGameOver = false;
    public static bool isSaucerAttacking = false;
    public static bool isDrag = false;

    public static int livesCount = 3;
    public static int scoreCount = 0;
    public static int scoreCountSaucer = 0;

    public MainWindow()
    {
      InitializeComponent();
      CanvasMain = Canvas_main;
      Player = Image_Player;
      PlayerBoost = Image_PlayerBoost;
      Saucer = Image_Saucer;
      Instance = this;

      BackgroundWorker mainWorker = new BackgroundWorker();
      mainWorker.DoWork += MainWorker;
      mainWorker.RunWorkerAsync(1000);


      Manage.Start(10);
      sw.Start();

      ToggleTo(0);

    }

    private void MainWorker(object sender, DoWorkEventArgs e)
    {
      BackgroundWorker worker = (BackgroundWorker)sender;
      while (!worker.CancellationPending)
      {
        // Placing try to catch error
        try
        {
          // Accessing UI Thread 
          App.Current.Dispatcher.Invoke(() =>
          {
            if (stages[0])
            {
              Stage0();
            }
            else if (stages[1])
            {
              Stage1();
            }
            else if (stages[2])
            {
              Stage2();
              UpdateLives();
              Game();
            }
            else if (stages[3])
            {
              Stage3();
            }
          });
        }
        catch { }
      }
    }

    // Functions are running on background thread
    void Stage0()
    {
      for (int i = 0; i < Manage.List_Comet.Count; i++)
      {
        Manage.MoveComet(Manage.List_Comet[i], Manage.List_CometSlope[i]);
      }
      if (sw.ElapsedMilliseconds >= 800)
      {
        Order.StartLabel_Flicker(Label_StartFlicker);
        sw = new Stopwatch();
        sw.Start();
      }
    }
    void Stage1()
    {
      if (sw.ElapsedMilliseconds >= 3000)
      {
        ToggleTo(2);
      }
    }
    void Stage2()
    {
      if (livesCount == 0)
      {
        Appearance(Label_GameOver, 1);
        isGameOver = true;
        ToggleTo(3);
      }

      // Label_Score.Content = scoreCount.ToString();
      UpdatePlayer();
      UpdateSaucer();
      Game();
    }
    void Stage3()
    {
      if (sw2.ElapsedMilliseconds > 5000)
      {
        Manage.ResetComets();
        Manage.Start(10);
        ToggleTo(0);
      }
      else
      {
        Game();
      }
    }

    private static void Game()
    {
      if (!isRoundStarted)
      {
        if (!sw4.IsRunning)
        {
          sw4.Start();
        }
        if (sw4.ElapsedMilliseconds > 2000)
        {
          sw4.Reset();
          Manage.Start(gameDifficulty);
          gameDifficulty++;
          isRoundStarted = true;
        }
      }
      else
      {
        if (Manage.List_Comet.Count == 0)
        {
          isRoundStarted = false;
        }
      }

      Manage.CollisionPatrole(0, Player, Saucer);
      Manage.CollisionPatrole(1, Player, Saucer);
      Manage.CollisionPatrole(2, Player, Saucer);

      if (Manage.List_ExplosionObject.Count > 0)
      {
        Manage.Explode();
      }
    }

    private void UpdatePlayer()
    {
      if (!isPlayerReset)
      {
        if (Keyboard.IsKeyDown(Key.Space))
        {
          if (!sw2.IsRunning)
          {
            sw2.Start();
          }
          if (sw2.ElapsedMilliseconds > 50)
          {
            Order.PlayerBoost_Flicker(PlayerBoost);
            sw2.Reset();
          }

          if (temp == 0)
          {
            sw.Restart();
            temp = 1;
          }

          if (isDrag == false)
          {
            ManagePlayer.Move(Player, sw.Elapsed.TotalSeconds);
          }
          else
          {
            ManagePlayer.OverrideDrag(Player, sw.Elapsed.TotalSeconds);
          }
        }
        else if (Keyboard.IsKeyUp(Key.Space))
        {
          isDrag = true;
          ManagePlayer.Stop(Player, sw.Elapsed.TotalSeconds);
        }
      }
    }

    private static void UpdateSaucer()
    {
      if (scoreCount > scoreCountSaucer && !isSaucerAttacking)
      {
        Saucer.Visibility = Visibility.Visible;
        Canvas.SetLeft(Saucer, 1110);
        Canvas.SetBottom(Saucer, Order.UFORandomPosition().Y);
        sw5.Restart();
        isSaucerAttacking = true;
      }

      if (isSaucerAttacking)
      {
        Manage.FlyUFO(Saucer, sw5);
      }
    }
    private void UpdateLives()
    {

      if (livesCount == 2)
      {
        Appearance(Image_Lives3, 0);
      }
      else if (livesCount == 1)
      {
        Appearance(Image_Lives2, 0);
      }
      else if (livesCount == 0)
      {
        Appearance(Image_Lives1, 0);
      }
    }

    void Handler_MouseMove(object sender, MouseEventArgs e)
    {

      if (stages[2])
      {
        var mousePoint = PointToScreen(Mouse.GetPosition(this));
        Point point = Player.TransformToAncestor(this).Transform(new Point(0, 0));
        var playerPoint = PointToScreen(point);

        ManagePlayer.UpdateRotation(Player, playerPoint, mousePoint, PlayerBoost);
      }
    }
    void Handler_KeyUp(object sender, KeyEventArgs e)
    {
      if (stages[0] && e.Key == Key.Space)
      {
        ToggleTo(1);
      }
      if (stages[2] && !isPlayerReset)
      {
        if (e.Key == Key.Space)
        {
          ManagePlayer.LastRise = ManagePlayer.Rise;
          ManagePlayer.LastRun = ManagePlayer.Run;
          PlayerBoost.Visibility = Visibility.Hidden;

        }

        if (e.Key == Key.W)
        {
          Manage.CreatePellet();
        }
      }
    }

    public void ToggleTo(int stage)
    {
      for (int i = 0; i < stages.Count; i++)
      {
        if (i == stage)
        {
          stages[i] = true;
        }
        else
        {
          stages[i] = false;
        }
      }

      switch (stage)
      {
        case 0:
          RenderOptions.SetBitmapScalingMode(Player, BitmapScalingMode.LowQuality);
          RenderOptions.SetBitmapScalingMode(PlayerBoost, BitmapScalingMode.LowQuality);
          RenderOptions.SetBitmapScalingMode(Saucer, BitmapScalingMode.LowQuality);
          Appearance(Player, 0);
          Appearance(PlayerBoost, 0);
          Appearance(Saucer, 0);
          Appearance(Label_StartFlicker, 1);
          Appearance(Label_Coin, 1);
          Appearance(Image_Lives1, 0);
          Appearance(Image_Lives2, 0);
          Appearance(Image_Lives3, 0);
          Appearance(Label_Player1, 0);
          Appearance(Label_GameOver, 0);
          // Label_Score.Content = "0";
          scoreCount = 0;
          scoreCountSaucer = 1000;
          livesCount = 3;
          gameDifficulty = 3;
          isPlayerReset = false;
          isGameOver = false;
          isDrag = false;
          isSaucerAttacking = false;
          temp = 0;
          sw4.Reset();
          sw5.Reset();
          ManagePlayer.ResetRotation(Player);
          break;

        case 1:
          Appearance(Label_StartFlicker, 0);
          Appearance(Label_Coin, 0);
          Appearance(Image_Lives1, 1);
          Appearance(Image_Lives2, 1);
          Appearance(Image_Lives3, 1);
          Appearance(Label_Player1, 1);
          Manage.ResetComets();
          sw.Restart();

          break;
        case 2:
          Appearance(Label_Player1, 0);
          Appearance(Player, 1);
          sw.Reset();
          break;

        default:
          Appearance(Saucer, 0);
          sw2.Restart();
          Manage.ResetPellets();
          Manage.List_PelletSaucer.Clear();
          break;
      }
    }

    void Appearance(UIElement element, int num)
    {
      if (num == 0)
      {
        element.Visibility = Visibility.Hidden;
      }
      else element.Visibility = Visibility.Visible;
    }
  }
}
