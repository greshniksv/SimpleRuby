using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrbEngine
{
    class Module : IDisposable
    {




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
