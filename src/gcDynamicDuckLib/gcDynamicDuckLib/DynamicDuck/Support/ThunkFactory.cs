using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GeniusCode.Components.DynamicDuck.Support.Extensions;
using GeniusCode.Components.Extensions;

namespace GeniusCode.Components.DynamicDuck.Support
{
    public class ThunkFactory<TProvider> : ThunkFactory
        where TProvider : IDynamicInteractionProvider, new()
    {
        public ThunkFactory()
            : base(new TProvider())
        {
        }
    }

    public enum ImplementMode
    {
        AllExplicit = 1,
        AllImplicit,
        PropertiesImplicit
    }

    /// <summary>
    /// Extension methods for duck typing and dynamic casts.
    /// </summary>
    public class ThunkFactory
    {
        readonly NestedDictionary<Type, string, object> _defaultValues = new NestedDictionary<Type, string, object>();
        readonly Dictionary<Type, Type> _thunks = new Dictionary<Type, Type>();


        private readonly IDynamicInteractionProvider _provider;

        public int GenerateCount { get; private set; }

        protected virtual bool CalculateShouldImplementExplicitly(MemberInfo member)
        {
            var result = true;
            if (!member.TryAs<PropertyInfo>(p => result = _mode == ImplementMode.AllExplicit))
                member.TryAs<MethodInfo>(m => result = _mode == ImplementMode.PropertiesImplicit || _mode == ImplementMode.AllExplicit);

            return result;
        }

        readonly ImplementMode _mode;
        #region Constuctors

        public ThunkFactory(IDynamicInteractionProvider provider)
            : this(provider, ImplementMode.PropertiesImplicit)
        {
        }
        public ThunkFactory(IDynamicInteractionProvider provider, ImplementMode mode)
        {
            _provider = provider;
            _mode = mode;

        }
        #endregion

        #region assets

