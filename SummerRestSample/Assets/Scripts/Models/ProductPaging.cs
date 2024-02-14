using System;

namespace Models
{
    [Serializable]
    public struct ProductPaging
    {
        public Product[] products;
        public int total;
        public int skip;
        public int limit;
    }
}