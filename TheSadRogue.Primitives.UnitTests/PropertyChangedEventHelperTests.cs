using System;
using System.Collections.Generic;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class ValueChangeEventsMock
    {
        private readonly bool _fireChanging;
        private readonly bool _supportsHandled;
        private readonly bool _supportsCanceled;

        private int _value;

        public int Value
        {
            get => _value;
            set
            {
                if (_fireChanging)
                    this.SafelySetProperty(ref _value, value, ValueChanging, ValueChanged, _supportsCanceled,
                        _supportsHandled);
                else
                    this.SafelySetProperty(ref _value, value, ValueChanged, _supportsHandled);
            }
        }

        public event EventHandler<ValueChangingEventArgs<int>>? ValueChanging;
        public event EventHandler<ValueChangedEventArgs<int>>? ValueChanged;

        public ValueChangeEventsMock(bool fireChanging, bool supportsCanceled, bool supportsHandled)
        {
            _fireChanging = fireChanging;
            _supportsCanceled = supportsCanceled;
            _supportsHandled = supportsHandled;
        }
    }

    public class PropertyChangedEventHelperTests
    {
        #region Test Data
        private static readonly bool[] s_booleans = { true, false };

        public static IEnumerable<(bool fireChanging, bool supportsCanceled, bool supportsHandled)> ChangedTestData =
            s_booleans.Combinate(s_booleans).Combinate(s_booleans);

        public static IEnumerable<(bool supportsCanceled, bool supportsHandled)> ChangingTestData =
            s_booleans.Combinate(s_booleans);

        public static IEnumerable<bool> BooleanValues => s_booleans;

        public static IEnumerable<(bool fireChanging, bool supportsCancel)> ValidChangingStates =
            new[] { (true, true), (true, false), (false, false) };

        #endregion


        [Theory]
        [MemberDataTuple(nameof(ChangedTestData))]
        public void ChangedFires(bool fireChanging, bool supportsCanceled, bool supportsHandled)
        {
            var obj = new ValueChangeEventsMock(fireChanging, supportsCanceled, supportsHandled);
            int changedCounter = 0;
            obj.ValueChanged += (s, e) =>
            {
                Assert.Same(obj, s);
                Assert.Equal(0, e.OldValue);
                Assert.Equal(1, e.NewValue);
                Assert.Equal(e.NewValue, obj.Value);
                changedCounter++;
            };

            obj.Value = 1;

            Assert.Equal(1, changedCounter);
        }

        [Theory]
        [MemberDataTuple(nameof(ChangingTestData))]
        public void ChangingFires(bool supportsCanceled, bool supportsHandled)
        {
            var obj = new ValueChangeEventsMock(true, supportsCanceled, supportsHandled);
            int changingCounter = 0;
            int changedCounter = 0;
            obj.ValueChanging += (s, e) =>
            {
                Assert.Equal(0, changingCounter);
                Assert.Equal(0, changedCounter);
                Assert.Same(obj, s);
                Assert.Equal(0, e.OldValue);
                Assert.Equal(1, e.NewValue);
                Assert.Equal(e.OldValue, obj.Value);
                changingCounter++;
            };
            obj.ValueChanged += (s, e) =>
            {
                Assert.Equal(1, changingCounter);
                Assert.Equal(0, changedCounter);
                Assert.Same(obj, s);
                Assert.Equal(0, e.OldValue);
                Assert.Equal(1, e.NewValue);
                Assert.Equal(e.NewValue, obj.Value);
                changedCounter++;
            };

            obj.Value = 1;

            Assert.Equal(1, changingCounter);
            Assert.Equal(1, changedCounter);
        }

        [Theory]
        [MemberDataEnumerable(nameof(BooleanValues))]
        public void TestCancelled(bool supportsHandled)
        {
            var obj = new ValueChangeEventsMock(true, true, supportsHandled);

            int changingCounter = 0;
            obj.ValueChanging += (s, e) =>
            {
                changingCounter += 1;
            };
            obj.ValueChanging += (s, e) =>
            {
                changingCounter += 2;
                e.IsCancelled = true;
            };
            // Should never execute due to cancellation
            obj.ValueChanging += (s, e) =>
            {
                changingCounter = 5;
            };

            obj.ValueChanged += (s, e)
                => Assert.Fail("Changed event should not be executed when event is cancelled during changing event.");

            obj.Value = 1;

            Assert.Equal(3, changingCounter);
            Assert.Equal(0, obj.Value);
        }

        [Theory]
        [MemberDataEnumerable(nameof(BooleanValues))]
        public void CancelledUnsupported(bool supportsHandled)
        {
            var obj = new ValueChangeEventsMock(true, false, supportsHandled);

            int changingCounter = 0;
            obj.ValueChanging += (s, e) =>
            {
                changingCounter += 1;
            };
            obj.ValueChanging += (s, e) =>
            {
                changingCounter += 2;
                e.IsCancelled = true; // Should do nothing since cancellation isn't supported
            };
            obj.ValueChanging += (s, e) =>
            {
                changingCounter = 5;
            };

            int changedCounter = 0;
            obj.ValueChanged += (s, e)
                => changedCounter++;

            obj.Value = 1;

            Assert.Equal(5, changingCounter);
            Assert.Equal(1, changedCounter);
            Assert.Equal(1, obj.Value);
        }

        [Theory]
        [MemberDataTuple(nameof(ValidChangingStates))]
        public void TestHandled(bool fireChanging, bool supportsCanceled)
        {
            var obj = new ValueChangeEventsMock(fireChanging, supportsCanceled, true);

            int changedCounter = 0;
            obj.ValueChanged += (s, e) =>
            {
                changedCounter += 1;
            };
            obj.ValueChanged += (s, e) =>
            {
                changedCounter += 2;
                e.IsHandled = true;
            };
            // Should never execute due to the previous handler marking IsHandled
            obj.ValueChanged += (s, e) =>
            {
                changedCounter = 5;
            };

            obj.Value = 1;

            Assert.Equal(3, changedCounter);
            Assert.Equal(1, obj.Value);
        }

        [Theory]
        [MemberDataTuple(nameof(ValidChangingStates))]
        public void HandledUnsupported(bool fireChanging, bool supportsCanceled)
        {
            var obj = new ValueChangeEventsMock(fireChanging, supportsCanceled, false);

            int changedCounter = 0;
            obj.ValueChanged += (s, e) =>
            {
                changedCounter += 1;
            };
            obj.ValueChanged += (s, e) =>
            {
                changedCounter += 2;
                e.IsHandled = true;  // Should do nothing since handling isn't supported
            };
            obj.ValueChanged += (s, e) =>
            {
                changedCounter = 5;
            };

            obj.Value = 1;

            Assert.Equal(5, changedCounter);
            Assert.Equal(1, obj.Value);
        }
    }
}
