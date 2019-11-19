// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack.Resolvers;
using Xunit;

namespace MessagePack.Tests
{
    public class AllowPrivateFix
    {
        [Fact]
        public void AllowPrivateFixTest()
        {
            var resolver = CompositeResolver.Create(new IFormatterResolver[] {
                BuiltinResolver.Instance, // Try Builtin
                AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]
                DynamicEnumResolver.Instance, // Try Enum
                DynamicGenericResolver.Instance, // Try Array, Tuple, Collection, Enum(Generic Fallback)
                DynamicUnionResolver.Instance, // Try Union(Interface)
                DynamicObjectResolver.Instance,
            });
            var resolver2 = CompositeResolver.Create(new IFormatterResolver[] {
                BuiltinResolver.Instance, // Try Builtin
                AttributeFormatterResolver.Instance, // Try use [MessagePackFormatter]
                DynamicEnumResolver.Instance, // Try Enum
                DynamicGenericResolver.Instance, // Try Array, Tuple, Collection, Enum(Generic Fallback)
                DynamicUnionResolver.Instance, // Try Union(Interface)
                DynamicObjectResolverAllowPrivate.Instance,
            });
            //var options = MessagePackSerializerOptions.Standard.WithLZ4Compression(true).WithResolver(StandardResolverAllowPrivate.Instance); //.WithResolver(resolver);
            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver2);
            //MessagePackSerializer.DefaultOptions = options; //affects other tests

            var c = new AllowPrivateChild(1, "one");
            var s = MessagePackSerializer.Serialize(c, options);
            var c2 = MessagePackSerializer.Deserialize<AllowPrivateChild>(s, options);
            var c3 = MessagePackSerializer.Deserialize<AllowPrivateChild2>(s, options);

            var p = new AllowPrivateParent(12, "second") { First = new AllowPrivateChild(10, "fir") };
            var s2 = MessagePackSerializer.Serialize(p, options);
            var p2 = MessagePackSerializer.Deserialize<AllowPrivateParent>(s2, options);

            Console.WriteLine("fin");
        }
    }

    [MessagePackObject]
    public class AllowPrivateParent
    {
        [Key(0)]
        public AllowPrivateChild First { get; set; }

        [Key(1)]
        private AllowPrivateChild second;

        public AllowPrivateParent(int id, string name)
        {
            second = new AllowPrivateChild(id, name);
        }
        public AllowPrivateParent() { }
    }

        [MessagePackObject]
    public class AllowPrivateChild
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; }

        public AllowPrivateChild(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /*public ContractlessBugChild()
        {
        }*/
    }

    [MessagePackObject]
    public class AllowPrivateChild2
    {
        [Key(0)]
        private int Id { get; set; }

        //[Key(1)]
        //public string Name { get; set; }

        [Key(2)]
        private int Year { get; set; } = 999;

        [Key(3)]
        public string Memo { get; set; } = "empty2";//invalid 

        public AllowPrivateChild2()
        {
        }
    }
}
