using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace radar
{
    class Objects
    {
        public Ellipse ellipse;


        public float opacityCount1;
        public float opacityCount2;
        public float opacityCount1Copy;
        public float opacityCount2Copy;

        public float opacityCount1Decreasing;
        public float opacityCount2Decreasing;

        public bool goingLeft = true;
        public bool goingRight = false;

        public void initialization()
        {
            SolidColorBrush br = new SolidColorBrush();
            br.Color = Color.FromRgb(127, 255, 0);

            DropShadowEffect she = new DropShadowEffect();
            she.Color = Color.FromScRgb(255, 0, 2.3f, 0);
            she.ShadowDepth = 0;
            she.Direction = 0;
            she.BlurRadius = 20;

            ellipse = new Ellipse();
            ellipse.StrokeThickness = 4;
            ellipse.Stroke = br;
            ellipse.Fill = br;
            ellipse.Effect = she;

        }




    }
}
