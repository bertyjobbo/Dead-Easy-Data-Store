﻿using System.Reflection;

namespace DedStore.System
{
    public class RegisteredType
    {
        [DedStorePrimaryKey]
        public string Id { get; set; }
        public string PrimaryKeyPropertyName { get; set; }
        public string PrimaryKeyPropertyTypeName { get; set; }
    }
}