        static readonly MethodInfo GetPropertyMethod = typeof(DynamicProxyBase).GetMethod("GetProperty", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly MethodInfo SetPropertyMethod = typeof(DynamicProxyBase).GetMethod("SetProperty", BindingFlags.NonPublic | BindingFlags.Instance);
        private const MethodAttributes ImplicitImplementAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.SpecialName;
        private const MethodAttributes ExplicitImplementAttributes = MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
        static readonly MethodInfo GetInvokeVoidMethod = typeof(DynamicProxyBase).GetMethod("InvokeVoidMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly MethodInfo OnSetDefaultValues = typeof(DynamicProxyBase).GetMethod("OnSetDefaultValues", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly MethodInfo SetDefaultValueUsingFunc = typeof(DynamicProxyBase).GetMethod("SetDefaultValueUsingFunc", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly MethodInfo SetDefaultValueWithoutFunc = typeof(DynamicProxyBase).GetMethod("SetDefaultValueWithoutFunc", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly MethodInfo GetInvokeReturnMethod = typeof(DynamicProxyBase).GetMethod("InvokeReturnMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly MethodInfo GetArgInfoMethod = typeof(DynamicProxyBase).GetMethod("GetArgInfo", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly MethodInfo AddHandlerMethod = typeof(DynamicProxyBase).GetMethod("AddHandler", BindingFlags.NonPublic | BindingFlags.Instance);
        static readonly MethodInfo RemoveHandlerMethod = typeof(DynamicProxyBase).GetMethod("RemoveHandler", BindingFlags.NonPublic | BindingFlags.Instance);
        #endregion
        #region Public methods

        public T AsIfRewrap<T>(IDynamicProxy proxy, bool setDefaultValues) where T : class
        {
            if (!ReferenceEquals(proxy.Provider, _provider))
                throw new NotSupportedException("In order to rewrap, providers must be same object reference.");

            return AsIf<T>(proxy.Target, setDefaultValues);
        }

        private Type NameModuleAndBuildThunkType<T>() where T : class
        {
            var targetType = typeof(T);
            const string thunkTypeName = "<>__Thunks.Thunk";// +targetType.FullName;

            var thunkType = BuildThunkType(targetType, thunkTypeName);
            GenerateCount++;
            return thunkType;
        }

        public Type GetThunkType<T>() where T : class
        {
            return _thunks.CreateOrGetValue(typeof(T), NameModuleAndBuildThunkType<T>);
        }

        /// <summary>
        /// Creates a wrapper object of type <typeparamref name="T">T</typeparamref> around the specified <paramref name="target">target</paramref> object.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="target">Object to be wrapped.</param>
        /// <param name="setDefaultValues"></param>
        /// <returns>Wrapper around the specified object.</returns>
        /// <exception cref="InvalidOperationException">Occurs when the specified target type is not an interface.</exception>
        /// <remarks>
        /// This method allows a form of duck typing where interfaces are used to specify a late-bound contract morphed over an existing object.
        /// </remarks>
        public T AsIf<T>(object target, bool setDefaultValues) where T : class
        {
            var thunkType = GetThunkType<T>();

            //Create thunk instance            
            var output = (T)Activator.CreateInstance(thunkType, target, _provider);

            // set default values
            if (setDefaultValues)
                output.TryAs<DynamicProxyBase>(pb => pb.SetDefaultValues(s => _defaultValues[typeof(T)][s]));

            _provider.OnObjectWrapped(output);

            return output;
        }

        #endregion
        #region Private methods

        private void PushDefaultValuesIntoDictionaryForType(Type targetType)
        {
            var newDict = new Dictionary<string, object>();

            var q = from t in GetPropertiesForDefaultValue(targetType)
                    let at = t.GetCustomAttributes<DefaultValueAttribute>(true).SingleOrDefault()
                    where at != null
                    select new { t, at.Value };

            q.ToList().ForEach(a => newDict.Add(_provider.GetQualifiedName(a.t), a.Value));

            _defaultValues.Add(targetType, newDict);
        }

        private IEnumerable<PropertyInfo> GetPropertiesForDefaultValue(Type targetType)
        {
            // get properties on this interface
            var accessibleProperties = targetType.GetProperties().Where(p => p.CanWrite && p.CanRead).ToList();
            // get properties on base interfaces
            var baseProperties = targetType.GetInterfaces().SelectMany(o => o.GetProperties().Where(p => p.CanRead && p.CanRead)).ToList();
            // contacinate lists


            var newAccessibleProperties = from pi in accessibleProperties.Concat(baseProperties)
                                          select pi;

            return newAccessibleProperties;
        }

        /// <summary>
        /// Builds a thunk type definition with the specified <paramref name="thunkTypeName">name</paramref> for the specified <paramref name="targetType">target type</paramref>.
        /// </summary>
        /// <param name="targetType">Target type to create a thunk type definition for.</param>
        /// <param name="thunkTypeName">Name to be used for the created thunk type definition.</param>
        /// <returns>Thunk type definition for the specified <paramref name="targetType">target type</paramref>.</returns>
        private Type BuildThunkType(Type targetType, string thunkTypeName)
        {
            ModuleBuilder builder = CreateModuleBuilder();
            TypeBuilder typeBuilder = GetTypeBuilder(thunkTypeName, builder);

            //
            // Set the parent type to Dynamic.
            //
            typeBuilder.SetParent(typeof(DynamicProxyBase));

            //
            // Implement constructor for thunked object.
            //
            ImplementConstructor(typeBuilder);

            //
            // Implement all interfaces.
            //
            foreach (Type interfaceType in GetInterfaces(targetType))
            {
                ImplementInterface(interfaceType, typeBuilder);
            }

            //populate default values in cache:
            PushDefaultValuesIntoDictionaryForType(targetType);

            // override default values method
            BuildDefaultValuesMethod(targetType, typeBuilder);


            return typeBuilder.CreateType();
        }


        private void BuildDefaultValuesMethod(Type targetType, TypeBuilder typeBuilder)
        {
            var methodBuilder = typeBuilder.DefineMethod("OnSetDefaultValues", MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, CallingConventions.HasThis, typeof(void), new[] { typeof(Func<string, object>) });
            typeBuilder.DefineMethodOverride(methodBuilder, OnSetDefaultValues);

            EmitDefaultValuesMethod(methodBuilder.GetILGenerator(), targetType);
        }

        private void EmitDefaultValuesMethod(ILGenerator g, Type targetType)
        {
            var properties = GetPropertiesForDefaultValue(targetType);

            var myDict = _defaultValues[targetType];

            properties.ToList().ForEach(p =>
            {
                g.Emit(OpCodes.Nop);
                g.Emit(OpCodes.Ldarg_0);
                g.Emit(OpCodes.Ldstr, _provider.GetQualifiedName(p));
                if (myDict.ContainsKey(_provider.GetQualifiedName(p)))
                {
                    g.Emit(OpCodes.Ldarg_1);
                    g.Emit(OpCodes.Call, SetDefaultValueUsingFunc.MakeGenericMethod(p.PropertyType));
                }
                else
                    g.Emit(OpCodes.Call, SetDefaultValueWithoutFunc.MakeGenericMethod(p.PropertyType));
            });

            g.Emit(OpCodes.Nop);
            g.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Implements the constructor for a thunk type definition.
        /// </summary>
        /// <param name="typeBuilder">Type builder to emit to.</param>
        private void ImplementConstructor(TypeBuilder typeBuilder)
        {
            //
            // public <class>(object @object) : base(@object)
            // {
            // }
            //
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new[] { typeof(object), typeof(IDynamicInteractionProvider) });
            var ctorILGen = ctorBuilder.GetILGenerator();
            ctorILGen.Emit(OpCodes.Ldarg_0);
            ctorILGen.Emit(OpCodes.Ldarg_1);
            ctorILGen.Emit(OpCodes.Ldarg_2);
            ctorILGen.Emit(OpCodes.Call, typeof(DynamicProxyBase).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(object), typeof(IDynamicInteractionProvider) }, null));
            ctorILGen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Implements the specified <paramref name="interfaceType">interface type</paramref>.
        /// </summary>
        /// <param name="interfaceType">Interface type to implement.</param>
        /// <param name="typeBuilder">Type builder to emit to.</param>
        private void ImplementInterface(Type interfaceType, TypeBuilder typeBuilder)
        {
            // Add implements clause.
            typeBuilder.AddInterfaceImplementation(interfaceType);


            //properties.
            interfaceType.GetProperties().ToList().ForEach(p => BuildProperty(typeBuilder, p));
            //methods.
            interfaceType.GetMethods().WhereExplicitMethodDefinitions().ToList().ForEach(m => BuildMethod(typeBuilder, m));
            //events.
            interfaceType.GetEvents().ToList().ForEach(e => BuildEvent(typeBuilder, e));
        }

        #region Method

        private static string GetInterfaceImplementerName(MemberInfo memberInfo)
        {
            return memberInfo.DeclaringType.FullName + "." + memberInfo.Name;
        }

        private void BuildMethod(TypeBuilder typeBuilder, MethodInfo methodInfo)
        {
            // collect parameter info
            var parameters = methodInfo.GetParameters();
            var genericArguements = methodInfo.GetGenericArguments();
            var parameterTypes = parameters.Select(pi => pi.ParameterType).ToArray();

            string methodName;
            MethodAttributes methodAttributes;

            if (CalculateShouldImplementExplicitly(methodInfo))
            {
                methodName = GetInterfaceImplementerName(methodInfo);
                methodAttributes = ExplicitImplementAttributes;
            }
            else
            {
                methodName = methodInfo.Name;
                methodAttributes = ImplicitImplementAttributes;
            }


            var methodBuilder = typeBuilder.DefineMethod(methodName, methodAttributes, CallingConventions.HasThis, methodInfo.ReflectedType, parameterTypes);

            // if this is a generic method, set generic variables
            if (methodInfo.IsGenericMethod)
            {
                // aggregate generic params
                var genericParams = methodBuilder.DefineGenericParameters(genericArguements.Select(t => t.Name).ToArray());
                var stuff = (from t in genericArguements
                             join t2 in genericParams on t.Name equals t2.Name
                             select new { t, t2 }).ToList();

                // copy constraints
                stuff.ForEach(a =>
                {
                    // constraints based on attributes
                    a.t2.SetGenericParameterAttributes(a.t.GenericParameterAttributes);

                    // interface constraints
                    var interfaceConstraints = a.t.GetInterfaces();
                    if (interfaceConstraints.Any())
                        a.t2.SetInterfaceConstraints(interfaceConstraints);

                    // base type constraints
                    if (a.t.BaseType != typeof(Object))
                        a.t2.SetBaseTypeConstraint(a.t.BaseType);

                });
            }
            // set return type
            methodBuilder.SetReturnType(methodInfo.ReturnType);

            // set names on parameters
            int x = 1;
            parameters.ToList().ForEach(p =>
            {
                methodBuilder.DefineParameter(x, ParameterAttributes.None, p.Name);
                x++;
            });

            // set method as interface implementation
            typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);

            // emit method body
            EmitMethodBody(methodBuilder.GetILGenerator(), methodInfo);

        }

        private void EmitMethodParameter(OpCode ldInfoArgArray, ILGenerator g, ParameterInfo[] parameters, int methodInfoPosition)
        {
            int msilmethodArgPosition = methodInfoPosition + 1;
            g.Emit(ldInfoArgArray);
            g.Emit(OpCodes.Ldc_I4_S, methodInfoPosition); // put something in position 1 of arg array
            g.Emit(OpCodes.Ldarg_0);
            g.Emit(OpCodes.Ldstr, parameters[methodInfoPosition].Name); //"input2" string
            g.Emit(OpCodes.Ldarg_S, msilmethodArgPosition); // input2 variable 
            g.Emit(OpCodes.Call, GetArgInfoMethod.MakeGenericMethod(parameters[methodInfoPosition].ParameterType));

            g.Emit(OpCodes.Stelem_Ref);
        }

        private void EmitMethodBody(ILGenerator g, MethodInfo methodInfo)
        {

            // Set Variables: 
            var returnsValue = !methodInfo.IsVoidReturnType();
            var endOfMthd = g.DefineLabel();
            // store opCodes, as we switch depending on if method is void or returns value
            OpCode ldInfoArgArray;
            OpCode stInfoArgArray;




            #region set local value variables for ArgInfoArray

            if (!returnsValue)
            {
                ldInfoArgArray = OpCodes.Ldloc_0;
                stInfoArgArray = OpCodes.Stloc_0;
            }
            else
            {
                ldInfoArgArray = OpCodes.Ldloc_1;
                stInfoArgArray = OpCodes.Stloc_1;
                // set return variable, if appropriate
                g.DeclareLocal(methodInfo.ReturnType);
            }

            #endregion

            var parameters = methodInfo.GetParameters();


            // declare ArgInfo array:
            g.DeclareLocal(typeof(IArgInfo[]));

            // start emitting method body:
            g.Emit(OpCodes.Nop);
            g.Emit(OpCodes.Ldarg_0);
            g.Emit(OpCodes.Ldstr, _provider.GetQualifiedName(methodInfo));


            if (parameters.Any())
            {
                //   g.Emit(OpCodes.Ldarg_0); this used to be here....

                // create array
                g.Emit(OpCodes.Ldc_I4_S, parameters.Count()); // there are 2 parameters
                g.Emit(OpCodes.Newarr, typeof(IArgInfo));
                g.Emit(stInfoArgArray);

                // process parameters
                for (int i = 0; i < parameters.Count(); i++)
                {
                    EmitMethodParameter(ldInfoArgArray, g, parameters, i);
                }

                // convert parameters into array
                g.Emit(ldInfoArgArray);
            }
            else
                g.Emit(OpCodes.Ldnull);


            if (returnsValue)
            {
                g.Emit(OpCodes.Call, GetInvokeReturnMethod.MakeGenericMethod(methodInfo.ReturnType));
                g.Emit(OpCodes.Stloc_0);
                g.Emit(OpCodes.Br_S, endOfMthd);
                g.MarkLabel(endOfMthd);
                g.Emit(OpCodes.Ldloc_0);
            }
            else
            {
                g.Emit(OpCodes.Call, GetInvokeVoidMethod);
                g.Emit(OpCodes.Nop);
            }

            g.Emit(OpCodes.Ret);


        }
        #endregion

        private void BuildEvent(TypeBuilder typeBuilder, EventInfo eventInfo)
        {
            MethodAttributes methodAttributes;

            string eventName;

            if (CalculateShouldImplementExplicitly(eventInfo))
            {
                eventName = GetInterfaceImplementerName(eventInfo);
                methodAttributes = ExplicitImplementAttributes;
            }
            else
            {
                eventName = eventInfo.Name;
                methodAttributes = ImplicitImplementAttributes;
            }

            EventBuilder eventBuilder = typeBuilder.DefineEvent(eventName, EventAttributes.None, eventInfo.EventHandlerType);

            //ADD
            MethodBuilder addMethodBuilder = typeBuilder.DefineMethod("add_" + eventName, methodAttributes, CallingConventions.HasThis, typeof(void), new[] { eventInfo.EventHandlerType });
            EmitAddHandler(addMethodBuilder.GetILGenerator(), eventInfo, eventName);
            addMethodBuilder.SetImplementationFlags(MethodImplAttributes.IL);

            //REMOVE
            MethodBuilder removeMethodBuilder = typeBuilder.DefineMethod("remove_" + eventName, methodAttributes, CallingConventions.HasThis, typeof(void), new[] { eventInfo.EventHandlerType });
            EmitRemoveHandler(removeMethodBuilder.GetILGenerator(), eventInfo, eventName);
            removeMethodBuilder.SetImplementationFlags(MethodImplAttributes.IL);

            // Integrate
            eventBuilder.SetAddOnMethod(addMethodBuilder);
            typeBuilder.DefineMethodOverride(addMethodBuilder, eventInfo.GetAddMethod());
            eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
            typeBuilder.DefineMethodOverride(removeMethodBuilder, eventInfo.GetRemoveMethod());
        }

        private void EmitAddHandler(ILGenerator g, EventInfo eventinfo, string eventName)
        {
            g.Emit(OpCodes.Nop);
            g.Emit(OpCodes.Ldarg_0);
            g.Emit(OpCodes.Ldstr, eventName);
            g.Emit(OpCodes.Ldarg_1);
            g.Emit(OpCodes.Call, AddHandlerMethod.MakeGenericMethod(eventinfo.EventHandlerType));
            g.Emit(OpCodes.Nop);
            g.Emit(OpCodes.Ret);
        }

        private void EmitRemoveHandler(ILGenerator g, EventInfo eventinfo, string eventName)
        {
            g.Emit(OpCodes.Nop);
            g.Emit(OpCodes.Ldarg_0);
            g.Emit(OpCodes.Ldstr, eventName);
            g.Emit(OpCodes.Ldarg_1);
            g.Emit(OpCodes.Call, RemoveHandlerMethod.MakeGenericMethod(eventinfo.EventHandlerType));
            g.Emit(OpCodes.Nop);
            g.Emit(OpCodes.Ret);
        }


        #region Property

        private void BuildProperty(TypeBuilder typeBuilder, PropertyInfo propertyInfo)
        {
            Type returnType = propertyInfo.PropertyType;
            string propertyName;
            MethodAttributes methodAttributes;

            if (CalculateShouldImplementExplicitly(propertyInfo))
            {
                propertyName = GetInterfaceImplementerName(propertyInfo);
                methodAttributes = ExplicitImplementAttributes;
            }
            else
            {
                propertyName = propertyInfo.Name;
                methodAttributes = ImplicitImplementAttributes;
            }



            Type[] parameterTypes = propertyInfo.GetIndexParameters().Select(parameter => parameter.ParameterType).ToArray();
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, returnType, parameterTypes);



            if (propertyInfo.CanRead)
            {
                MethodBuilder methodBuilder = typeBuilder.DefineMethod("get_" + propertyName, methodAttributes, CallingConventions.HasThis, propertyInfo.PropertyType, null);
                EmitPropertyGetMethodBody(methodBuilder.GetILGenerator(), propertyInfo);
                propertyBuilder.SetGetMethod(methodBuilder);
                typeBuilder.DefineMethodOverride(methodBuilder, propertyInfo.GetAccessors().Single(o => !o.IsVoidReturnType()));
            }
            if (propertyInfo.CanWrite)
            {
                MethodBuilder methodBuilder = typeBuilder.DefineMethod("set_" + propertyName, methodAttributes, CallingConventions.HasThis, typeof(void), new[] { propertyInfo.PropertyType });
                EmitPropertySet(methodBuilder.GetILGenerator(), propertyInfo);
                propertyBuilder.SetSetMethod(methodBuilder);
                typeBuilder.DefineMethodOverride(methodBuilder, propertyInfo.GetAccessors().Single(o => o.IsVoidReturnType()));
            }


        }

