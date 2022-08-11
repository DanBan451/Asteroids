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
using AsteroidsAtari;
using CometF;
using PlayerM;

namespace CometM
{
  public class Manage
  {
    public static List<Image> List_Comet = new List<Image>();
    public static List<Vector> List_CometSlope = new List<Vector>();
    public static List<EllipseGeometry> List_CometEllipse = new List<EllipseGeometry>();

    public static List<Image> List_Pellet = new List<Image>();
    public static List<Vector> List_PelletSlope = new List<Vector>();
    public static List<EllipseGeometry> List_PelletEllipse = new List<EllipseGeometry>();

    public static List<Image> List_ExplosionObject = new List<Image>();
    public static List<Stopwatch> List_ExplosionTimer = new List<Stopwatch>();
    public static List<int> List_ExplosionState = new List<int>();

    public static List<Image> List_PelletSaucer = new List<Image>();
    // public static List<EllipseGeometry> List_PelletSaucerEllipse = new List<EllipseGeometry>();

    public static EllipseGeometry PlayerEllipse = new EllipseGeometry();
    public static EllipseGeometry SaucerEllipse = new EllipseGeometry();

    public static void MoveComet(Image comet, Vector slope)
    {
      BorderPatrole(comet);
      Canvas.SetLeft(comet, Canvas.GetLeft(comet) + slope.X);
      Canvas.SetTop(comet, Canvas.GetTop(comet) + slope.Y);
    }

    public static void MovePellets(Image pellet, Vector slope)
    {
      var pelletPosition = VisualTreeHelper.GetOffset(pellet);

      if ((pelletPosition.X > -10 && pelletPosition.X < 1000 && pelletPosition.Y > -10 && pelletPosition.Y < 710))
      {
        Canvas.SetLeft(pellet, Canvas.GetLeft(pellet) + slope.X);
        Canvas.SetBottom(pellet, Canvas.GetBottom(pellet) + slope.Y);
      }
      else
      {
        MainWindow.CanvasMain.Children.Remove(pellet);
        List_PelletEllipse.RemoveAt(List_Pellet.IndexOf(pellet));
        List_Pellet.Remove(pellet);
        List_PelletSlope.Remove(slope);
      }
    }

