// WARNING - AUTOGENERATED - DO NOT EDIT
// 
// Generated using `sharpie urho`
// 
// ConstraintDistance2D.cs
// 
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Urho.Urho2D;
using Urho.Gui;
using Urho.Resources;
using Urho.IO;
using Urho.Navigation;
using Urho.Network;

namespace Urho.Urho2D
{
	/// <summary>
	/// 2D distance constraint component.
	/// </summary>
	public unsafe partial class ConstraintDistance2D : Constraint2D
	{
		public ConstraintDistance2D (IntPtr handle) : base (handle)
		{
		}

		protected ConstraintDistance2D (UrhoObjectFlag emptyFlag) : base (emptyFlag)
		{
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int ConstraintDistance2D_GetType (IntPtr handle);

		private StringHash UrhoGetType ()
		{
			Runtime.ValidateRefCounted (this);
			return new StringHash (ConstraintDistance2D_GetType (handle));
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ConstraintDistance2D_GetTypeName (IntPtr handle);

		private string GetTypeName ()
		{
			Runtime.ValidateRefCounted (this);
			return Marshal.PtrToStringAnsi (ConstraintDistance2D_GetTypeName (handle));
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int ConstraintDistance2D_GetTypeStatic ();

		private static StringHash GetTypeStatic ()
		{
			Runtime.Validate (typeof(ConstraintDistance2D));
			return new StringHash (ConstraintDistance2D_GetTypeStatic ());
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ConstraintDistance2D_GetTypeNameStatic ();

		private static string GetTypeNameStatic ()
		{
			Runtime.Validate (typeof(ConstraintDistance2D));
			return Marshal.PtrToStringAnsi (ConstraintDistance2D_GetTypeNameStatic ());
		}

		public ConstraintDistance2D () : this (Application.CurrentContext)
		{
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ConstraintDistance2D_ConstraintDistance2D (IntPtr context);

		public ConstraintDistance2D (Context context) : base (UrhoObjectFlag.Empty)
		{
			Runtime.Validate (typeof(ConstraintDistance2D));
			handle = ConstraintDistance2D_ConstraintDistance2D ((object)context == null ? IntPtr.Zero : context.Handle);
			Runtime.RegisterObject (this);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ConstraintDistance2D_RegisterObject (IntPtr context);

		/// <summary>
		/// Register object factory.
		/// </summary>
		public new static void RegisterObject (Context context)
		{
			Runtime.Validate (typeof(ConstraintDistance2D));
			ConstraintDistance2D_RegisterObject ((object)context == null ? IntPtr.Zero : context.Handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ConstraintDistance2D_SetOwnerBodyAnchor (IntPtr handle, ref Urho.Vector2 anchor);

		/// <summary>
		/// Set owner body anchor.
		/// </summary>
		private void SetOwnerBodyAnchor (Urho.Vector2 anchor)
		{
			Runtime.ValidateRefCounted (this);
			ConstraintDistance2D_SetOwnerBodyAnchor (handle, ref anchor);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ConstraintDistance2D_SetOtherBodyAnchor (IntPtr handle, ref Urho.Vector2 anchor);

		/// <summary>
		/// Set other body anchor.
		/// </summary>
		private void SetOtherBodyAnchor (Urho.Vector2 anchor)
		{
			Runtime.ValidateRefCounted (this);
			ConstraintDistance2D_SetOtherBodyAnchor (handle, ref anchor);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ConstraintDistance2D_SetFrequencyHz (IntPtr handle, float frequencyHz);

		/// <summary>
		/// Set frequency Hz.
		/// </summary>
		private void SetFrequencyHz (float frequencyHz)
		{
			Runtime.ValidateRefCounted (this);
			ConstraintDistance2D_SetFrequencyHz (handle, frequencyHz);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ConstraintDistance2D_SetDampingRatio (IntPtr handle, float dampingRatio);

		/// <summary>
		/// Set damping ratio.
		/// </summary>
		private void SetDampingRatio (float dampingRatio)
		{
			Runtime.ValidateRefCounted (this);
			ConstraintDistance2D_SetDampingRatio (handle, dampingRatio);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Urho.Vector2 ConstraintDistance2D_GetOwnerBodyAnchor (IntPtr handle);

		/// <summary>
		/// Return owner body anchor.
		/// </summary>
		private Urho.Vector2 GetOwnerBodyAnchor ()
		{
			Runtime.ValidateRefCounted (this);
			return ConstraintDistance2D_GetOwnerBodyAnchor (handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern Urho.Vector2 ConstraintDistance2D_GetOtherBodyAnchor (IntPtr handle);

		/// <summary>
		/// Return other body anchor.
		/// </summary>
		private Urho.Vector2 GetOtherBodyAnchor ()
		{
			Runtime.ValidateRefCounted (this);
			return ConstraintDistance2D_GetOtherBodyAnchor (handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern float ConstraintDistance2D_GetFrequencyHz (IntPtr handle);

		/// <summary>
		/// Return frequency Hz.
		/// </summary>
		private float GetFrequencyHz ()
		{
			Runtime.ValidateRefCounted (this);
			return ConstraintDistance2D_GetFrequencyHz (handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern float ConstraintDistance2D_GetDampingRatio (IntPtr handle);

		/// <summary>
		/// Return damping ratio.
		/// </summary>
		private float GetDampingRatio ()
		{
			Runtime.ValidateRefCounted (this);
			return ConstraintDistance2D_GetDampingRatio (handle);
		}

		public override StringHash Type {
			get {
				return UrhoGetType ();
			}
		}

		public override string TypeName {
			get {
				return GetTypeName ();
			}
		}

		public new static StringHash TypeStatic {
			get {
				return GetTypeStatic ();
			}
		}

		public new static string TypeNameStatic {
			get {
				return GetTypeNameStatic ();
			}
		}

		/// <summary>
		/// Return owner body anchor.
		/// Or
		/// Set owner body anchor.
		/// </summary>
		public Urho.Vector2 OwnerBodyAnchor {
			get {
				return GetOwnerBodyAnchor ();
			}
			set {
				SetOwnerBodyAnchor (value);
			}
		}

		/// <summary>
		/// Return other body anchor.
		/// Or
		/// Set other body anchor.
		/// </summary>
		public Urho.Vector2 OtherBodyAnchor {
			get {
				return GetOtherBodyAnchor ();
			}
			set {
				SetOtherBodyAnchor (value);
			}
		}

		/// <summary>
		/// Return frequency Hz.
		/// Or
		/// Set frequency Hz.
		/// </summary>
		public float FrequencyHz {
			get {
				return GetFrequencyHz ();
			}
			set {
				SetFrequencyHz (value);
			}
		}

		/// <summary>
		/// Return damping ratio.
		/// Or
		/// Set damping ratio.
		/// </summary>
		public float DampingRatio {
			get {
				return GetDampingRatio ();
			}
			set {
				SetDampingRatio (value);
			}
		}
	}
}