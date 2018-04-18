using GeoAPI.Geometries;
using System;

namespace GeoAPI.Benchmarks
{
    public class DummyGeometryServices : IGeometryServices
    {
        public int DefaultSRID => throw new NotImplementedException();

        public ICoordinateSequenceFactory DefaultCoordinateSequenceFactory => throw new NotImplementedException();

        public IPrecisionModel DefaultPrecisionModel => throw new NotImplementedException();

        public IGeometryFactory CreateGeometryFactory() => throw new NotImplementedException();

        public IGeometryFactory CreateGeometryFactory(int srid) => throw new NotImplementedException();

        public IGeometryFactory CreateGeometryFactory(ICoordinateSequenceFactory coordinateSequenceFactory) => throw new NotImplementedException();

        public IGeometryFactory CreateGeometryFactory(IPrecisionModel precisionModel) => throw new NotImplementedException();

        public IGeometryFactory CreateGeometryFactory(IPrecisionModel precisionModel, int srid) => throw new NotImplementedException();

        public IGeometryFactory CreateGeometryFactory(IPrecisionModel precisionModel, int srid, ICoordinateSequenceFactory coordinateSequenceFactory) => throw new NotImplementedException();

        public IPrecisionModel CreatePrecisionModel(PrecisionModels modelType) => throw new NotImplementedException();

        public IPrecisionModel CreatePrecisionModel(IPrecisionModel modelType) => throw new NotImplementedException();

        public IPrecisionModel CreatePrecisionModel(double scale) => throw new NotImplementedException();

        public void ReadConfiguration() => throw new NotImplementedException();

        public void WriteConfiguration() => throw new NotImplementedException();
    }
}
