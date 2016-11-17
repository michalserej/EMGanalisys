using System;
using System.Collections.Generic;
using System.IO;

namespace C3D
{
    public sealed class C3DFile
    {
        private C3DProcessorType _processorType;
        private C3DHeader _header;
        private C3DParameterDictionary _parameterDictionary;
        private C3DFrameCollection _frameCollection;

        public C3DProcessorType CreateProcessorType
        {
            get { return this._processorType; }
        }

        public C3DHeader Header
        {
            get { return this._header; }
        }

        public C3DParameterDictionary Parameters
        {
            get { return this._parameterDictionary; }
            set { this._parameterDictionary = value; }
        }

        public C3DFrameCollection AllFrames
        {
            get { return this._frameCollection; }
            set { this._frameCollection = value; }
        }

        private C3DFile()
        {
            this._processorType = C3DConstants.FILE_DEFAULT_PROCESSOR_TYPE;
            this._header = new C3DHeader();
            this._parameterDictionary = C3DParameterDictionary.CreateNewParameterDictionary();
            this._frameCollection = new C3DFrameCollection();
        }

        private C3DFile(Stream stream)
        {
            C3DReader reader = null;

            try
            {
                reader = new C3DReader(stream);

                this._processorType = reader.CreateProcessorType;
                this._header = reader.ReadHeader();
                this._parameterDictionary = reader.ReadParameters();
                this._frameCollection = new C3DFrameCollection();

                try
                {
                    C3DParameterCache paramCache = C3DParameterCache.CreateCache(this);
                    C3DFrame frame = null;
                    while ((frame = reader.ReadNextFrame(paramCache)) != null)
                    {
                        this._frameCollection.Add(frame);
                    }
                }
                catch { }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        public void SaveTo(String filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                C3DWriter writer = new C3DWriter(fs);

                writer.WriteC3DFile(this);
            }
        }

        public static C3DFile Create()
        {
            return new C3DFile();
        }

        public static C3DFile LoadFromStream(Stream stream)
        {
            return new C3DFile(stream);
        }

        public static C3DFile LoadFromFile(String filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return new C3DFile(fs);
            }
        }
    }
}