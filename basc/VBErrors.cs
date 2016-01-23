using System;
using System.Collections.Generic;
using System.Reflection;

namespace Basic
{
    public static class VBErrors
    {
        public static readonly MessageDescriptor IdentifierExpected = new MessageDescriptor
                                                                          {
                                                                              Code = "BC30203",
                                                                              Message =
                                                                                  StringResources.
                                                                                  IdentifierExpected,
                                                                              Type = MessageType.Error,
                                                                              IsVisualizable = true
                                                                          };

        public static readonly MessageDescriptor RighParenthesisExpected = new MessageDescriptor
                                                                               {
                                                                                   Code = "BC30198",
                                                                                   Message =
                                                                                       StringResources.
                                                                                       RightParenthesisExpected,
                                                                                   Type = MessageType.Error,
                                                                                   IsVisualizable = true
                                                                               };

        public static readonly MessageDescriptor EndOfStatementExpected = new MessageDescriptor
                                                                              {
                                                                                  Code = "BC30205",
                                                                                  Message =
                                                                                      StringResources.
                                                                                      EndOfStatementExpected,
                                                                                  Type = MessageType.Error,
                                                                                  IsVisualizable = true,
                                                                                  Priority = 0
                                                                              };

        public static readonly MessageDescriptor SyntaxError = new MessageDescriptor
                                                                   {
                                                                       Code = "BC30035",
                                                                       Message = StringResources.SyntaxError,
                                                                       Type = MessageType.Error,
                                                                       IsVisualizable = true
                                                                   };

        public static readonly MessageDescriptor BracketedIdentifierIsMissingClosingRightSquareBracket =
            new MessageDescriptor
                {
                    Code = "BC30034",
                    Message = StringResources.BracketedIdentifierIsMissingClosingRightSquareBracket,
                    Type = MessageType.Error,
                    IsVisualizable = true
                };

        public static readonly MessageDescriptor
            CannotReferToAnInstanceMemberOfAClassFromWithiASharedMethodOrSharedMemberInitializerWithoutAnExplicitInstanceOfTheClass
                =
                new MessageDescriptor
                    {
                        Code = "BC30369",
                        Message =
                            StringResources.
                            CannotReferToAnInstanceMemberOfAClassFromWithiASharedMethodOrSharedMemberInitializerWithoutAnExplicitInstanceOfTheClass,
                        Type = MessageType.Error,
                        IsVisualizable = true
                    };

        public static readonly MessageDescriptor LocalVariable_variablename_IsAlreadyDeclaredInTheCurrentBlock =
            new MessageDescriptor
                {
                    Code = "BC30288",
                    Message = StringResources.LocalVariable_variablename_IsAlreadyDeclaredInTheCurrentBlock,
                    Type = MessageType.Error,
                    NumberOfArguments = 1,
                    IsVisualizable = true
                };

        public static readonly MessageDescriptor StatementCannotAppearWithinAMethodBodyEndOfMethodAssumedError =
            new MessageDescriptor
                {
                    Code = "BC30289",
                    Message = StringResources.StatementCannotAppearWithinAMethodBodyEndOfMethodAssumedError,
                    Type = MessageType.Error,
                    IsVisualizable = true
                };

        public static readonly MessageDescriptor EndSubExpected =
            new MessageDescriptor
                {
                    Code = "BC30026",
                    Message = StringResources.EndSubExpected,
                    Type = MessageType.Error,
                    IsVisualizable = true
                };

        public static readonly MessageDescriptor EndFunctionExpected =
            new MessageDescriptor
                {
                    Code = "BC30027",
                    Message = StringResources.EndFunctionExpected,
                    Type = MessageType.Error,
                    IsVisualizable = true
                };

        private static List<MessageDescriptor> items;

        public static MessageDescriptor ClassStatementMustEndWithAMatchingEndClass = new MessageDescriptor
                                                                                         {
                                                                                             Code = "BC30481",
                                                                                             Message =
                                                                                                 StringResources.
                                                                                                 ClassStatementMustEndWithAMatchingEndClass,
                                                                                             Type = MessageType.Error,
                                                                                             IsVisualizable = true
                                                                                         };

        public static MessageDescriptor ModuleStatementMustEndWithAMatchingEndModule = new MessageDescriptor
                                                                                           {
                                                                                               Code = "BC30481",
                                                                                               Message =
                                                                                                   StringResources.
                                                                                                   ModuleStatementMustEndWithAMatchingEndModule,
                                                                                               Type = MessageType.Error,
                                                                                               IsVisualizable = true
                                                                                           };

        public static MessageDescriptor ExpressionExpected = new MessageDescriptor
                                                                 {
                                                                     Code = "BC0201",
                                                                     Message = StringResources.ExpressionExpected,
                                                                     Type = MessageType.Error,
                                                                     IsVisualizable = true
                                                                 };

        public static MessageDescriptor CommaOrValidExpressionContinuationExpected = new MessageDescriptor
                                                                                         {
                                                                                             Code = "BC32017",
                                                                                             Message =
                                                                                                 StringResources.
                                                                                                 CommaOrValidExpressionContinuationExpected,
                                                                                             Type = MessageType.Error,
                                                                                             IsVisualizable = true
                                                                                         };

        public static MessageDescriptor GetError(string errorCode)
        {
            if (items == null)
            {
                items = new List<MessageDescriptor>();

                MessageDescriptor error = null;

                foreach (FieldInfo field in typeof (MessageDescriptor).GetFields(BindingFlags.Static))
                {
                    if (field.FieldType == typeof (MessageDescriptor))
                    {
                        error = (MessageDescriptor) field.GetValue(null);
                        items.Add(error);
                    }
                }
            }

            try
            {
                return items.Find(x => x.Code == errorCode);
            }
            catch (Exception)
            {
                throw new InternalCompilerException("");
            }
        }
    }
}