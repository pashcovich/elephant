﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Ploeh.AutoFixture;
using Takenet.Elephant.Memory;
using Takenet.Elephant.Specialized;
using Xunit;

namespace Takenet.Elephant.Tests.Specialized
{
    public abstract class FallbackQueueFacts<T> : QueueFacts<T>
    {
        public override IQueue<T> Create()
        {
            return Create(new Queue<T>(), new Queue<T>());
        }

        public IQueue<T> Create(IQueue<T> master, IQueue<T> slave)
        {
            return new FallbackQueue<T>(master, slave);
        }

        [Fact(DisplayName = "EnqueueWhenMasterIsDownShouldSucceed")]
        public virtual async Task EnqueueWhenMasterIsDownShouldSucceed()
        {
            // Arrange
            var master = new Mock<IQueue<T>>();
            var slave = new Queue<T>();
            var queue = Create(master.Object, slave);
            var item = Fixture.Create<T>();
            master
                .Setup(q => q.EnqueueAsync(It.IsAny<T>()))
                .Throws(new Exception());

            // Act
            await queue.EnqueueAsync(item);

            // Assert
            AssertEquals(await slave.GetLengthAsync(), 1);
            AssertEquals(await slave.DequeueOrDefaultAsync(), item);
            AssertEquals(await slave.GetLengthAsync(), 0);
            master.Verify(q => q.EnqueueAsync(item), Times.Once);
        }

        [Fact(DisplayName = "EnqueueMultipleTimesWhenMasterIsDownShouldSynchronizeAfterRecovery")]
        public virtual async Task EnqueueMultipleTimesWhenMasterIsDownShouldSynchronizeAfterRecovery()
        {
            // Arrange
            var master = new Mock<IQueue<T>>();
            var slave = new Queue<T>();
            var queue = Create(master.Object, slave);
            var item1 = Fixture.Create<T>();
            var item2 = Fixture.Create<T>();
            var item3 = Fixture.Create<T>();
            var item4 = Fixture.Create<T>();
            var item5 = Fixture.Create<T>();
            var item6 = Fixture.Create<T>();
            master
                .SetupSequence(q => q.EnqueueAsync(It.IsAny<T>()))
                    .Returns(TaskUtil.CompletedTask)
                    .Throws(new Exception())
                    .Throws(new Exception())
                    .Returns(TaskUtil.CompletedTask)
                    .Returns(TaskUtil.CompletedTask) // Synchronization call
                    .Returns(TaskUtil.CompletedTask) // Synchronization call
                    .Throws(new Exception())
                    .Returns(TaskUtil.CompletedTask)
                    .Returns(TaskUtil.CompletedTask); // Synchronization call

            // Act
            await queue.EnqueueAsync(item1);
            await queue.EnqueueAsync(item2);
            await queue.EnqueueAsync(item3);
            await queue.EnqueueAsync(item4);
            await queue.EnqueueAsync(item5);
            await queue.EnqueueAsync(item6);

            // Assert
            AssertEquals(await slave.GetLengthAsync(), 0);
            master.Verify(q => q.EnqueueAsync(item1), Times.Once);
            master.Verify(q => q.EnqueueAsync(item2), Times.Exactly(2));
            master.Verify(q => q.EnqueueAsync(item3), Times.Exactly(2));
            master.Verify(q => q.EnqueueAsync(item4), Times.Once);
            master.Verify(q => q.EnqueueAsync(item5), Times.Exactly(2));
            master.Verify(q => q.EnqueueAsync(item6), Times.Once);
        }

        [Fact(DisplayName = "EnqueueMultipleTimesWhenMasterIsUpShouldNotSynchronize")]
        public virtual async Task EnqueueMultipleTimesWhenMasterIsUpShouldNotSynchronize()
        {
            // Arrange
            var master = new Mock<IQueue<T>>();
            var slave = new Queue<T>();
            var queue = Create(master.Object, slave);
            var item1 = Fixture.Create<T>();
            var item2 = Fixture.Create<T>();
            var item3 = Fixture.Create<T>();
            master
                .SetupSequence(q => q.EnqueueAsync(It.IsAny<T>()))
                    .Returns(TaskUtil.CompletedTask)
                    .Returns(TaskUtil.CompletedTask)
                    .Returns(TaskUtil.CompletedTask);

            // Act
            await queue.EnqueueAsync(item1);
            await queue.EnqueueAsync(item2);
            await queue.EnqueueAsync(item3);

            // Assert
            AssertEquals(await slave.GetLengthAsync(), 0);
            master.Verify(q => q.EnqueueAsync(item1), Times.Once);
            master.Verify(q => q.EnqueueAsync(item2), Times.Once);
            master.Verify(q => q.EnqueueAsync(item3), Times.Once);
        }

        [Fact(DisplayName = "DequeueWhenMasterIsDownShouldSucceed")]
        public virtual async Task DequeueWhenMasterIsDownShouldSucceed()
        {
            // Arrange
            var master = new Mock<IQueue<T>>();
            var slave = new Queue<T>();
            var queue = Create(master.Object, slave);
            var item = Fixture.Create<T>();
            master
                .Setup(q => q.EnqueueAsync(It.IsAny<T>()))
                .Throws(new Exception());
            master
                .Setup(q => q.DequeueOrDefaultAsync())
                .Throws(new Exception());

            // Act
            await queue.EnqueueAsync(item);
            var actual = await queue.DequeueOrDefaultAsync();


            // Assert
            AssertEquals(actual, item);
            AssertEquals(await slave.GetLengthAsync(), 0);
        }
    }
}
