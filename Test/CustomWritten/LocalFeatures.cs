using System;

namespace Local
{
    internal partial class LocalFeatures
    {
        ///<summary>
        /// This field was defined with the generate=false option
        /// So we have to implement it ourself here
        ///</summary>
        public double Amount { get; set; }

        /// <summary>
        /// The serializer requires a constructor with zero arguments.
        /// But it can be private.
        /// </summary>
        private LocalFeatures()
        {

        }

        public LocalFeatures(string secret)
        {
            this.Secret = secret;
        }

        protected virtual void BeforeSerialize()
        {
        }

        protected virtual void AfterDeserialize()
        {
        }

        public void Deny(string trolls)
        {
            this.Denial = trolls;
        }

        public override bool Equals(object obj)
        {
            LocalFeatures l = obj as LocalFeatures;
            if (l == null)
                return false;

            if (l.Uptime != this.Uptime)
                return false;
            if (l.DueDate != this.DueDate)
                return false;
            if (l.Amount != this.Amount)
                return false;
            if (l.Denial != this.Denial)
                return false;
            if (l.Secret != this.Secret)
                return false;
            if (l.Internal != this.Internal)
                return false;
            if (l.PR != this.PR)
                return false;
            if (l.MyEnum != this.MyEnum)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}

