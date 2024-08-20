using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HQFramework
{
    public sealed class HQFrameworkEngine
    {
        private delegate void ModuleLifecycleMethod(HQModuleBase module);

        private static HashSet<Type> registeredModuleTypeSet;
        private static LinkedList<HQModuleBase> moduleList;
        private static ModuleLifecycleMethod moduleInitialize;
        private static ModuleLifecycleMethod moduleUpdate;
        private static ModuleLifecycleMethod moduleShutdown;

        public static void Initialize()
        {
            TimeManager.Initialize();
            registeredModuleTypeSet = new HashSet<Type>();
            moduleList = new LinkedList<HQModuleBase>();

            Type baseModuleType = typeof(HQModuleBase);
            MethodInfo initializeMethod = baseModuleType.GetMethod("OnInitialize", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo updateMethod = baseModuleType.GetMethod("OnUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo destroyMethod = baseModuleType.GetMethod("OnShutdown", BindingFlags.Instance | BindingFlags.NonPublic);

            Type methodType = typeof(ModuleLifecycleMethod);
            moduleInitialize = Delegate.CreateDelegate(methodType, initializeMethod) as ModuleLifecycleMethod;
            moduleUpdate = Delegate.CreateDelegate(methodType, updateMethod) as ModuleLifecycleMethod;
            moduleShutdown = Delegate.CreateDelegate(methodType, destroyMethod) as ModuleLifecycleMethod;

            ProcessAutoRegisterModules();
        }

        private static void ProcessAutoRegisterModules()
        {
            Assembly frameworkAssembly = Assembly.GetExecutingAssembly();
            Type baseManagerType = typeof(HQModuleBase);
            Type[] types = frameworkAssembly.GetTypes();
            List<(AutoRegisterAttribute, Type)> managerOrderList = new List<(AutoRegisterAttribute, Type)>();
            for (int j = 0; j < types.Length; j++)
            {
                Type type = types[j];
                if (!type.IsAbstract && baseManagerType.IsAssignableFrom(type))
                {
                    AutoRegisterAttribute orderAttribute = type.GetCustomAttribute<AutoRegisterAttribute>();
                    if (orderAttribute != null)
                    {
                        managerOrderList.Add((orderAttribute, type));
                    }
                }
            }

            managerOrderList.Sort((orderType1, orderType2) =>
            {
                if (orderType1.Item1.groupOrder < orderType2.Item1.groupOrder)
                    return -1;
                else if (orderType1.Item1.groupOrder == orderType2.Item1.groupOrder)
                    return orderType1.Item1.internalOrder <= orderType2.Item1.internalOrder ? -1 : 1;
                else
                    return 1;
            });

            for (int n = 0; n < managerOrderList.Count; n++)
            {
                RegisterModule(managerOrderList[n].Item2);
            }
        }

        public static HQModuleBase RegisterModule(Type type)
        {
            if (!typeof(HQModuleBase).IsAssignableFrom(type))
            {
                throw new ArgumentException($"{type} is not assignable from BaseModule.");
            }
            else if (registeredModuleTypeSet.Contains(type))
            {
                throw new ArgumentException($"{type} has been registered, you can't do it again.");
            }
            HQModuleBase module = Activator.CreateInstance(type) as HQModuleBase;
            moduleList.AddLast(module);
            registeredModuleTypeSet.Add(type);
            moduleInitialize.Invoke(module);
            return module;
        }

        public static T RegisterModule<T>() where T : HQModuleBase
        {
            return (T)RegisterModule(typeof(T));
        }

        public static HQModuleBase GetModule(Type moduleType)
        {
            if (registeredModuleTypeSet.Contains(moduleType))
            {
                for (LinkedListNode<HQModuleBase> node = moduleList.First; node != null; node = node.Next)
                {
                    if (node.Value.GetType() == moduleType)
                        return node.Value;
                }
                return null;
            }
            else
            {
                throw new KeyNotFoundException($"You have not registered the module : {moduleType}");
            }
        }

        public static T GetModule<T>() where T : HQModuleBase
        {
            return GetModule(typeof(T)) as T;
        }

        public static void UnregisterModule(Type type)
        {
            if (registeredModuleTypeSet.Contains(type))
            {
                HQModuleBase module = null;
                for (LinkedListNode<HQModuleBase> node = moduleList.First; node != null; node = node.Next)
                {
                    if (node.Value.GetType() == type)
                    {
                        module = node.Value;
                        moduleShutdown.Invoke(module);
                        registeredModuleTypeSet.Remove(type);
                        moduleList.Remove(node);
                        break;
                    }
                }
            }
            else
            {
                throw new KeyNotFoundException($"You have not registered the module : {type}");
            }
        }

        public static void UnregisterModule<T>() where T : HQModuleBase
        {
            UnregisterModule(typeof(T));
        }

        public static void Update(float deltaTimeLogic, float deltaTimeRealtime)
        {
            TimeManager.OnUpdate(deltaTimeLogic, deltaTimeRealtime);

            for (LinkedListNode<HQModuleBase> node = moduleList.First; node != null; node = node.Next)
            {
                moduleUpdate.Invoke(node.Value);
            }
        }

        public static void Shutdown()
        {
            for (LinkedListNode<HQModuleBase> node = moduleList.Last; node != null; node = node.Previous)
            {
                moduleShutdown.Invoke(node.Value);
            }
            registeredModuleTypeSet.Clear();
            moduleList.Clear();

            ReferencePool.ClearAll();
        }
    }
}
