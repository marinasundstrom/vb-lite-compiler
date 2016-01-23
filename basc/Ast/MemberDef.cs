using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Basic.Ast
{
    /// <summary>
    /// A MemberDef.
    /// </summary>
    public class MemberDef : DeclarationSpace, IMemberDefinition
    {
        public Dictionary<string, LocalBuilder> Locals;
        protected List<AttributeInstance> attributes;
        protected FileNamespaceEntry fileNamespaceEntry;
        protected bool isDefined;

        public MemberDef(TypeDef parent, MemberName memberName, Modifier modifiers)
        {
            Modifiers = modifiers;
            MemberName = memberName;
            Parent = parent;

            Locals = new Dictionary<string, LocalBuilder>();

            attributes = new List<AttributeInstance>();
        }

        public MemberDef(TypeDef parent, string memberName, Modifier modifiers)
            : this(parent, new MemberName(memberName), modifiers)
        {
            //this.Modifiers = modifiers;
        }

        /// <summary>
        /// Gets the member name.
        /// </summary>
        public MemberName MemberName { get; private set; }

        /// <summary>
        /// Gets the modifiers,
        /// </summary>
        public Modifier Modifiers { get; set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public TypeDef Parent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this member has been defined or not.
        /// </summary>
        public bool IsDefined
        {
            get { return isDefined; }
        }

        /// <summary>
        /// Gets the fullname.
        /// </summary>
        public string FullName
        {
            get { return MemberName.BaseName; }
        }

        /// <summary>
        /// Gets an object that represents the namespace in a specific file in which this member has been declared.
        /// </summary>
        public FileNamespaceEntry FileNamespaceEntry
        {
            get
            {
                if (fileNamespaceEntry == null)
                {
                    return Parent.fileNamespaceEntry;
                }

                return fileNamespaceEntry;
            }
        }

        public bool IsType
        {
            get { return this is TypeDef; }
        }

        #region IMemberDefinition Members

        ITypeDefinition IMemberDefinition.DeclaringType
        {
            get { return Parent; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return MemberName.Name; }
        }

        public bool IsShared
        {
            get { return (Modifiers & Modifier.Shared) == Modifier.Shared; }
        }

        public bool IsPublic
        {
            get { return (Modifiers & Modifier.Public) == Modifier.Public; }
        }

        public bool IsPrivate
        {
            get { return (Modifiers & Modifier.Private) == Modifier.Private; }
        }

        public bool IsAssembly
        {
            get { return (Modifiers & Modifier.Internal) == Modifier.Internal; }
        }

        /// <summary>
        /// Gets the MemberInfo.
        /// </summary>
        /// <returns></returns>
        public MemberInfo GetMemberInfo()
        {
            if (this is ClassStructOrModuleDef)
                return TypeBuilder;

            if (this is EnumDef)
                return EnumBuilder;

            if (this is MethodDef)
                return MethodBuilder;

            if (this is FieldDef)
                return FieldBuilder;

            throw new Exception();
        }


        public bool IsFamily
        {
            get { return (Modifiers & Modifier.Protected) == Modifier.Protected; }
        }

        public virtual ITypeDefinition[] GetAttributeTypes(bool inherit)
        {
            return attributes.Select(x => x.AttributeType).ToArray();
        }

        public virtual AttributeInstance[] GetAttributes(bool inherit)
        {
            var types = new List<ITypeDefinition>();

            return attributes.ToArray();
        }

        #endregion

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <param name="genericArgs">Should it be the signature of a generic type.</param>
        /// <returns></returns>
        public virtual string GetSignature(bool genericArgs)
        {
            return FullName;
        }

        /// <summary>
        /// Deines the member.
        /// </summary>
        public virtual void Define()
        {
            if (isDefined)
                throw new Exception("Member is already defined.");

            isDefined = true;
        }

        /// <summary>
        /// Resolves the member.
        /// </summary>
        public virtual void Resolve()
        {
        }

        /// <summary>
        /// Emits IL.
        /// </summary>
        public virtual void Emit()
        {
        }

        //public void  FullNamedExpression LookupMember(string name)
        //{

        //}      

        //public void  FullNamedExpression LookupAlias(string name)
        //{

        //} 

        //public void  FullNamedExpression LookupNamespaceOrType(string name)
        //{

        //}

        public override string ToString()
        {
            return FullName;
        }

        public bool SetAttribute(AttributeInstance attribute)
        {
            return false;
        }
    }
}