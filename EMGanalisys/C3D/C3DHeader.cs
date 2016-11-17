using System;
using System.Text;

using C3D.Numerics;

namespace C3D
{
    public sealed class C3DHeader
    {
        private Byte[] _data;

        private Byte Signature
        {
            set { this._data[1] = value; }
        }
        
        public UInt16 PointCount
        {
            get { return (UInt16)this.GetInt16Record(2); }
            set { this.SetInt16Record(2, (Int16)value); }
        }

        public UInt16 AnalogMeasurementCount
        {
            get { return (UInt16)this.GetInt16Record(3); }
            set { this.SetInt16Record(3, (Int16)value); }
        }

        public UInt16 FirstFrameIndex
        {
            get { return (UInt16)this.GetInt16Record(4); }
            set { this.SetInt16Record(4, (Int16)value); }
        }

        public UInt16 LastFrameIndex
        {
            get { return (UInt16)this.GetInt16Record(5); }
            set { this.SetInt16Record(5, (Int16)value); }
        }

        public Int16 MaxInterpolationGaps
        {
            get { return this.GetInt16Record(6); }
            set { this.SetInt16Record(6, value); }
        }

        public Single ScaleFactor
        {
            get { return this.GetSingleRecord(7); }
            set { this.SetSingleRecord(7, value); }
        }

        public UInt16 AnalogSamplesPerFrame
        {
            get { return (UInt16)this.GetInt16Record(10); }
            set { this.SetInt16Record(10, (Int16)value); }
        }

        public Single FrameRate
        {
            get { return this.GetSingleRecord(11); }
            set { this.SetSingleRecord(11, value); }
        }

        public Boolean HasLableRangeData
        {
            get { return this.GetInt16Record(148) == C3DConstants.FILEHEADER_KEY_VALUE; }
            set { this.SetInt16Record(148, (value ? C3DConstants.FILEHEADER_KEY_VALUE : (Int16)0)); }
        }

        public Boolean IsSupport4CharsLabel
        {
            get { return this.GetInt16Record(150) == C3DConstants.FILEHEADER_KEY_VALUE; }
            set { this.SetInt16Record(150, (value ? C3DConstants.FILEHEADER_KEY_VALUE : (Int16)0)); }
        }

        public Int16 HeaderEventsCount
        {
            get { return this.GetInt16Record(151); }
            private set { this.SetInt16Record(151, value); }
        }

        internal Byte FirstParameterSectionID
        {
            get { return this._data[0]; }
            private set { this._data[0] = value; }
        }

        internal UInt16 FirstDataSectionID
        {
            get { return (UInt16)this.GetInt16Record(9); }
            set { this.SetInt16Record(9, (Int16)value); }
        }

        internal UInt16 FirstLabelRangeSectionID
        {
            get { return (UInt16)this.GetInt16Record(149); }
            set { this.SetInt16Record(149, (Int16)value); }
        }

        internal C3DHeader(C3DProcessorType processorType, Byte[] rawData)
        {
            this._data = rawData;

            this.UpdateHeaderContent(processorType);
        }

        internal C3DHeader()
        {
            this._data = new Byte[C3DConstants.FILE_SECTION_SIZE];

            this.Signature = C3DConstants.FILE_SIGNATURE;
            this.FirstParameterSectionID = C3DConstants.FILE_DEFAULT_FIRST_PARAM_SECTION;
            this.PointCount = C3DConstants.DEFAULT_POINT_USED;
            this.AnalogMeasurementCount = (UInt16)(C3DConstants.DEFAULT_POINT_RATE * C3DConstants.DEFAULT_ANALOG_RATE * C3DConstants.DEFAULT_ANALOG_USED);
            this.FirstFrameIndex = C3DConstants.DEFAULT_POINT_FIRST_FRAME;
            this.LastFrameIndex = C3DConstants.DEFAULT_POINT_LAST_FRAME;
            this.ScaleFactor = C3DConstants.DEFAULT_POINT_SCALE;
            this.AnalogSamplesPerFrame = (UInt16)(C3DConstants.DEFAULT_ANALOG_RATE / C3DConstants.DEFAULT_POINT_RATE);
            this.FrameRate = C3DConstants.DEFAULT_POINT_RATE;
            this.IsSupport4CharsLabel = true;
        }

