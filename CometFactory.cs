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

namespace CometF
{
  public class Order : Window
  {
    public static List<Vector> RandomHotspots(int amount)
    {
      var list = new List<Vector>();
      for (int i = 0; i < amount; i++)
      {
        float xPosition = 0;
        float yPosition = 0;
        var num = new Random().Next(0, 4);
        switch (num)
        {
          case 0:
            xPosition = new Random().Next(100, 900);
            yPosition = 500;
            break;

          case 1:
            xPosition = new Random().Next(100, 900);
            yPosition = 100;
            break;

          case 2:
            yPosition = new Random().Next(100, 500);
            xPosition = 900;
            break;

          default:
            yPosition = new Random().Next(100, 500);
            xPosition = 100;
            break;
        }

        list.Add(new Vector(xPosition, yPosition));
      }

      return list;
    }

    public static Image RandomComet(int size)
    {
      var num = new Random().Next(1, 9);
      switch (num)
      {
        case 1:
          Image image = new Image();
          image = CreateImage(image, "Static_comet1", size);
          return image;
        case 2:
          Image image2 = new Image();
          image2 = CreateImage(image2, "Static_comet2", size);
          return image2;
        case 3:
          Image image3 = new Image();
          image3 = CreateImage(image3, "Static_comet3", size);
          return image3;
        case 4:
          Image image4 = new Image();
          image4 = CreateImage(image4, "Static_comet4", size);
          return image4;
        case 5:
          Image image5 = new Image();
          image5 = CreateImage(image5, "Static_comet5", size);
          return image5;
        case 6:
          Image image6 = new Image();
          image6 = CreateImage(image6, "Static_comet6", size);
          return image6;
        case 7:
          Image image7 = new Image();
          image7 = CreateImage(image7, "Static_comet7", size);
          return image7;
        default:
          Image image8 = new Image();
          image8 = CreateImage(image8, "Static_comet8", size);
          return image8;
      }
    }

    public static Image CreateImage(Image img, String path, int size)
    {
      // img.Source = new BitmapImage(new Uri("AsteroidsAtari;component/" + path, UriKind.RelativeOrAbsolute));
      // img.Source = AsteroidsAtari.MainWindow.Instance.Resources[path] as BitmapImage;
      img.Source = Application.Current.TryFindResource(path) as BitmapImage;
      img.Height = size;
      RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.LowQuality);

      if (!AsteroidsAtari.MainWindow.CanvasMain.Children.Contains(img))
      {
        AsteroidsAtari.MainWindow.CanvasMain.Children.Add(img);
      }
      return img;
    }

    public static Vector RandomSlope()
    {
      var slopeX = 0d;
      var slopeY = 0d;

      var num = new Random().Next(0, 2);
      if (num == 0)
      {
        while (Math.Abs(slopeX) < 0.01 && Math.Abs(slopeY) < 0.1)
        {
          slopeX = (new Random().NextDouble()) * -1;
          slopeY = new Random().NextDouble();
        }
      }
      else if (num == 1)
      {
        while (Math.Abs(slopeX) < 0.01 && Math.Abs(slopeY) < 0.1)
        {
          slopeX = new Random().NextDouble();
          slopeY = (new Random().NextDouble()) * -1;
        }
      }
      else
      {
        while (Math.Abs(slopeX) < 0.01 && Math.Abs(slopeY) < 0.1)
        {
          slopeX = (new Random().NextDouble()) * -1;
          slopeY = (new Random().NextDouble()) * -1;
        }
      }

      return new Vector(slopeX / 60, slopeY / 60);
    }

    public static Vector UFORandomPosition()
    {
      var vector = new Vector();
      vector.X = 1110;
      vector.Y = new Random().Next(50, 550);

      return vector;
    }


    public static void ExploionStateAt(Image img, int state)
    {
      switch (state)
      {
        case 0:
          img.Source = Application.Current.TryFindResource("Static_explosion1") as BitmapImage;
          break;
        case 1:
          img.Source = Application.Current.TryFindResource("Static_explosion2") as BitmapImage;
          break;
        case 2:
          img.Source = Application.Current.TryFindResource("Static_explosion3") as BitmapImage;
          break;
        case 3:
          img.Source = Application.Current.TryFindResource("Static_explosion4") as BitmapImage;
          break;
        default:
          img.Source = Application.Current.TryFindResource("Static_explosion5") as BitmapImage;
          break;
      }
    }

    // UI
    public static void StartLabel_Flicker(Label label)
    {
      if (label.Visibility == Visibility.Hidden)
      {
        label.Visibility = Visibility.Visible;
      }
      else
      {
        label.Visibility = Visibility.Hidden;
      }
    }

    public static void PlayerBoost_Flicker(Image boost)
    {
      if (boost.Visibility == Visibility.Hidden)
      {
        boost.Visibility = Visibility.Visible;
      }
      else
      {
        boost.Visibility = Visibility.Hidden;
      }
    }

  }
}
