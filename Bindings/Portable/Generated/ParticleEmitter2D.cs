// WARNING - AUTOGENERATED - DO NOT EDIT
// 
// Generated using `sharpie urho`
// 
// ParticleEmitter2D.cs
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
	/// 2D particle emitter component.
	/// </summary>
	public unsafe partial class ParticleEmitter2D : Drawable2D
	{
		unsafe partial void OnParticleEmitter2DCreated ();

		[Preserve]
		public ParticleEmitter2D (IntPtr handle) : base (handle)
		{
			OnParticleEmitter2DCreated ();
		}

		[Preserve]
		protected ParticleEmitter2D (UrhoObjectFlag emptyFlag) : base (emptyFlag)
		{
			OnParticleEmitter2DCreated ();
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int ParticleEmitter2D_GetType (IntPtr handle);

		private StringHash UrhoGetType ()
		{
			Runtime.ValidateRefCounted (this);
			return new StringHash (ParticleEmitter2D_GetType (handle));
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ParticleEmitter2D_GetTypeName (IntPtr handle);

		private string GetTypeName ()
		{
			Runtime.ValidateRefCounted (this);
			return Marshal.PtrToStringAnsi (ParticleEmitter2D_GetTypeName (handle));
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern int ParticleEmitter2D_GetTypeStatic ();

		private static StringHash GetTypeStatic ()
		{
			Runtime.Validate (typeof(ParticleEmitter2D));
			return new StringHash (ParticleEmitter2D_GetTypeStatic ());
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ParticleEmitter2D_GetTypeNameStatic ();

		private static string GetTypeNameStatic ()
		{
			Runtime.Validate (typeof(ParticleEmitter2D));
			return Marshal.PtrToStringAnsi (ParticleEmitter2D_GetTypeNameStatic ());
		}

		public ParticleEmitter2D () : this (Application.CurrentContext)
		{
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ParticleEmitter2D_ParticleEmitter2D (IntPtr context);

		public ParticleEmitter2D (Context context) : base (UrhoObjectFlag.Empty)
		{
			Runtime.Validate (typeof(ParticleEmitter2D));
			handle = ParticleEmitter2D_ParticleEmitter2D ((object)context == null ? IntPtr.Zero : context.Handle);
			Runtime.RegisterObject (this);
			OnParticleEmitter2DCreated ();
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ParticleEmitter2D_RegisterObject (IntPtr context);

		/// <summary>
		/// Register object factory. drawable2d must be registered first.
		/// </summary>
		public new static void RegisterObject (Context context)
		{
			Runtime.Validate (typeof(ParticleEmitter2D));
			ParticleEmitter2D_RegisterObject ((object)context == null ? IntPtr.Zero : context.Handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ParticleEmitter2D_OnSetEnabled (IntPtr handle);

		/// <summary>
		/// Handle enabled/disabled state change.
		/// </summary>
		public override void OnSetEnabled ()
		{
			Runtime.ValidateRefCounted (this);
			ParticleEmitter2D_OnSetEnabled (handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ParticleEmitter2D_SetEffect (IntPtr handle, IntPtr effect);

		/// <summary>
		/// Set particle effect.
		/// </summary>
		private void SetEffect (ParticleEffect2D effect)
		{
			Runtime.ValidateRefCounted (this);
			ParticleEmitter2D_SetEffect (handle, (object)effect == null ? IntPtr.Zero : effect.Handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ParticleEmitter2D_SetSprite (IntPtr handle, IntPtr sprite);

		/// <summary>
		/// Set sprite.
		/// </summary>
		private void SetSprite (Sprite2D sprite)
		{
			Runtime.ValidateRefCounted (this);
			ParticleEmitter2D_SetSprite (handle, (object)sprite == null ? IntPtr.Zero : sprite.Handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ParticleEmitter2D_SetBlendMode (IntPtr handle, BlendMode blendMode);

		/// <summary>
		/// Set blend mode.
		/// </summary>
		private void SetBlendMode (BlendMode blendMode)
		{
			Runtime.ValidateRefCounted (this);
			ParticleEmitter2D_SetBlendMode (handle, blendMode);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ParticleEmitter2D_SetMaxParticles (IntPtr handle, uint maxParticles);

		/// <summary>
		/// Set max particles.
		/// </summary>
		private void SetMaxParticles (uint maxParticles)
		{
			Runtime.ValidateRefCounted (this);
			ParticleEmitter2D_SetMaxParticles (handle, maxParticles);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ParticleEmitter2D_GetEffect (IntPtr handle);

		/// <summary>
		/// Return particle effect.
		/// </summary>
		private ParticleEffect2D GetEffect ()
		{
			Runtime.ValidateRefCounted (this);
			return Runtime.LookupObject<ParticleEffect2D> (ParticleEmitter2D_GetEffect (handle));
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ParticleEmitter2D_GetSprite (IntPtr handle);

		/// <summary>
		/// Return sprite.
		/// </summary>
		private Sprite2D GetSprite ()
		{
			Runtime.ValidateRefCounted (this);
			return Runtime.LookupObject<Sprite2D> (ParticleEmitter2D_GetSprite (handle));
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern BlendMode ParticleEmitter2D_GetBlendMode (IntPtr handle);

		/// <summary>
		/// Return blend mode.
		/// </summary>
		private BlendMode GetBlendMode ()
		{
			Runtime.ValidateRefCounted (this);
			return ParticleEmitter2D_GetBlendMode (handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint ParticleEmitter2D_GetMaxParticles (IntPtr handle);

		/// <summary>
		/// Return max particles.
		/// </summary>
		private uint GetMaxParticles ()
		{
			Runtime.ValidateRefCounted (this);
			return ParticleEmitter2D_GetMaxParticles (handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern ResourceRef ParticleEmitter2D_GetParticleEffectAttr (IntPtr handle);

		/// <summary>
		/// Return particle model attr.
		/// </summary>
		private ResourceRef GetParticleEffectAttr ()
		{
			Runtime.ValidateRefCounted (this);
			return ParticleEmitter2D_GetParticleEffectAttr (handle);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern ResourceRef ParticleEmitter2D_GetSpriteAttr (IntPtr handle);

		/// <summary>
		/// Return sprite attribute.
		/// </summary>
		private ResourceRef GetSpriteAttr ()
		{
			Runtime.ValidateRefCounted (this);
			return ParticleEmitter2D_GetSpriteAttr (handle);
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
		/// Return particle effect.
		/// Or
		/// Set particle effect.
		/// </summary>
		public ParticleEffect2D Effect {
			get {
				return GetEffect ();
			}
			set {
				SetEffect (value);
			}
		}

		/// <summary>
		/// Return sprite.
		/// Or
		/// Set sprite.
		/// </summary>
		public Sprite2D Sprite {
			get {
				return GetSprite ();
			}
			set {
				SetSprite (value);
			}
		}

		/// <summary>
		/// Return blend mode.
		/// Or
		/// Set blend mode.
		/// </summary>
		public BlendMode BlendMode {
			get {
				return GetBlendMode ();
			}
			set {
				SetBlendMode (value);
			}
		}

		/// <summary>
		/// Return max particles.
		/// Or
		/// Set max particles.
		/// </summary>
		public uint MaxParticles {
			get {
				return GetMaxParticles ();
			}
			set {
				SetMaxParticles (value);
			}
		}

		/// <summary>
		/// Return particle model attr.
		/// </summary>
		public ResourceRef ParticleEffectAttr {
			get {
				return GetParticleEffectAttr ();
			}
		}

		/// <summary>
		/// Return sprite attribute.
		/// </summary>
		public ResourceRef SpriteAttr {
			get {
				return GetSpriteAttr ();
			}
		}
	}
}
