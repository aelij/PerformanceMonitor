using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PerformanceMonitor.Models;
using PerformanceMonitor.Utilities;

namespace PerformanceMonitor.ViewModels
{
    public class CategoriesViewModel : ViewModel
    {
        private readonly KeyedCollectionProxy<string, CategoryViewModel> _categories;

        private CategoryViewModel _selectedCategory;
        private ObservableCollection<object> _selectedCounters;
        private ObservableCollection<object> _selectedInstances;
        private ObservableCollection<object> _selectedSearchResults;
        private string _searchText;
        private IEnumerable<SearchResultItem> _searchResults;
        private bool _hasSearchResults;

        public CategoriesViewModel()
        {
            _categories = new KeyedCollectionProxy<string, CategoryViewModel>(c => c.Name);
            _categories.CollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            AddCommand = CreateCommand(PublishAdd,
                () => SelectedCategory != null &&
                      SelectedCounters != null && SelectedCounters.Count > 0 &&
                      SelectedInstances != null && SelectedInstances.Count > 0);

            AddSearchResultsCommand = CreateCommand(PublishAddSearchResults,
                () => SelectedSearchResults != null && SelectedSearchResults.OfType<SearchResultItem>().Any(t => t.IsValid));

            CloseCommand = CreateCommand(OnClosed);

            Load();
        }

        public event Action Closed;

        private void PublishAddSearchResults()
        {
            foreach (SearchResultItem item in SelectedSearchResults)
            {
                if (item.IsValid)
                {
                    PublishAdd(item.Key);
                }
            }

            SearchText = null;
        }

        private void PublishAdd()
        {
            foreach (string counter in SelectedCounters)
                foreach (string instance in SelectedInstances)
                    PublishAdd(new CounterKey(SelectedCategory.Name, counter, instance));
        }

        public event Action<CounterKey> Added;

        public void PublishAdd(CounterKey key)
        {
            var handler = Added;
            handler?.Invoke(key);
        }

        public ICommandEx AddCommand { get; }

        public ICommandEx AddSearchResultsCommand { get; }

        public ICommandEx CloseCommand { get; }

        public void Load()
        {
            var categories = PerformanceCounterCategory.GetCategories()
                .Select(t => new CategoryViewModel(t));
            _categories.Load(categories);
        }

        public CategoryViewModel GetCategory(string name)
        {
            CategoryViewModel category;
            _categories.TryGetItem(name, out category);
            return category;
        }

        public ICollectionView Categories => _categories.CollectionView;

        public CategoryViewModel SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                RaiseAndSetIfChanged(ref _selectedCategory, value);
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<object> SelectedCounters
        {
            get { return _selectedCounters; }
            set
            {
                var previous = _selectedCounters;
                if (RaiseAndSetIfChanged(ref _selectedCounters, value))
                {
                    if (previous != null) previous.CollectionChanged -= OnSelectionChanged;
                    if (value != null) value.CollectionChanged += OnSelectionChanged;
                }
            }
        }

        public ObservableCollection<object> SelectedInstances
        {
            get { return _selectedInstances; }
            set
            {
                var previous = _selectedInstances;
                if (RaiseAndSetIfChanged(ref _selectedInstances, value))
                {
                    if (previous != null) previous.CollectionChanged -= OnSelectionChanged;
                    if (value != null) value.CollectionChanged += OnSelectionChanged;
                }
            }
        }

        public ObservableCollection<object> SelectedSearchResults
        {
            get { return _selectedSearchResults; }
            set
            {
                var previous = _selectedSearchResults;
                if (RaiseAndSetIfChanged(ref _selectedSearchResults, value))
                {
                    if (previous != null) previous.CollectionChanged -= OnSelectedSearchResultsChanged;
                    if (value != null) value.CollectionChanged += OnSelectedSearchResultsChanged;
                }
            }
        }

        private void OnSelectedSearchResultsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            AddSearchResultsCommand.RaiseCanExecuteChanged();
        }

        private void OnSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            AddCommand.RaiseCanExecuteChanged();
        }
        
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                RaiseAndSetIfChanged(ref _searchText, value);
                PerformSearch(value);
            }
        }

        private void PerformSearch(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                SelectedSearchResults?.Clear();
                SearchResults = null;
                HasSearchResults = false;
                return;
            }

            var keywords = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var results = new List<SearchResultItem>();
            foreach (var category in _categories)
            {
                var resultWithKeywords = 
                    from counter in category.Counters
                    from instance in category.Instances.DefaultIfEmpty()
                    select new SearchResultItem(keywords, category.Name, counter, instance);

                results.AddRange(resultWithKeywords.Where(result => result.IsMatch));
            }

            SearchResults = results;
            HasSearchResults = results.Any();
        }
        
        public IEnumerable<SearchResultItem> SearchResults
        {
            get { return _searchResults; }
            private set { RaiseAndSetIfChanged(ref _searchResults, value); }
        }

        public bool HasSearchResults
        {
            get { return _hasSearchResults; }
            private set { RaiseAndSetIfChanged(ref _hasSearchResults, value); }
        }

        public EventHandler M
        {
            get { return (o, a) => { }; }
        }

        protected void OnClosed()
        {
            Closed?.Invoke();
        }
    }

    public struct SearchResultItem
    {
        private const string KeywordPrefix = "[b]";
        private const string KeywordPostfix = "[/]";

        public SearchResultItem(IList<string> keywords, string category, string counter, string instance)
        {
            Key = new CounterKey(category, counter, instance);

            var searchText = $"{category} > {counter} > {instance}";
            var keywordItems = keywords.Select(keyword => new KeywordItem(keyword, searchText)).ToArray();
            IsMatch = keywordItems.All(keywordItem => keywordItem.Index >= 0);
            IsValid = false;

            if (IsMatch)
            {
                string instanceName;
                if (instance != null)
                {
                    IsValid = true;
                    instanceName = instance;
                }
                else
                {
                    instanceName = "[i]No instance[/i]";
                }

                var displayTextBeforeHighlight = $"{category} > {counter} > {instanceName}";
                DisplayText = BuildDisplayText(displayTextBeforeHighlight, keywordItems.OrderBy(item => item.Index));
            }
            else
            {
                DisplayText = null;
            }
        }

        private static string BuildDisplayText(string text, IEnumerable<KeywordItem> keywords)
        {
            var sb = new StringBuilder(text);
            var offset = 0;
            foreach (var keywordItem in keywords)
            {
                sb.Insert(keywordItem.Index + keywordItem.Keyword.Length + offset, KeywordPostfix);
                sb.Insert(keywordItem.Index + offset, KeywordPrefix);
                offset += KeywordPostfix.Length + KeywordPrefix.Length;
            }
            var displayText = sb.ToString();
            return displayText;
        }

        public string DisplayText { get; }

        public bool IsMatch { get; }

        public CounterKey Key { get; }

        public bool IsValid { get; }

        struct KeywordItem
        {
            public KeywordItem(string keyword, string text)
            {
                Keyword = keyword;
                Index = text.IndexOf(keyword, StringComparison.InvariantCultureIgnoreCase);
            }

            public string Keyword { get; }

            public int Index { get; }
        }
    }
}
