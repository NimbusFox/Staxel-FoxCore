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
}
