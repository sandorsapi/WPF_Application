using GalaSoft.MvvmLight;

namespace ATTS.ViewModel
{
    public class MainWindowViewModel :ViewModelBase
    {
        private ATTSMenuViewModel ATTSMenuViewModel;
        private ATTSContentViewModel ATTSContentViewModel;
        
        public MainWindowViewModel()
        {
            this.ATTSContentViewModel = new ATTSContentViewModel();
            this.ATTSMenuViewModel = new ATTSMenuViewModel(this.ATTSContentViewModel);            
        }

        public ATTSMenuViewModel ContextATTSMenuViewModel
        {
            get { return ATTSMenuViewModel; }
            set { Set(nameof(ContextATTSMenuViewModel), ref ATTSMenuViewModel, value); }
        }
        public ATTSContentViewModel ContextATTSContentViewModel
        {
            get { return ATTSContentViewModel; }
            set { Set(nameof(ContextATTSContentViewModel), ref ATTSContentViewModel, value); }
        }
    }
}