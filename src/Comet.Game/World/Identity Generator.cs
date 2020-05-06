using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Comet.Game.World
{
    public sealed class IdentityGenerator
    {
        private readonly ConcurrentQueue<long> m_cqidQueue = new ConcurrentQueue<long>();
        private readonly long m_idMax = uint.MaxValue;
        private readonly long m_idMin;
        private long m_idNext;


        public IdentityGenerator(long min, long max)
        {
            m_idNext = m_idMin = min;
            m_idMax = max;

            for (long i = m_idMin; i <= m_idMax; i++)
            {
                m_cqidQueue.Enqueue(i);
            }

            m_idNext = m_idMax + 1;
        }

        public long GetNextIdentity
        {
            get
            {
                if (!m_cqidQueue.IsEmpty)
                {
                    if (m_cqidQueue.TryDequeue(out long result))
                        return result;
                    return m_idNext < m_idMax ? Interlocked.Increment(ref m_idNext) : 0;
                }

                return m_idNext < m_idMax ? Interlocked.Increment(ref m_idNext) : 0;
            }
        }

        public void ReturnIdentity(long id)
        {
            if (!m_cqidQueue.Contains(id))
                m_cqidQueue.Enqueue(id);
        }
    }
}
