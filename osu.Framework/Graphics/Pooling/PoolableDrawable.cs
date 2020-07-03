// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Layout;

namespace osu.Framework.Graphics.Pooling
{
    public class PoolableDrawable : CompositeDrawable
    {
        public override bool DisposeOnDeathRemoval => pool == null;

        /// <summary>
        /// Whether this pooled drawable is currently in use.
        /// </summary>
        public bool IsInUse { get; private set; }

        /// <summary>
        /// Whether this drawable is currently managed by a pool.
        /// </summary>
        public bool IsInPool => pool != null;

        private IDrawablePool pool;

        /// <summary>
        /// A flag to keep the drawable present to guarantee <see cref="prepare"/> can be performed as a scheduled call.
        /// </summary>
        private bool waitingForPrepare;

        public override bool IsPresent => waitingForPrepare || base.IsPresent;

        /// <summary>
        /// Return this drawable to its pool manually. Note that this is not required if the drawable is using lifetime cleanup.
        /// </summary>
        public void Return()
        {
            if (!IsInUse)
                throw new InvalidOperationException($"This {nameof(PoolableDrawable)} was already returned");

            IsInUse = false;

            FreeAfterUse();

            // intentionally don't throw if a pool was not associated or otherwise.
            // supports use of PooledDrawables outside of a pooled scenario without special handling.
            pool?.Return(this);
            waitingForPrepare = false;
        }

        /// <summary>
        /// Perform any initialisation on new usage of this drawable.
        /// </summary>
        protected virtual void PrepareForUse()
        {
        }

        /// <summary>
        /// Perform any clean-up required before returning this drawable to a pool.
        /// </summary>
        protected virtual void FreeAfterUse()
        {
        }

        /// <summary>
        /// Set the associated pool this drawable is currently associated with.
        /// </summary>
        /// <param name="pool">The target pool, or null to disassociate from all pools (and cause the drawable to be disposed as if it was not pooled). </param>
        /// <exception cref="InvalidOperationException">Thrown if this drawable is still in use, or is already in another pool.</exception>
        internal void SetPool(IDrawablePool pool)
        {
            if (IsInUse)
                throw new InvalidOperationException($"This {nameof(PoolableDrawable)} is still in use");

            if (pool != null && this.pool != null)
                throw new InvalidOperationException($"This {nameof(PoolableDrawable)} is already in a pool");

            this.pool = pool;
        }

        /// <summary>
        /// Assign this drawable to a consumer.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this drawable is still in use.</exception>
        internal void Assign()
        {
            if (IsInUse)
                throw new InvalidOperationException($"This {nameof(PoolableDrawable)} is already in use");

            IsInUse = true;

            LifetimeStart = double.MinValue;
            LifetimeEnd = double.MaxValue;

            waitingForPrepare = true;

            // prepare call is scheduled as it may contain user code dependent on the clock being updated.
            Schedule(prepare);
        }

        private void prepare()
        {
            waitingForPrepare = false;
            PrepareForUse();
        }

        protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
        {
            if (invalidation.HasFlag(Invalidation.Parent))
            {
                if (IsInUse && Parent == null)
                    Return();
            }

            return base.OnInvalidate(invalidation, source);
        }
    }
}
