using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Domain.Entities;
using System.Linq;

namespace Cinema.Api.Services
{
    public class FirestoreAuditLogService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "auditlogs";

        public FirestoreAuditLogService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var configPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public async Task AddAuditLogAsync(AuditLog log)
        {
            if (string.IsNullOrEmpty(log.Id))
            {
                log.Id = Guid.NewGuid().ToString();
            }

            if (log.Timestamp == DateTime.MinValue)
            {
                log.Timestamp = DateTime.UtcNow;
            }

            var docRef = _firestoreDb.Collection(CollectionName).Document(log.Id);
            await docRef.SetAsync(log);
        }

        public async Task<AuditLog?> GetAuditLogAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
                return null;
            return snapshot.ConvertTo<AuditLog>();
        }

        public async Task<List<AuditLog>> GetAllAuditLogsAsync()
        {
            var query = _firestoreDb.Collection(CollectionName).OrderByDescending("Timestamp").Limit(1000);
            var snapshot = await query.GetSnapshotAsync();
            var logs = new List<AuditLog>();
            foreach (var doc in snapshot.Documents)
                logs.Add(doc.ConvertTo<AuditLog>());
            return logs;
        }

        public async Task<List<AuditLog>> GetAuditLogsByUserAsync(string userId)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("UserId", userId)
                .OrderByDescending("Timestamp")
                .Limit(500);
            var snapshot = await query.GetSnapshotAsync();
            var logs = new List<AuditLog>();
            foreach (var doc in snapshot.Documents)
                logs.Add(doc.ConvertTo<AuditLog>());
            return logs;
        }

        public async Task<List<AuditLog>> GetAuditLogsByEntityAsync(string entityType, string entityId)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("EntityType", entityType)
                .WhereEqualTo("EntityId", entityId)
                .OrderByDescending("Timestamp")
                .Limit(500);
            var snapshot = await query.GetSnapshotAsync();
            var logs = new List<AuditLog>();
            foreach (var doc in snapshot.Documents)
                logs.Add(doc.ConvertTo<AuditLog>());
            return logs;
        }

        public async Task<List<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereGreaterThanOrEqualTo("Timestamp", startDate)
                .WhereLessThanOrEqualTo("Timestamp", endDate)
                .OrderByDescending("Timestamp")
                .Limit(1000);
            var snapshot = await query.GetSnapshotAsync();
            var logs = new List<AuditLog>();
            foreach (var doc in snapshot.Documents)
                logs.Add(doc.ConvertTo<AuditLog>());
            return logs;
        }

        public async Task<List<AuditLog>> GetAuditLogsByActionAsync(string action)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("Action", action)
                .OrderByDescending("Timestamp")
                .Limit(500);
            var snapshot = await query.GetSnapshotAsync();
            var logs = new List<AuditLog>();
            foreach (var doc in snapshot.Documents)
                logs.Add(doc.ConvertTo<AuditLog>());
            return logs;
        }

        public async Task DeleteAuditLogAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }

        public async Task<int> GetAuditLogCountAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            return snapshot.Count;
        }
    }
}
