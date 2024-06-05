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
    public class OperationTypesViewModel : INotifyPropertyChanged
    {
        private static OperationTypesHttpClient httpClient = new OperationTypesHttpClient(App.ApiHost, new HttpClient());

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<OperationType> _Items;
        public ObservableCollection<OperationType> Items { get { return _Items; } set { _Items = value; OnPropertyChanged(); } }

        private OperationType _SelectedItem;
        public OperationType SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged(); }
        }

        private OperationType _NewItem;
        public OperationType NewItem
        {
            get { return _NewItem; }
            set { _NewItem = value; OnPropertyChanged(); }
        }

        public OperationTypesViewModel()
        {
            NewItem = new OperationType();

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
            Items = new ObservableCollection<OperationType>(httpClient.GetAllAsync(App.ApiVersion).Result);
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
                var item = (OperationType)parameter;

                try
                {
                    httpClient.UpdateAsync(item.OperationTypeID, App.ApiVersion, item).Wait();
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
                var item = (OperationType)parameter;

                try
                {
                    httpClient.DeleteAsync(item.OperationTypeID, App.ApiVersion).Wait();
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
                        NewItem = new OperationType();

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
