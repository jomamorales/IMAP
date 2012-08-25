using System;
using System.Collections.Generic;
using System.Text;
using Magas.Data;

namespace Magas.Data.Xml
{
    public class DataProviderFactory : IDataProviderFactory
    {
        public IDataContext GetDataContext()
        {
            return new XmlDataContext();
        }
    }
}
