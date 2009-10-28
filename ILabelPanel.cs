using System.Windows;
using System.Windows.Controls;

namespace JBsLabelPanel
{
    public interface ILabelPanel
    {
        bool NeedAnotherColumnPair(int col);
        void AddColumnPair();
        void AddRow();
        int GetLastRow();
        void AddVisualToGrid(int row, int col, UIElement visual);
        Label GetLabel(UIElement visual);
        bool NeedAnotherRow(int row);
        int GetLastColumn();
    }
}