namespace ntlm.MonitoringPlan
{
    using System;

    public class ServiceDownException : Exception
    {
        public Service Service { get; private set; }
        public ServiceDownException(Service service, Exception innerException) : base(innerException.Message, innerException)
        {
            Service = service;
        }
    }
}
