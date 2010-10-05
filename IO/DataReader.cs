using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using Modbus.Device;
using Modbus.Utility;

namespace SharpMonitor.IO
{
    public class DataReader
    {
        private readonly IModbusSerialMaster master;

        private byte slaveID = SharpMonitor.Properties.Settings.Default.DeviceId;
        private ushort startAddress = 1031;
        private ushort numRegisters = 73;

        public DataReader(SerialPort port)
        {
            master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = 250;
        }

        public DataPacket Read()
        {
            ushort[] data = master.ReadInputRegisters(slaveID, startAddress, numRegisters);

            var t = typeof(DataPacket);
            DataPacket r = Activator.CreateInstance(t) as DataPacket;

            foreach (var p in t.GetProperties())
            {
                if (p.CanWrite)
                {
                    DataBlockAttribute attr = p.GetCustomAttributes(typeof(DataBlockAttribute), false).First() as DataBlockAttribute;

                    var value = attr.Is32BitValue ?
                        ConvertFrom(ModbusUtility.GetUInt32(data[attr.StartIndex], data[attr.StartIndex + 1]), p.PropertyType) :
                        ConvertFrom(data[attr.StartIndex], p.PropertyType);

                    if (value is double)
                    {
                        value = (p.Name.Contains("Voltage")) ? (double)value / 10 : (double)value / 100;
                    }

                    p.SetValue(r, value, null);
                }
            }

            return r;
        }

        protected object ConvertFrom(object value, Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var nc = new NullableConverter(t);
                return nc.ConvertFrom(value);
            }
            return Convert.ChangeType(value, t);
        }
    }
}
