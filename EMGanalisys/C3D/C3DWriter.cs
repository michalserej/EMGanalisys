using System;
using System.Collections.Generic;
using System.IO;

using C3D.IO;

namespace C3D
{
    public sealed class C3DWriter : IDisposable
    {
        private C3DBinaryWriter _writer;
        private Int64 _dataStartOffset;
        private UInt16 _newDataStartBlockIndex;

        public C3DWriter(Stream stream)
        {
            this._writer = new C3DBinaryWriter(stream);
        }

        public void WriteC3DFile(C3DFile file)
        {
            this._dataStartOffset = 0;
            this._newDataStartBlockIndex = 0;

            this._writer.Write(new Byte[C3DConstants.FILE_SECTION_SIZE]);
            this.WriteParameters(file.Header.FirstParameterSectionID, file.Parameters);

            C3DParameterCache paramCache = C3DParameterCache.CreateCache(file);
            this.WriteFrameCollection(paramCache, file.AllFrames);

            this.UpdateHeaderAndParameters(file);
            this.WriteHeader(file.Header);
        }

        public void Flush()
        {
            this._writer.Flush();
        }

        public void Close()
        {
            this.Dispose(true);
        }

        private void WriteHeader(C3DHeader header)
        {
            this._writer.Seek(0, SeekOrigin.Begin);
            this._writer.Write(header.ToArray());
        }

        private void WriteParameters(Byte firstParameterBlockIndex, C3DParameterDictionary dictionary)
        {
            Int32 startPosition = (firstParameterBlockIndex - 1) * C3DConstants.FILE_SECTION_SIZE;
            this._writer.Seek(startPosition, SeekOrigin.Begin);
            this._writer.Write(C3DConstants.FILEPARAMETER_FIRST_PARAM_BLOCK);
            this._writer.Write(C3DConstants.FILEPARAMETER_SIGNATURE);
            this._writer.Write((Byte)0);
            this._writer.Write((Byte)C3DConstants.FILE_DEFAULT_PROCESSOR_TYPE);

            foreach (C3DParameterGroup group in dictionary)
            {
                if (group.ID < 0)
                {
                    this.WriteParameter(null, group, false);

                    foreach (C3DParameter param in group)
                    {
                        this.WriteParameter(group, param, false);
                    }
                }
            }

            C3DParameterGroup lastGroup = new C3DParameterGroup(0, "", "");
            this.WriteParameter(null, lastGroup, true);

            this._newDataStartBlockIndex = (UInt16)((this._writer.BaseStream.Position + C3DConstants.FILE_SECTION_SIZE) / C3DConstants.FILE_SECTION_SIZE + 1);
            this._writer.Write(new Byte[(this._newDataStartBlockIndex - 1) * C3DConstants.FILE_SECTION_SIZE - this._writer.BaseStream.Position]);

            this._writer.Seek(startPosition + 2, SeekOrigin.Begin);
            this._writer.Write((Byte)(this._newDataStartBlockIndex - 2));
        }

        private void WriteParameter(C3DParameterGroup parent, AbstractC3DParameter param, Boolean isLast)
        {
            this._writer.Write((SByte)(param.IsLocked ? -param.Name.Length : param.Name.Length));
            this._writer.Write(param.ID);
            this._writer.Write(param.GetNameByteArray());

            Byte[] lastData = (param.ID > 0 ? param.GetLastDataArrayWithoutDescrption() : null);

            Int16 nextPosition = (Int16)(isLast ? 0 :
                (2 + (lastData == null ? 0 : lastData.Length) + 
                1 + (String.IsNullOrEmpty(param.Description) ? 0 : param.Description.Length)));
            this._writer.Write(nextPosition);

            if (parent != null && String.Equals(parent.Name, "POINT", StringComparison.OrdinalIgnoreCase))
            {
                if (String.Equals(param.Name, "DATA_START", StringComparison.OrdinalIgnoreCase))
                {
                    this._dataStartOffset = this._writer.BaseStream.Position;
                }
            }

            if (lastData != null)
            {
                this._writer.Write(lastData);
            }

            if (param.Description == null)
            {
                this._writer.Write((Byte)0);
            }
            else
            {
                this._writer.Write((Byte)param.Description.Length);
                this._writer.Write(param.GetDescriptionByteArray());
            }
        }

