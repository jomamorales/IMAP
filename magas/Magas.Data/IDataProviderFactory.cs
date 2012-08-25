using System;
using System.Collections.Generic;
using System.Text;

namespace Magas.Data
{
    public interface IDataProviderFactory
    {
        IDataContext GetDataContext();
    }
}
