namespace PerformanceMonitor.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            Categories = new CategoriesViewModel();
            Counters = new CountersViewModel(Categories);
        }

        public CategoriesViewModel Categories { get; }

        public CountersViewModel Counters { get; set; }
    }
}
