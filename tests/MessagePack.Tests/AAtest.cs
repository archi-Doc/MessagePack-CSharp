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
    [MessagePackObject]
    public class Class4a
    {
        [Key(0)]
        public int x { get; set; } = 1;
        [Key(1)]
        public int y { get; set; } = 2;
    }
    [MessagePackObject]
    public class Class4b
    {
        [Key(0)]
        public int x { get; set; } = 3;
        [Key(2)]
        public int y { get; set; } = 4;
    }

    public class KeepValueTest
    {
        [Fact]
        public void KeepValueTest1()
        {
            {
                var x = new Class4a();
                var ss = MessagePackSerializer.Serialize(x);
                var x2 = MessagePackSerializer.Deserialize<Class4a>(ss);
                var x3 = MessagePackSerializer.Deserialize<Class4b>(ss);

                var y = new Class4b();
                var ss2 = MessagePackSerializer.Serialize(y);
                var y2 = MessagePackSerializer.Deserialize<Class4a>(ss2);
                var y3 = MessagePackSerializer.Deserialize<Class4b>(ss2);
            }

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
                DynamicObjectResolverKeepValue.Instance,
            });
            //var options = MessagePackSerializerOptions.Standard.WithLZ4Compression(true).WithResolver(StandardResolverAllowPrivate.Instance); //.WithResolver(resolver);
            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver2);
            //MessagePackSerializer.DefaultOptions = options; //affects other tests.

            var c = new KeepValueChild(1, "one", 11);
            var s = MessagePackSerializer.Serialize(c);
            var c2 = MessagePackSerializer.Deserialize<KeepValueChild>(s);
            var c3 = MessagePackSerializer.Deserialize<KeepValueChild2>(s);

            var p = new KeepValueParent(12, "second", 122) { First = new KeepValueChild(10, "fir", 1) };
            var s2 = MessagePackSerializer.Serialize(p, options);
            var p2 = MessagePackSerializer.Deserialize<KeepValueParent>(s2, options);

            Console.WriteLine("fin");
        }
    }

    [MessagePackObject]
    public class KeepValueParent
    {
        [Key(0)]
        public KeepValueChild First { get; set; }

        [Key(1)]
        public KeepValueChild Second { get; set; }

        public KeepValueParent(int id, string name, int age)
        {
            Second = new KeepValueChild(id, name, age);
        }

        public KeepValueParent() { }
    }

    [MessagePackObject]
    public class KeepValueChild
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; }

        [Key(3)]
        public int Age { get; set; } = -1; //initial value

        public KeepValueChild(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }

        public KeepValueChild()
        {
        }
    }

    [MessagePackObject]
    public class KeepValueChild2
    {
        [Key(0)]
        public int Id { get; set; } = -1;

        [Key(2)]
        public string Memo { get; set; } = "empty"; //invalid

        [Key(4)]
        public string Height { get; set; }

        public KeepValueChild2()
        {
            Height = "100";
        }
    }
}
