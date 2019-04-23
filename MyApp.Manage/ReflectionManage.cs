using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using MyApp.ServiceModel.Common;

namespace MyApp.Manage
{
    public class ReflectionManage : ManageBase, IReflectionManage
    {
        private static void CreateProperty(TypeBuilder typeBuilder, EntityProperty entityProperty)
        {
            var fieldBuilder =
                typeBuilder.DefineField("_" + entityProperty.Name, entityProperty.Type, FieldAttributes.Private);
            var propertyBuilder =
                typeBuilder.DefineProperty(entityProperty.Name, PropertyAttributes.HasDefault, entityProperty.Type,
                    null);
            var getPropMthdBldr = typeBuilder.DefineMethod("get_" + entityProperty.Name,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                entityProperty.Type,
                Type.EmptyTypes);
            var getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr =
                typeBuilder.DefineMethod("set_" + entityProperty.Name,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] {entityProperty.Type});

            var setIl = setPropMthdBldr.GetILGenerator();
            var modifyProperty = setIl.DefineLabel();
            var exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        public async Task<TypeInfo> CreateEntityType(EntityDef entityDef)
        {
            var assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(entityDef.Name), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var typeBuilder = moduleBuilder.DefineType(entityDef.Name,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                entityDef.BaseType);

            typeBuilder.DefineDefaultConstructor(
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName);

            foreach (var field in entityDef.Properties)
            {
                CreateProperty(typeBuilder, field);
            }

            return await Task.FromResult(typeBuilder.CreateTypeInfo());
        }
    }
}