using System;

namespace SeleniumUtilities.Web
{
    /// <summary>
    /// This class abstracts URL construction and allows clients to swap out different host environments
    /// without having to change test code.
    /// </summary>
    public class Host
    {
        // Default to HTTP but probably need to handle HTTPS soon
        private const string protocol = "http://";

        public string Name { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">the host name with no leading protocols or trailing slashes or paths</param>
        public Host(string host)
        {
            Name = host;
        }

        /// <summary>
        /// Creates a full url from the host domain name plus the path. Only does HTTP right now.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ConstructUrl(string path)
        {
            return protocol + Name + path;
        }
    }
}
