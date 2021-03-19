using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services
{
    public interface ITagService
    {
        void Add(string name, int? tagImportance);

        void BulkTagToProperties();
    }
}
