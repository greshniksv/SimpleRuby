using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrbEngine
{
    public class RModule : IDisposable
    {
		private List<RClass> _classList = new List<RClass>();

        public string Name { get; set; }

        public List<RClass> ClassList
	    {
		    get { return _classList; }
		    set { _classList = value; }
	    }

	    #region Despose patternt
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Implement dispose pattern
        public void Dispose(bool managed)
        {
            if (managed)
            {
               
            }
        }
        #endregion
    }
}
