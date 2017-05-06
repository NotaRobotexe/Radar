using System;
using System.Collections.Generic;
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
using System.IO.Ports;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media.Effects;

namespace radar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort mySerialPort;
        string data;
        float distance,distanceNormal=536;
        int angle=0;

        float[,] border = new float[6,181];
        float[] intruderBorder = new float[181];

        bool borderIsComplete = false;
        bool isDrawed = false;
        bool test = false;

        Line line;
        Line line2;
        Polyline bor;

        Objects[] obj;
        Ellipse[] ell;
        Ellipse[] intr; 

        public MainWindow()
        {
            InitializeComponent();
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri(@"radar.png", UriKind.Relative));
            canvas.Background = ib;

            obj = new Objects[181];
            ell = new Ellipse[181];
            intr = new Ellipse[181]; 

            lineSettings1();
            lineSetting2();
            borderSetting();
            intruderSetting();

            mySerialPort = new SerialPort(texbox1.Text);
            mySerialPort.BaudRate = 9600;
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            for (int i = 1; i < 181; i++)
             {
                float Testangle = (i * Convert.ToSingle(Math.PI)) / 180;
                obj[i] = new Objects();
                ell[i] = new Ellipse();               

                obj[i].opacityCount1 = (180 - i)*2;
                obj[i].opacityCount2 = i*2;
                obj[i].opacityCount1Copy = (180 - i) * 2;
                obj[i].opacityCount2Copy = i * 2;

                obj[i].opacityCount1Decreasing = 1 / obj[i].opacityCount1;
                obj[i].opacityCount2Decreasing = 1 / obj[i].opacityCount2;

                obj[i].initialization();
                ell[i] = obj[i].ellipse;
                ell[i].Width = 12;
                ell[i].Height = 12;
                /*Canvas.SetTop(ell[i], (536f - Math.Sin(0) * (distanceNormal-30)));
                Canvas.SetLeft(ell[i], (Math.Cos(0) * (distanceNormal-30) + 536));

                canvas.Children.Add(ell[i]);*/
             }


            dis1.Content = (float.Parse(distanceGlobal.Text, System.Globalization.CultureInfo.InvariantCulture) / 4).ToString() + "cm";
            dis2.Content = (float.Parse(distanceGlobal.Text, System.Globalization.CultureInfo.InvariantCulture) / 4 * 2).ToString() + "cm";
            dis3.Content = (float.Parse(distanceGlobal.Text, System.Globalization.CultureInfo.InvariantCulture) / 4 * 3).ToString() + "cm";
            dis4.Content = (float.Parse(distanceGlobal.Text, System.Globalization.CultureInfo.InvariantCulture) / 4 * 4).ToString() + "cm";

            line.Y2 = 536f - Math.Sin(0) * distanceNormal;
            line.X2 = Math.Cos(0) * distanceNormal + 536;

            line2.Y2 = 536f - Math.Sin(0) * distanceNormal;
            line2.X2 = Math.Cos(0) * distanceNormal + 536;


            canvas.Children.Add(line);
            canvas.Children.Add(line2);
        }


        private void StopCOM(object sender, RoutedEventArgs e) //stopCOM is actually startCOM and startCOM is stopCOM I don't
        {                                                       // know why but this is how it work from now.
           
            try
            {
                mySerialPort.Open();
            }
            catch
            {
            }

            try
            {
                if (mySerialPort.IsOpen)
                {
                    mySerialPort.WriteLine(textbox2.Text);
                    mySerialPort.WriteLine("-1");
                    
                }
            }
            catch { }
        }

        private void startCOM(object sender, RoutedEventArgs e)
        {
            if (mySerialPort.IsOpen)
            {
                mySerialPort.WriteLine("-2");
                distance = 0;
                angle = 0;

                canvas.Children.Remove(bor);
                for (int i = 0; i < 181; i++)
                {
                    canvas.Children.Remove(ell[i]);
                    canvas.Children.Remove(intr[i]);
                }               
            }
        }


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string indata = sp.ReadLine();              
                data = indata;
            }
            catch
            {

            }

            Dispatcher.Invoke(() =>
            {
                printDataAndConvertDataToInt();

                if (angle == 180)
                {
                    test = true;
                }
                else if (angle == 0)
                {
                    test = false;
                }

                if(test == true)
                {
                    angle += 10;
                }
                if (angle <=180) {
                    if (!checkBox.IsChecked.Value)
                    {
                        draw();
                        objectDraw();
                    }
                    else
                    {
                        if (borderIsComplete == false)
                        {
                            scan_status.Content = "finding border ..";
                            scanBorder();
                            draw();
                            scan_status.Content = "finding border ...";
                        }
                        else
                        {
                            if (isDrawed == false)
                            {
                                scan_status.Content = "done";
                                drawBorder();
                                isDrawed = true;
                            }
                            else
                            {
                                draw();
                                drawIntruder();
                            }

                        }
                    }
                }
            });
            
        }        

        private void datachanget(object sender, KeyEventArgs e)
        {                        
            try
            {
                dis1.Content = (float.Parse(distanceGlobal.Text, System.Globalization.CultureInfo.InvariantCulture) / 4).ToString() + "cm";
                dis2.Content = (float.Parse(distanceGlobal.Text, System.Globalization.CultureInfo.InvariantCulture) / 4 * 2).ToString() + "cm";
                dis3.Content = (float.Parse(distanceGlobal.Text, System.Globalization.CultureInfo.InvariantCulture) / 4 * 3).ToString() + "cm";
                dis4.Content = (float.Parse(distanceGlobal.Text, System.Globalization.CultureInfo.InvariantCulture) / 4 * 4).ToString() + "cm";
            }
            catch { }
        }

        private void printDataAndConvertDataToInt()
        {
            string[] values = data.Split('*');
            //Console.WriteLine(values[0] + "/*/"+ values[1]);
            values[0] = values[0].Replace(".", ",");
            //values[1] = values[1].Replace(".", ",");
            label1.Content = values[0] + "cm";
            label2.Content = values[1] + "°";

            distance = float.Parse(values[0]);
            Int32.TryParse(values[1],out angle);
        }

        private void draw()
        {
            canvas.Children.Remove(line);
            canvas.Children.Remove(line2);
            float angleLocal = (angle * Convert.ToSingle(Math.PI))/180;

            line.Y2 = 536f - Math.Sin(angleLocal) * distanceNormal;
            line.X2 = Math.Cos(angleLocal) * distanceNormal+536;

            line2.Y2 = 536f - Math.Sin(angleLocal) * distanceNormal;
            line2.X2 = Math.Cos(angleLocal) * distanceNormal + 536;


            canvas.Children.Add(line);
            canvas.Children.Add(line2);

        }

        private void objectDraw()
        {
            float maxDistance = float.Parse(dis4.Content.ToString().Replace("cm", ""));


            if (distance <= maxDistance)
            {
                float ratio = 536 / maxDistance;

                float localAngle = (angle * Convert.ToSingle(Math.PI)) / 180;

               if (canvas.Children.Contains(ell[angle]))
                {
                    canvas.Children.Remove(ell[angle]);
                }

                try
                {
                    Canvas.SetTop(ell[angle], 536f - (Math.Sin(localAngle) * distance * ratio));
                    Canvas.SetLeft(ell[angle], 536 + (Math.Cos(localAngle) * distance *ratio));

                    canvas.Children.Add(ell[angle]);
                }
                catch
                {
                }

               // Console.WriteLine(angle + " " + (536f - (Math.Sin(localAngle) * distance * ratio)) + " " + (536 + (Math.Cos(localAngle) * distance * ratio + " "+ distance)));
            }
            else
            {
                canvas.Children.Remove(ell[angle]);
            }

            /*for (int i = 0; i < 181; i++) // if i want do slowly fading 
            {
                if (canvas.Children.Contains(ell[i]))
                {
                    if (obj[i].opacityCount1 == -1 && obj[i].goingLeft == true) {
                        obj[i].goingRight = true;
                        obj[i].goingLeft = false;
                        obj[i].opacityCount1 = obj[i].opacityCount1Copy;
                    }
                    else if (obj[i].opacityCount2 == -1 && obj[i].goingRight == true)
                    {
                        obj[i].goingRight = false;
                        obj[i].goingLeft = true;
                        obj[i].opacityCount2 = obj[i].opacityCount2Copy;
                    }

                    if (obj[i].goingLeft)
                    {
                       left(i);
                    }
                    else if (obj[i].goingRight)
                    {
                       right(i);
                    }

                }
            }*/
        }

        /* private void left(int i) // if i want do slowly fading 
         {
             obj[i].opacityCount1--;
             canvas.Children.Remove(ell[i]);
             ell[i].Opacity = 1 - obj[i].opacityCount1Decreasing * (obj[i].opacityCount1Copy - obj[i].opacityCount1);
             canvas.Children.Add(ell[i]);
         }

         private void right(int i)
         {
             obj[i].opacityCount2--;
             canvas.Children.Remove(ell[i]);
             ell[i].Opacity = 1 - obj[i].opacityCount2Decreasing * (obj[i].opacityCount2Copy - obj[i].opacityCount2);
             canvas.Children.Add(ell[i]);
         }*/

    
        private void scanBorder()
        {
            float maxDistance = float.Parse(dis4.Content.ToString().Replace("cm", ""));

            if (border[0,180].ToString() == "0")
            {
                if (distance > maxDistance)
                {
                    border[0, angle] = maxDistance;
                }
                else
                {
                    border[0,angle] = distance;
                }               
            }
            else if (border[1, 0].ToString() == "0")
            {
                if (distance > maxDistance)
                {
                    border[1, angle] = maxDistance;
                }
                else
                {
                    border[1, angle] = distance;
                }                
            }
            else if (border[2, 180].ToString() == "0")
            {
                if (distance > maxDistance)
                {
                    border[2, angle] = maxDistance;
                }
                else
                {
                    border[2, angle] = distance;
                }                
            }
            else if (border[3, 0].ToString() == "0")
            {
                if (distance > maxDistance)
                {
                    border[3, angle] = maxDistance;
                }
                else
                {
                    border[3, angle] = distance;
                }                
            }
            else if (border[4, 180].ToString() == "0")
            {
                if (distance > maxDistance)
                {
                    border[4, angle] = maxDistance;
                }
                else
                {
                    border[4, angle] = distance;
                }
            }
            else if (border[5, 0].ToString() == "0")
            {
                if (distance > maxDistance)
                {
                    border[5, angle] = maxDistance;
                }
                else
                {
                    border[5, angle] = distance;
                }
            }
            else
            {
                borderIsComplete = true;
            }
        }

        private void drawIntruder()
        {
            short confirmed = 0;
            if (distance < (border[0, angle] - 5))
            {
                confirmed++;
            }
            if (distance < (border[1, angle] - 5))
            {
                confirmed++;
            }
            if (distance < (border[2, angle] - 5))
            {
                confirmed++;
            }
            if (distance < (border[3, angle] - 5))
            {
                confirmed++;
            }
            if (distance < (border[4, angle] - 5))
            {
                confirmed++;
            }
            if (distance < (border[5, angle] - 5))
            {
                confirmed++;
            }


            if (confirmed == 6)
            {
                float maxDistance = float.Parse(dis4.Content.ToString().Replace("cm", ""));
                
                float ratio = 536 / maxDistance;

                float localAngle = (angle * Convert.ToSingle(Math.PI)) / 180;
            
                if (canvas.Children.Contains(intr[angle]))
                {
                    canvas.Children.Remove(intr[angle]);
                }

               try
                {
                    Canvas.SetTop(intr[angle], 536f - (Math.Sin(localAngle) * distance * ratio));
                    Canvas.SetLeft(intr[angle], 536 + (Math.Cos(localAngle) * distance * ratio));

                    canvas.Children.Add(intr[angle]);
               }
               catch { }

                    // Console.WriteLine(angle + " " + (536f - (Math.Sin(localAngle) * distance * ratio)) + " " + (536 + (Math.Cos(localAngle) * distance * ratio + " "+ distance)));
               
                
            }
            else if (confirmed == 0)
            {
                if (canvas.Children.Contains(intr[angle]))
                {
                    canvas.Children.Remove(intr[angle]);
                }
            }
            

        }

        private void drawBorder() {            
            Point[] po = new Point[181];
            PointCollection polygonPoints = new PointCollection();

            float maxDistance = float.Parse(dis4.Content.ToString().Replace("cm", ""));
            float ratio = 536 / maxDistance;

            for (int i = 0; i < 181; i++)
            {
                float angleLocal = (i * Convert.ToSingle(Math.PI)) / 180;
                float avgdis = 0;

                for (int a = 0; a < 6; a++)
                {
                    avgdis += border[a, i];                    
                }

                avgdis /= 6;

                intruderBorder[i] = avgdis;
                po[i] = new Point(Convert.ToSingle((Math.Cos(angleLocal) * avgdis*ratio) + 536f), Convert.ToSingle(536f - (Math.Sin(angleLocal) * avgdis*ratio)));
                polygonPoints.Add(po[i]);
            }
            bor.Points = polygonPoints;
            canvas.Children.Add(bor);
        }

        private void intruderSetting()
        {
            SolidColorBrush br = new SolidColorBrush();
            br.Color = Color.FromRgb(235, 12, 12);

            DropShadowEffect she = new DropShadowEffect();
            she.Color = Color.FromScRgb(0, 0, 2.3f, 255);
            she.ShadowDepth = 0;
            she.Direction = 0;
            she.BlurRadius = 20;

            for (int i = 0; i < 181; i++)
            {
                intr[i] = new Ellipse();

                intr[i].Width = 12;
                intr[i].Height = 12;
                intr[i].StrokeThickness = 4;
                intr[i].Stroke = br;
                intr[i].Fill = br;
               // intr[i].Effect = she;

            }
        }

        private void borderSetting()
        {
            DropShadowEffect she = new DropShadowEffect();
            she.Color = Color.FromScRgb(255, 0, 2.3f, 0);
            she.ShadowDepth = 0;
            she.Direction = 0;
            she.BlurRadius = 20;

            SolidColorBrush Brush = new SolidColorBrush();
            Brush.Color = Color.FromRgb(127, 255, 0);

            bor = new Polyline();
            bor.Effect = she;
            bor.Stroke = Brush;
            bor.StrokeThickness = 5;
        }

        private void lineSettings1()
        {
            line = new Line();
            line.X1 = 536;
            line.Y1 = 531;

            DropShadowEffect she = new DropShadowEffect();
            she.Color = Color.FromScRgb(255, 0, 2.3f, 0);
            she.ShadowDepth = 0;
            she.Direction = 0;
            she.BlurRadius = 20;

            SolidColorBrush Brush = new SolidColorBrush();
            Brush.Color = Color.FromRgb(127, 255, 0);

            line.Effect = she;
            line.Stroke = Brush;
            line.StrokeThickness = 5;

        }

        private void lineSetting2()
        {
            line2 = new Line();
            line2.X1 = 536;
            line2.Y1 = 531;

            DropShadowEffect she2 = new DropShadowEffect();
            she2.Color = Color.FromScRgb(255, 0, 2, 0);
            she2.ShadowDepth = 0;
            she2.Direction = 0;
            she2.BlurRadius = 80;

            SolidColorBrush Brush2 = new SolidColorBrush();
            Brush2.Color = Color.FromRgb(127, 255, 0);

            line2.Effect = she2;
            line2.Stroke = Brush2;
            line2.StrokeThickness = 5;
        }
    
    }
}