    public static void CollisionPatrole(int group, Image player, Image saucer)
    {
      switch (group)
      {
        case 0:
          for (int i = 0; i < List_Comet.Count; i++)
          {
            MoveComet(List_Comet[i], List_CometSlope[i]);

            if (!MainWindow.isPlayerReset)
            {
              // Comets ~ Player
              if (CollisionAt(List_Comet[i], player, List_CometEllipse[i], PlayerEllipse))
              {
                Image image = new Image();
                image.Height = MainWindow.Player.Height;
                Order.ExploionStateAt(image, 0);
                MainWindow.CanvasMain.Children.Add(image);
                Canvas.SetLeft(image, Canvas.GetLeft(player));
                Canvas.SetBottom(image, Canvas.GetBottom(player));

                var swExplode = new Stopwatch();
                swExplode.Start();
                List_ExplosionTimer.Add(swExplode);
                List_ExplosionState.Add(0);
                List_ExplosionObject.Add(image);

                MainWindow.isPlayerReset = true;
                MainWindow.livesCount--;
              }
              // Comets ~ Saucer
              if (CollisionAt(List_Comet[i], saucer, List_CometEllipse[i], SaucerEllipse) && MainWindow.isSaucerAttacking)
              {
                Image image = new Image();
                image.Height = saucer.Height;
                Order.ExploionStateAt(image, 0);
                MainWindow.CanvasMain.Children.Add(image);
                Canvas.SetLeft(image, Canvas.GetLeft(saucer));
                Canvas.SetBottom(image, Canvas.GetBottom(saucer));

                var swExplode = new Stopwatch();
                swExplode.Start();
                List_ExplosionTimer.Add(swExplode);
                List_ExplosionState.Add(0);
                List_ExplosionObject.Add(image);

                MainWindow.scoreCountSaucer = MainWindow.scoreCount + 1500;
                saucer.Visibility = Visibility.Hidden;
                MainWindow.isSaucerAttacking = false;
              }
            }
            else if (!PlayerResetProcess(MainWindow.sw3))
            {
              MainWindow.isPlayerReset = false;
            }
          }
          break;

        case 1:
          for (int i = 0; i < List_Pellet.Count; i++)
          {
            MovePellets(List_Pellet[i], List_PelletSlope[i]);

            if (!MainWindow.isPlayerReset)
            {
              // Player ~ SaucerPellets
              if (CollisionAt(player, List_Pellet[i], PlayerEllipse, List_PelletEllipse[i]) && List_PelletSaucer.Contains(List_Pellet[i]))
              {
                Image image = new Image();
                image.Height = player.Height;
                Order.ExploionStateAt(image, 0);
                MainWindow.CanvasMain.Children.Add(image);
                Canvas.SetLeft(image, Canvas.GetLeft(player));
                Canvas.SetBottom(image, Canvas.GetBottom(player));

                var swExplode = new Stopwatch();
                swExplode.Start();
                List_ExplosionTimer.Add(swExplode);
                List_ExplosionState.Add(0);
                List_ExplosionObject.Add(image);

                MainWindow.CanvasMain.Children.Remove(List_Pellet[i]);
                List_Pellet.Remove(List_Pellet[i]);
                List_PelletSlope.Remove(List_PelletSlope[i]);
                List_PelletSaucer.Remove(List_PelletSaucer[i]);
                List_PelletEllipse.RemoveAt(i);
                MainWindow.isPlayerReset = true;
                MainWindow.livesCount--;
              }
              // Saucer ~ PlayerPellets
              if (CollisionAt(saucer, List_Pellet[i], SaucerEllipse, List_PelletEllipse[i]) && !List_PelletSaucer.Contains(List_Pellet[i]) && MainWindow.isSaucerAttacking)
              {
                Image image = new Image();
                image.Height = saucer.Height;
                Order.ExploionStateAt(image, 0);
                MainWindow.CanvasMain.Children.Add(image);
                Canvas.SetLeft(image, Canvas.GetLeft(saucer));
                Canvas.SetBottom(image, Canvas.GetBottom(saucer));

                var swExplode = new Stopwatch();
                swExplode.Start();
                List_ExplosionTimer.Add(swExplode);
                List_ExplosionState.Add(0);
                List_ExplosionObject.Add(image);

                MainWindow.scoreCountSaucer = MainWindow.scoreCount + 1500;
                saucer.Visibility = Visibility.Hidden;
                MainWindow.isSaucerAttacking = false;
              }
            }
            else if (!PlayerResetProcess(MainWindow.sw3))
            {
              MainWindow.isPlayerReset = false;
            }
            // Comets ~ PlayerPellets
            for (int e = 0; e < List_Comet.Count; e++)
            {
              if (CollisionAt(List_Comet[e], List_Pellet[i], List_CometEllipse[e], List_PelletEllipse[i]) && !List_PelletSaucer.Contains(List_Pellet[i]))
              {
                Image image = new Image();
                image.Height = List_Comet[e].Height;
                Order.ExploionStateAt(image, 0);
                MainWindow.CanvasMain.Children.Add(image);
                Canvas.SetLeft(image, Canvas.GetLeft(List_Comet[e]));
                Canvas.SetTop(image, Canvas.GetTop(List_Comet[e]));

                var swExplode = new Stopwatch();
                swExplode.Start();
                List_ExplosionTimer.Add(swExplode);
                List_ExplosionState.Add(0);
                List_ExplosionObject.Add(image);

                if (List_Comet[e].Height == 80)
                {
                  CometSplit(List_Comet[e], 80);
                }
                else if (List_Comet[e].Height == 60)
                {
                  CometSplit(List_Comet[e], 60);
                }
                else if (List_Comet[e].Height == 40)
                {
                  CometSplit(List_Comet[e], 40);
                }
                else { } // Destroyed

                MainWindow.CanvasMain.Children.Remove(List_Comet[e]);
                List_Comet.Remove(List_Comet[e]);
                List_CometSlope.Remove(List_CometSlope[e]);
                List_CometEllipse.RemoveAt(e);

                MainWindow.CanvasMain.Children.Remove(List_Pellet[i]);
                List_Pellet.Remove(List_Pellet[i]);
                List_PelletSlope.Remove(List_PelletSlope[i]);
                List_PelletEllipse.RemoveAt(i);
              }
            }
          }
          break;

        case 2:
          if (CollisionAt(player, saucer, PlayerEllipse, SaucerEllipse) && MainWindow.isSaucerAttacking)
          {
            MainWindow.isPlayerReset = true;
            MainWindow.livesCount--;
          }
          break;
      }
    }

