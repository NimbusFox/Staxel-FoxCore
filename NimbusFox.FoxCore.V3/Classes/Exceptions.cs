using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NimbusFox.FoxCore.V3.Classes {
    public class MethodNotExistsException : Exception {
        public MethodNotExistsException() : base() { }

        public MethodNotExistsException(string message) : base(message) { }

        public MethodNotExistsException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidParametersException : Exception {
        public InvalidParametersException() : base() { }

        public InvalidParametersException(string message) : base(message) { }

        public InvalidParametersException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BlobDatabaseRecordException : Exception {
        public BlobDatabaseRecordException() : base() { }

        public BlobDatabaseRecordException(string message) : base(message) { }

        public BlobDatabaseRecordException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BlobDatabaseRecordTypeException : Exception {
        public BlobDatabaseRecordTypeException() : base() { }

        public BlobDatabaseRecordTypeException(string message) : base(message) { }

        public BlobDatabaseRecordTypeException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BlobKindException : Exception {
        public BlobKindException() { }

        public BlobKindException(string message) : base(message) { }

        public BlobKindException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class BlobValueException : Exception {
        public BlobValueException() { }

        public BlobValueException(string message) : base(message) { }

        public BlobValueException(string message, Exception innerException) : base(message, innerException) { }
    }
}
