using System.Reflection;

namespace Basic
{
    public class ImportedField : ImportedMember, IFieldDefinition
    {
        protected ImportedField(FieldInfo field)
            : base(field, null)
        {
        }

        public ImportedField(FieldInfo field, ImportedType parent)
            : base(field, parent)
        {
        }

        #region IFieldDefinition Members

        public FieldInfo GetFieldInfo()
        {
            return (FieldInfo) member;
        }

        public ITypeDefinition FieldType
        {
            get { return TypeManager.GetType(((FieldInfo) member).FieldType.FullName); }
        }

        public override bool IsShared
        {
            get { return GetFieldInfo().IsStatic; }
        }

        public override bool IsPrivate
        {
            get { return GetFieldInfo().IsPrivate; }
        }

        public override bool IsPublic
        {
            get { return GetFieldInfo().IsPublic; }
        }

        public override bool IsFamily
        {
            get { return GetFieldInfo().IsFamily; }
        }

        public override bool IsAssembly
        {
            get { return GetFieldInfo().IsAssembly; }
        }

        #endregion
    }
}