﻿//
// CURRENTLY
//    Generate base classes without a call to base
//    Generate the standard idioms/patterns for init, dispose
//
// WARNING
//
//    This file still contains code from the original CppBind, it is some legacy code
//    so it might be possible to remove.
//
//TODO:
//  operators
// 

using System;
using System.IO;
using System.Collections.Generic;
using Clang.Ast;
using ICSharpCode.NRefactory.CSharp;
using System.Linq;
using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;
using System.Reflection;
using Sharpie.Bind;
using System.Text;


namespace SharpieBinder
{
	class CxxBinder : AstVisitor
	{
		CSharpParser csParser = new CSharpParser();
		public static Clang.Ast.Type CleanType(QualType qt)
		{
			var et = qt.Type as ElaboratedType;
			if (et == null)
				return qt.Type;
			return et.UnqualifiedDesugaredType;
		}
		public static string CleanTypeCplusplus(QualType qt)
		{
			var s = CleanType(qt).ToString();
			if (s == "_Bool")
				return "bool";

			// Ok, I want to click on that blue thing and use the action, the "1"//
			if (s.IndexOf("_Bool") != -1)
				Console.WriteLine("f");
			if (s.StartsWith("class ")) {
				s = s.Substring(6);
				return s;
			}
			if (s.StartsWith("struct "))
				return s.Substring(7);
			return s;
		}
		class BaseNodeType
		{
			public CXXRecordDecl Decl { get; set; }

			readonly Action<CXXRecordDecl, CXXRecordDecl> bindHandler;

			public BaseNodeType(Action<CXXRecordDecl, CXXRecordDecl> bindHandler)
			{
				this.bindHandler = bindHandler;
			}

			public void Bind()
			{
				bindHandler(null, Decl);
			}

			public void Bind(CXXRecordDecl decl)
			{
				bindHandler(Decl, decl);
			}
		}

		readonly List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
		readonly Dictionary<string, BaseNodeType> baseNodeTypes;

		TypeDeclaration astVisitorType;
		TypeDeclaration currentType;
		StreamWriter cbindingStream;

		public CxxBinder(string outputDir)
		{
			baseNodeTypes = new Dictionary<string, BaseNodeType>
			{
				["Urho3D::Object"] = new BaseNodeType(Bind),
				["Urho3D::RefCounted"] = new BaseNodeType(Bind),
				["Urho3D::RefCount"] = new BaseNodeType(Bind),
				["Urho3D::String"] = new BaseNodeType(Bind),
				["Urho3D::XMLElement"] = new BaseNodeType(Bind)
			};
			cbindingStream = File.CreateText(Path.Combine(outputDir, "binding.cpp"));
			pn("// Autogenerated, do not edit");
			pn ("#define URHO3D_OPENGL");
			pn("#include \"../AllUrho.h\"");
			pn("#include \"../src/asserts.h\"");
			pn("using namespace Urho3D;");
			pn ("extern \"C\" {");

		}

		public void Close()
		{
			pn ("}");
			cbindingStream.Close();
			cbindingStream = null;
		}

		public void p(string fmt, params object[] args)
		{
			if (args.Length == 0)
				cbindingStream.Write(fmt);
			else
				cbindingStream.Write(String.Format(fmt, args));
		}

		public void pn(string fmt, params object[] args)
		{
			if (args.Length == 0)
				cbindingStream.WriteLine(fmt);
			else
				cbindingStream.WriteLine(String.Format(fmt, args));
		}

		public IEnumerable<SyntaxTree> Generate()
		{
			foreach (var syntaxTree in syntaxTrees) {
				//syntaxTree.AcceptVisitor(new Sharpie.Bind.Massagers.GenerateUsingStatementsMassager());

				yield return syntaxTree;
			}
		}

		CXXRecordDecl GetRootDecl(CXXRecordDecl decl)
		{
			return baseNodeTypes.Values.FirstOrDefault(node =>
								   node.Decl != null &&
								   node.Decl.Name != "DeclContext" &&
								   decl.IsDerivedFrom(node.Decl))?.Decl;
		}

