﻿namespace Disruptor.Tests.Support
{
    public class DummyNonBlockingWaitStrategy : INonBlockingWaitStrategy
    {
        public int SignalAllWhenBlockingCalls { get; private set; }

        public long WaitFor(long sequence, Sequence cursor, ISequence dependentSequence, SequenceBarrierAlert alert)
        {
            return 0;
        }

        public void SignalAllWhenBlocking()
        {
            SignalAllWhenBlockingCalls++;
        }
    }
}
