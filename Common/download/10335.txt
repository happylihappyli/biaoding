

using Test1;
using Timer = System.Threading.Timer;

namespace ToastNotifications
{


    public partial class Notification : Form
    {
        private static readonly List<Notification> OpenNotifications = new List<Notification>();
        private bool _allowFocus;
        private readonly FormAnimator _animator;
        private IntPtr _currentForegroundWindow;
        private int duration = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="duration"></param>
        /// <param name="animation"></param>
        /// <param name="direction"></param>
        public Notification(string title, string body, int duration, FormAnimator.AnimationMethod animation, FormAnimator.AnimationDirection direction)
        {
            InitializeComponent();

            if (duration < 0)
                this.duration = 10;
            else
                this.duration = duration;// * 1000;


            labelTitle.Text = title;
            labelBody.Text = body;

            _animator = new FormAnimator(this, animation, direction, 500);

            Region = Region.FromHrgn(NativeMethods.CreateRoundRectRgn(0, 0, Width - 5, Height - 5, 20, 20));

        }


        private void OnTick(object sender, EventArgs e)
        {
            // 关闭窗口
            this.Close();
        }



        /// <summary>
        /// Displays the form
        /// </summary>
        /// <remarks>
        /// Required to allow the form to determine the current foreground window before being displayed
        /// </remarks>
        public async new void Show()
        {
            _currentForegroundWindow = NativeMethods.GetForegroundWindow();

            base.Show();


            var autoEvent = new AutoResetEvent(false);

            StatusChecker statusChecker = new StatusChecker(duration);

            Console.WriteLine("{0:h:mm:ss.fff} Creating timer.\n",
                              DateTime.Now);

            if (statusChecker.CheckStatus != null)
            {
                var stateTimer = new Timer(statusChecker.CheckStatus, autoEvent, 0, 1000);

                autoEvent.WaitOne();
                autoEvent.Reset();
                stateTimer.Dispose();
            }
            this.Close();


        }


        private void Notification_Load(object sender, EventArgs e)
        {
            // Display the form just above the system tray.
            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Width,
                                      Screen.PrimaryScreen.WorkingArea.Height - Height);



            Action act = delegate ()
            {
                foreach (Notification openForm in OpenNotifications)
                {
                    openForm.Top -= Height;
                }
            };
            this.BeginInvoke(act, null);



            OpenNotifications.Add(this);
            //lifeTimer.Start();
        }

        private void Notification_Activated(object sender, EventArgs e)
        {
            // Prevent the form taking focus when it is initially shown
            if (!_allowFocus)
            {
                // Activate the window that previously had focus
                NativeMethods.SetForegroundWindow(_currentForegroundWindow);
            }
        }

        private void Notification_Shown(object sender, EventArgs e)
        {
            // Once the animation has completed the form can receive focus
            _allowFocus = true;

            // Close the form by sliding down.
            _animator.Duration = 0;
            _animator.Direction = FormAnimator.AnimationDirection.Down;
        }

        private void Notification_FormClosed(object sender, FormClosedEventArgs e)
        {


            Action act = delegate ()
            {
                foreach (Notification openForm in OpenNotifications)
                {
                    if (openForm == this)
                    {
                        break;
                    }
                    openForm.Top += Height;
                }
            };
            this.BeginInvoke(act, null);

            try
            {
                OpenNotifications.Remove(this);
            }catch (Exception ex)
            {
                Main.WriteLine(ex.ToString());
            }
        }

        private void lifeTimer_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void Notification_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void labelRO_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}