		void PushType(TypeDeclaration typeDeclaration)
		{
			var syntaxTree = new SyntaxTree
			{
				FileName = typeDeclaration.Name + ".cs"
			};
			//Console.WriteLine(syntaxTree);

			var headerLines = String.Format(
				"WARNING - AUTOGENERATED - DO NOT EDIT\n\n" +
				"Generated using `sharpie urho`\n\n" +
				"{0}\n\n" +
				"Copyright 2015 Xamarin Inc. All rights reserved.", syntaxTree.FileName
			).Split('\n');
			headerLines[headerLines.Length - 1] += "\n";

			foreach (var line in headerLines)
				syntaxTree.Members.Add(new Comment(" " + line));

			syntaxTree.Members.Add(new UsingDeclaration("System"));
			syntaxTree.Members.Add(new UsingDeclaration() { Import = new SimpleType("System.Runtime.InteropServices") });

			var ns = new NamespaceDeclaration("Urho");
			syntaxTree.Members.Add(ns);
			syntaxTrees.Add(syntaxTree);

			allTypes[typeDeclaration.Name] = typeDeclaration;
			currentTypeNames.Clear();
			uniqueMethodName = 0;
			ns.Members.Add(currentType = typeDeclaration);
		}
		HashSet<string> currentTypeNames = new HashSet<string>();
		int uniqueMethodName;
		Dictionary<string, TypeDeclaration> allTypes = new Dictionary<string, TypeDeclaration>();

		public override void VisitEnumDecl(EnumDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter || !decl.IsCompleteDefinition || decl.QualifiedName == null)
				return;

			//Console.WriteLine($"VisitingType: {decl.QualifiedName}");
			string typeName = RemapTypeName(decl.Name);

			PushType(new TypeDeclaration
			{
				Name = typeName,
				ClassType = ClassType.Enum,
				Modifiers = Modifiers.Public
			});

			foreach (var constantDecl in decl.Decls<EnumConstantDecl>()) {
				var valueName = constantDecl.Name;

				switch (valueName) {
				// LIST HERE ANY Values we want to skip
				case "foo":
					continue;

				}
				var x = new EnumMemberDeclaration();

				currentType.Members.Add(new EnumMemberDeclaration { Name = valueName });
			}
		}


		public override void VisitCXXRecordDecl(CXXRecordDecl decl, VisitKind visitKind)
		{
			//Console.WriteLine($"{decl.QualifiedName} {visitKind} ");
			if (decl.QualifiedName == ("Urho3D::String")) {
				//Console.WriteLine($"{decl.QualifiedName} {visitKind} {decl.IsCompleteDefinition}");
			}

			if (visitKind != VisitKind.Enter || !decl.IsCompleteDefinition || decl.Name == null) {
				return;
			}
			if (visitKind == VisitKind.Leave)
				currentType = null;


			BaseNodeType baseNodeType;
			if (baseNodeTypes.TryGetValue(decl.QualifiedName, out baseNodeType)) {
				baseNodeType.Decl = decl;
				baseNodeType.Bind();
				return;
			}

			foreach (var bnt in baseNodeTypes.Values) {
				if (bnt.Decl != null && decl.IsDerivedFrom(bnt.Decl)) {
					bnt.Bind(decl);
					return;
				}
			}
		}

		//
		// Determines if this is a structure that is part of native interop, so we would
		// keep the layout the same as managed code
		//
		public bool IsStructure(CXXRecordDecl decl)
		{
			// Quick and dirty: it really should use our manually curated list of known value types,
			// but for now, this will do
			if (decl.Name == "String")
				return false;

			if (decl.TagKind == TagDeclKind.Struct || !(decl.IsDerivedFrom(ScanBaseTypes.UrhoRefCounted) || decl == ScanBaseTypes.UrhoRefCounted))
				return true;
			
			return false;
		}

