using System;

namespace BatchImageProcessor.View
{
    [Serializable]
    public class ShellContextMenuException : Exception
    {
        /// <summary>Default contructor</summary>
        public ShellContextMenuException()
        {
        }

        /// <summary>Constructor with message</summary>
        /// <param name="message">Message</param>
        public ShellContextMenuException(string message)
            : base(message)
        {
        }
    }
}