using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace PerformanceMonitor.Controls
{
    public class ContentAdorner : Adorner, IDisposable
    {
        private readonly VisualCollection _children;
        private readonly FrameworkElement _child;
        private Size _constraint;
        private AdornerLayer _adornerLayer;

        private ContentAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _children = new VisualCollection(this);
 
            var contentPresenter = new ContentPresenter();
 
            contentPresenter.SetBinding(ContentPresenter.ContentProperty, new Binding
            {
                Path = new PropertyPath(AdornerContentProperty),
                Source = adornedElement
            });
 
            contentPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding
            {
                Path = new PropertyPath(AdornerContentTemplateProperty),
                Source = adornedElement
            });
 
            _child = contentPresenter;
            _children.Add(_child);
        }
 
        /// <summary>
        /// The AdornerContentTemplate property is used to attach a template for adorner content to a given item.
        /// </summary>
        public static DependencyProperty AdornerContentTemplateProperty =
            DependencyProperty.RegisterAttached("AdornerContentTemplate", typeof (DataTemplate), typeof (ContentAdorner),
                new FrameworkPropertyMetadata(null, OnAdornerContentTemplatePropertyChanged));
 
        public static DataTemplate GetAdornerContentTemplate(DependencyObject element)
        {
            return (DataTemplate) element.GetValue(AdornerContentTemplateProperty);
        }
 
        public static void SetAdornerContentTemplate(DependencyObject element, DataTemplate value)
        {
            element.SetValue(AdornerContentTemplateProperty, value);
        }
 
        public static object GetAdornerContent(DependencyObject obj)
        {
            return obj.GetValue(AdornerContentProperty);
        }
 
        public static void SetAdornerContent(DependencyObject obj, object value)
        {
            obj.SetValue(AdornerContentProperty, value);
        }
 
        public static readonly DependencyProperty AdornerContentProperty =
            DependencyProperty.RegisterAttached("AdornerContent", typeof (object), typeof (ContentAdorner),
                new FrameworkPropertyMetadata(null, OnAdornerContentTemplatePropertyChanged));
 
        public static Visibility GetAdornerVisibility(DependencyObject obj)
        {
            return (Visibility) obj.GetValue(AdornerVisibilityProperty);
        }
 
        public static void SetAdornerVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(AdornerVisibilityProperty, value);
        }
 
        public static readonly DependencyProperty AdornerVisibilityProperty =
            DependencyProperty.RegisterAttached("AdornerVisibility", typeof (Visibility), typeof (ContentAdorner),
                new FrameworkPropertyMetadata(Visibility.Collapsed));
       
        private static void OnAdornerContentTemplatePropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var target = (FrameworkElement) sender;
                var adorner = new ContentAdorner(target);
                adorner.ApplyAdorner(target);
            }
        }
 
        private void ApplyAdorner(FrameworkElement target)
        {
            if (target.IsVisible)
            {
                ApplyContentAdorner(true);
            }
            AdornedElement.IsVisibleChanged += OnAdornerTargetLoaded;
        }

        private void OnAdornerTargetLoaded(object sender, DependencyPropertyChangedEventArgs e)
        {
            ApplyContentAdorner((bool)e.NewValue);
        }
 
        private void ApplyContentAdorner(bool show)
        {
            if (show)
            {
                if (_adornerLayer == null)
                {
                    _adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);

                    SetBinding(VisibilityProperty,
                        new Binding { Source = AdornedElement, Path = new PropertyPath(AdornerVisibilityProperty) });
                }
                _adornerLayer?.Add(this);
            }
            else
            {
                _adornerLayer?.Remove(this);
            }
        }
 
        protected override Visual GetVisualChild(int index)
        {
            return _children[index];
        }
 
        protected override int VisualChildrenCount => _children.Count;

        protected override Size MeasureOverride(Size constraint)
        {
            _constraint = constraint;
            var adornedHeight = AdornedElement.RenderSize.Height;
            var relativeAdornedLocation = AdornedElement.TranslatePoint(new Point(), _adornerLayer);
 
            _child.Measure(new Size(AdornedElement.RenderSize.Width, constraint.Height - adornedHeight - relativeAdornedLocation.Y));
            return new Size(AdornedElement.RenderSize.Width, Math.Max(_child.DesiredSize.Height, adornedHeight));
        }
 
        protected override Size ArrangeOverride(Size finalSize)
        {
            var location = new Point(0, AdornedElement.RenderSize.Height);
            var rect = new Rect(location, new Size(finalSize.Width, Math.Min(finalSize.Height, _constraint.Height)));
            _child.Arrange(rect);
            return _child.RenderSize;
        }
 
        void IDisposable.Dispose()
        {
            _adornerLayer.Remove(this);
        }
    }
}
