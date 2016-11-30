// WARNING - AUTOGENERATED - DO NOT EDIT
// 
// Generated using `sharpie urho`
// 
// FontFaceFreeType.cs
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

namespace Urho.Gui
{
	/// <summary>
	/// Free type font face description.
	/// </summary>
	public unsafe partial class FontFaceFreeType : FontFace
	{
		unsafe partial void OnFontFaceFreeTypeCreated ();

		[Preserve]
		public FontFaceFreeType (IntPtr handle) : base (handle)
		{
			OnFontFaceFreeTypeCreated ();
		}

		[Preserve]
		protected FontFaceFreeType (UrhoObjectFlag emptyFlag) : base (emptyFlag)
		{
			OnFontFaceFreeTypeCreated ();
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr FontFaceFreeType_FontFaceFreeType (IntPtr font);

		public FontFaceFreeType (Font font) : base (UrhoObjectFlag.Empty)
		{
			Runtime.Validate (typeof(FontFaceFreeType));
			handle = FontFaceFreeType_FontFaceFreeType ((object)font == null ? IntPtr.Zero : font.Handle);
			Runtime.RegisterObject (this);
			OnFontFaceFreeTypeCreated ();
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool FontFaceFreeType_Load (IntPtr handle, byte* fontData, uint fontDataSize, int pointSize);

		/// <summary>
		/// Load font face.
		/// </summary>
		public override bool Load (byte* fontData, uint fontDataSize, int pointSize)
		{
			Runtime.ValidateRefCounted (this);
			return FontFaceFreeType_Load (handle, fontData, fontDataSize, pointSize);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern FontGlyph* FontFaceFreeType_GetGlyph (IntPtr handle, uint c);

		/// <summary>
		/// Return pointer to the glyph structure corresponding to a character. Return null if glyph not found.
		/// </summary>
		public override FontGlyph* GetGlyph (uint c)
		{
			Runtime.ValidateRefCounted (this);
			return FontFaceFreeType_GetGlyph (handle, c);
		}

		[DllImport (Consts.NativeImport, CallingConvention = CallingConvention.Cdecl)]
		internal static extern bool FontFaceFreeType_HasMutableGlyphs (IntPtr handle);

		/// <summary>
		/// Return if font face uses mutable glyphs.
		/// </summary>
		public override bool HasMutableGlyphs ()
		{
			Runtime.ValidateRefCounted (this);
			return FontFaceFreeType_HasMutableGlyphs (handle);
		}
	}
}
