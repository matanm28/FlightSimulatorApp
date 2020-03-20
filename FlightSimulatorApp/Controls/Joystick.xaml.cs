using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace FlightSimulatorApp.Controls
{
    using System.Runtime.CompilerServices;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Interaction logic for Joystick.xaml
    /// </summary>
    public partial class Joystick : UserControl
    {
        private bool mousePressed = false;

        private double toX;

        private double toY;

        private Point knobCenter;


        public Joystick()
        {
            InitializeComponent();
            Storyboard sb = this.Knob.Resources["MoveKnob"] as Storyboard;
            DoubleAnimation animX = sb.Children[0] as DoubleAnimation;
            DoubleAnimation animY = sb.Children[1] as DoubleAnimation;
            animX.From = 0;
            animY.From = 0;
            this.knobCenter = new Point(this.Base.Width / 2, this.Base.Height / 2);
        }

        private void JoyStick_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.mousePressed)
            {
                this.moveKnobToCenter();
            }
        }

        private void moveKnobBase()
        {
            Storyboard sb = this.Knob.Resources["MoveKnob"] as Storyboard;
            DoubleAnimation x = sb.Children[0] as DoubleAnimation;
            DoubleAnimation y = sb.Children[1] as DoubleAnimation;
            x.To = this.toX - this.knobCenter.X;
            y.To = this.toY - this.knobCenter.Y;
            sb.Begin(this);
            x.From = x.To;
            y.From = y.To;
        }

        private void KnobBase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.mousePressed = true;
        }

        private void JoyStick_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.mousePressed)
            {
                this.toX = e.GetPosition(this.Base).X;
                this.toY = e.GetPosition(this.Base).Y;
                if (!this.knobOutOfBound())
                {
                    this.moveKnobBase();
                }
                else
                {
                    this.moveKnobToCenter();
                }
            }
        }

        private bool knobOutOfBound()
        {
            double bound = Math.Pow(this.toX - this.knobCenter.X, 2) / Math.Pow(this.borderEllipse.Width / 2, 2) +
                           Math.Pow(this.toY - this.knobCenter.Y, 2) / Math.Pow(this.borderEllipse.Height / 2, 2);
            return bound > 1;
        }



        private void moveKnobToCenter()
        {
            this.mousePressed = false;
            this.toX = this.knobCenter.X;
            this.toY = this.knobCenter.Y;
            this.moveKnobBase();
        }

        private void borderEllipse_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.mousePressed)
            {
                this.moveKnobToCenter();
            }
        }

        public void lostFocus()
        {
            if (this.mousePressed)
            {
                this.moveKnobToCenter();
            }
        }

    }
}