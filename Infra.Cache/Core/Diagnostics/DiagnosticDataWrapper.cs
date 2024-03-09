
using System;

namespace Infra.Cache.Core.Diagnostics
{
    public class DiagnosticDataWrapper
    {
        public Guid OperationId { get; set; }

        public Int64 Timestamp { get; set; }
    }

    public class DiagnosticExceptionWrapper : DiagnosticDataWrapper
    {
        public Exception Exception { get; set; }
    }

    public class DiagnosticDataWrapper<T> : DiagnosticDataWrapper
    {
        public T EventData { get; set; }
    }
}
