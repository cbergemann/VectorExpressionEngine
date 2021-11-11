using System.Windows;
using SimpleCalculator.Models;

namespace SimpleCalculator.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainModel MainModel => (MainModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();

            MainModel.HistoryRecalled += MainModel_HistoryRecalled;
            MainModel.ResultAdded += MainModel_ResultAdded;
        }

        private void MainModel_ResultAdded(object sender, System.EventArgs e)
        {
            resultScrollView.ScrollToEnd();
        }

        private void MainModel_HistoryRecalled(object sender, System.EventArgs e)
        {
            expressionInputBox.CaretIndex = expressionInputBox.Text.Length;
        }
    }
}