        private void EmitPropertyGetMethodBody(ILGenerator methodILGen, PropertyInfo propertyInfo)
        {
            methodILGen.DeclareLocal(propertyInfo.PropertyType);
            methodILGen.Emit(OpCodes.Ldarg_0);
            methodILGen.Emit(OpCodes.Ldstr, _provider.GetQualifiedName(propertyInfo));
            // added strongly typed support
            methodILGen.Emit(OpCodes.Call, GetPropertyMethod.MakeGenericMethod(propertyInfo.PropertyType));
            methodILGen.Emit(OpCodes.Stloc_0);
            methodILGen.Emit(OpCodes.Ldloc_0);
            methodILGen.Emit(OpCodes.Ret);
        }

        private void EmitPropertySet(ILGenerator methodILGen, PropertyInfo propertyInfo)
        {
            methodILGen.Emit(OpCodes.Ldarg_0);
            methodILGen.Emit(OpCodes.Ldstr, _provider.GetQualifiedName(propertyInfo));
            methodILGen.Emit(OpCodes.Ldarg_1);
            // added strongly typed support
            methodILGen.Emit(OpCodes.Call, SetPropertyMethod.MakeGenericMethod(propertyInfo.PropertyType));
            methodILGen.Emit(OpCodes.Ret);
        }



