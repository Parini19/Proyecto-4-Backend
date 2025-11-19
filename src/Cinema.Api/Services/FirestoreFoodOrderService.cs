using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;

namespace Cinema.Api.Services
{
    public class FirestoreFoodOrderService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "foodOrders";

        public FirestoreFoodOrderService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var configPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public async Task AddFoodOrderAsync(FoodOrder order)
        {
            if (string.IsNullOrEmpty(order.Id))
            {
                order.Id = Guid.NewGuid().ToString();
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(order.Id);
            await docRef.SetAsync(order);
        }

        public async Task<FoodOrder?> GetFoodOrderAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                return null;
            return snapshot.ConvertTo<FoodOrder>();
        }

        public async Task<List<FoodOrder>> GetAllFoodOrdersAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var orders = new List<FoodOrder>();
            foreach (var doc in snapshot.Documents)
                orders.Add(doc.ConvertTo<FoodOrder>());
            return orders;
        }

        public async Task UpdateFoodOrderAsync(FoodOrder order)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(order.Id);
            await docRef.SetAsync(order, SetOptions.Overwrite);
        }

        public async Task DeleteFoodOrderAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }
    }
}