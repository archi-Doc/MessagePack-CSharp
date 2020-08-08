// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MessagePack.Resolvers;
using Microsoft;
using MsgPack.Serialization;
using Nerdbank.Streams;
using SharedData;
using Xunit;

namespace MessagePack.Tests
{
    [MessagePackObject]
    public class ATestClass
    {
        [Key(0)]
        public int X { get; set; }

        [Key(1)]
        public byte[] Y { get; set; }

        public ATestClass()
        {
            this.X = 10;
            this.Y = new byte[1024 * 1024 * 10];
        }
    }

    public class MessagePack_ATest
    {
        [Fact]
        public void NonGeneric()
        {
            var c = new ATestClass();

            var b = MessagePackSerializer.Serialize(c);
            var c2 = MessagePackSerializer.Deserialize<ATestClass>(b);
        }
    }
}
