using System;
using Memory;

namespace kps_counter
{
    /// <summary>
    /// Interface for reading memory values from DOSBox
    /// </summary>
    class WingCommanderMemoryReader
    {
        // used for interfacing with DOSBox
        private Mem m;
        private int pid;

        /// <summary>
        /// true if successfully hooked to the DOSBox process
        /// </summary>
        public bool OpenProc { get; private set; } = false;

        /// <summary>
        /// base memory address for DOSBox RAM
        /// </summary>
        private const string memoryBase = "0x01D3A1A0";

        //private const string callSignOffset = "0x1E040"; // WC1
        /// <summary>
        /// memory offset for player callsign
        /// </summary>
        private const string callSignOffset = "0x27C9B"; // WC2
        /// <summary>
        /// memory offset for wingman callsign
        /// </summary>
        private const string wingmanCallsignOffset = "0x2AC86"; // WC2
        /// <summary>
        /// memory offset for wingman kills
        /// </summary>
        private const string wingmanKillsOffsetWC2 = "0x302d2"; // WC2
        /// <summary>
        /// memory offset for player name
        /// </summary>
        private const string playerNameOffset = "0x1E032";
        /// <summary>
        /// memory offset for player's sortie count
        /// </summary>
        private const string sortiesOffset = "0x1E052";
        /// <summary>
        /// memory offset for player's board kills
        /// </summary>
        private const string boardKillsOffset = "0x1E054";
        /// <summary>
        /// memory offset for player's in-mission kills
        /// </summary>
        //private const string currentKillsOffset = "0x1E402"; // WC1
        private const string currentKillsOffset = "0x302C8"; // WC2
        /// <summary>
        /// memory offset for player's rank value
        /// </summary>
        private const string rankOffset = "0x1E050";
        /// <summary>
        /// memory offset for player's ship kps
        /// </summary>
        //private const string setKpsOffset = "0x1FFE7";
        private const string setKpsOffset = "0x295FF";
        /// <summary>
        /// memory offset for player's remaining afterburner fuel
        /// </summary>
        private const string remainingFuelOffset = "0x2045C"; //WC1

        /// <summary>
        /// used to properly calcuate number of player kills, set when we enter Halcyon's debriefing and cleared at mission start
        /// </summary>
        public bool DebriefMode = false;

        /// <summary>
        /// instantiates a new MemoryReader
        /// </summary>
        public WingCommanderMemoryReader()
        {
            m = new Mem();

            SetPID();
        }

        /// <summary>
        /// sets and returns the process id of DOSBox if available
        /// </summary>
        /// <returns></returns>
        public int SetPID()
        {
            pid = m.getProcIDFromName("DOSBox");
            return pid;
        }

        /// <summary>
        /// attaches the memory reader interface to DOSBox throws DOSBoxMemoryException on error
        /// </summary>
        public void hook()
        {
            // refresh PID just to be safe
            SetPID();
            if (pid == 0) throw new DOSBoxMemoryException("getProcIDFromName returned 0. Are you sure DOSBox.exe is running?");
            OpenProc = m.OpenProcess(pid);
            if (OpenProc == false) throw new DOSBoxMemoryException("OpenProcess returned false. Are administrative priveleges acquried?");
        }

        /// <summary>
        /// fetches set kps value from DOSBox memory
        /// </summary>
        /// <returns>set kps</returns>
        public int GetSetKPS()
        {
            if (OpenProc)
            {
                string addr = memoryBase + "," + setKpsOffset;
                return m.readByte(addr) * 10;
            }
            throw new DOSBoxMemoryException("Must hook to DOSBox process before reading memory.");
        }

        /// <summary>
        /// fetches player name from DOSBox memory
        /// </summary>
        /// <returns>player name</returns>
        public string GetPlayerName()
        {
            if (OpenProc)
            {
                string addr = memoryBase + "," + playerNameOffset;
                return m.readString(addr, length: 14);
            }
            throw new DOSBoxMemoryException("Must hook to DOSBox process before reading memory.");
        }

        /// <summary>
        /// fetches player callsign from DOSBox memory
        /// </summary>
        /// <returns>player callsign</returns>
        public string GetPlayerCallsign()
        {
            if (OpenProc)
            {
                string addr = memoryBase + "," + callSignOffset;
                return m.readString(addr, length: 10);
            }
            throw new DOSBoxMemoryException("Must hook to DOSBox process before reading memory.");
        }

        /// <summary>
        /// fetches wingman callsign from DOSBox memory
        /// </summary>
        /// <returns>wingman callsign</returns>
        public string GetWingmanCallsign()
        {
            if (OpenProc)
            {
                string addr = memoryBase + "," + wingmanCallsignOffset;
                return m.readString(addr, length: 10);
            }
            throw new DOSBoxMemoryException("Must hook to DOSBox process before reading memory.");
        }

        /// <summary>
        /// fetches player sortie count from DOSBox memory
        /// </summary>
        /// <returns>player sortie count</returns>
        public int GetSorties()
        {
            if (OpenProc)
            {
                string addr = memoryBase + "," + sortiesOffset;
                return m.read2Byte(addr);
            }
            throw new DOSBoxMemoryException("Must hook to DOSBox process before reading memory.");
        }

        /// <summary>
        /// fetches player killboard kills from DOSBox memory
        /// </summary>
        /// <returns>player killboard kill count</returns>
        public int GetBoardKills()
        {
            if (OpenProc)
            {
                string addr = memoryBase + "," + boardKillsOffset;
                return m.read2Byte(addr);
            }
            throw new DOSBoxMemoryException("Must hook to DOSBox process before reading memory.");
        }

        /// <summary>
        /// fetches wingman in-mission kills from DOSBox memory
        /// </summary>
        /// <returns>wingman in-mission kills</returns>
        public int GetWingmanKills()
        {
            if (OpenProc)
            {
                string addr = memoryBase + "," + wingmanKillsOffsetWC2;
                return m.readByte(addr);
            }
            throw new DOSBoxMemoryException("Must hook to DOSBox process before reading memory.");
        }


        /// <summary>
        /// fetches player in-mission kills from DOSBox memory
        /// </summary>
        /// <returns>player in-mission kill count</returns>
        public int GetCurrentKills()
        {
            if (OpenProc)
            {
                string addr = memoryBase + "," + currentKillsOffset;
                return m.read2Byte(addr);
            }
            throw new DOSBoxMemoryException("Must hook to DOSBox process before reading memory.");
        }

        /// <summary>
        /// fetches player rank value from DOSBox memory
        /// </summary>
        /// <returns>player rank value</returns>
        public int GetRank()
        {
            if (OpenProc)
            {
                string addr = memoryBase + "," + rankOffset;
                return m.read2Byte(addr);
            }
            throw new DOSBoxMemoryException("Must hook to DOSBox process before reading memory.");
        }
    }


    [Serializable]
    public class DOSBoxMemoryException : Exception
    {
        public DOSBoxMemoryException() { }
        public DOSBoxMemoryException(string message) : base(message) { }
        public DOSBoxMemoryException(string message, Exception inner) : base(message, inner) { }
        protected DOSBoxMemoryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
