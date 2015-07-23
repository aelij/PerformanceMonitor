using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Avalon.Windows.Controls;
using Avalon.Windows.Utility;
using PerformanceMonitor.ViewModels;

namespace PerformanceMonitor.Views
{
    /// <summary>
    /// Interaction logic for SearchCategoryView.xaml
    /// </summary>
    public partial class SearchCategoryView
    {
        private InlineModalDialog _dialog;

        public SearchCategoryView()
        {
            InitializeComponent();

            DataContextChanged += (sender, args) =>
            {
                var viewModel = args.NewValue as CategoriesViewModel;
                if (viewModel != null)
                {
                    viewModel.Closed += () => _dialog?.Close();
                    viewModel.SelectedSearchResults = SearchResultsList.SelectedItems as ObservableCollection<object>;
                }
                PopupContent.DataContext = args.NewValue;
            };
        }

        public CategoriesViewModel ViewModel => (CategoriesViewModel)DataContext;

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                SearchResultsList.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                ViewModel.SearchText = string.Empty;
            }
        }

        private void SearchResultsList_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.Enter)
            {
                var element = e.OriginalSource as FrameworkElement;
                var item = element?.FindVisualAncestor(t => t is ListBox || t is ListBoxItem) as ListBoxItem;
                if (item != null)
                {
                    var searchResultItem = (SearchResultItem)item.DataContext;
                    Publish(searchResultItem);
                }
            }
        }

        private void Close()
        {
            ViewModel.SearchText = string.Empty;
            SearchTextBox.Focus();
        }

        private void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var searchResultItem = (SearchResultItem)((FrameworkElement)sender).DataContext;
            Publish(searchResultItem);
        }

        private void Publish(SearchResultItem searchResultItem)
        {
            if (searchResultItem.IsValid)
            {
                ViewModel.PublishAdd(searchResultItem.Key);
                Close();
            }
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            _dialog = new InlineModalDialog
            {
                Owner = this,
                Content = new CategoriesView { DataContext = ViewModel },
                Margin = new Thickness(40),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            _dialog.InputBindings.Add(new KeyBinding(InlineModalDialog.CloseCommand, Key.Escape, ModifierKeys.None));
            _dialog.Show();
        }
    }
}
