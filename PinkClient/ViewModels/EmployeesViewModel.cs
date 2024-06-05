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
    public class EmployeesViewModel : INotifyPropertyChanged
    {
        private static EmployeesHttpClient httpClient = new EmployeesHttpClient(App.ApiHost, new HttpClient());

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<Employee> _Items;
        public ObservableCollection<Employee> Items { get { return _Items; } set { _Items = value; OnPropertyChanged(); } }

        private Employee _SelectedItem;
        public Employee SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged(); }
        }

        private Employee _NewItem;
        public Employee NewItem
        {
            get { return _NewItem; }
            set { _NewItem = value; OnPropertyChanged(); }
        }

        public EmployeesViewModel()
        {
            NewItem = new Employee();

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
            Items = new ObservableCollection<Employee>(httpClient.GetAllAsync(App.ApiVersion).Result);
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
                var item = (Employee)parameter;

                try
                {
                    httpClient.UpdatePUTAsync(item.EmployeeID, App.ApiVersion, item).Wait();
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
                var item = (Employee)parameter;

                try
                {
                    httpClient.DeleteAsync(item.EmployeeID, App.ApiVersion).Wait();
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
                        NewItem = new Employee();

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
