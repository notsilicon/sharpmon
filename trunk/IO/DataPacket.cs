namespace SharpMonitor.IO
{
    public class DataPacket
    {
        [DataBlock(25)]
        public double Voltage { get; set; }

        [DataBlock(26)]
        public double Current { get; set; }

        [DataBlock(27)]
        public uint Power { get; set; }

        [DataBlock(28)]
        public double Frequency { get; set; }

        [DataBlock(30)]
        public uint VoltageRedundant { get; set; }

        [DataBlock(31)]
        public double FrequencyRedundant { get; set; }

        [DataBlock(37)]
        public int Online { get; set; }

        [DataBlock(40, true)]
        public uint TodayWh { get; set; }

        [DataBlock(42, true)]
        public uint MinutesToday { get; set; }

        [DataBlock(44, true)]
        public double LifetimeWh { get; set; }

        [DataBlock(46, true)]
        public double LifeRuntime { get; set; }

        [DataBlock(48)]
        public ushort ChokeTemp { get; set; }

        [DataBlock(49)]
        public ushort D2DHeatsinkTemp { get; set; }

        [DataBlock(50)]
        public ushort InverterHeatsinkTemp { get; set; }

        [DataBlock(51)]
        public ushort AmbientTemp { get; set; }

        [DataBlock(56)]
        public ushort ACFlag { get; set; }
    }
}