        #endregion

        /// <summary>
        /// Gets the closure of all interfaces types implemented by the specified <paramref name="interfaceType">interface type</paramref>.
        /// </summary>
        /// <param name="interfaceType">Interface type to calculate the closure of implemented interface types for.</param>
        /// <returns>Closure of implemented interface types.</returns>
        /// <remarks>No particular order is guaranteed.</remarks>
        /// <example>
        ///    interface IBar {}
        ///    interface IFoo1 : IBar {}
        ///    interface IFoo2 : IBar {}
        ///    interface ISample : IFoo1, IFoo2 {}
        /// |-
        ///    CollectionAssert.AreEquivalent(GetInterfaces(typeof(ISample)), new Type[] { typeof(ISample), typeof(IFoo1), typeof(IFoo2), typeof(IBar) })
        /// </example>
        private static IEnumerable<Type> GetInterfaces(Type interfaceType)
        {
            var interfaces = new HashSet<Type>();

            //
            // Call helper function to find closure of all interfaces to implement.
            //
            GetInterfacesInternal(interfaces, interfaceType);

            return interfaces.ToArray();
        }

        /// <summary>
        /// Helper method to calculate the closure of implemented interfaces for the specified <paramref name="interfaceType">interface type</paramref> recursively.
        /// </summary>
        /// <param name="interfaces">Collected set of interface types.</param>
        /// <param name="interfaceType">Interface type to find all implemented interfaces for recursively.</param>
        private static void GetInterfacesInternal(HashSet<Type> interfaces, Type interfaceType)
        {
            //
            // Avoid duplication.
            //
            if (!interfaces.Contains(interfaceType))
            {
                interfaces.Add(interfaceType);

                //
                // Recursive search.
                //
                foreach (Type subInterfaceType in interfaceType.GetInterfaces())
                {
                    GetInterfacesInternal(interfaces, subInterfaceType);
                }
            }
        }

