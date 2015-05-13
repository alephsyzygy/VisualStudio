using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
#if DEBUG
using System.Diagnostics;
using FileRenamer.ViewModel;
#endif

namespace FileRenamer.View
{
    /// <summary>
    /// Interaction logic for FileListView.xaml
    /// </summary>
    public partial class FileListView : UserControl
    {
        private Point _startPoint;
        private bool _dragIsOutOfScope;
        private DragAdorner _adorner;
        private AdornerLayer _layer;

        public FileListView()
        {
            InitializeComponent();
        }

        // Drag and drop support from https://fxmax.wordpress.com/2010/10/05/wpf/

        private void FileNameListView_Drop(object sender, DragEventArgs e)
        {
            var viewModel = this.DataContext as FileListViewModel;
            if (e.Data.GetDataPresent("myFormat") && viewModel != null)
            {
                int? fromIndex = e.Data.GetData("myFormat") as int?;
                ListViewItem listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem != null)
                {
                    //object nameToReplace = FileNameListView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                    //int index = FileNameListView.Items.IndexOf(nameToReplace);

                    int toIndex = FileNameListView.ItemContainerGenerator.IndexFromContainer(listViewItem);

                    if (toIndex >= 0 && fromIndex.HasValue)
                    {
                        // Have to setup a command here to move the elements.
                        //FileNameListView.Items.Remove(name);
                        //FileNameListView.Items.Insert(index, name);
#if DEBUG
                        Debug.WriteLine("{0} {1}", fromIndex.Value, toIndex);
#endif
                        viewModel.Move.Command.Execute(new Tuple<int, int>(fromIndex.Value, toIndex));
                    }
                }
                else
                {
                    //FileNameListView.Items.Remove(name);
                    //FileNameListView.Items.Add(name);
                    if (fromIndex.HasValue)
                    {
#if DEBUG
                        Debug.WriteLine("{0}", fromIndex.Value);
#endif
                        viewModel.Move.Command.Execute(new Tuple<int, int>(fromIndex.Value, -1));
                    }
                }
            }
        }

        private void FileNameListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void FileNameListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    BeginDrag(e);
                }
            }
        }

        private void FileNameListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void BeginDrag(MouseEventArgs e)
        {
            ListView listView = this.FileNameListView;
            ListViewItem listViewItem =
                FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

            if (listViewItem == null)
                return;

            // get the data for the ListViewItem
            //object name = listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
            int index = listView.ItemContainerGenerator.IndexFromContainer(listViewItem);

            //setup the drag adorner.
            InitialiseAdorner(listViewItem);

            //add handles to update the adorner.
            listView.PreviewDragOver += FileNameListView_DragOver;
            listView.DragLeave += FileNameListView_DragLeave;
            listView.DragEnter += FileNameListView_DragEnter;

            DataObject data = new DataObject("myFormat", index);
            DragDropEffects de = DragDrop.DoDragDrop(this.FileNameListView, data, DragDropEffects.Move);

            //cleanup
            listView.PreviewDragOver -= FileNameListView_DragOver;
            listView.DragLeave -= FileNameListView_DragLeave;
            listView.DragEnter -= FileNameListView_DragEnter;

            if (_adorner != null)
            {
                AdornerLayer.GetAdornerLayer(listView).Remove(_adorner);
                _adorner = null;
            }
        }

        private void InitialiseAdorner(ListViewItem listViewItem)
        {
            VisualBrush brush = new VisualBrush(listViewItem);
            _adorner = new DragAdorner((UIElement)listViewItem, listViewItem.RenderSize, brush);
            _adorner.Opacity = 0.5;
            _layer = AdornerLayer.GetAdornerLayer(FileNameListView as Visual);
            _layer.Add(_adorner);
        }

        private void ListViewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this._dragIsOutOfScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true;
            }
        }

        private void FileNameListView_DragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == FileNameListView)
            {
                Point point = e.GetPosition(FileNameListView);
                Rect rect = VisualTreeHelper.GetContentBounds(FileNameListView);

                //Check if within range of list view.
                if (!rect.Contains(point))
                {
                    this._dragIsOutOfScope = true;
                    e.Handled = true;
                }
            }
        }

        void FileNameListView_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.OffsetLeft = args.GetPosition(FileNameListView).X;
                _adorner.OffsetTop = args.GetPosition(FileNameListView).Y - _startPoint.Y;
            }
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }


    class DragAdorner : Adorner
    {
        private Rectangle child = null;
        private double offsetLeft = 0;
        private double offsetTop = 0;

        /// <summary>
        /// Initializes a new instance of DragVisualAdorner.
        /// </summary>
        /// <param name="adornedElement">The element being adorned.</param>
        /// <param name="size">The size of the adorner.</param>
        /// <param name="brush">A brush to with which to paint the adorner.</param>
        public DragAdorner(UIElement adornedElement, Size size, Brush brush)
            : base(adornedElement)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = brush;
            rect.Width = size.Width;
            rect.Height = size.Height;
            rect.IsHitTestVisible = false;
            this.child = rect;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.offsetLeft, this.offsetTop));
            return result;
        }


        /// <summary>
        /// Gets/sets the horizontal offset of the adorner.
        /// </summary>
        public double OffsetLeft
        {
            get { return this.offsetLeft; }
            set
            {
                this.offsetLeft = value;
                UpdateLocation();
            }
        }


        /// <summary>
        /// Updates the location of the adorner.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void SetOffsets(double left, double top)
        {
            this.offsetLeft = left;
            this.offsetTop = top;
            this.UpdateLocation();
        }


        /// <summary>
        /// Gets/sets the vertical offset of the adorner.
        /// </summary>
        public double OffsetTop
        {
            get { return this.offsetTop; }
            set
            {
                this.offsetTop = value;
                UpdateLocation();
            }
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        /// <summary>
        /// Override.  Always returns 1.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }


        private void UpdateLocation()
        {
            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
                adornerLayer.Update(this.AdornedElement);
        }
    }


    // Found online:
    // https://social.msdn.microsoft.com/Forums/vstudio/en-US/80e09a3e-ddc0-4f37-aab8-743cceb364af/how-can-i-set-gridviewcolumns-width-as-relative-in-listview-in-wpf?forum=wpf
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int columnsCount = System.Convert.ToInt32(parameter);
            // Subtract some distance for scollbars.  There must be a better way of doing this.
            double width = (double)value - 1.6 * SystemParameters.VerticalScrollBarWidth;
            return width / columnsCount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
