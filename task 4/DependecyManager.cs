using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleApp30
{
    public class DependencyManager
    {
        private static Context context = new Context();
        private IDictionary<Type, BeanDefinition> dependencies = new Dictionary<Type, BeanDefinition>();

        public DependencyManager()
        {

        }

        public void AddTransient<TService, TImplementation>()
        {
            dependencies[typeof(TService)] = new BeanDefinition(false, typeof(TImplementation).GetConstructors());
        }

        public void AddSingleton<TService, TImplementation>()
        {
            dependencies[typeof(TService)] = new BeanDefinition(true, typeof(TImplementation).GetConstructors());
        }

        public object Get<T>()
        {
            return (T)Get(typeof(T), context);
        }

        private object Get(Type type, Context context)
        {
            BeanDefinition beanDefinition = dependencies[type];

            if (beanDefinition == null) throw new Exception(String.Format("No dependencies for type {}", type));

            Type cycledType = null;
            context.startBeanInit(type);
            foreach (var constructor in beanDefinition.GetConstructors())
            {
                object[] args = new object[constructor.GetParameters().Length];
                bool readyForInit = true;
                for (int i = 0; i < args.Length; i++)
                {
                    Type argType = constructor.GetParameters()[i].ParameterType;
                    if (context.beanInitStatus(argType) == BeanInitStatus.INPROGRESS)
                    {
                        readyForInit = false;
                        cycledType = argType;
                        break;
                    }

                    if (context.beanInitStatus(argType) == BeanInitStatus.INITED)
                    {
                        if (dependencies[argType].Singleton) args[i] = context.GetSingleton(argType);
                    }

                    if (context.beanInitStatus(argType) == null ||
                        context.beanInitStatus(argType) == BeanInitStatus.NONE)
                    {
                        args[i] = Get(argType, context);
                        if (args[i] == null)
                        {
                            readyForInit = false;
                            break;
                        }
                    }
                }

                if (readyForInit)
                {
                    object result = constructor.Invoke(args);

                    context.finishBeanInit(type);
                    if (beanDefinition.Singleton)
                    {
                        context.AddSingleton(type, result);
                    }

                    return result;
                }
            }

            if (cycledType != null) throw new Exception(String.Format("Cycle in dependency between {0} and {1}", type, cycledType));
            throw new Exception(String.Format("Can't initiate bean with type {0}", type));
        }
    }



    enum BeanInitStatus
    {
        NONE, INPROGRESS, INITED
    }

    class Context
    {
        private IDictionary<Type, object> singletons = new Dictionary<Type, object>();
        private IDictionary<Type, BeanInitStatus> statuses = new Dictionary<Type, BeanInitStatus>();

        public void AddSingleton(Type type, object singleton)
        {
            singletons[type] = singleton;
        }

        public object GetSingleton(Type type)
        {
            return singletons[type];
        }

        public BeanInitStatus beanInitStatus(Type type)
        {
            if (statuses.ContainsKey(type))
            {
                return statuses[type];
            }
            else
            {
                return BeanInitStatus.NONE;
            }
        }

        public void startBeanInit(Type type)
        {
            statuses[type] = BeanInitStatus.INPROGRESS;
        }

        public void finishBeanInit(Type type)
        {
            statuses[type] = BeanInitStatus.INITED;
        }
    }


    class BeanDefinition
    {
        public bool Singleton { get; set; } = false;

        private ConstructorInfo[] constructors;

        public BeanDefinition(bool singleton, ConstructorInfo[] constructorsInfo)
        {
            this.Singleton = singleton;
            this.constructors = constructorsInfo;
        }

        public ConstructorInfo[] GetConstructors()
        {
            return constructors;
        }
    }
}