        private void WriteFrameCollection(C3DParameterCache cache, C3DFrameCollection frameCollection)
        {
            Int32 startPosition = (this._newDataStartBlockIndex - 1) * C3DConstants.FILE_SECTION_SIZE;
            this._writer.Seek(startPosition, SeekOrigin.Begin);

            for (Int32 i = 0; i < frameCollection.Count; i++)
            {
                if (cache.ScaleFactor < 0)
                {
                    this.WriteFloatFrame(cache, frameCollection[i]);
                }
                else
                {
                    this.WriteIntFrame(cache, frameCollection[i]);
                }
            }

            Int16 finalIndex = (Int16)((this._writer.BaseStream.Position + C3DConstants.FILE_SECTION_SIZE) / C3DConstants.FILE_SECTION_SIZE + 1);
            this._writer.Write(new Byte[(finalIndex - 1) * C3DConstants.FILE_SECTION_SIZE - this._writer.BaseStream.Position]);
        }

        private void WriteFloatFrame(C3DParameterCache cache, C3DFrame frame)
        {
            if (frame.Point3Ds != null)
            {
                for (Int32 i = 0; i < frame.Point3Ds.Length; i++)
                {
                    this._writer.Write(frame.Point3Ds[i].X);
                    this._writer.Write(frame.Point3Ds[i].Y);
                    this._writer.Write(frame.Point3Ds[i].Z);
                    this._writer.Write(frame.Point3Ds[i].GetFloatLastPart(cache.ScaleFactor));
                }
            }

            if (frame.AnalogSamples != null)
            {
                for (Int32 j = 0; j < cache.AnalogSamplesPerFrame; j++)
                {
                    for (Int32 i = 0; i < cache.AnalogChannelCount; i++)
                    {
                        Single data = frame.AnalogSamples[i][j] / cache.AnalogGeneralScale / (cache.AnalogChannelScale != null && cache.AnalogChannelScale.Length > 0 ? cache.AnalogChannelScale[i] : 1.0F)
                             + ((cache.AnalogZeroOffset != null && cache.AnalogZeroOffset.Length > 0) ? cache.AnalogZeroOffset[i] : (Int16)0);

                        this._writer.Write(data);
                    }
                }
            }
        }

        private void WriteIntFrame(C3DParameterCache cache, C3DFrame frame)
        {
            if (frame.Point3Ds != null)
            {
                for (Int32 i = 0; i < frame.Point3Ds.Length; i++)
                {
                    this._writer.Write((Int16)Math.Round(frame.Point3Ds[i].X / cache.ScaleFactor, MidpointRounding.AwayFromZero));
                    this._writer.Write((Int16)Math.Round(frame.Point3Ds[i].Y / cache.ScaleFactor, MidpointRounding.AwayFromZero));
                    this._writer.Write((Int16)Math.Round(frame.Point3Ds[i].Z / cache.ScaleFactor, MidpointRounding.AwayFromZero));
                    this._writer.Write(frame.Point3Ds[i].GetIntLastPart(cache.ScaleFactor));
                }
            }

            if (frame.AnalogSamples != null)
            {
                for (Int32 j = 0; j < cache.AnalogSamplesPerFrame; j++)
                {
                    for (Int32 i = 0; i < cache.AnalogChannelCount; i++)
                    {
                        Single data = frame.AnalogSamples[i][j] / cache.AnalogGeneralScale / (cache.AnalogChannelScale != null && cache.AnalogChannelScale.Length > 0 ? cache.AnalogChannelScale[i] : 1.0F)
                             + ((cache.AnalogZeroOffset != null && cache.AnalogZeroOffset.Length > 0) ? cache.AnalogZeroOffset[i] : (Int16)0);

                        this._writer.Write((Int16)Math.Round(data, MidpointRounding.AwayFromZero));
                    }
                }
            }
        }

        private void UpdateHeaderAndParameters(C3DFile file)
        {
            file.Header.FirstDataSectionID = this._newDataStartBlockIndex;
            C3DParameter dataStart = file.Parameters["POINT", "DATA_START"];
            if (dataStart != null)
            {
                dataStart.InternalSetData<UInt16>(this._newDataStartBlockIndex);

                this._writer.Seek(this._dataStartOffset, SeekOrigin.Begin);
                this._writer.Write(dataStart.GetLastDataArrayWithoutDescrption());
            }
        }

        private void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (this._writer != null)
                {
                    this._writer.Close();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}