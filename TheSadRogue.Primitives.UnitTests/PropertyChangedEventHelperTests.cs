using System;
using System.Collections.Generic;
using Xunit;
using XUnit.ValueTuples;

namespace SadRogue.Primitives.UnitTests
{
    public class ValueChangeEventsMock
    {
        private readonly bool _fireChanging;

        private int _value;

        public int Value
        {
            get => _value;
            set
            {
                if (_fireChanging)
                    this.SafelySetProperty(ref _value, value, ValueChanging, ValueChanged);
                else
                    this.SafelySetProperty(ref _value, value, ValueChanged);
            }
        }

        public event EventHandler<ValueChangedEventArgs<int>>? ValueChanging;
        public event EventHandler<ValueChangedEventArgs<int>>? ValueChanged;

        public ValueChangeEventsMock(bool fireChanging)
        {
            _fireChanging = fireChanging;
        }
    }

    public class ValueChangeEventsMockNullableRef
    {
        private readonly bool _fireChanging;

        private string? _value;

        public string? Value
        {
            get => _value;
            set
            {
                if (_fireChanging)
                    this.SafelySetProperty(ref _value, value, ValueChanging, ValueChanged);
                else
                    this.SafelySetProperty(ref _value, value, ValueChanged);
            }
        }

        public event EventHandler<ValueChangedEventArgs<string?>>? ValueChanging;
        public event EventHandler<ValueChangedEventArgs<string?>>? ValueChanged;

        public ValueChangeEventsMockNullableRef(bool fireChanging)
        {
            _fireChanging = fireChanging;
        }
    }

    public class PropertyChangedEventHelperTests
    {
        #region Test Data
        public static IEnumerable<bool> BooleanValues => new[] {true, false };

        #endregion


        [Theory]
        [MemberDataEnumerable(nameof(BooleanValues))]
        public void ChangedFires(bool fireChanging)
        {
            var obj = new ValueChangeEventsMock(fireChanging);
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

        [Fact]
        public void ChangingFires()
        {
            var obj = new ValueChangeEventsMock(true);
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
        public void EventsDontFireOnSameValue(bool fireChanging)
        {
            var obj = new ValueChangeEventsMockNullableRef(fireChanging);
            int changingCounter = 0;
            obj.ValueChanging += (s, e) =>
            {
                // ReSharper disable once AccessToModifiedClosure
                changingCounter++;
            };
            int changedCounter = 0;
            obj.ValueChanged += (s, e) =>
            {
                // ReSharper disable once AccessToModifiedClosure
                changedCounter++;
            };

            obj.Value = null;
            Assert.Equal(0, changingCounter);
            Assert.Equal(0, changedCounter);

            obj.Value = "hi";
            changedCounter = 0;
            changingCounter = 0;

            obj.Value = "hi";
            Assert.Equal(0, changingCounter);
            Assert.Equal(0, changedCounter);
        }

        [Theory]
        [MemberDataEnumerable(nameof(BooleanValues))]
        public void InvalidOperationExceptionReverts(bool fireChanging)
        {
            var obj = new ValueChangeEventsMock(fireChanging);

            obj.ValueChanged += (s, e) => throw new InvalidOperationException();

            Assert.Throws<InvalidOperationException>(() => obj.Value = 1);
            Assert.Equal(0, obj.Value);
        }
    }
}
