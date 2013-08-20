using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPool
{
    /// <summary>
    /// BitPool that connects to a BitPool, supporting obfuscation-nesting.
    /// </summary>
    internal class MatryoshkaClient
    {
        public string ServerURL { get; private set; }
    }
}
