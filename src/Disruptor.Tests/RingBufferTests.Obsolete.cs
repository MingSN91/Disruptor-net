﻿using System;
using NUnit.Framework;
using static Disruptor.Tests.RingBufferEqualsConstraint;

namespace Disruptor.Tests
{
    partial class RingBufferTests
    {
        [Test]
        public void ShouldPublishEventNoArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> translator = new NoArgEventTranslator();

            ringBuffer.PublishEvent(translator);
            ringBuffer.TryPublishEvent(translator);

            Assert.That(ringBuffer, IsRingBufferWithEvents(0L, 1L));
        }

        [Test]
        public void ShouldPublishEventOneArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            ringBuffer.PublishEvent(translator, "Foo");
            ringBuffer.TryPublishEvent(translator, "Foo");

            Assert.That(ringBuffer, IsRingBufferWithEvents("Foo-0", "Foo-1"));
        }

        [Test]
        public void ShouldPublishEventTwoArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            ringBuffer.PublishEvent(translator, "Foo", "Bar");
            ringBuffer.TryPublishEvent(translator, "Foo", "Bar");

            Assert.That(ringBuffer, IsRingBufferWithEvents("FooBar-0", "FooBar-1"));
        }

        [Test]
        public void ShouldPublishEventThreeArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            ringBuffer.PublishEvent(translator, "Foo", "Bar", "Baz");
            ringBuffer.TryPublishEvent(translator, "Foo", "Bar", "Baz");

            Assert.That(ringBuffer, IsRingBufferWithEvents("FooBarBaz-0", "FooBarBaz-1"));
        }

        [Test]
        public void ShouldPublishEventVarArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorVararg<object[]> translator = new VarArgEventTranslator();

            ringBuffer.PublishEvent(translator, "Foo", "Bar", "Baz", "Bam");
            ringBuffer.TryPublishEvent(translator, "Foo", "Bar", "Baz", "Bam");

            Assert.That(ringBuffer, IsRingBufferWithEvents("FooBarBazBam-0", "FooBarBazBam-1"));
        }

        [Test]
        public void ShouldPublishEventsNoArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> eventTranslator = new NoArgEventTranslator();
            var translators = new[] { eventTranslator, eventTranslator };

            ringBuffer.PublishEvents(translators);
            Assert.IsTrue(ringBuffer.TryPublishEvents(translators));

            Assert.That(ringBuffer, IsRingBufferWithEvents(0L, 1L, 2L, 3L));
        }

        [Test]
        public void ShouldNotPublishEventsNoArgIfBatchIsLargerThanRingBuffer()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> eventTranslator = new NoArgEventTranslator();
            var translators =
                new[] { eventTranslator, eventTranslator, eventTranslator, eventTranslator, eventTranslator };

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(translators));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldPublishEventsWithBatchSizeOfOne()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> eventTranslator = new NoArgEventTranslator();
            var translators =
                new[] { eventTranslator, eventTranslator, eventTranslator };

            ringBuffer.PublishEvents(translators, 0, 1);
            Assert.IsTrue(ringBuffer.TryPublishEvents(translators, 0, 1));

            Assert.That(ringBuffer, IsRingBufferWithEvents(0L, 1L, null, null));
        }

        [Test]
        public void ShouldPublishEventsWithinBatch()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> eventTranslator = new NoArgEventTranslator();
            var translators =
                new[] { eventTranslator, eventTranslator, eventTranslator };

            ringBuffer.PublishEvents(translators, 1, 2);
            Assert.IsTrue(ringBuffer.TryPublishEvents(translators, 1, 2));

            Assert.That(ringBuffer, IsRingBufferWithEvents(0L, 1L, 2L, 3L));
        }

        [Test]
        public void ShouldPublishEventsOneArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            ringBuffer.PublishEvents(translator, new[] { "Foo", "Foo" });
            Assert.IsTrue(ringBuffer.TryPublishEvents(translator, new[] { "Foo", "Foo" }));

            Assert.That(ringBuffer, IsRingBufferWithEvents("Foo-0", "Foo-1", "Foo-2", "Foo-3"));
        }

        [Test]
        public void ShouldNotPublishEventsOneArgIfBatchIsLargerThanRingBuffer()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(translator, new[] { "Foo", "Foo", "Foo", "Foo", "Foo" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldPublishEventsOneArgBatchSizeOfOne()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            ringBuffer.PublishEvents(translator, 0, 1, new[] { "Foo", "Foo" });
            Assert.IsTrue(ringBuffer.TryPublishEvents(translator, 0, 1, new[] { "Foo", "Foo" }));

            Assert.That(ringBuffer, IsRingBufferWithEvents("Foo-0", "Foo-1", null, null));
        }

        [Test]
        public void ShouldPublishEventsOneArgWithinBatch()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            ringBuffer.PublishEvents(translator, 1, 2, new[] { "Foo", "Foo", "Foo" });
            Assert.IsTrue(ringBuffer.TryPublishEvents(translator, 1, 2, new[] { "Foo", "Foo", "Foo" }));

            Assert.That(ringBuffer, IsRingBufferWithEvents("Foo-0", "Foo-1", "Foo-2", "Foo-3"));
        }

        [Test]
        public void ShouldPublishEventsTwoArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            ringBuffer.PublishEvents(translator, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" });
            ringBuffer.TryPublishEvents(translator, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" });

            Assert.That(ringBuffer, IsRingBufferWithEvents("FooBar-0", "FooBar-1", "FooBar-2", "FooBar-3"));
        }

        [Test]
        public void ShouldNotPublishEventsITwoArgIfBatchSizeIsBiggerThanRingBuffer()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator,
                                                     new[] { "Foo", "Foo", "Foo", "Foo", "Foo" },
                                                     new[] { "Bar", "Bar", "Bar", "Bar", "Bar" }));
                ;
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldPublishEventsTwoArgWithBatchSizeOfOne()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            ringBuffer.PublishEvents(translator, 0, 1, new[] { "Foo0", "Foo1" }, new[] { "Bar0", "Bar1" });
            ringBuffer.TryPublishEvents(translator, 0, 1, new[] { "Foo2", "Foo3" }, new[] { "Bar2", "Bar3" });

            Assert.That(ringBuffer, IsRingBufferWithEvents("Foo0Bar0-0", "Foo2Bar2-1", null, null));
        }

        [Test]
        public void ShouldPublishEventsTwoArgWithinBatch()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            ringBuffer.PublishEvents(
                translator, 1, 2, new[] { "Foo0", "Foo1", "Foo2" }, new[] { "Bar0", "Bar1", "Bar2" });
            ringBuffer.TryPublishEvents(
                translator, 1, 2, new[] { "Foo3", "Foo4", "Foo5" }, new[] { "Bar3", "Bar4", "Bar5" });

            Assert.That(ringBuffer, IsRingBufferWithEvents("Foo1Bar1-0", "Foo2Bar2-1", "Foo4Bar4-2", "Foo5Bar5-3"));
        }

        [Test]
        public void ShouldPublishEventsThreeArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            ringBuffer.PublishEvents(
                translator, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }, new[] { "Baz", "Baz" });
            ringBuffer.TryPublishEvents(
                translator, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }, new[] { "Baz", "Baz" });

            Assert.That(ringBuffer, IsRingBufferWithEvents("FooBarBaz-0", "FooBarBaz-1", "FooBarBaz-2", "FooBarBaz-3"));
        }

        [Test]
        public void ShouldNotPublishEventsThreeArgIfBatchIsLargerThanRingBuffer()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator,
                                                     new[] { "Foo", "Foo", "Foo", "Foo", "Foo" },
                                                     new[] { "Bar", "Bar", "Bar", "Bar", "Bar" },
                                                     new[] { "Baz", "Baz", "Baz", "Baz", "Baz" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldPublishEventsThreeArgBatchSizeOfOne()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            ringBuffer.PublishEvents(
                translator, 0, 1, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }, new[] { "Baz", "Baz" });
            ringBuffer.TryPublishEvents(
                translator, 0, 1, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }, new[] { "Baz", "Baz" });

            Assert.That(ringBuffer, IsRingBufferWithEvents("FooBarBaz-0", "FooBarBaz-1", null, null));
        }

        [Test]
        public void ShouldPublishEventsThreeArgWithinBatch()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            ringBuffer.PublishEvents(
                translator, 1, 2, new[] { "Foo0", "Foo1", "Foo2" }, new[] { "Bar0", "Bar1", "Bar2" },
                new[] { "Baz0", "Baz1", "Baz2" }
                );
            Assert.IsTrue(
                ringBuffer.TryPublishEvents(
                    translator, 1, 2, new[] { "Foo3", "Foo4", "Foo5" }, new[] { "Bar3", "Bar4", "Bar5" },
                    new[] { "Baz3", "Baz4", "Baz5" }));

            Assert.That(ringBuffer, IsRingBufferWithEvents("Foo1Bar1Baz1-0", "Foo2Bar2Baz2-1", "Foo4Bar4Baz4-2", "Foo5Bar5Baz5-3"));
        }

        [Test]
        public void ShouldPublishEventsVarArg()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorVararg<object[]> translator = new VarArgEventTranslator();

            ringBuffer.PublishEvents(translator, new[] { "Foo", "Bar", "Baz", "Bam" }, new[] { "Foo", "Bar", "Baz", "Bam" });
            Assert.IsTrue(ringBuffer.TryPublishEvents(translator, new[] { "Foo", "Bar", "Baz", "Bam" }, new[] { "Foo", "Bar", "Baz", "Bam" }));

            Assert.That(ringBuffer, IsRingBufferWithEvents("FooBarBazBam-0", "FooBarBazBam-1", "FooBarBazBam-2", "FooBarBazBam-3"));
        }

        [Test]
        public void ShouldNotPublishEventsVarArgIfBatchIsLargerThanRingBuffer()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorVararg<object[]> translator = new VarArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator,
                                                     new[] { "Foo", "Bar", "Baz", "Bam" },
                                                     new[] { "Foo", "Bar", "Baz", "Bam" },
                                                     new[] { "Foo", "Bar", "Baz", "Bam" },
                                                     new[] { "Foo", "Bar", "Baz", "Bam" },
                                                     new[] { "Foo", "Bar", "Baz", "Bam" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldPublishEventsVarArgBatchSizeOfOne()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorVararg<object[]> translator = new VarArgEventTranslator();

            ringBuffer.PublishEvents(
                translator, 0, 1, new object[] { "Foo", "Bar", "Baz", "Bam" }, new object[] { "Foo", "Bar", "Baz", "Bam" });
            Assert.IsTrue(
                ringBuffer.TryPublishEvents(
                    translator, 0, 1, new object[] { "Foo", "Bar", "Baz", "Bam" }, new object[] { "Foo", "Bar", "Baz", "Bam" }));

            Assert.That(
                ringBuffer, IsRingBufferWithEvents(
                    "FooBarBazBam-0", "FooBarBazBam-1", null, null));
        }

        [Test]
        public void ShouldPublishEventsVarArgWithinBatch()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorVararg<object[]> translator = new VarArgEventTranslator();

            ringBuffer.PublishEvents(
                translator, 1, 2, new object[] { "Foo0", "Bar0", "Baz0", "Bam0" },
                new object[] { "Foo1", "Bar1", "Baz1", "Bam1" },
                new object[] { "Foo2", "Bar2", "Baz2", "Bam2" });
            Assert.IsTrue(
                ringBuffer.TryPublishEvents(
                    translator, 1, 2, new object[] { "Foo3", "Bar3", "Baz3", "Bam3" },
                    new object[] { "Foo4", "Bar4", "Baz4", "Bam4" },
                    new object[] { "Foo5", "Bar5", "Baz5", "Bam5" }));

            Assert.That(
                ringBuffer, IsRingBufferWithEvents(
                    "Foo1Bar1Baz1Bam1-0", "Foo2Bar2Baz2Bam2-1", "Foo4Bar4Baz4Bam4-2", "Foo5Bar5Baz5Bam5-3"));
        }

        [Test]
        public void ShouldNotPublishEventsNoArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> translator = new NoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(new[] { translator, translator, translator, translator }, 1, 0));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsNoArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> translator = new NoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(new[] { translator, translator, translator, translator }, 1, 0));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> translator = new NoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(new[] { translator, translator, translator }, 1, 3));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> translator = new NoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(new[] { translator, translator, translator }, 1, 3));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsNoArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> translator = new NoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(new[] { translator, translator, translator, translator }, 1, -1));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsNoArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> translator = new NoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(new[] { translator, translator, translator, translator }, 1, -1));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> translator = new NoArgEventTranslator();
            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(new[] { translator, translator, translator, translator }, -1, 2));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslator<object[]> translator = new NoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(new[] { translator, translator, translator, translator }, -1, 2));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsOneArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(translator, 1, 0, new[] { "Foo", "Foo" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsOneArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(translator, 1, 0, new[] { "Foo", "Foo" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsOneArgWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(translator, 1, 3, new[] { "Foo", "Foo" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsOneArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(translator, 1, -1, new[] { "Foo", "Foo" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsOneArgWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();
            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(translator, -1, 2, new[] { "Foo", "Foo" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsOneArgWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(translator, 1, 3, new[] { "Foo", "Foo" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsOneArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(translator, 1, -1, new[] { "Foo", "Foo" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsOneArgWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorOneArg<object[], string> translator = new OneArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(translator, -1, 2, new[] { "Foo", "Foo" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsTwoArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(
                                                     translator, 1, 0, new[] { "Foo", "Foo" },
                                                     new[] { "Bar", "Bar" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsTwoArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator, 1, 0, new[] { "Foo", "Foo" },
                                                     new[] { "Bar", "Bar" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsTwoArgWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(translator, 1, 3, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsTwoArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(translator, 1, -1, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsTwoArgWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();
            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(translator, -1, 2, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsTwoArgWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(translator, 1, 3, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsTwoArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(translator, 1, -1, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsTwoArgWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorTwoArg<object[], string, string> translator = new TwoArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(translator, -1, 2, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsThreeArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(
                                                     translator, 1, 0, new[] { "Foo", "Foo" },
                                                     new[] { "Bar", "Bar" }, new[] { "Baz", "Baz" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsThreeArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator, 1, 0, new[] { "Foo", "Foo" },
                                                     new[] { "Bar", "Bar" }, new[] { "Baz", "Baz" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsThreeArgWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(
                                                     translator, 1, 3, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" },
                                                     new[] { "Baz", "Baz" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsThreeArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(
                                                     translator, 1, -1, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" },
                                                     new[] { "Baz", "Baz" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsThreeArgWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(
                                                     translator, -1, 2, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" },
                                                     new[] { "Baz", "Baz" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsThreeArgWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator, 1, 3, new[] { "Foo", "Foo" }, new[] { "Bar", "Bar" },
                                                     new[] { "Baz", "Baz" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsThreeArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator, 1, -1, new[] { "Foo", "Foo" },
                                                     new[] { "Bar", "Bar" }, new[] { "Baz", "Baz" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsThreeArgWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            IEventTranslatorThreeArg<object[], string, string, string> translator = new ThreeArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator, -1, 2, new[] { "Foo", "Foo" },
                                                     new[] { "Bar", "Bar" }, new[] { "Baz", "Baz" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsVarArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            var translator = new VarArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(
                                                     translator, 1, 0, new object[] { "Foo0", "Bar0", "Baz0", "Bam0" },
                                                     new object[] { "Foo1", "Bar1", "Baz1", "Bam1" },
                                                     new object[] { "Foo2", "Bar2", "Baz2", "Bam2" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsVarArgWhenBatchSizeIs0()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            var translator = new VarArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator, 1, 0, new object[] { "Foo0", "Bar0", "Baz0", "Bam0" },
                                                     new object[] { "Foo1", "Bar1", "Baz1", "Bam1" },
                                                     new object[] { "Foo2", "Bar2", "Baz2", "Bam2" }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsVarArgWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            var translator = new VarArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(
                                                     translator, 1, 3, new object[] { "Foo0", "Bar0", "Baz0", "Bam0" },
                                                     new object[] { "Foo1", "Bar1", "Baz1", "Bam1" }, new object[]
                                                     {
                                                         "Foo2", "Bar2",
                                                         "Baz2", "Bam2"
                                                     }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsVarArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            var translator = new VarArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(
                                                     translator, 1, -1, new object[] { "Foo0", "Bar0", "Baz0", "Bam0" },
                                                     new object[] { "Foo1", "Bar1", "Baz1", "Bam1" }, new object[]
                                                     {
                                                         "Foo2", "Bar2",
                                                         "Baz2", "Bam2"
                                                     }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotPublishEventsVarArgWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            var translator = new VarArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.PublishEvents(
                                                     translator, -1, 2, new object[] { "Foo0", "Bar0", "Baz0", "Bam0" },
                                                     new object[] { "Foo1", "Bar1", "Baz1", "Bam1" }, new object[]
                                                     {
                                                         "Foo2", "Bar2",
                                                         "Baz2", "Bam2"
                                                     }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsVarArgWhenBatchExtendsPastEndOfArray()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            var translator = new VarArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator, 1, 3, new object[] { "Foo0", "Bar0", "Baz0", "Bam0" },
                                                     new object[] { "Foo1", "Bar1", "Baz1", "Bam1" }, new object[]
                                                     {
                                                         "Foo2", "Bar2",
                                                         "Baz2", "Bam2"
                                                     }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsVarArgWhenBatchSizeIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            var translator = new VarArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator, 1, -1, new object[] { "Foo0", "Bar0", "Baz0", "Bam0" },
                                                     new object[] { "Foo1", "Bar1", "Baz1", "Bam1" }, new object[]
                                                     {
                                                         "Foo2", "Bar2",
                                                         "Baz2", "Bam2"
                                                     }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        [Test]
        public void ShouldNotTryPublishEventsVarArgWhenBatchStartsAtIsNegative()
        {
            var ringBuffer = RingBuffer<object[]>.CreateSingleProducer(() => new object[1], 4);
            var translator = new VarArgEventTranslator();

            try
            {
                Assert.Throws<ArgumentException>(() => ringBuffer.TryPublishEvents(
                                                     translator, -1, 2, new object[] { "Foo0", "Bar0", "Baz0", "Bam0" },
                                                     new object[] { "Foo1", "Bar1", "Baz1", "Bam1" }, new object[]
                                                     {
                                                         "Foo2", "Bar2",
                                                         "Baz2", "Bam2"
                                                     }));
            }
            finally
            {
                AssertEmptyRingBuffer(ringBuffer);
            }
        }

        private class NoArgEventTranslator : IEventTranslator<object[]>
        {
            public void TranslateTo(object[] eventData, long sequence)
            {
                eventData[0] = sequence;
            }
        }

        private class VarArgEventTranslator : IEventTranslatorVararg<object[]>
        {
            public void TranslateTo(object[] eventData, long sequence, params object[] args)
            {
                eventData[0] = (string)args[0] + args[1] + args[2] + args[3] + "-" + sequence;
            }
        }

        private class ThreeArgEventTranslator : IEventTranslatorThreeArg<object[], string, string, string>
        {
            public void TranslateTo(object[] eventData, long sequence, string arg0, string arg1, string arg2)
            {
                eventData[0] = arg0 + arg1 + arg2 + "-" + sequence;
            }
        }

        private class TwoArgEventTranslator : IEventTranslatorTwoArg<object[], string, string>
        {
            public void TranslateTo(object[] eventData, long sequence, string arg0, string arg1)
            {
                eventData[0] = arg0 + arg1 + "-" + sequence;
            }
        }

        private class OneArgEventTranslator : IEventTranslatorOneArg<object[], string>
        {
            public void TranslateTo(object[] eventData, long sequence, string arg0)
            {
                eventData[0] = arg0 + "-" + sequence;
            }
        }
    }
}
