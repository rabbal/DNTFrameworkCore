using System;

namespace DNTFrameworkCore.Dependency
{
    public interface IFactory<out T>
    {
        T Create();
    }

    public class Factory<T> : IFactory<T>
    {
        private readonly Func<T> _initFunc;

        public Factory(Func<T> initFunc)
        {
            _initFunc = initFunc;
        }

        public T Create()
        {
            return _initFunc();
        }
    }
}