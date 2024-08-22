using System;
using System.Collections.Generic;
using System.Reflection;

namespace HQFramework
{
    public sealed class HQFrameworkEngine
    {
        private delegate void ModuleLifecycleMethod(HQModuleBase module);

        private static Dictionary<Type, HQModuleBase> registeredModuleDic;
        private static LinkedList<HQModuleBase> moduleList;
        private static ModuleLifecycleMethod moduleInitialize;
        private static ModuleLifecycleMethod moduleUpdate;
        private static ModuleLifecycleMethod moduleShutdown;
        private static Assembly frameworkAssembly;

        private static void Initialize()
        {
            TimeManager.Initialize();
            registeredModuleDic = new Dictionary<Type, HQModuleBase>();
            moduleList = new LinkedList<HQModuleBase>();

            Type baseModuleType = typeof(HQModuleBase);
            MethodInfo initializeMethod = baseModuleType.GetMethod("OnInitialize", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo updateMethod = baseModuleType.GetMethod("OnUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo destroyMethod = baseModuleType.GetMethod("OnShutdown", BindingFlags.Instance | BindingFlags.NonPublic);

            Type methodType = typeof(ModuleLifecycleMethod);
            moduleInitialize = Delegate.CreateDelegate(methodType, initializeMethod) as ModuleLifecycleMethod;
            moduleUpdate = Delegate.CreateDelegate(methodType, updateMethod) as ModuleLifecycleMethod;
            moduleShutdown = Delegate.CreateDelegate(methodType, destroyMethod) as ModuleLifecycleMethod;

            frameworkAssembly = Assembly.GetExecutingAssembly();
        }

        private static HQModuleBase RegisterModule(Type interfaceType)
        {
            string typeName = interfaceType.Name.Substring(1); // IXXXManager => XXXManager
            Type type = frameworkAssembly.GetType($"{interfaceType.Namespace}.{typeName}");
            if (!typeof(HQModuleBase).IsAssignableFrom(type))
            {
                throw new ArgumentException($"{type} is not assignable from BaseModule.");
            }
            HQModuleBase module = Activator.CreateInstance(type) as HQModuleBase;
            LinkedListNode<HQModuleBase> current = moduleList.Last;
            while (current != null)
            {
                if (current.Value.Priority <= module.Priority)
                {
                    break;
                }
                current = current.Next;
            }
            if (current == null)
            {
                moduleList.AddFirst(module);
            }
            else
            {
                moduleList.AddAfter(current, module);
            }
            registeredModuleDic.Add(interfaceType, module);
            moduleInitialize.Invoke(module);
            return module;
        }

        internal static HQModuleBase GetModule(Type interfaceType)
        {
            if (!interfaceType.IsInterface) // interface segregation principle
            {
                throw new ArgumentException($"{interfaceType} is not an interface type, you can only get module interface from HQFramework.");
            }
            if (registeredModuleDic.ContainsKey(interfaceType))
            {
                return registeredModuleDic[interfaceType];
            }
            else
            {
                return RegisterModule(interfaceType);
            }
        }

        /// <summary>
        /// Get HQFramework Module
        /// </summary>
        /// <typeparam name="T">Module interface type</typeparam>
        /// <returns>Module interface object</returns> 
        /// <summary>
        public static T GetModule<T>() where T : class
        {
            return GetModule(typeof(T)) as T;
        }

        public static void UnregisterModule<T>() where T : class
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException($"{interfaceType} is not an interface type, you can only get module interface from HQFramework.");
            }
            if (registeredModuleDic.ContainsKey(interfaceType))
            {
                HQModuleBase module = registeredModuleDic[interfaceType];
                moduleShutdown.Invoke(module);
                moduleList.Remove(module);
                registeredModuleDic.Remove(interfaceType);
            }
            else
            {
                throw new KeyNotFoundException($"You have not registered the module : {interfaceType}");
            }
        }

        private static void Update(float deltaTimeLogic, float deltaTimeRealtime)
        {
            TimeManager.OnUpdate(deltaTimeLogic, deltaTimeRealtime);

            for (LinkedListNode<HQModuleBase> node = moduleList.First; node != null; node = node.Next)
            {
                moduleUpdate.Invoke(node.Value);
            }
        }

        private static void Shutdown()
        {
            for (LinkedListNode<HQModuleBase> node = moduleList.Last; node != null; node = node.Previous)
            {
                moduleShutdown.Invoke(node.Value);
            }
            registeredModuleDic.Clear();
            moduleList.Clear();

            ReferencePool.ClearAll();
        }
    }
}
