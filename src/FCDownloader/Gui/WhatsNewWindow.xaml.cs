using System;
using System.Threading.Tasks;
using System.Windows;

namespace AridityTeam.BetaFortressSetup.Gui
{
    /// <summary>
    /// Interaction logic for WhatsNewWindow.xaml
    /// </summary>
    public partial class WhatsNewWindow : Window
    {
        public WhatsNewWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var win = ThreadedWaitDialogWindow.ShowWaitDialog("fetching-update-info-dlg", "Fetching release information...", "Fetching release information...", true, false, true, this);
            Task.Run(async () =>
            {   
                await Task.Delay(2500);

                win.CloseWaitDialog();
            });
        }
    }
}
