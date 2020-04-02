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

namespace FlightSimulatorApp.Controls {
    using System.Runtime.CompilerServices;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Interaction logic for Joystick.xaml
    /// </summary>
    public partial class Joystick : UserControl {
        private bool mousePressed = false;

        private double toX;

        private double toY;

        private Point mouseInEllipse;

        private readonly Point ellipseCenter;

        private double borderRadius;

        private Point knobCenter;

        public delegate void CoordinatesChangedEvent(double x, double y);

        public event CoordinatesChangedEvent CoordinatesChanged;

        public Joystick() {
            InitializeComponent();
            Storyboard sb = this.Knob.Resources["MoveKnob"] as Storyboard;
            DoubleAnimation animX = sb.Children[0] as DoubleAnimation;
            DoubleAnimation animY = sb.Children[1] as DoubleAnimation;
            animX.From = 0;
            animY.From = 0;
            this.knobCenter = new Point(this.Base.Width / 2, this.Base.Height / 2);
            this.ellipseCenter = new Point(this.borderEllipse.Width / 2, this.borderEllipse.Height / 2);
            this.borderRadius = this.borderEllipse.Width / 2;
        }
        

        private void JoyStick_MouseUp(object sender, MouseButtonEventArgs e) {
            if (this.mousePressed) {
                this.moveKnobToCenter();
            }
        }

        private void moveKnobBase() {
            Storyboard sb = this.Knob.Resources["MoveKnob"] as Storyboard;
            DoubleAnimation x = sb.Children[0] as DoubleAnimation;
            DoubleAnimation y = sb.Children[1] as DoubleAnimation;
            x.To = this.toX - this.knobCenter.X;
            y.To = this.toY - this.knobCenter.Y;
            sb.Begin(this);
            x.From = x.To;
            y.From = y.To;
        }

        private void joystickMoveValueTranslation() {
            double normalX = (this.mouseInEllipse.X - this.ellipseCenter.X) / this.borderRadius;
            double normalY = (this.mouseInEllipse.Y - this.ellipseCenter.Y) / this.borderRadius;
            if (this.CoordinatesChanged != null) {
                this.CoordinatesChanged(normalX, -normalY);
            }
        }

        private void JoyStick_MouseMove(object sender, MouseEventArgs e) {
            if (this.mousePressed) {
                this.toX = e.GetPosition(this.Base).X;
                this.toY = e.GetPosition(this.Base).Y;
                this.mouseInEllipse = e.GetPosition(this.borderEllipse);
                if (!this.knobOutOfBound()) {
                    this.moveKnobBase();
                    this.joystickMoveValueTranslation();
                } else {
                    this.moveKnobToCenter();
                }
            }
        }

        private void slider_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Slider slider = sender as Slider;
            slider.Value = 0;
        }

        private bool knobOutOfBound() {
            double bound = Math.Pow(this.toX - this.knobCenter.X, 2) / Math.Pow(this.borderEllipse.Width / 2, 2)
                           + Math.Pow(this.toY - this.knobCenter.Y, 2) / Math.Pow(this.borderEllipse.Height / 2, 2);
            return bound > 1;
        }

        private void moveKnobToCenter() {
            this.mousePressed = false;
            this.toX = this.knobCenter.X;
            this.toY = this.knobCenter.Y;
            this.mouseInEllipse = this.ellipseCenter;
            this.moveKnobBase();
            this.joystickMoveValueTranslation();
        }

        private void borderEllipse_MouseLeave(object sender, MouseEventArgs e) {
            if (this.mousePressed) {
                this.moveKnobToCenter();
            }
        }

        public void lostFocus() {
            if (this.mousePressed) {
                this.moveKnobToCenter();
            }
        }

        private void KnobBase_MouseDown(object sender, MouseButtonEventArgs e) {
            this.mousePressed = true;
        }

        public void keyboardPressed(object sender, KeyEventArgs e) {
            if (e.Key == Key.Up) {
                this.throttle.Value += this.throttle.SmallChange;
            }
            else if (e.Key == Key.Down) {
                this.throttle.Value -= this.throttle.SmallChange;
            }
            else if (e.Key == Key.Right) {
                this.rudder.Value += this.rudder.SmallChange;
            }
            else if (e.Key == Key.Left) {
                this.rudder.Value -= this.rudder.SmallChange;
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e) {
            keyboardPressed(sender, e);
        }
    }
}