		void Bind(CXXRecordDecl baseDecl, CXXRecordDecl decl)
		{
			var name = decl.Name;
			//Console.WriteLine(name);

			bool isStruct = IsStructure (decl);

			PushType(new TypeDeclaration
			{
				Name = RemapTypeName (decl.Name),
				ClassType = isStruct ? ClassType.Struct : ClassType.Class,
				Modifiers = Modifiers.Partial | Modifiers.Public | Modifiers.Unsafe
			});

			if (baseDecl != null) {
				foreach (var baseType in decl.Bases) {
					var baseName = baseType.Decl?.Name;
					if (baseName == null)
						continue;

					// WorkItem is a structure, with a RefCount base class, avoid that
					if (IsStructure(decl))
						continue;
					// 
					// Only a (File, MemoryBuffer, VectorBuffer)
					// implement that, and the Serializer class is never
					// surfaced as an API entry point, so we should just inline the methods
					// from those classes into the generated class
					//
					if (baseName == "GPUObject" || baseName == "Thread" || baseName == "Octant" ||
					   baseName == "b2Draw" || baseName == "b2ContactListener")
						continue;

					if (currentType.BaseTypes.Count > 0)
						baseName = "I" + baseName;

					currentType.BaseTypes.Add(new SimpleType(RemapTypeName(baseName)));
				}
			}

			// Determines if this is a subclass of RefCounted (but not RefCounted itself)
			bool refCountedSubclass = decl.TagKind == TagDeclKind.Class && decl.QualifiedName != "Urho3D::RefCounted" && decl.IsDerivedFrom(ScanBaseTypes.UrhoRefCounted);

			if (refCountedSubclass) {
				var nativeCtor = new ConstructorDeclaration
				{
					Modifiers = Modifiers.Internal,
					Body = new BlockStatement(),
					Initializer = new ConstructorInitializer()
				};


				nativeCtor.Parameters.Add(new ParameterDeclaration(new SimpleType("IntPtr"), "handle"));
				nativeCtor.Initializer.Arguments.Add(new IdentifierExpression("handle"));

				currentType.Members.Add(nativeCtor);

				// The construtor with the emtpy chain flag
				nativeCtor = new ConstructorDeclaration
				{
					Modifiers = Modifiers.Internal,
					Body = new BlockStatement(),
					Initializer = new ConstructorInitializer()
				};


				nativeCtor.Parameters.Add(new ParameterDeclaration(new SimpleType("UrhoObjectFlag"), "emptyFlag"));
				nativeCtor.Initializer.Arguments.Add(new IdentifierExpression("emptyFlag"));

				currentType.Members.Add(nativeCtor);

			} else if (IsStructure(decl)) {
				var serializable = new Attribute()
				{
					Type = new SimpleType("StructLayout")
				};

				serializable.Arguments.Add(new TypeReferenceExpression(new MemberType(new SimpleType("LayoutKind"), "Sequential")));
				var attrs = new AttributeSection();
				attrs.Attributes.Add(serializable);
				currentType.Attributes.Add(attrs);
			}
		}

		public static AstType CreateAstType(string dottedName, IEnumerable<AstType> typeArguments)
		{
			var parts = dottedName.Split('.');
			if (parts.Length == 1)
				return new SimpleType(dottedName, typeArguments);

			AstType type = new SimpleType(parts[0]);

			for (int i = 1; i < parts.Length; i++) {
				if (i < parts.Length - 1)
					type = new MemberType(type, parts[i]);
				else
					type = new MemberType(type, parts[i], typeArguments);
			}

			return type;
		}

		AstType GenerateReflectedType(System.Type type)
		{
			if (type == typeof(void))
				return new PrimitiveType("void");

			var name = type.FullName;
			if (type.IsGenericType)
				name = name.Substring(0, name.IndexOf('`'));

			return CreateAstType(name, type.GetGenericArguments().Select(at => GenerateReflectedType(at)));
		}

		static string MakeName(string typeName)
		{
			return typeName;
		}

		const string ConstStringReference = "const class Urho3D::String &";
		// Temporary, just to help us get the bindings bootstrapped
		static bool IsUnsupportedType(QualType qt)
		{
			var ct = CleanType(qt);
			#if true || STRING_REF
			if (ct.ToString() == ConstStringReference)
				return false;
			#endif
			var s = ct.Bind().ToString();
			if (s.Contains ("unsupported")) {
				return true;
			}

			// Quick hack, just while we get thigns going
			if (s.Contains("EventHandler**")) {
				return true;
			}
			if (s.Contains("b2"))
				return true;
			if (s.StartsWith("dt"))
				return true;

			// Candidate for a hand binding (Connection.h)
			if (qt.ToString().Contains("kNet::SharedPtr"))
				return true;
			if (s.Contains("XMLElement"))
				return true;
			return false;
		}

		// 
		// Given a pointer type, returns the underlying type 
		//
		static RecordType GetPointersUnderlyingType(PointerType ptrType)
		{
			return (ptrType.PointeeQualType.Type.UnqualifiedDesugaredType as RecordType);
		}

