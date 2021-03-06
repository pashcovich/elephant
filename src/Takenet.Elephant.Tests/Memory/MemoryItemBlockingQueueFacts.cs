﻿using Takenet.Elephant.Memory;
using Xunit;

namespace Takenet.Elephant.Tests.Memory
{
    [Trait("Category", nameof(Memory))]
    public class MemoryItemBlockingQueueFacts : ItemBlockingQueueFacts
    {
        public override IQueue<Item> Create()
        {
            return new Queue<Item>();
        }
    }
}
