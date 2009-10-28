using System.Windows;

namespace JBsLabelPanel
{
    public class VerticalStrategy : ILabelPanelOrientationStrategy
    {
        public void AddVisual(UIElement element, ILabelPanel labelPanel)
        {
            if (labelPanel.NeedAnotherColumnPair(1))
                labelPanel.AddColumnPair();

            labelPanel.AddRow();

            int row = labelPanel.GetLastRow();

            labelPanel.AddVisualToGrid(row, 0, labelPanel.GetLabel(element));
            labelPanel.AddVisualToGrid(row, 1, element);
        }
    }
}