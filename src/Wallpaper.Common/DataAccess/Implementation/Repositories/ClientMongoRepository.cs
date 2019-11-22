using System;
using Framework.Contracts.Services.DataAccess;
using MongoDB.Driver;
using Plugin.Application.Wallpaper.Common.DataAccess.Contracts.Repositories;
using Plugin.Application.Wallpaper.Common.Model.Clients;
using Plugin.DataAccess.MongoDb;

namespace Plugin.Application.Wallpaper.Common.DataAccess.Implementation.Repositories
{
    public class ClientMongoRepository : EntityMongoRepository<Client>, IClientRepository
    {
        public ClientMongoRepository(IMongoDataAccessProvider dataAccessProvider)
            : base(dataAccessProvider)
        { }

        public Client GetExternalClient(string externalId, string currentUsername)
        {
            var updateResult = Collection.FindOneAndUpdate<Client>(Filter().Eq(x => x.ExternalId, externalId),
                                Update().Combine(
                                Update().Set(x => x.Name, currentUsername),
                                Update().SetOnInsert(x => x.Created, DateTime.Now),
                                Update().SetOnInsert(x => x.Id, Guid.NewGuid())),
                                new FindOneAndUpdateOptions<Client, Client>
                                {
                                    ReturnDocument = ReturnDocument.After,
                                    IsUpsert = true
                                });

            return updateResult;
        }
    }
}
