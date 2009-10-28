using System.Windows;

namespace JBsLabelPanel
{
    public interface ILabelPanelOrientationStrategy
    {
        void AddVisual(UIElement element, ILabelPanel labelPanel);
    }
}