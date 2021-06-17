namespace Comet.Network.Packets.Internal
{
    public abstract class MsgAccServerGameInformation<T> : MsgBase<T>
    {
        public int Status { get; set; }        
        public int PlayerCount { get; set; }
        public int PlayerCountRecord { get; set; }
        public int PlayerLimit { get; set; }

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new (bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new ();
            writer.Write((ushort)PacketType.MsgAccServerInformation);
            return writer.ToArray();
        }
    }
}
