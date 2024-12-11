using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.ViewModels;

public class ItemViewModel
{
    public string Description { get; set; } = "";
    public string Title { get; set; } = "";
    public int Index { get; set; }

    public override string ToString()
    {
        return $"{Index}; {Description}; {Title}";
    }
}
