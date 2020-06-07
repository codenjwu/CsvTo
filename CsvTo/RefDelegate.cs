using System.Threading.Tasks;

namespace CsvTo
{
    internal delegate Task RefDelegate<T, S>(T prama1, ref S refer);
}
