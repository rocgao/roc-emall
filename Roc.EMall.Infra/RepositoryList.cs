using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Roc.EMall.Infra
{
    public class RepositoryList<T>:ICollection<T>
    {
        private readonly Collection<T> _addedCollection = new();
        private readonly Collection<T> _removedCollection = new();
        private readonly Collection<T> _currentCollection = new();
        
        public RepositoryList(){}

        public RepositoryList(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                _currentCollection.Add(item);
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return _currentCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _currentCollection.Add(item);
            _addedCollection.Add(item);
        }

        public void Clear()
        {
            _currentCollection.Clear();
            _addedCollection.Clear();
        }

        public bool Contains(T item)
        {
            return _currentCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _currentCollection.CopyTo(array,arrayIndex);
        }

        public bool Remove(T item)
        {
            if (_currentCollection.Remove(item))
            {
                _removedCollection.Add(item);
                return true;
            }

            return false;
        }

        public int Count => _currentCollection.Count;
        public bool IsReadOnly => false;
        public ICollection<T> Added => _addedCollection;
        public ICollection<T> Removed => _removedCollection;
    }
}