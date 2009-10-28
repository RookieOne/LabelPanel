using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace JBsLabelPanel
{
    public class LabelPanel : Panel
    {
        static LabelPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LabelPanel),
                                                     new FrameworkPropertyMetadata(typeof (LabelPanel)));

            LabelProperty = DependencyProperty.RegisterAttached("Label",
                                                                typeof (object),
                                                                typeof (LabelPanel));
        }

        public LabelPanel()
        {
            _Grid = new Grid();

            Children.Add(_Grid);
        }

        public static DependencyProperty LabelProperty;
        readonly Grid _Grid;

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public static object GetLabel(DependencyObject obj)
        {
            return obj.GetValue(LabelProperty);
        }

        public static void SetLabel(DependencyObject obj, object value)
        {
            obj.SetValue(LabelProperty, value);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualAdded == _Grid)
                return;

            if (visualAdded != null)
            {
                Children.Remove(visualAdded as UIElement);

                var labelRow = visualAdded as LabelRow;

                if (labelRow == null)
                {
                    AddVisual(visualAdded as UIElement);
                }
                else
                {
                    AddLabelRow(labelRow);
                }
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        void AddLabelRow(LabelRow labelRow)
        {
            AddRow();

            int row = GetRow();
            int col = 0;

            foreach (UIElement child in labelRow.Children)
            {
                if (NeedAnotherColumnPair(col))
                    AddColumnPair();

                AddVisualToGrid(row, col, GetLabel(child));
                AddVisualToGrid(row, col + 1, child);

                col += 2;
            }
        }

        void AddVisual(UIElement visual)
        {
            if (NeedAnotherColumnPair(1))
                AddColumnPair();

            AddRow();

            int row = GetRow();

            AddVisualToGrid(row, 0, GetLabel(visual));
            AddVisualToGrid(row, 1, visual);
        }

        void AddVisualToGrid(int row, int col, UIElement visual)
        {
            Grid.SetRow(visual, row);
            Grid.SetColumn(visual, col);

            _Grid.Children.Add(visual);
        }

        Label GetLabel(UIElement visual)
        {
            var label = new Label();
            var binding = new Binding();
            binding.Source = visual;
            binding.Path = new PropertyPath(LabelProperty);

            label.SetBinding(ContentControl.ContentProperty, binding);

            return label;
        }

        int GetRow()
        {
            return _Grid.RowDefinitions.Count - 1;
        }

        void AddRow()
        {
            var rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;

            _Grid.RowDefinitions.Add(rowDefinition);
        }

        bool NeedAnotherColumnPair(int col)
        {
            return _Grid.ColumnDefinitions.Count < col;
        }

        void AddColumnPair()
        {
            var labelColumn = new ColumnDefinition();
            labelColumn.Width = GridLength.Auto;
            _Grid.ColumnDefinitions.Add(labelColumn);

            var contentColumn = new ColumnDefinition();
            contentColumn.Width = new GridLength(1, GridUnitType.Star);
            _Grid.ColumnDefinitions.Add(contentColumn);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _Grid;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _Grid.Measure(availableSize);

            return _Grid.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _Grid.Arrange(new Rect(finalSize));

            return base.ArrangeOverride(finalSize);
        }
    }
}