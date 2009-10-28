using System.Windows;
using System.Windows.Controls;

namespace JBsLabelPanel
{
    public interface ILabelPanelLayoutStrategy
    {
        void AddVisual(Label label, UIElement element, ILabelPanelGridFacade grid);
    }
}