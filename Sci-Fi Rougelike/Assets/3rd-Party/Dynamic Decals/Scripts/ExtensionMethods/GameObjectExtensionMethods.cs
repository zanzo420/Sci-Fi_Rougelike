#region

using System.Reflection;
using UnityEngine;

#endregion

namespace LlockhamIndustries.ExtensionMethods
{
    public static class GameObjectExtensionMethods
    {
        public static T AddComponent<T>(this GameObject GameObject, T Source) where T : MonoBehaviour
        {
            return GameObject.AddComponent<T>().GetCopyOf(Source);
        }

        public static MonoBehaviour AddComponent(this GameObject GameObject, MonoBehaviour Source)
        {
            var type = Source.GetType();

            #if !UNITY_EDITOR && UNITY_WSA
            bool isSubclass = type.GetTypeInfo().IsSubclassOf(typeof(MonoBehaviour));
            #else
            var isSubclass = type.IsSubclassOf(typeof(MonoBehaviour));
            #endif

            if (isSubclass)
                return ((MonoBehaviour)GameObject.AddComponent(type)).GetCopyOf(Source);
            return null;
        }

        public static T GetCopyOf<T>(this MonoBehaviour Monobehaviour, T Source) where T : MonoBehaviour
        {
            //Type check
            var type = Monobehaviour.GetType();
            if (type != Source.GetType()) return null;

            //Declare Binding Flags
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            
            //Iterate through all types until monobehaviour is reached
            while (type != typeof(MonoBehaviour))
            {
                //Apply Fields
                #if !UNITY_EDITOR && UNITY_WSA
                System.Reflection.FieldInfo[] fields = type.GetFields(flags).ToArray();
                #else
                var fields = type.GetFields(flags);
                #endif
                foreach (var field in fields)
                    field.SetValue(Monobehaviour, field.GetValue(Source));

                //Move to base class
                #if !UNITY_EDITOR && UNITY_WSA
                type = type.GetTypeInfo().BaseType;
                #else
                type = type.BaseType;
                #endif
                
            }
            return Monobehaviour as T;
        }
    }
}