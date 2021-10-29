using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using VectorExpressionEngine;

namespace SimpleCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var lib = new MathLibrary();
            var refCtx = new ReflectionContext(lib);
            lib.Context = refCtx;
            _ctx = refCtx;
        }

        private Context _ctx;

        private void EvaluateExpression(string expression)
        {
            var resultEntry = new ResultModel
            {
                Expression = expression,
            };

            try
            {
                var nodes = Parser.Parse(expression);
                ////nodes.PrintNodes();
                ////TreeOptimizer.Optimize(nodes, _ctx);
                ////nodes.PrintNodes();
                var result = nodes.Select(node => node.Eval(_ctx)).Last();

                if (result is double)
                {
                    resultEntry.Result = ((double)result).ToString();
                    resultEntry.Type = ResultType.Value;
                }
                else if (result is string)
                {
                    resultEntry.Result = "'" + ((string)result) + "'";
                    resultEntry.Type = ResultType.Value;
                }
                else if (result is bool)
                {
                    resultEntry.Result = ((bool)result) ? "true" : "false";
                    resultEntry.Type = ResultType.Value;
                }
                else if (result is double[])
                {
                    var rda = (double[])result;
                    resultEntry.Result = "[" + string.Join(", ", rda.Select(r => r.ToString("G5"))) + "]";
                    resultEntry.Type = ResultType.Value;
                }
                else if (result is string[])
                {
                    var rda = (string[])result;
                    resultEntry.Result = "['" + string.Join("', '", rda.Select(r => r.ToString())) + "']";
                    resultEntry.Type = ResultType.Value;
                }
                else if (result is bool[])
                {
                    var rda = (bool[])result;
                    resultEntry.Result = "[" + string.Join(", ", rda.Select(r => r ? "true" : "false")) + "]";
                    resultEntry.Type = ResultType.Value;
                }
                else if (result is LineSeries)
                {
                    var rls = (LineSeries)result;

                    resultEntry.Type = ResultType.LineSeries;
                    var serie = new OxyPlot.Series.LineSeries();
                    serie.Points.AddRange(rls.X.Zip(rls.Y, (x, y) => new OxyPlot.DataPoint(x, y)));
                    resultEntry.LineSeries = new OxyPlot.PlotModel();
                    resultEntry.LineSeries.Series.Add(serie);
                }
                else if (result == null)
                {
                    resultEntry.Type = ResultType.Null;
                }
                else
                {
                    resultEntry.Result = "Unknown Result!!!";
                    resultEntry.Type = ResultType.Error;
                }
            }
            catch (SyntaxException ex)
            {
                resultEntry.Result = "Error: " + ex.Message;
                resultEntry.Type = ResultType.Error;
            }

            var model = (MainModel)DataContext;
            model.Results.Add(resultEntry);

            resultScrollView.ScrollToEnd();
        }

        private void ExpressionInputBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                var exp = ExpressionInputBox.Text.Trim();
                ExpressionInputBox.Text = string.Empty;

                if (!string.IsNullOrEmpty(exp))
                {
                    EvaluateExpression(exp);
                }
            }
        }
    }
}
