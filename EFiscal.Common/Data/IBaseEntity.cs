using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.Common.Data
{
    public interface IBaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
        string Name { get; set; }
        string NormalizedName { get; }
    }
}
