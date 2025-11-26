using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;

namespace Cinema.Api.Services
{
    public class FirestoreTheaterRoomService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "theaterRooms";

        public FirestoreTheaterRoomService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var configPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public async Task AddTheaterRoomAsync(TheaterRoom room)
        {
            if (string.IsNullOrEmpty(room.Id))
            {
                room.Id = Guid.NewGuid().ToString();
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(room.Id);
            await docRef.SetAsync(room);
        }

        public async Task<TheaterRoom?> GetTheaterRoomAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                return null;
            return snapshot.ConvertTo<TheaterRoom>();
        }

        public async Task<List<TheaterRoom>> GetAllTheaterRoomsAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var rooms = new List<TheaterRoom>();
            foreach (var doc in snapshot.Documents)
                rooms.Add(doc.ConvertTo<TheaterRoom>());
            return rooms;
        }

        public async Task UpdateTheaterRoomAsync(TheaterRoom room)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(room.Id);
            await docRef.SetAsync(room, SetOptions.Overwrite);
        }

        public async Task DeleteTheaterRoomAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }
    }
}