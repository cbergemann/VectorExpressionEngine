using OxyPlot;
using OxyPlot.Wpf;
using SimpleCalculator.Helper;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VectorExpressionEngine;

namespace SimpleCalculator.Models
{

    public class ResultModel
    {
        public static ResultModel FromExpression(Context ctx, string expression)
        {
            var resultEntry = new ResultModel
            {
                Expression = expression,
            };

            object result;

            try
            {
                var nodes = Parser.Parse(expression);
                result = nodes.Select(node => node.Eval(ctx)).Last();
                ctx.AssignVariable("ans", result);
            }
            catch (SyntaxException ex)
            {
                resultEntry.Result = "Error: " + ex.Message;
                resultEntry.Type = ResultType.Error;

                return resultEntry;
            }

            if (result is double doubleResult)
            {
                resultEntry.Result = doubleResult.ToString();
                resultEntry.Type = ResultType.Value;

                return resultEntry;
            }
            
            if (result is string stringResult)
            {
                resultEntry.Result = "'" + stringResult + "'";
                resultEntry.Type = ResultType.Value;

                return resultEntry;
            }

            if (result is bool boolResult)
            {
                resultEntry.Result = boolResult ? "true" : "false";
                resultEntry.Type = ResultType.Value;

                return resultEntry;
            }
            
            if (result is double[] doubleArrayResult)
            {
                resultEntry.Result = "[" + string.Join(", ", doubleArrayResult.Select(r => r.ToString("G5"))) + "]";
                resultEntry.Type = ResultType.Value;

                return resultEntry;
            }

            if (result is string[] stringArrayResult)
            {
                resultEntry.Result = "['" + string.Join("', '", stringArrayResult.Select(r => r.ToString())) + "']";
                resultEntry.Type = ResultType.Value;

                return resultEntry;
            }

            if (result is bool[] boolArrayResult)
            {
                resultEntry.Result = "[" + string.Join(", ", boolArrayResult.Select(r => r ? "true" : "false")) + "]";
                resultEntry.Type = ResultType.Value;

                return resultEntry;
            }
            
            if (result is LineSeries lineSeriesResult)
            {
                resultEntry.Type = ResultType.LineSeries;
                var serie = new OxyPlot.Series.LineSeries();
                serie.Points.AddRange(lineSeriesResult.X.Zip(lineSeriesResult.Y, (x, y) => new DataPoint(x, y)));
                resultEntry.LineSeries = new PlotModel();
                resultEntry.LineSeries.Series.Add(serie);

                return resultEntry;
            }
            
            if (result == null)
            {
                resultEntry.Type = ResultType.Null;
                return resultEntry;
            }

            resultEntry.Result = "Unknown Result!!!";
            resultEntry.Type = ResultType.Error;
            return resultEntry;
        }

        public ResultModel()
        {
            CopyResultToClipboard = new RelayCommand(CopyResultToClipboard_Executed, CopyResultToClipboard_CanExecute);
        }

        public ICommand CopyResultToClipboard { get; }

        public bool CopyResultToClipboard_CanExecute(object parameter)
        {
            return Type == ResultType.Value || Type == ResultType.LineSeries || Type == ResultType.Error;
        }

        public void CopyResultToClipboard_Executed(object parameter)
        {
            switch (Type)
            {
                case ResultType.Value:
                case ResultType.Error:
                    Clipboard.SetText(Result);
                    return;

                case ResultType.LineSeries:
                    var pngExporter = new PngExporter { Width = 600, Height = 400, Background = OxyColors.White };
                    var bitmap = pngExporter.ExportToBitmap(LineSeries);
                    Clipboard.SetImage(bitmap);
                    return;

                default:
                    return;
            }
        }

        public string Expression { get; set; }

        public string Result { get; set; }

        public PlotModel LineSeries { get; set; }

        public ResultType Type { get; set; }
    }
}
