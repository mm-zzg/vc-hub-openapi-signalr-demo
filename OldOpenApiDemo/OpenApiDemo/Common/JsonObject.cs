using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenApiDemo.Common
{
    public class JsonObject : IDictionary<string, object>
    {

        private System.Collections.Concurrent.ConcurrentDictionary<string, object> Properties { get; } = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();

        public object this[string key]
        {
            get
            {
                if (Properties.TryGetValue(key, out object value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                Properties.AddOrUpdate(key, k => value, (a, b) => value);
            }
        }

        public ICollection<string> Keys => Properties.Keys;

        public ICollection<object> Values => Properties.Values;

        public int Count => Properties.Count;
        public bool IsReadOnly => false;

        public void Add(string key, object value)
        {
            if (!Properties.TryAdd(key, value))
            {
                throw new ArgumentException($"The item with key {key} already exists");
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Properties.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return Properties.ContainsKey(item.Key);
        }

        public bool ContainsKey(string key)
        {
            return Properties.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            var items = Properties.Skip(arrayIndex);

            var i = 0;
            foreach (var item in items)
            {
                array[i++] = item;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Properties.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return Properties.Remove(key, out object _);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return Properties.Remove(item.Key, out object _);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        {
            return Properties.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Properties.GetEnumerator();
        }

        public override string ToString()
        {

            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new ObjectConverter());

            return JsonSerializer.Serialize(this, options);
            //return base.ToString();
        }

        public JsonObject DeepClone()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new ObjectConverter());
            var json = JsonSerializer.Serialize(this, options);
            return JsonSerializer.Deserialize<JsonObject>(json, options);
        }
    }
}
