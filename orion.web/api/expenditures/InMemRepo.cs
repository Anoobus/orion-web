using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace orion.web.api.expenditures
{
    public interface IInMemRepo<T>
    {
        T FindById(Guid id);
        IEnumerable<T> Search(Func<T,bool> query);
        T AddOrUpdate(T instance, Func<T, Guid> idSelector);
    }

    public class InMemRepo<T> : IInMemRepo<T>
    {
        private static readonly ConcurrentDictionary<Guid, T> saved = new ConcurrentDictionary<Guid, T>();
        public InMemRepo(Func<(Func<T,Guid> keyLookup, T[] data)> preset = null)
        {
            if(saved.Keys.Count == 0 && preset != null)
            {
                var(keyLookup, presetData) = preset();
                foreach(var item in presetData)
                {
                    var temp = item;
                    saved.TryAdd(keyLookup(temp), temp);
                }
            }

        }
        public T AddOrUpdate(T instance, Func<T, Guid> idSelector)
        {
            var key = idSelector(instance);
            return saved.AddOrUpdate(key, instance, (k, old) => instance);
        }

        public T FindById(Guid id)
        {
            return saved.TryGetValue(id, out var found) ? found : default;
        }

        public IEnumerable<T> Search(Func<T, bool> query)
        {
            return saved.Values.Where(x => query(x) == true).ToList();
        }

    }
    public static class Help
    {
        public static Guid ToGuid(this string value)
        {
            using(MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(value));
                return new Guid(hash);
            }

        }
    }

}
