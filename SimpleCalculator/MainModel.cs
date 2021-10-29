using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculator
{
    public class MainModel
    {
        public ObservableCollection<ResultModel> Results { get; set; } = new ObservableCollection<ResultModel>();
    }
}
