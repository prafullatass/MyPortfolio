using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPortfolio.Models.ChartModel
{ 
    public static class ModelHelper
    {
        public static List<object> MultiLineData()
        {
            List<object> objs = new List<object>();
            objs.Add(new[] { "x", "sin(x)", "cos(x)", "sin(x)^2" });
            for (int i = 0; i < 70; i++)
            {
                double x =  i;
                objs.Add(new[] { x, x*10, x*5, x*20 });
            }
            return objs;
        }
    }
}
