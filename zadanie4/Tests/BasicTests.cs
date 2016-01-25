//Mateusz Osipa
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exercise;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Tests
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void Types()
        {
            Assert.IsTrue(typeof(IRoom).IsInterface);
            var r = new Room(35.0, RoomStandard.Apartment);
            Assert.IsTrue(typeof(Room).GetInterfaces().Contains(typeof(IRoom)));
            Assert.AreEqual(35.0, (r as IRoom).SquareMeters);
            Assert.IsTrue(typeof(IHotel).IsInterface);
            Assert.IsTrue(typeof(Hotel<Room, SquareMetersComparer>).GetInterfaces().Contains(typeof(IHotel)));
            var h = new Hotel<Room, SquareMetersComparer>(5, r);
            Assert.AreEqual((uint)5, (h as IHotel).Stars);
            Assert.IsTrue(typeof(SquareMetersComparer).GetInterfaces().Contains(typeof(IComparer<Room>)));
            Assert.IsTrue(typeof(StandardMultipliedSquareMetersComparer).GetInterfaces().Contains(typeof(IComparer<Room>)));
            Assert.AreEqual(typeof(SquareMetersComparer).BaseType, typeof(StandardMultipliedSquareMetersComparer).BaseType);
            Assert.AreNotEqual(typeof(SquareMetersComparer).BaseType, typeof(object));
            Assert.AreNotEqual(typeof(SquareMetersComparer).BaseType, typeof(ValueType));
            Assert.IsTrue(typeof(SquareMetersComparer).BaseType.IsAbstract);
            Assert.AreNotEqual(typeof(SquareMetersComparer).BaseType, typeof(Comparer<Room>));
        }

        [TestMethod]
        public void GenericConstrains()
        {
            var genericArguments = typeof(Hotel<,>).GetGenericArguments();
            Assert.IsTrue(genericArguments[0].GetGenericParameterConstraints().Contains(typeof(IRoom)));
            Assert.AreEqual(GenericParameterAttributes.DefaultConstructorConstraint, genericArguments[1].GenericParameterAttributes);
        }

        [TestMethod]
        public void RoomStandards()
        {
            Assert.AreEqual((byte)RoomStandard.Apartment, (byte)RoomStandard.Student * 3);
            Assert.AreEqual((byte)RoomStandard.Standard, (byte)RoomStandard.Student * 2);
            Assert.IsTrue(typeof(RoomStandard).IsEnum);
            Assert.AreEqual(sizeof(byte), sizeof(RoomStandard));
        }

        [TestMethod]
        public void Constructor()
        {
            var r1 = new Room(35.0, RoomStandard.Apartment);
            var r2 = new Room(24.5, RoomStandard.Student);
            var h1 = new Hotel<Room, SquareMetersComparer>(3, r1);
            Assert.AreEqual((uint)3, h1.Stars);
            var h2 = new Hotel<Room, SquareMetersComparer>(3, r1, r2);
            var h3 = new Hotel<Room, SquareMetersComparer>(3, new Room[] { r1, r2 });
            var ht = typeof(Hotel<Room, SquareMetersComparer>);
            var constructors = ht.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);
            var parameterlessConstructor = constructors.FirstOrDefault(c => c.GetParameters().Count() == 0);
            var noEmptyConstructorMessage = String.Format("Parameterless constructor {0} of type {1} is redundant.", parameterlessConstructor, ht.Name);
            Assert.IsNull(parameterlessConstructor, noEmptyConstructorMessage);
            var arrayConstructor = constructors.Where(c => c.GetParameters().Count() > 0)
                .FirstOrDefault(c => c.GetParameters().Last().ParameterType == typeof(Room[]));
            var arrayConstructorCustomAttribues = arrayConstructor.GetParameters().Last().CustomAttributes;
            var incorrectSignatureMessage = String.Format("Missing variable argument list parameter modifier \"params\" assisting array of type {0} in constructor {1} of type {2}.", r1.GetType().Name, arrayConstructor, ht.Name);
            Assert.IsTrue(arrayConstructorCustomAttribues.Count(a => a.AttributeType == typeof(ParamArrayAttribute)) == 1, incorrectSignatureMessage);
            var redundantConstructorsMessage = String.Format("Additional constructors accepting {0} instances are redundant in type {1}.", r1.GetType().Name, ht.Name);
            var spareConstructorsCount = constructors.Where(c => c.GetParameters().Count() > 0)
                .Count(c => c.GetParameters().Last().ParameterType == typeof(Room));
            Assert.AreEqual(0, spareConstructorsCount, redundantConstructorsMessage);
        }

        [TestMethod]
        public void Comparers()
        {
            var smsmc = new StandardMultipliedSquareMetersComparer();
            var smc = new SquareMetersComparer();
            var r1 = new Room(35.0, RoomStandard.Apartment);
            Assert.AreEqual(0, smsmc.Compare(new Room(105.0, RoomStandard.Student), r1)); // 35*3x=105x
            var r2 = new Room(25.0, RoomStandard.Standard);
            Assert.AreEqual(0, smsmc.Compare(new Room(50.0, RoomStandard.Student), r2)); // 25*2x=50x
            var r3 = new Room(40.0, RoomStandard.Student); // 40*1x=40x
            var rooms = new Room[] { r1, r2, r3 };
            var h1 = new Hotel<Room, SquareMetersComparer>(5, rooms);
            Assert.IsTrue(h1.AsEnumerable().SequenceEqual(new Room[] { r2, r1, r3 }));
            var h2 = new Hotel<Room, StandardMultipliedSquareMetersComparer>(5, rooms);
             Assert.IsTrue(h2.AsEnumerable().SequenceEqual(new Room[] { r3, r2, r1 }));
            Assert.AreEqual(-1, smsmc.Compare(null, r1));
            Assert.AreEqual(-1, smc.Compare(null, r1));
            Assert.AreEqual(1, smsmc.Compare(r1, null));
            Assert.AreEqual(1, smc.Compare(r1, null));
            Assert.AreEqual(0, smsmc.Compare(null, null));
            Assert.AreEqual(0, smc.Compare(null, null));
            var r4 = new Room(25.0, RoomStandard.Standard);
            Assert.AreEqual(0, smsmc.Compare(r2, r4));
            Assert.AreEqual(0, smc.Compare(r2, r4));
        }

        [TestMethod]
        public void Booking()
        {
            var r1 = new Room(35.0, RoomStandard.Apartment);
            var r2 = new Room(25.0, RoomStandard.Standard);
            var r3 = new Room(40.0, RoomStandard.Student);
            var rooms = new Room[] { r1, r2, r3 };
            var h1 = new Hotel<Room, SquareMetersComparer>(5, rooms);
            var h2 = new Hotel<Room, SquareMetersComparer>(5, rooms);
            var now = DateTime.Now;
            Assert.IsTrue(h1.IsFree(r2, now.AddDays(10), now.AddDays(30)));
            h1.Book(r2, now.AddDays(10), now.AddDays(30));
            h2.Book(r2, now.AddDays(10), now.AddDays(30));
            Assert.IsFalse(h1.IsFree(r2, now.AddDays(10), now.AddDays(30)));
            Assert.IsTrue(h1.IsFree(r1, now.AddDays(10), now.AddDays(30)));
            Assert.IsTrue(h1.IsFree(r3, now.AddDays(10), now.AddDays(30)));
            Assert.IsFalse(h1.IsFree(r2, now.AddDays(1), now.AddDays(20)));
            Assert.IsFalse(h1.IsFree(r2, now.AddDays(20), now.AddDays(40)));
            Assert.IsFalse(h1.IsFree(r2, now.AddDays(9), now.AddDays(31)));
            Assert.IsFalse(h1.IsFree(r2, now.AddDays(11), now.AddDays(29)));
            Assert.IsTrue(h1.IsFree(r2, now.AddDays(1), now.AddDays(9)));
            Assert.IsTrue(h1.IsFree(r2, now.AddDays(31), now.AddDays(40)));
            Assert.IsTrue(h1.IsFree(r2, now.AddHours(1), now.AddHours(2)));
            h1.Book(r2, now.AddHours(1), now.AddHours(2));
            Assert.IsTrue(h2.IsFree(r2, now.AddHours(1), now.AddHours(2)));
            Assert.IsTrue(h1.IsFree(r2, now.AddHours(3), now.AddHours(4)));
            h1.Book(r2, now.AddHours(3), now.AddHours(4));
            var r4 = new Room(25.0, RoomStandard.Standard);
            var forbidden = new Action[]
            {
                () => h1.IsFree(r2, now.AddDays(2), now.AddDays(1)),
                () => h1.Book(r2, now.AddDays(2), now.AddDays(1)),
                () => h1.IsFree(r4, now.AddDays(1), now.AddDays(2)),
                () => h1.Book(r4, now.AddDays(1), now.AddDays(2)),
                () => h1.Book(r2, now, now.AddDays(2)),
                () => h1.Book(r2, now.AddDays(10), now.AddDays(30))
            };

            foreach (var action in forbidden)
            {
                try
                {
                    action();
                    Assert.Fail("Invalid operation performed.");
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (AssertFailedException)
                {
                    throw;
                }
                catch
                {
                    Assert.Fail("Invalid exception type.");
                }
            }
        }

        [TestMethod]
        public void NoLinqMethodDuplication()
        {
            Expression<Func<IEnumerable<Room>, IEnumerable<Room>>> call = rooms => Enumerable.AsEnumerable<Room>(rooms);
            var forbidden = (call.Body as MethodCallExpression).Method.Name;
            var ghost = typeof(Hotel<Room, StandardMultipliedSquareMetersComparer>).GetMethod(forbidden, BindingFlags.Public | BindingFlags.Instance);
            Assert.IsNull(ghost);
        }

        [TestMethod]
        public void ParameterNamesStartingWithSmallLetter()
        {
            var types = typeof(Hotel<Room, SquareMetersComparer>).Assembly.GetTypes();
            var memberFilter = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            Func<ParameterInfo, bool> parameterFilter = p => Char.IsUpper(p.Name.First());
            Func<IEnumerable<ParameterInfo>, MemberInfo, Type, string> message = (parameters, member, type) =>
            {
                var arrified = parameters.Select(pi => pi.ToString()).ToArray();
                string result = String.Format("Invalid parameters \"{0}\" in member \"{1}\" of type \"{2}\".", String.Join(", ", arrified), member, type);
                return result;
            };
            foreach (var type in types)
            {
                var settersIndexersAndOtherMethods = type.GetMethods(memberFilter);
                foreach (var method in settersIndexersAndOtherMethods)
                {
                    var parameters = method.GetParameters()
                        .Where(parameterFilter);
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
            var types = typeof(Hotel<Room, SquareMetersComparer>).Assembly.GetTypes();
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
        public void NoPublicFieldsAndPropertySetters()
        {
            var types = typeof(Room).Assembly.GetTypes();
            var withPublicSetter = types.Where(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Count(p => p.GetSetMethod(false) != null) > 0)
                .Select(t => t.FullName).ToArray();
            var withPublicSetterMessage = String.Format("Detected type(s) {0} with public setter, which is not allowed.", String.Join(", ", withPublicSetter));
            Assert.AreEqual(0, withPublicSetter.Count(), withPublicSetterMessage);
            var withPublicField = types.Where(t => !t.IsEnum)
                .Where(t => t.GetFields(BindingFlags.Public | BindingFlags.Instance).Count() > 0)
                .Where(t => !t.Name.StartsWith("<>c__DisplayClass")) // generated by LINQ ForEach and Any methods
                .Where(t => !t.Name.StartsWith("<GetEnumerator>d__")) // generated by yield return construct
                .ToDictionary(t => t.FullName, t => t.GetFields(BindingFlags.Public | BindingFlags.Instance));
            if (withPublicField.Any())
            {
                var oddOne = withPublicField.First();
                var message = String.Format("Detected type \"{0}\" with public filed(s) \"{1}\", which is not allowed.",
                oddOne.Key,
                String.Join(", ", oddOne.Value.Select(fi => fi.ToString())));
                Assert.Fail(message);
            }
        }

        [TestMethod]
        public void NoExtraFieldsOrPropertiesInHotel()
        {
            var allowed = new Dictionary<Type[], Tuple<uint, bool>>();
            allowed.Add(new Type[]
            {
                typeof(uint)
            }, new Tuple<uint, bool>(1, false));
            allowed.Add(new Type[]
            {
                typeof(SquareMetersComparer),
                typeof(SquareMetersComparer).BaseType
            }, new Tuple<uint, bool>(1, false));
            allowed.Add(new Type[]
            {
                typeof(IEnumerable<Room>),
                typeof(IDictionary<Room, List<KeyValuePair<DateTime, DateTime>>>),
                typeof(IDictionary<Room, Collection<KeyValuePair<DateTime, DateTime>>>),
                typeof(IDictionary<Room, Stack<KeyValuePair<DateTime, DateTime>>>),
                typeof(IDictionary<Room, Queue<KeyValuePair<DateTime, DateTime>>>),
                typeof(IDictionary<Room, KeyValuePair<DateTime, DateTime>[]>),
                typeof(IDictionary<Room, List<Tuple<DateTime, DateTime>>>),
                typeof(IDictionary<Room, Collection<Tuple<DateTime, DateTime>>>),
                typeof(IDictionary<Room, Stack<Tuple<DateTime, DateTime>>>),
                typeof(IDictionary<Room, Queue<Tuple<DateTime, DateTime>>>),
                typeof(IDictionary<Room, Tuple<DateTime, DateTime>[]>),
                typeof(IDictionary<Room, List<List<DateTime>>>),
                typeof(IDictionary<Room, Collection<List<DateTime>>>),
                typeof(IDictionary<Room, Stack<List<DateTime>>>),
                typeof(IDictionary<Room, Queue<List<DateTime>>>),
                typeof(IDictionary<Room, List<DateTime>[]>),
            }, new Tuple<uint, bool>(1, true));
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var ht = typeof(Hotel<Room, SquareMetersComparer>);
            Func<MemberInfo, bool> skipInternals = mi =>
                !mi.Name.StartsWith("CS$<>9__CachedAnonymousMethodDelegate"); // generated by LINQ select clauses
            var fields = ht.GetFields(flags)
                .Where(skipInternals)
                .Cast<FieldInfo>();
            var checkedFields = new List<FieldInfo>();
            var properties = ht.GetProperties(flags)
                .Where(skipInternals)
                .Cast<PropertyInfo>();
            var checkedProperties = new List<PropertyInfo>();
            foreach (var a in allowed)
            {
                uint allowedCount = a.Value.Item1;
                bool allowDerivedType = a.Value.Item2;
                string message = String.Format("At most {0} field{1} or propert{2} allowed of type{1} {3} {4}. ",
                    a.Value.Item1,
                    a.Value.Item1 > 1 ? "s" : String.Empty,
                    a.Value.Item1 > 1 ? "ies are" : "y is",
                    a.Key.Length > 1 ? "{" + String.Join(", ", a.Key.Select(t => t.FriendlyName()).ToArray()) + "}" : a.Key.First().FullName,
                    allowDerivedType ? "or derived types" : "with no derived type possibilities");
                Func<Type, bool> interestingFilter = checkedType =>
                    a.Key.Contains(checkedType) ||
                    (allowDerivedType ? a.Key.Any(t => t.IsAssignableFrom(checkedType)) : false);
                var interestingFields = fields.Where(fi => interestingFilter(fi.FieldType));
                var interestingFieldsString = String.Format("Found fields {0}.", "{" + String.Join(", ", interestingFields.Select(fi => fi.FieldType.FriendlyName() + " " + fi.Name)) + "}");
                Assert.IsTrue(allowedCount >= interestingFields.Count(), message + interestingFieldsString);
                checkedFields.AddRange(interestingFields);
                var interestingProperties = properties.Where(pi => interestingFilter(pi.PropertyType));
                var interestingPropertiesString = String.Format("Found properties {0}.", "{" + String.Join(", ", interestingProperties.Select(pi => pi.ToString())) + "}");
                Assert.IsTrue(allowedCount >= interestingProperties.Count(), message + interestingPropertiesString);
                checkedProperties.AddRange(interestingProperties);
            }
            var ghostFields = fields.Except(checkedFields.Distinct()).Select(fi => fi.ToString()).ToArray();
            var gfMessage = String.Format("Unexpected fields {0} in type {1}.", "{" + String.Join(", ", ghostFields) + "}", ht.Name);
            Assert.AreEqual(0, ghostFields.Count(), gfMessage);
            var ghostProperies = properties.Except(checkedProperties.Distinct()).Select(pi => pi.ToString()).ToArray();
            var gpMessage = String.Format("Unexpected properties {0} in type {1}.", "{" + String.Join(", ", ghostProperies) + "}", ht.Name);
            Assert.AreEqual(0, ghostProperies.Count(), gpMessage);
        }

        [TestMethod]
        public void HotelNotUsingRoomAndRoomNotUsingHotel()
        {
            Action<Type, Type> isOk = null;
            Action<Type, Type> isOkAlgorithm = (notExpected, actual) =>
            {
                Assert.AreNotEqual(notExpected, actual);
                if (actual.IsGenericType)
                {
                    foreach (var type in actual.GetGenericArguments())
                    {
                        isOk(notExpected, type);
                    }
                }
            };
            isOk = isOkAlgorithm;
            Action<Type, Type> test = (where, what) =>
            {
                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                foreach (var fi in where.GetFields(flags))
                {
                    isOk(what, fi.FieldType);
                }
                foreach (var pi in where.GetProperties(flags))
                {
                    isOk(what, pi.PropertyType);
                }
                foreach (var c in where.GetMethods(flags | BindingFlags.CreateInstance))
                {
                    foreach (var ga in c.GetGenericArguments())
                    {
                        isOk(what, ga);
                    }
                    foreach (var p in c.GetParameters())
                    {
                        isOk(what, p.ParameterType);
                    }
                }
            };
            var ht = typeof(Hotel<,>);
            var rt = typeof(Room);
            test(ht, rt);
            test(rt, ht);
        }
    }

    
}
