﻿using System.Collections.Generic;
using System.Linq;
using IdentityServer3.Admin.Persistence;
using IdentityServer3.Admin.Persistence.Models;
using IdentityServer3.Admin.Persistence.Models.Storage;

namespace IdentityServer3.Admin.Storage
{
    /// <summary>
    /// Provides basic storage capabilities for in memory client storage
    /// </summary>
    public class InMemoryClientStorage : IPersistence<Client>
    {
        private readonly IList<Client> _clients = new List<Client>();
        private int _internalClientCount = 1;

        public PageResult<Client> List(PagingInformation pagingInformation)
        {
            return new PageResult<Client>()
            {
                Items = _clients.Skip(pagingInformation.Skip).Take(pagingInformation.Take).ToList(),
                TotalCount = _clients.Count
            };
        }

        public Client Get(int key)
        {
            return _clients.SingleOrDefault(c => c.Id == key);
        }

        public void Delete(int key)
        {
            _clients.Remove(_clients.SingleOrDefault(c => c.Id == key));
        }

        public object Add(Client entity)
        {
            entity.Id = _internalClientCount++;

            _clients.Add(entity);

            return entity.Id;
        }

        public void Update(Client entity)
        {
            var oldId = entity.Id;
            Delete(entity.Id);
            Add(entity);

            _clients.Last().Id = oldId;
        }

        public int TotalCount()
        {
            return _clients.Count;
        }

        public bool IsNameAvailable(Client entity)
        {
            return !_clients.Any(c => c.ClientId == entity.ClientId && c.Id != entity.Id);
        }
    }
}