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
    public class UnitsViewModel : INotifyPropertyChanged
    {
        private static UnitsHttpClient httpClient = new UnitsHttpClient(App.ApiHost, new HttpClient());
        private static CompaniesHttpClient comClient = new CompaniesHttpClient(App.ApiHost, new HttpClient());

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<Unit> _Items;
        public ObservableCollection<Unit> Items { get { return _Items; } set { _Items = value; OnPropertyChanged(); } }


        private ObservableCollection<Company> _Companies;
        public ObservableCollection<Company> Companies { get { return _Companies; } set { _Companies = value; OnPropertyChanged(); } }


        private Unit _SelectedItem;
        public Unit SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged(); }
        }

        private Unit _NewItem;
        public Unit NewItem
        {
            get { return _NewItem; }
            set { _NewItem = value; OnPropertyChanged(); }
        }

        public UnitsViewModel()
        {
            NewItem = new Unit();

            UpdateList();
        }

        public ICommand UpdateListCommand
        {
            get
            {
                return new DelegateCommand(UpdateList);
            }
        }


        private void UpdateList(object? _ = null)
        {
            Items = new ObservableCollection<Unit>(httpClient.GetAllAsync(App.ApiVersion).Result);

            Companies = new ObservableCollection<Company>(comClient.GetAllAsync(App.ApiVersion).Result);
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
                var item = (Unit)parameter;

                try
                {
                    httpClient.UpdatePUTAsync(item.UnitID, App.ApiVersion, item).Wait();
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
                var item = (Unit)parameter;

                try
                {
                    httpClient.DeleteAsync(item.UnitID, App.ApiVersion).Wait();
                }
                catch (AggregateException ex)
                {
                    var innerEx = ex.InnerException;
                    if (innerEx is ApiException apiEx)
                    {
                        if (apiEx.StatusCode == 204)
                        {
                            Items.Remove(item);

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
                        Items.Add(NewItem);
                        NewItem = new Unit();

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
