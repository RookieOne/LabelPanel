using System.Windows;

namespace JBsLabelPanel
{
    public class HorizontalStrategy : ILabelPanelOrientationStrategy
    {
        public void AddVisual(UIElement element, ILabelPanel labelPanel)
        {
            if (labelPanel.NeedAnotherRow(1))
                labelPanel.AddRow();

            int row = labelPanel.GetLastRow();

            labelPanel.AddColumnPair();

            int col = labelPanel.GetLastColumn();

            labelPanel.AddVisualToGrid(row, col - 1, labelPanel.GetLabel(element));
            labelPanel.AddVisualToGrid(row, col, element);
        }
    }
}