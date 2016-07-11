using DataAccessLayer;
using GalaSoft.MvvmLight;
using Shared;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.ComponentModel;

namespace ATTS.ViewModel
{
    public class ATTSContentViewModel : ViewModelBase
    {
        private ObservableCollection<ATTSContents> contentItems { get; set;} = new ObservableCollection<ATTSContents>();
        
        public ATTSContentViewModel()
        {
            this.FillContentItems();         
        }

        public ObservableCollection<ATTSContents> ContentItems
        {
            get
            {
                return contentItems;
            }
            set
            {
                contentItems = value;
                RaisePropertyChanged(() => ContentItems);
            }
        }

        private ATTSContents selectedAttsContent;

        public ATTSContents SelectedAttsContent
        {
            get { return selectedAttsContent; }
            set { Set(nameof(SelectedAttsContent), ref selectedAttsContent, value); }
        }

        public void FillContentItems()
        {
            this.ContentItems.Clear();

            using (var dbContext = new ATTSDbContext())
            {
                try
                {
                    var context = dbContext.DataTables.Select(s => s).ToArray();

                    foreach (var contextItem in context)
                    {
                        this.ContentItems.Add(new ATTSContents
                        {
                            Account = contextItem.Account,
                            Description = contextItem.Description,
                            CurrencyCode = contextItem.CurrencyCode,
                            Value = contextItem.Value,
                            Symbol = contextItem.Symbol
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The database is not available!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}