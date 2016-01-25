//Mateusz Osipa
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exercise;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections;

namespace Tests
{
    [TestClass]
    public class BasicTests : IEqualityComparer<Card>
    {
        /// <summary>
        /// http://www.functionx.com/cppcli/classes2/Lesson24d.htm
        /// http://msdn.microsoft.com/en-us/library/skef48fy.aspx
        /// http://stackoverflow.com/questions/880984/implementing-an-interface-declared-in-c-sharp-from-c-cli
        /// https://msdn.microsoft.com/pl-pl/library/system.icloneable%28v=vs.110%29.aspx
        /// </summary>
        [TestMethod]
        public void ClassDefinitions()
        {
            Assert.IsTrue(typeof(Card).GetInterfaces().Contains(typeof(IComparable<Card>)));
            Assert.IsTrue(typeof(Card).GetInterfaces().Contains(typeof(ICloneable)));
            Assert.IsTrue(typeof(CardSet<Card>).GetInterfaces().Contains(typeof(IEnumerable<Card>)));
            Assert.IsTrue(typeof(CardSet<>).GetInterfaces().Any(i => i.Name == typeof(IEnumerable<>).Name));
            Assert.IsTrue(typeof(CardSet<>).GetInterfaces().Contains(typeof(ICloneable)));
            var genericArgument = typeof(CardSet<>).GetGenericArguments().First();
            var genericArgumentConstraints = genericArgument.GetGenericParameterConstraints();
            var iComparableConstraint = genericArgumentConstraints.FirstOrDefault(t => t.GUID == typeof(IComparable<>).GUID);
            Assert.AreEqual(genericArgument, iComparableConstraint.GetGenericArguments().First());
        }

