﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MessagePack.Attributeless
{
    public class Validation
    {
        readonly Configuration _configuration;

        public Validation(Configuration configuration) => _configuration = configuration;

        public byte[] Checksum =>
            new SHA512Managed().ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", KeyTable)));

        public IEnumerable<string> KeyTable
        {
            get
            {
                yield return "---Subtypes---";
                foreach (var (type, mapping) in _configuration.SubTypeMappedTypes)
                {
                    yield return type.FullName;
                    foreach (var (subtype, key) in mapping.Mappings.OrderBy(kvp => kvp.Key.FullName))
                        yield return $"  - {subtype.FullName} : {key}";
                }

                yield return "---Properties---";
                foreach (var (type, mapping) in _configuration.PropertyMappedTypes)
                {
                    yield return type.FullName;
                    foreach (var (property, key) in mapping.Mappings.OrderBy(kvp => kvp.Key.Name))
                        yield return $"  - {property.Name} : {key}";
                }
            }
        }
    }
}