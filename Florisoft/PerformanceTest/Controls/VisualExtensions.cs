using Avalonia;
using Avalonia.Animation;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest.Controls;

public static class VisualExtensions
{
    class TransitionDisabler : IDisposable
    {
        //static TransitionDisabler _instance = new TransitionDisabler();

        //public static TransitionDisabler Instance => _instance;
        record VisualTransitions(Visual Visual, Transitions Transitions);

        Visual _root;
        List<VisualTransitions>? _visualWithTransitions;

        private TransitionDisabler(Visual root)
        {
            _root = root;
        }

        private IDisposable Disable()
        {
            foreach (var x in _root.GetSelfAndVisualDescendants())
            {
                if (x.Transitions is not null)
                {
                    _visualWithTransitions ??= new List<VisualTransitions>();
                    _visualWithTransitions.Add(new VisualTransitions(x, x.Transitions));
                }
                x.Transitions = null;//?.Clear();
            }

            return this;
        }

        public static IDisposable Disable(Visual root)
        {
            var td = new TransitionDisabler(root);
            return td.Disable();
        }

        public void Enable()
        {
            if (_visualWithTransitions is null || _visualWithTransitions.Count == 0) return;

            foreach(var vt in _visualWithTransitions)
            {
                vt.Visual.Transitions = vt.Transitions;
            }

            _visualWithTransitions.Clear();
        }

        public void Dispose()
        {
            Enable();
        }
    }

    public static IDisposable DisableTransitions(this Visual visual)
    {
        return TransitionDisabler.Disable(visual);
    }

    public static void InvalidateAllMeasures(this Visual visual, Size availableSize)
    {
        //bool test = false;



        ////////if (visual.DataContext?.ToString() == "P3")
        ////////{
        ////////    test = true;
        ////////}

        //if (test)
        //{
        //    if (visual is Avalonia.Controls.Control c)
        //    {
        //        c.Measure(availableSize);

        //        if (c.DesiredSize.Height < 60.0)
        //        {
        //            c.InvalidateMeasure();

        //            c.Measure(availableSize);
        //        }
        //        else
        //        {
        //            test = false;
        //        }
        //    }
        //}

        foreach(var v in visual.GetSelfAndVisualDescendants())
        {
            if( v is Avalonia.Layout.Layoutable l)
            {
                //if (test)
                //{
                //    if (l.IsMeasureValid)
                //    {
                //    }

                //    if (!l.IsVisible)
                //    {

                //    }
                //}
                l.InvalidateMeasure();
            }
        }
    }
}
