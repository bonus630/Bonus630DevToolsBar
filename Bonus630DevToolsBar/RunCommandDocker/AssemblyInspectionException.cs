using System;
using System.Collections.Generic;
using System.Linq;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public class AssemblyInspectionException : Exception
    {
        public IReadOnlyList<Exception> LoaderExceptions { get; }

        public AssemblyInspectionException(
            string message,
            Exception inner,
            IEnumerable<Exception> loaderExceptions = null)
            : base(message, inner)
        {
            LoaderExceptions = loaderExceptions.ToList();
            if (loaderExceptions == null)
                loaderExceptions = Array.Empty<Exception>();
        }
    }

}