    public static bool CollisionAt(Image object1, Image object2, EllipseGeometry ellip1, EllipseGeometry ellip2)
    {
      int addingX = 0;
      int addingY = 0;
      if (List_Comet.Contains(object1))
      {
        switch (object1.Height)
        {
          case 80:
            addingX = 35;
            addingY = 25;
            break;
          case 60:
            addingX = 23;
            addingY = 17;
            break;
          default:
            addingX = 0;
            addingY = 0;
            break;
        }
      }

      ellip1.RadiusX = object1.Height / 2;
      ellip1.RadiusY = object1.Height / 2;
      ellip2.RadiusX = object2.Height / 2;
      ellip2.RadiusY = object2.Height / 2;

      ellip1.Center = (Point)(new Vector(VisualTreeHelper.GetOffset(object1).X + addingX, VisualTreeHelper.GetOffset(object1).Y + addingY));
      ellip2.Center = (Point)(new Vector(VisualTreeHelper.GetOffset(object2).X, VisualTreeHelper.GetOffset(object2).Y));

      if (Distance(ellip1.Center, ellip2.Center) <= ellip1.RadiusX + ellip2.RadiusX)
      {
        return true;
      }
      return false;
    }

    public static void CometSplit(Image comet, int size)
    {
      var cometPosition = VisualTreeHelper.GetOffset(comet);
      if (size == 80)
      {
        MainWindow.scoreCount += 50;
        for (int i = 0; i <= 1; i++)
        {
          var smallerComet = Order.RandomComet(60);
          List_Comet.Add(smallerComet);
          EllipseGeometry ellipse = new EllipseGeometry();
          List_CometEllipse.Add(ellipse);

          Canvas.SetLeft(smallerComet, cometPosition.X);
          Canvas.SetTop(smallerComet, cometPosition.Y);
          List_CometSlope.Add(Order.RandomSlope() * 1.5);
        }
      }
      else if (size == 60)
      {
        MainWindow.scoreCount += 100;
        for (int i = 0; i <= 1; i++)
        {
          var smallerComet = Order.RandomComet(40);
          List_Comet.Add(smallerComet);
          EllipseGeometry ellipse = new EllipseGeometry();
          List_CometEllipse.Add(ellipse);

          Canvas.SetLeft(smallerComet, cometPosition.X);
          Canvas.SetTop(smallerComet, cometPosition.Y);
          List_CometSlope.Add(Order.RandomSlope() * 1.75);
        }
      }
      else if (size == 40)
      {
        MainWindow.scoreCount += 175;
        for (int i = 0; i <= 1; i++)
        {
          var smallerComet = Order.RandomComet(20);
          List_Comet.Add(smallerComet);
          EllipseGeometry ellipse = new EllipseGeometry();
          List_CometEllipse.Add(ellipse);

          Canvas.SetLeft(smallerComet, cometPosition.X);
          Canvas.SetTop(smallerComet, cometPosition.Y);
          List_CometSlope.Add(Order.RandomSlope() * 2.5);
        }

      }
      else if (size == 20)
      {
        MainWindow.scoreCount += 200;
        // Dead
      }

    }

    public static void Explode()
    {
      for (int i = 0; i < List_ExplosionObject.Count; i++)
      {
        if (List_ExplosionTimer[i].Elapsed.TotalMilliseconds > 30)
        {
          if (List_ExplosionState[i] != 8)
          {
            List_ExplosionState[i]++;
            Order.ExploionStateAt(List_ExplosionObject[i], List_ExplosionState[i]);
            List_ExplosionTimer[i].Restart();
          }
          else
          {
            MainWindow.CanvasMain.Children.Remove(List_ExplosionObject[i]);
            List_ExplosionObject.RemoveAt(i);
            List_ExplosionState.RemoveAt(i);
            List_ExplosionTimer.RemoveAt(i);
          }
        }
      }
    }

    public static void Start(int num)
    {

      var hotspots = Order.RandomHotspots(num);

      for (int i = 0; i < hotspots.Count; i++)
      {
        List_Comet.Add(Order.RandomComet(80));
        Canvas.SetLeft(List_Comet[i], hotspots[i].X);
        Canvas.SetTop(List_Comet[i], hotspots[i].Y);
        List_CometSlope.Add(Order.RandomSlope());

        EllipseGeometry ellipse = new EllipseGeometry();
        List_CometEllipse.Add(ellipse);
      }
    }

