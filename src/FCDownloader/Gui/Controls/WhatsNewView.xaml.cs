using System;
using ReactiveUI;
using System.Windows;
using System.Windows.Controls;

using AridityTeam.BetaFortressSetup.Gui.ViewModels;

namespace AridityTeam.BetaFortressSetup.Gui.Controls
{
    /// <summary>
    /// Interaction logic for WhatsNewView.xaml
    /// </summary>
    public partial class WhatsNewView : ReactiveUserControl<WhatsNewViewModel>
    {
        public WhatsNewView()
        {
            InitializeComponent();
        }
    }
}
