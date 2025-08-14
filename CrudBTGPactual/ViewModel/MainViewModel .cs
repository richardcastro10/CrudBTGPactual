using CrudBTGPactual.Models;
using CrudBTGPactual.Service;
using CrudBTGPactual.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrudBTGPactual.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IClientService _clientService;
        private Client? _selectedClient;

        public MainViewModel(IClientService clientService)
        {
            _clientService = clientService;
            Clients = _clientService.GetClients();

            AddClientCommand = new RelayCommand(async _ => await AddClient());
            EditClientCommand = new RelayCommand(async _ => await EditClient(), _ => SelectedClient != null);
            DeleteClientCommand = new RelayCommand(async _ => await DeleteClient(), _ => SelectedClient != null);
            RefreshCommand = new RelayCommand(_ => RefreshClients());
        }

        public ObservableCollection<Client> Clients { get; }

        public Client? SelectedClient
        {
            get => _selectedClient;
            set
            {
                SetProperty(ref _selectedClient, value);
                ((RelayCommand)EditClientCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteClientCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddClientCommand { get; }
        public ICommand EditClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand RefreshCommand { get; }

        private async Task AddClient()
        {
            try
            {
                var clientFormPage = new ClientFormPage();
                var viewModel = new ClientFormViewModel(_clientService, null);
                clientFormPage.BindingContext = viewModel;

                // Usar navegação modal no .NET MAUI
                await Application.Current!.MainPage!.Navigation.PushModalAsync(clientFormPage);

                // O refresh será feito quando a página modal for fechada
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    Device.BeginInvokeOnMainThread(() => RefreshClients());
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro ao abrir formulário: {ex.Message}", "OK");
            }
        }

        private async Task EditClient()
        {
            if (SelectedClient == null) return;

            try
            {
                // Criar uma cópia do cliente para edição
                var clientToEdit = new Client
                {
                    Id = SelectedClient.Id,
                    Name = SelectedClient.Name,
                    Lastname = SelectedClient.Lastname,
                    Age = SelectedClient.Age,
                    Address = SelectedClient.Address
                };

                var clientFormPage = new ClientFormPage();
                var viewModel = new ClientFormViewModel(_clientService, clientToEdit);
                clientFormPage.BindingContext = viewModel;

                // Usar navegação modal no .NET MAUI
                await Application.Current!.MainPage!.Navigation.PushModalAsync(clientFormPage);

                // O refresh será feito quando a página modal for fechada
                // Vamos aguardar um pouco e depois atualizar
                _ = Task.Run(async () =>
                {
                    await Task.Delay(500);
                    Device.BeginInvokeOnMainThread(() => RefreshClients());
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro ao abrir formulário: {ex.Message}", "OK");
            }
        }

        private async Task DeleteClient()
        {
            if (SelectedClient == null) return;

            try
            {
                bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                    "Confirmar Exclusão",
                    $"Deseja realmente excluir o cliente {SelectedClient.FullName}?",
                    "Sim",
                    "Cancelar");

                if (confirm)
                {
                    _clientService.DeleteClient(SelectedClient.Id);
                    SelectedClient = null;
                    RefreshClients();
                    await Application.Current!.MainPage!.DisplayAlert("Sucesso", "Cliente excluído com sucesso!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro ao excluir cliente: {ex.Message}", "OK");
            }
        }

        private void RefreshClients()
        {
            // A ObservableCollection já notifica automaticamente as mudanças
            OnPropertyChanged(nameof(Clients));
        }


    }
}
