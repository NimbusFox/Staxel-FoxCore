using System;

namespace NimbusFox.FoxCore.Classes.Exceptions {
    public class MethodNotExistsException : Exception {
        public MethodNotExistsException() : base() { }

        public MethodNotExistsException(string message) : base(message) { }

        public MethodNotExistsException(string message, Exception innerException) : base (message, innerException) { }
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
}
