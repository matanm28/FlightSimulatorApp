using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlightSimulatorApp.Controls {
    using System.Windows.Media.Animation;

    /// <summary>
    /// Interaction logic for Joystick.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class Joystick : UserControl {
        /// <summary>
        /// indicates if mouse button is pressed
        /// </summary>
        private bool mousePressed = false;

        /// <summary>
        /// To x
        /// </summary>
        private double toX;

        /// <summary>
        /// To y
        /// </summary>
        private double toY;

        /// <summary>
        /// The mouse in ellipse
        /// </summary>
        private Point mouseInEllipse;

        /// <summary>
        /// The ellipse center
        /// </summary>
        private readonly Point ellipseCenter;

        /// <summary>
        /// The border radius
        /// </summary>
        private double borderRadius;

        /// <summary>
        /// The knob center
        /// </summary>
        private readonly Point knobCenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Joystick"/> class.
        /// </summary>
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

        /// <summary>
        /// The x property
        /// </summary>
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
            "X",
            typeof(double),
            typeof(Joystick),
            new PropertyMetadata(default(double)));

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public double X {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// The y property
        /// </summary>
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
            "Y",
            typeof(double),
            typeof(Joystick),
            new PropertyMetadata(default(double)));

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public double Y {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// a method to apply if this instance lost focus
        /// </summary>
        public void InstanceLostFocus() {
            if (this.mousePressed) {
                this.moveKnobToCenter();
            }
        }

        /// <summary>
        /// Handles the MouseUp event of the JoyStick control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void JoyStick_MouseUp(object sender, MouseButtonEventArgs e) {
            if (this.mousePressed) {
                this.moveKnobToCenter();
            }
        }

        /// <summary>
        /// Moves the knob base.
        /// </summary>
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

        /// <summary>
        /// Joysticks the move value translation.
        /// </summary>
        private void joystickMoveValueTranslation() {
            double normalX = (this.mouseInEllipse.X - this.ellipseCenter.X) / this.borderRadius;
            double normalY = (this.mouseInEllipse.Y - this.ellipseCenter.Y) / this.borderRadius;
            this.X = normalX;
            this.Y = normalY;
        }

        /// <summary>
        /// Handles the MouseMove event of the JoyStick control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// checks if knob circle is out of bounds.
        /// </summary>
        /// <returns></returns>
        private bool knobOutOfBound() {
            double bound = Math.Pow(this.toX - this.knobCenter.X, 2) / Math.Pow(this.borderEllipse.Width / 2, 2)
                           + Math.Pow(this.toY - this.knobCenter.Y, 2) / Math.Pow(this.borderEllipse.Height / 2, 2);
            return bound > 1;
        }

        /// <summary>
        /// Moves the knob to center.
        /// </summary>
        private void moveKnobToCenter() {
            this.mousePressed = false;
            this.toX = this.knobCenter.X;
            this.toY = this.knobCenter.Y;
            this.mouseInEllipse = this.ellipseCenter;
            this.moveKnobBase();
            this.joystickMoveValueTranslation();
        }

        /// <summary>
        /// Handles the MouseLeave event of the borderEllipse control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void borderEllipse_MouseLeave(object sender, MouseEventArgs e) {
            if (this.mousePressed) {
                this.moveKnobToCenter();
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the KnobBase control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void KnobBase_MouseDown(object sender, MouseButtonEventArgs e) {
            this.mousePressed = true;
        }
    }
}