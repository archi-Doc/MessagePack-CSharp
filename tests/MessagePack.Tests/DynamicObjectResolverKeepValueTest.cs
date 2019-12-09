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

    public enum ByteEnumGG : byte { A, B, C, D, E }
    [MessagePackObject(true)]
    public class SimpleStringKeyDataGG
    {
        public int Prop1 { get; set; }

        public ByteEnumGG Prop2 { get; set; }

        public int Prop3 { get; set; }
    }

    [MessagePackObject]
    public class SimpleIntKeyDataGG
    {
        /*[Key(0)]
        public int Prop1 { get; set; }
        [Key(1)]
        public ByteEnumGG Prop2 { get; set; }
        [Key(2)]
        public string Prop3 { get; set; }*/

        [Key(3)]
        public SimpleStringKeyDataGG Prop4 { get; set; }

        //[Key(4)]
        //public SimpleStructIntKeyData Prop5 { get; set; }

        //[Key(5)]
        //public SimpleStructStringKeyData Prop6 { get; set; }
    }

    public class KeepValueTest
    {
        [Fact]
        public void KeepValueTest1()
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
                //DynamicObjectResolver.Instance,
                DynamicObjectResolverKeepValue.Instance,
            });
            //var options = MessagePackSerializerOptions.Standard.WithLZ4Compression(true).WithResolver(StandardResolverAllowPrivate.Instance); //.WithResolver(resolver);
            var options = MessagePackSerializerOptions.Standard.WithResolver(resolver2);
            //MessagePackSerializer.DefaultOptions = options; //affects other tests.

            SimpleIntKeyDataGG n = null;
            var bytes = MessagePackSerializer.Serialize(n);
            var gg = MessagePackSerializer.Deserialize<SimpleIntKeyDataGG>(bytes);

            var sss = MessagePackSerializer.Serialize<Class4a>(null);
            var yyy = MessagePackSerializer.Deserialize<Class4a>(sss);
            {
                var x = new Class4a();
                var ss = MessagePackSerializer.Serialize(x);
                var x2 = MessagePackSerializer.Deserialize<Class4a>(ss);
                var x3 = MessagePackSerializer.Deserialize<Class4b>(ss);
                var x4 = MessagePackSerializer.Deserialize<Class4b>(ss, options);

                var y = new Class4b();
                var ss2 = MessagePackSerializer.Serialize(y);
                var y2 = MessagePackSerializer.Deserialize<Class4a>(ss2);
                var y3 = MessagePackSerializer.Deserialize<Class4b>(ss2);
                var y4 = MessagePackSerializer.Deserialize<Class4b>(ss2, options);
            }

            var c = new KeepValueChild(1, "one", 11);
            var s = MessagePackSerializer.Serialize(c);
            var c2 = MessagePackSerializer.Deserialize<KeepValueChild>(s);
            var c3 = MessagePackSerializer.Deserialize<KeepValueChild2>(s);
            var c4 = MessagePackSerializer.Deserialize<KeepValueChild2>(s, options);

            var p = new KeepValueParent(12, "second", 122) { First = new KeepValueChild(10, "fir", 1) };
            var s2 = MessagePackSerializer.Serialize(p, options);
            var p2 = MessagePackSerializer.Deserialize<KeepValueParent>(s2, options);
            var p3 = MessagePackSerializer.Deserialize<KeepValueParent3>(s2, options);

            var list = new List<KeepValueChild>();
            list.Add(new KeepValueChild() { Id = 1, Name = "one", Age = 10 });
            list.Add(new KeepValueChild() { Id = 2, Name = "two", Age = 20 });
            //list.Add(new KeepValueChild(1, "one", 10) { Id = 1, Name = "one", Age = 10 });
            //list.Add(new KeepValueChild(2, "two", 20) { Id = 1, Name = "one", Age = 10 });
            var s3 = MessagePackSerializer.Serialize(list, options);
            var p4 = MessagePackSerializer.Deserialize<List<KeepValueChild>>(s3, options);
            var p5 = MessagePackSerializer.Deserialize<List<KeepValueChild_IdName>>(s3, options);
            var p6 = MessagePackSerializer.Deserialize<List<KeepValueChild_IdNameAgeMemo>>(s3, options);
            var p7 = MessagePackSerializer.Deserialize<List<KeepValueChild2>>(s3, options);

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
            //Second = new KeepValueChild(id, name, age);
            Second = new KeepValueChild() { Id = id, Name = name, Age = age };
        }

        public KeepValueParent() { }
    }

    [MessagePackObject]
    public class KeepValueParent3
    {
        [Key(0)]
        public KeepValueChild First { get; set; }

        [Key(1)]
        public KeepValueChild Second { get; set; }

        [Key(2)]
        public KeepValueChild Third { get; set; }

        public KeepValueParent3()
        {
            //Third = new KeepValueChild(3, "three", 33);
            Third = new KeepValueChild() { Id = 3, Name = "three", Age = 36 };
        }
    }

    [MessagePackObject]
    public class KeepValueChild
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string Memo { get; set; }// = "empty"; //invalid
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
    public class KeepValueChild_IdName
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; }

        public KeepValueChild_IdName()
        {
        }
    }
    [MessagePackObject]
    public class KeepValueChild_IdNameAgeMemo
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public string Name { get; set; }

        [Key(3)]
        public int Age { get; set; } = -1; //initial value

        [Key(4)]
        public string Memo { get; set; } = "default memo";

        public KeepValueChild_IdNameAgeMemo()
        {
        }
    }

    [MessagePackObject]
    public class KeepValueChild2
    {
        [Key(0)]
        public int Id { get; set; } = -1;
        [Key(1)]
        public string Name { get; set; }
        //[Key(2)]
        //public string Memo { get; set; } = "empty"; //invalid
        [Key(3)]
        public int Age { get; set; } = -1; //initial value
        [Key(4)]
        public string Height { get; set; } = "100";

        public KeepValueChild2()
        {
        }
    }
}
