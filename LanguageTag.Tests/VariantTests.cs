﻿using NUnit.Framework;
using System;
using System.Linq;

namespace AbbyyLS.Globalization.Bcp47
{
	[TestFixture]
	public class VariantTests
	{
		[Test]
		public void CheckSwitches()
		{
			foreach (var text in TestContent.GetVariants())
			{
				var variant = text.TryParseFromVariant().Value;
				variant.GetPrefixes();
				Assert.NotNull(variant.ToText());
			}
		}

		[Test]
		public void GetPrefixesFail()
		{
			Assert.Throws<NotImplementedException>(() =>
			{
				var variant = (Variant)(-1);
				variant.GetPrefixes();
			});
		}

		[Test]
		public void ToTextFail()
		{
			Assert.Throws<NotImplementedException>(() =>
			{
				var variant = (Variant)(-1);
				variant.ToText();
			});
		}

		[TestCase("xxx", null)]
		[TestCase("aluku", Variant.Aluku)]
		[TestCase("1901", Variant.V1901)]
		public void ParseFromVariant(string text, Variant? expected)
		{
			Assert.AreEqual(expected, text.TryParseFromVariant());
		}

		[TestCase("sr-ekavsk")]
		[TestCase("sr-Cyrl-ekavsk")]
		[TestCase("sr-fonipa")]
		[TestCase("sr-Cyrl-fonipa")]
		[TestCase("sr-Hans-fonipa")]
		[TestCase("az-Baku1926")]
		[TestCase("en-fonipa-Scotland")]
		[TestCase("en-scotland-fonipa")]
		[TestCase("sl-rozaj-biske")]
		public void VariantCollectionCreate(string tagText)
		{
			var tag = new LanguageTag(tagText);
			var variants = VariantCollection.Create(tag.Language, tag.Script, tag.Variants);

			Assert.That(variants, Is.EqualTo(tag.Variants));
		}

		[TestCase("en",  Variant.Ekavsk)]
		[TestCase("sr-Hans", Variant.Ekavsk)]
		[TestCase("sl", Variant.V1994)]
		[TestCase("en", Variant.Baku1926)]
		[TestCase("sl-rozaj-biske-fonipa", Variant.V1994)]
		public void VariantCollectionCreate(string tagText, Variant appendVariant)
		{
			Assert.Throws<FormatException>(() =>
			{
				var tag = new LanguageTag(tagText);
				VariantCollection.Create(tag.Language, tag.Script, tag.Variants.Union(new Variant[] { appendVariant }));
			});
		}

		[Test]
		public void Equals()
		{
			var vc1 = new VariantCollection();
			var vc2 = new VariantCollection();

			var vc3 = new VariantCollection(Variant.Alalc97, Variant.Aluku);
			var vc4 = new VariantCollection(Variant.Alalc97, Variant.Aluku);

			var vc5 = new VariantCollection(Variant.Alalc97);

			Assert.IsFalse(vc1.Equals(null));
			Assert.IsTrue(vc1.Equals((object)vc2));
			Assert.AreEqual(vc1, vc1);
			Assert.AreEqual(vc1, vc2);
			Assert.AreNotEqual(vc1, vc3);
			Assert.AreEqual(vc3, vc3);
			Assert.AreEqual(vc3, vc4);
			Assert.AreNotEqual(vc4, vc5);

			Assert.IsTrue(vc1 == vc2);
			Assert.IsFalse(vc1 != vc2);

			Assert.IsTrue(vc3 != vc5);
			Assert.IsFalse(vc3 == vc5);
		}

		[TestCase(new Variant[] { }, Variant.Alalc97, false)]
		[TestCase(new Variant[] { Variant.Aluku }, Variant.Alalc97, false)]
		[TestCase(new Variant[] { Variant.Aluku }, Variant.Aluku, true)]
		[TestCase(new Variant[] { Variant.Aluku, Variant.Alalc97 }, Variant.Alalc97, true)]
		public void Contains(Variant[] variants, Variant tag, bool expected)
		{
			Assert.That(new VariantCollection(variants).Contains(tag), Is.EqualTo(expected));
		}
	}
}
