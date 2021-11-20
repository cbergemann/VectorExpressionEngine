using SimpleCalculator.Models;

namespace SimpleCalculator.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private MainModel MainModel => (MainModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();

        MainModel.HistoryRecalled += MainModel_HistoryRecalled;
        MainModel.ResultAdded += MainModel_ResultAdded;
    }

    private void MainModel_ResultAdded(object sender, ResultModel e)
    {
        resultScrollView.ScrollToEnd();
    }

    private void MainModel_HistoryRecalled(object sender, ResultModel e)
    {
        expressionInputBox.CaretIndex = expressionInputBox.Text.Length;
    }
}