		enum WrapKind
		{
			None,
			HandleMember,
			EventHandler,
			StringHash
		}
		// 
		// Given a Clang QualType, returns the AstType to use to marshal, both for the 
		// P/Invoke signature and for the high level binding
		//
		// Handles RefCounted objects that we wrap
		//
		void LookupMarshalTypes(QualType qt, 
		                        out AstType lowLevel, out ICSharpCode.NRefactory.CSharp.ParameterModifier lowLevelParameterMod, 
		                        out AstType highLevel, out ICSharpCode.NRefactory.CSharp.ParameterModifier highLevelParameterMod, 
		                        out WrapKind wrapKind, bool isReturn = false)
		{
			wrapKind = WrapKind.None;
			lowLevelParameterMod = ICSharpCode.NRefactory.CSharp.ParameterModifier.None;
			highLevelParameterMod = ICSharpCode.NRefactory.CSharp.ParameterModifier.None;

			var cleanType = CleanType(qt);
			var cleanTypeStr = cleanType.ToString();
			switch (cleanTypeStr) {
			case "const char *":
				lowLevel = new PrimitiveType("string");
				highLevel = new PrimitiveType("string");
				return;
			case "ThreadID":
			// Troublesome because on windows it is a sizeof(unsigned), on unix sizeof(pthread_t), which is 
			// 32 or 64 bits.
			case "void *":
				lowLevel = new PrimitiveType("IntPtr");
				highLevel = new PrimitiveType("IntPtr");
				return;
			
			case ConstStringReference:
				if (isReturn) {
					lowLevel = csParser.ParseTypeReference("string");
					highLevel = csParser.ParseTypeReference("string");
					return;
				} else {
					lowLevel = csParser.ParseTypeReference("string");
					highLevel = new PrimitiveType("string");
					return;
				}
			case "class Urho3D::StringHash":
				highLevel = new SimpleType ("StringHash");
				lowLevel = new PrimitiveType ("int");
				wrapKind = WrapKind.StringHash;
				return;
			default:
				//Console.WriteLine (cleanTypeStr);
				break;
            }

			if (cleanType.Kind == TypeKind.Pointer) {
				var ptrType = cleanType as PointerType;
				var underlying = GetPointersUnderlyingType(ptrType);

				CXXRecordDecl decl;
				if (underlying != null && ScanBaseTypes.nameToDecl.TryGetValue(underlying.Decl.QualifiedName, out decl)) {
					if (decl.IsDerivedFrom(ScanBaseTypes.UrhoRefCounted)) {
						lowLevel = new SimpleType("IntPtr");
						var remapped = RemapTypeName(decl.Name);
						if (remapped != decl.Name)
							highLevel = csParser.ParseTypeReference("Urho." + remapped);
						else
							highLevel = underlying.Bind();
						wrapKind = WrapKind.HandleMember;
						return;
					}
					if (decl == ScanBaseTypes.EventHandlerType) {
						wrapKind = WrapKind.EventHandler;
						lowLevel = new SimpleType("IntPtr");
						highLevel = new SimpleType("IntPtr");
						return;
					}
					if (decl.Name == "ProfilerBlock") {
						lowLevel = new SimpleType("ProfilerBlock");
						highLevel = new SimpleType("ProfilerBlock");

					}
					if (decl.Name == "Deserializer") {
						lowLevel = new SimpleType("IntPtr");
						highLevel = new SimpleType("IDeserializer");
						wrapKind = WrapKind.HandleMember;
						return;
					}
					if (decl.Name == "Serializer") {
						lowLevel = new SimpleType("IntPtr");
						highLevel = new SimpleType("ISerializer");
						wrapKind = WrapKind.HandleMember;
						return;
					}
				}
			}
			lowLevel = cleanType.Bind();
			highLevel = cleanType.Bind();
		}

		public string RemapMemberName(string name)
		{
			if (name == "GetType")
				return "UrhoGetType";
			if (name == "GetBaseType")
				return "UrhoGetBaseType";
			if (name == "ToString")
				return "ToDebugString";
			return name;
		}

		public string RemapTypeName(string type)
		{
			switch (type) {
			case "Object": return "UrhoObject";
			case "String": return "UrhoString";
			case "Console": return "UrhoConsole";
			default:
				return type;
			}
		}

		// Avoid generating methods that conflict in their signatures after we turn Urho::String into string
		bool SkipMethod (CXXMethodDecl decl)
		{
			switch (currentType.Name) {
			case "Graphics":
				if (decl.Name == "GetShader") 
					return decl.Parameters.Skip (1).First ().QualType.ToString () == "const char *";
				break;
			case "Node":
				if (decl.Name == "GetChild")
					return decl.Parameters.First ().QualType.ToString () == "const char *";
				break;
			case "Shader":
				if (decl.Name == "GetVariation")
					return decl.Parameters.Skip (1).First ().QualType.ToString () == "const char *";
				break;
			}

			return false;
		}

