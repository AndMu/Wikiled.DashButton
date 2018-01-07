using System;
using System.Runtime.Serialization;

namespace Wikiled.DashButton.Monitor
{
    [Serializable]
    public class PcapMissingException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public PcapMissingException()
        {
        }

        public PcapMissingException(string message)
            : base(message)
        {
        }

        public PcapMissingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected PcapMissingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
