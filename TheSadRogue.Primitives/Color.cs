// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace SadRogue.Primitives
{
    /// <summary>
    /// Describes a 32-bit packed color.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public readonly struct Color : IEquatable<Color>, IMatchable<Color>
    {
        static Color()
        {
            TransparentBlack = new Color(0);
            Transparent = new Color(0);
            AliceBlue = new Color(0xfffff8f0);
            AntiqueWhite = new Color(0xffd7ebfa);
            Aqua = new Color(0xffffff00);
            Aquamarine = new Color(0xffd4ff7f);
            Azure = new Color(0xfffffff0);
            Beige = new Color(0xffdcf5f5);
            Bisque = new Color(0xffc4e4ff);
            Black = new Color(0xff000000);
            BlanchedAlmond = new Color(0xffcdebff);
            Blue = new Color(0xffff0000);
            BlueViolet = new Color(0xffe22b8a);
            Brown = new Color(0xff2a2aa5);
            BurlyWood = new Color(0xff87b8de);
            CadetBlue = new Color(0xffa09e5f);
            Chartreuse = new Color(0xff00ff7f);
            Chocolate = new Color(0xff1e69d2);
            Coral = new Color(0xff507fff);
            CornflowerBlue = new Color(0xffed9564);
            Cornsilk = new Color(0xffdcf8ff);
            Crimson = new Color(0xff3c14dc);
            Cyan = new Color(0xffffff00);
            DarkBlue = new Color(0xff8b0000);
            DarkCyan = new Color(0xff8b8b00);
            DarkGoldenrod = new Color(0xff0b86b8);
            DarkGray = new Color(0xffa9a9a9);
            DarkGreen = new Color(0xff006400);
            DarkKhaki = new Color(0xff6bb7bd);
            DarkMagenta = new Color(0xff8b008b);
            DarkOliveGreen = new Color(0xff2f6b55);
            DarkOrange = new Color(0xff008cff);
            DarkOrchid = new Color(0xffcc3299);
            DarkRed = new Color(0xff00008b);
            DarkSalmon = new Color(0xff7a96e9);
            DarkSeaGreen = new Color(0xff8bbc8f);
            DarkSlateBlue = new Color(0xff8b3d48);
            DarkSlateGray = new Color(0xff4f4f2f);
            DarkTurquoise = new Color(0xffd1ce00);
            DarkViolet = new Color(0xffd30094);
            DeepPink = new Color(0xff9314ff);
            DeepSkyBlue = new Color(0xffffbf00);
            DimGray = new Color(0xff696969);
            DodgerBlue = new Color(0xffff901e);
            Firebrick = new Color(0xff2222b2);
            FloralWhite = new Color(0xfff0faff);
            ForestGreen = new Color(0xff228b22);
            Fuchsia = new Color(0xffff00ff);
            Gainsboro = new Color(0xffdcdcdc);
            GhostWhite = new Color(0xfffff8f8);
            Gold = new Color(0xff00d7ff);
            Goldenrod = new Color(0xff20a5da);
            Gray = new Color(0xff808080);
            Green = new Color(0xff008000);
            GreenYellow = new Color(0xff2fffad);
            Honeydew = new Color(0xfff0fff0);
            HotPink = new Color(0xffb469ff);
            IndianRed = new Color(0xff5c5ccd);
            Indigo = new Color(0xff82004b);
            Ivory = new Color(0xfff0ffff);
            Khaki = new Color(0xff8ce6f0);
            Lavender = new Color(0xfffae6e6);
            LavenderBlush = new Color(0xfff5f0ff);
            LawnGreen = new Color(0xff00fc7c);
            LemonChiffon = new Color(0xffcdfaff);
            LightBlue = new Color(0xffe6d8ad);
            LightCoral = new Color(0xff8080f0);
            LightCyan = new Color(0xffffffe0);
            LightGoldenrodYellow = new Color(0xffd2fafa);
            LightGray = new Color(0xffd3d3d3);
            LightGreen = new Color(0xff90ee90);
            LightPink = new Color(0xffc1b6ff);
            LightSalmon = new Color(0xff7aa0ff);
            LightSeaGreen = new Color(0xffaab220);
            LightSkyBlue = new Color(0xffface87);
            LightSlateGray = new Color(0xff998877);
            LightSteelBlue = new Color(0xffdec4b0);
            LightYellow = new Color(0xffe0ffff);
            Lime = new Color(0xff00ff00);
            LimeGreen = new Color(0xff32cd32);
            Linen = new Color(0xffe6f0fa);
            Magenta = new Color(0xffff00ff);
            Maroon = new Color(0xff000080);
            MediumAquamarine = new Color(0xffaacd66);
            MediumBlue = new Color(0xffcd0000);
            MediumOrchid = new Color(0xffd355ba);
            MediumPurple = new Color(0xffdb7093);
            MediumSeaGreen = new Color(0xff71b33c);
            MediumSlateBlue = new Color(0xffee687b);
            MediumSpringGreen = new Color(0xff9afa00);
            MediumTurquoise = new Color(0xffccd148);
            MediumVioletRed = new Color(0xff8515c7);
            MidnightBlue = new Color(0xff701919);
            MintCream = new Color(0xfffafff5);
            MistyRose = new Color(0xffe1e4ff);
            Moccasin = new Color(0xffb5e4ff);
            MonoGameOrange = new Color(0xff003ce7);
            NavajoWhite = new Color(0xffaddeff);
            Navy = new Color(0xff800000);
            OldLace = new Color(0xffe6f5fd);
            Olive = new Color(0xff008080);
            OliveDrab = new Color(0xff238e6b);
            Orange = new Color(0xff00a5ff);
            OrangeRed = new Color(0xff0045ff);
            Orchid = new Color(0xffd670da);
            PaleGoldenrod = new Color(0xffaae8ee);
            PaleGreen = new Color(0xff98fb98);
            PaleTurquoise = new Color(0xffeeeeaf);
            PaleVioletRed = new Color(0xff9370db);
            PapayaWhip = new Color(0xffd5efff);
            PeachPuff = new Color(0xffb9daff);
            Peru = new Color(0xff3f85cd);
            Pink = new Color(0xffcbc0ff);
            Plum = new Color(0xffdda0dd);
            PowderBlue = new Color(0xffe6e0b0);
            Purple = new Color(0xff800080);
            Red = new Color(0xff0000ff);
            RosyBrown = new Color(0xff8f8fbc);
            RoyalBlue = new Color(0xffe16941);
            SaddleBrown = new Color(0xff13458b);
            Salmon = new Color(0xff7280fa);
            SandyBrown = new Color(0xff60a4f4);
            SeaGreen = new Color(0xff578b2e);
            SeaShell = new Color(0xffeef5ff);
            Sienna = new Color(0xff2d52a0);
            Silver = new Color(0xffc0c0c0);
            SkyBlue = new Color(0xffebce87);
            SlateBlue = new Color(0xffcd5a6a);
            SlateGray = new Color(0xff908070);
            Snow = new Color(0xfffafaff);
            SpringGreen = new Color(0xff7fff00);
            SteelBlue = new Color(0xffb48246);
            Tan = new Color(0xff8cb4d2);
            Teal = new Color(0xff808000);
            Thistle = new Color(0xffd8bfd8);
            Tomato = new Color(0xff4763ff);
            Turquoise = new Color(0xffd0e040);
            Violet = new Color(0xffee82ee);
            Wheat = new Color(0xffb3def5);
            White = new Color(uint.MaxValue);
            WhiteSmoke = new Color(0xfff5f5f5);
            Yellow = new Color(0xff00ffff);
            YellowGreen = new Color(0xff32cd9a);
        }

        // Stored as RGBA with R in the least significant octet:
        // |-------|-------|-------|-------
        // A       B       G       R
        [DataMember] private readonly uint _packedValue;

        /// <summary>
        /// Constructs an RGBA color from a packed value.
        /// The value is a 32-bit unsigned integer, with R in the least significant octet.
        /// </summary>
        /// <param name="packedValue">The packed value.</param>
        public Color(uint packedValue) => _packedValue = packedValue;

        /// <summary>
        /// Constructs an RGBA color from a <see cref="Color"/> and an alpha value.
        /// </summary>
        /// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
        /// <param name="alpha">The alpha component value from 0 to 255.</param>
        public Color(Color color, int alpha)
        {
            if ((alpha & 0xFFFFFF00) != 0)
            {
                uint clampedA = (uint)MathHelpers.Clamp(alpha, byte.MinValue, byte.MaxValue);

                _packedValue = (color._packedValue & 0x00FFFFFF) | (clampedA << 24);
            }
            else
                _packedValue = (color._packedValue & 0x00FFFFFF) | ((uint)alpha << 24);
        }

        /// <summary>
        /// Constructs an RGBA color from color and alpha value.
        /// </summary>
        /// <param name="color">A <see cref="Color"/> for RGB values of new <see cref="Color"/> instance.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public Color(Color color, float alpha)
            :
            this(color, (int)(alpha * 255))
        { }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        public Color(float r, float g, float b)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255))
        { }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0.0f to 1.0f.</param>
        /// <param name="g">Green component value from 0.0f to 1.0f.</param>
        /// <param name="b">Blue component value from 0.0f to 1.0f.</param>
        /// <param name="alpha">Alpha component value from 0.0f to 1.0f.</param>
        public Color(float r, float g, float b, float alpha)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255))
        { }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green and blue values. Alpha value will be opaque.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        public Color(int r, int g, int b)
        {
            _packedValue = 0xFF000000; // A = 255

            if (((r | g | b) & 0xFFFFFF00) != 0)
            {
                uint clampedR = (uint)MathHelpers.Clamp(r, byte.MinValue, byte.MaxValue);
                uint clampedG = (uint)MathHelpers.Clamp(g, byte.MinValue, byte.MaxValue);
                uint clampedB = (uint)MathHelpers.Clamp(b, byte.MinValue, byte.MaxValue);

                _packedValue |= (clampedB << 16) | (clampedG << 8) | clampedR;
            }
            else
                _packedValue |= ((uint)b << 16) | ((uint)g << 8) | (uint)r;
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <param name="r">Red component value from 0 to 255.</param>
        /// <param name="g">Green component value from 0 to 255.</param>
        /// <param name="b">Blue component value from 0 to 255.</param>
        /// <param name="alpha">Alpha component value from 0 to 255.</param>
        public Color(int r, int g, int b, int alpha)
        {
            if (((r | g | b | alpha) & 0xFFFFFF00) != 0)
            {
                uint clampedR = (uint)MathHelpers.Clamp(r, byte.MinValue, byte.MaxValue);
                uint clampedG = (uint)MathHelpers.Clamp(g, byte.MinValue, byte.MaxValue);
                uint clampedB = (uint)MathHelpers.Clamp(b, byte.MinValue, byte.MaxValue);
                uint clampedA = (uint)MathHelpers.Clamp(alpha, byte.MinValue, byte.MaxValue);

                _packedValue = (clampedA << 24) | (clampedB << 16) | (clampedG << 8) | clampedR;
            }
            else
                _packedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | (uint)r;
        }

        /// <summary>
        /// Constructs an RGBA color from scalars representing red, green, blue and alpha values.
        /// </summary>
        /// <remarks>
        /// This overload sets the values directly without clamping, and may therefore be faster than the other overloads.
        /// </remarks>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="alpha"></param>
        public Color(byte r, byte g, byte b, byte alpha)
            => _packedValue = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | r;

        /// <summary>
        /// Gets or sets the blue component.
        /// </summary>
        public byte B
        {
            get
            {
                unchecked
                {
                    return (byte)(_packedValue >> 16);
                }
            }
        }

        /// <summary>
        /// Gets or sets the green component.
        /// </summary>
        public byte G
        {
            get
            {
                unchecked
                {
                    return (byte)(_packedValue >> 8);
                }
            }
        }

        /// <summary>
        /// Gets or sets the red component.
        /// </summary>
        public byte R
        {
            get
            {
                unchecked
                {
                    return (byte)_packedValue;
                }
            }
        }

        /// <summary>
        /// Gets or sets the alpha component.
        /// </summary>
        public byte A
        {
            get
            {
                unchecked
                {
                    return (byte)(_packedValue >> 24);
                }
            }
        }

        /// <summary>
        /// Compares whether two <see cref="Color"/> instances are equal.
        /// </summary>
        /// <param name="a"><see cref="Color"/> instance on the left of the equal sign.</param>
        /// <param name="b"><see cref="Color"/> instance on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        [Pure]
        public static bool operator ==(Color a, Color b) => a._packedValue == b._packedValue;

        /// <summary>
        /// Compares whether two <see cref="Color"/> instances are not equal.
        /// </summary>
        /// <param name="a"><see cref="Color"/> instance on the left of the not equal sign.</param>
        /// <param name="b"><see cref="Color"/> instance on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
        [Pure]
        public static bool operator !=(Color a, Color b) => a._packedValue != b._packedValue;

        /// <summary>
        /// Gets the hash code of this <see cref="Color"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="Color"/>.</returns>
        [Pure]
        public override int GetHashCode() => _packedValue.GetHashCode();

        /// <summary>
        /// Compares whether current instance is equal to specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Color"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        [Pure]
        public override bool Equals(object? obj) => obj is Color color && Equals(color);

        #region Color Bank Ansi

        /// <summary>
        /// The black ansi color (0, 0, 0).
        /// </summary>
        public static readonly Color AnsiBlack = new Color(0, 0, 0);

        /// <summary>
        /// The Red ansi color (170, 0, 0).
        /// </summary>
        public static readonly Color AnsiRed = new Color(170, 0, 0);

        /// <summary>
        /// The Green ansi color 0, 170, 0).
        /// </summary>
        public static readonly Color AnsiGreen = new Color(0, 170, 0);

        /// <summary>
        /// The Yellow ansi color (170, 85, 0).
        /// </summary>
        public static readonly Color AnsiYellow = new Color(170, 85, 0);

        /// <summary>
        /// The Blue ansi color (0, 0, 170).
        /// </summary>
        public static readonly Color AnsiBlue = new Color(0, 0, 170);

        /// <summary>
        /// The Magenta ansi color (170, 0, 170).
        /// </summary>
        public static readonly Color AnsiMagenta = new Color(170, 0, 170);

        /// <summary>
        /// The Cyan ansi color (0, 170, 170).
        /// </summary>
        public static readonly Color AnsiCyan = new Color(0, 170, 170);

        /// <summary>
        /// The White ansi color (170, 170, 170).
        /// </summary>
        public static readonly Color AnsiWhite = new Color(170, 170, 170);

        /// <summary>
        /// The BlackBright ansi color (85, 85, 85).
        /// </summary>
        public static readonly Color AnsiBlackBright = new Color(85, 85, 85);

        /// <summary>
        /// The RedBright ansi color (255, 85, 85).
        /// </summary>
        public static readonly Color AnsiRedBright = new Color(255, 85, 85);

        /// <summary>
        /// The GreenBright ansi color (85, 255, 85).
        /// </summary>
        public static readonly Color AnsiGreenBright = new Color(85, 255, 85);

        /// <summary>
        /// The YellowBright ansi color (255, 255, 85).
        /// </summary>
        public static readonly Color AnsiYellowBright = new Color(255, 255, 85);

        /// <summary>
        /// The BlueBright ansi color (85, 85, 255).
        /// </summary>
        public static readonly Color AnsiBlueBright = new Color(85, 85, 255);

        /// <summary>
        /// The MagentaBright ansi color (255, 85, 255).
        /// </summary>
        public static readonly Color AnsiMagentaBright = new Color(255, 85, 255);

        /// <summary>
        /// The CyanBright ansi color (85, 255, 255).
        /// </summary>
        public static readonly Color AnsiCyanBright = new Color(85, 255, 255);

        /// <summary>
        /// The WhiteBright ansi color (255, 255, 255).
        /// </summary>
        public static readonly Color AnsiWhiteBright = new Color(255, 255, 255);

        #endregion

        #region Color Bank

        /// <summary>
        /// TransparentBlack color (R:0,G:0,B:0,A:0).
        /// </summary>
        public static readonly Color TransparentBlack;

        /// <summary>
        /// Transparent color (R:0,G:0,B:0,A:0).
        /// </summary>
        public static readonly Color Transparent;

        /// <summary>
        /// AliceBlue color (R:240,G:248,B:255,A:255).
        /// </summary>
        public static readonly Color AliceBlue;

        /// <summary>
        /// AntiqueWhite color (R:250,G:235,B:215,A:255).
        /// </summary>
        public static readonly Color AntiqueWhite;

        /// <summary>
        /// Aqua color (R:0,G:255,B:255,A:255).
        /// </summary>
        public static readonly Color Aqua;

        /// <summary>
        /// Aquamarine color (R:127,G:255,B:212,A:255).
        /// </summary>
        public static readonly Color Aquamarine;

        /// <summary>
        /// Azure color (R:240,G:255,B:255,A:255).
        /// </summary>
        public static readonly Color Azure;

        /// <summary>
        /// Beige color (R:245,G:245,B:220,A:255).
        /// </summary>
        public static readonly Color Beige;

        /// <summary>
        /// Bisque color (R:255,G:228,B:196,A:255).
        /// </summary>
        public static readonly Color Bisque;

        /// <summary>
        /// Black color (R:0,G:0,B:0,A:255).
        /// </summary>
        public static readonly Color Black;

        /// <summary>
        /// BlanchedAlmond color (R:255,G:235,B:205,A:255).
        /// </summary>
        public static readonly Color BlanchedAlmond;

        /// <summary>
        /// Blue color (R:0,G:0,B:255,A:255).
        /// </summary>
        public static readonly Color Blue;

        /// <summary>
        /// BlueViolet color (R:138,G:43,B:226,A:255).
        /// </summary>
        public static readonly Color BlueViolet;

        /// <summary>
        /// Brown color (R:165,G:42,B:42,A:255).
        /// </summary>
        public static readonly Color Brown;

        /// <summary>
        /// BurlyWood color (R:222,G:184,B:135,A:255).
        /// </summary>
        public static readonly Color BurlyWood;

        /// <summary>
        /// CadetBlue color (R:95,G:158,B:160,A:255).
        /// </summary>
        public static readonly Color CadetBlue;

        /// <summary>
        /// Chartreuse color (R:127,G:255,B:0,A:255).
        /// </summary>
        public static readonly Color Chartreuse;

        /// <summary>
        /// Chocolate color (R:210,G:105,B:30,A:255).
        /// </summary>
        public static readonly Color Chocolate;

        /// <summary>
        /// Coral color (R:255,G:127,B:80,A:255).
        /// </summary>
        public static readonly Color Coral;

        /// <summary>
        /// CornflowerBlue color (R:100,G:149,B:237,A:255).
        /// </summary>
        public static readonly Color CornflowerBlue;

        /// <summary>
        /// Cornsilk color (R:255,G:248,B:220,A:255).
        /// </summary>
        public static readonly Color Cornsilk;

        /// <summary>
        /// Crimson color (R:220,G:20,B:60,A:255).
        /// </summary>
        public static readonly Color Crimson;

        /// <summary>
        /// Cyan color (R:0,G:255,B:255,A:255).
        /// </summary>
        public static readonly Color Cyan;

        /// <summary>
        /// DarkBlue color (R:0,G:0,B:139,A:255).
        /// </summary>
        public static readonly Color DarkBlue;

        /// <summary>
        /// DarkCyan color (R:0,G:139,B:139,A:255).
        /// </summary>
        public static readonly Color DarkCyan;

        /// <summary>
        /// DarkGoldenrod color (R:184,G:134,B:11,A:255).
        /// </summary>
        public static readonly Color DarkGoldenrod;

        /// <summary>
        /// DarkGray color (R:169,G:169,B:169,A:255).
        /// </summary>
        public static readonly Color DarkGray;

        /// <summary>
        /// DarkGreen color (R:0,G:100,B:0,A:255).
        /// </summary>
        public static readonly Color DarkGreen;

        /// <summary>
        /// DarkKhaki color (R:189,G:183,B:107,A:255).
        /// </summary>
        public static readonly Color DarkKhaki;

        /// <summary>
        /// DarkMagenta color (R:139,G:0,B:139,A:255).
        /// </summary>
        public static readonly Color DarkMagenta;

        /// <summary>
        /// DarkOliveGreen color (R:85,G:107,B:47,A:255).
        /// </summary>
        public static readonly Color DarkOliveGreen;

        /// <summary>
        /// DarkOrange color (R:255,G:140,B:0,A:255).
        /// </summary>
        public static readonly Color DarkOrange;

        /// <summary>
        /// DarkOrchid color (R:153,G:50,B:204,A:255).
        /// </summary>
        public static readonly Color DarkOrchid;

        /// <summary>
        /// DarkRed color (R:139,G:0,B:0,A:255).
        /// </summary>
        public static readonly Color DarkRed;

        /// <summary>
        /// DarkSalmon color (R:233,G:150,B:122,A:255).
        /// </summary>
        public static readonly Color DarkSalmon;

        /// <summary>
        /// DarkSeaGreen color (R:143,G:188,B:139,A:255).
        /// </summary>
        public static readonly Color DarkSeaGreen;

        /// <summary>
        /// DarkSlateBlue color (R:72,G:61,B:139,A:255).
        /// </summary>
        public static readonly Color DarkSlateBlue;

        /// <summary>
        /// DarkSlateGray color (R:47,G:79,B:79,A:255).
        /// </summary>
        public static readonly Color DarkSlateGray;

        /// <summary>
        /// DarkTurquoise color (R:0,G:206,B:209,A:255).
        /// </summary>
        public static readonly Color DarkTurquoise;

        /// <summary>
        /// DarkViolet color (R:148,G:0,B:211,A:255).
        /// </summary>
        public static readonly Color DarkViolet;

        /// <summary>
        /// DeepPink color (R:255,G:20,B:147,A:255).
        /// </summary>
        public static readonly Color DeepPink;

        /// <summary>
        /// DeepSkyBlue color (R:0,G:191,B:255,A:255).
        /// </summary>
        public static readonly Color DeepSkyBlue;

        /// <summary>
        /// DimGray color (R:105,G:105,B:105,A:255).
        /// </summary>
        public static readonly Color DimGray;

        /// <summary>
        /// DodgerBlue color (R:30,G:144,B:255,A:255).
        /// </summary>
        public static readonly Color DodgerBlue;

        /// <summary>
        /// Firebrick color (R:178,G:34,B:34,A:255).
        /// </summary>
        public static readonly Color Firebrick;

        /// <summary>
        /// FloralWhite color (R:255,G:250,B:240,A:255).
        /// </summary>
        public static readonly Color FloralWhite;

        /// <summary>
        /// ForestGreen color (R:34,G:139,B:34,A:255).
        /// </summary>
        public static readonly Color ForestGreen;

        /// <summary>
        /// Fuchsia color (R:255,G:0,B:255,A:255).
        /// </summary>
        public static readonly Color Fuchsia;

        /// <summary>
        /// Gainsboro color (R:220,G:220,B:220,A:255).
        /// </summary>
        public static readonly Color Gainsboro;

        /// <summary>
        /// GhostWhite color (R:248,G:248,B:255,A:255).
        /// </summary>
        public static readonly Color GhostWhite;

        /// <summary>
        /// Gold color (R:255,G:215,B:0,A:255).
        /// </summary>
        public static readonly Color Gold;

        /// <summary>
        /// Goldenrod color (R:218,G:165,B:32,A:255).
        /// </summary>
        public static readonly Color Goldenrod;

        /// <summary>
        /// Gray color (R:128,G:128,B:128,A:255).
        /// </summary>
        public static readonly Color Gray;

        /// <summary>
        /// Green color (R:0,G:128,B:0,A:255).
        /// </summary>
        public static readonly Color Green;

        /// <summary>
        /// GreenYellow color (R:173,G:255,B:47,A:255).
        /// </summary>
        public static readonly Color GreenYellow;

        /// <summary>
        /// Honeydew color (R:240,G:255,B:240,A:255).
        /// </summary>
        public static readonly Color Honeydew;

        /// <summary>
        /// HotPink color (R:255,G:105,B:180,A:255).
        /// </summary>
        public static readonly Color HotPink;

        /// <summary>
        /// IndianRed color (R:205,G:92,B:92,A:255).
        /// </summary>
        public static readonly Color IndianRed;

        /// <summary>
        /// Indigo color (R:75,G:0,B:130,A:255).
        /// </summary>
        public static readonly Color Indigo;

        /// <summary>
        /// Ivory color (R:255,G:255,B:240,A:255).
        /// </summary>
        public static readonly Color Ivory;

        /// <summary>
        /// Khaki color (R:240,G:230,B:140,A:255).
        /// </summary>
        public static readonly Color Khaki;

        /// <summary>
        /// Lavender color (R:230,G:230,B:250,A:255).
        /// </summary>
        public static readonly Color Lavender;

        /// <summary>
        /// LavenderBlush color (R:255,G:240,B:245,A:255).
        /// </summary>
        public static readonly Color LavenderBlush;

        /// <summary>
        /// LawnGreen color (R:124,G:252,B:0,A:255).
        /// </summary>
        public static readonly Color LawnGreen;

        /// <summary>
        /// LemonChiffon color (R:255,G:250,B:205,A:255).
        /// </summary>
        public static readonly Color LemonChiffon;

        /// <summary>
        /// LightBlue color (R:173,G:216,B:230,A:255).
        /// </summary>
        public static readonly Color LightBlue;

        /// <summary>
        /// LightCoral color (R:240,G:128,B:128,A:255).
        /// </summary>
        public static readonly Color LightCoral;

        /// <summary>
        /// LightCyan color (R:224,G:255,B:255,A:255).
        /// </summary>
        public static readonly Color LightCyan;

        /// <summary>
        /// LightGoldenrodYellow color (R:250,G:250,B:210,A:255).
        /// </summary>
        public static readonly Color LightGoldenrodYellow;

        /// <summary>
        /// LightGray color (R:211,G:211,B:211,A:255).
        /// </summary>
        public static readonly Color LightGray;

        /// <summary>
        /// LightGreen color (R:144,G:238,B:144,A:255).
        /// </summary>
        public static readonly Color LightGreen;

        /// <summary>
        /// LightPink color (R:255,G:182,B:193,A:255).
        /// </summary>
        public static readonly Color LightPink;

        /// <summary>
        /// LightSalmon color (R:255,G:160,B:122,A:255).
        /// </summary>
        public static readonly Color LightSalmon;

        /// <summary>
        /// LightSeaGreen color (R:32,G:178,B:170,A:255).
        /// </summary>
        public static readonly Color LightSeaGreen;

        /// <summary>
        /// LightSkyBlue color (R:135,G:206,B:250,A:255).
        /// </summary>
        public static readonly Color LightSkyBlue;

        /// <summary>
        /// LightSlateGray color (R:119,G:136,B:153,A:255).
        /// </summary>
        public static readonly Color LightSlateGray;

        /// <summary>
        /// LightSteelBlue color (R:176,G:196,B:222,A:255).
        /// </summary>
        public static readonly Color LightSteelBlue;

        /// <summary>
        /// LightYellow color (R:255,G:255,B:224,A:255).
        /// </summary>
        public static readonly Color LightYellow;

        /// <summary>
        /// Lime color (R:0,G:255,B:0,A:255).
        /// </summary>
        public static readonly Color Lime;

        /// <summary>
        /// LimeGreen color (R:50,G:205,B:50,A:255).
        /// </summary>
        public static readonly Color LimeGreen;

        /// <summary>
        /// Linen color (R:250,G:240,B:230,A:255).
        /// </summary>
        public static readonly Color Linen;

        /// <summary>
        /// Magenta color (R:255,G:0,B:255,A:255).
        /// </summary>
        public static readonly Color Magenta;

        /// <summary>
        /// Maroon color (R:128,G:0,B:0,A:255).
        /// </summary>
        public static readonly Color Maroon;

        /// <summary>
        /// MediumAquamarine color (R:102,G:205,B:170,A:255).
        /// </summary>
        public static readonly Color MediumAquamarine;


        /// <summary>
        /// MediumBlue color (R:0,G:0,B:205,A:255).
        /// </summary>
        public static readonly Color MediumBlue;

        /// <summary>
        /// MediumOrchid color (R:186,G:85,B:211,A:255).
        /// </summary>
        public static readonly Color MediumOrchid;

        /// <summary>
        /// MediumPurple color (R:147,G:112,B:219,A:255).
        /// </summary>
        public static readonly Color MediumPurple;

        /// <summary>
        /// MediumSeaGreen color (R:60,G:179,B:113,A:255).
        /// </summary>
        public static readonly Color MediumSeaGreen;

        /// <summary>
        /// MediumSlateBlue color (R:123,G:104,B:238,A:255).
        /// </summary>
        public static readonly Color MediumSlateBlue;

        /// <summary>
        /// MediumSpringGreen color (R:0,G:250,B:154,A:255).
        /// </summary>
        public static readonly Color MediumSpringGreen;

        /// <summary>
        /// MediumTurquoise color (R:72,G:209,B:204,A:255).
        /// </summary>
        public static readonly Color MediumTurquoise;

        /// <summary>
        /// MediumVioletRed color (R:199,G:21,B:133,A:255).
        /// </summary>
        public static readonly Color MediumVioletRed;

        /// <summary>
        /// MidnightBlue color (R:25,G:25,B:112,A:255).
        /// </summary>
        public static readonly Color MidnightBlue;

        /// <summary>
        /// MintCream color (R:245,G:255,B:250,A:255).
        /// </summary>
        public static readonly Color MintCream;

        /// <summary>
        /// MistyRose color (R:255,G:228,B:225,A:255).
        /// </summary>
        public static readonly Color MistyRose;

        /// <summary>
        /// Moccasin color (R:255,G:228,B:181,A:255).
        /// </summary>
        public static readonly Color Moccasin;

        /// <summary>
        /// MonoGame orange theme color (R:231,G:60,B:0,A:255).
        /// </summary>
        public static readonly Color MonoGameOrange;

        /// <summary>
        /// NavajoWhite color (R:255,G:222,B:173,A:255).
        /// </summary>
        public static readonly Color NavajoWhite;

        /// <summary>
        /// Navy color (R:0,G:0,B:128,A:255).
        /// </summary>
        public static readonly Color Navy;

        /// <summary>
        /// OldLace color (R:253,G:245,B:230,A:255).
        /// </summary>
        public static readonly Color OldLace;

        /// <summary>
        /// Olive color (R:128,G:128,B:0,A:255).
        /// </summary>
        public static readonly Color Olive;

        /// <summary>
        /// OliveDrab color (R:107,G:142,B:35,A:255).
        /// </summary>
        public static readonly Color OliveDrab;

        /// <summary>
        /// Orange color (R:255,G:165,B:0,A:255).
        /// </summary>
        public static readonly Color Orange;

        /// <summary>
        /// OrangeRed color (R:255,G:69,B:0,A:255).
        /// </summary>
        public static readonly Color OrangeRed;

        /// <summary>
        /// Orchid color (R:218,G:112,B:214,A:255).
        /// </summary>
        public static readonly Color Orchid;

        /// <summary>
        /// PaleGoldenrod color (R:238,G:232,B:170,A:255).
        /// </summary>
        public static readonly Color PaleGoldenrod;

        /// <summary>
        /// PaleGreen color (R:152,G:251,B:152,A:255).
        /// </summary>
        public static readonly Color PaleGreen;

        /// <summary>
        /// PaleTurquoise color (R:175,G:238,B:238,A:255).
        /// </summary>
        public static readonly Color PaleTurquoise;

        /// <summary>
        /// PaleVioletRed color (R:219,G:112,B:147,A:255).
        /// </summary>
        public static readonly Color PaleVioletRed;

        /// <summary>
        /// PapayaWhip color (R:255,G:239,B:213,A:255).
        /// </summary>
        public static readonly Color PapayaWhip;

        /// <summary>
        /// PeachPuff color (R:255,G:218,B:185,A:255).
        /// </summary>
        public static readonly Color PeachPuff;

        /// <summary>
        /// Peru color (R:205,G:133,B:63,A:255).
        /// </summary>
        public static readonly Color Peru;

        /// <summary>
        /// Pink color (R:255,G:192,B:203,A:255).
        /// </summary>
        public static readonly Color Pink;

        /// <summary>
        /// Plum color (R:221,G:160,B:221,A:255).
        /// </summary>
        public static readonly Color Plum;

        /// <summary>
        /// PowderBlue color (R:176,G:224,B:230,A:255).
        /// </summary>
        public static readonly Color PowderBlue;

        /// <summary>
        ///  Purple color (R:128,G:0,B:128,A:255).
        /// </summary>
        public static readonly Color Purple;

        /// <summary>
        /// Red color (R:255,G:0,B:0,A:255).
        /// </summary>
        public static readonly Color Red;

        /// <summary>
        /// RosyBrown color (R:188,G:143,B:143,A:255).
        /// </summary>
        public static readonly Color RosyBrown;

        /// <summary>
        /// RoyalBlue color (R:65,G:105,B:225,A:255).
        /// </summary>
        public static readonly Color RoyalBlue;

        /// <summary>
        /// SaddleBrown color (R:139,G:69,B:19,A:255).
        /// </summary>
        public static readonly Color SaddleBrown;

        /// <summary>
        /// Salmon color (R:250,G:128,B:114,A:255).
        /// </summary>
        public static readonly Color Salmon;

        /// <summary>
        /// SandyBrown color (R:244,G:164,B:96,A:255).
        /// </summary>
        public static readonly Color SandyBrown;

        /// <summary>
        /// SeaGreen color (R:46,G:139,B:87,A:255).
        /// </summary>
        public static readonly Color SeaGreen;

        /// <summary>
        /// SeaShell color (R:255,G:245,B:238,A:255).
        /// </summary>
        public static readonly Color SeaShell;

        /// <summary>
        /// Sienna color (R:160,G:82,B:45,A:255).
        /// </summary>
        public static readonly Color Sienna;

        /// <summary>
        /// Silver color (R:192,G:192,B:192,A:255).
        /// </summary>
        public static readonly Color Silver;

        /// <summary>
        /// SkyBlue color (R:135,G:206,B:235,A:255).
        /// </summary>
        public static readonly Color SkyBlue;

        /// <summary>
        /// SlateBlue color (R:106,G:90,B:205,A:255).
        /// </summary>
        public static readonly Color SlateBlue;

        /// <summary>
        /// SlateGray color (R:112,G:128,B:144,A:255).
        /// </summary>
        public static readonly Color SlateGray;

        /// <summary>
        /// Snow color (R:255,G:250,B:250,A:255).
        /// </summary>
        public static readonly Color Snow;

        /// <summary>
        /// SpringGreen color (R:0,G:255,B:127,A:255).
        /// </summary>
        public static readonly Color SpringGreen;

        /// <summary>
        /// SteelBlue color (R:70,G:130,B:180,A:255).
        /// </summary>
        public static readonly Color SteelBlue;

        /// <summary>
        /// Tan color (R:210,G:180,B:140,A:255).
        /// </summary>
        public static readonly Color Tan;

        /// <summary>
        /// Teal color (R:0,G:128,B:128,A:255).
        /// </summary>
        public static readonly Color Teal;

        /// <summary>
        /// Thistle color (R:216,G:191,B:216,A:255).
        /// </summary>
        public static readonly Color Thistle;

        /// <summary>
        /// Tomato color (R:255,G:99,B:71,A:255).
        /// </summary>
        public static readonly Color Tomato;

        /// <summary>
        /// Turquoise color (R:64,G:224,B:208,A:255).
        /// </summary>
        public static readonly Color Turquoise;

        /// <summary>
        /// Violet color (R:238,G:130,B:238,A:255).
        /// </summary>
        public static readonly Color Violet;

        /// <summary>
        /// Wheat color (R:245,G:222,B:179,A:255).
        /// </summary>
        public static readonly Color Wheat;

        /// <summary>
        /// White color (R:255,G:255,B:255,A:255).
        /// </summary>
        public static readonly Color White;

        /// <summary>
        /// WhiteSmoke color (R:245,G:245,B:245,A:255).
        /// </summary>
        public static readonly Color WhiteSmoke;

        /// <summary>
        /// Yellow color (R:255,G:255,B:0,A:255).
        /// </summary>
        public static readonly Color Yellow;

        /// <summary>
        /// YellowGreen color (R:154,G:205,B:50,A:255).
        /// </summary>
        public static readonly Color YellowGreen;

        #endregion

        /// <summary>
        /// Gets the luma of an existing color.
        /// </summary>
        /// <returns>A value based on this code: (color.R + color.R + color.B + color.G + color.G + color.G) / 6f</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetLuma() => (R + R + B + G + G + G) / 6f;

        /// <summary>
        /// Gets the brightness of a color.
        /// </summary>
        /// <returns>The brightness value.</returns>
        /// <remarks>Taken from the mono source code.</remarks>
        [Pure]
        public float GetBrightness()
        {
            byte minval = Math.Min(R, Math.Min(G, B));
            byte maxval = Math.Max(R, Math.Max(G, B));

            return (float)(maxval + minval) / 510;
        }

        /// <summary>
        /// Gets the saturation of a color.
        /// </summary>
        /// <returns>The saturation value.</returns>
        /// <remarks>Taken from the mono source code.</remarks>
        [Pure]
        public float GetSaturation()
        {
            byte minval = Math.Min(R, Math.Min(G, B));
            byte maxval = Math.Max(R, Math.Max(G, B));


            if (maxval == minval)
                return 0.0f;

            int sum = maxval + minval;
            if (sum > 255)
                sum = 510 - sum;

            return (float)(maxval - minval) / sum;
        }

        /// <summary>
        /// Gets the hue of a color.
        /// </summary>
        /// <returns>The hue value.</returns>
        /// <remarks>Taken from the mono source code.</remarks>
        [Pure]
        public float GetHue()
        {
            byte minval = Math.Min(R, Math.Min(G, B));
            byte maxval = Math.Max(R, Math.Max(G, B));


            if (maxval == minval)
                return 0.0f;

            float diff = maxval - minval;
            float rnorm = (maxval - R) / diff;
            float gnorm = (maxval - G) / diff;
            float bnorm = (maxval - B) / diff;


            float hue = 0.0f;
            if (R == maxval)
                hue = 60.0f * (6.0f + bnorm - gnorm);

            if (G == maxval)
                hue = 60.0f * (2.0f + rnorm - bnorm);

            if (B == maxval)
                hue = 60.0f * (4.0f + gnorm - rnorm);

            if (hue > 360.0f)
                hue -= 360.0f;

            return hue;
        }

        /// <summary>
        /// Performs linear interpolation of <see cref="Color"/>.
        /// </summary>
        /// <param name="value1">Source <see cref="Color"/>.</param>
        /// <param name="value2">Destination <see cref="Color"/>.</param>
        /// <param name="amount">Interpolation factor.</param>
        /// <returns>Interpolated <see cref="Color"/>.</returns>
        [Pure]
        public static Color Lerp(Color value1, Color value2, float amount)
        {
            amount = MathHelpers.Clamp(amount, 0, 1);
            return new Color(
                (int)MathHelpers.Lerp(value1.R, value2.R, amount),
                (int)MathHelpers.Lerp(value1.G, value2.G, amount),
                (int)MathHelpers.Lerp(value1.B, value2.B, amount),
                (int)MathHelpers.Lerp(value1.A, value2.A, amount));
        }

        /// <summary>
        /// Returns true if the colors represent the same color values.
        /// </summary>
        /// <param name="other"/>
        /// <returns/>
        [Pure]
        public bool Matches(Color other) => Equals(other);

        /// <summary>
        /// Multiply <see cref="Color"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="Color"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Multiply(Color value, float scale) => new Color((int)(value.R * scale),
            (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));

        /// <summary>
        /// Creates an array of colors that includes the <paramref name="startingColor"/> and <paramref name="endingColor"/> and <paramref name="steps"/> of colors between them.
        /// </summary>
        /// <param name="startingColor">The starting color which will be at index 0 in the array.</param>
        /// <param name="endingColor">The ending color which will be at index `steps - 1` in the array.</param>
        /// <param name="steps">The gradient steps in the array which uses <see cref="Lerp(Color, Color, float)"/>.</param>
        /// <returns>An array of colors.</returns>
        [Pure]
        public static Color[] LerpSteps(Color startingColor, Color endingColor, int steps)
        {
            Color[] colors = new Color[steps];

            float stopStrength = 1f / (steps - 1);

            float lerpTotal = 0f;

            colors[0] = startingColor;
            colors[steps - 1] = endingColor;

            for (int i = 1; i < steps - 1; i++)
            {
                lerpTotal += stopStrength;

                colors[i] = Lerp(startingColor, endingColor, lerpTotal);
            }

            return colors;
        }

        /// <summary>
        /// Sets the color values based on HSL instead of RGB.
        /// </summary>
        /// <param name="h">The hue amount.</param>
        /// <param name="s">The saturation amount.</param>
        /// <param name="l">The luminance amount.</param>
        /// <remarks>Taken from http://www.easyrgb.com/index.php?X=MATH&amp;H=19#text19 </remarks>
        [Pure]
        public static Color FromHSL(float h, float s, float l)
        {
            if (Math.Abs(s) < 0.0000000001)
                return new Color(
                    (byte)(l * 255),
                    (byte)(l * 255),
                    (byte)(l * 255)
                );
            else
            {
                float var_2;
                float var_1;

                if (l < 0.5)
                    var_2 = l * (1 + s);
                else
                    var_2 = l + s - s * l;

                var_1 = 2 * l - var_2;

                return new Color(
                    (byte)(255 * Hue_2_RGB(var_1, var_2, h + 1f / 3)),
                    (byte)(255 * Hue_2_RGB(var_1, var_2, h)),
                    (byte)(255 * Hue_2_RGB(var_1, var_2, h - 1f / 3))
                );
            }
        }

        private static float Hue_2_RGB(float v1, float v2, float vH)
        {
            if (vH < 0)
                vH += 1;

            if (vH > 1)
                vH -= 1;

            if (6 * vH < 1)
                return v1 + (v2 - v1) * 6 * vH;

            if (2 * vH < 1)
                return v2;

            if (3 * vH < 2)
                return v1 + (v2 - v1) * (2f / 3 - vH) * 6;

            return v1;
        }

        /// <summary>
        /// Multiply <see cref="Color"/> by value.
        /// </summary>
        /// <param name="value">Source <see cref="Color"/>.</param>
        /// <param name="scale">Multiplicator.</param>
        /// <returns>Multiplication result.</returns>
        [Pure]
        public static Color operator *(Color value, float scale) => new Color((int)(value.R * scale),
            (int)(value.G * scale), (int)(value.B * scale), (int)(value.A * scale));

        /// <summary>
        /// Gets the packed value of this <see cref="Color"/>.
        /// </summary>
        public uint PackedValue => _packedValue;


        internal string DebugDisplayString => string.Concat(
            R.ToString(), "  ",
            G.ToString(), "  ",
            B.ToString(), "  ",
            A.ToString()
        );


        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Color"/> in the format:
        /// {R:[red] G:[green] B:[blue] A:[alpha]}
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="Color"/>.</returns>
        [Pure]
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(25);
            sb.Append("{R:");
            sb.Append(R);
            sb.Append(" G:");
            sb.Append(G);
            sb.Append(" B:");
            sb.Append(B);
            sb.Append(" A:");
            sb.Append(A);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Translate a non-premultipled alpha <see cref="Color"/> to a <see cref="Color"/> that contains premultiplied alpha.
        /// </summary>
        /// <param name="r">Red component value.</param>
        /// <param name="g">Green component value.</param>
        /// <param name="b">Blue component value.</param>
        /// <param name="a">Alpha component value.</param>
        /// <returns>A <see cref="Color"/> which contains premultiplied alpha data.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color FromNonPremultiplied(int r, int g, int b, int a)
            => new Color(r * a / 255, g * a / 255, b * a / 255, a);

        #region IEquatable<Color> Members

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="Color"/>.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        [Pure]
        public bool Equals(Color other) => PackedValue == other.PackedValue;

        #endregion

        /// <summary>
        /// Deconstruction method for <see cref="Color"/>.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        [Pure]
        public void Deconstruct(out float r, out float g, out float b)
        {
            r = R;
            g = G;
            b = B;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Color"/> with Alpha.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        [Pure]
        public void Deconstruct(out float r, out float g, out float b, out float a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Color"/>.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        [Pure]
        public void Deconstruct(out byte r, out byte g, out byte b)
        {
            r = R;
            g = G;
            b = B;
        }

        /// <summary>
        /// Deconstruction method for <see cref="Color"/> with Alpha.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        [Pure]
        public void Deconstruct(out byte r, out byte g, out byte b, out byte a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }
    }
}
