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

namespace PlayerM
{
   public class ManagePlayer
   {
      static double speedChecker = 0;
      public static float Rise;
      public static float Run;
      public static float LastRise;
      public static float LastRun;
      static double speedDrag = 0;
      static double speedDragLast = 0;
      public static double angle;
      static int temp = 0;

      static float drag = 0;
      static float dragLast = 0;

      
      public static void UpdateRotation(Image player, Point PlayerPoint, Point MousePoint , Image playerBoost)
      {

         var mouseX = MousePoint.X;
         var mouseY = PlayerPoint.Y;
         var playerX = PlayerPoint.X;
         var playerY = MousePoint.Y;

         var hypotenuse = Math.Sqrt(Math.Pow(mouseX - playerX, 2) + Math.Pow(mouseY - playerY, 2));
         var opposite = mouseY - playerY; 
         angle = Math.Sin(opposite / hypotenuse);

         if (mouseX >= playerX)
         {
            angle = 90 - (angle * 100);
         }
         else if (mouseX < playerX)
         {
            angle = ((angle * 100)) - 90;
         }

         if ((MousePoint.X - PlayerPoint.X < 130 && MousePoint.X - PlayerPoint.X > -130) && (PlayerPoint.Y - MousePoint.Y < 130 && PlayerPoint.Y - MousePoint.Y > -130))
         {
         }
         else
         {
            RotateTransform rotateTransform = new RotateTransform(angle);
            player.RenderTransform = rotateTransform;
            playerBoost.RenderTransform = rotateTransform;

            var d = Math.Sqrt( Math.Pow(mouseX - playerX, 2) + Math.Pow(mouseY - playerY, 2) );
            var t = 0.2/d;

            var pointX = (1-t)*playerX + t*mouseX;
            var pointY = (1-t)*playerY + t*mouseY;
            
            Run = (float)(pointX - playerX);
            Rise = (float)(pointY - playerY);      

                  
         }

      }

      public static void Move(Image player, double speed)
      {
         Manage.BorderPatrole(player);
         temp = 1;
         if (speedChecker != speed)
         {
            speedDrag += 0.000005d;
            speedChecker = speed;
         }
         var rise = (Rise) * speedDrag;
         var run = (Run) * speedDrag;

         Canvas.SetLeft(player, Canvas.GetLeft(player) + run);
         Canvas.SetBottom(player, Canvas.GetBottom(player) + rise);

      }

      public static void OverrideDrag(Image player, double speed)
      {
         Manage.BorderPatrole(player);
         temp = 0;
         if (speedChecker != speed && speedDrag > 0)
         {
            speedDrag -= 0.00001d;
            speedDragLast += 0.00001d;
            speedChecker = speed;
         }
         else
         {
            speedDrag = speedDragLast;
            speedDragLast = 0;
            AsteroidsAtari.MainWindow.isDrag = false;
         }

         var rise = Rise * (speedDragLast) + (LastRise * speedDrag);
         var run = Run * (speedDragLast) + (LastRun * speedDrag);

         Canvas.SetBottom(player, Canvas.GetBottom(player) + rise);
         Canvas.SetLeft(player, Canvas.GetLeft(player) + run);
      }

      public static void Stop(Image player, double speed)
      {
         Manage.BorderPatrole(player);
         if (speedDragLast > speedDrag && temp == 0)
         {
            speedDrag = speedDragLast;
            temp++;
         }
   
         if (speedChecker != speed)
         {
            if (speedDrag > 0)
            {
               speedDrag -= 0.000001d;
            }
            if (speedDragLast > 0)
            {
               speedDragLast = 0;
            }
            if (speedDrag <= 0)
            {
               AsteroidsAtari.MainWindow.isDrag = false;
            }
            speedChecker = speed;
         }

         var rise = (LastRise) * (speedDrag + speedDragLast);
         var run = (LastRun) * (speedDrag + speedDragLast);

         Canvas.SetBottom(player, Canvas.GetBottom(player) + rise);
         Canvas.SetLeft(player, Canvas.GetLeft(player) + run);

      }

      public static void ResetPlayer()
      {
         Rise = 0;
         Run = 0;
         LastRise = 0;
         LastRun = 0;
         speedDrag = 0;
         speedDragLast = 0;
         angle = 0;
         Canvas.SetLeft(AsteroidsAtari.MainWindow.Player, 515);
         Canvas.SetBottom(AsteroidsAtari.MainWindow.Player, 140);
      }

      public static int OperatorOf(double num)
      {
         if (num < 0)
         {
            return -1;
         }
         else
         {
            return 1;
         }
      }

      public static void ResetRotation(Image player)
      {
         RotateTransform rotateTransform = new RotateTransform(90);
         player.RenderTransform = rotateTransform;
      }

   }
}
