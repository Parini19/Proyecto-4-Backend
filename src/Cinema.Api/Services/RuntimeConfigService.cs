namespace Cinema.Api.Services
{
    /// <summary>
    /// Service to manage runtime configuration settings that can be changed without restarting the API.
    /// Singleton service that holds configuration state in memory.
    /// </summary>
    public class RuntimeConfigService
    {
        private bool _auditLoggingEnabled;
        private readonly object _lock = new object();

        public RuntimeConfigService(IConfiguration configuration)
        {
            // Initialize from appsettings
            _auditLoggingEnabled = configuration.GetValue<bool>("FeatureManagement:AuditLogging", false);
        }

        /// <summary>
        /// Gets the current state of audit logging
        /// </summary>
        public bool IsAuditLoggingEnabled
        {
            get
            {
                lock (_lock)
                {
                    return _auditLoggingEnabled;
                }
            }
        }

        /// <summary>
        /// Enables or disables audit logging at runtime
        /// </summary>
        public void SetAuditLogging(bool enabled)
        {
            lock (_lock)
            {
                _auditLoggingEnabled = enabled;
            }
        }
    }
}
