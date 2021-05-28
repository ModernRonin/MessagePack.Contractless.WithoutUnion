﻿using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace MessagePack.Contractless.Subtypes
{
    public class MessagePackSerializerOptionsBuilder
    {
        static readonly IMessagePackFormatter[] _nativeFormatters =
        {
            NativeDateTimeArrayFormatter.Instance,
            NativeDateTimeFormatter.Instance,
            NativeDecimalFormatter.Instance,
            NativeGuidFormatter.Instance
        };

        readonly List<IMessagePackFormatter> _formatters = new List<IMessagePackFormatter>();
        readonly MessagePackSerializerOptions _options;

        readonly Dictionary<Type, IPropertyToKeyMapping> _propertyMappedTypes =
            new Dictionary<Type, IPropertyToKeyMapping>();

        readonly Dictionary<Type, ISubTypeToKeyMapping> _subTypeMappedTypes =
            new Dictionary<Type, ISubTypeToKeyMapping>();

        bool _doesUseNativeResolvers;

        public MessagePackSerializerOptionsBuilder(MessagePackSerializerOptions options) =>
            _options = options;

        public MessagePackSerializerOptionsBuilder AddNativeFormatters()
        {
            _doesUseNativeResolvers = true;
            return this;
        }

        public MessagePackSerializerOptionsBuilder AutoKeyed<T>() where T : new()
        {
            var formatter = new ConfigurableKeyFormatter<T>();
            formatter.UseAutomaticKeys();
            _propertyMappedTypes.Add(typeof(T), formatter);
            _formatters.Add(formatter);

            return this;
        }

        public MessagePackSerializerOptions Build()
        {
            var formatters = _formatters.ToList();
            if (_doesUseNativeResolvers) formatters.AddRange(_nativeFormatters);
            var composite = CompositeResolver.Create(_formatters.Concat(_nativeFormatters).ToArray(),
                new[] {_options.Resolver});
            return _options.WithResolver(composite);
        }
    }
}