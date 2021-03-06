﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Takenet.Elephant.Specialized
{
    public interface ISemaphore
    {
        Task WaitAsync(CancellationToken cancellationToken);

        void Release();
    }
}
