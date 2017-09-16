using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Filterinqer.Helpers
{
    public class FixedSizedStack<T> : Stack<T>
    {
        private int _maxSize;

        public FixedSizedStack(int maxSize) : base()
        {
            _maxSize = maxSize;
        }

        public new void Push(T item)
        {
            if (base.Count == _maxSize)
            {
                var list = base.ToArray().Reverse();
                base.Clear();
                list.Skip(1).ToList().ForEach(base.Push);
            }
            base.Push(item);
        }
    }
}
