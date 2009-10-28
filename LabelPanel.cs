using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace JBsLabelPanel
{
    public class LabelPanel : Panel, ILabelPanel
    {
        static LabelPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LabelPanel),
                                                     new FrameworkPropertyMetadata(typeof (LabelPanel)));

            LabelProperty = DependencyProperty.RegisterAttached("Label",
                                                                typeof (object),
                                                                typeof (LabelPanel));

            OrientationProperty = DependencyProperty.Register("Orientation",
                                                              typeof (Orientation),
                                                              typeof (LabelPanel),
                                                              new PropertyMetadata(Orientation.Vertical,
                                                                                   OnOrientationChanged));
        }

        public LabelPanel()
        {
            _ActualChildren = new List<UIElement>();

            _OrientationStrategy = new VerticalStrategy();
        }

        public static DependencyProperty LabelProperty;
        public static DependencyProperty OrientationProperty;
        readonly List<UIElement> _ActualChildren;

        Grid _Grid;
        bool _GridMustBeRebuilt;
        ILabelPanelOrientationStrategy _OrientationStrategy;

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public void AddVisualToGrid(int row, int col, UIElement visual)
        {
            Grid.SetRow(visual, row);
            Grid.SetColumn(visual, col);

            _Grid.Children.Add(visual);
        }

        public Label GetLabel(UIElement visual)
        {
            var label = new Label();
            var binding = new Binding();
            binding.Source = visual;
            binding.Path = new PropertyPath(LabelProperty);

            label.SetBinding(ContentControl.ContentProperty, binding);

            return label;
        }

        public int GetLastRow()
        {
            return _Grid.RowDefinitions.Count - 1;
        }

        public int GetLastColumn()
        {
            return _Grid.ColumnDefinitions.Count - 1;
        }

        public void AddRow()
        {
            var rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;

            _Grid.RowDefinitions.Add(rowDefinition);
        }

        public bool NeedAnotherColumnPair(int col)
        {
            return _Grid.ColumnDefinitions.Count < col;
        }

        public bool NeedAnotherRow(int row)
        {
            return _Grid.RowDefinitions.Count < row;
        }

        public void AddColumnPair()
        {
            var labelColumn = new ColumnDefinition();
            labelColumn.Width = GridLength.Auto;
            _Grid.ColumnDefinitions.Add(labelColumn);

            var contentColumn = new ColumnDefinition();
            contentColumn.Width = new GridLength(1, GridUnitType.Star);
            _Grid.ColumnDefinitions.Add(contentColumn);
        }

        static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var labelPanel = d as LabelPanel;

            if (labelPanel == null)
                return;

            labelPanel._OrientationStrategy = labelPanel.GetStrategy();
            labelPanel._GridMustBeRebuilt = true;
        }

        public static object GetLabel(DependencyObject obj)
        {
            return obj.GetValue(LabelProperty);
        }

        public static void SetLabel(DependencyObject obj, object value)
        {
            obj.SetValue(LabelProperty, value);
        }

        ILabelPanelOrientationStrategy GetStrategy()
        {
            switch (Orientation)
            {
                case Orientation.Vertical:
                    return new VerticalStrategy();

                case Orientation.Horizontal:
                    return new HorizontalStrategy();

                default:
                    return new VerticalStrategy();
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            if (visualAdded == _Grid)
                return;

            if (visualAdded != null)
            {
                var uiElement = visualAdded as UIElement;

                Children.Remove(uiElement);
                _ActualChildren.Add(uiElement);
                _GridMustBeRebuilt = true;
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _Grid;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_GridMustBeRebuilt)
                BuildGrid();

            _Grid.Measure(availableSize);

            return _Grid.DesiredSize;
        }

        void BuildGrid()
        {
            if (_Grid != null)
            {
                Children.Remove(_Grid);
                _Grid.Children.Clear();
            }


            _Grid = new Grid();
            Children.Add(_Grid);

            foreach (UIElement child in _ActualChildren)
                _OrientationStrategy.AddVisual(child, this);

            _GridMustBeRebuilt = false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _Grid.Arrange(new Rect(finalSize));

            return base.ArrangeOverride(finalSize);
        }
    }
}