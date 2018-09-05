using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcmeContract.Utilities.Interfaces
{    

    public interface ICsvReader
    {
        List<T> ProcessCsvFile<T>(string csvFile) where T : class;
    }
}
