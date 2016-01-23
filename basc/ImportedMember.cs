using System.Linq;
using System.Reflection;

namespace Basic
{
    public abstract class ImportedMember : IMemberDefinition
    {
        private readonly ITypeDefinition declaringType;
        protected MemberInfo member;

        public ImportedMember(MemberInfo member, ImportedType parent)
        {
            this.member = member;
            declaringType = parent;
        }

        #region IMemberDefinition Members

        public ITypeDefinition DeclaringType
        {
            get { return declaringType; }
        }

        public string Name
        {
            get { return member.Name; }
        }

        public MemberInfo GetMemberInfo()
        {
            return member;
        }

        public abstract bool IsShared { get; }

        public abstract bool IsPrivate { get; }

        public abstract bool IsPublic { get; }

        public abstract bool IsFamily { get; }

        public abstract bool IsAssembly { get; }

        public ITypeDefinition[] GetAttributeTypes(bool inherit)
        {
            return GetMemberInfo()
                .GetCustomAttributes(inherit)
                .Select(x => TypeManager.ResolveType(x.GetType().FullName))
                .ToArray();
        }

        public AttributeInstance[] GetAttributes(bool inherit)
        {
            return GetMemberInfo()
                .GetCustomAttributes(inherit)
                .Select(x => new AttributeInstance(x))
                .ToArray();
        }

        #endregion
    }
}