        public C3DHeaderEvent[] GetAllHeaderEvents()
        {
            Int16 count = this.HeaderEventsCount;

            if (count == 0)
            {
                return null;
            }
            else if (count > C3DConstants.FILEHEADER_MAX_EVENTS_COUNT)
            {
                count = C3DConstants.FILEHEADER_MAX_EVENTS_COUNT;
            }

            C3DHeaderEvent[] array = new C3DHeaderEvent[count];

            for (Int16 i = 0; i < count; i++)
            {
                array[i] = new C3DHeaderEvent(
                    Encoding.ASCII.GetString(this._data, 396 + i * 4, 4),
                    this.GetSingleRecord((Int16)(153 + i * 2)),
                    this._data[376 + i] == 1);
            }

            return array;
        }

        public void SetAllHeaderEvents(C3DHeaderEvent[] events)
        {
            Int16 count = (Int16)(events != null ? events.Length : 0);

            if (count > C3DConstants.FILEHEADER_MAX_EVENTS_COUNT)
            {
                throw new ArgumentOutOfRangeException("Header events is too much.");
            }

            for (Int16 i = 0; i < count; i++)
            {
                if (!String.IsNullOrEmpty(events[i].EventName) && events[i].EventName.Length > 4)
                {
                    throw new ArgumentOutOfRangeException("Event name is too long.");
                }
            }

            Byte[] empty = new Byte[4] { 0x20, 0x20, 0x20, 0x20 };
            for (Int16 i = 0; i < count; i++)
            {
                this.SetSingleRecord((Int16)(153 + i * 2), events[i].EventTime);
                this._data[376 + i] = (Byte)(events[i].IsDisplay ? 1 : 0);

                Array.Copy(empty, 0, this._data, 396 + i * 4, empty.Length);

                if (!String.IsNullOrEmpty(events[i].EventName))
                {
                    Byte[] nameData = Encoding.ASCII.GetBytes(events[i].EventName);
                    Array.Copy(nameData, 0, this._data, 396 + i * 4, nameData.Length);
                }
            }

            this.HeaderEventsCount = count;
        }

        internal Byte[] ToArray()
        {
            return this._data;
        }

        private Int16 GetInt16Record(Int16 index)
        {
            return C3DBitConverter.ToInt16(this._data, (index - 1) * 2);
        }

        private void SetInt16Record(Int16 index, Int16 value)
        {
            Array.Copy(C3DBitConverter.GetBytes(value), 0, this._data, (index - 1) * 2, sizeof(Int16));
        }

        private Single GetSingleRecord(Int16 index)
        {
            return C3DBitConverter.ToSingle(this._data, (index - 1) * 2);
        }

        private void SetSingleRecord(Int16 index, Single value)
        {
            Array.Copy(C3DBitConverter.GetBytes(value), 0, this._data, (index - 1) * 2, sizeof(Single));
        }

        private void UpdateInt16Record(C3DProcessorType oldType, Int16 index)
        {
            Int16 value = C3DBitConverter.ToInt16(oldType, this._data, (index - 1) * 2);
            this.SetInt16Record(index, value);
        }

        private void UpdateSingleRecord(C3DProcessorType oldType, Int16 index)
        {
            Single value = C3DBitConverter.ToSingle(oldType, this._data, (index - 1) * 2);
            this.SetSingleRecord(index, value);
        }

        private void UpdateHeaderEventTimes(C3DProcessorType oldType)
        {
            Int16 count = this.HeaderEventsCount;

            for (Int16 index = 153; index < 153 + count * 2; index += 2)
            {
                this.UpdateSingleRecord(oldType, index);
            }
        }

        private void UpdateHeaderContent(C3DProcessorType processorType)
        {
            this.UpdateInt16Record(processorType, 2);
            this.UpdateInt16Record(processorType, 3);
            this.UpdateInt16Record(processorType, 4);
            this.UpdateInt16Record(processorType, 5);
            this.UpdateInt16Record(processorType, 6);
            this.UpdateSingleRecord(processorType, 7);
            this.UpdateInt16Record(processorType, 9);
            this.UpdateInt16Record(processorType, 10);
            this.UpdateSingleRecord(processorType, 11);
            this.UpdateInt16Record(processorType, 148);
            this.UpdateInt16Record(processorType, 149);
            this.UpdateInt16Record(processorType, 150);
            this.UpdateInt16Record(processorType, 151);
            this.UpdateHeaderEventTimes(processorType);
        }
    }
}