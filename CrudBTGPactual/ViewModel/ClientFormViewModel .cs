using CrudBTGPactual.Models;
using CrudBTGPactual.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CrudBTGPactual.ViewModel
{
    public class ClientFormViewModel : BaseViewModel
    {
        private readonly IClientService _clientService;
        private readonly Client? _originalClient;
        private string _name = string.Empty;
        private string _lastname = string.Empty;
        private string _ageText = string.Empty;
        private string _address = string.Empty;
        private bool _isEditing;

        // Inicializar os comandos como propriedades
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public ClientFormViewModel(IClientService clientService, Client? client = null)
        {
            _clientService = clientService;
            _originalClient = client;
            _isEditing = client != null;

            // Inicializar os comandos ANTES de definir as propriedades
            SaveCommand = new RelayCommand(async _ => await SaveClient(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());

            // Agora definir as propriedades
            if (client != null)
            {
                Name = client.Name;
                Lastname = client.Lastname;
                AgeText = client.Age.ToString();
                Address = client.Address;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                // Verificar se SaveCommand não é null antes de chamar RaiseCanExecuteChanged
                if (SaveCommand is RelayCommand saveCmd)
                    saveCmd.RaiseCanExecuteChanged();
            }
        }

        public string Lastname
        {
            get => _lastname;
            set
            {
                SetProperty(ref _lastname, value);
                // Verificar se SaveCommand não é null antes de chamar RaiseCanExecuteChanged
                if (SaveCommand is RelayCommand saveCmd)
                    saveCmd.RaiseCanExecuteChanged();
            }
        }

        public string AgeText
        {
            get => _ageText;
            set
            {
                SetProperty(ref _ageText, value);
                // Verificar se SaveCommand não é null antes de chamar RaiseCanExecuteChanged
                if (SaveCommand is RelayCommand saveCmd)
                    saveCmd.RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(IsAgeValid));
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                SetProperty(ref _address, value);
                // Verificar se SaveCommand não é null antes de chamar RaiseCanExecuteChanged
                if (SaveCommand is RelayCommand saveCmd)
                    saveCmd.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditing => _isEditing;
        public string Title => IsEditing ? "Editar Cliente" : "Adicionar Cliente";
        public string SaveButtonText => IsEditing ? "Atualizar" : "Salvar";

        public bool IsAgeValid => int.TryParse(AgeText, out int age) && age > 0 && age <= 150;

        // Remover a declaração duplicada dos comandos já que foram movidos para cima

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Lastname) &&
                   !string.IsNullOrWhiteSpace(Address) &&
                   IsAgeValid;
        }

        private async Task SaveClient()
        {
            try
            {
                if (!int.TryParse(AgeText, out int age))
                {
                    await Application.Current!.MainPage!.DisplayAlert("Erro", "Idade deve ser um número válido.", "OK");
                    return;
                }

                if (age <= 0 || age > 150)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Erro", "Idade deve estar entre 1 e 150 anos.", "OK");
                    return;
                }

                var client = new Client
                {
                    Name = Name.Trim(),
                    Lastname = Lastname.Trim(),
                    Age = age,
                    Address = Address.Trim()
                };

                if (IsEditing && _originalClient != null)
                {
                    client.Id = _originalClient.Id;
                    _clientService.UpdateClient(client);
                    await Application.Current!.MainPage!.DisplayAlert("Sucesso", "Cliente atualizado com sucesso!", "OK");
                }
                else
                {
                    _clientService.AddClient(client);
                    await Application.Current!.MainPage!.DisplayAlert("Sucesso", "Cliente adicionado com sucesso!", "OK");
                }

                await CloseWindow();
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro ao salvar cliente: {ex.Message}", "OK");
            }
        }

        private void Cancel()
        {
            _ = CloseWindow();
        }

        private async Task CloseWindow()
        {
            try
            {
                // Fechar a página modal
                if (Application.Current?.MainPage?.Navigation.ModalStack.Count > 0)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                // Se falhar, tentar fechar de outra forma
                System.Diagnostics.Debug.WriteLine($"Erro ao fechar janela: {ex.Message}");
            }
        }
    }
}