    public static void ResetComets()
    {
      for (int i = 0; i < List_Comet.Count; i++)
      {
        MainWindow.CanvasMain.Children.Remove(List_Comet[i]);
      }
      List_Comet.Clear();
      List_CometSlope.Clear();
      for (int i = 0; i < List_ExplosionObject.Count; i++)
      {
        MainWindow.CanvasMain.Children.Remove(List_ExplosionObject[i]);
      }
      List_ExplosionObject.Clear();
      List_ExplosionTimer.Clear();
      List_ExplosionState.Clear();
    }

    public static void FlyUFO(Image obj, Stopwatch sw)
    {
      if (Canvas.GetLeft(obj) < -10)
      {
        MainWindow.scoreCountSaucer = MainWindow.scoreCount + 1500;
        obj.Visibility = Visibility.Hidden;
        MainWindow.isSaucerAttacking = false;
      }
      else
      {
        Canvas.SetLeft(obj, Canvas.GetLeft(obj) - 0.01);

        if (sw.Elapsed.TotalMilliseconds > 1500)
        {
          Image imag = new Image();
          Canvas.SetLeft(imag, Canvas.GetLeft(obj));
          Canvas.SetBottom(imag, Canvas.GetBottom(obj));
          List_Pellet.Add(Order.CreateImage(imag, "Static_playerPellet", 22));
          List_PelletSlope.Add(Order.RandomSlope() * 10);
          List_PelletSaucer.Add(imag);
          EllipseGeometry ellipse = new EllipseGeometry();
          List_PelletEllipse.Add(ellipse);
          sw.Restart();
        }
      }
    }

    public static void ResetPellets()
    {
      for (int i = 0; i < List_Pellet.Count; i++)
      {
        MainWindow.CanvasMain.Children.Remove(List_Pellet[i]);
      }
      List_Pellet.Clear();
      List_PelletSlope.Clear();
    }

    public static bool PlayerResetProcess(Stopwatch timer)
    {
      if (timer.IsRunning == false)
      {
        ManagePlayer.ResetPlayer();
        MainWindow.Player.Visibility = Visibility.Hidden;
        MainWindow.PlayerBoost.Visibility = Visibility.Hidden;

        timer.Start();
      }
      if (timer.Elapsed.TotalMilliseconds > 5000 && MainWindow.isGameOver == false)
      {
        timer.Reset();
        MainWindow.Player.Visibility = Visibility.Visible;
        MainWindow.PlayerBoost.Visibility = Visibility.Hidden;
        return false;
      }
      return true;
    }

    public static void CreatePellet()
    {
      Image imag = new Image();
      imag.RenderTransformOrigin = new Point(0.5, 0.5);
      RotateTransform rotateTransform = new RotateTransform(ManagePlayer.angle);
      imag.RenderTransform = rotateTransform;
      Canvas.SetLeft(imag, Canvas.GetLeft(AsteroidsAtari.MainWindow.Player));
      Canvas.SetBottom(imag, Canvas.GetBottom(AsteroidsAtari.MainWindow.Player));
      RenderOptions.SetBitmapScalingMode(imag, BitmapScalingMode.LowQuality);
      List_Pellet.Add(Order.CreateImage(imag, "Static_playerPellet", 20));
      List_PelletSlope.Add(new Vector(ManagePlayer.Run, ManagePlayer.Rise));
      EllipseGeometry ellipse = new EllipseGeometry();
      List_PelletEllipse.Add(ellipse);

    }

    static double Distance(Point point1, Point point2)
    {
      return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
    }

    public static void BorderPatrole(Image suspect)
    {
      var position = VisualTreeHelper.GetOffset(suspect);

      if (position.X < -120)
      {
        Canvas.SetLeft(suspect, Canvas.GetLeft(suspect) + 1110);
      }
      else if (position.X > 1000)
      {
        Canvas.SetLeft(suspect, Canvas.GetLeft(suspect) - 1000);
      }

      if (List_Comet.Contains(suspect))
      {
        if (position.Y < -100)
        {
          Canvas.SetTop(suspect, Canvas.GetTop(suspect) + 700);
        }
        else if (position.Y > 600)
        {
          Canvas.SetTop(suspect, Canvas.GetTop(suspect) - 700);
        }
      }
      else
      {
        if (position.Y < -100)
        {
          Canvas.SetBottom(suspect, Canvas.GetBottom(suspect) - 700);
        }
        else if (position.Y > 600)
        {
          Canvas.SetBottom(suspect, Canvas.GetBottom(suspect) + 700);
        }
      }
    }

  }
}
