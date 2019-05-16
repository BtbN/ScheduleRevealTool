using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace ScheduleRevealTool
{
    public class IgnoreDpiDecorator : Decorator
    {
        public IgnoreDpiDecorator()
        {
            Loaded += (s, e) =>
            {
                Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
                if (dpiTransform.CanFreeze)
                    dpiTransform.Freeze();
                LayoutTransform = dpiTransform;
            };
        }
    }
}
