using System;
using System.Collections.Generic;

namespace AutoProtobuf.Tests
{
    public class ComplexClass<T>
    {
        private Guid key;

        public ComplexClass(Guid key)
        {
            this.key = key;
        }

        public Guid Key
        {
            get { return key; }
        }

        public List<T> Values { get; set; }
    }
}