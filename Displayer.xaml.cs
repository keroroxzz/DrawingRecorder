using System.Windows;
using System.Windows.Input;

namespace DRnamespace
{
    public partial class Displayer : Window
    {
        public Displayer()
        {
            InitializeComponent();

            Visibility = Visibility.Visible;
            Hide();
            ShowInTaskbar = false;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