		public override void VisitCXXMethodDecl(CXXMethodDecl decl, VisitKind visitKind)
		{
			// Global definitions, not inside a class, skip
			if (currentType == null) {
				return;
			}

			if (visitKind != VisitKind.Enter)
				return;

			var isConstructor = decl is CXXConstructorDecl;
			if (isConstructor && decl.Parent.IsAbstract)
				return;

			if (decl is CXXDestructorDecl)
				return;

			if (decl.IsCopyAssignmentOperator || decl.IsMoveAssignmentOperator)
				return;

			// TODO: temporary, do not handle opreators
			if (!isConstructor && decl.Name.StartsWith("operator", StringComparison.Ordinal))
				return;

			if (RemapTypeName (decl.Parent.Name) != currentType.Name) {
				//Console.WriteLine("For some reason we go t amethod that does not belong here {0}.{1} on {2}", decl.Parent.Name, decl.Name, currentType.Name);
				return;
			}

			if (decl.Access != AccessSpecifier.Public)
				return;

			if (SkipMethod (decl))
				return;

			// Temporary: while we add support for other things, just to debug things
			// remove types we do not support
			foreach (var p in decl.Parameters) {
				if (IsUnsupportedType(p.QualType)) {

					//Console.WriteLine($"Bailing out on {p.QualType} from {decl.QualifiedName}");
					return;
				}
			}
			if (IsUnsupportedType(decl.ReturnQualType)) {
				// HANDLE HERE STRING
				//Console.WriteLine($"RETURN Bailing out on {decl.ReturnQualType} from {decl.QualifiedName}");
				return;
			}

			AstType pinvokeReturn, methodReturn;
			WrapKind returnIsWrapped;
			ICSharpCode.NRefactory.CSharp.ParameterModifier pinvokeMod, methodMod;

			LookupMarshalTypes(decl.ReturnQualType, out pinvokeReturn, out pinvokeMod, out methodReturn, out methodMod, out returnIsWrapped, isReturn: true);
			var methodReturn2 = methodReturn.Clone();

			var propertyInfo = ScanBaseTypes.GetPropertyInfo(decl);
			if (propertyInfo != null) {
				propertyInfo.HostType = currentType;
				if (decl.Name.StartsWith("Get"))
					propertyInfo.MethodReturn = methodReturn.Clone();
			}

			var methodName = decl.Name;
			if (currentTypeNames.Contains(methodName))
				methodName += (uniqueMethodName++).ToString();
			currentTypeNames.Add(methodName);

			// PInvoke declaration
			string pinvoke_name = MakeName(currentType.Name) + "_" + methodName;
			if (isConstructor) {
				pinvokeReturn = new SimpleType("IntPtr");
			}
			var pinvoke = new MethodDeclaration
			{
				Name = pinvoke_name,
				ReturnType = pinvokeReturn,
				Modifiers = Modifiers.Extern | Modifiers.Static | Modifiers.Internal
			};
			if (!decl.IsStatic && !isConstructor)
				pinvoke.Parameters.Add(new ParameterDeclaration(new SimpleType("IntPtr"), "handle"));


			var dllImport = new Attribute()
			{
				Type = new SimpleType("DllImport")
			};
			dllImport.Arguments.Add(new PrimitiveExpression("mono-urho"));

			pinvoke.Attributes.Add(new AttributeSection(dllImport));
			currentType.Members.Add(pinvoke);

			// The C counterpart
			var cinvoke = new StringBuilder();
			string marshalReturn = "{0}";
			string creturnType = CleanTypeCplusplus(decl.ReturnQualType);

			string extra_code;
			switch (creturnType) {
			case "Urho3D::StringHash":
				creturnType = "int";
				marshalReturn = "({0}).Value ()";
				break;
			case "const class Urho3D::String &":
				creturnType = "const char *";
				marshalReturn = "({0}).CString ()";
				break;
			
			}
			
			if (isConstructor)
				p($"void *\n{pinvoke_name} (");
			else 
				p($"{creturnType}\n{pinvoke_name} (");

			if (decl.IsStatic) {
				cinvoke.Append($"{decl.Parent.Name}::{decl.Name} (");

			} else if (isConstructor) {
				cinvoke.Append($"new {decl.Name} (");
			} else {
				p($"{decl.Parent.Name} *_target");
				if (decl.Parameters.Count() > 0)
					p(", ");
				cinvoke.Append($"_target->{decl.Name} (");
			}
			// Method declaration
			MethodDeclaration method = null;
			ConstructorDeclaration constructor = null;

			if (isConstructor) {
				constructor = new ConstructorDeclaration
				{
					Name = RemapMemberName(decl.Name),

					Modifiers = (decl.IsStatic ? Modifiers.Static : 0) |
						(propertyInfo != null ? Modifiers.Private : Modifiers.Public)  |
						(decl.Name == "ToString" ? Modifiers.Override : 0)
				};
				constructor.Body = new BlockStatement();
			} else {
				method = new MethodDeclaration
				{
					Name = RemapMemberName(decl.Name),
					ReturnType = methodReturn,
					Modifiers = (decl.IsStatic ? Modifiers.Static : 0) |
						(propertyInfo != null ? Modifiers.Private : Modifiers.Public)
				};
				method.Body = new BlockStatement();
			}


			var invoke = new InvocationExpression(new IdentifierExpression(pinvoke_name));
			if (!decl.IsStatic && !isConstructor)
				invoke.Arguments.Add(new IdentifierExpression("handle"));
			bool first = true;
			int anonymousParameterNameCount = 1;
			foreach (var param in decl.Parameters) {
				AstType pinvokeParameter, parameter;
				WrapKind wrapKind;

				if (!first) {
					cinvoke.Append(", ");
					p(", ");
				} else
					first = false;

				LookupMarshalTypes(param.QualType, out pinvokeParameter, out pinvokeMod, out parameter, out methodMod, out wrapKind);

				string paramName = param.Name;
				if (paramName == "" || paramName == null)
					paramName = "param" + (anonymousParameterNameCount++);

				if (constructor == null)
					method.Parameters.Add(new ParameterDeclaration(parameter, paramName, methodMod));
				else
					constructor.Parameters.Add(new ParameterDeclaration(parameter, paramName, methodMod));

				pinvoke.Parameters.Add(new ParameterDeclaration(pinvokeParameter, paramName, pinvokeMod));
				if (wrapKind == WrapKind.HandleMember) {
					var cond = new ConditionalExpression (new BinaryOperatorExpression (new CastExpression (new PrimitiveType ("object"), new IdentifierExpression (paramName)), BinaryOperatorType.Equality, new PrimitiveExpression (null)),
						           csParser.ParseExpression ("IntPtr.Zero"), csParser.ParseExpression (paramName + ".handle"));

					invoke.Arguments.Add (cond);
				} else if (wrapKind == WrapKind.StringHash) {
					invoke.Arguments.Add (csParser.ParseExpression (paramName + ".Code"));
				} else {

					invoke.Arguments.Add(new IdentifierExpression(paramName));
				}
				var ctype = CleanTypeCplusplus (param.QualType);
				string paramInvoke = paramName;
				switch (ctype) {
				case "const class Urho3D::String &":
					ctype = "const char *";
					paramInvoke = $"Urho3D::String({paramInvoke})";
					break;
				case "Urho3D::StringHash":
					ctype = "int";
					paramInvoke = $"Urho3D::StringHash({paramInvoke})";
					break;
				}

				p($"{ctype} {paramName}");
				cinvoke.Append($"{paramInvoke}");
			}
			cinvoke.Append(")");
			p(")\n{\n\t");

			if (method != null && methodReturn is Sharpie.Bind.Types.VoidType) {
				method.Body.Add(invoke);
				pn($"{cinvoke.ToString()};");
			} else {
				ReturnStatement ret = null;
				Expression returnExpression;


				if (!isConstructor)
					ret = new ReturnStatement();

				if (returnIsWrapped == WrapKind.HandleMember) {

					//var id = new IdentifierExpression("LookupObject");
					//id.TypeArguments.Add(methodReturn2);

					//ret.Expression = new InvocationExpression(id, invoke);
					returnExpression = new ObjectCreateExpression (methodReturn2, invoke); // new IdentifierExpression("handle"));
				} else if (returnIsWrapped == WrapKind.EventHandler) {
					returnExpression = invoke;
				} else if (returnIsWrapped == WrapKind.StringHash) {
					returnExpression = new ObjectCreateExpression (new SimpleType ("StringHash"), invoke);
				} else {
					returnExpression = invoke;
				}
				if (ret != null) {
					ret.Expression = returnExpression;
					method.Body.Add(ret);
				} else {
					if (currentType.ClassType == ClassType.Class){
						if (currentType.BaseTypes.Count() != 0) {

							constructor.Initializer = new ConstructorInitializer()
							{
								ConstructorInitializerType = ConstructorInitializerType.Base,
							};

							constructor.Initializer.Arguments.Add(csParser.ParseExpression("UrhoObjectFlag.Empty"));
						}
						var ctorAssign = new AssignmentExpression(new IdentifierExpression("handle"), returnExpression);
						constructor.Body.Add(new ExpressionStatement(ctorAssign));
					}
				}
				var rstr = String.Format(marshalReturn, cinvoke.ToString());
				pn($"{extra_code}");
				pn($"return {rstr};");
			}
			pn("}\n");

			if (method == null)
				currentType.Members.Add(constructor);
			else
				currentType.Members.Add(method);
		}

