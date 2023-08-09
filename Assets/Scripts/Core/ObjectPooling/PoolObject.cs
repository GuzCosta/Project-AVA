﻿using System;
using UnityEngine;

namespace AVA.Core
{
    public class PoolObject : MonoBehaviour, IPoolable<PoolObject>
    {
        private Action<PoolObject> returnToPool;

        private void OnDisable()
        {
            ReturnToPool();
        }

        public void InitializePoolable(Action<PoolObject> returnAction)
        {
            //cache reference to return action
            this.returnToPool = returnAction;
        }

        public void ReturnToPool()
        {
            //invoke and return this object to pool
            returnToPool?.Invoke(this);
        }

    }
}
