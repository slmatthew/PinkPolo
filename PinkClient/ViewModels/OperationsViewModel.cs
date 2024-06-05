using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using PinkClient.Models;
using System.Windows;
using PinkHttpClient;
using System.Windows.Input;
using System.Text.Json;

namespace PinkClient.ViewModels
{
    public class OperationsViewModel : INotifyPropertyChanged
    {
        private static OperationsHttpClient httpClient = new OperationsHttpClient(App.ApiHost, new HttpClient());

        private static EmployeesHttpClient empClient = new EmployeesHttpClient(App.ApiHost, new HttpClient());
        private static UnitsHttpClient uniClient = new UnitsHttpClient(App.ApiHost, new HttpClient());
        private static OperationTypesHttpClient optClient = new OperationTypesHttpClient(App.ApiHost, new HttpClient());

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<Operation> _Items;
        public ObservableCollection<Operation> Items { get { return _Items; } set { _Items = value; OnPropertyChanged(); } }

        private ObservableCollection<Employee> _Employees;
        public ObservableCollection<Employee> Employees { get { return _Employees; } set { _Employees = value; OnPropertyChanged(); } }

        private ObservableCollection<Unit> _Units;
        public ObservableCollection<Unit> Units { get { return _Units; } set { _Units = value; OnPropertyChanged(); } }

        private ObservableCollection<OperationType> _OperationTypes;
        public ObservableCollection<OperationType> OperationTypes { get { return _OperationTypes; } set { _OperationTypes = value; OnPropertyChanged(); } }

        private Operation _SelectedItem;
        public Operation SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged(); }
        }

        private Operation _NewItem;
        public Operation NewItem
        {
            get { return _NewItem; }
            set { _NewItem = value; OnPropertyChanged(); }
        }

        public OperationsViewModel()
        {
            UpdateList();

            NewItem = new Operation()
            {
                EmployeeID = null,
                IssuedAt = DateTime.Now,
            };
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
            Items = new ObservableCollection<Operation>(httpClient.GetAllAsync(App.ApiVersion).Result);

            Employees = new ObservableCollection<Employee>(empClient.GetAllAsync(App.ApiVersion).Result);
            Employees.Add(new Employee() { EmployeeID = 0, FullName = "/пусто/" });

            Units = new ObservableCollection<Unit>(uniClient.GetAllAsync(App.ApiVersion).Result);

            OperationTypes = new ObservableCollection<OperationType>(optClient.GetAllAsync(App.ApiVersion).Result);

            Trace.WriteLine(JsonSerializer.Serialize(Items));
            Trace.WriteLine(JsonSerializer.Serialize(Employees));
            Trace.WriteLine(JsonSerializer.Serialize(Units));
            Trace.WriteLine(JsonSerializer.Serialize(OperationTypes));
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
                var item = (Operation)parameter;
                if(item.EmployeeID == 0)
                {
                    item.EmployeeID = null;
                }

                try
                {
                    httpClient.UpdatePUTAsync(item.OperationID, App.ApiVersion, item).Wait();
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
                var item = (Operation)parameter;

                try
                {
                    httpClient.DeleteAsync(item.OperationID, App.ApiVersion).Wait();
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
                Trace.WriteLine(JsonSerializer.Serialize(NewItem));
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

                        NewItem = new Operation();
                        NewItem.IssuedAt = DateTime.Now;

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