		void ScanBases(TypeDeclaration td, Func<TypeDeclaration, bool> cback)
		{
			if (td.BaseTypes.Count() == 0)
				return;
			var j = td.BaseTypes.First();
			var btype = allTypes[RemapTypeName ((j as SimpleType).Identifier)];
			if (cback(btype))
				return;
			ScanBases(btype, cback);
		}

		public void ForAllMembers(Action<TypeDeclaration, EntityDeclaration> handler)
		{
			foreach (var typeDeclaration in allTypes.Values) {
				foreach (var member in typeDeclaration.Members)
					handler(typeDeclaration, member);
			}
		}

		void UpdateMembers(EntityDeclaration baseDeclaration, EntityDeclaration declaration)
		{
			if (declaration.Modifiers.HasFlag(Modifiers.Static)) {
				declaration.Modifiers |= Modifiers.New;
			} else {
				declaration.Modifiers |= Modifiers.Override;
				baseDeclaration.Modifiers |= Modifiers.Virtual;
			}
		}

		public void FixupOverrides()
		{
			ForAllMembers((typeDeclaration, member) =>
			{
				if (member is PropertyDeclaration) {
					var property = member as PropertyDeclaration;
					ScanBases(typeDeclaration, baseType =>
					{
						foreach (var baseProperty in baseType.Members.Where(x => x is PropertyDeclaration).Select(x => x as PropertyDeclaration)) {
							if (baseProperty.Name == property.Name) {
								UpdateMembers(baseProperty, property);
								return true;
							}
						}
						return false;
					});
				}
				if (member is MethodDeclaration) {
					var method = member as MethodDeclaration;
					if (method.Modifiers.HasFlag(Modifiers.Private))
						return;
					ScanBases(typeDeclaration, baseType =>
					{
						foreach (var baseMethod in baseType.Members.Where(x => x is MethodDeclaration).Select(x => x as MethodDeclaration)) {
							if (baseMethod.Name != method.Name)
								continue;

							if (baseMethod.Parameters.Count() != method.Parameters.Count())
								continue;

							var bpars = baseMethod.Parameters.ToArray();
							var pars = method.Parameters.ToArray();
							bool fail = false;
							for (int i = 0; i < pars.Length; i++) {
								//Console.WriteLine("{0} {1} {2}", bpars[i].Type, pars[i].Type, pars[i].Type);
								if (bpars[i].Type.ToString() != pars[i].Type.ToString()) {
									fail = true;
									break;
								}
							}
							if (!fail) {
								UpdateMembers(baseMethod, method);
								return true;
							}

						}
						return false;
					});
				}
			});

			ForAllMembers((typeDeclaration, member) =>
		   {
			   if (member.Modifiers.HasFlag(Modifiers.Virtual | Modifiers.Override))
				   member.Modifiers = member.Modifiers & ~Modifiers.Virtual;
		   });
		}

