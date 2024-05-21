using System;

namespace ETicket.Tests.Utils
{
    public class Builder<T>
    {
        private readonly T _instance;

        public static Builder<T> Build()
        {
            return new Builder<T>();
        }

        public Builder<T> With(Action<T> setter)
        {
            setter.Invoke(_instance);
            return this;
        }

        private Builder()
        {
            try
            {
                 _instance = (T)Activator.CreateInstance(typeof(T));
            }
            catch (TypeInitializationException)
            {
                throw new Exception($"Can't instantiate an object of type {typeof(T).GetType()}");
            }
        }

        public T GetInstance()
        {
            return _instance;
        }
    }
}