        [TestMethod]
        public void CollectionCloneInConstructor()
        {
            var two = new Card(CardType.Two, CardColor.Diamond);
            var three = new Card(CardType.Three, CardColor.Club);
            var cards = new Card[] { two, three };
            var set = new CardSet<Card>(cards);
            cards[cards.Length - 1] = new Card(CardType.Ace, CardColor.Spade);
            Assert.AreNotEqual(cards[cards.Length - 1].Type, set.ToArray()[cards.Length - 1].Type);
            Assert.AreNotEqual(cards[cards.Length - 1].Color, set.ToArray()[cards.Length - 1].Color);
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/tzz3794d.aspx
        /// http://pl.wikipedia.org/wiki/Kier
        /// http://pl.wikipedia.org/wiki/Karo
        /// http://pl.wikipedia.org/wiki/Pik
        /// http://pl.wikipedia.org/wiki/Trefl
        /// </summary>
        [TestMethod]
        public void EnumerationConstrains()
        {
            Assert.AreEqual(1, sizeof(SortAlgorithm));
            Assert.AreEqual(1, sizeof(CardColor));
            var colors = Enum.GetValues(typeof(CardColor))
                .Cast<CardColor>()
                .ToArray();
            Assert.AreEqual(4, colors.Length);
            Assert.AreEqual(2, sizeof(CardType));
            var types = Enum.GetValues(typeof(CardType))
                .Cast<CardType>()
                .ToArray();
            Assert.AreEqual(13, types.Length);
            for (int i = 0; i < types.Length; i++)
            {
                Assert.AreEqual(Math.Pow(2, i), (double)types[i]);
            }
        }

        /// <summary>
        /// http://stackoverflow.com/questions/2133536/implementing-ienumerablet-in-c-cli
        /// http://msdn.microsoft.com/en-us/library/2kb28261.aspx
        /// http://msdn.microsoft.com/en-us/library/dtbydz1t.aspx
        /// </summary>
        [TestMethod]
        public void EnumerateCards()
        {
            var cards = new Card[] { new Card(CardType.Nine, CardColor.Spade), new Card(CardType.Jack, CardColor.Club) };
            var initialLength = cards.Length;
            var set = new CardSet<Card>(cards);
            Array.Resize(ref cards, 3);
            cards[2] = new Card(CardType.Ace, CardColor.Spade);
            var enumerated = set.AsEnumerable().ToArray();
            Assert.AreEqual(initialLength, enumerated.Count());
            for (int i = 0; i < initialLength; i++)
            {
                Assert.AreEqual(cards[i], enumerated[i]);
            }
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/vstudio/bb335435%28v=vs.100%29.aspx
        /// </summary>
        [TestMethod]
        public void NoLinqMethodDuplication()
        {
            Expression<Func<IEnumerable<Card>, IEnumerable<Card>>> call = cards => Enumerable.AsEnumerable<Card>(cards);
            var forbidden = (call.Body as MethodCallExpression).Method.Name;
            var ghost = typeof(CardSet<Card>).GetMethod(forbidden, BindingFlags.Public | BindingFlags.Instance);
            Assert.IsNull(ghost);
        }

        /// <summary>
        /// http://www.geocities.ws/jeff_louie/deterministic_destructors.htm
        /// http://msdn.microsoft.com/en-us/library/system.idisposable.dispose.aspx
        /// </summary>
        [TestMethod]
        public void ParameterNamesStartingWithSmallLetter()
        {
            var types = typeof(CardSet<>).Assembly.GetTypes();
            var memberFilter = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            Func<MethodInfo, bool> methodsFilter = mi =>
                !((mi.DeclaringType.GetInterfaces().Contains(typeof(IDisposable)) &&
                   mi.Name == "Dispose" &&
                   mi.ReturnType == typeof(void) &&
                   mi.GetParameters().Count() == 1 &&
                   mi.GetParameters().First().ParameterType == typeof(bool)) ||
                  (mi.DeclaringType.IsValueType &&
                   mi.DeclaringType.FullName == "std.basic_string<char\\,std::char_traits<char>\\,std::allocator<char> >"));
            Func<ParameterInfo, bool> parameterFilter = p => Char.IsUpper(p.Name.First());
            Func<IEnumerable<ParameterInfo>, MemberInfo, Type, string> message = (parameters, member, type) =>
            {
                var arrified = parameters.Select(pi => pi.ToString()).ToArray();
                string result = String.Format("Invalid parameters \"{0}\" in member \"{1}\" of type \"{2}\".", String.Join(", ", arrified), member, type);
                return result;
            };
            foreach (var type in types)
            {
                var settersIndexersAndOtherMethods = type.GetMethods(memberFilter)
                    .Where(methodsFilter);
                foreach (var method in settersIndexersAndOtherMethods)
                {
                    var parameters = method.GetParameters()
                        .Where(parameterFilter);
                    int x = parameters.Count();
                    Assert.AreEqual(0, parameters.Count(), message(parameters, method, type));
                }
                var constructors = type.GetConstructors(memberFilter | BindingFlags.CreateInstance);
                foreach (var constructor in constructors)
                {
                    var parameters = constructor.GetParameters()
                        .Where(parameterFilter);
                    Assert.AreEqual(0, parameters.Count(), message(parameters, constructor, type));
                }
            }
        }

        [TestMethod]
        public void MethodNamesStartingWithBigLetter()
        {
            var types = typeof(CardSet<>).Assembly.GetTypes();
            var memberFilter = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            Func<MethodInfo, bool> methodFilter = mi =>
                ((mi.Attributes & MethodAttributes.SpecialName) != MethodAttributes.SpecialName) &&
                (Char.IsLower(mi.Name.First(c => c != '_')));
            foreach (var type in types)
            {
                var oddOnes = type.GetMethods(memberFilter).Where(methodFilter);
                if (oddOnes.Any())
                {
                    var methodNames = oddOnes.Select(oddOne => oddOne.ToString()).ToArray();
                    var message = String.Format("Invalid method name(s) \"{0}\" of type \"{1}\".", String.Join(", ", methodNames), type);
                    Assert.Fail(message);
                }
            }
        }

        [TestMethod]
        public void NonPublicEnumerators()
        {
            var types = typeof(CardSet<>).Assembly.GetTypes();
            var enumeratorTypes = types.Where(t => t.GetInterfaces().Contains(typeof(IEnumerator)) || t.GetInterfaces().Contains(typeof(IEnumerator<>)));
            foreach (var type in enumeratorTypes)
            {
                Assert.IsFalse(type.IsPublic);
            }
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms235303.aspx
        /// http://stackoverflow.com/questions/15358631/why-c-cli-indexed-properties-does-not-work-in-c
        /// http://msdn.microsoft.com/en-us/library/2f1ec0b1.aspx
        /// </summary>
        [TestMethod]
        public void IndexedProperties()
        {
            var a1 = new Card(CardType.Ace, CardColor.Spade);
            var a2 = new Card(CardType.Ace, CardColor.Diamond);
            var q = new Card(CardType.Queen, CardColor.Heart);
            var s = new Card(CardType.Seven, CardColor.Spade);
            var set = new CardSet<Card>(new Card[] { a1, a2, q, s });
            Assert.AreEqual(q, set[2]);
            var temp = set[2];
            set[2] = set[1];
            set[1] = temp;
            Assert.AreEqual(q, set[1]);
            Assert.AreEqual(a2, set[2]);
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/43hc6wht%28v=vs.110%29.aspx
        /// </summary>
        [TestMethod]
        public void CardComparison()
        {
            var a1 = new Card(CardType.Ace, CardColor.Spade);
            var a2 = new Card(CardType.Ace, CardColor.Diamond);
            Assert.AreEqual(0, a1.CompareTo(a2));
            var types = Enum.GetValues(typeof(CardType))
                .Cast<CardType>()
                .ToArray();
            var colors = Enum.GetValues(typeof(CardColor))
                .Cast<CardColor>()
                .ToArray();
            foreach (var t in types)
            {
                foreach (var c1 in colors)
                {
                    foreach (var c2 in colors.Except(new CardColor[] { c1 }))
                    {
                        var a = new Card(t, c1);
                        var b = new Card(t, c2);
                        Assert.AreEqual(0, a.CompareTo(b));
                    }
                }
                foreach (var c in colors)
                {
                    var a = new Card(t, c);
                    var b = new Card(t, c);
                    Assert.AreEqual(0, a.CompareTo(b));
                }
            }
            var q = new Card(CardType.Queen, CardColor.Heart);
            Assert.IsTrue(a1.CompareTo(q) > 0);
            Assert.IsTrue(q.CompareTo(a1) < 0);
            Assert.AreEqual(2, a1.CompareTo(q));
            var s = new Card(CardType.Seven, CardColor.Spade);
            Assert.AreEqual(-5, s.CompareTo(q));
            for (int i = 0; i < types.Length; i++)
            {
                var x = new Card(types[i], CardColor.Heart);
                for (int j = 0; j < i; j++)
                {
                    var y = new Card(types[j], CardColor.Heart);
                    Assert.AreEqual(i - j, x.CompareTo(y));
                }
                for (int j = i + 1; j < types.Length; j++)
                {
                    var y = new Card(types[j], CardColor.Heart);
                    Assert.AreEqual(i - j, x.CompareTo(y));
                }
            }
        }

        /// <summary>
        /// http://www.functionx.com/cppcli/classes2/Lesson24d.htm
        /// </summary>
        [TestMethod]
        public void PropertyConstrains()
        {
            var cardsPi = typeof(CardSet<>).GetProperty("Cards", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsTrue(cardsPi.GetSetMethod(true).IsPrivate);
            Assert.IsTrue(cardsPi.GetGetMethod(true).IsPrivate);
            var typePi = BasicTests.GetProperty<Card, CardType>(c => c.Type);
            Assert.IsTrue(typePi.GetSetMethod(true).IsPrivate);
            var colorPi = BasicTests.GetProperty<Card, CardColor>(c => c.Color);
            Assert.IsTrue(colorPi.GetSetMethod(true).IsPrivate);
            var comparisonsPi = BasicTests.GetProperty<SortSummary, uint>(s => s.Comparisons);
            Assert.IsTrue(comparisonsPi.GetSetMethod(true).IsPrivate);
            Assert.AreEqual(typeof(uint), comparisonsPi.PropertyType);
            var swapsPi = BasicTests.GetProperty<SortSummary, uint>(s => s.Swaps);
            Assert.IsTrue(swapsPi.GetSetMethod(true).IsPrivate);
            Assert.AreEqual(typeof(uint), swapsPi.PropertyType);
        }

        [TestMethod]
        public void EveryCardHasNewIndexAfterRandomize()
        {
            var types = Enum.GetValues(typeof(CardType))
                .Cast<CardType>()
                .ToArray();
            var cards = types.Select(t => new Card(t, CardColor.Club));
            var set = new CardSet<Card>(cards.ToArray());
            set.Randomize();
            var randomized = set.AsEnumerable().Select(r => r.Type).ToArray();
            for (int i = 0; i < types.Length; i++)
            {
                Assert.AreNotEqual(types[i], randomized[i]);
            }
        }

        [TestMethod]
        public void NewOrderAfterRandomize()
        {
            var types = Enum.GetValues(typeof(CardType))
                .Cast<CardType>()
                .ToArray();
            var cards = types.Select(t => new Card(t, CardColor.Club));
            var first = cards.First().Type;
            var set = new CardSet<Card>(cards.ToArray());
            set.Randomize();
            var randomized = set.AsEnumerable().Select(c => c.Type).ToList();
            var offset = randomized.IndexOf(first);
            var same = 1;
            for (int i = 1; i < types.Length; i++)
            {
                var r = randomized[(i + offset) % types.Length];
                if (types[i] == r)
                {
                    same++;
                }
            }
            Assert.AreNotEqual(types.Length, same);
        }

        /// <summary>
        /// http://en.wikipedia.org/wiki/Bubble_sort
        /// http://en.wikipedia.org/wiki/Quicksort
        /// http://edu.i-lo.tarnow.pl/inf/alg/003_sort/0018.php
        /// </summary>
        [TestMethod]
        public void SortAlgorithms()
        {
            var types = Enum.GetValues(typeof(CardType))
                .Cast<CardType>()
                .ToArray();
            var clubs = types.Select(t => new Card(t, CardColor.Club));
            var hearts = types.Select(t => new Card(t, CardColor.Heart));
            var set = new CardSet<Card>(clubs.Union(hearts).ToArray());
            set.Randomize();
            var enumerated = set.AsEnumerable().ToArray();
            var bubbleSummary = set.Sort(SortAlgorithm.Bubble);
            Assert.IsTrue(bubbleSummary.Swaps > 0);
            Assert.IsTrue(bubbleSummary.Comparisons > 0);
            var bubblePass = set.AsEnumerable().ToArray();
            for (int i = 0; i < types.Length; i++)
            {
                Assert.AreEqual(types[i], bubblePass[2 * i].Type);
                Assert.AreEqual(types[i], bubblePass[2 * i + 1].Type);
            }
            set = new CardSet<Card>(enumerated);
            var quickSummary = set.Sort(SortAlgorithm.Quick);
            Assert.IsTrue(quickSummary.Swaps > 0);
            Assert.IsTrue(quickSummary.Comparisons > 0);
            Assert.IsTrue(bubbleSummary.Swaps > quickSummary.Swaps);
            Assert.IsTrue(bubbleSummary.Comparisons > quickSummary.Comparisons);
            var quickPass = set.AsEnumerable().ToArray();
            for (int i = 0; i < types.Length; i++)
            {
                Assert.AreEqual(types[i], quickPass[2 * i].Type);
                Assert.AreEqual(types[i], quickPass[2 * i + 1].Type);
            }
            uint samePositions = 0;
            uint differentPositions = 0;
            for (int i = 0; i < enumerated.Length; i++)
            {
                if (bubblePass[i] == quickPass[i])
                {
                    samePositions++;
                }
                else
                {
                    differentPositions++;
                }
            }
            Assert.IsTrue(samePositions > 0);
            Assert.IsTrue(differentPositions > 0);
        }

        [TestMethod]
        public void NoSortSummaryExtraPublicMethods()
        {
            var allowed = new MethodInfo[]
            {
                GetProperty<SortSummary, uint>(ss => ss.Swaps).GetGetMethod(),
                GetProperty<SortSummary, uint>(ss => ss.Comparisons).GetGetMethod()
            };
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
            var publicMethods = typeof(SortSummary).GetMethods(flags)
                .Where(m => m.DeclaringType == typeof(SortSummary));
            var ghosts = publicMethods.Except(allowed);
            if (ghosts.Any())
            {
                var signatures = ghosts.Select(mi => mi.ToString()).ToArray();
                var message = String.Format("Found methods \"{0}\" in type \"{1}\", that are not allowed.", String.Join(", ", signatures), typeof(SortSummary).FullName);
                Assert.Fail(message);
            }
        }

        [TestMethod]
        public void Clone()
        {
            var types = Enum.GetValues(typeof(CardType))
               .Cast<CardType>()
               .ToList();
            var colors = Enum.GetValues(typeof(CardColor))
                .Cast<CardColor>()
                .ToList();
            var cards = new List<Card>();
            colors.ForEach(c => types.ForEach(t => cards.Add(new Card(t, c))));
            var fullSet = new CardSet<Card>(cards.ToArray());
            fullSet.Randomize();
            var clone = (CardSet<Card>)fullSet.Clone();
            Assert.AreNotSame(fullSet, clone);
            Assert.AreEqual(0, fullSet.Except(clone, this).Count());
            for (int i = 0; i < types.Count * colors.Count; i++)
            {
                Assert.AreNotSame(fullSet[i], clone[i]);
                Assert.AreEqual(fullSet[i].Color, clone[i].Color);
                Assert.AreEqual(fullSet[i].Type, clone[i].Type);
            }
        }

        [TestMethod]
        public void GetHashCodeAndEquals()
        {
            var card = new Card(CardType.Ace, CardColor.Heart);
            var clone = (Card)(card.Clone());
            Assert.AreEqual(card.GetHashCode(), clone.GetHashCode());
            Assert.AreEqual(this.GetHashCode(card), clone.GetHashCode());
            Assert.AreEqual(this.Equals(card, clone), Comparer.Equals(card, clone));
        }

        private static PropertyInfo GetProperty<T, P>(Expression<Func<T, P>> e)
        {
            var propertyName = (e.Body as System.Linq.Expressions.MemberExpression).Member.Name;
            var pi = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return pi;
        }

        public bool Equals(Card x, Card y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else
            {
                if ((x == null) || (y == null))
                {
                    return false;
                }
                else
                {
                    if (object.ReferenceEquals(x, y))
                    {
                        return true;
                    }
                    else
                    {
                        return x.Color == y.Color && x.Type == y.Type;
                    }
                }
            }
        }

        public int GetHashCode(Card obj)
        {
            return (obj != null) ? (int)obj.Color * (int)obj.Type : 0;
        }
    }
}
