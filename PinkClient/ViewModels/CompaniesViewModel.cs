using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using PinkClient.Models;
using System.Windows;
using PinkHttpClient;
using System.Windows.Input;

namespace PinkClient.ViewModels
{
    public class CompaniesViewModel : INotifyPropertyChanged
    {
        private static CompaniesHttpClient httpClient = new CompaniesHttpClient(App.ApiHost, new HttpClient());

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<Company> _Companies;
        public ObservableCollection<Company> Companies { get { return _Companies; } set { _Companies = value; OnPropertyChanged(); } }

        private Company _SelectedItem;
        public Company SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged(); }
        }

        private Company _NewItem;
        public Company NewItem
        {
            get { return _NewItem; }
            set { _NewItem = value; OnPropertyChanged(); }
        }

        public CompaniesViewModel()
        {
            NewItem = new Company();

            UpdateList();
        }

        public ICommand UpdateListCommand
        {
            get
            {
                Trace.WriteLine("opa");
                return new DelegateCommand(UpdateList);
            }
        }


        private void UpdateList(object? _ = null)
        {
            Companies = new ObservableCollection<Company>(httpClient.GetAllAsync(App.ApiVersion).Result);
        }

        public DelegateCommand EditItemCommand
        {
            get
            {
                return new DelegateCommand(EditItem);
            }
        }

        private void EditItem(object parameter)
        {
            if (parameter != null)
            {
                var item = (Company)parameter;

                try
                {
                    httpClient.UpdatePUTAsync(item.CompanyID, App.ApiVersion, item).Wait();
                }
                catch (AggregateException ex)
                {
                    var innerEx = ex.InnerException;
                    if (innerEx is ApiException apiEx)
                    {
                        if (apiEx.StatusCode == 204)
                        {
                            UpdateList();
                        }
                        else throw;
                    }
                    else throw;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        public DelegateCommand DeleteItemCommand
        {
            get
            {
                return new DelegateCommand(DeleteItem);
            }
        }

        private void DeleteItem(object parameter)
        {
            if (parameter != null)
            {
                var item = (Company)parameter;

                try
                {
                    httpClient.DeleteAsync(item.CompanyID, App.ApiVersion).Wait();
                }
                catch (AggregateException ex)
                {
                    var innerEx = ex.InnerException;
                    if (innerEx is ApiException apiEx)
                    {
                        if (apiEx.StatusCode == 204)
                        {
                            Companies.Remove(item);

                            UpdateList();
                        }
                        else if (apiEx.StatusCode == 404)
                        {
                            MessageBox.Show("Запись не существует");
                        }
                        else throw;
                    }
                    else throw;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        public DelegateCommand AddItemCommand
        {
            get
            {
                return new DelegateCommand(AddItem);
            }
        }

        private void AddItem(object parameter)
        {
            try
            {
                httpClient.CreateAsync(App.ApiVersion, NewItem).Wait();
            }
            catch (AggregateException ex)
            {
                var innerEx = ex.InnerException;
                if (innerEx is ApiException apiEx)
                {
                    if (apiEx.StatusCode == 201)
                    {
                        Companies.Add(NewItem);
                        NewItem = new Company();

                        UpdateList();
                    }
                    else if (apiEx.StatusCode == 400)
                    {
                        MessageBox.Show("Заполните все поля");
                    }
                    else if (apiEx.StatusCode == 500)
                    {
                        MessageBox.Show("Ошибка сервера: возможно неуникальное значение поля");
                    }
                    else throw;
                }
                else throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
