using System;
using System.Collections.Generic;
using System.Text;

namespace Magas.Data.Xml
{
    internal interface IDataMapper<T>
    {
        IList<T> GetAll();
        IList<T> GetByField(string fieldname, object value);
    }
}