		public void GenerateProperties()
		{
			foreach (var typeKV in ScanBaseTypes.allProperties) {
				foreach (var propNameKV in typeKV.Value) {
					foreach (var gs in propNameKV.Value.Values) {
						string pname = gs.Name;
						Modifiers mods = 0;
						switch (typeKV.Key) {

						case "UIElement":
							if (pname == "BringToFront")
								pname = "BringToFrontOnFocus";
							if (pname == "BringToBack")
								pname = "BringToBackOnFocus";
							if (pname == "SortChildren")
								pname = "ShouldSortChildren";
							break;
						case "Menu":
							if (pname == "ShowPopup")
								pname = "IsPopupShown";
							break;
						case "File":
							if (pname == "Handle")
								pname = "FileHandle";
							break;
						case "Text":
							if (pname == "Text")
								pname = "Value";
							break;
						case "View":
							// View.Graphics is the Urho C++ strong type to GetSubssytem(Graphics)
							if (pname == "Graphics" || pname == "Renderer")
								mods = Modifiers.New;
							
							break;
						}

						var p = new PropertyDeclaration()
						{
							Name = pname,
							ReturnType = gs.MethodReturn,
							Modifiers = Modifiers.Public | (gs.Getter.IsStatic ? Modifiers.Static : 0) | mods
						};

						p.Getter = new Accessor()
						{
							Body = new BlockStatement() {
								new ReturnStatement (new InvocationExpression (new IdentifierExpression (RemapMemberName (gs.Getter.Name))))
							}
						};
						if (gs.Setter != null) {
							p.Setter = new Accessor()
							{
								Body = new BlockStatement()
								{
									new InvocationExpression (new IdentifierExpression (gs.Setter.Name), new IdentifierExpression ("value"))
								}
							};
						}
						// We are unable to bind
						if (gs.HostType == null)
							continue;
						gs.HostType.Members.Add(p);
					}
				}
			}
		}
	}

	//
	// Finds a few types that we use later to make decisions, and scans for methods for get/set patterns
	//
	class ScanBaseTypes : AstVisitor
	{
		static public CXXRecordDecl UrhoRefCounted, EventHandlerType;
		public static Dictionary<string, CXXRecordDecl> nameToDecl = new Dictionary<string, CXXRecordDecl>();

		public override void VisitCXXRecordDecl(CXXRecordDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter || !decl.IsCompleteDefinition || decl.Name == null)
				return;

			nameToDecl[decl.QualifiedName] = decl;
			switch (decl.QualifiedName) {
			case "Urho3D::RefCounted":
				UrhoRefCounted = decl;
				break;
			case "Urho3D::EventHandler":
				EventHandlerType = decl;
				break;
			}
		}

		public class GetterSetter
		{
			public CXXMethodDecl Getter, Setter;
			public TypeDeclaration HostType;
			public AstType MethodReturn;
			public string Name;
		}

		// typeName to propertyName to returnType to GetterSetter pairs
		public static Dictionary<string, Dictionary<string, Dictionary<QualType, GetterSetter>>> allProperties =
			new Dictionary<string, Dictionary<string, Dictionary<QualType, GetterSetter>>>();
		public int bad;
		public override void VisitCXXMethodDecl(CXXMethodDecl decl, VisitKind visitKind)
		{
			if (visitKind != VisitKind.Enter)
				return;

			var isConstructor = decl is CXXConstructorDecl;
			if (decl is CXXDestructorDecl || isConstructor)
				return;

			if (decl.IsCopyAssignmentOperator || decl.IsMoveAssignmentOperator)
				return;

			if (decl.Parent == null)
				return;
			if (!decl.Parent.QualifiedName.StartsWith("Urho3D::"))
				return;

			// Only get methods prefixed with Get with no parameters
			// and Set methods that return void and take a single parameter
			var name = decl.Name;

			// Handle Get methods that are not really getters
			// This is a get method that does not get anything
			if (decl.ReturnQualType.ToString() == "void")
				return;


			QualType type;
			if (name.StartsWith("Get")) {
				if (decl.Parameters.Count() != 0)
					return;
				type = decl.ReturnQualType;
			} else if (name.StartsWith("Set")) {
				if (decl.Parameters.Count() != 1)
					return;
				if (!(decl.ReturnQualType.Bind() is Sharpie.Bind.Types.VoidType))
					return;
				type = decl.Parameters.FirstOrDefault().QualType;
			} else
				return;

			Dictionary<string, Dictionary<QualType, GetterSetter>> typeProperties;
			if (!allProperties.TryGetValue(decl.Parent.Name, out typeProperties)) {
				typeProperties = new Dictionary<string, Dictionary<QualType, GetterSetter>>();
				allProperties[decl.Parent.Name] = typeProperties;
			}
			var propName = name.Substring(3);
			Dictionary<QualType, GetterSetter> property;

			if (!typeProperties.TryGetValue(propName, out property)) {
				property = new Dictionary<QualType, GetterSetter>();
				typeProperties[propName] = property;
			}
			GetterSetter gs;
			if (!property.TryGetValue(type, out gs)) {
				gs = new GetterSetter() { Name = propName };
			}

			if (name.StartsWith("Get")) {

				if (gs.Getter != null)
					throw new Exception("Can not happen");
				gs.Getter = decl;
			} else {
				if (gs.Setter != null) {
					throw new Exception("Can not happen");
				}
				gs.Setter = decl;
			}
			property[type] = gs;
		}

		// Contains a list of all methods that will be part of a property
		static Dictionary<CXXMethodDecl, GetterSetter> allPropertyMethods = new Dictionary<CXXMethodDecl, GetterSetter>();

		public static GetterSetter GetPropertyInfo(CXXMethodDecl decl)
		{
			GetterSetter gs;
			if (allPropertyMethods.TryGetValue(decl, out gs))
				return gs;
			return null;
		}

		//
		// After we colleted the information, remove pairs that only had a setter, but no getter
		//
		public void PrepareProperties()
		{
			var typeRemovals = new List<string>();
			foreach (var typeKV in allProperties) {
				var propertyRemovals = new List<string>();
				foreach (var propNameKV in typeKV.Value) {
					var qualTypeRemoval = new List<QualType>();
					foreach (var propTypeKV in propNameKV.Value) {
						if (propTypeKV.Value.Getter == null)
							qualTypeRemoval.Add(propTypeKV.Key);
					}
					foreach (var qualType in qualTypeRemoval)
						propNameKV.Value.Remove(qualType);
					if (propNameKV.Value.Count == 0)
						propertyRemovals.Add(propNameKV.Key);
				}
				foreach (var property in propertyRemovals)
					typeKV.Value.Remove(property);

				if (typeKV.Value.Count == 0)
					typeRemovals.Add(typeKV.Key);
			}
			foreach (var type in typeRemovals)
				allProperties.Remove(type);

			foreach (var typeKV in allProperties) {
				foreach (var propNameKV in typeKV.Value) {
					foreach (var gs in propNameKV.Value.Values) {
						allPropertyMethods[gs.Getter] = gs;
						if (gs.Setter != null)
							allPropertyMethods[gs.Setter] = gs;
					}
				}
			}
		}
	}
}