        /// <summary>
        /// Gets a type builder for a type with the specified <paramref name="thunkTypeName">type name</paramref>.
        /// </summary>
        /// <param name="thunkTypeName">Name of the type to create a type builder for.</param>
        /// <param name="builder"></param>
        /// <returns>Type builder for the specified <paramref name="thunkTypeName">name</paramref>.</returns>
        private static TypeBuilder GetTypeBuilder(string thunkTypeName, ModuleBuilder builder)
        {
            return builder.DefineType(thunkTypeName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);
        }

#if !SILVERLIGHT
        public AssemblyBuilder LastAssemblyBuilder
        {
            get;
            private set;
        }
#endif

        /// <summary>
        /// Ensures the module builder singleton is available.
        /// </summary>
        private ModuleBuilder CreateModuleBuilder()
        {
            string newAssemblyName = "DuckTaperGen_" + Guid.NewGuid().ToString();
            string newModuleName = "Thunks_" + Guid.NewGuid().ToString();

            AssemblyBuilderAccess access;

#if !SILVERLIGHT
            access = AssemblyBuilderAccess.RunAndSave;
#else
            access = AssemblyBuilderAccess.Run;
#endif

            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(newAssemblyName), access);
            ModuleBuilder output;

#if !SILVERLIGHT
            output = assemblyBuilder.DefineDynamicModule(newModuleName, newModuleName + ".dll", true);
            LastAssemblyBuilder = assemblyBuilder;
#else
            output = assemblyBuilder.DefineDynamicModule(newModuleName, false);
#endif

            return output;

        }


        #endregion
    }

}
