using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;

namespace Cinema.Api.Services
{
    public class FirestoreFoodComboService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "foodCombos";

        public FirestoreFoodComboService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var configPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public async Task AddFoodComboAsync(FoodCombo combo)
        {
            if (string.IsNullOrEmpty(combo.Id))
            {
                combo.Id = Guid.NewGuid().ToString();
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(combo.Id);
            await docRef.SetAsync(combo);
        }

        public async Task<FoodCombo?> GetFoodComboAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                return null;
            return snapshot.ConvertTo<FoodCombo>();
        }

        public async Task<List<FoodCombo>> GetAllFoodCombosAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var combos = new List<FoodCombo>();
            foreach (var doc in snapshot.Documents)
                combos.Add(doc.ConvertTo<FoodCombo>());
            return combos;
        }

        public async Task UpdateFoodComboAsync(FoodCombo combo)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(combo.Id);
            await docRef.SetAsync(combo, SetOptions.Overwrite);
        }

        public async Task DeleteFoodComboAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }
    }
}