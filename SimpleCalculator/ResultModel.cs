using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculator
{
    public enum ResultType
    {
        Value,
        LineSeries,
        Error,
        Null,
    }

    public class ResultModel
    {
        public string Expression { get; set; }

        public string Result { get; set; }

        public PlotModel LineSeries { get; set; }

        public ResultType Type { get; set; }
    }
}
