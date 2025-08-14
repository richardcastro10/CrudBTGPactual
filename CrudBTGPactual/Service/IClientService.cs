using CrudBTGPactual.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudBTGPactual.Service
{
    public interface IClientService
    {
        ObservableCollection<Client> GetClients();
        void AddClient(Client client);
        void UpdateClient(Client client);
        void DeleteClient(int id);
        Client? GetClientById(int id);
    }

    public class ClientService : IClientService
    {
        private readonly ObservableCollection<Client> _clients;
        private int _nextId = 1;

        public ClientService()
        {
            _clients = new ObservableCollection<Client>();

            // Dados de exemplo para demonstração
            AddSampleData();
        }

        public ObservableCollection<Client> GetClients()
        {
            return _clients;
        }

        public void AddClient(Client client)
        {
            client.Id = _nextId++;
            _clients.Add(client);
        }

        public void UpdateClient(Client client)
        {
            var existingClient = _clients.FirstOrDefault(c => c.Id == client.Id);
            if (existingClient != null)
            {
                existingClient.Name = client.Name;
                existingClient.Lastname = client.Lastname;
                existingClient.Age = client.Age;
                existingClient.Address = client.Address;
            }
        }

        public void DeleteClient(int id)
        {
            var client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                _clients.Remove(client);
            }
        }

        public Client? GetClientById(int id)
        {
            return _clients.FirstOrDefault(c => c.Id == id);
        }

        private void AddSampleData()
        {
            AddClient(new Client
            {
                Name = "João",
                Lastname = "Silva",
                Age = 30,
                Address = "Rua das Flores, 123"
            });

            AddClient(new Client
            {
                Name = "Maria",
                Lastname = "Santos",
                Age = 25,
                Address = "Av. Paulista, 456"
            });

            AddClient(new Client
            {
                Name = "Pedro",
                Lastname = "Oliveira",
                Age = 35,
                Address = "Rua Augusta, 789"
            });
        }
    }
}
