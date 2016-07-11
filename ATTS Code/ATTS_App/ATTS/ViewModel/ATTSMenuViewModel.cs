using DataAccessLayer;
using DataAccessLayer.Entities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace ATTS.ViewModel
{
    public class ATTSMenuViewModel : ViewModelBase
    {
        private List<ATTSContents> dataLists { get; set; } = new List<ATTSContents>();
        private string[] dataTitle;
        private ATTSContentViewModel ATTSContentViewModel;
        public ATTSMenuViewModel(ATTSContentViewModel ATTSContentViewModel)
        {
            this.ATTSContentViewModel = ATTSContentViewModel;

            this.LoadFileCommand = new RelayCommand(this.LoadFile);
            this.SaveDatabaseCommand = new RelayCommand(this.SaveDatabase);
        }

        private int progressValue;

        public int ProgressValue
        {
            get { return progressValue; }
            set { Set(nameof(ProgressValue), ref progressValue, value); }
        }

        public RelayCommand LoadFileCommand { get; private set; }

        private void LoadFile()
        {
            this.ProgressValue = 0;

            OpenFileDialog openfiledialog = new OpenFileDialog();

            openfiledialog.Filter = "CSV files (*.csv)| *.csv| Excel files (*.xls)| *.xls";

            if (openfiledialog.ShowDialog() == true)
            {
                string[] files = openfiledialog.FileName.Split('.');
                if (files[1] == "csv")
                {
                    CsvProgress(openfiledialog.FileName);
                }
                else
                {
                    ExcelProgress(openfiledialog.FileName);
                }
            }
        }

        public RelayCommand SaveDatabaseCommand { get; private set; }

        private void SaveDatabase()
        {
            this.ProgressValue = 0;

            DataTable dt = new DataTable();

            if (dataLists.Count != 0)
            {
                using (var dbContext = new ATTSDbContext())
                {
                    try
                    {
                        foreach (var dataListItem in dataLists)
                        {
                            dt.Account = dataListItem.Account;
                            dt.Description = dataListItem.Description;
                            dt.CurrencyCode = dataListItem.CurrencyCode;
                            dt.Value = dataListItem.Value;
                            dt.Symbol = dataListItem.Symbol;

                            dbContext.DataTables.Add(dt);
                            dbContext.SaveChanges();

                            this.ProgressValue += 1;
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("The database is not available!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                this.ATTSContentViewModel.FillContentItems();
                MessageBox.Show("Recording of data", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                this.ProgressValue = 0;
            }
            else
            {
                MessageBox.Show("No processed data!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CsvProgress(string fileName)
        {
            this.dataLists.Clear();
            string[] data;
            int i = 0;
            string symbol;
            string[] separator = { ";" };
            string[] dataRow = System.IO.File.ReadAllLines(fileName);
            while (i != dataRow.Length)
            {
                for (; i < dataRow.Length; i++)
                {
                    if (dataRow[i] != "")
                    {
                        if (i == 0)
                        {
                            this.dataTitle = dataRow[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        }
                        else
                        {
                            data = dataRow[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (CurrencyCode.TryGetCurrencySymbol(data[2], out symbol))
                            {
                                this.dataLists.Add(new ATTSContents
                                {
                                    Account = data[0],
                                    Description = data[1],
                                    CurrencyCode = data[2],
                                    Value = Convert.ToInt64(data[3]),
                                    Symbol = symbol
                                });
                                this.ProgressValue += 1;
                                Thread.Sleep(100);
                            }
                            else
                            {
                                MessageBox.Show(string.Format("The current code '{0}' is wrong", data[2]), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
                MessageBox.Show("Processed file", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                this.ProgressValue = 0;
            }
        }

        private void ExcelProgress(string filename)
        {
            this.dataLists.Clear();
            int i = 2;
            int j = 1;
            string symbol;
            if (File.Exists(filename))
            {
                try
                {
                    Excel.Application excelApp = new Excel.Application();
                    Excel.Workbook excelWorkbook;
                    Excel.Worksheet excelWorksheet;
                    object misValue = System.Reflection.Missing.Value;

                    excelWorkbook = excelApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    excelWorksheet = excelWorkbook.ActiveSheet;

                    int rows = excelWorksheet.UsedRange.Rows.Count;
                    int columns = excelWorksheet.UsedRange.Columns.Count;
                    object[,] data = excelWorksheet.Range[excelWorksheet.Cells[1, 1], excelWorksheet.Cells[rows, columns]].Cells.Value2;

                    for (; i < rows + 1; i++)
                    {
                        if (!data.Equals(null))
                        {
                            if (CurrencyCode.TryGetCurrencySymbol(data[i, 3].ToString(), out symbol))
                            {
                                this.dataLists.Add(new ATTSContents
                                {
                                    Account = data[i, 1].ToString(),
                                    Description = data[i, 2].ToString(),
                                    CurrencyCode = data[i, 3].ToString(),
                                    Value = Convert.ToInt64(data[i, 4].ToString()),
                                    Symbol = symbol
                                });
                                this.ProgressValue += 1;
                                Thread.Sleep(100);
                            }
                            else
                            {
                                MessageBox.Show(string.Format("The current code '{0}' is wrong", data[i, 3].ToString()), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                    excelWorkbook.Close(true, misValue, misValue);
                    excelApp.Quit();

                    MessageBox.Show("Processed file", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.ProgressValue = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error excel file processing!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("The file does not exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}